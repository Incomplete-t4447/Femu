using System;
using System.Net;
using CSLibrary.Net;

namespace CS203Engine
{
    /// <summary>
    ///     Represent a basic structure of a device
    /// </summary>
    public struct DeviceInfo
    {
        public string description;
        public string ipaddress;
        public string name;
    }

    /// <summary>
    ///     Represents a NetScanner which search the network for RFID devices.
    /// </summary>
    public class NetScanner
    {
        /// <summary>
        ///     Delegate SearchCompleteHandle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void SearchCompleteHandle(object sender, ScanCompleteEventArgs e);

        private readonly NetFinder netfinder;

        public NetScanner()
        {
            netfinder = new NetFinder();
            netfinder.OnSearchCompleted += NativeOnSearchCompleted;
        }

        /// <summary>
        ///     If the NetScanner is currently operating
        /// </summary>
        public bool IsScanning { get; private set; }

        /// <summary>
        ///     Start scanning
        /// </summary>
        public void Scan()
        {
            Scan(null);
        }

        /// <summary>
        ///     Start scanning
        /// </summary>
        /// <param name="IP">A direct ip to scan for</param>
        public void Scan(string IP)
        {
            if (IsScanning)
            {
                throw new CS203EngineException("Scanner is already running");
            }
            IsScanning = true;
            if (IP != null)
            {
                var ipaddress = IPAddress.Broadcast;
                if (IPAddress.TryParse(IP, out ipaddress))
                {
                    netfinder.SearchDevice(ipaddress);
                }
            }
            else
            {
                netfinder.SearchDevice();
            }
        }

        /// <summary>
        ///     Dispose the NetScanner
        /// </summary>
        public void Kill()
        {
            Stop();
            netfinder.Dispose();
        }

        /// <summary>
        ///     Stops scanner
        /// </summary>
        public void Stop()
        {
            if (IsScanning || netfinder.Operation == RecvOperation.SEARCH)
            {
                IsScanning = false;
                netfinder.Stop();
            }
        }

        /// <summary>
        ///     Event to handle every scan result.
        ///     Is called for every scan result
        /// </summary>
        public event SearchCompleteHandle OnScanComplete;

        /// <summary>
        ///     Native OnSearchCompleted event handle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NativeOnSearchCompleted(object sender, DeviceFinderArgs e)
        {
            if (OnScanComplete == null) return;

            var eventArgs =
                new ScanCompleteEventArgs(new DeviceInfo
                {
                    name = e.Found.DeviceName,
                    description = e.Found.Description,
                    ipaddress = e.Found.IPAddress
                });
            OnScanComplete(this, eventArgs);
        }

        /// <summary>
        ///     Event args for OnScanComplete event
        /// </summary>
        public class ScanCompleteEventArgs : EventArgs
        {
            public ScanCompleteEventArgs(DeviceInfo _device)
            {
                device = _device;
            }

            /// <summary>
            ///     The found device
            /// </summary>
            public DeviceInfo device { get; private set; }
        }
    }
}