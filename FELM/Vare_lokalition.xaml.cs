using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for Vare_lokalition.xaml
    /// </summary>
    public partial class Vare_lokalition : Window
    {
        public static List<VareClass> myList = new List<VareClass>();
        public static List<send> mySend = new List<send>();

        public static List<lokalitionEventClass> myList2 = new List<lokalitionEventClass>();
        API vareApi = new API();

        public Vare_lokalition()
        {
            miList();
            InitializeComponent();
            lokalition.ItemsSource = myList2;
        }

        private void id_change(object sender, TextChangedEventArgs e){
            string str = id.Text;

            if (!string.IsNullOrEmpty(str))
            {
                bool found = false;
                int pos = 0;
                while (pos <=(myList.Count - 1) && found == false)
                {
                    if(myList[pos].VareNr == id.Text)
                    {
                        this.vareB.Content = myList[pos].Beskrivelse;
                        this.vareB.Visibility = Visibility.Visible;
                        this.lokalition.Visibility = Visibility.Visible;
                        found = true;
                    }
                    else
                    {
                        pos +=1;
                    }
                }
                
                
            }
        }

        public void miList()
        {
            myList2.Add(new lokalitionEventClass("festeval", "øl tælt"));
            myList2.Add(new lokalitionEventClass("konsert", "øl bord"));
        }
        void Onload(object sender, RoutedEventArgs e)
        {
            _ = GetVareAsync();
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



                myList.Add(new VareClass(itemVarenummer, itemBeskrivelse, itemTilgang, itemAfgang, itemAmpere, itemStatus, itemAntal, itemVareLokation, itemPinNr, itemLength, itemFavorit, itemNote, itemWebshopNummer, itemRfidNummer, itemQrKode));
            }

            return myList;
        }

        private void lokalition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (vareB.Visibility == Visibility.Visible)
            {
                loSend.Visibility = Visibility.Visible;
            }
        }

        private void loSend_Click(object sender, RoutedEventArgs e)
        {
            
            ComboBoxItem cbi = (ComboBoxItem)lokalition.SelectedItem;
            string boxVal = cbi.Content.ToString();
            string labVal = vareB.Content.ToString();
            string idVal = id.Text;

            mySend.Add(new send(idVal, labVal));
        }
        public class send
        {
            private string _id;
            private string _lokalition;
            

            public send(string id, string lokalition)
            {
                this._id = id;
                this._lokalition = lokalition;
                
            }

            public string id
            {
                get { return _id; }
                set { _id = value; }
            }

            public string lokalition
            {
                get { return _lokalition; }
                set { _lokalition = value; }
            }
            
        }
    }
}
