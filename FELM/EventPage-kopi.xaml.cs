using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using Microsoft.Win32;
using System.ComponentModel;
using Excel = Microsoft.Office.Interop.Excel; //Hent nugget package Microsoft.Office.Interop.Excel
using System.Globalization;


namespace FELM
{
    /// <summary>
    /// Interaction logic for EventPage.xaml
    /// </summary>
    public partial class EventPage_kopi : Page
    {
        // forskellige variabler brugt til funktioner
        public static List<EventClass> myList = new List<EventClass>();
        public static List<lokationerClass> lokationList = new List<lokationerClass>();
        public static List<VareClass> eventVareList = new List<VareClass>();
        public static List<ExportData> exportList = new List<ExportData>();
        ICollectionView cvVare = CollectionViewSource.GetDefaultView(eventVareList);
        public string searchstr;
        API EventApi = new API();
        bool ToggleEvent = false;
        string eventstr = " ";
        string redigerId;
        string lokationStr;
        string NewLokation;
        string currentEventName;
        string checkBoxKopir_Checked_String;
        int currentEventId;
        bool toogleOmbytte = false;
        bool ombytte = false;
        bool checkBoxKopir_Checked_Bool;
        VareClass bytteVare1;
        VareClass bytteVare2;

        public EventPage_kopi()
        {
            InitializeComponent();
            maList();

            EventGrid.ItemsSource = myList;
        }

        public void maList()
        {
            myList.Add(new EventClass("12", "asd", "asd", "asd", "asd", "asd", "asd", "asd", "asd", "true"));
            lokationList.Add(new lokationerClass("12", "scene"));
            lokationList.Add(new lokationerClass("12", "ølhus"));
            lokationList.Add(new lokationerClass("12", "el-lager"));

        }

        void Onload(object sender, RoutedEventArgs e)
        {
            _ = AllEventsListQueryAsync();
        }

        public bool Check_Event()
        {
            if (string.IsNullOrEmpty(EventName.Text) ||
               string.IsNullOrEmpty(StartDatoBox.Text) ||
               string.IsNullOrEmpty(SlutDatoBox.Text) ||
               string.IsNullOrEmpty(NoteBox.Text) ||
               string.IsNullOrEmpty(DieselstanderStartBox.Text) ||
               string.IsNullOrEmpty(DielselstanderSlutBox.Text) ||
               string.IsNullOrEmpty(DefektePærerBox.Text) ||
               string.IsNullOrEmpty(TlfNummerBox.Text))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private async Task<JArray> CreateEvent(string name, string startDato, string slutDato, string note, string dieselStart, string dieselSlut, string defektPærer, string tlfNr, string aktiv)
        {
            JArray stuff = await EventApi.CreateEventAsync(name, startDato, slutDato, note, dieselStart, dieselSlut, defektPærer, tlfNr, aktiv);
            return stuff;
        }

        public void Tilføjning_af_Event()
        {
            if (Check_Event())
            {

                DateTime startDato;
                DateTime slutDato;

                if(DateTime.TryParseExact(StartDatoBox.Text, "yyyy-MM-dd HH:mm:ss", new CultureInfo("da-DK"), DateTimeStyles.None, out startDato) && DateTime.TryParseExact(SlutDatoBox.Text, "yyyy-MM-dd HH:mm:ss", new CultureInfo("da-DK"), DateTimeStyles.None, out slutDato))
                {
                    var new_row =
                    new EventClass(
                        "",
                        EventName.Text,
                        startDato.ToString("yyyy-MM-dd HH:mm:ss"),
                        slutDato.ToString("yyyy-MM-dd HH:mm:ss"),
                        NoteBox.Text,
                        DieselstanderStartBox.Text,
                        DielselstanderSlutBox.Text,
                        DefektePærerBox.Text,
                        TlfNummerBox.Text,
                        AktivComboBox.Text
                    );

                    myList.Add(new_row);
                    _ = CreateEvent(
                            EventName.Text,
                            startDato.ToString("yyyy-MM-dd HH:mm:ss"),
                            slutDato.ToString("yyyy-MM-dd HH:mm:ss"),
                            NoteBox.Text,
                            DieselstanderStartBox.Text,
                            DielselstanderSlutBox.Text,
                            DefektePærerBox.Text,
                            TlfNummerBox.Text,
                            AktivComboBox.Text
                        );

                    EventName.Text = "";
                    StartDatoBox.Text = "";
                    SlutDatoBox.Text = "";
                    NoteBox.Text = "";
                    DieselstanderStartBox.Text = "";
                    DielselstanderSlutBox.Text = "";
                    DefektePærerBox.Text = "";
                    TlfNummerBox.Text = "";

                    MessageBox.Show("Eventet er oprettet");
                }
                else
                {
                    MessageBox.Show("Datoen skal skrives som følgene: år-måned-dag timer:minutter");
                }

            }
            else
            {
                MessageBox.Show("Vær gerne sikker på at du har udfyldt felterne " +
                    "(EventName, " +
                    "StartDato, " +
                    "SlutDato, " +
                    "Note, " +
                    "DieselstanderStart, " +
                    "DielselstanderSlut, " +
                    "DefektePærer, " +
                    "TlfNummer," +
                    "Aktiv.");
            }
        }

        private void NumberValidation(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9.,]+");

            if (e.Handled = re.IsMatch(e.Text))
            {
                MessageBox.Show("Slet ikke tilladt");
            }
        }

        private void Tilføj_Button(object sender, RoutedEventArgs e)
        {
            Tilføjning_af_Event();
            EventGrid.Items.Refresh();
        }

        // helt events fra database til liste 
        private async Task<List<EventClass>> AllEventsListQueryAsync()
        {
            // api kald
            JArray EventResult = await EventApi.AllEventsListQueryAsync();

            myList.Clear();
            // hent events 
            foreach (JObject item in EventResult)
            {
                string evnentId = (string)item.GetValue("EventId");
                string eventName = (string)item.GetValue("EventName");
                string eventStartDato = (string)item.GetValue("EventStartDato");
                string eventSlutDato = (string)item.GetValue("EventSlutDato");
                string eventNote = (string)item.GetValue("EventNote");
                string eventDieselstanderStart = (string)item.GetValue("EventDieselStart");
                string eventDielselstanderSlut = (string)item.GetValue("EventDieselSlut");
                string eventDefektePærer = (string)item.GetValue("EventDefekt");
                string eventTlfNummer = (string)item.GetValue("EventTlf");
                string eventAktiv = (string)item.GetValue("EventAktiv");


                // tilføj events til lisre
                myList.Add(new EventClass(evnentId, eventName, eventStartDato, eventSlutDato, eventNote, eventDieselstanderStart, eventDielselstanderSlut, eventDefektePærer, eventTlfNummer, eventAktiv));
            }


            // opfrisk liste
            EventGrid.Items.Refresh();



            return myList;
        }

        private async Task<JArray> EditEventAsync(string navn, string newnavn, string startdato, string slutdato, string note, string dieselstart, string dieselslut, string defektepære, string tlfnr, string aktiv)
        {
            JArray editEvent = await EventApi.EditEvent(navn, newnavn, startdato, slutdato, note, dieselstart, dieselslut, defektepære, tlfnr, aktiv);
            return editEvent;
        }

        private void RedigerGem_button(object sender, RoutedEventArgs e)
        {
            string startDate = null;
            string slutDate = null;

            DateTime startDate2;
            DateTime slutDate2;

            bool done = false;
            int pos = 0;

            while (done == false)
            {
                if (myList[pos].EventId == redigerId)
                {
                    if (StartDatoBox.Text != null && StartDatoBox.Text != "" )
                    {
                        if (DateTime.TryParseExact(StartDatoBox.Text, "yyyy-MM-dd HH:mm:ss", new CultureInfo("da-DK"), DateTimeStyles.None, out startDate2) && DateTime.TryParseExact(SlutDatoBox.Text, "yyyy-MM-dd HH:mm:ss", new CultureInfo("da-DK"), DateTimeStyles.None, out slutDate2))
                        {
                            startDate = startDate2.ToString("yyyy-MM-dd HH:mm:ss");
                            slutDate = slutDate2.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        else
                        {
                            MessageBox.Show("Datoen skal skrives som følgene: år-måned-dag timer:minutter:sekunder");
                            break;
                        }
                    }
                    else
                    {
                        startDate = "";
                        slutDate = "";
                    }

                    myList[pos] = new EventClass(redigerId,
                                                 EventName.Text,
                                                 startDate,
                                                 slutDate,
                                                 NoteBox.Text,
                                                 DieselstanderStartBox.Text,
                                                 DielselstanderSlutBox.Text,
                                                 DefektePærerBox.Text,
                                                 TlfNummerBox.Text,
                                                 AktivComboBox.Text);

                    _ = EditEventAsync(myList[pos].EventName,
                                       EventName.Text,
                                       startDate,
                                       slutDate,
                                       NoteBox.Text,
                                       DieselstanderStartBox.Text,
                                       DielselstanderSlutBox.Text,
                                       DefektePærerBox.Text,
                                       TlfNummerBox.Text,
                                       AktivComboBox.Text);

                    EventGrid.Items.Refresh();

                    EventName.Text = "";
                    StartDatoBox.Text = "";
                    SlutDatoBox.Text = "";
                    NoteBox.Text = "";
                    DieselstanderStartBox.Text = "";
                    DielselstanderSlutBox.Text = "";
                    DefektePærerBox.Text = "";
                    TlfNummerBox.Text = "";

                    RedigerGem.Visibility = Visibility.Collapsed;
                    done = true;
                    redigerId = null;

                    MessageBox.Show("Event er blevet redigeret");
                }
                else
                {
                    pos += 1;
                }
            }
        }
        private async void Lokation_button(object sender, RoutedEventArgs e)
        {

            var row = (EventClass)EventGrid.SelectedItem;

            if(row != null)
            {
                lokationPanel.Children.Clear();
                addLokationStack.Visibility = Visibility.Collapsed;
                lokationList.Clear();


                JArray eventLocations = await EventApi.GetEventLokation(Int32.Parse(row.EventId));

                if(eventLocations != null)
                {
                    foreach (JObject item in eventLocations)
                    {
                        int eventId = (int)item.GetValue("EventLokationEventId");
                        string eventLokation = (string)item.GetValue("Lokation");
                        lokationList.Add(new lokationerClass(eventId.ToString(), eventLokation));
                    }

                    bool emptyOrNot = false;
                    int count = 0;
                    bool done = false;

                    while (done == false && count < lokationList.Count())
                    {

                        if (lokationList[count].EventId == row.EventId)
                        {
                            emptyOrNot = true;
                            done = true;
                        }
                        else
                        {
                            count += 1;
                            emptyOrNot = false;
                        }
                    }

                    if (emptyOrNot == true)
                    {
                        BrushConverter bc = new BrushConverter();
                        EventsBoxes.Visibility = Visibility.Collapsed;
                        lokationScroll.Visibility = Visibility.Visible;
                        for (int i = 0; i < lokationList.Count(); i++)
                        {
                            if (lokationList[i].EventId == row.EventId)
                            {
                                Button myButton = new Button();

                                if (i % 2 == 0)
                                {
                                    myButton.Background = (Brush)bc.ConvertFrom("#00b5a3");
                                }
                                else
                                {
                                    myButton.Background = (Brush)bc.ConvertFrom("#017056");
                                }

                                myButton.Name = $"LokationButton{i}";
                                myButton.Content = lokationList[i].Lokation;
                                myButton.HorizontalAlignment = HorizontalAlignment.Stretch;
                                myButton.HorizontalContentAlignment = HorizontalAlignment.Center;
                                myButton.VerticalAlignment = VerticalAlignment.Stretch;
                                myButton.VerticalContentAlignment = VerticalAlignment.Stretch;
                                myButton.FontWeight = FontWeights.Bold;
                                myButton.BorderThickness = new Thickness(0);
                                myButton.Foreground = (Brush)bc.ConvertFrom("#FFFFFF");
                                myButton.Margin = new Thickness(0, 10, 0, 0);

                                lokationPanel.Children.Add(myButton);

                                myButton.Click += (s, se) =>
                                {
                                    lokationScroll.Visibility = Visibility.Collapsed;
                                    redigerLokationStack.Visibility = Visibility.Visible;
                                    lokationStr = myButton.Content.ToString();
                                    lokationRedigerBox.Text = myButton.Content.ToString();
                                    redigerId = row.EventId;
                                };
                            }
                        }

                        Button AnnullerButton = new Button();
                        AnnullerButton.Content = "Annuller";
                        AnnullerButton.HorizontalContentAlignment = HorizontalAlignment.Center;
                        AnnullerButton.FontWeight = FontWeights.Bold;
                        AnnullerButton.BorderThickness = new Thickness(0);
                        AnnullerButton.Background = (Brush)bc.ConvertFrom("#00b5a3");
                        AnnullerButton.Foreground = (Brush)bc.ConvertFrom("#FFFFFF");
                        AnnullerButton.Margin = new Thickness(0, 40, 0, 0);
                        AnnullerButton.Height = 24;
                        AnnullerButton.Width = 130;

                        lokationPanel.Children.Add(AnnullerButton);

                        AnnullerButton.Click += (s, se) =>
                        {
                            lokationPanel.Children.Clear();
                            EventsBoxes.Visibility = Visibility.Visible;
                        };

                    }
                }
                else
                {
                    MessageBox.Show("Der er ingen Lokationer i denne event");
                }
       
            }
            else
            {
                MessageBox.Show("Du har ikke valgt et event");
            }
            
        }

        private void RedigereLokationButton_Click(object sender, RoutedEventArgs e)
        {
            int pos = 0;
            bool done = false;
            NewLokation = lokationRedigerBox.Text;

            if (NewLokation != null && lokationStr != NewLokation)
            {
                while (done == false && pos < lokationList.Count())
                {
                    if (lokationList[pos].EventId == redigerId && lokationList[pos].Lokation == lokationStr)
                    {
                        lokationList[pos] = new lokationerClass(redigerId, NewLokation);
                        redigerLokationStack.Visibility = Visibility.Collapsed;
                        EventsBoxes.Visibility = Visibility.Visible;
                        MessageBox.Show("lokation er blevet redigeret");
                    }
                    else
                    {
                        pos += 1;
                    }
                }
            }
            else
            {
                MessageBox.Show("Feltet er inten tom eller også er der ikke blevet gjort nogen ændring");
            }

        }

        private void Annuller_Click(object sender, RoutedEventArgs e)
        {


            if (ToggleEvent == true)
            {
                StatusPanel.Visibility = Visibility.Collapsed;
                KopierEventPanel.Visibility = Visibility.Collapsed;
                EventsStackPanelView.Visibility = Visibility.Visible;
            }
            else
            {
                redigerLokationStack.Visibility = Visibility.Collapsed;
                addLokationStack.Visibility = Visibility.Collapsed;
                EventsBoxes.Visibility = Visibility.Visible;
            }
        }

        //bliver kaldt når man klikker på en event knap
        private async void Event_Click(object sender, EventArgs e, int id, string eventName)
        {
            try
            {

                eventVareList.Clear();
                //Henter alle varer der er på et event
                JArray loadResult = await EventApi.GetItemsQueryAsync(id); //JArray som indeholder alt der passer på idInt i databasen
                currentEventId = id;
                currentEventName = eventName;

                if (loadResult != null)
                {
                    SpecificEventButtons.Visibility = Visibility.Visible;

                    foreach (JObject item in loadResult)
                    {
                        string itemBeskrivelse = (string)item.GetValue("beskrivelse");
                        int itemAntal = (int)item.GetValue("antal");
                        string itemStatus = (string)item.GetValue("eventVareStatus");
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
                        string itemEventVareId = (string)item.GetValue("eventVareId");


                        eventVareList.Add(new VareClass(itemVarenummer, itemBeskrivelse, itemTilgang, itemAfgang, itemAmpere, itemStatus, itemAntal, itemVareLokation, itemPinNr, itemLength, itemFavorit, itemNote, itemWebshopNummer, itemRfidNummer, itemQrKode, itemEventVareId));

                    }
                    EventVareGrid.ItemsSource = eventVareList;
                    EventVareGrid.Items.Refresh();

                }
                else
                {
                    MessageBox.Show("Der er ikke noget indhold at vise");
                }
            }
            catch
            {
                MessageBox.Show("Der er gået noget galt med forbindelsen til internettet eller databasen.");
            }

        }



        //laver alle event knapper når man trykker på "Events" knappen
        private async Task<JArray> PullEventsToButtons()
        {
            JArray eventResult = await EventApi.AllEventsQueryAsync();
            if (eventResult != null)
            {
                EventsStackPanel.Children.Clear();
                BrushConverter bc = new BrushConverter();
                int i = 1;
                foreach (JObject item in eventResult)
                {
                    string eventName = (string)item.GetValue("EventName");
                    string eventId = (string)item.GetValue("EventId");
                    Button myButton = new Button();

                    if (i % 2 == 0)
                    {
                        myButton.Background = (Brush)bc.ConvertFrom("#00b5a3");
                    }
                    else
                    {
                        myButton.Background = (Brush)bc.ConvertFrom("#017056");
                    }

                    //button customization
                    myButton.Name = $"Eventsbutton{eventId}";
                    myButton.Content = eventName;
                    myButton.Width = 420;
                    myButton.HorizontalAlignment = HorizontalAlignment.Center;
                    myButton.VerticalAlignment = VerticalAlignment.Center;
                    myButton.FontWeight = FontWeights.Bold;
                    myButton.BorderThickness = new Thickness(0);
                    myButton.Foreground = (Brush)bc.ConvertFrom("#FFFFFF");
                    myButton.Margin = new Thickness(0, 10, 0, 0);

                    ToggleEvent = true;

                    //sætter click events der sender eventid til event_click
                    myButton.Click += (s, EventArgs) =>
                    {
                        Regex re = new Regex("[^a-zA-Z]+");

                        eventVareList.Clear();
                        EventVareGrid.Items.Refresh();
                        eventstr = myButton.Name;
                        Match idString = re.Match(eventstr);

                        if (idString.Success)
                        {
                            int id = int.Parse(idString.Value);
                            Event_Click(s, EventArgs, id, eventName);
                        }

                    };

                    EventsStackPanel.Children.Add(myButton);
                    i++;
                }
            }
            else
            {
                MessageBox.Show("Der er ikke noget indhold at vise.");
            }
            return eventResult;
        }

        //Knap der aktivere events
        private void Start_Events(object sender, RoutedEventArgs e)
        {
            eventVareList.Clear();
            EventVareGrid.Items.Refresh();

            if (ToggleEvent == false)
            {
                try
                {
                    EventButtons.Visibility = Visibility.Collapsed;
                    RedigerGem.Visibility = Visibility.Collapsed;
                    addEvent.Visibility = Visibility.Collapsed;
                    lokationPanel.Children.Clear();
                    addLokationStack.Visibility = Visibility.Collapsed;

                    favLabel.Visibility = Visibility.Visible;
                    statusLabel.Visibility = Visibility.Visible;
                    searchLabel.Visibility = Visibility.Visible;
                    txtSearchBox.Visibility = Visibility.Visible;
                    fav.Visibility = Visibility.Visible;
                    status.Visibility = Visibility.Visible;

                    EventsStackPanelView.Visibility = Visibility.Visible;
                    Events.Visibility = Visibility.Collapsed;

                    EventVareBorder.Visibility = Visibility.Visible;
                    EventVareGrid.Visibility = Visibility.Visible;

                    EventGrid.Visibility = Visibility.Collapsed;
                    EventGridBorder.Visibility = Visibility.Collapsed;

                    _ = PullEventsToButtons();

                }
                catch
                {
                    MessageBox.Show("Der er gået noget galt med forbindelsen til internettet eller databasen.");
                }


            }
            else if (ToggleEvent == true)
            {
                SpecificEventButtons.Visibility = Visibility.Collapsed;

                KopierEventPanel.Visibility = Visibility.Collapsed;
                OmbytVarePanel.Visibility = Visibility.Collapsed;

                EventsStackPanelView.Visibility = Visibility.Collapsed;

                EventVareBorder.Visibility = Visibility.Collapsed;
                EventVareGrid.Visibility = Visibility.Collapsed;

                StatusPanel.Visibility = Visibility.Collapsed;

                favLabel.Visibility = Visibility.Collapsed;
                statusLabel.Visibility = Visibility.Collapsed;
                searchLabel.Visibility = Visibility.Collapsed;
                txtSearchBox.Visibility = Visibility.Collapsed;
                fav.Visibility = Visibility.Collapsed;
                status.Visibility = Visibility.Collapsed;

                EventButtons.Visibility = Visibility.Visible;
                addEvent.Visibility = Visibility.Visible;
         
                Events.Visibility = Visibility.Visible;
                EventsBoxes.Visibility = Visibility.Visible;

                EventGrid.Visibility = Visibility.Visible;
                EventGridBorder.Visibility = Visibility.Visible;

                ToggleEvent = false;
                EventGrid.ItemsSource = myList;
                EventGrid.Items.Refresh();

            }

        }
        private void EventGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var row = (EventClass)EventGrid.SelectedItem;
            EventsBoxes.Visibility = Visibility.Visible;
            lokationPanel.Children.Clear();
            addLokationStack.Visibility = Visibility.Collapsed;

            if (row != null)
            {
                if(row.StartDato != null && row.StartDato != "" && row.SlutDato != null && row.SlutDato != "")
                {
                    DateTime startDato;
                    DateTime slutDato;
                    if(DateTime.TryParseExact(row.StartDato, "yyyy-MM-dd HH:mm:ss", null, DateTimeStyles.None, out startDato) && DateTime.TryParseExact(row.SlutDato, "yyyy-MM-dd HH:mm:ss", null, DateTimeStyles.None, out slutDato))
                    {
                        StartDatoBox.Text = startDato.ToString("yyyy-MM-dd HH:mm:ss");
                        SlutDatoBox.Text = slutDato.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    else
                    {
                        StartDatoBox.Text = DateTime.ParseExact(row.StartDato, "yyyy-MM-dd HH:mm:ss", null).ToString("yyyy-MM-dd HH:mm:ss");
                        SlutDatoBox.Text = DateTime.ParseExact(row.SlutDato, "yyyy-MM-dd HH:mm:ss", null).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }
                else
                {
                    StartDatoBox.Text = "";
                    SlutDatoBox.Text = "";
                }

                redigerId = row.EventId;
                EventName.Text = row.EventName;
                EventName.Text = row.EventName;
                NoteBox.Text = row.Note;
                DieselstanderStartBox.Text = row.DieselstanderStart;
                DielselstanderSlutBox.Text = row.DielselstanderSlut;
                DefektePærerBox.Text = row.DefektePærer;
                TlfNummerBox.Text = row.TlfNummer;
                RedigerGem.Visibility = Visibility.Visible;
            }
        }

        private void ExportPdf_Click(object sender, RoutedEventArgs e)
        {
            // sorter vare listen efter WebshopVareNummer 
            List<VareClass> order = eventVareList.OrderBy(o => o.WebshopVareNummer).ToList();

            // bestem hvor pdf skal gemmes og hvad den skal hedde
            if (EventVareGrid.Items.Count > 0)
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

                        //skriv til pdf fil
                        PdfWriter writer = PdfWriter.GetInstance(document, fs);

                        //gem en font
                        BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

                        iTextSharp.text.Font Headerfont = new iTextSharp.text.Font(bf, 5, iTextSharp.text.Font.BOLD);
                        iTextSharp.text.Font font = new iTextSharp.text.Font(bf, 5, iTextSharp.text.Font.NORMAL);

                        // lav tabeller 
                        PdfPTable headerTable = new PdfPTable(EventVareGrid.Columns.Count);
                        headerTable.DefaultCell.Padding = 2;
                        headerTable.WidthPercentage = 100;
                        headerTable.HorizontalAlignment = Element.ALIGN_LEFT;
                        PdfPTable vareTable = new PdfPTable(EventVareGrid.Columns.Count);
                        vareTable.DefaultCell.Padding = 2;
                        vareTable.WidthPercentage = 100;
                        vareTable.HorizontalAlignment = Element.ALIGN_LEFT;



                        // tilføj indehold til tabeller
                        int cPos = 0;
                        int rPos = 0;


                        while (cPos < EventVareGrid.Columns.Count)
                        {
                            PdfPCell cell = new PdfPCell(new Phrase(EventVareGrid.Columns[cPos].Header.ToString(), Headerfont));
                            headerTable.AddCell(cell);
                            cPos += 1;

                        }


                        while (rPos < EventVareGrid.Items.Count)
                        {
                            PdfPCell VareNr = new PdfPCell(new Phrase(order[rPos].VareNr, font));
                            PdfPCell beskrivelse = new PdfPCell(new Phrase(order[rPos].Beskrivelse, font));
                            PdfPCell Tilgang = new PdfPCell(new Phrase(order[rPos].Tilgang, font));
                            PdfPCell Afgang = new PdfPCell(new Phrase(order[rPos].Afgang, font));
                            PdfPCell Antal = new PdfPCell(new Phrase(order[rPos].Antal.ToString(), font));
                            PdfPCell Status = new PdfPCell(new Phrase(order[rPos].Status, font));
                            PdfPCell VareLokation = new PdfPCell(new Phrase(order[rPos].VareLokation, font));
                            PdfPCell Ampere = new PdfPCell(new Phrase(order[rPos].Ampere, font));
                            PdfPCell PinNr = new PdfPCell(new Phrase(order[rPos].PinNr, font));
                            PdfPCell Længde = new PdfPCell(new Phrase(order[rPos].Længde.ToString(), font));
                            PdfPCell WebshopVareNummer;
                            PdfPCell RFIDNummer = new PdfPCell(new Phrase(order[rPos].RFIDNummer, font));
                            PdfPCell Note = new PdfPCell(new Phrase(order[rPos].Note, font)); ;
                            PdfPCell QR = new PdfPCell(new Phrase(order[rPos].QR, font));

                            if (order[rPos].WebshopVareNummer == null || order[rPos].WebshopVareNummer == "")
                            {
                                WebshopVareNummer = new PdfPCell(new Phrase("99999", font));
                            }
                            else
                            {
                                WebshopVareNummer = new PdfPCell(new Phrase(order[rPos].WebshopVareNummer, font));
                            };

                            vareTable.AddCell(VareNr);
                            vareTable.AddCell(beskrivelse);
                            vareTable.AddCell(Tilgang);
                            vareTable.AddCell(Afgang);
                            vareTable.AddCell(Antal);
                            vareTable.AddCell(Status);
                            vareTable.AddCell(VareLokation);
                            vareTable.AddCell(Ampere);
                            vareTable.AddCell(PinNr);
                            vareTable.AddCell(Længde);
                            vareTable.AddCell(WebshopVareNummer);
                            vareTable.AddCell(RFIDNummer);

                            vareTable.AddCell(Note);
                            vareTable.AddCell(QR);
                            rPos += 1;

                        }

                        // fyld pdf egenskab ud
                        document.AddAuthor("Femu");
                        document.AddKeywords("Liste med vare til et event");
                        document.AddSubject("Vare Liste");
                        document.AddTitle(saveFile.FileName);

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

                        // åben og luk panneler efter behov
                        eventVareList.Clear();
                        EventVareGrid.Items.Refresh();
                        ExportPdf.Visibility = Visibility.Collapsed;
                        // succes besked
                        MessageBox.Show("Succes vare listen er blevet gemt");




                    }
                    // fejl besked
                    catch (IOException)
                    {
                        MessageBox.Show("Kunne ikke lave/overskrive pdf filen. \nHvis du har filen åben så luk filen og prøv igen.\nHvis filen ikke er åben så tjek om filen er skrivebeskyttet.");
                    }

                }
                // fejl besked
                else
                {
                    MessageBox.Show("Du gemte ikke filen");
                }


            }
            // fejl besked
            else
            {
                MessageBox.Show("Din liste er tom prøv igen på med en liste der ikke er tom");
            }

        }



        //Eksportere eventvare til csv fra et eventid
        private async void ExportCsv_Click(object sender, RoutedEventArgs e)
        {
            /*string path = currentEventName + "_vare.csv";
            path = path.Replace(" ", "_");*/
            Excel.Application xlexcel;
            object misValue = System.Reflection.Missing.Value;
            xlexcel = new Excel.Application();
            xlexcel.Visible = true;

            xlexcel.Workbooks.Add();
            Excel._Worksheet workSheet = (Excel.Worksheet)xlexcel.ActiveSheet;

            JArray loadResult = await EventApi.EventAndVareQueryAsync(currentEventId); //JArray som indeholder alt der passer på idInt i databasen

            if (loadResult != null)
            {

                foreach (JObject item in loadResult)
                {
                    string exportStartDato = (string)item.GetValue("EventStartDato");
                    string exportSlutDato = (string)item.GetValue("EventSlutDato");
                    string exportVarenummer = (string)item.GetValue("webshopNummer");
                    string exportAntal = (string)item.GetValue("eventVareCount");

                    //Parser string date til type DateTime for at convert det til ISO_8601 string date format :)
                    var parsedDate = DateTime.Parse(exportStartDato);
                    var parsedStartDato = parsedDate.ToString("yyyy-MM-ddTHH:mm:ss:fffZ");
                    var parsedDate2 = DateTime.Parse(exportSlutDato);
                    var parsedSlutDato = parsedDate2.ToString("yyyy-MM-ddTHH:mm:ss:fffZ");

                    exportList.Add(new ExportData(parsedStartDato, parsedSlutDato, exportVarenummer, exportAntal));
                }

            }


            //Laver Excell headers
            workSheet.Cells[1, "A"] = "fromDate";
            workSheet.Cells[1, "B"] = "toDate";
            workSheet.Cells[1, "C"] = "number";
            workSheet.Cells[1, "D"] = "count";
            workSheet.Columns[1].AutoFit();
            workSheet.Columns[2].AutoFit();
            workSheet.Columns[3].AutoFit();
            workSheet.Columns[4].AutoFit();
            ((Excel.Range)workSheet.Columns[1]).ColumnWidth = 23;
            ((Excel.Range)workSheet.Columns[2]).ColumnWidth = 23;
            ((Excel.Range)workSheet.Columns[3]).AutoFit();
            ((Excel.Range)workSheet.Columns[4]).AutoFit();

            //Fylder cells ud med data fra api call
            int i = 2;
            foreach (var rows in exportList)
            {
                workSheet.Cells[i, "A"] = rows.StartDato;
                workSheet.Cells[i, "B"] = rows.SlutDato;
                workSheet.Cells[i, "C"] = rows.Varenummer;
                workSheet.Cells[i, "D"] = rows.Antal;
                i++;
            }
            //Måske bedre uden at den automatisk gemmer filen
            //workSheet.SaveAs(path, Excel.XlFileFormat.xlCSV);
        }


        //Aktivere det venstre panel til at kopiere events
        private void CopyEvent_Button(object sender, RoutedEventArgs e)
        {
            Events.Visibility = Visibility.Collapsed;
            EventsStackPanelView.Visibility = Visibility.Collapsed;
            StatusPanel.Visibility = Visibility.Collapsed;
            OmbytVarePanel.Visibility = Visibility.Collapsed;

            KopierEventPanel.Visibility = Visibility.Visible;

        }

        //Knap til at vise ændringerne der bliver lavet når man kopiere et event
        private void ShowVareCopy_Click(object sender, RoutedEventArgs e)
        {
            var result = eventVareList.FindAll(v => v.Status == "Udlånt");
            eventVareList.Clear();
            foreach (var item in result)
            {
                eventVareList.Add(new VareClass(item.VareNr, item.Beskrivelse, item.Tilgang, item.Afgang, item.Ampere, item.Status, item.Antal, item.VareLokation, item.PinNr, item.Længde, item.Favorit, item.Note, item.WebshopVareNummer, item.RFIDNummer, item.QR));
            }
            EventVareGrid.Items.Refresh();
        }

        //Kopier event api call som skal bruge et eventid og et nyt navn til det nye event
        private async Task<JArray> CopyEvent(int id, string name, string copy)
        {
            JArray stuff = await EventApi.CopyEventAsync(id, name, copy);
            return stuff;
        }

        //Knap til at bekræfte kopier event som kalder api kaldet til at kopiere event og refresh event knapper
        private void ConfirmCopyEvent_Click(object sender, RoutedEventArgs e)
        {

            string newName = NewEventName.Text;
            if (newName != "")
            {
                //Kører funktion til at copy event
                _ = CopyEvent(currentEventId, newName, checkBoxKopir_Checked_String);
                MessageBox.Show("Event Kopieret");
                //Kører funktion til at opdatere events til knapper
                _ = PullEventsToButtons();
                KopierEventPanel.Visibility = Visibility.Collapsed;
                EventsStackPanelView.Visibility = Visibility.Visible;
                KopierEvent_button.Visibility = Visibility.Collapsed;

            }
            else
            {
                MessageBox.Show("Være venligst sikker på at du har givet dit event et navn");
            }

            // første parameter er den der skal kopiers fra currentEventId og anden er den skal kopier til det nye eventId
            //if (checkBoxKopir_Checked_Bool)
            //{
            //    insertLokation(id.ToString(), id.ToString());
            //}

        }

        //Knap til at aktivere venstre panel til at skifte status på et event vare
        private void ChangeStatus_Button(object sender, RoutedEventArgs e)
        {

            Events.Visibility = Visibility.Collapsed;
            EventsStackPanelView.Visibility = Visibility.Collapsed;
            KopierEventPanel.Visibility = Visibility.Collapsed;
            OmbytVarePanel.Visibility = Visibility.Collapsed;

            StatusPanel.Visibility = Visibility.Visible;

        }

        //Api call til at edit event vare som skal bruge id (ikke eventId) og status som det skal ændres til
        private async Task<List<VareClass>> EditEventVareStatusAsync(string id, string status)
        {

            JArray VareResult = await EventApi.EditEventVareStatusAsync(id, status);
            return eventVareList;

        }

        //Knap til at ændre status på et event vare
        private void ConfirmStatusChangeButton_Click(object sender, RoutedEventArgs e)
        {
            var row = (VareClass)EventVareGrid.SelectedItem;
            int pos = 0;
            bool done = false;
            string newStatus = ChangeStatusBox.Text;

            if (row != null && newStatus != "" && row.Status != newStatus)
            {
                //Looper igennem datagrid for at finde positionen på det item du har valgt
                while (done == false && pos < eventVareList.Count())
                {
                    //Hvis position data stemmer overens med det valgte datagrid data -> gå amok
                    if (eventVareList[pos].VareNr == row.VareNr && eventVareList[pos].Status == row.Status)
                    {
                        _ = EditEventVareStatusAsync(eventVareList[pos].EventVareId, newStatus);
                        eventVareList[pos] = new VareClass(row.VareNr, row.Beskrivelse, row.Tilgang, row.Afgang, row.Ampere, newStatus, row.Antal, row.VareLokation, row.PinNr, row.Længde, row.Favorit, row.Note, row.WebshopVareNummer, row.RFIDNummer, row.QR);

                        EventsStackPanelView.Visibility = Visibility.Visible;
                        StatusPanel.Visibility = Visibility.Collapsed;

                        EventVareGrid.Items.Refresh();
                        done = true;
                        MessageBox.Show("Status er blevet redigeret");
                    }
                    else
                    {
                        pos += 1;
                    }
                }
            }
            else
            {
                MessageBox.Show("Feltet er enten tomt eller også er der ikke blevet lavet en ændring");
            }
        }

        // Gør feltet hvor man kan tilfæje lokationer synligt eller usynligt
        private void addLokation_Click(object sender, RoutedEventArgs e)
        {
            var row = (EventClass)EventGrid.SelectedItem;
            lokationPanel.Children.Clear();

            addLokationStack.Visibility = Visibility.Visible;
            EventsBoxes.Visibility = Visibility.Collapsed;
  

        }

        private async Task<List<VareClass>> createEventLokation(int id, string lokation)
        {
            JArray VareResult = await EventApi.CreateEventLokationAsync(id, lokation);
            return eventVareList;
        }

        private void addLokationButton_Click(object sender, RoutedEventArgs e)
        {
            var row = (EventClass)EventGrid.SelectedItem;
            lokationPanel.Children.Clear();

            // Tilføj lokationer til event

            if (row != null && lokationAdd.Text != "")
            {
                lokationList.Add(new lokationerClass(row.EventId, lokationAdd.Text));
                _ = createEventLokation(Convert.ToInt32(row.EventId), lokationAdd.Text);
                lokationAdd.Text = "";
            }
            else // Hvis et felt ikke er markeret
            {
                MessageBox.Show("Marker venligst et felt!");
            }

            // Ændre om feltet er synligt

            addLokationStack.Visibility = Visibility.Collapsed;
            EventsBoxes.Visibility = Visibility.Visible;



        }

        bool kopirEventVisible = false;

        // Kopir Lokationen fra et ID til et Andet
        private void kopiLokationButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> lokationEvents = new List<string>();

            // Tag alle entries fra lokationList hvor ID'et Matcher den første boks i textfeltet "lokationID1" og put dem i lokationEvents listen

            foreach (var item in lokationList)
            {
                if (item.EventId == lokationID1.Text)
                {
                    lokationEvents.Add(item.Lokation.ToString());
                }

            }

            // Tilføj alle entries fra lokationEvents og put dem i lokations listen
            for (int i = 0; i < lokationEvents.Count; i++)
            {
                lokationList.Add(new lokationerClass(lokationID2.Text, lokationEvents[i]));
            }

            // Skjul feltet hvis det er synligt og vis det hvis det er skjult

            if (kopirEventVisible == true)
            {
                lokationKopiStack.Visibility = Visibility.Collapsed;
                EventsBoxes.Visibility = Visibility.Visible;
                kopirEventVisible = false;
            }
            else
            {
                lokationKopiStack.Visibility = Visibility.Collapsed;
                EventsBoxes.Visibility = Visibility.Visible;
                kopirEventVisible = true;
            }


        }
        // åben panel til at ombytte vare 
        private void Ombytte_button(object sender, RoutedEventArgs e)
        {   
            toogleOmbytte = true;
            Events.Visibility = Visibility.Collapsed;
            EventsStackPanelView.Visibility = Visibility.Collapsed;
            OmbytVarePanel.Visibility = Visibility.Visible;
            StatusPanel.Visibility = Visibility.Collapsed;
            KopierEventPanel.Visibility = Visibility.Collapsed;          
        }

        // ombyt 2 vares lokationer efter man har valgt 2 vare
        private void ombytteVareButton_Click(object sender, RoutedEventArgs e)
        {
            // variabler til at gemme de valgte vare i og deres lokationer
            int vare1 = 0;
            int vare2 = 0;
            string str1 = "";
            string str2 = "";

            // stopper loopen
            bool gate = false;
            // tjek om der er valgt 2 vare og hvis ikke skal der komme en fejl
            if (bytteVare1 == null && bytteVare2 == null || bytteVare1 != null && bytteVare2 == null)
            {
                MessageBox.Show("Du har ikke valgt 2 vare til at bytte med hinanden");
            }
            // køre en loop til at bytte de 2 vares lokationer
            else
            {
                int pos = 0;
                bool done = false;
                while (pos < eventVareList.Count && done == false)
                {
                    if (eventVareList[pos].VareNr == bytteVare1.VareNr && gate == false)
                    {
                        vare1 = pos;
                        str1 = bytteVare2.VareLokation;
                        gate = true;
                        pos = 0;
                    }
                    else if (eventVareList[pos].VareNr == bytteVare2.VareNr && gate == true)
                    {
                        vare2 = pos;
                        str2 = bytteVare1.VareLokation;
                        pos = 0;
                        done = true;

                    }
                    else
                    {
                        pos += 1;
                    }
                }

                // opfrisk listen
                eventVareList[vare1].VareLokation = str1;
                eventVareList[vare2].VareLokation = str2;
                EventVareGrid.Items.Refresh();

                // succes besked
                MessageBox.Show("Varerne er blevet byttet");

                // gør klare til næste gang
                ombytteVare1.Content = "Vælg den første vare";
                ombytteVare2.Content = "Vælg den anden vare";

                // åben og luk de forskellige paneler 
                toogleOmbytte = false;
                ombytte = false;

                EventsStackPanelView.Visibility = Visibility.Visible;
                OmbytVarePanel.Visibility = Visibility.Collapsed;

            }
        }

        // fortryd at ombytte vare 
        private void ombytteAnnuller_Click(object sender, RoutedEventArgs e)
        {
            // gør klar til næste gang
            ombytteVare1.Content = "Vælg den første vare";
            ombytteVare2.Content = "Vælg den anden vare";

            // åben og luk de forskellige paneler 
            toogleOmbytte = false;
            ombytte = false;
            EventsStackPanelView.Visibility = Visibility.Visible;
            OmbytVarePanel.Visibility = Visibility.Collapsed;

        }

        // vælg forskellige vare at bytte om
        private void EventVareGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //gem den valgte vare i en variabel
            var row = (VareClass)EventVareGrid.SelectedItem;

            // gem den første vare
            if (toogleOmbytte == true && ombytte == false)
            {
                ombytteVare1.Content = row.Beskrivelse;
                bytteVare1 = row;
                ombytte = true;
            }
            //gem den anden vare
            else if (toogleOmbytte == true && ombytte == true)
            {
                //fejl besked hvis vælger den samme vare 2 gange
                if (row == bytteVare1)
                {
                    MessageBox.Show("Du har valgt den samme vare igen vælge en anden vare");
                }
                else
                {
                    ombytteVare2.Content = row.Beskrivelse;
                    bytteVare2 = row;
                    ombytte = false;
                }
            }



        }

        private void txtSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (textbox != null)
            {
                // Hvis søgebar ikke er tom skal den filtre
                searchstr = textbox.Text;
                if (!string.IsNullOrEmpty(searchstr))
                {
                    cvVare.Filter = new Predicate<object>(filter);
                }
                // hvis søgebare er tom skal den stop med at filtre og opfriske listen
                else
                {
                    cvVare.Refresh();
                    EventVareGrid.ItemsSource = cvVare;

                }


            }
        }

        // filtre til søgebare, favorit checkbox og status dropdown
        private bool filter(object o)
        {
            // gem hvad der står i søgebaren i en variabel
            string searchhWord = txtSearchBox.Text;
            // split hver søgeord og gem dem i en array
            string[] searchhWords = Regex.Split(searchhWord, ", ");

            //bliver brugt til at vælge vare der godkendt og til at stoppe loopen
            bool result = false;

            // stopper loopen
            bool end = true;


            int pos = 0;


            if (o is VareClass)
            {
                VareClass v = (o as VareClass);

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

                // Køre en loop der går igennem alle søge ord og filtre vare efter dem
                while (result == false && end == true)
                {
                    // hvis favorit chekbox er ikke trykket og status er tom
                    if (searchhWords.Length == 1 && fav.IsChecked == false && val == " ")
                    {
                        if ((v.Beskrivelse.ToUpper().Contains(searchhWords[pos].ToUpper()) || v.Tilgang.ToUpper().Contains(searchhWords[pos].ToUpper()) || v.Afgang.ToUpper().Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.Ampere.ToUpper()).Contains(searchhWords[pos].ToUpper()) ||
                                v.Note.ToUpper().Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.VareLokation.ToUpper()).Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.VareNr.ToUpper()).Contains(searchhWords[pos].ToUpper())
                                || Convert.ToString(v.Antal).Contains(searchhWords[pos]) || Convert.ToString(v.PinNr.ToUpper()).Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.WebshopVareNummer.ToUpper()).Contains(searchhWords[pos].ToUpper())
                                || Convert.ToString(v.Længde).Contains(searchhWords[pos]) || Convert.ToString(v.RFIDNummer.ToUpper()).Contains(searchhWords[pos].ToUpper())))
                        {

                            result = true;

                        }
                        else
                        {
                            end = false;

                        }
                    }
                    else if ((searchhWords.Length - 1) > pos && fav.IsChecked == false && val == " ")
                    {
                        if ((v.Beskrivelse.ToUpper() == (searchhWords[pos].ToUpper()) || v.Tilgang.ToUpper() == (searchhWords[pos].ToUpper()) || v.Afgang.ToUpper() == (searchhWords[pos].ToUpper()) || Convert.ToString(v.Ampere.ToUpper()) == (searchhWords[pos].ToUpper()) ||
                                v.Note.ToUpper() == (searchhWords[pos].ToUpper()) || Convert.ToString(v.VareLokation.ToUpper()) == (searchhWords[pos].ToUpper()) || Convert.ToString(v.VareNr.ToUpper()) == (searchhWords[pos].ToUpper())
                                || Convert.ToString(v.Antal) == (searchhWords[pos]) || Convert.ToString(v.PinNr.ToUpper()) == (searchhWords[pos].ToUpper()) || Convert.ToString(v.WebshopVareNummer.ToUpper()) == (searchhWords[pos].ToUpper())
                                || Convert.ToString(v.Længde) == (searchhWords[pos]) || Convert.ToString(v.RFIDNummer.ToUpper()) == (searchhWords[pos].ToUpper())))
                        {
                            pos += 1;

                        }
                        else
                        {
                            pos += 1;
                            end = false;

                        }
                    }
                    else if ((searchhWords.Length - 1) == pos && fav.IsChecked == false && val == " ")
                    {
                        if ((v.Beskrivelse.ToUpper().Contains(searchhWords[pos].ToUpper()) || v.Tilgang.ToUpper().Contains(searchhWords[pos].ToUpper()) || v.Afgang.ToUpper().Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.Ampere.ToUpper()).Contains(searchhWords[pos].ToUpper()) ||
                                v.Note.ToUpper().Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.VareLokation.ToUpper()).Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.VareNr.ToUpper()).Contains(searchhWords[pos].ToUpper())
                                || Convert.ToString(v.Antal).Contains(searchhWords[pos]) || Convert.ToString(v.PinNr.ToUpper()).Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.WebshopVareNummer.ToUpper()).Contains(searchhWords[pos].ToUpper())
                                || Convert.ToString(v.Længde).Contains(searchhWords[pos]) || Convert.ToString(v.RFIDNummer.ToUpper()).Contains(searchhWords[pos].ToUpper())))
                        {
                            pos += 1;
                            result = true;
                        }
                        else
                        {
                            pos += 1;
                            end = false;

                        }
                    }
                    // hvis favorit er ikke trykket og status ikke er tom
                    if (searchhWords.Length == 1 && fav.IsChecked == false && val != " ")
                    {
                        if ((v.Status == val) && (v.Beskrivelse.ToUpper().Contains(searchhWords[pos].ToUpper()) || v.Tilgang.ToUpper().Contains(searchhWords[pos].ToUpper()) || v.Afgang.ToUpper().Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.Ampere.ToUpper()).Contains(searchhWords[pos].ToUpper()) ||
                                v.Note.ToUpper().Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.VareLokation.ToUpper()).Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.VareNr.ToUpper()).Contains(searchhWords[pos].ToUpper())
                                || Convert.ToString(v.Antal).Contains(searchhWords[pos]) || Convert.ToString(v.PinNr.ToUpper()).Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.WebshopVareNummer.ToUpper()).Contains(searchhWords[pos].ToUpper())
                                || Convert.ToString(v.Længde).Contains(searchhWords[pos]) || Convert.ToString(v.RFIDNummer.ToUpper()).Contains(searchhWords[pos].ToUpper())))
                        {

                            result = true;

                        }
                        else
                        {
                            end = false;

                        }
                    }
                    else if ((searchhWords.Length - 1) > pos && fav.IsChecked == false && val != " ")
                    {
                        if ((v.Status == val) && (v.Beskrivelse.ToUpper() == (searchhWords[pos].ToUpper()) || v.Tilgang.ToUpper() == (searchhWords[pos].ToUpper()) || v.Afgang.ToUpper() == (searchhWords[pos].ToUpper()) || Convert.ToString(v.Ampere.ToUpper()) == (searchhWords[pos].ToUpper()) ||
                                v.Note.ToUpper() == (searchhWords[pos].ToUpper()) || Convert.ToString(v.VareLokation.ToUpper()) == (searchhWords[pos].ToUpper()) || Convert.ToString(v.VareNr.ToUpper()) == (searchhWords[pos].ToUpper())
                                || Convert.ToString(v.Antal) == (searchhWords[pos]) || Convert.ToString(v.PinNr.ToUpper()) == (searchhWords[pos].ToUpper()) || Convert.ToString(v.WebshopVareNummer.ToUpper()) == (searchhWords[pos].ToUpper())
                                || Convert.ToString(v.Længde) == (searchhWords[pos]) || Convert.ToString(v.RFIDNummer.ToUpper()) == (searchhWords[pos].ToUpper())))
                        {
                            pos += 1;

                        }
                        else
                        {
                            pos += 1;
                            end = false;

                        }
                    }
                    else if ((searchhWords.Length - 1) == pos && fav.IsChecked == false && val != " ")
                    {
                        if ((v.Status == val) && (v.Beskrivelse.ToUpper().Contains(searchhWords[pos].ToUpper()) || v.Tilgang.ToUpper().Contains(searchhWords[pos].ToUpper()) || v.Afgang.ToUpper().Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.Ampere.ToUpper()).Contains(searchhWords[pos].ToUpper()) ||
                                v.Note.ToUpper().Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.VareLokation.ToUpper()).Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.VareNr.ToUpper()).Contains(searchhWords[pos].ToUpper())
                                || Convert.ToString(v.Antal).Contains(searchhWords[pos]) || Convert.ToString(v.PinNr.ToUpper()).Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.WebshopVareNummer.ToUpper()).Contains(searchhWords[pos].ToUpper())
                                || Convert.ToString(v.Længde).Contains(searchhWords[pos]) || Convert.ToString(v.RFIDNummer.ToUpper()).Contains(searchhWords[pos].ToUpper())))
                        {
                            pos += 1;
                            result = true;
                        }
                        else
                        {
                            pos += 1;
                            end = false;

                        }
                    }

                    // hvis favorit er trykket og status er tom
                    if (searchhWords.Length == 1 && fav.IsChecked == true && val == " ")
                    {
                        if ((v.Favorit == 1) && (v.Beskrivelse.ToUpper().Contains(searchhWords[pos].ToUpper()) || v.Tilgang.ToUpper().Contains(searchhWords[pos].ToUpper()) || v.Afgang.ToUpper().Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.Ampere.ToUpper()).Contains(searchhWords[pos].ToUpper()) ||
                                v.Note.ToUpper().Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.VareLokation.ToUpper()).Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.VareNr.ToUpper()).Contains(searchhWords[pos].ToUpper())
                                || Convert.ToString(v.Antal).Contains(searchhWords[pos]) || Convert.ToString(v.PinNr.ToUpper()).Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.WebshopVareNummer.ToUpper()).Contains(searchhWords[pos].ToUpper())
                                || Convert.ToString(v.Længde).Contains(searchhWords[pos]) || Convert.ToString(v.RFIDNummer.ToUpper()).Contains(searchhWords[pos].ToUpper())))
                        {

                            result = true;

                        }
                        else
                        {
                            end = false;

                        }
                    }
                    else if ((searchhWords.Length - 1) > pos && fav.IsChecked == true && val == " ")
                    {
                        if ((v.Favorit == 1) && (v.Beskrivelse.ToUpper() == (searchhWords[pos].ToUpper()) || v.Tilgang.ToUpper() == (searchhWords[pos].ToUpper()) || v.Afgang.ToUpper() == (searchhWords[pos].ToUpper()) || Convert.ToString(v.Ampere.ToUpper()) == (searchhWords[pos].ToUpper()) ||
                                v.Note.ToUpper() == (searchhWords[pos].ToUpper()) || Convert.ToString(v.VareLokation.ToUpper()) == (searchhWords[pos].ToUpper()) || Convert.ToString(v.VareNr.ToUpper()) == (searchhWords[pos].ToUpper())
                                || Convert.ToString(v.Antal) == (searchhWords[pos]) || Convert.ToString(v.PinNr.ToUpper()) == (searchhWords[pos].ToUpper()) || Convert.ToString(v.WebshopVareNummer.ToUpper()) == (searchhWords[pos].ToUpper())
                                || Convert.ToString(v.Længde) == (searchhWords[pos]) || Convert.ToString(v.RFIDNummer.ToUpper()) == (searchhWords[pos].ToUpper())))
                        {
                            pos += 1;

                        }
                        else
                        {
                            pos += 1;
                            end = false;

                        }
                    }
                    else if ((searchhWords.Length - 1) == pos && fav.IsChecked == true && val == " ")
                    {
                        if ((v.Favorit == 1) && (v.Beskrivelse.ToUpper().Contains(searchhWords[pos].ToUpper()) || v.Tilgang.ToUpper().Contains(searchhWords[pos].ToUpper()) || v.Afgang.ToUpper().Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.Ampere.ToUpper()).Contains(searchhWords[pos].ToUpper()) ||
                                v.Note.ToUpper().Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.VareLokation.ToUpper()).Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.VareNr.ToUpper()).Contains(searchhWords[pos].ToUpper())
                                || Convert.ToString(v.Antal).Contains(searchhWords[pos]) || Convert.ToString(v.PinNr.ToUpper()).Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.WebshopVareNummer.ToUpper()).Contains(searchhWords[pos].ToUpper())
                                || Convert.ToString(v.Længde).Contains(searchhWords[pos]) || Convert.ToString(v.RFIDNummer.ToUpper()).Contains(searchhWords[pos].ToUpper())))
                        {
                            pos += 1;
                            result = true;
                        }
                        else
                        {
                            pos += 1;
                            end = false;

                        }
                    }
                    // hvis favorit er tykket og status ikke er tom
                    if (searchhWords.Length == 1 && fav.IsChecked == true && val != " ")
                    {
                        if ((v.Status == val) && (v.Favorit == 1) && (v.Beskrivelse.ToUpper().Contains(searchhWords[pos].ToUpper()) || v.Tilgang.ToUpper().Contains(searchhWords[pos].ToUpper()) || v.Afgang.ToUpper().Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.Ampere.ToUpper()).Contains(searchhWords[pos].ToUpper()) ||
                                v.Note.ToUpper().Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.VareLokation.ToUpper()).Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.VareNr.ToUpper()).Contains(searchhWords[pos].ToUpper())
                                || Convert.ToString(v.Antal).Contains(searchhWords[pos]) || Convert.ToString(v.PinNr.ToUpper()).Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.WebshopVareNummer.ToUpper()).Contains(searchhWords[pos].ToUpper())
                                || Convert.ToString(v.Længde).Contains(searchhWords[pos]) || Convert.ToString(v.RFIDNummer.ToUpper()).Contains(searchhWords[pos].ToUpper())))
                        {

                            result = true;

                        }
                        else
                        {
                            end = false;

                        }
                    }
                    else if ((searchhWords.Length - 1) > pos && fav.IsChecked == true && val != " ")
                    {
                        if ((v.Status == val) && (v.Favorit == 1) && (v.Beskrivelse.ToUpper() == (searchhWords[pos].ToUpper()) || v.Tilgang.ToUpper() == (searchhWords[pos].ToUpper()) || v.Afgang.ToUpper() == (searchhWords[pos].ToUpper()) || Convert.ToString(v.Ampere.ToUpper()) == (searchhWords[pos].ToUpper()) ||
                                v.Note.ToUpper() == (searchhWords[pos].ToUpper()) || Convert.ToString(v.VareLokation.ToUpper()) == (searchhWords[pos].ToUpper()) || Convert.ToString(v.VareNr.ToUpper()) == (searchhWords[pos].ToUpper())
                                || Convert.ToString(v.Antal) == (searchhWords[pos]) || Convert.ToString(v.PinNr.ToUpper()) == (searchhWords[pos].ToUpper()) || Convert.ToString(v.WebshopVareNummer.ToUpper()) == (searchhWords[pos].ToUpper())
                                || Convert.ToString(v.Længde) == (searchhWords[pos]) || Convert.ToString(v.RFIDNummer.ToUpper()) == (searchhWords[pos].ToUpper())))
                        {
                            pos += 1;

                        }
                        else
                        {
                            pos += 1;
                            end = false;

                        }
                    }
                    else if ((searchhWords.Length - 1) == pos && fav.IsChecked == true && val != " ")
                    {
                        if ((v.Status == val) && (v.Favorit == 1) && (v.Beskrivelse.ToUpper().Contains(searchhWords[pos].ToUpper()) || v.Tilgang.ToUpper().Contains(searchhWords[pos].ToUpper()) || v.Afgang.ToUpper().Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.Ampere.ToUpper()).Contains(searchhWords[pos].ToUpper()) ||
                                v.Note.ToUpper().Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.VareLokation.ToUpper()).Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.VareNr.ToUpper()).Contains(searchhWords[pos].ToUpper())
                                || Convert.ToString(v.Antal).Contains(searchhWords[pos]) || Convert.ToString(v.PinNr.ToUpper()).Contains(searchhWords[pos].ToUpper()) || Convert.ToString(v.WebshopVareNummer.ToUpper()).Contains(searchhWords[pos].ToUpper())
                                || Convert.ToString(v.Længde).Contains(searchhWords[pos]) || Convert.ToString(v.RFIDNummer.ToUpper()).Contains(searchhWords[pos].ToUpper())))
                        {
                            pos += 1;
                            result = true;
                        }
                        else
                        {
                            pos += 1;
                            end = false;

                        }
                    }

                }

                // hvis vare overholder filtre regler så skal den ikke filtres fra
                if (result == true)
                {
                    return true;
                }
                // hvis var ikke overholder filtre regler så skal den filtres fra
                else
                {
                    pos = 0;
                    return false;
                }
            }
            else
            {
                return false;
            }



        }

        // Tjek om checkbox ikke er trykket og køre den rigtige filtre
        private void handleUnchecked(object sender, RoutedEventArgs e)
        {
            // hvis søgebar er tom skal den kun køre filtre til favorit checkbox og status dropdown
            if (string.IsNullOrEmpty(txtSearchBox.Text))
            {
                cvVare.Filter = new Predicate<object>(checkFilter);
            }
            // hvis søgebar ikke er tom skal den kun køre filtre til søgebar, favorit checkbox og status dropdown
            else
            {
                cvVare.Filter = new Predicate<object>(filter);
            }

        }

        // Tjek om checkbox er trykket og køre den rigtige filtre
        private void handleChecked(object sender, RoutedEventArgs e)
        {


            // hvis søgebar er tom skal den kun køre filtre til favorit checkbox og status dropdown
            if (string.IsNullOrEmpty(txtSearchBox.Text))
            {
                cvVare.Filter = new Predicate<object>(checkFilter);
            }
            // hvis søgebar ikke er tom skal den kun køre filtre til søgebar, favorit checkbox og status dropdown
            else
            {
                cvVare.Filter = new Predicate<object>(filter);
            }


        }

        // Tjek om status er valgt og køre den rigtige filtre
        private void status_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // hvis søgebar er tom skal den kun køre filtre til favorit checkbox og status dropdown
            if (string.IsNullOrEmpty(txtSearchBox.Text))
            {

                cvVare.Filter = new Predicate<object>(checkFilter);
            }
            // hvis søgebar ikke er tom skal den kun køre filtre til søgebar, favorit checkbox og status dropdown
            else
            {
                cvVare.Filter = new Predicate<object>(filter);
            }
        }

        // filtre til favorit checkbox og status dropdown
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

            // hvis favorit er trykket men ingen status er valgt
            if (fav.IsChecked == true && val != " ")
            {
                if (o is VareClass)
                {
                    VareClass v = (o as VareClass);


                    if (v.Favorit == 1 && v.Status == val)
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
            // hvis favorit ikke er trykket men ingen status er valgt
            else if (fav.IsChecked == false && val == " ")
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
            // hvis favorit er trykket og et status er valgt
            else if (fav.IsChecked == true && val == " ")
            {
                if (o is VareClass)
                {
                    VareClass v = (o as VareClass);


                    if (v.Favorit == 1)
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
            // hvis favorit ikke er trykket men et status er valgt
            else if (fav.IsChecked == false && val != " ")
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

            else
            {
                return false;
            }

        }

        public void insertLokation(string lokationID1, string lokationID2)
        {
            List<string> lokationEvents = new List<string>();

            // Tag alle entries fra lokationList hvor ID'et Matcher den første boks i textfeltet "lokationID1" og put dem i lokationEvents listen

            foreach (var item in lokationList)
            {
                if (item.EventId == lokationID1)
                {
                    lokationEvents.Add(item.Lokation.ToString());
                }

            }

            // Tilføj alle entries fra lokationEvents og put dem i lokations listen
            for (int i = 0; i < lokationEvents.Count; i++)
            {
                lokationList.Add(new lokationerClass(lokationID2, lokationEvents[i]));
            }
        }

        private void checkBoxKopir_Checked(object sender, RoutedEventArgs e)
        {

            checkBoxKopir_Checked_Bool = true;
            checkBoxKopir_Checked_String = "true";
        }

        private void checkBoxKopir_Unchecked(object sender, RoutedEventArgs e)
        {
            checkBoxKopir_Checked_Bool = false;
            checkBoxKopir_Checked_String = "false";
        }

    }
}