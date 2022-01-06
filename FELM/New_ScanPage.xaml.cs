using CS203Engine;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Extensions.Logging;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using ComboBox = System.Windows.Controls.ComboBox;
using FrameworkElement = System.Windows.FrameworkElement;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Label = System.Windows.Controls.Label;
using MessageBox = System.Windows.MessageBox;
using Page = System.Windows.Controls.Page;
using Style = System.Windows.Style;
using TextBox = System.Windows.Controls.TextBox;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace FELM
{
    /// <summary>
    /// Interaction logic for New_ScanPage.xaml
    /// </summary>
    ///
    public delegate void DataTransferDelegate(string data, string hejsa, string tryhatss);
    public partial class New_ScanPage : Page
    {

        List<VareClass> GennemgangRfid = new List<VareClass>(); // til at hente alle vare for at holde rfidtag op mod alle vare. -

        List<VareClass> vareList = new List<VareClass>();

        List<EventVare> NewList = new List<EventVare>(); // midlertidig!!

        List<EventData> EventsList = new List<EventData>();

        List<string> lokationsList = new List<string>();

        List<EventVare> AfleveretList = new List<EventVare>();

        List<TilbehørClass> ScannetTilbehørList = new List<TilbehørClass>();

        //List<string> compareList = new List<string>();

        IDictionary<string, EventData> numberNames = new Dictionary<string, EventData>();

        List<ekstraClass> ekstraList = new List<ekstraClass>(); // skal væk kun til første stadige af tilbehør oprettelse

        Afleveringsskærme AfWindow = new Afleveringsskærme();

        List<eventUsersClass> eventBruger = new List<eventUsersClass>();

        List<queueClass> queue = new List<queueClass>();

        List<EventEkstraClass> eventTilbehør = new List<EventEkstraClass>();

        List<TilbehørClass> udlåntTilbehør = new List<TilbehørClass>();

        bool EventClicked = false;

        bool nyBruger = false;
        bool erScannerAktiv = false;
        bool udlejStatus = false;
        bool afleverStatus = false;

        AddUserData bruger;
        bool userSet = false;
        bool brugerFundet;
        bool needUserID;
        bool minimer = false;
        public bool TjekkerSpan = false;
        private int currentEventid;
        private string currentLokation;
        private string currentEvent;

        public bool UdlejningBool = false;
        public bool AfleveringBool = false;


        SolidColorBrush userColor = new SolidColorBrush();



        API api = new API();
        string eventstr = " ";
        Button start_stop_ScannerBtn = new Button();
        Button UdlejOgAflevere = new Button();


        public DataTransferDelegate del;

        // JESPER VARIABLER!!!
        // scanPowerValue = dBm * 10 (Scanner Styrke)
        // https://en.wikipedia.org/wiki/DBm
        // Må have en maks værdi på 300 (30 dBm) og en mindste værdi på 0, (unsigned integer) men det ville ikke give mening at gå under 10 da det bliver divideret alligevel
        // så det mindste vil altid værre 1 dBm
        uint scanPowerValue = 200;
        // IP Til scanner, så den nemt kan ændres af brugeren skulle det blive nødvendigt
        string scannerIP = "192.168.2.60";

        public New_ScanPage()
        {
            //
            InitializeComponent();
            //SetAfleveringsSkærm();
            //Start_Scanner();
            udlejOgAfleverBtn();
            _ =GetEkstraAsync();
            ManueltVAreTxt.KeyDown += TilføjVaremanuelt;

            Scan_Page_DataGridUdlej.ItemsSource = NewList;
            //TilbeHørDataGrid.ItemsSource = ScannetTilbehørList;

        }

        public void Onload(object sender, RoutedEventArgs e)
        {
            EventClicked = false;
            if (Globalvar.UpdateEventListe == true)
            {
                EventsList.Clear();
                EventStackPanel.Children.Clear();
                GetAktivEvents();
            }
            else
            {
                GetAktivEvents();
            }
        }


        // ----------------------------------------------------------------------------------------------------api kald der henter events og deres data -----------------------------------------------------------------------------------
        private async void GetEvents()                   /// api kald til at retunere en fyldt event liste
        {
            //api kald hvor resultatet bliver gemt som et JArray som vi kalder events
            JArray events = await api.getEvents();

            //For hver item i events henter vi det information vi vil have
            foreach (JObject item in events)
            {
                string name = (string)item.GetValue("EventName");
                int ID = (int)item.GetValue("EventId");
                string aktiv = (string)item.GetValue("EventAktiv");
                string brugerId = (string)item.GetValue("EventBrugerId");


                // Tilføj events til liste
                EventsList.Add(new EventData(name, ID, aktiv, brugerId));

            }
            tryhard();
            //return EventsList;
        }

        private async void GetAktivEvents()                   /// api kald til at retunere en fyldt event liste
        {
            //api kald hvor resultatet bliver gemt som et JArray som vi kalder events
            JArray events = await api.getEvents();

            //For hver item i events henter vi det information vi vil have
            foreach (JObject item in events)
            {
                string name = (string)item.GetValue("EventName");
                int ID = (int)item.GetValue("EventId");
                string aktiv = (string)item.GetValue("EventAktiv");
                string brugerId = (string)item.GetValue("EventBrugerId");


                // Tilføj events til liste
                EventsList.Add(new EventData(name, ID, aktiv, brugerId));

            }
            tryhard();

            aktivCheck.IsChecked = true;
            //return EventsList;
        }

        private async Task<List<EventEkstraClass>> GetEventTilbehør(string id, string eventnavn)                   /// api kald til at retunere en fyldt event liste
        {
            eventTilbehør.Clear();
            //api kald hvor resultatet bliver gemt som et JArray som vi kalder events
            JArray events = await api.GetEventEkstraAsync();

            //For hver item i events henter vi det information vi vil have
            foreach (JObject item in events)
            {
                
                int ID = (int)item.GetValue("Id");
                string Beskrivelse = (string)item.GetValue("Beskrivelse");
                string Antal = (string)item.GetValue("Antal");
                string EventId = (string)item.GetValue("EventId");
                string EventLokation = (string)item.GetValue("EventLokation");
                string Status = (string)item.GetValue("Status");


                // Tilføj events til liste
                if(id == EventId)
                {
                    eventTilbehør.Add(new EventEkstraClass(ID, Beskrivelse, Antal, EventId, EventLokation, Status));
                }
            }

            udlåntTilbehør.Clear();

            foreach (EventEkstraClass et in eventTilbehør)
            {
                udlåntTilbehør.Add(new TilbehørClass(et.Beskrivelse, Int32.Parse(et.Antal), et.EventLokation, eventnavn, et.Status));
            }

            Scan_Page_TilbehørGrid.ItemsSource = udlåntTilbehør;

            Scan_Page_TilbehørGrid.Items.Refresh();

            return eventTilbehør;
            //return EventsList;
        }
        private async Task<AddUserData> GetUsers(string rfid)
        {
            //api kald hvor resultatet bliver gemt som et JArray som vi kalder useres
            JArray Users = await api.getUsers();
            nyBruger = false;
            brugerFundet = false;

            //For hveritem i Users henter vi det information vi vil have
            foreach (JObject item in Users)
            {
                string username = (string)item.GetValue("username");
                string name = (string)item.GetValue("name");
                string password = null;
                string mail = (string)item.GetValue("mail");
                string color = (string)item.GetValue("color");
                string type = (string)item.GetValue("type");
                string adresse = (string)item.GetValue("adresse");
                string nr = (string)item.GetValue("husnr");
                int postnr = (int)item.GetValue("postnr");
                int telefonnummer = (int)item.GetValue("tlfnr");
                string rfidNummer = (string)item.GetValue("rfidNummer");
                string checkBrugerRFID;


                if(rfid == rfidNummer)
                {
                    brugerFundet = true;
                    if (bruger == null)
                    {
                        checkBrugerRFID = "0";
                    }
                    else
                    {
                        checkBrugerRFID = bruger.Rfidnummer;
                    }
                    
                    if (checkBrugerRFID == rfidNummer)
                    {
                        nyBruger = false;
                    }
                    else
                    {
                        nyBruger = true;
                        bruger = new AddUserData(username, name, password, mail, color, type, adresse, nr, postnr, telefonnummer, rfidNummer);
                        break;
                    }
                }
                else
                {
                }
            }

            return bruger;
        }

        private async void GetEventsUsers(string eventName)
        {
            eventBruger.Clear();
            //api kald hvor resultatet bliver gemt som et JArray som vi kalder events
            JArray events = await api.GetEventUsers();

            //For hver item i events henter vi det information vi vil have
            foreach (JObject item in events)
            {
                
                int ID = (int)item.GetValue("Id");
                string brugerNavn = (string)item.GetValue("Username");
                string eventnavn = (string)item.GetValue("EventName");


                // Tilføj events til liste
                if(eventName == eventnavn)
                {
                    eventBruger.Add(new eventUsersClass(ID, brugerNavn, eventnavn));
                }
                
            }
            //return EventsList;
        }
        // ----------------------------------------------------------------------------------------------------api kald der henter lokationer på events og deres data ------------------------------------------------------------------------
        List<string> lokationIDList = new List<string>();
        List<string> lokationEventIDList = new List<string>();
        List<int> lokationEditIDList = new List<int>();
        int MidlerTidligtEventId;
        private async void GetLocation(int EventID, object sender)        /// api kald til at retunere en fyldt lokations liste
        {
            Button btn = (Button)sender;
            EventNameLaben.Content = string.Format("Event: {0}", btn.Content);

            //api kald hvor resultatet bliver gemt som et JArray som vi kalder events
            JArray lokations = await api.GetEventLokation(EventID);
            if (lokations != null)
            {
                EventScroll.Visibility = Visibility.Collapsed; // gemmer eventscroll så der er klar til Lokationenerne
                LokationScroll.Visibility = Visibility.Visible;
                //For hver item i events henter vi det information vi vil have
                foreach (JObject item in lokations)
                {
                    string name = (string)item.GetValue("Lokation");
                    string lokationsID = (string)item.GetValue("EventLokationId");
                    string eventID = (string)item.GetValue("EventLokationEventId");



                    // Tilføj events til liste
                    lokationsList.Add(name);
                    lokationIDList.Add(lokationsID);
                    lokationEventIDList.Add(eventID);
                }

                EventNameLaben.Content = string.Format("Event: {0}", btn.Content);
                //int eventId = int.Parse(btn.Name.Replace("c", ""));
                foreach (var sted in EventsList
                ) // Søger igennem eventliste og opretter dynamisk nye knapper LokationStackPanel.Children
                {
                    if (sted.Name == btn.Content)
                    {
                        for (int i = 0; i < lokationsList.Count; i++)
                        {
                            Button LokationsKnapper = new Button();
                            Button LokationsTextBoxNot = new Button();

                            LokationsTextBoxNot.Name = RegexSave(lokationsList[i]);
                            LokationsTextBoxNot.Content = RegexSave(lokationsList[i]);
                            // Pass Lokation ID til LokationsTextBoxNot som i et tag
                            //LokationsTextBoxNot.Tag = lokationIDList[i];
                            LokationsTextBoxNot.Tag = i;
                            System.Console.WriteLine(sted.ID);

                            lokationEditIDList.Add(i);

                            LokationsTextBoxNot.Height = 35;
                            LokationsTextBoxNot.FontSize = 26;
                            LokationsTextBoxNot.Margin = new Thickness(20, 10, 10, 10); LokationsKnapper.Content = lokationsList[i];

                            LokationsKnapper.Name = RegexSave(lokationsList[i]);


                            object resource =
                                Application.Current
                                    .FindResource(
                                        "EandLBtn"); //Tager Stylen fra App.xaml der hedder NewScnbtn og tilføre alle styling til knappen.
                            if (resource != null && resource.GetType() == typeof(Style))
                            {
                                LokationsKnapper.Style = (Style)resource;
                            }

                            LokationStackPanel.Children.Add(LokationsKnapper);
                            LokationsKnapper.Click += LokationsKnapper_Click;
                            LokationsTextBoxNot.Click += LokationsTextBoxNot_Click;
                            EditLokationStack.Children.Add(LokationsTextBoxNot);
                        }
                    }

                }

                EventsTxt.Content = "Lokationer";
                TilbageKnap();
            }
            else
            {
                // spørge om man vil tilføje en lokation ved tryk på de events der ikke har, her har man mulighedne for at sig ja eller nej.
                if (MessageBox.Show("Der er ikke nogen Lokation på dette event. Tilføj en lokation!", "Manglende Lokation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {

                }
                else
                {
                    // Kontrollere om man har valgt et event før man kan tilføje til det.
                    if (EventNameLaben.Content.ToString() != "Event:")
                    {
                        AddLokationQuick.Visibility = Visibility.Visible;
                        QuickAddLocationTxtBox.Focus();
                        MidlerTidligtEventId = EventID;
                    }
                    else
                    {
                        MessageBox.Show("Du skal vælge et Event før du kan tilføje en Lokation til den!");
                    }
                }
            }


        }
        string selectedLokationID;
        string selectedEventID;
        int selectedEditLokationID;

        void LokationsTextBoxNot_Click(object sender, RoutedEventArgs e)
        {
            Button Btn = (Button)sender;
            AddTxt.Text = Btn.Name;
            //selectedLokationID = Btn.Tag.ToString();
            selectedLokationID = lokationIDList[(int)Btn.Tag];
            selectedEventID = lokationEventIDList[(int)Btn.Tag];
            selectedEditLokationID = (int)Btn.Tag;

            System.Console.WriteLine(selectedLokationID);
            System.Console.WriteLine(selectedEventID);
            // LokationIGuess = Btn.Name;
            // LokationsIDChaneg = Btn.Tag.ToString();

            /*
          string LokationsIDChaneg;
string EventIDChange;
string LokationIGuess;
 */
        }

        //-----------------------------------------------------------------------------------Opretter knapper til events  on load----------------------------------------------------------------------------------------------------------------------------

        public void tryhard()
        {
            int i = 0;
            foreach (var item in EventsList)
            {
                Button Newbtn = new Button();
                Newbtn.Content = item.Name;
                
                string replace = 'c' + item.ID.ToString();
                replace = replace.Replace(" ", "");

                Newbtn.Name = replace;

                object resource = Application.Current.FindResource("EandLBtn"); //Tager Stylen fra App.xaml der hedder NewScnbtn og tilføre alle styling til knappen.

                if (resource != null && resource.GetType() == typeof(Style))
                    Newbtn.Style = (Style)resource;

                Newbtn.Click += (s, EventArgs) => 
                {
                    _ = GetEventTilbehør(item.ID.ToString(), Newbtn.Content.ToString());

                    

                    GetEventsUsers(Newbtn.Content.ToString());

                    int eventId = int.Parse(Newbtn.Name.Replace("c", ""));


                    currentEventid = eventId;
                    currentEvent = item.Name;
                    GetLocation(eventId, Newbtn);
                    GetEventVare(eventId, "Udlånt");
                };

                EventStackPanel.Children.Add(Newbtn);
            }
            GetBrugteEventsMedVare();

        }

        public async void GetBrugteEventsMedVare()
        {
            if (EventClicked) { return; }
            for (int i = 0; i < EventStackPanel.Children.Count; i++)
            {
                if (EventClicked) { return; }
                var nuIDBarn = EventStackPanel.Children[i];
                string nuID = (nuIDBarn as Button).Name.Replace("c", "");
                Console.WriteLine(nuID);
                JArray eventVare = await api.GetEventVare(nuID, "Udlånt");
                if (eventVare != null)
                {
                    //(EventStackPanel.Children[i] as Button).FontSize = 18;
                    (EventStackPanel.Children[i] as Button).FontWeight = FontWeights.Bold;

                }
            }
        }

        public async void brugerIdGetBrugteEventsMedVare()
        {
            
            for (int i = 0; i < EventStackPanel.Children.Count; i++)
            {
                
                var nuIDBarn = EventStackPanel.Children[i];
                string nuID = (nuIDBarn as Button).Name.Replace("c", "");
                Console.WriteLine(nuID);
                JArray eventVare = await api.GetEventVare(nuID, "Udlånt");
                if (eventVare != null)
                {
                    //(EventStackPanel.Children[i] as Button).FontSize = 18;
                    (EventStackPanel.Children[i] as Button).FontWeight = FontWeights.Bold;

                }
            }
        }


        // --------------------------------------------------------------------------------- Evnethandler til eventknapper-----------------------------------------------------------------------------------------------------------------------------------
        private void EventKnapper_Click(object sender, EventArgs e)
        {
            EventClicked = true;
            //create an instance of button based on sender object
            Button btn = (Button)sender;

            //display the name of button and text in it
            //MessageBox.Show(btn.Name);
            // Event der laver lokation om
            GetEventsUsers(btn.Content.ToString());

            currentEventid = int.Parse(btn.Name.Replace("c", ""));
            int eventId = int.Parse(btn.Name.Replace("c", ""));
            currentEvent = btn.Content.ToString();
            GetLocation(eventId, sender);
            GetEventVare(eventId, "Udlånt");
            /*foreach (var sted in EventsList
            ) // Søger igennem eventliste og opretter dynamisk nye knapper LokationStackPanel.Children
            {

                if (sted.Name == btn.Name)
                {
                    foreach (var Lokationer in lokationsList)
                    {
                        Button LokationsKnapper = new Button();
                        LokationsKnapper.Content = Lokationer;
                        LokationsKnapper.Name = Lokationer;
                        object resource =
                            Application.Current
                                .FindResource(
                                    "EandLBtn"); //Tager Stylen fra App.xaml der hedder NewScnbtn og tilføre alle styling til knappen.
                        if (resource != null && resource.GetType() == typeof(Style))
                            LokationsKnapper.Style = (Style) resource;
                        LokationStackPanel.Children.Add(LokationsKnapper);
                        LokationsKnapper.Click += LokationsKnapper_Click;
                    }

                }
            }

            EventsTxt.Content = "Loaktioner";
            TilbageKnap();
            */
        }


        //----------------------------------------------------------------------------------- Tilbageknap under lokationer ----------------------------------------------------------------------------------------------------------------------------------
        public void TilbageKnap()
        {
            Button
                Back = new Button();
            Back.Content = "Tilbage";
            Back.Name = "TilbageKnap";
            object
                res = Application.Current
                    .FindResource(
                        "EandLBtn"); //Tager Stylen fra App.xaml der hedder NewScnbtn og tilføre alle styling til knappen.
            if (res != null && res.GetType() == typeof(Style))
                Back.Style = (Style)res;
            LokationStackPanel.Children.Add(Back);
            Back.Click += TilbageBtn_Click;

            lokationsList.Clear();  // liste clear her ////////////////////////////////////////////////////////////////!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        }

        //-----------------------------------------------------------------------------------EventHandler til Tilbage knappen--------------------------------------------------------------------------------------------------------------------------------
        private void TilbageBtn_Click(object sender, RoutedEventArgs e)
        {
            LokationScroll.Visibility = Visibility.Collapsed;

            EventScroll.Visibility = Visibility.Visible; // gemmer eventscroll så der er klar til Lokationenerne
            LokationStackPanel.Children.Clear();

            NewList.Clear();
            Scan_Page_DataGridUdlej.Items.Refresh();
            LokationNameLaben.Content = "Lokation:";
            EventNameLaben.Content = "Event:";
            EventsTxt.Content = "Events";
            currentEvent = null;
            //compareList.Clear();

        }

        //-----------------------------------------------------------------------------------EventHandler til Lokationer-------------------------------------------------------------------------------------------------------------------------------------
        private void LokationsKnapper_Click(object sender, RoutedEventArgs e)
        {
            Button Btn = (Button)sender;
            currentLokation = Btn.Name;
            LokationNameLaben.Content = string.Format("Lokation: {0}", Btn.Name);
        }


        private void Vare_MouseDoubleClick(object sender, MouseButtonEventArgs e) //åbner mulighed for at ændre status på vare
        {
            if (Scan_Page_DataGridUdlej.SelectedItem == null && Scan_Page_DataGridAflevere.SelectedItem == null)
            {
                MessageBox.Show("Den valgte vare er tom");
            }
            else if(Scan_Page_DataGridUdlej.SelectedItem != null && Scan_Page_DataGridAflevere.SelectedItem == null)
            {
                EventVare tempStatus = (EventVare)Scan_Page_DataGridUdlej.SelectedItem;
                udlejStatus = true;
                foreach(ComboBoxItem cbi in statusCombo.Items)
                {
                    if(cbi.Content.ToString() == tempStatus.Status)
                    {
                        statusCombo.SelectedItem = cbi;
                        break;
                    }
                }
                editStatusGrid.Visibility = Visibility.Visible;
            }
            else if (Scan_Page_DataGridUdlej.SelectedItem == null && Scan_Page_DataGridAflevere.SelectedItem != null)
            {
                EventVare tempStatus = (EventVare)Scan_Page_DataGridAflevere.SelectedItem;
                afleverStatus = true;
                foreach (ComboBoxItem cbi in statusCombo.Items)
                {
                    if (cbi.Content.ToString() == tempStatus.Status)
                    {
                        statusCombo.SelectedItem = cbi;
                        break;
                    }
                }
                editStatusGrid.Visibility = Visibility.Visible;
            }


            //var SI = Scan_Page_DataGridUdlej.SelectedItem as EventVare; //tjekker hvilken item der blev trykket på

            //EditStatus
            //    statusEdit =
            //        new EditStatus(SI, api); //laver vores popup med vare hele VareClass objected som et argument

            //statusEdit.ShowDialog();
            //if (statusEdit.DialogResult.Value) //hvis DialogResult.Value er en Boolean som bliver True kun hvis user har indsat et status ændring og ikke har efterladt et tomt felt
            //{
            //    Scan_Page_DataGridUdlej.CommitEdit(); //virker men skal åbenbart køres 2 gange eftersom at den gemmer collumn og så row
            //    Scan_Page_DataGridUdlej.CommitEdit();
            //    Scan_Page_DataGridUdlej.Items.Refresh();
            //}

        }



                // ----------------------------------------------------------------------------------------Ind og ud scannings knapper --------------------------------------------------------------- ---------------------------------------------------------------------
        public void removeBtn() // fjerner alle eventhandler fra alt hvad der har med ind og udlejning.
        {
            ManueltVAreTxt.KeyDown -= TilføjVaremanuelt;
            ManueltVAreTxt.KeyDown -= AflevereVaremanuelt;
            ManueltVAreTxt.KeyDown -= TilføjVaremanuelt;
            UdlejOgAflevere.Click -= AdleveringBtn_Click;
        }
        public void udlejOgAfleverBtn() //opretter Ny knap til at starte og stoppe scanner
        {

            UdlejOgAflevere.Content = "Afleverings mode!";
            object
                resource = Application.Current
                    .FindResource(
                        "NewScanBtn"); //Tager Stylen fra App.xaml der hedder NewScnbtn og tilføre alle styling til knappen.
            if (resource != null && resource.GetType() == typeof(Style))
                UdlejOgAflevere.Style = (Style)resource;
            Button_Maneger.Children.Add(UdlejOgAflevere); // adder knapper til Wrap panelet..

            UdlejOgAflevere.Click += AdleveringBtn_Click;
        }

        private void UdlejningBtn_Click(object sender, RoutedEventArgs e) // sætter alle knapper op til udlejning
        {
            removeBtn();
            ManueltVAreTxt.KeyDown -= AflevereVaremanuelt; 
            ManueltVAreTxt.KeyDown += TilføjVaremanuelt;
            UdlejOgAflevere.Click += AdleveringBtn_Click;
            UdlejOgAflevere.Content = "Afleverings mode!";
            UdHjem.Content = "Udlejning";
            AfWindow.Close();
        }


        private void AdleveringBtn_Click(object sender, RoutedEventArgs e) // sætter alle knapper op til aflevering
        {
            SetAfleveringsSkærm();
            removeBtn();
            ManueltVAreTxt.KeyDown -= TilføjVaremanuelt;
            ManueltVAreTxt.KeyDown += AflevereVaremanuelt;
            UdlejOgAflevere.Click += UdlejningBtn_Click;
            UdlejOgAflevere.Content = "Udlejnings mode!";
            UdHjem.Content = "Aflevering";
            //AfWindow.Show();

        }


        //---------------------------------------------------------------------------------Temp scan knap, skal med tiden køre scanner! --------------------------------------------------------------------------------------------------------------------
        /*private void Scan_Click(object sender, RoutedEventArgs e)
        {
            string prøve = LokationNameLaben.Content.ToString();
            if (prøve == "Lokation:")
            {
                MessageBox.Show("Du skal vælge en Loaktion før du kan arbejde med vare!");
            }
            else
            {
                string LokationTxt;
                int i = 0;
                foreach (var Item in Tryhardish)
                {
                    LokationTxt = LokationNameLaben.Content.ToString().Replace("Lokation:", ""); // Ændre Lokations lablen og Fjerne "Lokation:" med ingenting så det kun er selv lokationen der bliver gemt i listen.
                    if (ManueltVAreTxt.Text == Item.Vareid) //  loopet vil altid finde alle med same vare nummer og adde dem til datagriddet dette vil apien tage sig af da der aldrig er 2 rfid med samme tag!
                    {
                        i++;
                        Item.Status = "Udlånt";


                        NewList.Add(Item);
                        Item.EventLokation = LokationTxt;
                        Scan_Page_DataGrid.Items.Refresh();
                    }
                }


                if (i == 0)
                    MessageBox.Show("Varen er desværre ikke på lager!");

                else
                    i = 0;

                Scan_Page_DataGrid.ItemsSource =
                    null; // Disse 2 skal være der, hvor man først clear itemssource for at tilføje den igen. ellers updater datagrid ikke efter sortering.
                Scan_Page_DataGrid.ItemsSource = NewList;
            }


        }*/

        //public string nameOrAddress = "192.168.2.60";
        //public bool Tryhardder = PingHost("192.168.2.60");
        public static bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                if (reply.Status == IPStatus.TimedOut)
                {
                    pingable = false;
                }
                else
                {
                    pingable = true;
                }


            }
            catch (PingException)
            {
                MessageBox.Show("Det ser ud til der er problemer med scanner funktionen");
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return pingable;
        }


        //------------------------------------------------------------------------------------------------------------- Scanner Functioner------------------------------------------------------------------------------------------------------------------
        private string EventName;
        bool makeSound = true;
        private void ConnectionInterface_OnTagRead(object sender, TagReadEventArgs e) //Kører hver eneste gang at scanneren læser et RFID Ta
        {
            if (erScannerAktiv == false) { return; }

            Dispatcher.Invoke(() => {
                string labelStatus = (string)UdHjem.Content;
                string status;
                if (labelStatus == "Udlejning")
                {
                    status = "Udlånt";
                }
                else
                {
                    status = "Hjemme";
                }
                string GetEventId = (string)EventNameLaben.Content; // Tager event navn og holder op i objectet for at finde EventID
                GetEventId = GetEventId.Replace("Event: ", "");
                string GetEventIDFound = "";

                foreach (var VARIABLE in EventsList)
                {
                    if (VARIABLE.Name == GetEventId)
                    {
                        GetEventIDFound = VARIABLE.ID.ToString();
                    }
                }
                string LokationTxt = LokationNameLaben.Content.ToString().Replace("Lokation: ", "");
                CheckRfid(GetEventIDFound, e.tagID, LokationTxt, status);
                if (makeSound) { CSLibrary.Tools.Sound.Beep(700, 200); }
                makeSound = true;
            });
        }

        private void Historik_Click(object sender, RoutedEventArgs e)
        {
            //HistorikWindow historikWindow = new HistorikWindow(); // ikke sikker på den her er rigtig!!
            HistorikGrid.Visibility = Visibility.Visible;
            Renew();
            lukHistorik.Focus();


        }

        public async void Renew()
        {
            JArray CArray = await api.GetLastVare();
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
            HistorikPage_DataGrid.ItemsSource = vareList;
            HistorikPage_DataGrid.Items.Refresh();
        }


        //-----------------------------------------------------------------------------------------------------------scanner Knap -------------------------------------------------------------------------------------------------------------------------
        private bool ScannerOn = false;
        private ConnectionInterface connectionInterface;

        private void StartScannerBtn1_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                Dispatcher.Invoke(() => { loadingscrean("on"); ScannerBtn.Content = "Stop Scanner"; });

                //LoadingGrid.Visibility = Visibility.Visible;
                bool ReturnValueTryhardStack = PingHost("192.168.2.60");
                if (ReturnValueTryhardStack == true)
                {
                    Dispatcher.Invoke(() => {
                        EventName = EventNameLaben.Content.ToString();
                        EventName = EventName.Replace("Event: ", "");
                    });

                    if (!ScannerOn)
                    {
                        connectionInterface = new ConnectionInterface(scannerIP); //Opretter vores scanner variabel
                        ScannerOn = connectionInterface.connect(); //Opretter forbindelse til scanner
                        Antenna antenna = new Antenna(connectionInterface, 0);
                        connectionInterface.personList = new List<string>();
                        connectionInterface.OnTagRead += ConnectionInterface_OnTagRead;     // kan kun køre hvis scanneren er sat til og ip indstillinger er lavet..

                        antenna.enabled = true;
                        connectionInterface.setPower(0, scanPowerValue); // Sætter styrken på scanneren
                        connectionInterface.startListening(); //starter scanneren
                        Dispatcher.Invoke(() => {
                            Scanner_Laben.Content = "Scanner ON!";
                            Globalvar.scannerOn = true;
                            Scanner_Laben.Foreground = Brushes.Green;
                            loadingscrean("off");
                            AktivScannerBox.IsChecked = true;
                        });
                        ScannerOn = true;
                        //LoadingGrid.Visibility = Visibility.Collapsed;
                    }
                    else //hvis scanneren er tændt stopper den med at scanne og bliver slået fra
                    {
                        connectionInterface.stopListening();
                        connectionInterface.disconnect();
                        connectionInterface.Dispose();

                        Dispatcher.Invoke(() => {
                            Scanner_Laben.Content = "Scanner OFF!";
                            Globalvar.scannerOn = false;
                            ScannerBtn.Content = "Start Scanner";
                            Scanner_Laben.Foreground = Brushes.Red;
                            loadingscrean("off");
                            AktivScannerBox.IsChecked = false;
                        });

                        ScannerOn = false;
                    }
                }
                else
                {
                    //LoadingGrid.Visibility = Visibility.Collapsed;

                    Dispatcher.Invoke(() => { loadingscrean("off"); });
                MessageBox.Show("Scanner blev ikke fundet!"); /// Guids til opsætning af scanner
                }
            });
        }
        // ----------------------------------------------------------------------------------- tryhad async loading -----------------------------------------------------------------------------------------------------------------------------------------------------
        public void loadingscrean(string on)
        {
            switch (on)
            {
                case "on":
                    LoadingGrid.Visibility = Visibility.Visible;
                    break;
                case "off":
                    LoadingGrid.Visibility = Visibility.Collapsed;
                    break;
            }


        }
        public async void offloadingscrean()
        {
            LoadingGrid.Visibility = Visibility.Collapsed;
        }


        //------------------------------------------------------------------------------------------------------------ Tilføj Lokation------------------------------------------------------------------------------------------------------------------------------------------------
        int Tæller = 0;
        string TællerString;


        // LocationFunction
        private void LokationsBtn_Click(object sender, RoutedEventArgs e)
        {
            LokationsFunction();
        }

        public void LokationsFunction() // function til at åbne Add lokations  griddet.
        {
            if (EventNameLaben.Content.ToString() != "Event:"
            ) // Kontrollere om man har valgt et event før man kan tilføje til det.
            {
                MultiGrid.Visibility = Visibility.Visible;
                string tryhard = "";



                foreach (var sted in EventsList)
                {
                    tryhard = EventNameLaben.Content.ToString();
                    tryhard = tryhard.Replace("Event: ", "");
                    EventLabel.Content = EventNameLaben.Content;

                    if (sted.Name == tryhard)
                    {
                        foreach (var list in lokationsList)
                        {
                            System.Windows.Controls.TextBox txtNumber = new System.Windows.Controls.TextBox();
                            TællerString = String.Format("Nr{0}", Tæller);


                            txtNumber.Name = list;
                            txtNumber.Text = list;
                            txtNumber.Height = 35;
                            txtNumber.FontSize = 26;

                            txtNumber.Margin = new Thickness(20, 10, 10, 10);

                            EditLokationStack.Children.Add(txtNumber);

                            Tæller++;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Du skal vælge et Event før du kan tilføje en Lokation til den!");
            }
        }


        // -----------------------------------------------------------------------------------Api kald til at oprette et nyt event lokation------------------------------------------------------
        private async Task<List<EventData>> createEventLokation(int id, string lokation)
        {
            JArray eventLokation = await api.CreateEventLokationAsync(id, lokation);
            return EventsList;

        }

        bool ContiniousEdit = false;

        void EditLocation(string LokationsID, string EventID, string NyLokation)
        {
            Console.WriteLine("Lol JK");
            _ = api.editEventLocation(LokationsID, EventID, NyLokation);
            Button LokationsKnapper = new Button();
            Button LokationsTextBoxNot = new Button();

            LokationsTextBoxNot.Name = RegexSave(AddTxt.Text);
            LokationsTextBoxNot.Content = RegexSave(AddTxt.Text);
            // Pass Lokation ID til LokationsTextBoxNot som i et tag
            //LokationsTextBoxNot.Tag = lokationIDList[i];
            LokationsTextBoxNot.Tag = selectedEditLokationID;

            LokationsTextBoxNot.Height = 35;
            LokationsTextBoxNot.FontSize = 26;
            LokationsTextBoxNot.Margin = new Thickness(20, 10, 10, 10); LokationsKnapper.Content = AddTxt.Text;

            LokationsKnapper.Name = RegexSave(AddTxt.Text);


            object resource =
                Application.Current
                    .FindResource(
                        "EandLBtn"); //Tager Stylen fra App.xaml der hedder NewScnbtn og tilføre alle styling til knappen.
            if (resource != null && resource.GetType() == typeof(Style))
            {
                LokationsKnapper.Style = (Style)resource;
            }

            LokationStackPanel.Children.RemoveAt(selectedEditLokationID);
            EditLokationStack.Children.RemoveAt(selectedEditLokationID);

            LokationsKnapper.Click += LokationsKnapper_Click;
            LokationsTextBoxNot.Click += LokationsTextBoxNot_Click;
            LokationStackPanel.Children.Insert(selectedEditLokationID, LokationsKnapper);
            EditLokationStack.Children.Insert(selectedEditLokationID, LokationsTextBoxNot);

            AddTxt.Clear();

            if (ContiniousEdit == false)
            {
                MultiGrid.Visibility = Visibility.Hidden;
            }

        }
        private void AddLokationsBtn_Click(object sender, RoutedEventArgs e)
        {
            EditLocation(selectedLokationID, selectedEventID, AddTxt.Text);
        }

        private string Event = "";

        // -----------------------------------------------------------------------------------Afslutter alt fra kokations pop up og gemmer den nye liste til eventet ,med nye og redigeret lokationer------------------------------------------------------
        private void EndGrid_Click(object sender, RoutedEventArgs e)
        {
            MultiGrid.Visibility = Visibility.Collapsed;
            /*
            List<string> compareList = new List<string>();


            Event = EventNameLaben.Content.ToString();
            Event = Event.Replace("Event: ", "");


            foreach (object child in EditLokationStack.Children)
            {
                string childname = null;
                if (child is FrameworkElement)
                {
                    string FirstSave = (child as TextBox).Text;

                    childname = Regex.Replace(FirstSave, @"[^0-9a-zA-ZÆØÅæøå]+",
                        "_"); // mangler stadig at godkende æøå både stort som småt

                }

                if (childname != null && childname != "")
                    compareList.Add(childname);
            }



            /*foreach (var sted in EventsList) // skal sammenligne event navn for at finde den orginale liste.
            {
                if (Event == sted.Name)
                    sted.setLokation(compareList);
            }*/

            /*
            LokationStackPanel.Children.Clear();

            foreach (var sted in EventsList)
            {
                if (sted.Name == Event)
                {
                    foreach (var List in lokationsList) /////! en liste til at styre lokationer!!!!!!!!!!! fixer alt
                    {
                        string crossover = List;
                        string crossÓver =
                            Regex.Replace(crossover, @"[^0-9a-zA-ZÆØÅæøå]+", "_"); //implementere æøå stort som småt
                        Button LokationsKnapper = new Button();
                        LokationsKnapper.Content = crossÓver;
                        LokationsKnapper.Name = crossÓver;
                        object resource =
                            Application.Current
                                .FindResource(
                                    "NewScanBtn"); //Tager Stylen fra App.xaml der hedder NewScnbtn og tilføre alle styling til knappen.
                        if (resource != null && resource.GetType() == typeof(Style))
                            LokationsKnapper.Style = (Style)resource;
                        LokationStackPanel.Children.Add(LokationsKnapper);
                        LokationsKnapper.Click += LokationsKnapper_Click;


                    }
                }
            }

            EditLokationStack.Children.Clear();

            TilbageKnap();


            Tæller = 0;
            TællerString = "";*/
        }



        // ----------------------------------------------------------------------------------------Ind og ud scannings knapper --------------------------------------------------------------- ---------------------------------------------------------------------






        public void PutAfleveringSkærmPåSkærm()
        {
            AfWindow = new Afleveringsskærme();
            Debug.Assert(System.Windows.Forms.SystemInformation.MonitorCount > 1);
            System.Drawing.Rectangle workingArea = System.Windows.Forms.Screen.AllScreens[1].WorkingArea; // sæt skærmen ved at vælge skærm [0-3] alt efter hvor mange skærme der er. lige nu står den på 2. skærm da et array køre med 0 som først og op efter.
            AfWindow.Left = workingArea.Left;
            AfWindow.Top = workingArea.Top;
            AfWindow.Width = workingArea.Width;
            AfWindow.Height = workingArea.Height;
            AfWindow.WindowStyle = WindowStyle.None;
            AfWindow.Topmost = true;
        }

        // ------------------------------------------------------------------------------------------------function til at åbne afleveringskærmen-------------------------------------------------------------------------------------------------------
        public void SetAfleveringsSkærm() // function til at åbne afleveringskærmen på et bestemt window i fuldskærm, linket op med Window_Loaded på Afleveringskærmen.xaml.cs
        {
            AfWindow.WindowStartupLocation = WindowStartupLocation.Manual; // kunne også sættes i Xaml på selve winduet.
            int monitors = System.Windows.Forms.SystemInformation.MonitorCount;
            Console.WriteLine(System.Windows.Forms.SystemInformation.MonitorCount);
            if (monitors > 1)
            {
                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Vil du have afleveringsskærm på alle dine skærme?", "Aflerveringskærm", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)  // error is here
                {
                    PutAfleveringSkærmPåSkærm();
                    AfWindow.Show();
                }
                else
                {
                    Console.WriteLine("Oki Doki");
                }
            }


        }


        // --------------------------------------------------------------------------------------- nyt scan_event-------------------------------------------------------------------------------------------------------------------------------------------

        //var newListTemp = NewList; //opretter en midlertidig liste så vi ikke for en error at rangen er ændret i vores loop.
        private void Scan_Ind_Click(object sender, RoutedEventArgs e)
        {
            string LokationTxt;
            int i = 0;
            int Indextæller = 0;
            int marker = 0;
            string Trigger = "";


            foreach (var Item in NewList) // Tager en midlertidig liste så vi kan slette fra originalen
            {
                LokationTxt =
                    LokationNameLaben.Content.ToString()
                        .Replace("Lokation:",
                            ""); // Ændre Lokations lablen og Fjerne "Lokation:" med ingenting så det kun er selv lokationen der bliver gemt i listen.
                if (
                    ManueltVAreTxt.Text ==
                    Item.Vareid) //  loopet vil altid finde alle med same vare nummer og adde dem til datagriddet dette vil apien tage sig af da der aldrig er 2 rfid med samme tag! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                {
                    i++;
                    Item.Status = "Hjemme";
                    Trigger =
                        "onefound"; // sætter Trigger som en "trigger" så vores if kan se en ændring og derefter slette varen fra udlejet datagrid.
                    marker = Indextæller; // gemmer indexet som vi nedenfor sletter med. dermed dynamsik.
                    Item.EventLokation = LokationTxt;
                    //AfleveretList.Add(Item); MÅSKE FJERNES
                    AfleveretList.Insert(0, Item);

                }

                Indextæller++;
            }

            Indextæller = 0;
            if (Trigger == "onefound") //marker kan ikke være 0 da første index i en list og et array er 0.
            {
                NewList.RemoveAt(marker);
                Scan_Page_DataGridUdlej.Items.Refresh();

            }
            else
                MessageBox.Show("Der skete en Fejl!");



            if (i == 0)
                MessageBox.Show("Varen er desværre ikke på lager!");

            else
                i = 0;

        }


        //---------------------------------------------------------------------------------------------udlån og Aflever knap. skifter mellem at vise vare som er afleveret og udlånt mellem 2 lister. dynamisk----------------------------------------------
        private void AfleveretBtn_Click(object sender, RoutedEventArgs e)
        {
            if (EventNameLaben.Content.ToString() == "Event:") { MessageBox.Show("Du skal have valgt et Event før du kan klikke her!"); return; }

            const string udlejet = "Udlejet Vare";
            const string afleveret = "Afleveret Vare";




            string GetEventId = (string)EventNameLaben.Content; // Tager event navn og holder op i iobjectet for at finde EventID
            GetEventId = GetEventId.Replace("Event: ", "");
            string GetEventIDFound = "";
            foreach (var VARIABLE in EventsList)
            {
                if (VARIABLE.Name == GetEventId)
                {
                    GetEventIDFound = VARIABLE.ID.ToString();
                }
            }

            switch (AfleveretBtn.Content)
            {
                case afleveret:
                    Scan_Page_DataGridUdlej.Visibility = Visibility.Collapsed;
                    Scan_Page_DataGridAflevere.Visibility = Visibility.Visible;

                    AfleveretBtn.Content = udlejet;
                    Scan_Page_DataGridAflevere.ItemsSource = null;
                    Scan_Page_DataGridAflevere.ItemsSource = AfleveretList;
                    GetEventVareAfleveret(Int32.Parse(GetEventIDFound), "Hjemme");
                    Scan_Page_DataGridAflevere.Items.Refresh();


                    break;
                case udlejet:
                    Scan_Page_DataGridAflevere.Visibility = Visibility.Collapsed;
                    Scan_Page_DataGridUdlej.Visibility = Visibility.Visible;
                    AfleveretBtn.Content = afleveret;
                    Scan_Page_DataGridUdlej.ItemsSource = null;
                    Scan_Page_DataGridUdlej.ItemsSource = NewList;
                    GetEventVare(Int32.Parse(GetEventIDFound), "Udlånt");
                    Scan_Page_DataGridUdlej.Items.Refresh();
                    break;

            }


        }

        // ------------------------------------------------------------------------ Funktion til at tømme vare køen ---------------------------------------------------------------------
        public async void aflastQueue()
        {
            for (int j = 0; j < queue.Count; j++)
            {

            JArray CheckRfidNr = await api.GetAllVare();
            int i = 0;
            foreach (JObject item in CheckRfidNr)
            {
                string Rfid = (string)item.GetValue("rfidNummer");

                if (Rfid == queue[j].rfidNr)
                {
                    _ = AfScanVareRfid(queue[j].eventId, queue[j].rfidNr, queue[j].eventLokation, queue[j].status, bruger.Name);
                    break;
                }
                i++;
            }

            eventIdsend = queue[j].eventId;
            rfidNrSend = queue[j].rfidNr;
            eventLokationSend = queue[j].eventLokation;
            statusSend = queue[j].status;

            //MessageBox.Show(erdender);
            if (queue[j].status == "Udlånt")
            {
                GetEventVare(Int32.Parse(queue[j].eventId), queue[j].status);
                GetEventVareAfleveret(Int32.Parse(queue[j].eventId), "Hjemme");
            }
            else
            {
                GetEventVare(Int32.Parse(queue[j].eventId), "Udlånt");
                GetEventVareAfleveret(Int32.Parse(queue[j].eventId), queue[j].status);
            }
            }
            Scan_Page_DataGridUdlej.Items.Refresh();
            Scan_Page_DataGridAflevere.Items.Refresh();
            queue.Clear();
        }
        //-------------------------------------------------------------------------Henter alle vare og går dem igennem for rfid----------------------------------------------------------
        string outrfidNr;
        string eventIdsend;
        string rfidNrSend;
        string eventLokationSend;
        string statusSend;

        public async void CheckRfid(string eventId, string rfidNr, string eventLokation, string status)
        {
            if (EventNameLaben.Content.ToString() == "Event:") { makeSound = false; MessageBox.Show("Du skal have valgt et Event før du kan skanne!"); return; }
            if ((string)LokationNameLaben.Content == "Lokation:"){ currentLokation = "_"; eventLokation = "_"; LokationNameLaben.Content = string.Format("Lokation: {0}", "_");}
            string erdender = "";
            bool control = false;
            AddUserData bla = await GetUsers(rfidNr);
            if (needUserID == false)
            {
                if(brugerFundet == true) { /*MessageBox.Show("Afbryder");*/ return;}
                JArray CheckRfidNr = await api.GetAllVare();
                foreach (JObject item in CheckRfidNr)
                {
                    string Rfid = (string)item.GetValue("rfidNummer");

                    if (Rfid == rfidNr)
                    {
                        control = false;
                        _ = scanVareRfid(eventId, rfidNr, eventLokation, status);
                        break;
                    }
                    else
                    {
                        control = true;
                    }
                }
                if (control == true)
                {
                    JaNej(rfidNr);
                }
                eventIdsend = eventId;
                rfidNrSend = rfidNr;
                eventLokationSend = eventLokation;
                statusSend = status;

                //MessageBox.Show(erdender);
                if (status == "Udlånt")
                {
                    GetEventVare(Int32.Parse(eventId), status);
                    GetEventVareAfleveret(Int32.Parse(eventId), "Hjemme");
                }
                else
                {
                    GetEventVare(Int32.Parse(eventId), "Udlånt");
                    GetEventVareAfleveret(Int32.Parse(eventId), status);
                }
                /*GetEventVare(Int32.Parse(eventId), status);
                GetEventVareAfleveret(Int32.Parse(eventId), status);*/
                return;
            }

            if (nyBruger == true)
            {
                // Kør bruger funktionalitet her
                userSet = true;
                //MessageBox.Show("Bruger Fundet!");
            }

            if (nyBruger == false && brugerFundet == true)
            {
                //MessageBox.Show("Bruger fundet igen");
            }
            if(brugerFundet == true)
            {
                aflastQueue();
            }
              
            if (brugerFundet == false && nyBruger == false)
            {
                if (bruger == null)
                {
                    // Her Skal udlegning og afleverings kode med bruger puttes
                    queue.Add(new queueClass(eventId, rfidNr, eventLokation, status));
                }
                else
                {
                    JArray CheckRfidNr = await api.GetAllVare();
                    foreach (JObject item in CheckRfidNr)
                    {
                        string Rfid = (string)item.GetValue("rfidNummer");

                        if (Rfid == rfidNr)
                        {
                            control = false;
                            _ = AfScanVareRfid(eventId, rfidNr, eventLokation, status, bruger.Name);
                            break;
                        }
                        else
                        {
                            control = true;
                        }
                    }
                    if (control == true)
                    {
                        JaNej(rfidNr);
                    }
                    eventIdsend = eventId;
                    rfidNrSend = rfidNr;
                    eventLokationSend = eventLokation;
                    statusSend = status;

                    //MessageBox.Show(erdender);
                    if (status == "Udlånt")
                    {
                        GetEventVare(Int32.Parse(eventId), status);
                        GetEventVareAfleveret(Int32.Parse(eventId), "Hjemme");
                    }
                    else
                    {
                        GetEventVare(Int32.Parse(eventId), "Udlånt");
                        GetEventVareAfleveret(Int32.Parse(eventId), status);
                    }
                }
            }
        }
        public void JaNej(string rfidNr)
        {
            if (MessageBox.Show("Vil du Tilføje en Vare til dette Rfid ?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                outrfidNr = rfidNr;
                TilføjRfidTilVAreNummer.Visibility = Visibility.Visible;
            }
            else
            {
                // nej
            }
        }
        //--------------------------------------------------------------------------------Kanpper til at styre  Tilføjelse af rfid--------------------------------------
        private void TRTVNSBtn_Click(object sender, RoutedEventArgs e)
        {
            string TRTVNS = TRTVNSTxt.Text;
            _ = api.tilføjRfidTilVare(TRTVNS, outrfidNr);
            _ = scanVareRfid(eventIdsend, rfidNrSend, eventLokationSend, statusSend);

            GetEventVare(Int32.Parse(eventIdsend), statusSend);
            TilføjRfidTilVAreNummer.Visibility = Visibility.Collapsed;
        }

        private void Cansel_Click(object sender, RoutedEventArgs e)
        {
            TilføjRfidTilVAreNummer.Visibility = Visibility.Collapsed;
        }
        // --------------------------------------------------------------------------------- Api kald til at hente udlejet vare-----------------------------------------------------------------------------------------------------------------------------------
        public async void GetEventVare(int EventID, string VareStatus)
        {
            string eventID = EventID.ToString();

            JArray eventVare = await api.GetEventVare(eventID, VareStatus);
            NewList.Clear();
            if (eventVare != null)
            {
                foreach (JObject item in eventVare)
                {
                    int Id = (int)item.GetValue("Id");
                    string VareId = (string)item.GetValue("VareId");
                    string itemBeskrivelse = (string)item.GetValue("Beskrivelse");
                    string ampere = (string)item.GetValue("Ampere");
                    string EventLokation = (string)item.GetValue("EventLokation");
                    string EventName = (string)item.GetValue("EventName");
                    string Status = (string)item.GetValue("Status");
                    string Aflevere = (string)item.GetValue("Aflevere");
                    string WebshopVarenummer = (string)item.GetValue("WebshopVarenummer");
                    string PinNr = (string)item.GetValue("PinNr");
                    string Container = (string)item.GetValue("Container");

                    //NewList.Add(new EventVare(VareId, itemBeskrivelse, ampere, EventLokation, EventName, Status, Aflevere, WebshopVarenummer, PinNr, Container)); MÅSKE FJERNES
                    NewList.Insert(0, new EventVare(Id ,VareId, itemBeskrivelse, ampere, EventLokation, EventName, Status, Aflevere, WebshopVarenummer, PinNr, Container));
                    Dispatcher.Invoke(() => {
                        Scan_Page_DataGridUdlej.Items.Refresh();
                        Scan_Page_DataGridAflevere.Items.Refresh();
                    });
                }
            }
        }
        public async void GetEventVareAfleveret(int EventID, string VareStatus)
        {
            string eventID = EventID.ToString();

            JArray eventVare = await api.GetEventVare(eventID, VareStatus);
            AfleveretList.Clear();
            if (eventVare != null)
            {
                foreach (JObject item in eventVare)
                {
                    int Id = (int)item.GetValue("Id");
                    string VareId = (string)item.GetValue("VareId");
                    string itemBeskrivelse = (string)item.GetValue("Beskrivelse");
                    string ampere = (string)item.GetValue("Ampere");
                    string EventLokation = (string)item.GetValue("EventLokation");
                    string EventName = (string)item.GetValue("EventName");
                    string Status = (string)item.GetValue("Status");
                    string Aflevere = (string)item.GetValue("Aflevere");
                    string WebshopVarenummer = (string)item.GetValue("WebshopVarenummer");
                    string PinNr = (string)item.GetValue("PinNr");
                    string Container = (string)item.GetValue("Container");

                    AfleveretList.Insert(0, new EventVare(Id, VareId, itemBeskrivelse, ampere, EventLokation, EventName, Status, Aflevere, WebshopVarenummer, PinNr, Container));
                    Dispatcher.Invoke(() => {
                        Scan_Page_DataGridUdlej.Items.Refresh();
                        Scan_Page_DataGridAflevere.Items.Refresh();
                    });
                }
            }
        }

        public async Task<List<EventVare>> scanVareId(string eventId, string vareId, string eventlokation, string status)
        {
            JArray ScannetVare = await api.CreateEventVareVareId(eventId, vareId, eventlokation, status);
            Dispatcher.Invoke(() => {
                Scan_Page_DataGridUdlej.Items.Refresh();
                Scan_Page_DataGridAflevere.Items.Refresh();
            });
            //Scan_Page_DataGridUdlej.Items.Refresh();
            //Scan_Page_DataGridAflevere.Items.Refresh();
            return NewList;
        }
        //-------------------------------------------------------------------------------------------------------------Api kald til at scanne vare med rfid nummer------------------------------------------------------------------------------------------------------------------
        public async Task<JObject> scanVareRfid(string eventId, string rfid, string eventLokation, string status)
        {
            JObject ScannetVareRfid = await api.CreateEventVareRfid(eventId, rfid, eventLokation, status);
            Dispatcher.Invoke(() => {
                Scan_Page_DataGridUdlej.Items.Refresh();
                Scan_Page_DataGridAflevere.Items.Refresh();
            });
            //Scan_Page_DataGridUdlej.Items.Refresh();
            //Scan_Page_DataGridAflevere.Items.Refresh();
            return ScannetVareRfid;
        }

        public async Task<JObject> AfScanVareRfid(string eventId, string rfid, string eventLokation, string status, string bruger)
        {
            JObject ScannetVareRfid = await api.AflevereEventVareRfid(eventId, rfid, eventLokation, status, bruger);
            Dispatcher.Invoke(() => {
                Scan_Page_DataGridUdlej.Items.Refresh();
                Scan_Page_DataGridAflevere.Items.Refresh();
            });
            //Scan_Page_DataGridUdlej.Items.Refresh();
            //Scan_Page_DataGridAflevere.Items.Refresh();
            return ScannetVareRfid;
        }

        // ----------------------------------------------------------------------------------------------------- eventhandlers der kontrollere om enter bliver pressed i varenummer text boksen. både ind og udlevere-----------------
        private async void TilføjVaremanuelt(object sender, KeyEventArgs e)
        {

            string GetEventId = (string)EventNameLaben.Content; // Tager event navn og holder op i i objectet for at finde EventID
            GetEventId = GetEventId.Replace("Event: ", "");
            string GetEventIDFound = "";

            foreach (var VARIABLE in EventsList)
            {
                if (VARIABLE.Name == GetEventId)
                {
                    GetEventIDFound = VARIABLE.ID.ToString();
                }
            }
            string prøve = LokationNameLaben.Content.ToString();

            if (e.Key == Key.Enter)
            {

                if (prøve == "Lokation:")
                {
                    MessageBox.Show("Du skal vælge en lokation for at arbejde med vare!");
                }
                else
                {
                    EventVare o = null;
                    string LokationTxt = LokationNameLaben.Content.ToString().Replace("Lokation: ", ""); // Ændre Lokations lablen og Fjerne "Lokation:" med ingenting så det kun er selv lokationen der bliver gemt i listen.
                    int i = 0;
                    if (NewList.Count == 0)
                    {
                        i++;

                        //CheckRfid(GetEventIDFound, "sdfsdfsdfsfdvrfg", LokationTxt, "Udlånt"); // kun til test!!!!!!!!!!!!!!!!!!!!!!!!
                        _ = scanVareId(GetEventIDFound, ManueltVAreTxt.Text, LokationTxt, "Udlånt");

                        GetEventVare(Int32.Parse(GetEventIDFound), "Udlånt");
                        GetEventVareAfleveret(Int32.Parse(GetEventIDFound), "Hjemme");

                        Scan_Page_DataGridUdlej.Items.Refresh();
                        ManueltVAreTxt.Text = "";
                    }
                    else
                    {
                        foreach (var Item in NewList)
                        {

                            if (!Item.Vareid.Equals(ManueltVAreTxt.Text))
                            {
                                i++;
                                //CheckRfid(GetEventIDFound, "PIHL", LokationTxt, "Udlånt"); // kun til test!!!!!!!!!!!!!!!!!!!!

                                _ = scanVareId(GetEventIDFound, ManueltVAreTxt.Text, LokationTxt, "Udlånt");

                                GetEventVare(Int32.Parse(GetEventIDFound), "Udlånt");
                                GetEventVareAfleveret(Int32.Parse(GetEventIDFound), "Hjemme");

                                Scan_Page_DataGridUdlej.Items.Refresh();
                                ManueltVAreTxt.Text = "";
                            }


                        }
                    }


                    if (i == 0)
                        MessageBox.Show("Varen er desværre ikke på lager!");

                    else
                        i = 0;


                }
            }
        }

        private void AflevereVaremanuelt(object sender, KeyEventArgs e)
        {
            string LokationTxt;
            int i = 0;
            int Indextæller = 0;
            int marker = 0;
            string Trigger = "";
            string GetEventId = (string)EventNameLaben.Content; // Tager event navn og holder op i i objectet for at finde EventID
            GetEventId = GetEventId.Replace("Event: ", "");
            string GetEventIDFound = "";

            foreach (var VARIABLE in EventsList)
            {
                if (VARIABLE.Name == GetEventId)
                {
                    GetEventIDFound = VARIABLE.ID.ToString();
                }
            }

            if (e.Key == Key.Enter)
            {
                LokationTxt = LokationNameLaben.Content.ToString().Replace("Lokation: ", ""); // Ændre Lokations lablen og Fjerne "Lokation:" med ingenting så det kun er selv lokationen der bliver gemt i listen.
                i = 0;
                if (NewList.Count == 0)
                {

                }
                else
                {
                    foreach (var Item in NewList)
                    {

                        if (Item.Vareid.Equals(ManueltVAreTxt.Text))
                        {
                            i++;
                            //CheckRfid(GetEventIDFound, "PIHL", LokationTxt, "Udlånt"); // kun til test!!!!!!!!!!!!!!!!!!!!

                            _ = scanVareId(GetEventIDFound, ManueltVAreTxt.Text, LokationTxt, "Hjemme");

                            GetEventVareAfleveret(Int32.Parse(GetEventIDFound), "Hjemme");
                            GetEventVare(Int32.Parse(GetEventIDFound), "Udlånt");

                            Scan_Page_DataGridAflevere.Items.Refresh();
                            ManueltVAreTxt.Text = "";
                            Trigger = "onefound"; // sætter Trigger som en "trigger" så vores if kan se en ændring og derefter slette varen fra udlejet datagrid.
                            marker = Indextæller; // gemmer indexet som vi nedenfor sletter med. dermed dynamsik.
                            Item.EventLokation = LokationTxt;


                            string Name = Item.Beskrivelse;
                            string hylde = Item.EventLokation;
                            string krog = Item.PinNr;


                            Label itemNavn = new Label();
                            itemNavn.Content = Name;
                            itemNavn.Height = 55;
                            itemNavn.Foreground = Brushes.White;
                            itemNavn.Background = userColor;
                            itemNavn.BorderBrush = Brushes.White;
                            itemNavn.FontSize = 30;
                            itemNavn.HorizontalContentAlignment = HorizontalAlignment.Center;
                            itemNavn.BorderThickness = new Thickness(2.0);

                            Label itemHylde = new Label();
                            itemHylde.Content = hylde;
                            itemHylde.Height = 55;
                            itemHylde.Foreground = Brushes.White;
                            itemHylde.Background = userColor;
                            itemHylde.BorderBrush = Brushes.White;
                            itemHylde.FontSize = 30;
                            itemHylde.HorizontalContentAlignment = HorizontalAlignment.Center;
                            itemHylde.BorderThickness = new Thickness(2.0);

                            Label itemKrog = new Label();
                            itemKrog.Content = krog;
                            itemKrog.Height = 55;
                            itemKrog.Foreground = Brushes.White;
                            itemKrog.Background = userColor;
                            itemKrog.BorderBrush = Brushes.White;
                            itemKrog.FontSize = 30;
                            itemKrog.HorizontalContentAlignment = HorizontalAlignment.Center;
                            itemKrog.BorderThickness = new Thickness(2.0);

                            AfWindow.UserStackpanelNavn.Children.Insert(0, itemNavn);
                            AfWindow.UserStackpanelHylde.Children.Insert(0, itemHylde);
                            AfWindow.UserStackpanelKrog.Children.Insert(0, itemKrog);

                            if (AfWindow.UserStackpanelNavn.Children.Count == 11 && AfWindow.UserStackpanelHylde.Children.Count == 11 && AfWindow.UserStackpanelKrog.Children.Count == 11)
                            {
                                AfWindow.UserStackpanelNavn.Children.RemoveAt(10);
                                AfWindow.UserStackpanelHylde.Children.RemoveAt(10);
                                AfWindow.UserStackpanelKrog.Children.RemoveAt(10);
                            }
                        }

                        Indextæller++;
                    }
                    //Indextæller = 0;
                    //if (Trigger == "onefound") //marker kan ikke være 0 da første index i en list og et array er 0.
                    //{
                    //    NewList.RemoveAt(marker);
                    //    Scan_Page_DataGrid.Items.Refresh();

                    //}
                    //else
                    //    MessageBox.Show("Der skete en fejl!");



                    //if (i == 0)
                    //    MessageBox.Show("Varen er desværre ikke på lager!");

                    //else
                    //    i = 0;
                }
                /*foreach (var Item in NewList) // Tager en midlertidig liste så vi kan slette fra originalen
                {
                    LokationTxt = LokationNameLaben.Content.ToString().Replace("Lokation:",
                                ""); // Ændre Lokations lablen og Fjerne "Lokation:" med ingenting så det kun er selv lokationen der bliver gemt i listen.
                    if ( ManueltVAreTxt.Text == Item.Vareid) //  loopet vil altid finde alle med same varenummer og adde dem til datagriddet dette vil apien tage sig af da der aldrig er 2 rfid med samme tag! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    {
                        i++;
                        Item.Status = "Hjemme";
                        Trigger = "onefound"; // sætter Trigger som en "trigger" så vores if kan se en ændring og derefter slette varen fra udlejet datagrid.
                        marker = Indextæller; // gemmer indexet som vi nedenfor sletter med. dermed dynamsik.
                        Item.EventLokation = LokationTxt;
                        AfleveretList.Add(Item);


                        string Name = Item.Beskrivelse;
                        string hylde = Item.EventLokation;
                        string krog = Item.PinNr;


                        Label itemNavn = new Label();
                        itemNavn.Content = Name;
                        itemNavn.Height = 55;
                        itemNavn.Foreground = Brushes.White;
                        itemNavn.Background = userColor;
                        itemNavn.BorderBrush = Brushes.White;
                        itemNavn.FontSize = 30;
                        itemNavn.HorizontalContentAlignment = HorizontalAlignment.Center;
                        itemNavn.BorderThickness = new Thickness(2.0);

                        Label itemHylde = new Label();
                        itemHylde.Content = hylde;
                        itemHylde.Height = 55;
                        itemHylde.Foreground = Brushes.White;
                        itemHylde.Background = userColor;
                        itemHylde.BorderBrush = Brushes.White;
                        itemHylde.FontSize = 30;
                        itemHylde.HorizontalContentAlignment = HorizontalAlignment.Center;
                        itemHylde.BorderThickness = new Thickness(2.0);

                        Label itemKrog = new Label();
                        itemKrog.Content = krog;
                        itemKrog.Height = 55;
                        itemKrog.Foreground = Brushes.White;
                        itemKrog.Background = userColor;
                        itemKrog.BorderBrush = Brushes.White;
                        itemKrog.FontSize = 30;
                        itemKrog.HorizontalContentAlignment = HorizontalAlignment.Center;
                        itemKrog.BorderThickness = new Thickness(2.0);

                        AfWindow.UserStackpanelNavn.Children.Insert(0, itemNavn);
                        AfWindow.UserStackpanelHylde.Children.Insert(0, itemHylde);
                        AfWindow.UserStackpanelKrog.Children.Insert(0, itemKrog);

                        if (AfWindow.UserStackpanelNavn.Children.Count == 11 && AfWindow.UserStackpanelHylde.Children.Count == 11 && AfWindow.UserStackpanelKrog.Children.Count == 11)
                        {
                            AfWindow.UserStackpanelNavn.Children.RemoveAt(10);
                            AfWindow.UserStackpanelHylde.Children.RemoveAt(10);
                            AfWindow.UserStackpanelKrog.Children.RemoveAt(10);
                        }

                    }

                    Indextæller++;
                }*/

                /*Indextæller = 0;
                if (Trigger == "onefound") //marker kan ikke være 0 da første index i en list og et array er 0.
                {
                    NewList.RemoveAt(marker);
                    Scan_Page_DataGrid.Items.Refresh();

                }
                else
                    MessageBox.Show("Der skete en fejl!");



                if (i == 0)
                    MessageBox.Show("Varen er desværre ikke på lager!");

                else
                    i = 0;*/
            }


        }

        // --------------------------------------------------------------------------------------------------------------- Funktion til at tilføje tilbehør -----------------------------------------------------------------------------



        private void ScanTilbehør(object sender, RoutedEventArgs e)
        {
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

            int i = 0;
            string web = "";
            string cbContent = "";
            int tæller = 0;
            bool exits = false;
            string mode = ScanTilbehørButton.Content.ToString();

            for (int j = 0; j < MotherOfAllStacksCombo1.Children.Count; j++)
            {
                object child = MotherOfAllStacksCombo1.Children[j];

                string childname = null;
                if (child is FrameworkElement)
                {
                    childname = (child as FrameworkElement).Name; // tager navnet fra et element i comboboxen og gemmer som en string variable..
                    i = (child as ComboBox).SelectedIndex;
                    web = (child as ComboBox).Tag.ToString();
                    if ((child as ComboBox).SelectedValue != null)
                    {
                        cbContent = (child as ComboBox).SelectedValue.ToString();
                    }
                    
                    //string s = (child as ComboBox).SelectedItem.ToString();
                    //string ss = (child as ComboBox).Text;
                }

                if (i != -1) // skal være (i != -1)
                {

                    foreach (var Variable in ekstraList)
                    {
                        if (Variable.Beskrivelse == childname)
                        {
                            foreach(TilbehørClass ev in udlåntTilbehør)
                            {
                                if(mode == "Udlej tilbehør")
                                {
                                    if (ev.VareNavn == Variable.Beskrivelse && ev.Lokation == currentLokation)
                                    {
                                        _ = udlejEkstraAsync(web, currentEventid.ToString(), currentLokation, "Udlånt", ev.Antal + Int32.Parse(cbContent));
                                        ev.Antal = ev.Antal + Int32.Parse(cbContent);
                                        Scan_Page_TilbehørGrid.Items.Refresh();
                                        exits = true;
                                        _ = GetEkstraAsync();
                                        break;
                                    }
                                }
                                else if(mode == "Aflever tilbehør")
                                {
                                    if (ev.VareNavn == Variable.Beskrivelse && ev.Lokation == currentLokation)
                                    {
                                        _ = udlejEkstraAsync(web, childname, currentLokation, "Udlånt", ev.Antal - Int32.Parse(cbContent));
                                        if((ev.Antal - Int32.Parse(cbContent)) == 0)
                                        {
                                            udlåntTilbehør.Remove(ev);
                                        }
                                        else
                                        {
                                            ev.Antal = ev.Antal - Int32.Parse(cbContent);
                                        }
                                        
                                        Scan_Page_TilbehørGrid.Items.Refresh();
                                        _ = GetEkstraAsyncAf();
                                        exits = true;
                                        break;
                                    }
                                }
                                
                            }
                            if(mode == "Udlej tilbehør" && exits == false)
                            {
                                _ = udlejEkstraAsync(web, currentEventid.ToString(), currentLokation, "Udlånt", Int32.Parse(cbContent));
                                udlåntTilbehør.Add(new TilbehørClass(childname, Int32.Parse(cbContent), currentLokation, currentEvent, "Udlånt"));
                                _ = GetEkstraAsync();
                                Scan_Page_TilbehørGrid.Items.Refresh();
                                Variable.Antal = Variable.Antal - i;
                                break;
                            }
                            else if (mode == "Aflever tilbehør" && exits == false)
                            {
                                MessageBox.Show("Du kan ikke aflevere " + Variable.Beskrivelse + " fordi den er ikke udlejet til eventet");
                            }

                        }

                    }
                }


            }

            exits = false;

            for (int j = 0; j < MotherOfAllStacksCombo2.Children.Count; j++)
            {
                object child = MotherOfAllStacksCombo2.Children[j];

                string childname = null;
                if (child is FrameworkElement)
                {
                    childname = (child as FrameworkElement).Name; // tager navnet fra et element i comboboxen og gemmer som en string variable..
                    i = (child as ComboBox).SelectedIndex;
                    web = (child as ComboBox).Tag.ToString();
                    if ((child as ComboBox).SelectedValue != null)
                    {
                        cbContent = (child as ComboBox).SelectedValue.ToString();
                    }

                    //string s = (child as ComboBox).SelectedItem.ToString();
                    //string ss = (child as ComboBox).Text;
                }

                if (i != -1) // skal være (i != -1)
                {

                    foreach (var Variable in ekstraList)
                    {
                        if (Variable.Beskrivelse == childname)
                        {
                            foreach (TilbehørClass ev in udlåntTilbehør)
                            {
                                if (mode == "Udlej tilbehør")
                                {
                                    if (ev.VareNavn == Variable.Beskrivelse && ev.Lokation == currentLokation)
                                    {
                                        _ = udlejEkstraAsync(web, currentEventid.ToString(), currentLokation, "Udlånt", ev.Antal + Int32.Parse(cbContent));
                                        ev.Antal = ev.Antal + Int32.Parse(cbContent);
                                        
                                        Scan_Page_TilbehørGrid.Items.Refresh();
                                        _ = GetEkstraAsync();
                                        exits = true;
                                        break;
                                    }
                                }
                                else if (mode == "Aflever tilbehør")
                                {
                                    if (ev.VareNavn == Variable.Beskrivelse && ev.Lokation == currentLokation)
                                    {
                                        _ = udlejEkstraAsync(web, currentEventid.ToString(), currentLokation, "Udlånt", ev.Antal - Int32.Parse(cbContent));
                                        if ((ev.Antal - Int32.Parse(cbContent)) == 0)
                                        {
                                            udlåntTilbehør.Remove(ev);
                                        }
                                        else
                                        {
                                            ev.Antal = ev.Antal - Int32.Parse(cbContent);
                                        }
                                        Scan_Page_TilbehørGrid.Items.Refresh();
                                        _= GetEkstraAsyncAf();
                                        exits = true;
                                        break;
                                    }
                                }

                            }
                            if (mode == "Udlej tilbehør" && exits == false)
                            {
                                _ = udlejEkstraAsync(web, currentEventid.ToString(), currentLokation, "Udlånt",  Int32.Parse(cbContent));
                                udlåntTilbehør.Add(new TilbehørClass(childname, Int32.Parse(cbContent), currentLokation, currentEvent, "Udlånt"));
                                _ = GetEkstraAsync();
                                Scan_Page_TilbehørGrid.Items.Refresh();
                                Variable.Antal = Variable.Antal - i;
                                break;
                            }
                            else if (mode == "Aflever tilbehør" && exits == false)
                            {
                                MessageBox.Show("Du kan ikke aflevere " + Variable.Beskrivelse + " fordi den er ikke udlejet til event lokationen");
                            }

                        }

                    }
                }


            }


        }
        // ------------------------------------------------------------------------------Tilføjer ekstra tilbehør  til griddet som man kan udleje fra. --------------------------
        private async Task<List<ekstraClass>> GetEkstraAsync()
        {
            ekstraList.Clear();
            MotherOfAllStacksCombo1.Children.Clear();
            MotherOfAllStacksCombo2.Children.Clear();
            MotherOfAllStacksLabel1.Children.Clear();
            MotherOfAllStacksLabel2.Children.Clear();

            JArray EkstraResult = await api.GetEkstraAsync();

            int Tæller = 0;
            bool Switchh = true;

            //looper api kald
            foreach (JObject item in EkstraResult)
            {
                int combosetter = 0;
                int j = 0;
                int i = (int)item.GetValue("antal");
                string NameChanger = (string)item.GetValue("beskrivelse") + 'N'; // opretter dynamiske navne til labels da de ikke må hede det samme som comboboxene

                Dispatcher.Invoke(() =>
                {
                    ComboBox newCombo = new ComboBox(); // opretter dynamisk combobox
                    newCombo.Name = (string)item.GetValue("beskrivelse");
                    newCombo.Tag = (string)item.GetValue("webshop");
                    newCombo.Margin = new Thickness(9);
                    newCombo.Height = 23;


                    while (j <= i)
                    {
                        newCombo.Items.Add(j);
                        j = j + 1;
                    }
                    //newCombo.SelectedItem = j -1; /// skal måske bruges. til at pre print højeste antal der er tilbage efter udlejning.


                    Label lbl = new Label();
                    lbl.Name = NameChanger;
                    lbl.Content = (string)item.GetValue("beskrivelse");
                    lbl.FontSize = 22;
                    lbl.Foreground = Brushes.White;
                    lbl.Height = 40;


                    switch (Switchh) // skifter mellem højere og venstre stack så der altid er lige fyldt ud..
                    {
                        case true:
                            MotherOfAllStacksLabel1.Children.Add(lbl);
                            MotherOfAllStacksCombo1.Children.Add(newCombo);
                            Switchh = false;
                            break;
                        case false:
                            MotherOfAllStacksLabel2.Children.Add(lbl);
                            MotherOfAllStacksCombo2.Children.Add(newCombo);
                            Switchh = true;
                            break;
                    }

                    j = 0;

                    Tæller++;
                });

                ekstraList.Add(new ekstraClass((string)item.GetValue("beskrivelse"), (int)item.GetValue("antal"), (string)item.GetValue("webshop"), (string)item.GetValue("id")));
            }
            //TilbehørFunction();
            return ekstraList;

        }

        private async Task<List<ekstraClass>> GetEkstraAsyncAf()
        {

            MotherOfAllStacksCombo1.Children.Clear();
            MotherOfAllStacksCombo2.Children.Clear();
            MotherOfAllStacksLabel1.Children.Clear();
            MotherOfAllStacksLabel2.Children.Clear();

            JArray EkstraResult = await api.GetEkstraAsync();

            int Tæller = 0;
            bool Switchh = true;

            //looper api kald
            foreach (JObject item in EkstraResult)
            {
                foreach(TilbehørClass tc in udlåntTilbehør)
                {
                    if(tc.VareNavn == (string)item.GetValue("beskrivelse"))
                    {
                        int j = 0;
                        int i = tc.Antal;
                        string NameChanger = (string)item.GetValue("beskrivelse") + 'N'; // opretter dynamiske navne til labels da de ikke må hede det samme som comboboxene

                        Dispatcher.Invoke(() =>
                        {
                            ComboBox newCombo = new ComboBox(); // opretter dynamisk combobox
                            newCombo.Name = (string)item.GetValue("beskrivelse");
                            newCombo.Tag = (string)item.GetValue("webshop");
                            newCombo.Margin = new Thickness(9);
                            newCombo.Height = 23;


                            while (j <= i)
                            {
                                newCombo.Items.Add(j);
                                j = j + 1;
                            }
                            //newCombo.SelectedItem = j -1; /// skal måske bruges. til at pre print højeste antal der er tilbage efter udlejning.


                            Label lbl = new Label();
                            lbl.Name = NameChanger;
                            lbl.Content = (string)item.GetValue("beskrivelse");
                            lbl.FontSize = 22;
                            lbl.Foreground = Brushes.White;
                            lbl.Height = 40;


                            switch (Switchh) // skifter mellem højere og venstre stack så der altid er lige fyldt ud..
                            {
                                case true:
                                    MotherOfAllStacksLabel1.Children.Add(lbl);
                                    MotherOfAllStacksCombo1.Children.Add(newCombo);
                                    Switchh = false;
                                    break;
                                case false:
                                    MotherOfAllStacksLabel2.Children.Add(lbl);
                                    MotherOfAllStacksCombo2.Children.Add(newCombo);
                                    Switchh = true;
                                    break;
                            }

                            j = 0;

                            Tæller++;
                        });

                        
                    }
                }

            }
            //TilbehørFunction();
            return ekstraList;

        }

        private async Task<List<TilbehørClass>> udlejEkstraAsync(string webshopNr, string eventId, string eventLokation, string status, int antal)
        {
            JArray scanResult = await api.udlejEkstraAsync(webshopNr, eventId, eventLokation, status, antal);


            return udlåntTilbehør;
        }

        private void TilbehørSkiftMode(object sender, RoutedEventArgs e)
        {

            if (AfleveringBool == true)
            {
                AfleveringBool = false;
                UdlejningBool = true;

                _ = GetEkstraAsync();

                ScanTilbehørButton.Content = "Udlej tilbehør";
            }

            else if (UdlejningBool == true)
            {
                AfleveringBool = true;
                UdlejningBool = false;

                _ = GetEkstraAsyncAf();

                ScanTilbehørButton.Content = "Aflever tilbehør";

            }

            else
            {
                AfleveringBool = true;
                UdlejningBool = false;

                _ = GetEkstraAsyncAf();

                ScanTilbehørButton.Content = "Aflever tilbehør";

            }
        }




        private void EkstraTilbehør_Click(object sender, RoutedEventArgs e)
        {
            
            int TællerTilbehør = 0;

            if (EventNameLaben.Content.ToString() == "Event:")
            {
                MessageBox.Show("Du skal vælge et event før du kan tilføje Tilbehør! ");
                fdg.Focus();
            }
            else if (LokationNameLaben.Content.ToString() == "Lokation:")
            {
                MessageBox.Show("Du skal vælge et lokation før du kan tilføje Tilbehør! ");
            }
            else
            {


                
                TilføjTilbehør.Focusable = false;
                TilbehørGrid.Visibility = Visibility.Visible;
                Keyboard.Focus(MotherOfAllStacksCombo1.Children[0]);



                TilbehørEventLabel.Content = EventNameLaben.Content;
                TilbehørLokationLabel.Content = LokationNameLaben.Content;
                KeyboardNavigation.SetTabNavigation(TilbehørGrid2, KeyboardNavigationMode.Cycle);
                KeyboardNavigation.SetControlTabNavigation(TilbehørGrid2, KeyboardNavigationMode.Cycle);
                KeyboardNavigation.SetDirectionalNavigation(TilbehørGrid2, KeyboardNavigationMode.Cycle);





            }


        }

        



        private void BackScanTilbehørButton_OnClickScanTilbehør(object sender, RoutedEventArgs e)
        {
            TilbehørGrid.Visibility = Visibility.Collapsed;
        }

        //---------------------------------------------------------------------Tilbage knap------------------------------------------------------------------------
        private void goBack_Click(object sender, RoutedEventArgs e)
        {

            NavigationService.Navigate(Pages.p5);

        }
        //------------------------------------------------------------------------Checkboxe eventhandlers til userid--------------------------------------------------

        private void UserId_Checked(object sender, RoutedEventArgs e)
        {
            needUserID = true;

            EventData tempEvent = null;

            if (currentEvent != null)
            {
                foreach (EventData ev in EventsList)
                {
                    if (ev.Name == currentEvent)
                    {
                        tempEvent = ev;
                        break;
                    }
                }

                if (tempEvent.BrugerId == "Nej")
                {
                    MessageBox.Show("Eventet du har valgt er ikke sat til at skulle bruge Hjælper id");
                    UserCheckBox.IsChecked = false;
                }
            }

            if(UserCheckBox.IsChecked == true)
            {
                List<EventData> temp = new List<EventData>();

                if (aktivCheck.IsChecked == false)
                {
                    foreach (EventData ev in EventsList)
                    {
                        if (ev.BrugerId == "Ja")
                        {
                            temp.Add(ev);
                        }
                    }
                }
                else
                {
                    foreach (EventData ev in EventsList)
                    {
                        if (ev.BrugerId == "Ja" && ev.Aktiv == "Ja")
                        {
                            temp.Add(ev);
                        }
                    }
                }

                if (EventStackPanel != null)
                    EventStackPanel.Children.Clear();


                foreach (var item in temp)
                {
                    Button Newbtn = new Button();
                    Newbtn.Content = item.Name;
                    string replace = 'c' + item.ID.ToString();
                    replace = replace.Replace(" ", "");

                    Newbtn.Name = replace;

                    object resource = Application.Current.FindResource("EandLBtn"); //Tager Stylen fra App.xaml der hedder NewScnbtn og tilføre alle styling til knappen.

                    if (resource != null && resource.GetType() == typeof(Style))
                        Newbtn.Style = (Style)resource;

                    EventStackPanel.Children.Add(Newbtn);

                    Newbtn.Click += EventKnapper_Click;
                }
                brugerIdGetBrugteEventsMedVare();
            }
            
                

            

            
        }

        private void UserId_Unchecked(object sender, RoutedEventArgs e)
        {
            needUserID = false;

            List<EventData> temp = new List<EventData>();

            if (aktivCheck.IsChecked == false)
            {
                EventStackPanel.Children.Clear();

                tryhard();
            }
            else
            {
                EventStackPanel.Children.Clear();

                foreach (EventData ev in EventsList)
                {
                    if (ev.Aktiv == "Ja")
                    {
                        temp.Add(ev);
                    }
                }

                if (EventStackPanel != null)
                    EventStackPanel.Children.Clear();


                foreach (var item in temp)
                {
                    Button Newbtn = new Button();

                    

                    Newbtn.Content =  item.Name;
                    string replace = 'c' + item.ID.ToString();
                    replace = replace.Replace(" ", "");

                    Newbtn.Name = replace;

                    object resource = Application.Current.FindResource("EandLBtn"); //Tager Stylen fra App.xaml der hedder NewScnbtn og tilføre alle styling til knappen.

                    if (resource != null && resource.GetType() == typeof(Style))
                        Newbtn.Style = (Style)resource;

                    EventStackPanel.Children.Add(Newbtn);

                    Newbtn.Click += EventKnapper_Click;
                }

                brugerIdGetBrugteEventsMedVare();
            }

            
        }

        // ---------------------------------------------------------------------------------------------------------------- Udvider Scan datagrid og viser tilbehør udlånt og afleveret----------------------------------------
        
        private void TilbehørGridKanpBtn_Click(object sender, RoutedEventArgs e)
        {
            if(EventNameLaben.Content.ToString() == "Event:")
            {
                MessageBox.Show("Du skal vælge et event først før du kan se udlejet tilbehør");
            }
            else
            {
                const string Open = "Vis Udlejet Tilbehør";
                const string NotOpen = "Skjul Udlejet Tilbehør";

                switch (SeekstraTilbehørBtn.Content) // tjekker op på teksten i knappen for at åbne grid med ekstra tilbehør og ændre Content alt efter om den er åben eller lukket.
                {
                    case Open:
                        SeekstraTilbehørBtn.Content = NotOpen;
                        break;

                    case NotOpen:
                        SeekstraTilbehørBtn.Content = Open;
                        break;
                }

                if (!TjekkerSpan && !minimer) // åbner og lukker grid med ekstra tilbehør ved button press
                {
                    Scan_Page_DataGridUdlej.SetValue(Grid.ColumnSpanProperty, 1);
                    Scan_Page_DataGridUdlej.SetValue(Grid.ColumnProperty, 2);
                    Scan_Page_DataGridAflevere.SetValue(Grid.ColumnSpanProperty, 1);
                    Scan_Page_DataGridAflevere.SetValue(Grid.ColumnProperty, 2);

                    ScanBorder.SetValue(Grid.ColumnSpanProperty, 1);
                    ScanBorder.SetValue(Grid.ColumnProperty, 2);
                    Overskrift.SetValue(Grid.ColumnProperty, 1);

                    Overskrift.Visibility = Visibility.Collapsed;
                    Scan_Page_TilbehørGrid.Visibility = Visibility.Visible;
                    TilbehørBorder.Visibility = Visibility.Visible;

                    TjekkerSpan = true;
                }
                else if(TjekkerSpan && !minimer)
                {
                    Scan_Page_DataGridUdlej.SetValue(Grid.ColumnSpanProperty, 2);
                    Scan_Page_DataGridUdlej.SetValue(Grid.ColumnProperty, 1);
                    Scan_Page_DataGridAflevere.SetValue(Grid.ColumnSpanProperty, 2);
                    Scan_Page_DataGridAflevere.SetValue(Grid.ColumnProperty, 1);
                    ScanBorder.SetValue(Grid.ColumnSpanProperty, 2);
                    ScanBorder.SetValue(Grid.ColumnProperty, 1);
                    Overskrift.SetValue(Grid.ColumnProperty, 0);

                    Overskrift.Visibility = Visibility.Visible;


                    Scan_Page_TilbehørGrid.Visibility = Visibility.Collapsed;
                    TilbehørBorder.Visibility = Visibility.Collapsed;

                    TjekkerSpan = false;
                }
                else if(!TjekkerSpan && minimer)
                {
                    TjekkerSpan = true;

                    sidePannel.Visibility = Visibility.Collapsed;

                    Scan_Page_DataGridUdlej.SetValue(Grid.ColumnSpanProperty, 1);
                    Scan_Page_DataGridUdlej.SetValue(Grid.ColumnProperty, 2);
                    Scan_Page_DataGridAflevere.SetValue(Grid.ColumnSpanProperty, 1);
                    Scan_Page_DataGridAflevere.SetValue(Grid.ColumnProperty, 2);
                    ScanBorder.SetValue(Grid.ColumnSpanProperty, 1);
                    ScanBorder.SetValue(Grid.ColumnProperty, 2);
                    Overskrift.SetValue(Grid.ColumnProperty, 1);

                    Overskrift.Visibility = Visibility.Collapsed;
                    Scan_Page_TilbehørGrid.Visibility = Visibility.Visible;
                    TilbehørBorder.Visibility = Visibility.Visible;

                    TilbehørBorder.SetValue(Grid.ColumnProperty, 0);
                    TilbehørBorder.SetValue(Grid.ColumnSpanProperty, 2);
                    TilbehørBorder.Margin = new System.Windows.Thickness(10, 5, 20, 5);

                    Scan_Page_TilbehørGrid.SetValue(Grid.ColumnProperty, 0);
                    Scan_Page_TilbehørGrid.SetValue(Grid.ColumnSpanProperty, 2);
                    Scan_Page_TilbehørGrid.Margin = new System.Windows.Thickness(30, 55, 20, 0);
                }
                else if(TjekkerSpan && minimer)
                {
                    TjekkerSpan = false;

                    Overskrift.Visibility = Visibility.Visible;


                    Scan_Page_TilbehørGrid.Margin = new System.Windows.Thickness(40, 55, 20, 0);
                    Scan_Page_TilbehørGrid.SetValue(Grid.ColumnProperty, 1);
                    Scan_Page_TilbehørGrid.SetValue(Grid.ColumnSpanProperty, 1);
                    TilbehørBorder.Margin = new System.Windows.Thickness(20, 5, 20, 5);
                    TilbehørBorder.SetValue(Grid.ColumnProperty, 1);
                    TilbehørBorder.SetValue(Grid.ColumnSpanProperty, 1);
                    Overskrift.SetValue(Grid.ColumnProperty, 0);



                    Scan_Page_TilbehørGrid.Visibility = Visibility.Collapsed;
                    TilbehørBorder.Visibility = Visibility.Collapsed;


                    ScanBorder.Margin = new System.Windows.Thickness(10, 5, 20, 5);
                    Scan_Page_DataGridUdlej.Margin = new System.Windows.Thickness(30, 55, 20, 0);
                    Scan_Page_DataGridUdlej.SetValue(Grid.ColumnProperty, 0);
                    Scan_Page_DataGridUdlej.SetValue(Grid.ColumnSpanProperty, 3);
                    Scan_Page_DataGridAflevere.Margin = new System.Windows.Thickness(30, 55, 20, 0);
                    Scan_Page_DataGridAflevere.SetValue(Grid.ColumnProperty, 0);
                    Scan_Page_DataGridAflevere.SetValue(Grid.ColumnSpanProperty, 3);
                    ScanBorder.SetValue(Grid.ColumnSpanProperty, 3);
                    ScanBorder.SetValue(Grid.ColumnProperty, 0);
                }
            }
            
        }

        private void ScannerIndstillinger_Click(object sender, RoutedEventArgs e)
        {
            if ((string)Scanner_Laben.Content != "Scanner OFF!")
                return;
            ScannerIndstillingerBorder.Visibility = Visibility.Visible;
            SkjulScannerIndstillinger.Focus();
        }

        private void SetPowerKnap_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ChangeScannerIPKnap_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SetPowerKnapSkan_Click(object sender, RoutedEventArgs e)
        {
            changeStyrkeBorder.Visibility = Visibility.Visible;
            SkjulStyrkeBorder.Focus();
            SetPowerKnapSkan.Visibility = Visibility.Collapsed;
            ChangeScannerIPKnapSkan.Visibility = Visibility.Collapsed;
            SkjulScannerIndstillinger.Visibility = Visibility.Collapsed;
        }

        private void SkjulStyrkeBorder_Click(object sender, RoutedEventArgs e)
        {
            changeStyrkeBorder.Visibility = Visibility.Collapsed;
            SetPowerKnapSkan.Visibility = Visibility.Visible;
            ChangeScannerIPKnapSkan.Visibility = Visibility.Visible;
            SkjulScannerIndstillinger.Visibility = Visibility.Visible;
            SetPowerKnapSkan.Focus();
        }

        private void SkjulScannerIndstillinger_Click(object sender, RoutedEventArgs e)
        {
            ScannerIndstillingerBorder.Visibility = Visibility.Collapsed;
            SetPowerKnapSkan.Visibility = Visibility.Visible;
            ChangeScannerIPKnapSkan.Visibility = Visibility.Visible;
        }

        private void GemChangedIP_Click(object sender, RoutedEventArgs e)
        {
            scannerIP = scannerIPTextBox.Text;
            changeScannerIPSkanPage.Visibility = Visibility.Collapsed;
        }

        private void userAntennaPower_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Hvis man ændre slideren ændre man værdien i scanPowerValue variablen, som er den der styre antenne styrken
            AntennaPowerLabel.Content = userAntennaPower.Value.ToString();
            scanPowerValue = Convert.ToUInt32(userAntennaPower.Value);
        }

        private void ChangeScannerIPKnapSkan_Click(object sender, RoutedEventArgs e)
        {
            changeScannerIPSkanPage.Visibility = Visibility.Visible;
            SetPowerKnapSkan.Visibility = Visibility.Collapsed;
            ChangeScannerIPKnapSkan.Visibility = Visibility.Collapsed;
            SkjulScannerIndstillinger.Visibility = Visibility.Collapsed;
            scannerIPTextBox.Focus();
        }

        private void SkjulIPChanger_Click(object sender, RoutedEventArgs e)
        {
            changeScannerIPSkanPage.Visibility = Visibility.Collapsed;
            SetPowerKnapSkan.Visibility = Visibility.Visible;
            ChangeScannerIPKnapSkan.Visibility = Visibility.Visible;
            SkjulScannerIndstillinger.Visibility = Visibility.Visible;
            SetPowerKnapSkan.Focus();
        }

        private void AddLokationsBtn2_Click(object sender, RoutedEventArgs e)
        {
            // Kontrollere om man har valgt et event før man kan tilføje til det.
            if (EventNameLaben.Content.ToString() != "Event:")
            {
                AddLokationQuick.Visibility = Visibility.Visible;
                QuickAddLocationTxtBox.Focus();
            }
            else
            {
                MessageBox.Show("Du skal vælge et Event før du kan tilføje en Lokation til den!");
            }
        }
        private void AddTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddLokationsBtn_Click(sender, e);
            }
        }
        private void QuickAddLocationTxtBox_KeyDown(object sender, KeyEventArgs e)
        {
            Button btn = new Button();
            if (e.Key == Key.Enter)
            {
                GemLokationButton_Click(btn, e);
            }
        }

        private void AnnulerQuickAddLocation_Click(object sender, RoutedEventArgs e)
        {
            QuickAddLocationTxtBox.Text = "";
            AddLokationQuick.Visibility = Visibility.Collapsed;
        }

        public string RegexSave(string StrToSafe)
        {
            StrToSafe = Regex.Replace(StrToSafe, @"[^0-9a-zA-ZÆØÅæøå]+", "_");
            StrToSafe = Regex.Replace(StrToSafe, @"\s", "_");
            StrToSafe = Regex.Replace(StrToSafe, @"^\d", "_");
            StrToSafe = Regex.Replace(StrToSafe, "½", "_");
            return StrToSafe;
        }

        private void GemLokationButton_Click(object sender, RoutedEventArgs e)
        {
            string LocationText = RegexSave(QuickAddLocationTxtBox.Text);



            string GetEventId = (string)EventNameLaben.Content; // Tager event navn og holder op i iobjectet for at finde EventID
            GetEventId = GetEventId.Replace("Event: ", "");
            string GetEventIDFound = "";
            foreach (var VARIABLE in EventsList)
            {
                if (VARIABLE.Name == GetEventId)
                {
                    GetEventIDFound = VARIABLE.ID.ToString();
                }
            }
            _ = createEventLokation(Int32.Parse(GetEventIDFound), LocationText);

            Button LokationsKnapper = new Button();
            LokationsKnapper.Content = LocationText;
            LokationsKnapper.Name = RegexSave(LocationText);
            object resource =
                Application.Current
                    .FindResource(
                        "EandLBtn"); //Tager Stylen fra App.xaml der hedder NewScnbtn og tilføre alle styling til knappen.
            LokationsKnapper.Style = (Style)resource;

            if (LokationStackPanel.Children.Count >= 2)
            {
                if (resource != null && resource.GetType() == typeof(Style))
                    LokationStackPanel.Children.RemoveAt(LokationStackPanel.Children.Count - 1);
                LokationStackPanel.Children.Add(LokationsKnapper);
                LokationsKnapper.Click += LokationsKnapper_Click;
                TilbageKnap();
            }
            else
            {
                //GetLocation(MidlerTidligtEventId, sender);
            }
            // Close & Hide
            // QuickAddLocationTxtBox.Text = "";
            QuickAddLocationTxtBox.Clear();
            AddLokationQuick.Visibility = Visibility.Collapsed;
        }

        private void aktivCheck_Checked(object sender, RoutedEventArgs e)
        {
            List<EventData> temp = new List<EventData>();


            if(UserCheckBox.IsChecked == false)
            {
                foreach (EventData ev in EventsList)
                {
                    if (ev.Aktiv == "Ja")
                    {
                        temp.Add(ev);
                    }
                }
            }
            else
            {
                foreach (EventData ev in EventsList)
                {
                    if (ev.Aktiv == "Ja" && ev.BrugerId == "Ja")
                    {
                        temp.Add(ev);
                    }
                }
            }
            
            if (EventStackPanel != null)
            EventStackPanel.Children.Clear();

            int i = 0;
            foreach (var item in temp)
            {
                Button Newbtn = new Button();
                Newbtn.Content = item.Name;
                string replace = 'c' + item.ID.ToString();
                replace = replace.Replace(" ", "");

                Newbtn.Name = replace;

                object resource = Application.Current.FindResource("EandLBtn"); //Tager Stylen fra App.xaml der hedder NewScnbtn og tilføre alle styling til knappen.

                if (resource != null && resource.GetType() == typeof(Style))
                    Newbtn.Style = (Style)resource;

                EventStackPanel.Children.Add(Newbtn);

                Newbtn.Click += EventKnapper_Click;
            }
                GetBrugteEventsMedVare();
        }

        private void aktivCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            List<EventData> temp = new List<EventData>();

            if(UserCheckBox.IsChecked == false)
            {
                EventStackPanel.Children.Clear();

                tryhard();
            }
            else
            {
                foreach (EventData ev in EventsList)
                {
                    if (ev.BrugerId == "Ja")
                    {
                        temp.Add(ev);
                    }
                }

                if (EventStackPanel != null)
                    EventStackPanel.Children.Clear();

                int i = 0;
                foreach (var item in temp)
                {
                    Button Newbtn = new Button();
                    Newbtn.Content = item.Name;
                    string replace = 'c' + item.ID.ToString();
                    replace = replace.Replace(" ", "");

                    Newbtn.Name = replace;

                    object resource = Application.Current.FindResource("EandLBtn"); //Tager Stylen fra App.xaml der hedder NewScnbtn og tilføre alle styling til knappen.

                    if (resource != null && resource.GetType() == typeof(Style))
                        Newbtn.Style = (Style)resource;



                    EventStackPanel.Children.Add(Newbtn);

                    Newbtn.Click += EventKnapper_Click;
                }
                    

            }

            GetBrugteEventsMedVare();


        }
        private void scannerIPTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                GemChangedIP_Click(sender, e);
            }
        }

        private void FjernVare_Click(object sender, RoutedEventArgs e)
        {
            if (Scan_Page_DataGridUdlej.SelectedItem == null && Scan_Page_DataGridAflevere.SelectedItem == null) { return; }

            bool udlej = false;

            if (Scan_Page_DataGridUdlej.SelectedItem != null)
            {
                udlej = true;
            }

            //if (Scan_Page_DataGridAflevere.SelectedItem == null) { return; }
            //MessageBox.Show("It worked :)");
            EventVare row;
            if (udlej == false)
            {
                row = (EventVare)Scan_Page_DataGridAflevere.SelectedItem;
            }
            else
            {
                row = (EventVare)Scan_Page_DataGridUdlej.SelectedItem;
            }
            Console.WriteLine(row.Vareid);
            FjernVare(row);
        }
        // TODO hvis status er Hjemme Tilføj hvis status er Hjemme så aflever
        public void FjernVare(EventVare row)
        {
            string vareID = row.Vareid;
            string status = row.Status;
            if (status == "Udlånt")
            {
                string LokationTxt;
                int i = 0;
                int Indextæller = 0;
                int marker = 0;
                string Trigger = "";
                string GetEventId = (string)EventNameLaben.Content; // Tager event navn og holder op i i objectet for at finde EventID
                GetEventId = GetEventId.Replace("Event: ", "");
                string GetEventIDFound = "";

                foreach (var VARIABLE in EventsList)
                {
                    if (VARIABLE.Name == GetEventId)
                    {
                        GetEventIDFound = VARIABLE.ID.ToString();
                    }
                }


                    LokationTxt = LokationNameLaben.Content.ToString().Replace("Lokation: ", ""); // Ændre Lokations lablen og Fjerne "Lokation:" med ingenting så det kun er selv lokationen der bliver gemt i listen.
                    i = 0;
                    if (NewList.Count == 0)
                    {

                    }
                    else
                    {
                    /*for (int j = 0; j > NewList.Count; j++)
                    {
                        if (NewList[j].Vareid.Equals(vareID))
                        {
                            i++;

                            _ = scanVareId(GetEventIDFound, vareID, LokationTxt, "Hjemme");

                            GetEventVareAfleveret(Int32.Parse(GetEventIDFound), "Hjemme");
                            GetEventVare(Int32.Parse(GetEventIDFound), "Udlånt");

                            Scan_Page_DataGridAflevere.Items.Refresh();
                            Trigger = "onefound"; // sætter Trigger som en "trigger" så vores if kan se en ændring og derefter slette varen fra udlejet datagrid.
                            marker = Indextæller; // gemmer indexet som vi nedenfor sletter med. dermed dynamsik.
                            NewList[j].EventLokation = LokationTxt;


                            string Name = NewList[j].Beskrivelse;
                            string hylde = NewList[j].EventLokation;
                            string krog = NewList[j].PinNr;


                            Label itemNavn = new Label();
                            itemNavn.Content = Name;
                            itemNavn.Height = 55;
                            itemNavn.Foreground = Brushes.White;
                            itemNavn.Background = userColor;
                            itemNavn.BorderBrush = Brushes.White;
                            itemNavn.FontSize = 30;
                            itemNavn.HorizontalContentAlignment = HorizontalAlignment.Center;
                            itemNavn.BorderThickness = new Thickness(2.0);

                            Label itemHylde = new Label();
                            itemHylde.Content = hylde;
                            itemHylde.Height = 55;
                            itemHylde.Foreground = Brushes.White;
                            itemHylde.Background = userColor;
                            itemHylde.BorderBrush = Brushes.White;
                            itemHylde.FontSize = 30;
                            itemHylde.HorizontalContentAlignment = HorizontalAlignment.Center;
                            itemHylde.BorderThickness = new Thickness(2.0);

                            Label itemKrog = new Label();
                            itemKrog.Content = krog;
                            itemKrog.Height = 55;
                            itemKrog.Foreground = Brushes.White;
                            itemKrog.Background = userColor;
                            itemKrog.BorderBrush = Brushes.White;
                            itemKrog.FontSize = 30;
                            itemKrog.HorizontalContentAlignment = HorizontalAlignment.Center;
                            itemKrog.BorderThickness = new Thickness(2.0);

                            AfWindow.UserStackpanelNavn.Children.Insert(0, itemNavn);
                            AfWindow.UserStackpanelHylde.Children.Insert(0, itemHylde);
                            AfWindow.UserStackpanelKrog.Children.Insert(0, itemKrog);

                            if (AfWindow.UserStackpanelNavn.Children.Count == 11 && AfWindow.UserStackpanelHylde.Children.Count == 11 && AfWindow.UserStackpanelKrog.Children.Count == 11)
                            {
                                AfWindow.UserStackpanelNavn.Children.RemoveAt(10);
                                AfWindow.UserStackpanelHylde.Children.RemoveAt(10);
                                AfWindow.UserStackpanelKrog.Children.RemoveAt(10);
                            }
                        }

                        Indextæller++;

                    }*/
                        foreach (var Item in NewList)
                        {

                            if (Item.Vareid.Equals(vareID))
                            {
                                i++;

                                _ = scanVareId(GetEventIDFound, vareID, LokationTxt, "Hjemme");
                            MessageBox.Show(GetEventIDFound + " " + vareID);

                                GetEventVareAfleveret(Int32.Parse(GetEventIDFound), "Hjemme");
                                GetEventVare(Int32.Parse(GetEventIDFound), "Udlånt");

                                Scan_Page_DataGridAflevere.Items.Refresh();
                                Trigger = "onefound"; // sætter Trigger som en "trigger" så vores if kan se en ændring og derefter slette varen fra udlejet datagrid.
                                marker = Indextæller; // gemmer indexet som vi nedenfor sletter med. dermed dynamsik.
                                Item.EventLokation = LokationTxt;


                                string Name = Item.Beskrivelse;
                                string hylde = Item.EventLokation;
                                string krog = Item.PinNr;


                                Label itemNavn = new Label();
                                itemNavn.Content = Name;
                                itemNavn.Height = 55;
                                itemNavn.Foreground = Brushes.White;
                                itemNavn.Background = userColor;
                                itemNavn.BorderBrush = Brushes.White;
                                itemNavn.FontSize = 30;
                                itemNavn.HorizontalContentAlignment = HorizontalAlignment.Center;
                                itemNavn.BorderThickness = new Thickness(2.0);

                                Label itemHylde = new Label();
                                itemHylde.Content = hylde;
                                itemHylde.Height = 55;
                                itemHylde.Foreground = Brushes.White;
                                itemHylde.Background = userColor;
                                itemHylde.BorderBrush = Brushes.White;
                                itemHylde.FontSize = 30;
                                itemHylde.HorizontalContentAlignment = HorizontalAlignment.Center;
                                itemHylde.BorderThickness = new Thickness(2.0);

                                Label itemKrog = new Label();
                                itemKrog.Content = krog;
                                itemKrog.Height = 55;
                                itemKrog.Foreground = Brushes.White;
                                itemKrog.Background = userColor;
                                itemKrog.BorderBrush = Brushes.White;
                                itemKrog.FontSize = 30;
                                itemKrog.HorizontalContentAlignment = HorizontalAlignment.Center;
                                itemKrog.BorderThickness = new Thickness(2.0);

                                AfWindow.UserStackpanelNavn.Children.Insert(0, itemNavn);
                                AfWindow.UserStackpanelHylde.Children.Insert(0, itemHylde);
                                AfWindow.UserStackpanelKrog.Children.Insert(0, itemKrog);

                                if (AfWindow.UserStackpanelNavn.Children.Count == 11 && AfWindow.UserStackpanelHylde.Children.Count == 11 && AfWindow.UserStackpanelKrog.Children.Count == 11)
                                {
                                    AfWindow.UserStackpanelNavn.Children.RemoveAt(10);
                                    AfWindow.UserStackpanelHylde.Children.RemoveAt(10);
                                    AfWindow.UserStackpanelKrog.Children.RemoveAt(10);
                                }
                            }

                            Indextæller++;
                        }
                     }
            }
            if (status == "Hjemme")
            {
                string GetEventId = (string)EventNameLaben.Content; // Tager event navn og holder op i i objectet for at finde EventID
                GetEventId = GetEventId.Replace("Event: ", "");
                string GetEventIDFound = "";

                foreach (var VARIABLE in EventsList)
                {
                    if (VARIABLE.Name == GetEventId)
                    {
                        GetEventIDFound = VARIABLE.ID.ToString();
                    }
                }
                string prøve = LokationNameLaben.Content.ToString();

                    if (prøve == "Lokation:")
                    {
                        MessageBox.Show("Du skal vælge en lokation for at arbejde med vare!");
                    }
                    else
                    {
                        EventVare o = null;
                        string LokationTxt = LokationNameLaben.Content.ToString().Replace("Lokation: ", ""); // Ændre Lokations lablen og Fjerne "Lokation:" med ingenting så det kun er selv lokationen der bliver gemt i listen.
                        int i = 0;
                        if (NewList.Count == 0)
                        {
                            i++;

                            _ = scanVareId(GetEventIDFound, vareID, LokationTxt, "Udlånt");

                            GetEventVare(Int32.Parse(GetEventIDFound), "Udlånt");
                            GetEventVareAfleveret(Int32.Parse(GetEventIDFound), "Hjemme");

                            Scan_Page_DataGridUdlej.Items.Refresh();
                        }
                        else
                        {
                            foreach (var Item in NewList)
                            {

                                if (!Item.Vareid.Equals(vareID))
                                {
                                    i++;

                                    _ = scanVareId(GetEventIDFound, vareID, LokationTxt, "Udlånt");

                                    GetEventVare(Int32.Parse(GetEventIDFound), "Udlånt");
                                    GetEventVareAfleveret(Int32.Parse(GetEventIDFound), "Hjemme");

                                    Scan_Page_DataGridUdlej.Items.Refresh();
                                }


                            }
                        }


                        if (i == 0)
                            MessageBox.Show("Varen er desværre ikke på lager!");

                        else
                            i = 0;


                }
            }
        }


        
        private void TilbageKnapEvent_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Globalvar.scannerOn == true) { MessageBox.Show("Vi tillader ikke at du forlader Scan Page før du har slukket scanneren!"); return; }
            NavigationService.Navigate(Pages.p5);
        }

        private void TilbageKnapEvent_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            TilbageKnapEvent.Foreground = (Brush)bc.ConvertFrom("#CCC");
        }

        private void TilbageKnapEvent_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            TilbageKnapEvent.Foreground = (Brush)bc.ConvertFrom("#FFF");
        }

        private void hide_Click(object sender, RoutedEventArgs e)
        {
            if (!minimer && !TjekkerSpan)
            {
                minimer = true;

                hidePic.RenderTransformOrigin = new Point(0.5, 0.5);

                hidePic.RenderTransform = new RotateTransform(180);


                Setter setter = new Setter();

                setter.Property = MarginProperty;
                setter.Value = new System.Windows.Thickness(0, 0, 40, 0); 

                sidePannel.Visibility = Visibility.Collapsed;
                ScanBorder.Margin = new System.Windows.Thickness(10, 5, 20, 5);
                Scan_Page_DataGridUdlej.Margin = new System.Windows.Thickness(30, 55, 20, 0);
                Scan_Page_DataGridUdlej.SetValue(Grid.ColumnProperty, 0);
                Scan_Page_DataGridUdlej.SetValue(Grid.ColumnSpanProperty, 3);

                Scan_Page_DataGridAflevere.Margin = new System.Windows.Thickness(30, 55, 20, 0);
                Scan_Page_DataGridAflevere.SetValue(Grid.ColumnProperty, 0);
                Scan_Page_DataGridAflevere.SetValue(Grid.ColumnSpanProperty, 3);
                ScanBorder.SetValue(Grid.ColumnSpanProperty, 3);
                ScanBorder.SetValue(Grid.ColumnProperty, 0);

            }
            else if(minimer && !TjekkerSpan)
            {
                minimer = false;

                hidePic.RenderTransformOrigin = new Point(0.5, 0.5);

                hidePic.RenderTransform = new RotateTransform(0);

                sidePannel.Visibility = Visibility.Visible;
                ScanBorder.Margin = new System.Windows.Thickness(20, 5, 20, 5);
                Scan_Page_DataGridUdlej.Margin = new System.Windows.Thickness(40, 55, 20, 0);
                Scan_Page_DataGridUdlej.SetValue(Grid.ColumnProperty, 1);
                Scan_Page_DataGridUdlej.SetValue(Grid.ColumnSpanProperty, 2);
                Scan_Page_DataGridAflevere.Margin = new System.Windows.Thickness(40, 55, 20, 0);
                Scan_Page_DataGridAflevere.SetValue(Grid.ColumnProperty, 1);
                Scan_Page_DataGridAflevere.SetValue(Grid.ColumnSpanProperty, 2);
                ScanBorder.SetValue(Grid.ColumnSpanProperty, 2);
                ScanBorder.SetValue(Grid.ColumnProperty, 1);
            }
            else if(!minimer && TjekkerSpan)
            {
                minimer = true;

                hidePic.RenderTransformOrigin = new Point(0.5, 0.5);

                hidePic.RenderTransform = new RotateTransform(180);

                sidePannel.Visibility = Visibility.Collapsed;

                Scan_Page_DataGridUdlej.SetValue(Grid.ColumnSpanProperty, 1);
                Scan_Page_DataGridUdlej.SetValue(Grid.ColumnProperty, 2);
                Scan_Page_DataGridAflevere.SetValue(Grid.ColumnSpanProperty, 1);
                Scan_Page_DataGridAflevere.SetValue(Grid.ColumnProperty, 2);
                ScanBorder.SetValue(Grid.ColumnSpanProperty, 1);
                ScanBorder.SetValue(Grid.ColumnProperty, 2);
                Overskrift.SetValue(Grid.ColumnProperty, 1);

                Overskrift.Visibility = Visibility.Collapsed;
                Scan_Page_TilbehørGrid.Visibility = Visibility.Visible;
                TilbehørBorder.Visibility = Visibility.Visible;

                TilbehørBorder.SetValue(Grid.ColumnProperty, 0);
                TilbehørBorder.SetValue(Grid.ColumnSpanProperty, 2);
                TilbehørBorder.Margin = new System.Windows.Thickness(10, 5, 20, 5);

                Scan_Page_TilbehørGrid.SetValue(Grid.ColumnProperty, 0);
                Scan_Page_TilbehørGrid.SetValue(Grid.ColumnSpanProperty, 2);
                Scan_Page_TilbehørGrid.Margin = new System.Windows.Thickness(30, 55, 20, 0);

            }
            else if(minimer && TjekkerSpan)
            {
                minimer = false;

                hidePic.RenderTransformOrigin = new Point(0.5, 0.5);

                hidePic.RenderTransform = new RotateTransform(0);

                sidePannel.Visibility = Visibility.Visible;
                Scan_Page_DataGridUdlej.SetValue(Grid.ColumnSpanProperty, 1);
                Scan_Page_DataGridUdlej.SetValue(Grid.ColumnProperty, 2);
                Scan_Page_DataGridAflevere.SetValue(Grid.ColumnSpanProperty, 1);
                Scan_Page_DataGridAflevere.SetValue(Grid.ColumnProperty, 2);
                ScanBorder.SetValue(Grid.ColumnSpanProperty, 1);
                ScanBorder.SetValue(Grid.ColumnProperty, 2);
                Overskrift.SetValue(Grid.ColumnProperty, 1);

                Overskrift.Visibility = Visibility.Collapsed;
                Scan_Page_TilbehørGrid.Visibility = Visibility.Visible;
                TilbehørBorder.Visibility = Visibility.Visible;

                TilbehørBorder.SetValue(Grid.ColumnProperty, 1);
                TilbehørBorder.SetValue(Grid.ColumnSpanProperty, 1);
                TilbehørBorder.Margin = new System.Windows.Thickness(20, 5, 20, 5);

                Scan_Page_TilbehørGrid.SetValue(Grid.ColumnProperty, 1);
                Scan_Page_TilbehørGrid.SetValue(Grid.ColumnSpanProperty, 1);
                Scan_Page_TilbehørGrid.Margin = new System.Windows.Thickness(40, 55, 20, 0);
            }
            
        }

        private async Task<List<EventVare>> EditEventVareStatusAsync(string id, string status)
        {

            JArray VareResult = await api.EditEventVareStatusAsync(id, status);
            return NewList;

        }

        private void AktivScannerBox_Checked(object sender, RoutedEventArgs e)
        {
            erScannerAktiv = true;
        }

        private void AktivScannerBox_Unchecked(object sender, RoutedEventArgs e)
        {
            erScannerAktiv = false;
        }

        private void confirmStatus_Click(object sender, RoutedEventArgs e)
        {
            int id = 0;
            string nyStatus = "";

            if (udlejStatus == true)
            {
                EventVare tempStatus = (EventVare)Scan_Page_DataGridUdlej.SelectedItem;

                ComboBoxItem status = (ComboBoxItem)statusCombo.SelectedItem;

                tempStatus.Status = status.Content.ToString();

                nyStatus = status.Content.ToString();

                Scan_Page_DataGridUdlej.Items.Refresh();

                foreach (EventVare ev in NewList)
                {
                    if (ev.Vareid == tempStatus.Vareid && ev.EventName == currentEvent && tempStatus.Status != "Hjemme")
                    {
                        id = ev.Id;
                        break;
                    }else if(ev.Vareid == tempStatus.Vareid && ev.EventName == currentEvent && tempStatus.Status == "Hjemme")
                    {
                        id = ev.Id;
                        NewList.Remove(ev);
                        AfleveretList.Insert(0, ev);
                        
                        Scan_Page_DataGridAflevere.Items.Refresh();
                        
                        break;
                    }
                }

                
                Scan_Page_DataGridUdlej.SelectedItem = null;
                udlejStatus = false;
            }
            else if(afleverStatus == true)
            {
                EventVare tempStatus = (EventVare)Scan_Page_DataGridAflevere.SelectedItem;

                ComboBoxItem status = (ComboBoxItem)statusCombo.SelectedItem;

                tempStatus.Status = status.Content.ToString();

                nyStatus = status.Content.ToString();

                Scan_Page_DataGridUdlej.Items.Refresh();

                foreach (EventVare ev in AfleveretList)
                {
                    if (ev.Vareid == tempStatus.Vareid && ev.EventName == currentEvent && tempStatus.Status != "Udlånt")
                    {
                        id = ev.Id;
                        break;
                    }
                    else if (ev.Vareid == tempStatus.Vareid && ev.EventName == currentEvent && tempStatus.Status == "Udlånt")
                    {
                        id = ev.Id;
                        AfleveretList.Remove(ev);
                        NewList.Insert(0, ev);
                        Scan_Page_DataGridAflevere.Items.Refresh();
                        Scan_Page_DataGridUdlej.Items.Refresh();
                        break;
                    }
                }

                
                afleverStatus = false;
            }

            _ = EditEventVareStatusAsync(id.ToString(), nyStatus);
            editStatusGrid.Visibility = Visibility.Collapsed;
            //MessageBox.Show(tempStatus.Beskrivelse);
        }

        private void annullerStatus_Click(object sender, RoutedEventArgs e)
        {
            udlejStatus = false;
            Scan_Page_DataGridUdlej.SelectedItem = null;
            editStatusGrid.Visibility = Visibility.Collapsed;
        }

        private void lukHistorik_Click(object sender, RoutedEventArgs e)
        {
            HistorikGrid.Visibility = Visibility.Collapsed;
        }
    }
}




/*string Event = EventNameLaben.Content.ToString();
            Event = Event.Replace("Event: ", "");
            string Lokation = LokationNameLaben.Content.ToString();
            Lokation = Lokation.Replace("Lokation: ", "");


            string TilbehørStatus = "Udlånt";

            if (UdlejningBool == true)
            {
                TilbehørStatus = "Udlånt";
            }
            else if (AfleveringBool == true)
            {
                TilbehørStatus = "Hjemme";
            }

            try
            {
                TilbehørClass tilbehørscannet = new TilbehørClass(TilbehørNavnText.Text,
                    Int32.Parse(AntalTilbehørText.Text), Lokation, Event, TilbehørStatus); // skal tage event og lokation der allerede er valgt

                if (tilbehørscannet.Events != "Vælg Event" && String.IsNullOrEmpty(AntalTilbehørText.Text) == false &&
                    String.IsNullOrEmpty(TilbehørNavnText.Text) == false) // if fejler
                {
                    ScannetTilbehørList.Add(tilbehørscannet);

                    //TilbehørGrid.Visibility = Visibility.Collapsed;

                    TilbehørNavnText.Clear();
                    AntalTilbehørText.Clear();

                    Scan_Page_TilbehørGrid.Items.Add(tilbehørscannet);

                }
                else
                {
                    MessageBox.Show("Et eller flere input felter blev ikke udfyldt! Prøv igen.");
                }
            }
            catch
            {
                MessageBox.Show("Fejl. Prøv igen.");
            }*/