using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using CSLibrary;
using CSLibrary.Constants;
using CSLibrary.Events;
using CSLibrary.Structures;
using FestivalContainer.Data;

namespace CS203Engine
{
    /// <summary>
    ///     A static collection of all connections to RFID devices.
    ///     This is necessary because a ConnectionInterface cannot connect and disconnect twice.
    /// </summary>
    public class RfidScannerFacade
    {
        /// <summary>
        ///     The public list of all ConnectionInterfaces
        /// </summary>
        public static List<ConnectionInterface> connectionInterfaces = new List<ConnectionInterface>();

        /// <summary>
        ///     Method to get an interface or create a new all depending on if it already exists in the list
        /// </summary>
        /// <param name="ip">The ip of the device</param>
        /// <returns>ConnectionInterface for the defined ip</returns>
        public static ConnectionInterface getConnectionInterface(string ip)
        {
            if (ip == null) return null;
            var conInt = connectionInterfaces.Where(o => o.IP == ip).FirstOrDefault();
            if (conInt == null)
            {
                var newInterface = new ConnectionInterface();
                newInterface.IP = ip;
                newInterface.connect();
                connectionInterfaces.Add(newInterface);
                return newInterface;
            }
            return conInt;
        }
    }

    /// <summary>
    ///     Represents an antenna for a ConnectionInterface
    /// </summary>
    public class Antenna
    {
        private readonly ConnectionInterface connectionInterface;

        public Antenna(ConnectionInterface _connectionInterface, int _index)
        {
            connectionInterface = _connectionInterface;
            port = _index;
        }

        /// <summary>
        ///     The port number of the current antenna
        /// </summary>
        public int port { get; } = -1;

        /// <summary>
        ///     The state of the current antenna
        /// </summary>
        public bool enabled
        {
            get { return connectionInterface.getEnable(port); }
            set { connectionInterface.setEnable(port, value); }
        }

        /// <summary>
        ///     The current power voltage of the antenna.
        ///     power = dBm / 10
        /// </summary>
        public uint power
        {
            get { return connectionInterface.getPower(port); }
            set { connectionInterface.setPower(port, value); }
        }
    }

    /// <summary>
    ///     Exception class for CS203Engine
    /// </summary>
    public class CS203EngineException : Exception
    {
        public CS203EngineException()
        {
        }

        public CS203EngineException(string message) : base(message)
        {
        }

        public CS203EngineException(string message, Exception inner) : base(message, inner)
        {
        }

        protected CS203EngineException(SerializationInfo info,
            StreamingContext context)
        {
            //
        }
    }

    /// <summary>
    ///     Represents a connection to a RFID device
    /// </summary>
    public class ConnectionInterface : IDisposable
    {
        /// <summary>
        ///     Delegate TagReadHandle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void TagReadHandle(object sender, TagReadEventArgs e);

        /// <summary>
        ///     IP-address of RFID device
        /// </summary>
        public string IP = "";

        public int Power = 200;

        public HighLevelInterface reader;

        public List<string> personList { get; set; }

        public bool UseInScanWait = true;


        /// <summary>
        ///     Maximum timeout for connection
        /// </summary>
        public uint timeout = 20000;

        /// <summary>
        ///     Create a ConnectionInterface
        /// </summary>
        /// <param name="_IP">IP address to RFID device</param>
        public ConnectionInterface(string _IP)
        {
            IP = _IP;
            //reader = new HighLevelInterface();
        }

        /// <summary>
        ///     Create a ConnectionInterface
        /// </summary>
        /// <param name="_IP">IP address to RFID device</param>
        /// <param name="_timeout">Maximum timeout for connection</param>
        public ConnectionInterface(string _IP, uint _timeout)
        {
            IP = _IP;
            timeout = _timeout;
        }

        /// <summary>
        ///     Create a ConnectionInterface
        /// </summary>
        public ConnectionInterface()
        {
        }

        /// <summary>
        ///     Represents if the ConnectionInterface is connected
        /// </summary>
        public bool Connected { get; private set; }

        /// <summary>
        ///     Represents all the Antennas in an array
        /// </summary>
        public Antenna[] antennas
        {
            get
            {
                var rtn = new Antenna[reader.AntennaList.Count];
                for (var i = 0; i < rtn.Length; i++)
                {
                    rtn[i] = getAntenna(i);
                }
                return rtn;
            }
        }

        /// <summary>
        ///     Count of all antennas
        /// </summary>
        public int antennaCount
        {
            get { return reader.AntennaList.Count; }
            set { ; }
        }

        /// <summary>
        ///     Gets if the ConnectionInterface is listening (scanning)
        /// </summary>
        public bool IsListening { get; private set; }

        /// <summary>
        ///     Dispose the ConnectionInterface
        /// </summary>
        public void Dispose()
        {
            try
            {
                disconnect();
                reader.Dispose();
                reader = null;
            }
            catch (ObjectDisposedException ex)
            {
                // nothing
            }
        }

        /// <summary>
        ///     Connects to the RFID device
        ///     A ConnectionInterface can only be connected once in a program
        /// </summary>
        /// <returns>If connection was succeeded</returns>
        public bool connect()
        {
            if (Connected) return true;
            reader = new HighLevelInterface();
            var result = new Result();
            if ((result = reader.Connect(IP, timeout)) == Result.OK)
            {
                LoadConfig();

                foreach (Antenna an in antennas)
                {
                    an.enabled = true;
                    an.power = (uint) this.Power;
                }

                var power = string.Join(",", antennas.Select(i => i.power).ToArray());
                var state = string.Join(",", antennas.Select(i => i.enabled).ToArray());
                var debug = "\n\nLoaded connection interface: " + IP + " antennas: " + antennaCount + " \npowers:" +
                            power + "\nstates:" + state + "\n\n";
                Debug.WriteLine(debug);
                //Console.WriteLine(debug);

                reader.OnAsyncCallback += NativeOnTagReadEvent;
                
                //reader.OnStateChanged += new EventHandler<CSLibrary.Events.OnStateChangedEventArgs>(OnStateChanged);
                reader.UDPKeepAliveOn();
                
                Connected = true;
                return true;
            }
            throw new CS203EngineException("Could not connect to device: " + result);
        }

        /// <summary>
        ///     Disconnect the connection to the device
        /// </summary>
        public void disconnect()
        {
            Console.WriteLine(reader.GetFirmwareVersion().ToString());
            if (Connected)
            {
                var debug = "Disconnecting connection interface: " + IP;
                Debug.WriteLine(debug);
                Console.WriteLine(debug);

                Connected = false;
                stopListening();
                reader.OnAsyncCallback -= NativeOnTagReadEvent;
                reader.Disconnect();
            }
        }

        /// <summary>
        ///     Get an antenna by index
        /// </summary>
        /// <param name="antenna">Index of antenna</param>
        /// <returns>The antenna object</returns>
        public Antenna getAntenna(int antenna)
        {
            if (antenna >= 0 && antenna < reader.AntennaList.Count)
            {
                var obj = reader.AntennaList[antenna];
                return new Antenna(this, antenna)
                    /*{
                    enabled = obj.State == CSLibrary.Constants.AntennaPortState.ENABLED,
                    power = obj.PowerLevel
                }*/;
            }
            throw new CS203EngineException("Antenna number is out of range");
        }

        /// <summary>
        ///     Set power of an antenna by index
        /// </summary>
        /// <param name="antenna">Index of antenna</param>
        /// <param name="power">Power value. power = dBm / 10</param>
        public void setPower(int antenna, uint power)
        {
            if (antenna >= 0 && antenna < reader.AntennaList.Count)
            {
                var obj = reader.AntennaList[antenna];
                if (obj.State == AntennaPortState.ENABLED)
                {
                    obj.AntennaConfig.powerLevel = power;
                    obj.Store(reader);
                    SaveConfig();
                }
                else
                {
                    throw new CS203EngineException("Antenna is not enabled");
                }
            }
            else
            {
                throw new CS203EngineException("Antenna number is out of range");
            }
        }

        /// <summary>
        ///     Get power of an antenna by index
        /// </summary>
        /// <param name="antenna">Index of antenna</param>
        /// <returns>Power value. power = dBm / 10</returns>
        public uint getPower(int antenna)
        {
            if (antenna >= 0 && antenna < reader.AntennaList.Count)
            {
                var obj = reader.AntennaList[antenna];
                return obj.AntennaConfig.powerLevel;
            }
            throw new CS203EngineException("Antenna number is out of range");
        }

        /// <summary>
        ///     Set enabled state of an antenna by index
        /// </summary>
        /// <param name="antenna">Index of antenna</param>
        /// <param name="enabled">State</param>
        public void setEnable(int antenna, bool enabled)
        {
            if (antenna >= 0 && antenna < reader.AntennaList.Count)
            {
                var obj = reader.AntennaList[antenna];
                obj.State = enabled ? AntennaPortState.ENABLED : AntennaPortState.DISABLED;
                obj.Store(reader);
                SaveConfig();
            }
            else
            {
                throw new CS203EngineException("Antenna number is out of range");
            }
        }

        /// <summary>
        ///     Get enabled state of an antenna by index
        /// </summary>
        /// <param name="antenna">Index of antenna</param>
        /// <returns>State</returns>
        public bool getEnable(int antenna)
        {
            if (antenna >= 0 && antenna < reader.AntennaList.Count)
            {
                var obj = reader.AntennaList[antenna];
                return obj.State == AntennaPortState.ENABLED;
            }
            throw new CS203EngineException("Antenna number is out of range");
        }

        /// <summary>
        ///     Event to handle every tag scanned in by a ConnectionInterface.
        ///     (No treshold or delay)
        /// </summary>
        public event TagReadHandle OnTagRead;

        private string prevTag;

        private Dictionary<string, DateTime> tags = new Dictionary<string, DateTime>();

        private PersonItemTagPair nextPair = new PersonItemTagPair();
        /// <summary>
        ///     The original HighLevelInterface event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NativeOnTagReadEvent(object sender, OnAsyncCallbackEventArgs e)
        {
            if (OnTagRead == null) return;
            if (reader.Name == "OFF")
            {
                return;
            }
            bool person = false;
            string tag = e.info.epc.ToString();
            var waitTime = 5;
            if (personList.Contains(tag))
            {
                if (reader.Name == "OUT" || !UseInScanWait)
                {
                    return;
                }
                person = true;
                waitTime = 5;
            } else
            {
                person = false;
            }
            if (reader.Name == "OUT")
            {
                waitTime = 60;
            }
            
            if (tags.ContainsKey(tag))
            {
                if (DateTime.Now > tags[tag].AddSeconds(waitTime))
                {
                    tags[tag] = DateTime.Now;
                }
                else
                {
                    return;
                }

            }
            else
            {
                tags.Add(tag, DateTime.Now);
            }
            prevTag = tag;

            var ignoredTag = null; // IgnoredTag.GetByTagId(tag);

            if (ignoredTag == null)
            {
                TagReadEventArgs eventArgs;
                if (reader.Name == "IN")
                {
                    if (person)
                    {
                        nextPair.PersonTag = tag;
                    } else
                    {
                        nextPair.ItemTag = tag;
                    }
                    if (nextPair.ItemTag != null && (nextPair.PersonTag != null || !UseInScanWait))
                    {
                        eventArgs = new TagReadEventArgs(tag, e.info.antennaPort, nextPair);
                        nextPair = new PersonItemTagPair();
                    } else
                    {
                        return;
                    }
                } else
                {
                    eventArgs = new TagReadEventArgs(tag, e.info.antennaPort, new PersonItemTagPair());
                }
                OnTagRead(this, eventArgs);
            }
        }

        /// <summary>
        ///     Starts listening (scanning) for tags
        /// </summary>
        public void startListening()
        {
            if (IsListening)
            {
                throw new CS203EngineException("Interface is already listening");
            }
            if (reader.State != RFState.IDLE)
            {
                reader.StopOperation(true);
            }

            reader.SetOperationMode(RadioOperationMode.CONTINUOUS);
            //reader.SetTagGroup(Program.appSetting.tagGroup);
            DynamicQParms m_dynQ = new DynamicQParms();
            m_dynQ.thresholdMultiplier = 0;
            m_dynQ.maxQValue = 15;
            m_dynQ.minQValue = 0;
            m_dynQ.retryCount = 0;
            m_dynQ.startQValue = 7;
            m_dynQ.toggleTarget = 1;

            reader.SetSingulationAlgorithmParms(SingulationAlgorithm.DYNAMICQ, m_dynQ);
            reader.Options.TagRanging.flags = SelectFlags.ZERO;
            reader.Options.TagRanging.QTMode = false; // reset to default
            reader.Options.TagRanging.accessPassword = 0x0; // reset to default
            reader.StartOperation(Operation.TAG_RANGING, false);

            IsListening = true;
        }

        /// <summary>
        ///     Saves configuration for the ConnectionInterface (power and antenna state)
        /// </summary>
        public void SaveConfig()
        {
            var RFIDserialNumber = reader.GetPCBAssemblyCode();
            var settingsObj = new SettingsObject
            {
                AntennaList = reader.AntennaList,
                timeout = timeout,
                serialNumber = RFIDserialNumber
            };

            var mySerializer = new XmlSerializer(typeof (SettingsObject));

            var stringWriter = new StringWriter();

            mySerializer.Serialize(stringWriter, settingsObj);

            stringWriter.Flush();

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = config.AppSettings.Settings;

            var xmlConfig = stringWriter.ToString();

            if (settings[RFIDserialNumber] == null)
            {
                settings.Add(RFIDserialNumber, xmlConfig);
            }
            else
            {
                settings[RFIDserialNumber].Value = xmlConfig;
            }

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
        }

        /// <summary>
        ///     Loads configuration for the ConnectionInterface (power and antenna state)
        /// </summary>
        public void LoadConfig()
        {
            var RFIDserialNumber = reader.GetPCBAssemblyCode();

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = config.AppSettings.Settings;


            var xmlConfig = "";

            if (settings[RFIDserialNumber] != null)
            {
                xmlConfig = settings[RFIDserialNumber].Value;

                var mySerializer = new XmlSerializer(typeof (SettingsObject));

                var stringReader = new StringReader(xmlConfig);

                var settingsObj = (SettingsObject) mySerializer.Deserialize(stringReader);

                stringReader.Close();

                reader.AntennaList = settingsObj.AntennaList;
                timeout = settingsObj.timeout;
            }
        }

        /// <summary>
        ///     Stops listening (scanning) for tags
        /// </summary>
        public void stopListening()
        {
            if (IsListening)
            {
                if (reader.State == RFState.BUSY)
                {
                    reader.StopOperation(true);
                    while (reader.State != RFState.IDLE)
                    {
                    }
                }
                IsListening = false;
            }
        }
    }

    /// <summary>
    ///     A settingsobject for storing configuration in XML format.
    /// </summary>
    [Serializable]
    public class SettingsObject
    {
        [XmlArray("AntennaList")] public AntennaList AntennaList;

        public string serialNumber;
        public uint timeout;
    }

    /// <summary>
    ///     TagReadEventArgs for every TagScanEvent
    /// </summary>
    public class TagReadEventArgs : EventArgs
    {
        public TagReadEventArgs(string _tagID, uint _antennaID, PersonItemTagPair _personItemTagPair)
        {
            tagID = _tagID;
            antennaID = _antennaID;
            personItemTagPair = _personItemTagPair;
        }

        /// <summary>
        ///     The TagID of the scanned tag
        /// </summary>
        public string tagID { get; private set; }

        /// <summary>
        ///     The antenna port number of the source which scanned the tag
        /// </summary>
        public uint antennaID { get; private set; }

        public PersonItemTagPair personItemTagPair { get; private set; } 
    }

    public struct PersonItemTagPair
    {
        public string ItemTag { get; set; }
        public string PersonTag { get; set; }
    }
}