using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FELM
{
    /// <summary>
    /// Interaction logic for EventVare.xaml
    /// </summary>
    public partial class EventVare : Window
    {
        string eventStr;

        public static List<VareClass> myList = new List<VareClass>();
        ICollectionView cvVare = CollectionViewSource.GetDefaultView(myList);
        public static List<send> mySend = new List<send>();
        API vareApi = new API();
        

        public EventVare( string str)
        {
            InitializeComponent();
            eventStr = str;
            
            

            Vare.ItemsSource = cvVare;
           
            
            eventLabel.Content = eventStr;


        }

        void Onload(object sender, RoutedEventArgs e)
        {
            _ = GetVareAsync();
        }
        private void id_change(object sender, TextChangedEventArgs e)
        {
            string str = id.Text;

            if (!string.IsNullOrEmpty(str))
            {
                
                bool found = false;
                int pos = 0;
                while (pos <= (myList.Count - 1) && found == false)
                {
                    if (myList[pos].VareNr == id.Text && myList[pos].Status == "Hjemme")
                    {
                        this.vareB.Content = myList[pos].Beskrivelse;
                        this.vareB.Visibility = Visibility.Visible;
                        this.Send.Visibility = Visibility.Visible;
                        found = true;
                    }    
                    else
                    {
                        this.vareB.Content = myList[pos].Beskrivelse;
                        this.vareB.Visibility = Visibility.Visible;
                        this.vareB.Content = "Vare er ikke klar til udlejning";
                        this.Send.Visibility = Visibility.Collapsed;
                        found = false;
                        
                        pos += 1;
                    }
                }


            }
            else
            {
                this.Send.Visibility = Visibility.Collapsed;
                this.vareB.Visibility = Visibility.Hidden;
            }
        }

        private bool filter(object o)
        {
            VareClass v = (o as VareClass);

            if (o is VareClass)
            {
                if(v.Status == "Hjemme")
                {
                    
                    return true;
                }
                else
                {
                    
                    return false;
                }
            }else
            {
                
                return false;
            }
        }

        private async Task<List<VareClass>> GetVareAsync()
        {

            JArray VareResult = await vareApi.GetAllVare();

            foreach (JObject item in VareResult)
            {
                string itemBeskrivelse = (string)item.GetValue("beskrivelse");
                int itemAntal = (int)item.GetValue("antal");
                string itemStatus = (string)item.GetValue("status");
                string itemVarenummer = (string)item.GetValue("varenummer");
                string itemTilgang = (string)item.GetValue("tilgang");
                string itemAfgang = (string)item.GetValue("afgang");
                string itemAmpere = (string)item.GetValue("ampere");
                string itemNote = (string)item.GetValue("note");
                string itemVareLokation = (string)item.GetValue("vareLokation");
                string itemPinNr = (string)item.GetValue("pinNr");
                string itemWebshopNummer = (string)item.GetValue("webshopNummer");
                int itemLength = (int)item.GetValue("length");
                string itemRfidNummer = (string)item.GetValue("rfidNummer");
                string itemQrKode = (string)item.GetValue("qrKode");
                int itemFavorit = (int)item.GetValue("favorit");

                Vare.Items.Refresh();

                myList.Add(new VareClass(itemVarenummer, itemBeskrivelse, itemTilgang, itemAfgang, itemAmpere, itemStatus, itemAntal, itemVareLokation, itemPinNr, itemLength, itemFavorit, itemNote, itemWebshopNummer, itemRfidNummer, itemQrKode));
                
            }
            cvVare.Filter = new Predicate<object>(filter);
            return myList;

        }
        public class send
        {
            private string _event;
            private string _vare;


            public send(string Event, string Vare)
            {
                this._event = Event;
                this._vare = Vare;

            }

            public string Event
            {
                get { return _event; }
                set { _event = value; }
            }

            public string Vare
            {
                get { return _vare; }
                set { _vare = value; }
            }

        }


        private void Send_Click(object sender, RoutedEventArgs e)
        {
            string sEvent = eventLabel.Content.ToString();
            string sVare = vareB.Content.ToString();

            mySend.Add(new send(sEvent, sVare));
        }
    }
}
