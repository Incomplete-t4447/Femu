using CS203Engine;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace FELM
{
    /// <summary>
    /// Interaction logic for Vare.xaml
    /// </summary>
    ///

    public partial class Vare : Page
    {
        // init variabler
        public string searchstr;

        private ICollectionView cvVare = CollectionViewSource.GetDefaultView(myList);
        public static List<VareClass> myList = new List<VareClass>();
        public static List<EventHistorik> eventHistorik = new List<EventHistorik>();
        public static List<ekstraClass> ekstraList = new List<ekstraClass>();
        public static List<containerClass> containerListe = new List<containerClass>();
        private API vareApi = new API();
        private string TempVareNr;
        private string tilbehørId;
        private bool toggleGridHistorik = true;
        private bool toggleGridTilbehør = true;
        private bool toggleScrolVare = true;
        private bool togglePrintVare = true;
        private bool toggleLokation = true;
        private bool toggleRediger = true;
        private VareClass row = null;
        private ConnectionInterface connectionInterface;
        private string scannedID = null;
        private bool scannerOnOff = false;


        public Vare()
        {
            InitializeComponent();
            VareGrid.ItemsSource = cvVare;
            RedigerButton.Visibility = Visibility.Hidden;
            MinimizeScroll();
        }

        void MinimizeScroll()
        {

            // ------ BORDER -----------
            scrollVare.Visibility = Visibility.Collapsed;
            dataBorder.SetValue(Grid.ColumnProperty, 0);
            dataBorder.SetValue(Grid.ColumnSpanProperty, 2);
            dataBorder.Margin = new System.Windows.Thickness(9, 30, 30, 30);

            // ------ DATAGRID -----
            VareGrid.Margin = new System.Windows.Thickness(-400, 39, 45, 39);

            // ----- Search bars ------
            SearchBars.SetValue(Grid.ColumnProperty, 0);
            SearchBars.SetValue(Grid.ColumnSpanProperty, 2);

            SearchBars.Margin = new System.Windows.Thickness(49, 10.2, 45.4, 473);
            QRBox.Margin = new System.Windows.Thickness(0, 0, 5, 0);
            SearchNote.Width = 68;

            for (int i = 0; i < SearchBars.Children.Count; i++)
            {
                SearchBars.Children[i].SetValue(WidthProperty, 80.0);
            }
            //VareNrBox.Width = 80;

            // ---- Search Bar Labels ------

            for (int i = 0; i < SearchBarLabels.Children.Count; i++)
            {
                SearchBarLabels.Children[i].SetValue(FontSizeProperty, 14.0);
                SearchBarLabels.Children[i].SetValue(MarginProperty, new System.Windows.Thickness(15, 0, 13, 0));
            }
            SearchVareNr.Margin = new System.Windows.Thickness(15, 0, 6, 0);
            SearchBeskrivelse.Margin = new System.Windows.Thickness(15, 0, 9, 0);
            SearchAntal.Margin = new System.Windows.Thickness(15, 0, 23, 0);
            SearchLokation.Margin = new System.Windows.Thickness(15, 0, 4, 0);
            SearchQR.Margin = new System.Windows.Thickness(15, 0, 18, 0);

            SearchBarLabels.SetValue(Grid.ColumnProperty, 0);
            SearchBarLabels.SetValue(Grid.ColumnSpanProperty, 2);
            SearchBarLabels.Margin = new System.Windows.Thickness(40, 240, 0, 490);
        }
        void UnMinimizeScroll()
        {
            // ------ BORDER -----------
            scrollVare.Visibility = Visibility.Visible;
            dataBorder.Margin = new System.Windows.Thickness(30, 30, 30, 30);
            dataBorder.SetValue(Grid.ColumnProperty, 1);
            dataBorder.SetValue(Grid.ColumnSpanProperty, 1);

            // ------ DATAGRID -----
            VareGrid.Margin = new System.Windows.Thickness(47, 39, 45, 39);

            // ----- Search bars ------
            SearchBars.SetValue(Grid.ColumnProperty, 1);
            SearchBars.SetValue(Grid.ColumnSpanProperty, 1);
            SearchBars.Margin = new System.Windows.Thickness(47, 10, 42, 473);
            SearchNote.Width = 41;

            for (int i = 0; i < SearchBars.Children.Count; i++)
            {
                SearchBars.Children[i].SetValue(WidthProperty, 50.0);
            }


            // ---- Search Bar Labels ------

            for (int i = 0; i < SearchBarLabels.Children.Count; i++)
            {
                SearchBarLabels.Children[i].SetValue(FontSizeProperty, 10.0);
                SearchBarLabels.Children[i].SetValue(MarginProperty, new System.Windows.Thickness(3, 0, 5, 0));
            }
            SearchVareNr.Margin = new System.Windows.Thickness(3, 0, 3, 0);
            SearchBeskrivelse.Margin = new System.Windows.Thickness(3, 0, 3, 0);
            SearchAntal.Margin = new System.Windows.Thickness(3, 0, 16, 0);
            SearchStatus.Margin = new System.Windows.Thickness(3, 0, 7, 0);
            SearchAmpere.Margin = new System.Windows.Thickness(3, 0, 7, 0);
            SearchTilgang.Margin = new System.Windows.Thickness(3, 0, 10, 0);
            SearchAfgang.Margin = new System.Windows.Thickness(3, 0, 13, 0);
            SearchPinNr.Margin = new System.Windows.Thickness(3, 0, 10, 0);
            SearchRFID.Margin = new System.Windows.Thickness(3, 0, 13, 0);
            SearchNote.Margin = new System.Windows.Thickness(3, 0, 15, 0);
            SearchQR.Margin = new System.Windows.Thickness(3, 0, 15, 0);

            SearchBarLabels.SetValue(Grid.ColumnProperty, 1);
            SearchBarLabels.SetValue(Grid.ColumnSpanProperty, 1);

            SearchBarLabels.Margin = new System.Windows.Thickness(47, 247, 0, 493);
        }

        void MinimizeGrid()
        {
            // ------ BORDER -----------
            dataBorder.Margin = new System.Windows.Thickness(30, 30, 30, 30);
            dataBorder.SetValue(Grid.ColumnProperty, 1);
            dataBorder.SetValue(Grid.ColumnSpanProperty, 1);

            // ------ DATAGRID -----
            VareGrid.Margin = new System.Windows.Thickness(47, 39, 45, 39);

            // ----- Search bars ------
            SearchBars.SetValue(Grid.ColumnProperty, 1);
            SearchBars.SetValue(Grid.ColumnSpanProperty, 1);
            SearchBars.Margin = new System.Windows.Thickness(47, 10, 30, 473);
            SearchNote.Width = 41;

            for (int i = 0; i < SearchBars.Children.Count; i++)
            {
                SearchBars.Children[i].SetValue(WidthProperty, 50.0);
            }

            // ---- Search Bar Labels ------

            for (int i = 0; i < SearchBarLabels.Children.Count; i++)
            {
                SearchBarLabels.Children[i].SetValue(FontSizeProperty, 12.0);
                SearchBarLabels.Children[i].SetValue(MarginProperty, new System.Windows.Thickness(0, 0, 0, 0));
            }
            SearchAntal.Margin = new System.Windows.Thickness(0, 0, 10, 0);
            SearchStatus.Margin = new System.Windows.Thickness(0, 0, 5, 0);
            SearchTilgang.Margin = new System.Windows.Thickness(0, 0, 8, 0);
            SearchAfgang.Margin = new System.Windows.Thickness(0, 0, 10, 0);
            SearchPinNr.Margin = new System.Windows.Thickness(0, 0, 5, 0);
            SearchLængde.Margin = new System.Windows.Thickness(0, 0, 5, 0);
            SearchRFID.Margin = new System.Windows.Thickness(0, 0, 7, 0);
            SearchNote.Margin = new System.Windows.Thickness(0, 0, 18, 0);
            SearchQR.Margin = new System.Windows.Thickness(0, 0, 17, 0);

            SearchBarLabels.SetValue(Grid.ColumnProperty, 1);
            SearchBarLabels.SetValue(Grid.ColumnSpanProperty, 1);

            QRBox.Margin = new System.Windows.Thickness(0, 0, 0, 0);

            SearchBarLabels.Margin = new System.Windows.Thickness(43, 247, 0, 493);
        }

        // Gå tilbage til hovedmenuen
        private void goBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(Pages.p5);

            visEventHistorik.Content = "Vis event historik";
            visEkstraTilbehør.Content = "Vis ekstra tilbehør";

            visEventHistorik.Visibility = Visibility.Visible;
            visEkstraTilbehør.Visibility = Visibility.Visible;

            EventGrid.Visibility = Visibility.Collapsed;

            labelSearch.Visibility = Visibility.Collapsed;
            txtEventSearchBox.Visibility = Visibility.Collapsed;

            VareGrid.Visibility = Visibility.Visible;

            SearchBars.Visibility = Visibility.Visible;
            SearchBarLabels.Visibility = Visibility.Visible;

            toggleGridHistorik = true;
            toggleGridTilbehør = true;

            MinimizeScroll();

            EkstraTilbehørGrid.Visibility = Visibility.Collapsed;

            cVareNr.IsChecked = false;
            cBeskrivelse.IsChecked = false;
            cAntal.IsChecked = false;
            cStatus.IsChecked = false;
            cTilgang.IsChecked = false;
            cAfgang.IsChecked = false;
            cLokation.IsChecked = false;
            cAmper.IsChecked = false;
            cPin.IsChecked = false;
            cLængde.IsChecked = false;
            cWebNr.IsChecked = false;
            cRFID.IsChecked = false;
            cNote.IsChecked = false;
            cQR.IsChecked = false;
            printInfo.Visibility = Visibility.Collapsed;
            togglePrintVare = true;




        }

        // hent vare når siden start op
        private void Onload(object sender, RoutedEventArgs e)
        {

            _ = GetVareAsync();
            _ = GetEkstraAsync();
            _ = GetLokationAsync();
        }

        //API funktion til at ligge varene fra database ind i VareClass
        private async Task<List<VareClass>> GetVareAsync()
        {
            JArray VareResult = await vareApi.GetAllVare();
            myList.Clear();
            //looper api kald
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
                string itemSalgspris = (string)item.GetValue("salgspris");

                myList.Add(new VareClass(itemVarenummer, itemBeskrivelse, itemTilgang, itemAfgang, itemAmpere, itemStatus, itemAntal, itemVareLokation, itemPinNr, itemLength, itemNote, itemSalgspris, itemWebshopNummer, itemRfidNummer, itemQrKode, null));
            }

            VareGrid.Items.Refresh();

            return myList;
        }

        private async Task<List<containerClass>> GetLokationAsync()
        {
            int i = 1;
            JArray locationResult = await vareApi.getLokations();
            kontainerStack.Children.Clear();
            containerListe.Clear();
            //looper api kald
            foreach (JObject item in locationResult)
            {
                int LocationdID = (int)item.GetValue("LocationdID");
                string location = (string)item.GetValue("location");
                string description = (string)item.GetValue("description");
                int VareNrStart = (int)item.GetValue("VareNrStart");
                int VareNrSlut = (int)item.GetValue("VareNrSlut");


                containerListe.Add(new containerClass(LocationdID, location, description, VareNrStart, VareNrSlut));
            }

            foreach (containerClass k in containerListe)
            {
                Button kontainerButton = new Button();
                BrushConverter bc = new BrushConverter();


                kontainerButton.Foreground = (Brush)bc.ConvertFrom("#FFFFFF");

                kontainerButton.Margin = new Thickness(10, 5, 0, 0);

                kontainerButton.Content = k.location + ": " + k.VareNrStart + "-" + k.VareNrSlut;



                if (i % 2 == 0)
                {
                    kontainerButton.Background = (Brush)bc.ConvertFrom("#00b5a3");
                }
                else
                {
                    kontainerButton.Background = (Brush)bc.ConvertFrom("#017056");

                }

                kontainerButton.Click += (s, EventArgs) =>
                {
                    editKontainerStack.Visibility = Visibility.Visible;
                    containerNavn.Content = k.location;
                    editVareNrStartBox.Text = k.VareNrStart.ToString();
                    editVareNrSlutBox.Text = k.VareNrSlut.ToString();
                };

                i++;

                kontainerStack.Children.Add(kontainerButton);

            }

            VareLokation.Items.Clear();

            foreach (containerClass c in containerListe)
            {
                if(c.VareNrSlut != 0)
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = c.location;

                    VareLokation.Items.Add(item);
                }

            }



            return containerListe;
        }

        //Api kald til ekstra tilbehør
        private async Task<List<ekstraClass>> GetEkstraAsync()
        {
            JArray EkstraResult = await vareApi.GetEkstraAsync();

            //looper api kald
            foreach (JObject item in EkstraResult)
            {
                string itemId = (string)item.GetValue("id");
                string itemBeskrivelse = (string)item.GetValue("beskrivelse");
                int itemAntal = (int)item.GetValue("antal");
                string itemWebshopNummer = (string)item.GetValue("webshop");

                ekstraList.Add(new ekstraClass(itemBeskrivelse, itemAntal, itemWebshopNummer, itemId));
            }

            EkstraTilbehørDataGrid.ItemsSource = ekstraList;
            EkstraTilbehørDataGrid.Items.Refresh();

            return ekstraList;
        }

        //api funktion til at rediger vare
        private async Task<List<VareClass>> EditVareAsync(string vareNummer)
        {
            JArray VareResult = await vareApi.EditVareAsync(vareNummer, VareNr.Text, Beskrivelse.Text, StikTypeTilgang.Text, StikTypeAfgang.Text, Ampere.Text, StatusComboBox.Text, Int32.Parse(Antal.Text), VareLokation.Text, PinNummer.Text, Int32.Parse(Længde.Text), Note.Text, WebshopVarenummer.Text, RFIDNummer.Text, "", Salgs.Text);


            return myList;
        }

        private async Task<List<VareClass>> updatePriceAsync()
        {
            JArray VareResult = await vareApi.updatePrice(WebshopVarenummer.Text, Salgs.Text);


            return myList;
        }

        private async Task<List<containerClass>> EditLokationAsync()
        {
            JArray VareResult = await vareApi.editlokation(containerNavn.Content.ToString(), "", Int32.Parse(editVareNrStartBox.Text) , Int32.Parse(editVareNrSlutBox.Text));


            return containerListe;
        }

        private async Task<List<containerClass>> EditLokationRangeAsync(containerClass container)
        {
            int nyStart = 10000;

            if(container.VareNrStart == 1)
            {
                nyStart = 9999;
            }

            JArray VareResult = await vareApi.editlokation(container.location, "", container.VareNrStart + nyStart, container.VareNrSlut + 10000);


            return containerListe;
        }

        private async Task<List<ekstraClass>> EditEkstraAsync(string tilbehørid)
        {
            JArray VareResult = await vareApi.EditEkstraAsync(tilbehørid, ekstraBeskrivelse.Text, ekstraAntal.Text, ekstraWebshopNummer.Text);

            EkstraTilbehørDataGrid.Items.Refresh();

            return ekstraList;
        }

        // start søge funktionn hvis der bliver skrevet i søge felter
        private void txtSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (textbox != null)
            {
                // Hvis søgebar ikke er tom skal den filtre
                searchstr = textbox.Text;
                if (!string.IsNullOrEmpty(searchstr))
                {
                    Filter();
                }
                // hvis søgebare er tom skal den stop med at filtre og opfriske listen
                else
                {
                    VareGrid.ItemsSource = myList;
                    VareGrid.Items.Refresh();
                }
            }
        }

        // Tjek om status er valgt og køre den rigtige filtre
        private void status_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // hvis søgebar er tom skal den kun køre filtre til favorit checkbox og status dropdown
            VareGrid.Items.Filter = new Predicate<object>(checkFilter);
        }

        // søge function til vare grit
        private void Filter()
        {
            int pos = 0;

            List<VareClass> data = myList;
            List<VareClass> MyTempList = new List<VareClass>();

            // tjek og gem hvilken/hvilke koloner gritet skal sorter efter
            while (pos < 14)
            {
                switch (pos)
                {
                    case 0:
                        if (!string.IsNullOrEmpty(VareNrBox.Text))
                        {
                            data = data.FindAll(x => x.VareNr.ToLower().Contains(VareNrBox.Text.ToLower()));
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case 1:
                        if (!string.IsNullOrEmpty(beskrivelseBox.Text))
                        {
                            data = data.FindAll(x => x.Beskrivelse.ToLower().Contains(beskrivelseBox.Text.ToLower()));
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case 2:
                        if (!string.IsNullOrEmpty(TilgangBox.Text))
                        {
                            data = data.FindAll(x => x.Tilgang.ToLower().Contains(TilgangBox.Text.ToLower()));
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case 3:
                        if (!string.IsNullOrEmpty(AfgangBox.Text))
                        {
                            data = data.FindAll(x => x.Afgang.ToLower().Contains(AfgangBox.Text.ToLower()));
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case 4:
                        if (!string.IsNullOrEmpty(AntalBox.Text))
                        {
                            data = data.FindAll(x => x.Antal.ToString().ToLower().Contains(AntalBox.Text.ToLower()));
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case 5:
                        if (!string.IsNullOrEmpty(LokationBox.Text))
                        {
                            data = data.FindAll(x => x.VareLokation.ToLower().Contains(LokationBox.Text.ToLower()));
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case 6:
                        if (!string.IsNullOrEmpty(AmpereBox.Text))
                        {
                            data = data.FindAll(x => x.Ampere.ToLower().Contains(AmpereBox.Text.ToLower()));
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case 7:
                        if (!string.IsNullOrEmpty(PinBox.Text))
                        {
                            data = data.FindAll(x => x.PinNr.ToLower().Contains(PinBox.Text.ToLower()));
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case 8:
                        if (!string.IsNullOrEmpty(LængdeBox.Text))
                        {
                            data = data.FindAll(x => x.Længde.ToString().ToLower().Contains(LængdeBox.Text.ToLower()));
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case 9:
                        if (!string.IsNullOrEmpty(webBox.Text))
                        {
                            data = data.FindAll(x => x.WebshopVareNummer.ToLower().Contains(webBox.Text.ToLower()));
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case 10:
                        if (!string.IsNullOrEmpty(RFIDBox.Text))
                        {
                            data = data.FindAll(x => x.RFIDNummer.ToLower().Contains(RFIDBox.Text.ToLower()));
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case 11:
                        if (!string.IsNullOrEmpty(NoteVareBox.Text))
                        {
                            data = data.FindAll(x => x.Note.ToLower().Contains(NoteVareBox.Text.ToLower()));
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case 12:
                        if (!string.IsNullOrEmpty(QRBox.Text))
                        {
                            data = data.FindAll(x => x.QR.ToLower().Contains(QRBox.Text.ToLower()));
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case 13:
                        if (!string.IsNullOrEmpty(SalgsBox.Text))
                        {
                            data = data.FindAll(x => x.Salgspris.ToLower().Contains(SalgsBox.Text.ToLower()));
                            break;
                        }
                        else
                        {
                            break;
                        }
                    default:
                        break;
                }

                // sorter efter data man har skrevet
                if (data.Count != 0 && data != myList && data != MyTempList)
                {
                    MyTempList = new List<VareClass>();

                    foreach (var row in data)
                    {
                        MyTempList.Add(new VareClass(row.VareNr, row.Beskrivelse, row.Tilgang, row.Afgang, row.Ampere, row.Status, row.Antal, row.VareLokation, row.PinNr, row.Længde, row.Note, row.Salgspris, row.WebshopVareNummer, row.RFIDNummer, row.QR, row.EventVareId));
                    }

                    data = MyTempList;

                    pos += 1;
                }
                else
                {
                    pos += 1;
                }
            }

            VareGrid.ItemsSource = data;
            VareGrid.Items.Refresh();
        }

        // Tjek om checkbox ikke er trykket og køre den rigtige filtre
        private void handleUnchecked(object sender, RoutedEventArgs e)
        {
            // hvis søgebar er tom skal den kun køre filtre til favorit checkbox og status dropdown
            VareGrid.Items.Filter = new Predicate<object>(checkFilter);
        }

        // Tjek om checkbox er trykket og køre den rigtige filtre
        private void handleChecked(object sender, RoutedEventArgs e)
        {
            // hvis søgebar er tom skal den kun køre filtre til favorit checkbox og status dropdown
            VareGrid.Items.Filter = new Predicate<object>(checkFilter);
        }

        private bool checkFilter(object o)
        {
            // tjekker om der er valgt en status og gemmer status i en variabel
            string val;
            ComboBoxItem cbi = (ComboBoxItem)status.SelectedItem;
            if (cbi == null)
            {
                val = " ";
            }
            else
            {
                val = cbi.Content.ToString();
            }

            // hvis en status er valgt
            if (val != " ")
            {
                if (o is VareClass)
                {
                    VareClass v = (o as VareClass);

                    if (v.Status == val)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            // hvis ingen status er valgt
            else if (val == " ")
            {
                if (o is VareClass)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        private void Lokation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // kør kun funktion hvis der valgt nået i combobox der ikke er lig med null
            if (VareLokation.SelectedValue != null)
            {


                // gem den valgte container i en string
                string containerString = ((ComboBoxItem)VareLokation.SelectedValue).Content.ToString();

                // find den sidste vare i container
                var result = myList.FindLast(v => v.VareLokation == containerString);

                var First = myList.Find(v => v.VareLokation == containerString);

                // init container
                containerClass container = containerListe[0];

                // init variabler
                int pos = 0;
                bool done = false;

                // find den rigtige container og gem den
                while (!done && pos < containerListe.Count)
                {
                    if(containerListe[pos].location == containerString)
                    {
                        container = containerListe[pos];
                        done = true;
                    }
                    else
                    {
                        pos += 1;
                    }
                }


                if (result != null)
                {
                    // hvis vare nr. er overskredet containerens range containerens range opdateres og varen skal smides i den nye range
                    if(Int32.Parse(result.VareNr) >= container.VareNrSlut)
                    {
                        // opdater vare nr. range
                        _ = EditLokationRangeAsync(container);


                        if (container.VareNrStart == 1)
                        {
                            container.VareNrStart  = container.VareNrStart + 9999;
                        }
                        else
                        {
                            container.VareNrStart = container.VareNrStart + 10000;
                        }

                        container.VareNrSlut = container.VareNrSlut + 10000;

                        container = containerListe[pos];
                        // fyld text box med den første post i den nye range
                        VareNr.Text = container.VareNrStart.ToString();


                    }
                    // hvis vare nr. er mindre end den første post i container efter at container er blvet opdateret
                    else if(Int32.Parse(result.VareNr) < container.VareNrStart)
                    {
                        VareNr.Text = container.VareNrStart.ToString();
                    }
                    // brug den næste post i container
                    else
                    {
                        VareNr.Text = (Int32.Parse(result.VareNr) + 1).ToString();

                    }

                }

            }

        }

        // Funktion til at kopier varenummer med et nyt varenummer som er 1 større end det sidste i listen
        private void Kopier_Button(object sender, RoutedEventArgs e)
        {


            var row = (VareClass)VareGrid.SelectedItem;
            if (toggleScrolVare && togglePrintVare && toggleGridTilbehør && toggleGridHistorik && toggleRediger)
            {


                if (row != null)
                {

                    toggleScrolVare = false;
                    UnMinimizeScroll();

                    //if (row.VareLokation != null)
                    //{
                    //    toggleLokation = false;
                    //    toggleRediger = false;

                    //    var result = myList.FindLast(v => v.VareLokation == row.VareLokation); //finder den sidste i listen hvor list.varelokation er det samme som row i datagrid

                    //    //Virker kun hvis varen er i lokation C, fordi resten har bogstaver i varenummeret
                    //    if (result != null)
                    //    {
                    //        var parsedInt = int.Parse(result.VareNr); //bliver holdt op imod rangen
                    //        var temp = int.Parse(result.VareNr); //det nye varenummer

                    //        if (result.VareLokation == "A" && parsedInt >= 1999) //laver rangen til 10000-11999
                    //        {
                    //            temp += 8001;
                    //            TempVareNr = temp.ToString();
                    //        }
                    //        else if (result.VareLokation == "C" && parsedInt == 2999) //laver rangen til 12000-12999
                    //        {
                    //            temp += 9001;
                    //            TempVareNr = temp.ToString();
                    //        }
                    //        else if (result.VareLokation == "D" && parsedInt == 3999) //laver rangen til 13000-13999
                    //        {
                    //            temp += 9001;
                    //            TempVareNr = temp.ToString();
                    //        }
                    //        else if (result.VareLokation == "S" && parsedInt >= 9999) //laver rangen til 15000-19999
                    //        {
                    //            temp += 5001;
                    //            TempVareNr = temp.ToString();
                    //        }
                    //        else
                    //        {
                    //            temp += 1;
                    //            TempVareNr = temp.ToString();
                    //        }
                    //    }
                    //}
                    RedigerButton.Visibility = Visibility.Collapsed;
                    //VareNr.Text = TempVareNr;
                    Beskrivelse.Text = row.Beskrivelse;
                    StikTypeTilgang.Text = row.Tilgang;
                    StikTypeAfgang.Text = row.Afgang;
                    Ampere.Text = row.Ampere.ToString();
                    Note.Text = row.Note;
                    Længde.Text = row.Længde.ToString();
                    Salgs.Text = row.Salgspris.ToString();
                    Antal.Text = row.Antal.ToString();
                    VareLokation.Text = row.VareLokation.ToString();
                    WebshopVarenummer.Text = row.WebshopVareNummer;
                    Længde.Text = row.Længde.ToString();
                    Salgs.Text = row.Salgspris.ToString();

                }
                else
                {
                    MessageBox.Show("Du skal vælge en vare for at kunne kopier");
                }
            }
            else if (!toggleGridTilbehør)
            {
                MessageBox.Show("Du kan ikke vælge en vare hvor du er nu. Gå ud af ekstra tilbehør og prøv igen");
            }
            else if (row != null && (!togglePrintVare || !toggleGridHistorik || !toggleRediger))
            {
                togglePrintVare = true;
                toggleGridTilbehør = true;
                toggleGridHistorik = true;
                toggleRediger = true;

                printInfo.Visibility = Visibility.Collapsed;
                scrollEkstra.Visibility = Visibility.Collapsed;
                EventGrid.Visibility = Visibility.Collapsed;
                txtEventSearchBox.Visibility = Visibility.Collapsed;
                EkstraTilbehørGrid.Visibility = Visibility.Collapsed;
                TilføjVareBtn.Visibility = Visibility.Visible;
                visEkstraTilbehør.Content = "Vis ekstra tilbehør";
                visEventHistorik.Content = "Vis event historik";
                VareGrid.Visibility = Visibility.Visible;

                toggleScrolVare = false;
                UnMinimizeScroll();

                //if (row.VareLokation != null)
                //{
                //    toggleLokation = false;
                //    toggleRediger = false;

                //    var result = myList.FindLast(v => v.VareLokation == row.VareLokation); //finder den sidste i listen hvor list.varelokation er det samme som row i datagrid

                //    //Virker kun hvis varen er i lokation C, fordi resten har bogstaver i varenummeret
                //    if (result != null)
                //    {
                //        var parsedInt = int.Parse(result.VareNr); //bliver holdt op imod rangen
                //        var temp = int.Parse(result.VareNr); //det nye varenummer

                //        if (result.VareLokation == "A" && parsedInt >= 1999) //laver rangen til 10000-11999
                //        {
                //            temp += 8001;
                //            TempVareNr = temp.ToString();
                //        }
                //        else if (result.VareLokation == "C" && parsedInt == 2999) //laver rangen til 12000-12999
                //        {
                //            temp += 9001;
                //            TempVareNr = temp.ToString();
                //        }
                //        else if (result.VareLokation == "D" && parsedInt == 3999) //laver rangen til 13000-13999
                //        {
                //            temp += 9001;
                //            TempVareNr = temp.ToString();
                //        }
                //        else if (result.VareLokation == "S" && parsedInt >= 9999) //laver rangen til 15000-19999
                //        {
                //            temp += 5001;
                //            TempVareNr = temp.ToString();
                //        }
                //        else
                //        {
                //            temp += 1;
                //            TempVareNr = temp.ToString();
                //        }
                //    }
                //}
                RedigerButton.Visibility = Visibility.Collapsed;
                VareNr.Text = TempVareNr;
                Beskrivelse.Text = row.Beskrivelse;
                StikTypeTilgang.Text = row.Tilgang;
                StikTypeAfgang.Text = row.Afgang;
                Ampere.Text = row.Ampere.ToString();
                Note.Text = row.Note;
                Længde.Text = row.Længde.ToString();
                Salgs.Text = row.Salgspris.ToString();
                Antal.Text = row.Antal.ToString();
                VareLokation.Text = row.VareLokation.ToString();
                WebshopVarenummer.Text = row.WebshopVareNummer;
                Længde.Text = row.Længde.ToString();
                Salgs.Text = row.Salgspris.ToString();

            }
            else
            {

                MessageBox.Show("Du skal vælge en vare for at kunne kopiere");
            }

        }

        // åben eller luk print panel efter behov
        private void Print_Button(object sender, RoutedEventArgs e)
        {
            // hvis intet er blevet toggelt
            if (togglePrintVare == true && toggleGridTilbehør != false && toggleGridHistorik)
            {
                scrollVare.Visibility = Visibility.Collapsed;
                printInfo.Visibility = Visibility.Visible;
                togglePrintVare = false;
                MinimizeGrid();
            }
            else if (!toggleGridTilbehør)
            {
                MessageBox.Show("Du kan ikke vælge en vare hvor du er nu. Gå ud af ekstra tilbehør og prøv igen");
            }
            // hvis ekstra tilbehøre er blevet toggle først
            else if (togglePrintVare == true && (!toggleGridHistorik || toggleScrolVare))
            {
                MinimizeGrid();
                scrollVare.Visibility = Visibility.Collapsed;
                printInfo.Visibility = Visibility.Visible;
                scrollEkstra.Visibility = Visibility.Collapsed;
                EventGrid.Visibility = Visibility.Collapsed;
                txtEventSearchBox.Visibility = Visibility.Collapsed;
                togglePrintVare = false;
                toggleScrolVare = true;
                toggleGridTilbehør = true;
                toggleGridHistorik = true;
                EkstraTilbehørGrid.Visibility = Visibility.Collapsed;
                visEkstraTilbehør.Content = "Vis ekstra tilbehør";
                visEventHistorik.Content = "Vis event historik";
                VareGrid.Visibility = Visibility.Visible;

                //søgebar variabler
                labelSearch.Visibility = Visibility.Collapsed;
                SearchBars.Visibility = Visibility.Visible;
                SearchBarLabels.Visibility = Visibility.Visible;
            }

            // lukke print ned igen
            else
            {
                MinimizeScroll();
                cVareNr.IsChecked = false;
                cBeskrivelse.IsChecked = false;
                cAntal.IsChecked = false;
                cStatus.IsChecked = false;
                cTilgang.IsChecked = false;
                cAfgang.IsChecked = false;
                cLokation.IsChecked = false;
                cAmper.IsChecked = false;
                cPin.IsChecked = false;
                cLængde.IsChecked = false;
                cWebNr.IsChecked = false;
                cRFID.IsChecked = false;
                cNote.IsChecked = false;
                cQR.IsChecked = false;
                printInfo.Visibility = Visibility.Collapsed;
                togglePrintVare = true;
            }
        }

        // luk print panel ned
        private void annuller_print(object sender, RoutedEventArgs e)
        {
            cVareNr.IsChecked = false;
            cBeskrivelse.IsChecked = false;
            cAntal.IsChecked = false;
            cStatus.IsChecked = false;
            cTilgang.IsChecked = false;
            cAfgang.IsChecked = false;
            cLokation.IsChecked = false;
            cAmper.IsChecked = false;
            cPin.IsChecked = false;
            cLængde.IsChecked = false;
            cWebNr.IsChecked = false;
            cRFID.IsChecked = false;
            cNote.IsChecked = false;
            cQR.IsChecked = false;
            MinimizeScroll();
            printInfo.Visibility = Visibility.Collapsed;
            togglePrintVare = true;
        }

        // vælg alle check boxe når man trykker på knappen
        private void allSelect_button(object sender, RoutedEventArgs e)
        {
            cVareNr.IsChecked = true;
            cBeskrivelse.IsChecked = true;
            cAntal.IsChecked = true;
            cStatus.IsChecked = true;
            cTilgang.IsChecked = true;
            cAfgang.IsChecked = true;
            cLokation.IsChecked = true;
            cAmper.IsChecked = true;
            cPin.IsChecked = true;
            cLængde.IsChecked = true;
            cWebNr.IsChecked = true;
            cRFID.IsChecked = true;
            cNote.IsChecked = true;
            cQR.IsChecked = true;
        }

        // gem alle vare i en liste i en pdf fil
        private async void Accept_print(object sender, RoutedEventArgs e)
        {
            int pos = 0;
            int progressMaxCount = 0;
            double barCount = 0;
            progressBar.Value = 0;

            // find ud af hvad en vare svar til i %
            double oneVare = (1.00 / VareGrid.Items.Count) * 100;

            List<int> checkboxList = new List<int>();

            // gem hvilke koloner der skal i vare listen
            while (pos < 14)
            {
                switch (pos)
                {
                    case 0:
                        if (cVareNr.IsChecked == true)
                        {
                            checkboxList.Add(0);
                            progressMaxCount += 1;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case 1:
                        if (cBeskrivelse.IsChecked == true)
                        {
                            checkboxList.Add(1);
                            progressMaxCount += 1;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case 2:
                        if (cAntal.IsChecked == true)
                        {
                            checkboxList.Add(2);
                            progressMaxCount += 1;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case 3:
                        if (cStatus.IsChecked == true)
                        {
                            checkboxList.Add(3);
                            progressMaxCount += 1;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case 4:
                        if (cLokation.IsChecked == true)
                        {
                            checkboxList.Add(4);
                            progressMaxCount += 1;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case 5:
                        if (cAmper.IsChecked == true)
                        {
                            checkboxList.Add(5);
                            progressMaxCount += 1;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case 6:
                        if (cTilgang.IsChecked == true)
                        {
                            checkboxList.Add(6);
                            progressMaxCount += 1;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case 7:
                        if (cAfgang.IsChecked == true)
                        {
                            checkboxList.Add(7);
                            progressMaxCount += 1;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case 8:
                        if (cPin.IsChecked == true)
                        {
                            checkboxList.Add(8);
                            progressMaxCount += 1;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case 9:
                        if (cLængde.IsChecked == true)
                        {
                            checkboxList.Add(9);
                            progressMaxCount += 1;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case 10:
                        if (cWebNr.IsChecked == true)
                        {
                            checkboxList.Add(10);
                            progressMaxCount += 1;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case 11:
                        if (cRFID.IsChecked == true)
                        {
                            checkboxList.Add(11);
                            progressMaxCount += 1;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case 12:
                        if (cNote.IsChecked == true)
                        {
                            checkboxList.Add(12);
                            progressMaxCount += 1;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case 13:
                        if (cQR.IsChecked == true)
                        {
                            checkboxList.Add(13);
                            progressMaxCount += 1;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    default:
                        break;
                }
                pos += 1;
            }

            pos = 0;

            // sorter vare listen efter WebshopVareNummer
            List<VareClass> vareListe = cvVare.Cast<VareClass>().ToList();
            List<VareClass> order = vareListe.ToList();

            if (checkboxList.Count != 0)
            {
                // bestem hvor pdf skal gemmes og hvad den skal hedde
                if (VareGrid.Items.Count > 0)
                {
                    SaveFileDialog saveFile = new SaveFileDialog()
                    {
                        Title = "Gem din Fil",
                        Filter = "Adobe PDF(*.pdf) | *.pdf",
                        FileName = "VareListe"
                    };

                    // lave indehold til pdf
                    if (saveFile.ShowDialog() == true)
                    {
                        try
                        {
                            // lav forbindelse til fil
                            FileStream fs = new FileStream(saveFile.FileName, FileMode.Create);

                            // vælg papir format
                            Document document = new Document(PageSize.A4, 25, 20, 30, 30);
                            /*document.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());*/

                            //skriv til pdf fil
                            PdfWriter writer = PdfWriter.GetInstance(document, fs);

                            // fyld pdf egenskab ud
                            document.AddAuthor("Femu");
                            document.AddKeywords("Liste med vare til et event");
                            document.AddSubject("Vare Liste");
                            document.AddTitle(saveFile.FileName);

                            //gem en font
                            BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

                            iTextSharp.text.Font Headerfont = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.BOLD);
                            iTextSharp.text.Font font = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.NORMAL);

                            // lav tabeller
                            PdfPTable headerTable = new PdfPTable(checkboxList.Count);
                            headerTable.DefaultCell.Padding = 2;
                            headerTable.WidthPercentage = 100;
                            headerTable.HorizontalAlignment = Element.ALIGN_LEFT;
                            PdfPTable vareTable = new PdfPTable(checkboxList.Count);
                            vareTable.DefaultCell.Padding = 2;
                            vareTable.WidthPercentage = 100;
                            vareTable.HorizontalAlignment = Element.ALIGN_LEFT;

                            // tilføj indehold til tabeller
                            int cPos = 0;
                            int rPos = 0;

                            // til header til tabel
                            while (cPos < checkboxList.Count)
                            {
                                PdfPCell cell = new PdfPCell(new Phrase(VareGrid.Columns[checkboxList[cPos]].Header.ToString(), Headerfont));
                                headerTable.AddCell(cell);
                                cPos += 1;
                            }

                            workDone.Content = "0% ud af 100% er færdig";

                            // sorter hvilken indehold må komme ind i tabel

                            progressStack.Visibility = Visibility.Visible;
                            foreach (VareClass i in order)
                            {
                                while (pos < checkboxList.Count)
                                {
                                    switch (checkboxList[pos])
                                    {
                                        case 0:
                                            PdfPCell VareNr = new PdfPCell(new Phrase(order[rPos].VareNr, font));
                                            vareTable.AddCell(VareNr);
                                            break;

                                        case 1:
                                            PdfPCell beskrivelse = new PdfPCell(new Phrase(order[rPos].Beskrivelse, font));
                                            vareTable.AddCell(beskrivelse);
                                            break;

                                        case 2:
                                            PdfPCell Antal = new PdfPCell(new Phrase(order[rPos].Antal.ToString(), font));
                                            vareTable.AddCell(Antal);
                                            break;

                                        case 3:
                                            PdfPCell Status = new PdfPCell(new Phrase(order[rPos].Status, font));
                                            vareTable.AddCell(Status);
                                            break;

                                        case 4:
                                            PdfPCell VareLokation = new PdfPCell(new Phrase(order[rPos].VareLokation, font));
                                            vareTable.AddCell(VareLokation);
                                            break;

                                        case 5:
                                            PdfPCell Ampere = new PdfPCell(new Phrase(order[rPos].Ampere, font));
                                            vareTable.AddCell(Ampere);
                                            break;

                                        case 6:
                                            PdfPCell Tilgang = new PdfPCell(new Phrase(order[rPos].Tilgang, font));
                                            vareTable.AddCell(Tilgang);
                                            break;

                                        case 7:
                                            PdfPCell Afgang = new PdfPCell(new Phrase(order[rPos].Afgang, font));
                                            vareTable.AddCell(Afgang);
                                            break;

                                        case 8:
                                            PdfPCell PinNr = new PdfPCell(new Phrase(order[rPos].PinNr, font));
                                            vareTable.AddCell(PinNr);
                                            break;

                                        case 9:
                                            PdfPCell Længde = new PdfPCell(new Phrase(order[rPos].Længde.ToString(), font));
                                            vareTable.AddCell(Længde);
                                            break;

                                        case 10:
                                            PdfPCell WebshopVareNummer;
                                            if (order[rPos].WebshopVareNummer == null || order[rPos].WebshopVareNummer == "")
                                            {
                                                WebshopVareNummer = new PdfPCell(new Phrase("99999", font));
                                            }
                                            else
                                            {
                                                WebshopVareNummer = new PdfPCell(new Phrase(order[rPos].WebshopVareNummer, font));
                                            };
                                            vareTable.AddCell(WebshopVareNummer);
                                            break;

                                        case 11:
                                            PdfPCell RFIDNummer = new PdfPCell(new Phrase(order[rPos].RFIDNummer, font));
                                            vareTable.AddCell(RFIDNummer);
                                            break;

                                        case 12:
                                            PdfPCell Note = new PdfPCell(new Phrase(order[rPos].Note, font));
                                            vareTable.AddCell(Note);
                                            break;

                                        case 13:
                                            PdfPCell QR = new PdfPCell(new Phrase(order[rPos].QR, font));
                                            vareTable.AddCell(QR);
                                            break;

                                        default:
                                            break;
                                    }

                                    pos += 1;
                                }

                                // fyld progress bar ud så man kan se hvor langt processen er
                                if (barCount >= 1.0008)
                                {
                                    progressBar.Value++;
                                    workDone.Content = $"{progressBar.Value}% ud af 100% er færdig";
                                    barCount = 0;
                                    await Task.Delay(15);
                                }
                                barCount = barCount + oneVare;
                                pos = 0;
                                rPos += 1;
                            }

                            //åben dokument
                            document.Open();

                            // tilføj titel
                            iTextSharp.text.Paragraph preface = new iTextSharp.text.Paragraph("Vare Liste\n\n");
                            preface.Alignment = Element.ALIGN_CENTER;
                            // tilføj tekst og tabeller
                            document.Add(new iTextSharp.text.Paragraph(preface));
                            document.Add(headerTable);
                            document.Add(vareTable);

                            // luk forbindelse til fil og luk fil
                            document.Close();
                            writer.Close();
                            fs.Close();

                            // succes besked
                            workDone.Content = "100% ud af 100% er færdig";
                            progressBar.Value = 100;
                            MessageBox.Show("Vare listen er blevet gemt!");

                            progressStack.Visibility = Visibility.Collapsed;
                        }
                        // fejl besked
                        catch (IOException)
                        {
                            progressStack.Visibility = Visibility.Collapsed;
                            MessageBox.Show("Kunne ikke lave/overskrive PDF filen. \nHvis du har filen åben så luk filen og prøv igen.\nHvis filen ikke er åben så tjek om filen er skrivebeskyttet.");
                        }
                    }
                    // fejl besked
                    else
                    {
                        progressStack.Visibility = Visibility.Collapsed;
                        MessageBox.Show("Filen blev ikke gemt");
                    }
                }
                // fejl besked
                else
                {
                    progressStack.Visibility = Visibility.Collapsed;
                    MessageBox.Show("Din liste er tom. Prøv igen med en liste der ikke er tom");
                }
            }
            else
            {
                progressStack.Visibility = Visibility.Collapsed;
                MessageBox.Show("Du har ikke valgt, hvad du vil have i vare listen");
            }
        }

        // tjek om vare felter er udfyldt
        public bool Check_vare()
        {
            if (string.IsNullOrEmpty(VareNr.Text) ||
               string.IsNullOrEmpty(StikTypeTilgang.Text) ||
               string.IsNullOrEmpty(StikTypeAfgang.Text) ||
               string.IsNullOrEmpty(Ampere.Text) ||
               string.IsNullOrEmpty(StatusComboBox.Text) ||
               string.IsNullOrEmpty(Antal.Text) ||
               string.IsNullOrEmpty(VareLokation.Text) ||
               string.IsNullOrEmpty(WebshopVarenummer.Text) ||
               string.IsNullOrEmpty(Længde.Text) ||
               string.IsNullOrEmpty(Salgs.Text))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        // tjek om ekstra felter er udfyldt
        public bool Check_Ekstra()
        {
            if (string.IsNullOrEmpty(ekstraBeskrivelse.Text) ||
               string.IsNullOrEmpty(ekstraAntal.Text) ||
               string.IsNullOrEmpty(ekstraWebshopNummer.Text))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //regex funktion der tjekker om det du taster er et tal
        private void NumberValidation(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9]+");
            e.Handled = re.IsMatch(e.Text); //hvis der er et match, så sætter den det i textbox
        }

        //funktion til at tilføje vare
        public async Task<List<VareClass>> createVare()
        {
            //Tjekker om textboxes er udfyldt
            if (Check_vare())
            {
                MessageBox.Show("Varen er blevet tilføjet");
                //Smider varen ind i vores VareClass
                var new_row =
                    new VareClass(
                        VareNr.Text,
                        Beskrivelse.Text,
                        StikTypeTilgang.Text,
                        StikTypeAfgang.Text,
                        Ampere.Text,
                        StatusComboBox.Text,
                        int.Parse(Antal.Text),
                        VareLokation.Text,
                        PinNummer.Text,
                        int.Parse(Længde.Text),
                        Note.Text,
                        Salgs.Text,
                        WebshopVarenummer.Text,
                        RFIDNummer.Text,
                        null
                    );

                MinimizeScroll();
                myList.Add(new_row);

                //Kalder api til at sende varen op til database
                _ = await vareApi.CreateVareAsync(
                    VareNr.Text,
                    Beskrivelse.Text,
                    StikTypeTilgang.Text,
                    StikTypeAfgang.Text,
                    Ampere.Text,
                    StatusComboBox.Text,
                    int.Parse(Antal.Text),
                    VareLokation.Text,
                    PinNummer.Text,
                    int.Parse(Længde.Text),
                    Note.Text,
                    WebshopVarenummer.Text,
                    RFIDNummer.Text,
                    "",
                    Salgs.Text
                    );

                Clear();
            }

            // fejl besked hvis man ikke har udfyldt alle felter
            else
            {
                MessageBox.Show("Vær gerne sikker på at du har udfyldt felterne " +
                    "(VareNr, " +
                    "StikTypeTilgang, " +
                    "StikTypeAfgang, " +
                    "Ampere, " +
                    "Antal, " +
                    "VareLokation, " +
                    "WebshopVarenummer, " +
                    "Længde, " +
                    "RFIDNummer " +
                    "og Salgs pris) " +
                    "for at tilføje varen");
            }
            return myList;
        }

        //funktion til at tilføje tilbehøre
        public async Task<List<ekstraClass>> createEkstra()
        {
            //Tjekker om textboxes er udfyldt
            if (Check_Ekstra())
            {
                MessageBox.Show("Ekstra er blevet tilføjet.");
                //Smider varen ind i vores VareClass
                var new_row =
                    new ekstraClass(
                        ekstraBeskrivelse.Text,
                        int.Parse(ekstraAntal.Text),
                        ekstraWebshopNummer.Text,
                        null
                    );

                ekstraList.Add(new_row);

                //Kalder api til at sende varen op til database
                _ = await vareApi.CreateEkstraAsync(
                    ekstraBeskrivelse.Text,
                    ekstraAntal.Text,
                    ekstraWebshopNummer.Text
                    );

                ekstraList.Add(new_row);
            }
            // fejl besked hvis man ikke har udfyldt alle felter
            else
            {
                MessageBox.Show("Vær gerne sikker på at du har udfyldt felterne " +
                    "(Beskrivelse, " +
                    "Antal, " +
                    "WebshopVarenummer)");
            }
            return ekstraList;
        }

        // knap til at tilføje en nye vare
        private void Tilføj_Button(object sender, RoutedEventArgs e)
        {
            toggleLokation = true;

            if (String.IsNullOrEmpty(RFIDNummer.Text))
            {
                MessageBoxResult result = MessageBox.Show("RFID nummer står tomt vil du gerne tildele et RFID nummer nu?",
                "RFID NUMMER", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    scanGrid.Visibility = Visibility.Visible;
                }
                else if (result == MessageBoxResult.No)
                {
                    toggleScrolVare = true;
                    _ = createVare();
                    VareGrid.Items.Refresh();
                }
            }
            else
            {
                toggleScrolVare = true;
                _ = createVare();
                VareGrid.Items.Refresh();
            }


        }

        // annuller knap hvis man fortyder at lave/rediger en vare
        private void annullerKnap(object sender, RoutedEventArgs e)
        {

            MinimizeScroll();
            scrollVare.Visibility = Visibility.Collapsed;
            togglePrintVare = true;
            toggleScrolVare = true;
            _ = GetLokationAsync();


            RedigerButton.Visibility = Visibility.Collapsed;
            TilføjVareBtn.Visibility = Visibility.Visible;
        }

        //funtkion til at ligge den redigeret vare op i databasen
        private void Rediger_Button(object sender, RoutedEventArgs e)
        {
            var vareNummer = row.VareNr.ToString();

            if (row != null)
            {
                var result = myList.Find(v => v.VareNr == VareNr.Text && row.VareNr.ToString() != v.VareNr);
                if (result != null)
                {
                    MessageBox.Show("Dette varenummer er allerede i brug. Vælg et andet");
                }
                else
                {

                    // tjek om der blvet lavet nogen ændring
                    if (row.VareNr == VareNr.Text && row.Beskrivelse == Beskrivelse.Text && row.Tilgang == StikTypeTilgang.Text && row.Afgang == StikTypeAfgang.Text &&
                        row.Ampere == Ampere.Text && row.Note == Note.Text && row.Status == StatusComboBox.Text && row.Antal == Int32.Parse(Antal.Text) &&
                        row.VareLokation == VareLokation.Text && row.PinNr == PinNummer.Text && row.Længde == Int32.Parse(Længde.Text) &&
                        row.WebshopVareNummer == WebshopVarenummer.Text && row.RFIDNummer == RFIDNummer.Text && row.Salgspris == Salgs.Text)
                    {
                        //fejl besked
                        MessageBox.Show("Du har ikke lavet nogen ændring");
                    }
                    // opdater vare til at passe med den nye info
                    else
                    {
                        toggleLokation = true;
                        toggleRediger = true;

                        row.VareNr = VareNr.Text;
                        row.Beskrivelse = Beskrivelse.Text;
                        row.Tilgang = StikTypeTilgang.Text;
                        row.Afgang = StikTypeAfgang.Text;
                        row.Ampere = Ampere.Text;
                        row.Note = Note.Text;
                        row.Status = StatusComboBox.Text;
                        row.Antal = Int32.Parse(Antal.Text);
                        row.VareLokation = VareLokation.Text;
                        row.PinNr = PinNummer.Text;
                        row.Længde = Int32.Parse(Længde.Text);
                        row.WebshopVareNummer = WebshopVarenummer.Text;
                        row.RFIDNummer = RFIDNummer.Text;

                        VareGrid.ItemsSource = cvVare;

                        RedigerButton.Visibility = Visibility.Collapsed;
                        TilføjVareBtn.Visibility = Visibility.Visible;

                        _ = EditVareAsync(vareNummer);
                        _ = updatePriceAsync();
                        myList.Clear();
                        _ = GetVareAsync();
                        _ = GetLokationAsync();
                        VareGrid.Items.Refresh();

                        MinimizeScroll();

                        //succes besked
                        MessageBox.Show("Din vare er blevet opdateret");


                        Clear();
                    }
                }
            }
            else
            {
            }
        }

        //dobbeltklik funktion til at kopier et row i datagrid, klar til at redigere
        private void Rediger_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // !-------------------- JESPER ------------------- !
            UnMinimizeScroll();


            row = (VareClass)VareGrid.SelectedItem;

            if (row != null && togglePrintVare)
            {
                toggleLokation = false;
                toggleRediger = false;
                toggleScrolVare = true;

                scrollVare.Visibility = Visibility.Visible;
                TilføjVareBtn.Visibility = Visibility.Collapsed;
                Beskrivelse.Text = row.Beskrivelse;
                StikTypeTilgang.Text = row.Tilgang;
                StikTypeAfgang.Text = row.Afgang;
                Ampere.Text = row.Ampere.ToString();
                Note.Text = row.Note;
                StatusComboBox.Text = row.Status.ToString();
                Antal.Text = row.Antal.ToString();
                VareLokation.Text = row.VareLokation.ToString();
                VareNr.Text = row.VareNr.ToString();
                PinNummer.Text = row.PinNr.ToString();
                Længde.Text = row.Længde.ToString();
                WebshopVarenummer.Text = row.WebshopVareNummer;
                RFIDNummer.Text = row.RFIDNummer;
                Salgs.Text = row.Salgspris;
                RedigerButton.Visibility = Visibility.Visible;
            }
            else
            {
                togglePrintVare = true;
                toggleGridTilbehør = true;
                toggleGridHistorik = true;
                toggleRediger = false;
                toggleScrolVare = true;
                toggleLokation = false;

                printInfo.Visibility = Visibility.Collapsed;
                VareGrid.Visibility = Visibility.Visible;


                scrollVare.Visibility = Visibility.Visible;
                TilføjVareBtn.Visibility = Visibility.Collapsed;
                Beskrivelse.Text = row.Beskrivelse;
                StikTypeTilgang.Text = row.Tilgang;
                StikTypeAfgang.Text = row.Afgang;
                Ampere.Text = row.Ampere.ToString();
                Note.Text = row.Note;
                StatusComboBox.Text = row.Status.ToString();
                Antal.Text = row.Antal.ToString();
                VareLokation.Text = row.VareLokation.ToString();
                PinNummer.Text = row.PinNr.ToString();
                VareNr.Text = row.VareNr.ToString();
                Længde.Text = row.Længde.ToString();
                WebshopVarenummer.Text = row.WebshopVareNummer;
                //RFIDNummer.Text = row.RFIDNummer;
                Salgs.Text = row.Salgspris;
                RedigerButton.Visibility = Visibility.Visible;
                RedigerButton.Visibility = Visibility.Visible;
            }


        }

        // ryd hvad der står i felter
        private void Clear()
        {
            VareNr.Text = "";
            Beskrivelse.Text = "";
            StikTypeTilgang.Text = "";
            StikTypeAfgang.Text = "";
            Ampere.Text = "";
            Note.Text = "";
            Antal.Text = "";
            VareLokation.Text = "";
            PinNummer.Text = "";
            WebshopVarenummer.Text = "";
            Længde.Text = "";
            RFIDNummer.Text = "";
            Salgs.Text = "";
        }

        //funktion til at vise liste over hvilke events en specifik vare har været på
        private async void ShowEventList(object sender, RoutedEventArgs e)
        {
            var row = (VareClass)VareGrid.SelectedItem;
            if (row != null)
            {
                //bool der tjekker på om eventhistorik grid er aktivt eller ej
                if (toggleGridHistorik && toggleGridTilbehør)
                {
                    string varenummer = row.VareNr;
                    MinimizeScroll();
                    printInfo.Visibility = Visibility.Collapsed;
                    toggleScrolVare = true;
                    toggleRediger = true;
                    togglePrintVare = true;

                    //api kald til at sende varenr
                    JArray historik = await vareApi.EventVareHistorik(varenummer);

                    //tjekker om varen har været på mindst 1 event
                    if (historik != null)
                    {
                        eventHistorik.Clear();

                        EventGrid.Visibility = Visibility.Visible;

                        labelSearch.Visibility = Visibility.Visible;
                        txtEventSearchBox.Visibility = Visibility.Visible;

                        VareGrid.Visibility = Visibility.Collapsed;

                        SearchBars.Visibility = Visibility.Collapsed;
                        SearchBarLabels.Visibility = Visibility.Collapsed;

                        //looper historik fra api og ligger det i EventHistorik class
                        foreach (JObject item in historik)
                        {
                            string eventId = (string)item.GetValue("eventId");
                            string eventNavn = (string)item.GetValue("navn");

                            eventHistorik.Add(new EventHistorik(eventId, eventNavn));
                        }

                        EventGrid.ItemsSource = eventHistorik;
                        EventGrid.Items.Refresh();
                        visEventHistorik.Content = "Vis varer";

                        visEkstraTilbehør.Visibility = Visibility.Collapsed;

                        toggleGridHistorik = false;
                    }
                    else
                    {
                        EventGrid.Visibility = Visibility.Collapsed;
                        VareGrid.Visibility = Visibility.Visible;
                        MessageBox.Show("Denne vare har ikke været på nogle events endnu");
                    }
                }
                // man tykker på knappen igen lukker funktion
                else if (!toggleGridHistorik)
                {
                    visEventHistorik.Content = "Vis event historik";
                    visEkstraTilbehør.Visibility = Visibility.Visible;

                    EventGrid.Visibility = Visibility.Collapsed;

                    labelSearch.Visibility = Visibility.Collapsed;
                    txtEventSearchBox.Visibility = Visibility.Collapsed;

                    VareGrid.Visibility = Visibility.Visible;

                    SearchBars.Visibility = Visibility.Visible;
                    SearchBarLabels.Visibility = Visibility.Visible;

                    toggleGridHistorik = true;
                }
                else if (!toggleGridTilbehør)
                {
                    MessageBox.Show("Du kan ikke vælge en vare op hvor du er nu. Gå ud af ekstra tilbehør og prøv igen");
                }
            }

            // fejl besked
            else
            {
                MessageBox.Show("Du har ikke valgt en vare.");
            }
        }

        // liste med ekstra tilbehør
        public void ShowEkstraTilbehør(object sender, RoutedEventArgs e)
        {
            // hvis intet er togglet til
            if (toggleGridTilbehør != false && togglePrintVare != false && toggleGridHistorik && toggleScrolVare)
            {
                //grid variabler
                MinimizeGrid();
                EkstraTilbehørGrid.Visibility = Visibility.Visible;

                // scroll variabler
                scrollEkstra.Visibility = Visibility.Visible;
                scrollVare.Visibility = Visibility.Collapsed;

                VareGrid.Visibility = Visibility.Collapsed;
                EventGrid.Visibility = Visibility.Collapsed;

                //søgebar variabler
                txtEventSearchBox.Visibility = Visibility.Collapsed;
                labelSearch.Visibility = Visibility.Collapsed;
                SearchBars.Visibility = Visibility.Collapsed;
                SearchBarLabels.Visibility = Visibility.Collapsed;

                //knap variabler
                visEkstraTilbehør.Content = "Vis Varer";

                visEventHistorik.Visibility = Visibility.Collapsed;

                toggleGridTilbehør = false;
            }
            // hvis nået er togglet
            else if (toggleGridTilbehør && (toggleScrolVare == false || !togglePrintVare || !toggleGridHistorik))
            {
                MessageBox.Show("Du kan ikke se ekstra tilbehør herfra. Gå ud af event historik og prøv igen.");
            }
            else
            {
                //grid variabler
                MinimizeScroll();
                VareGrid.Visibility = Visibility.Visible;

                EkstraTilbehørGrid.Visibility = Visibility.Collapsed;
                EventGrid.Visibility = Visibility.Collapsed;

                // scroll variabler
                scrollEkstra.Visibility = Visibility.Collapsed;

                //søgebar variabler
                labelSearch.Visibility = Visibility.Collapsed;
                SearchBars.Visibility = Visibility.Visible;
                SearchBarLabels.Visibility = Visibility.Visible;

                //knap variabel
                visEkstraTilbehør.Content = "Vis ekstra tilbehør";
                visEventHistorik.Visibility = Visibility.Visible;

                toggleGridTilbehør = true;
            }
        }

        // søg efter event i vare event historik
        private void txtEventSearch(object sender, TextChangedEventArgs e)
        {
            var filtered = eventHistorik.Where(v =>
                v.EventName.ToLower().StartsWith(txtEventSearchBox.Text.ToLower())
            );

            EventGrid.ItemsSource = filtered;
        }

        // start et api kald der tilføjer et nyt ekstra tilbehør
        private void nytEkstra_Click(object sender, RoutedEventArgs e)
        {
            _ = createEkstra();
            ekstraClear();
            EkstraTilbehørDataGrid.Items.Refresh();
        }

        // en knap der annuller enten at rediger eller lave et tilbehøre alt efter hvad man er i gang med
        private void annullerEkstra_Click(object sender, RoutedEventArgs e)
        {
            //grid variabler
            MinimizeScroll();
            VareGrid.Visibility = Visibility.Visible;

            EkstraTilbehørGrid.Visibility = Visibility.Collapsed;
            EventGrid.Visibility = Visibility.Collapsed;

            // scroll variabler
            scrollEkstra.Visibility = Visibility.Collapsed;

            //søgebar variabler
            labelSearch.Visibility = Visibility.Collapsed;
            SearchBars.Visibility = Visibility.Visible;
            SearchBarLabels.Visibility = Visibility.Visible;

            //knap variabel
            visEkstraTilbehør.Content = "Vis ekstra tilbehør";
            visEventHistorik.Visibility = Visibility.Visible;

            toggleGridTilbehør = true;
        }

        // dobbel klik et tilbehøre for at starte med at rediger den
        private void ekstra_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var row = (ekstraClass)EkstraTilbehørDataGrid.SelectedItem;

            if (row != null)
            {
                ekstraBeskrivelse.Text = row.Beskrivelse;
                ekstraAntal.Text = row.Antal.ToString();
                ekstraWebshopNummer.Text = row.WebshopVareNummer;
            }

            redigerEkstra.Visibility = Visibility.Visible;
            nytEkstra.Visibility = Visibility.Collapsed;
        }

        // sletter tilbehør felter
        private void ekstraClear()
        {
            ekstraBeskrivelse.Text = "";
            ekstraAntal.Text = "";
            ekstraWebshopNummer.Text = "";
        }

        // opdater det valgte tilbehør med den nye data fra felterne
        private void redigerEkstra_Click(object sender, RoutedEventArgs e)
        {
            ekstraClass row = (ekstraClass)EkstraTilbehørDataGrid.SelectedItem;

            // opdater vare til at passe med den nye info
            if (row != null)
            {
                row.Beskrivelse = ekstraBeskrivelse.Text;
                row.Antal = Int32.Parse(ekstraAntal.Text);
                row.WebshopVareNummer = ekstraWebshopNummer.Text;

                EkstraTilbehørDataGrid.Items.Refresh();
                EkstraTilbehørDataGrid.ItemsSource = ekstraList;
            }

            tilbehørId = row.Id;
            redigerEkstra.Visibility = Visibility.Collapsed;
            nytEkstra.Visibility = Visibility.Visible;

            _ = EditEkstraAsync(tilbehørId);

            ekstraClear();
        }

        // filtre alle vare fra der ikke har en vare nr. der hedder det samme som i textboxen når man trykker enter
        private void VareNrEnter(object sender, KeyEventArgs e)
        {
            List<VareClass> data = myList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.VareNr.ToLower().Equals(VareNrBox.Text.ToLower()));
            }

            VareGrid.ItemsSource = data;
        }

        // filtre alle vare fra der ikke har en Beskrivelse der hedder det samme som i textboxen når man trykker enter
        private void beskrivelseEnter(object sender, KeyEventArgs e)
        {
            List<VareClass> data = myList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.Beskrivelse.ToLower().Equals(beskrivelseBox.Text.ToLower()));
            }

            VareGrid.ItemsSource = data;
        }

        // filtre alle vare fra der ikke har en Antal der hedder det samme som i textboxen når man trykker enter
        private void AntalEnter(object sender, KeyEventArgs e)
        {
            List<VareClass> data = myList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.Antal.Equals(Int32.Parse(AntalBox.Text)));
            }

            VareGrid.ItemsSource = data;
        }

        // filtre alle vare fra der ikke har en Lokation der hedder det samme som i textboxen når man trykker enter
        private void LokationEnter(object sender, KeyEventArgs e)
        {
            List<VareClass> data = myList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.VareLokation.ToLower().Equals(LokationBox.Text.ToLower()));
            }

            VareGrid.ItemsSource = data;
        }

        // filtre alle vare fra der ikke har en Ampere der hedder det samme som i textboxen når man trykker enter
        private void AmpereEnter(object sender, KeyEventArgs e)
        {
            List<VareClass> data = myList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.Ampere.ToLower().Equals(AmpereBox.Text.ToLower()));
            }

            VareGrid.ItemsSource = data;
        }

        // filtre alle vare fra der ikke har en Tilgang der hedder det samme som i textboxen når man trykker enter
        private void TilgangEnter(object sender, KeyEventArgs e)
        {
            List<VareClass> data = myList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.Tilgang.ToLower().Equals(TilgangBox.Text.ToLower()));
            }

            VareGrid.ItemsSource = data;
        }

        // filtre alle vare fra der ikke har en Afgang der hedder det samme som i textboxen når man trykker enter
        private void AfgangEnter(object sender, KeyEventArgs e)
        {
            List<VareClass> data = myList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.Afgang.ToLower().Equals(AfgangBox.Text.ToLower()));
            }

            VareGrid.ItemsSource = data;
        }

        // filtre alle vare fra der ikke har en PinNr der hedder det samme som i textboxen når man trykker enter
        private void PinEnter(object sender, KeyEventArgs e)
        {
            List<VareClass> data = myList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.PinNr.ToLower().Equals(PinBox.Text.ToLower()));
            }

            VareGrid.ItemsSource = data;
        }

        // filtre alle vare fra der ikke har en Længde der hedder det samme som i textboxen når man trykker enter
        private void LængdeEnter(object sender, KeyEventArgs e)
        {
            List<VareClass> data = myList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.Længde.Equals(Int32.Parse(LængdeBox.Text)));
            }

            VareGrid.ItemsSource = data;
        }

        // filtre alle vare fra der ikke har en WebshopVareNummer der hedder det samme som i textboxen når man trykker enter
        private void WebEnter(object sender, KeyEventArgs e)
        {
            List<VareClass> data = myList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.WebshopVareNummer.ToLower().Equals(webBox.Text.ToLower()));
            }

            VareGrid.ItemsSource = data;
        }

        // filtre alle vare fra der ikke har en RFIDNummer der hedder det samme som i textboxen når man trykker enter
        private void RFIDEnter(object sender, KeyEventArgs e)
        {
            List<VareClass> data = myList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.RFIDNummer.ToLower().Equals(RFIDBox.Text.ToLower()));
            }

            VareGrid.ItemsSource = data;
        }

        // filtre alle vare fra der ikke har en Note der hedder det samme som i textboxen når man trykker enter
        private void NoteEnter(object sender, KeyEventArgs e)
        {
            List<VareClass> data = myList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.Note.ToLower().Equals(NoteVareBox.Text.ToLower()));
            }

            VareGrid.ItemsSource = data;
        }

        // filtre alle vare fra der ikke har en Note der hedder det samme som i textboxen når man trykker enter
        private void QREnter(object sender, KeyEventArgs e)
        {
            List<VareClass> data = myList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.QR.ToLower().Equals(QRBox.Text.ToLower()));
            }

            VareGrid.ItemsSource = data;
        }

        // filtre alle vare fra der ikke har en Note der hedder det samme som i textboxen når man trykker enter
        private void SalgsEnter(object sender, KeyEventArgs e)
        {
            List<VareClass> data = myList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.Salgspris.ToLower().Equals(SalgsBox.Text.ToLower()));
            }

            VareGrid.ItemsSource = data;

        }

        public bool Check_kontainer()
        {
            if (string.IsNullOrEmpty(kontainerBox.Text) ||
               string.IsNullOrEmpty(vareNrStartBox.Text) ||
               string.IsNullOrEmpty(vareNrSlutBox.Text))
            {
                return false;
            }
            else
            {
                if(vareNrStartBox.Text == "0" || vareNrSlutBox.Text == "0")
                {

                    MessageBox.Show("Du må ikke brugre 0 som dit vare nr start/slut");
                    return false;
                }
                else
                {
                    return true;
                }

            }
        }

        public async Task<List<containerClass>> createkontainer()
        {
            //Tjekker om textboxes er udfyldt
            if (Check_kontainer())
            {
                MessageBox.Show("kontainer er blevet tilføjet.");
                //Smider varen ind i vores VareClass
                var new_row =
                    new containerClass(
                        0,
                        kontainerBox.Text,
                        "",
                        int.Parse(vareNrStartBox.Text),
                        int.Parse(vareNrSlutBox.Text)

                    );

                containerListe.Add(new_row);

                //Kalder api til at sende varen op til database
                _ = await vareApi.createLokation(
                    kontainerBox.Text,
                    "",
                    int.Parse(vareNrStartBox.Text),
                    int.Parse(vareNrSlutBox.Text)
                    );

                containerListe.Add(new_row);
            }
            // fejl besked hvis man ikke har udfyldt alle felter
            else
            {
                MessageBox.Show("Vær gerne sikker på at du har udfyldt felterne " +
                    "(Kontainer navn, " +
                    "Vare nr. start, " +
                    "Vare nr. slut)");
            }
            return containerListe;
        }

        private void nyKontainerBtn_Click(object sender, RoutedEventArgs e)
        {
            _ = createkontainer();

            kontainerStack.Children.Clear();
            _ = GetLokationAsync();




        }

        private void annullerKontainerBtn_Click(object sender, RoutedEventArgs e)
        {
            nyKontainerStack.Visibility = Visibility.Collapsed;
            kontainerBox.Text = "";
        }

        private void nyKontainer_Click(object sender, RoutedEventArgs e)
        {
            _ = GetLokationAsync();
            nyKontainerStack.Visibility = Visibility.Visible;
            kontainerBox.Focus();


        }

        private void editKontainerBtn_Click(object sender, RoutedEventArgs e)
        {
            bool done = false;
            int pos = 0;
            containerClass valgteKontainer = containerListe[0];

            while (!done)
            {
                if (containerListe[pos].location == containerNavn.Content.ToString())
                {
                    valgteKontainer = containerListe[pos];
                    done = true;
                }
                else
                {
                    pos += 1;
                }
            }

            done = false;
            pos = 0;



            if (!String.IsNullOrEmpty(editVareNrStartBox.Text) && !String.IsNullOrEmpty(editVareNrSlutBox.Text))
            {

                while(pos < containerListe.Count && !done)
                {
                    if (containerListe[pos] != valgteKontainer && Int32.Parse(editVareNrStartBox.Text) >= containerListe[pos].VareNrStart && Int32.Parse(editVareNrStartBox.Text) <= containerListe[pos].VareNrSlut)
                    {
                        MessageBox.Show("Din nye container range ligger i container " + containerListe[pos].location + "ses range, skriv en ny range der ikke ligger i en anden contaiers range");
                        done = true;
                    }
                    else if (editVareNrStartBox.Text == "0" || editVareNrSlutBox.Text == "0")
                    {
                        MessageBox.Show(" Du må ikke bruge 0 som dit vare nr. start/slut");
                        done = true;
                    }
                    else
                    {
                        pos += 1;
                    }

                }

            }
            else
            {
                MessageBox.Show("Du skal skrive i begge felter");
            }

            if (!done)
            {
                editKontainerStack.Visibility = Visibility.Collapsed;
                nyKontainerStack.Visibility = Visibility.Collapsed;
                _ = EditLokationAsync();

                kontainerStack.Children.Clear();
                _ = GetLokationAsync();
                done = true;
            }



        }

        private void editAnnullerKontainerBtn_Click(object sender, RoutedEventArgs e)
        {
            editKontainerStack.Visibility = Visibility.Collapsed;
        }

        private void startScanButton_Click(object sender, RoutedEventArgs e)
        {
            if(scannerOnOff == false)
            {
                Task.Run(() =>
                 ScanKnapEvent() // Køre hele skan funktionen async
            );

                startScanButton.Content = "Sluk scanner";
            }
            else
            {
                connectionInterface.disconnect();
                connectionInterface.Dispose();

                scannerOnOff = false;
                startScanButton.Content = "Start scanner";
            }

        }

        private void gemScanVare_Click(object sender, RoutedEventArgs e)
        {

            // Når man forlader vinduet og slukker scanneren skal muligheden for at ændre scanner IP og scanner styrke igen

            if (scannedID == null)
            {
                _ = MessageBox.Show("Intet ID scannet");
                if (connectionInterface == null) { return; }

                connectionInterface.disconnect();
                connectionInterface.Dispose();
            }
            else
            {
                //smid scannedID til api eller hvad end det skal bruges til her
                ScanBox.Text = scannedID;
                RFIDNummer.Text = ScanBox.Text;
                toggleScrolVare = true;
                _ = createVare();
                connectionInterface.disconnect();
                connectionInterface.Dispose();
                scanGrid.Visibility = Visibility.Collapsed;
            }

        }



        // scanPowerValue = dBm * 10 (Scanner Styrke)
        // https://en.wikipedia.org/wiki/DBm
        // Må have en maks værdi på 300 (30 dBm) og en mindste værdi på 0, (unsigned integer) men det ville ikke give mening at gå under 10 da det bliver divideret alligevel
        // så det mindste vil altid værre 1 dBm
        uint scanPowerValue = 200;
        // IP Til scanner, så den nemt kan ændres af brugeren skulle det blive nødvendigt
        string scannerIP = "192.168.2.60";

        void ScanKnapEvent()
        {
            // Dispatcher så vi kan ænder GUI elementer selvom vi køre op en anden tråd
            // Skjuler knapperne Ændre IP og Ændre styrke, og starer loading skærm

            Dispatcher.Invoke(() =>
            {
                StarterScannerBorder.Visibility = Visibility.Visible;
            });

            bool fail = false;
            connectionInterface = new ConnectionInterface(scannerIP); //Opretter vores scanner variabel
            try
            {
                connectionInterface.connect();
                scannerOnOff = true;
            }
            catch (CS203EngineException)
            {
                fail = true;
                _ = MessageBox.Show("Kunne ikke oprette forbindelse til Scanner." +
                    "\nTjek at scanneren er forbundet til Computeren og at scanneren er tændt, RFID lampen burde lyse og Network LED burde blinke regelmæssigt" +
                    "\nTjek at din IP er statisk:" +
                    "\nåben dit Kontrol Panel, Netværk og Internet og derefter Netværk og Delingscenter." +
                    "\nUnder Netværk og Delingscenter vælg Ethernet forbindelsen og klik: Egenskaber og derefter Egenskaber under TCP/IPv4" +
                    "\nVælg: Brug den følgende IP Adresse, i det første felt indtast en IP adresse f. eks. 192.168.2.30 (Adressen skal indeholde 192.168.2 for at forbindelse kan etableres) Klik OK og luk vinduerne og prøv igen");
            }
            if (!fail)
            {
                Dispatcher.Invoke(() =>
                {
                    StarterScannerBorder.Visibility = Visibility.Collapsed; // Fjerner Loadingskærm
                });

                MessageBox.Show("klar");
                Antenna antenna = new Antenna(connectionInterface, 0); // Hvad for en Antenne der skal bruges, vi bruger altid 0
                connectionInterface.personList = new List<string>();
                connectionInterface.OnTagRead += ConnectionInterface_OnTagRead; // Funktion der bliver kørt ved hvert skan

                antenna.enabled = true;
                connectionInterface.setPower(0, scanPowerValue); // Sætter styrken på scanneren
                connectionInterface.startListening(); //starter scanneren
                connectionInterface.getPower(0); // console.writeliner antenna styken
                Console.WriteLine(connectionInterface.getPower(0)); // Samme som ovenfor
            }
        }


        private void ConnectionInterface_OnTagRead(object sender, TagReadEventArgs e)
        {
            scannedID = e.tagID;
            this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                ScanBox.Text = scannedID;
            }));
            // Beep ved Skan
            CSLibrary.Tools.Sound.Beep(700, 200);
        }

        private void scanAnuller_Click(object sender, RoutedEventArgs e)
        {
            if(scannerOnOff == true)
            {
                connectionInterface.disconnect();
                connectionInterface.Dispose();
                scannerOnOff = false;

            }
            else
            {
                scanGrid.Visibility = Visibility.Collapsed;
            }

        }

        private void TilbageKnapEvent_MouseLeave(object sender, MouseEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            TilbageKnapEvent.Foreground = (Brush)bc.ConvertFrom("#FFF");
        }

        private void TilbageKnapEvent_MouseEnter(object sender, MouseEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            TilbageKnapEvent.Foreground = (Brush)bc.ConvertFrom("#CCC");
        }

        private void TilbageKnapEvent_MouseDown(object sender, MouseButtonEventArgs e)
        {
            goBack_Click(sender, e);
        }
    }
}