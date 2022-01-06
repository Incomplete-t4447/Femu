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
    /// Interaction logic for HistorikWindow.xaml
    /// </summary>
    public partial class HistorikWindow : Window
    {
        List<VareClass> vareList = new List<VareClass>();
        API API = new API();
        public HistorikWindow()
        {
            InitializeComponent();
            HistorikPage_DataGrid.ItemsSource = vareList;
        }
        public async void Renew()
        {
            JArray CArray = await API.GetLastVare();
            vareList.Clear();
            foreach (JObject item in CArray)
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


                vareList.Add(new VareClass(itemVarenummer, itemBeskrivelse, itemTilgang, itemAfgang, itemAmpere, itemStatus, itemAntal, itemVareLokation, itemPinNr, itemLength, "0", itemNote, itemWebshopNummer, itemRfidNummer, itemQrKode, null));
            }
            HistorikPage_DataGrid.Items.Refresh();
        }
    }
}