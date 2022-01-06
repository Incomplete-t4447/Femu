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
    public partial class EventPage : Page
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
        string valgteEvent = "";
        string eventstr = " ";
        string EventId = "";
        string redigerId;
        string lokationId;
        string lokationStr;
        string NewLokation;
        string currentEventName;
        string checkBoxKopir_Checked_String;
        int currentEventId;
        bool toogleOmbytte = false;
        bool ombytte = false;
        bool checkBoxKopir_Checked_Bool;
        VareClass EventRow;
        string lastEventID = "";

        public EventPage()
        {
            InitializeComponent();

            EventGrid.ItemsSource = myList;
        }


        // hent events til datagrid
        void Onload(object sender, RoutedEventArgs e)
        {
            _ = AllEventsListQueryAsync();
        }

        // tjek om textboxe er tomme
        public bool Check_Event()
        {

            bool noDouble = true;

            int pos = 0;

            while(myList.Count > pos && noDouble == true)
            {
                if(myList[pos].EventName.ToLower() == EventName.Text.ToLower())
                {
                    MessageBox.Show("Du kan ikke have 2 events der hedder det samme");
                    noDouble = false;
                }
                else
                {
                    pos += 1;
                }
            }

            if ( noDouble == false ||
               string.IsNullOrEmpty(EventName.Text) ||
               string.IsNullOrEmpty(DatePickerStart.ToString()) ||
               string.IsNullOrEmpty(DatePickerSlut.ToString()))
            {
                if(string.IsNullOrEmpty(EventName.Text) ||
                string.IsNullOrEmpty(DatePickerStart.ToString()) ||
                string.IsNullOrEmpty(DatePickerSlut.ToString()))
                {
                    MessageBox.Show("Være sikker på at du har udfyldt felterne" +
                                        "(EventName, " +
                                        "StartDato, " +
                                        "SlutDato "
                                        );
                }
                

                return false;
            }
            else
            {
                return true;
            }
        }

        // api kald til at lave et nyt event
        private async Task<JArray> CreateEvent(string name, string startDato, string slutDato, string note, string dieselStart, string dieselSlut, string defektPærer, string tlfNr, string aktiv, string brugerId)
        {
            JArray stuff = await EventApi.CreateEventAsync(name, startDato, slutDato, note, dieselStart, dieselSlut, defektPærer, tlfNr, aktiv, brugerId);
            return stuff;
        }

        // tilføje event
        public async void Tilføjning_af_Event()
        {
            if (Check_Event())
            {

                DateTime startDato;
                DateTime slutDato;
                if (DieselstanderStartBox.Text == "")
                {
                    DieselstanderStartBox.Text = "0";
                }
                if (DielselstanderSlutBox.Text == "")
                {
                    DielselstanderSlutBox.Text = "0";
                }
                if (DefektePærerBox.Text == "")
                    DefektePærerBox.Text = "0";
                //if (TlfNummerBox.Text == "")
                //    TlfNummerBox.Text = " ";

                // gem data til nye event
                if (DateTime.TryParseExact(DatePickerStart.ToString(), "dd-MM-yyyy HH:mm:ss", new CultureInfo("da-DK"), DateTimeStyles.None, out startDato) && DateTime.TryParseExact(DatePickerSlut.ToString(), "dd-MM-yyyy HH:mm:ss", new CultureInfo("da-DK"), DateTimeStyles.None, out slutDato))
                {
                    string theEventName = EventName.Text;
                    var new_row =
                    new EventClass(
                        "",
                        EventName.Text,
                        startDato.ToString("yyyy-MM-dd"),
                        slutDato.ToString("yyyy-MM-dd"),
                        NoteBox.Text,
                        DieselstanderStartBox.Text,
                        DielselstanderSlutBox.Text,
                        DefektePærerBox.Text,
                        TlfNummerBox.Text,
                        AktivComboBox.Text,
                        brugerIdComboBox.Text
                    );

                    // lav nyt event med den nye data
                    myList.Add(new_row);
                    _ = CreateEvent(
                            EventName.Text,
                            startDato.ToString("yyyy-MM-dd"),
                            slutDato.ToString("yyyy-MM-dd"),
                            NoteBox.Text,
                            DieselstanderStartBox.Text,
                            DielselstanderSlutBox.Text,
                            DefektePærerBox.Text,
                            TlfNummerBox.Text,
                            AktivComboBox.Text,
                            brugerIdComboBox.Text
                        );

                    // nulstil textboxe
                    EventName.Text = "";
                    DatePickerStart.SelectedDate = null;
                    DatePickerSlut.SelectedDate = null;

                    
                    //DatePickerSlut.SelectedDate = DateTime.ParseExact(row.SlutDato, "dd-MM-yyyy", null);

                    //StartDatoBox.Text = "";
                    //SlutDatoBox.Text = "";
                    NoteBox.Text = "";
                    DieselstanderStartBox.Text = "";
                    DielselstanderSlutBox.Text = "";
                    DefektePærerBox.Text = "";
                    TlfNummerBox.Text = "";

                    await AllEventsListQueryAsync();

                    string eventIDD = "";
                    for(int i = 0; i < myList.Count; i++)
                    {
                        if (myList[i].EventName == theEventName)
                        {
                            eventIDD = myList[i].EventId;
                        }
                    }

                    _ = createEventLokation(Convert.ToInt32(eventIDD), "Midlertid ");
                    lastEventID = "";

                    // succes besked
                    MessageBox.Show("Eventet er oprettet");
                }
                // fejl besked
                else
                {
                    MessageBox.Show("Datoen skal skrives som følgene: år-måned-dag timer:minutter:sekunder");
                }

            }
            
        }

        private void DatePickerStart_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePickerSlut.SelectedDate = DatePickerStart.SelectedDate;
            
        }

        // Gå tilbage til hovedmenuen
        private void goBack_Click(object sender, RoutedEventArgs e)
        {
            EventName.Text = "";
            //StartDatoBox.Text = "";
            //SlutDatoBox.Text = "";
            DatePickerStart.SelectedDate = null;
            DatePickerSlut.SelectedDate = null;
            NoteBox.Text = "";
            DieselstanderStartBox.Text = "";
            DielselstanderSlutBox.Text = "";
            DefektePærerBox.Text = "";
            TlfNummerBox.Text = "";

            SpecificEventButtons1.Visibility = Visibility.Collapsed;
            SpecificEventButtons2.Visibility = Visibility.Collapsed;

            KopierEventPanel.Visibility = Visibility.Collapsed;
            OmbytVarePanel.Visibility = Visibility.Collapsed;

            EventsStackPanelView.Visibility = Visibility.Collapsed;
            Events2.Visibility = Visibility.Collapsed;

            EventVareBorder.Visibility = Visibility.Collapsed;
            EventVareGrid.Visibility = Visibility.Collapsed;

            StatusPanel.Visibility = Visibility.Collapsed;

            statusLabel.Visibility = Visibility.Collapsed;
            status.Visibility = Visibility.Collapsed;

            EventButtons.Visibility = Visibility.Visible;
            addEvent.Visibility = Visibility.Visible;

            Events.Visibility = Visibility.Visible;
            EventsBoxes.Visibility = Visibility.Visible;

            EventGrid.Visibility = Visibility.Visible;
            EventGridBorder.Visibility = Visibility.Visible;

            statusLabel.Visibility = Visibility.Collapsed;
            status.Visibility = Visibility.Collapsed;
            VareNrLabel.Visibility = Visibility.Collapsed;
            VareNrBox.Visibility = Visibility.Collapsed;
            beskrivelseLabel.Visibility = Visibility.Collapsed;
            beskrivelseBox.Visibility = Visibility.Collapsed;
            TilgangLabel.Visibility = Visibility.Collapsed;
            TilgangBox.Visibility = Visibility.Collapsed;
            AfgangLabel.Visibility = Visibility.Collapsed;
            AfgangBox.Visibility = Visibility.Collapsed;
            AntalLabel.Visibility = Visibility.Collapsed;
            AntalBox.Visibility = Visibility.Collapsed;
            LokationLabel.Visibility = Visibility.Collapsed;
            LokationBox.Visibility = Visibility.Collapsed;
            AmpereLabel.Visibility = Visibility.Collapsed;
            AmpereBox.Visibility = Visibility.Collapsed;
            PinLabel.Visibility = Visibility.Collapsed;
            PinBox.Visibility = Visibility.Collapsed;
            LængdeLabel.Visibility = Visibility.Collapsed;
            LængdeBox.Visibility = Visibility.Collapsed;
            webLabel.Visibility = Visibility.Collapsed;
            webBox.Visibility = Visibility.Collapsed;
            NoteLabel.Visibility = Visibility.Collapsed;
            NoteVareBox.Visibility = Visibility.Collapsed;



            AktivLabel.Visibility = Visibility.Visible;
            AktivCB.Visibility = Visibility.Visible;
            searchName.Visibility = Visibility.Visible;
            EventNameBox.Visibility = Visibility.Visible;
            searchStart.Visibility = Visibility.Visible;
            StartBox.Visibility = Visibility.Visible;
            searchSlut.Visibility = Visibility.Visible;
            SlutBox.Visibility = Visibility.Visible;
            searchNote.Visibility = Visibility.Visible;
            NoteSeachBox.Visibility = Visibility.Visible;
            searchDsStart.Visibility = Visibility.Visible;
            DsStartBox.Visibility = Visibility.Visible;
            searchDsSlut.Visibility = Visibility.Visible;
            DsSlutBox.Visibility = Visibility.Visible;
            searchDP.Visibility = Visibility.Visible;
            DPBox.Visibility = Visibility.Visible;
            searchTlf.Visibility = Visibility.Visible;
            TlfBox.Visibility = Visibility.Visible;
            brugerIdLabel.Visibility = Visibility.Visible;
            brugerCB.Visibility = Visibility.Visible;


            ToggleEvent = false;
            EventGrid.ItemsSource = myList;
            EventGrid.Items.Refresh();

            lokationPanel.Children.Clear();
            lokationPanel.Visibility = Visibility.Collapsed;
            lokationScroll.Visibility = Visibility.Collapsed;
            EventsBoxes.Visibility = Visibility.Visible;


            redigerLokationStack.Visibility = Visibility.Collapsed;
            addLokationStack.Visibility = Visibility.Collapsed;
            EventsBoxes.Visibility = Visibility.Visible;

            addEvent.Visibility = Visibility.Visible;
            RedigerGem.Visibility = Visibility.Collapsed;
            RedigerEventAnnuller.Visibility = Visibility.Collapsed;
            NavigationService.Navigate(Pages.p5);
        }

        // fejl besked hvis prøver at bruger bogstaver når manskriver tfl nr.
        private void NumberValidation(object sender, TextCompositionEventArgs e )
        {
            Regex re = new Regex("[^0-9.,]+");

            if (e.Handled = re.IsMatch(e.Text))
            {
                MessageBox.Show("Slet ikke tilladt");
            }


        }

        // fejl besked til textboxe hvis man prøver at kopier,klippe eller sætte ind
        private void textBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy ||
                e.Command == ApplicationCommands.Cut ||
                e.Command == ApplicationCommands.Paste)
            {
                MessageBox.Show("Slet ikke tilladt");
                e.Handled = true;
            }

        }

        // start funktion til at tilføje event
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
                string EventBrugerId = (string)item.GetValue("EventBrugerId");

                DateTime startDato;
                DateTime slutDato;

                DateTime.TryParseExact(eventStartDato, "yyyy-MM-dd", new CultureInfo("da-DK"), DateTimeStyles.None, out startDato);
                DateTime.TryParseExact(eventSlutDato, "yyyy-MM-dd", new CultureInfo("da-DK"), DateTimeStyles.None, out slutDato);

                // tilføj events til lisre
                myList.Add(new EventClass(evnentId, eventName, startDato.ToString("dd-MM-yyyy"), slutDato.ToString("dd-MM-yyyy"), eventNote, eventDieselstanderStart, eventDielselstanderSlut, eventDefektePærer, eventTlfNummer, eventAktiv, EventBrugerId));
            }



            // opfrisk liste
            EventGrid.Items.Refresh();



            return myList;
        }

        // api kald til at rediger events
        private async Task<JArray> EditEventAsync(string navn, string newnavn, string startdato, string slutdato, string note, string dieselstart, string dieselslut, string defektepære, string tlfnr, string aktiv, string brugerId)
        {
            JArray editEvent = await EventApi.EditEvent(navn, newnavn, startdato, slutdato, note, dieselstart, dieselslut, defektepære, tlfnr, aktiv, brugerId);
            return editEvent;
        }

        // Gem ændring på event
        private void RedigerGem_button(object sender, RoutedEventArgs e)
        {
            List<EventClass> eventListe = EventGrid.ItemsSource.Cast<EventClass>().ToList();
            var row = (EventClass)EventGrid.SelectedItem;

            string startDate = null;
            string slutDate = null;

            DateTime startDate2;
            DateTime slutDate2;

            bool done = false;
            int pos = 0;

            // gem ændringer
            while (done == false)
            {
                if (
                    eventListe[pos].EventId == redigerId)
                {
                    // gem dato og tid
                    if (DatePickerStart != null && DatePickerSlut.SelectedDate != null)
                    {
                        if (DateTime.TryParseExact(DatePickerStart.SelectedDate.ToString(), "dd-MM-yyyy HH:mm:ss", new CultureInfo("da-DK"), DateTimeStyles.None, out startDate2) && DateTime.TryParseExact(DatePickerSlut.SelectedDate.ToString(), "dd-MM-yyyy HH:mm:ss", new CultureInfo("da-DK"), DateTimeStyles.None, out slutDate2))
                        {
                            startDate = startDate2.ToString("yyyy-MM-dd");
                            slutDate = slutDate2.ToString("yyyy-MM-dd");
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
                    // updater event med ny data




                    

                    

                    _ = EditEventAsync(eventListe[pos].EventName,
                                       EventName.Text,
                                       startDate,
                                       slutDate,
                                       NoteBox.Text,
                                       DieselstanderStartBox.Text,
                                       DielselstanderSlutBox.Text,
                                       DefektePærerBox.Text,
                                       TlfNummerBox.Text,
                                       AktivComboBox.Text,
                                       brugerIdComboBox.Text);

                    // nulstil

                    

                    _ = AllEventsListQueryAsync();
                    EventGrid.Items.Refresh();

                    if (EventGrid.ItemsSource != myList)
                    {
                        row.EventName = EventName.Text;
                        row.StartDato = DatePickerStart.SelectedDate.ToString();
                        row.SlutDato = DatePickerSlut.SelectedDate.ToString();
                        row.Note = NoteBox.Text;
                        row.DieselstanderStart = DieselstanderStartBox.Text;
                        row.DielselstanderSlut = DielselstanderSlutBox.Text;
                        row.DefektePærer = DefektePærerBox.Text;
                        row.TlfNummer = TlfNummerBox.Text;
                        row.Aktiv = AktivComboBox.Text;
                        row.BrugerId = brugerIdComboBox.Text;
                    }



                    EventName.Text = "";
                    //StartDatoBox.Text = "";
                    //SlutDatoBox.Text = "";
                    DatePickerStart.SelectedDate = null;
                    DatePickerSlut.SelectedDate = null;
                    NoteBox.Text = "";
                    DieselstanderStartBox.Text = "";
                    DielselstanderSlutBox.Text = "";
                    DefektePærerBox.Text = "";
                    TlfNummerBox.Text = "";

                    RedigerGem.Visibility = Visibility.Collapsed;
                    RedigerEventAnnuller.Visibility = Visibility.Collapsed;
                    addEvent.Visibility = Visibility.Visible;
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

        public void lukRedigeringPannel()
        {
            EventName.Text = "";
            //StartDatoBox.Text = "";
            //SlutDatoBox.Text = "";
            DatePickerStart.SelectedDate = null;
            DatePickerSlut.SelectedDate = null;
            NoteBox.Text = "";
            DieselstanderStartBox.Text = "";
            DielselstanderSlutBox.Text = "";
            DefektePærerBox.Text = "";
            TlfNummerBox.Text = "";

            addEvent.Visibility = Visibility.Visible;
            RedigerGem.Visibility = Visibility.Collapsed;
            RedigerEventAnnuller.Visibility = Visibility.Collapsed;
        }

        private void AnnullerEventRedigering(object sender, RoutedEventArgs e)
        {
            lukRedigeringPannel();
        }

        // liste af lokationer på et event
        private async void Lokation_button(object sender, RoutedEventArgs e)
        {
            // gem event så
            var row = (EventClass)EventGrid.SelectedItem;

            // luk og åben de relatert paneller
            if (row != null)
            {
                lokationPanel.Children.Clear();
                lokationPanel.Visibility = Visibility.Visible;
                addLokationStack.Visibility = Visibility.Collapsed;
                lokationList.Clear();


                // hent alle lokationer
                JArray eventLocations = await EventApi.GetEventLokation(Int32.Parse(row.EventId));
                if (eventLocations != null)
                {

                    lukRedigeringPannel();
                    foreach (JObject item in eventLocations)
                    {
                        string eventlokationId = (string)item.GetValue("EventLokationId");
                        int eventId = (int)item.GetValue("EventLokationEventId");
                        string eventLokation = (string)item.GetValue("Lokation");
                        lokationList.Add(new lokationerClass(eventId.ToString(), eventLokation, eventlokationId));
                    }

                    bool emptyOrNot = false;
                    int count = 0;
                    bool done = false;

                    // gem lokationer der høre til det valgte event
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

                    // lav en liste af lokation
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

                                myButton.Name = $"LokationButton{lokationList[i].LokationId}";
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

                                // hvad der skal hvis man trykker på lokation knap
                                myButton.Click += (s, se) =>
                                {
                                    lokationScroll.Visibility = Visibility.Collapsed;
                                    redigerLokationStack.Visibility = Visibility.Visible;
                                    lokationStr = myButton.Content.ToString();
                                    lokationRedigerBox.Text = myButton.Content.ToString();

                                    lokationId = myButton.Name.Replace("LokationButton", "");
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
                            lokationPanel.Visibility = Visibility.Collapsed;
                            lokationScroll.Visibility = Visibility.Collapsed;
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

        // api kald til redigering af lokationer til events
        private async Task<JArray> editEventLocation(string id, string eventid, string lokation)
        {
            JArray editLocation = await EventApi.editEventLocation(id, eventid, lokation);
            return editLocation;
        }

        // gem redigering af lokation
        private void RedigereLokationButton_Click(object sender, RoutedEventArgs e)
        {
            int pos = 0;
            bool done = false;
            NewLokation = lokationRedigerBox.Text;

            // tjek om du har lavet en ændring
            if (NewLokation != null && lokationStr != NewLokation)
            {
                while (done == false && pos < lokationList.Count())
                {
                    // rediger lokation
                    if (lokationList[pos].EventId == redigerId && lokationList[pos].Lokation == lokationStr && lokationList[pos].LokationId == lokationId)
                    {
                        _ = editEventLocation(lokationId,redigerId, NewLokation);
                        lokationList[pos] = new lokationerClass(redigerId, NewLokation , lokationId);
                        redigerLokationStack.Visibility = Visibility.Collapsed;
                        EventsBoxes.Visibility = Visibility.Visible;
                        EventGrid.Items.Refresh();
                        // succes besked
                        MessageBox.Show("lokation er blevet redigeret");
                        done = true;
                    }
                    else
                    {
                        pos += 1;
                    }
                }
            }
            // fejl besked
            else
            {
                MessageBox.Show("Feltet er inten tom eller også er der ikke blevet gjort nogen ændring");
            }

        }

        // Annuller knap
        private void Annuller_Click(object sender, RoutedEventArgs e)
        {

            // luk ting ned når man tykker på knap
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

        private async void getEventVareList()
        {
            try
            {

                eventVareList.Clear();
                //Henter alle varer der er på et event
                JArray loadResult = await EventApi.GetItemsQueryAsync(currentEventId); //JArray som indeholder alt der passer på idInt i databasen


                if (loadResult != null)
                {

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
                        string itemSalgspris = (string)item.GetValue("salgspris");
                        string itemEventVareId = (string)item.GetValue("eventVareId");
                        string itemEventVareLokation = (string)item.GetValue("eventVareLokation");


                        eventVareList.Add(new VareClass(itemVarenummer, itemBeskrivelse, itemTilgang, itemAfgang, itemAmpere, itemStatus, itemAntal, itemEventVareLokation, itemPinNr, itemLength, itemSalgspris, itemNote, itemWebshopNummer, itemRfidNummer, itemQrKode, itemEventVareId));

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
                    SpecificEventButtons1.Visibility = Visibility.Visible;
                    SpecificEventButtons2.Visibility = Visibility.Visible;

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
                        string itemSalgspris = (string)item.GetValue("salgspris");
                        string itemEventVareId = (string)item.GetValue("eventVareId");
                        string itemEventVareLokation = (string)item.GetValue("eventVareLokation");


                        eventVareList.Add(new VareClass(itemVarenummer, itemBeskrivelse, itemTilgang, itemAfgang, itemAmpere, itemStatus, itemAntal, itemEventVareLokation, itemPinNr, itemLength, itemNote, itemSalgspris, itemWebshopNummer, itemRfidNummer, itemQrKode, itemEventVareId));

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
            JArray eventResult = await EventApi.AllEventsListQueryAsync();
            if (eventResult != null)
            {
                EventsStackPanel.Children.Clear();
                selectedEventLabel.Content = "Event: ";
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

                        EventId = eventId;
                        valgteEvent = myButton.Content.ToString();
                        eventVareList.Clear();
                        EventVareGrid.Items.Refresh();
                        eventstr = myButton.Name;
                        Match idString = re.Match(eventstr);
                        selectedEventLabel.Content = "Event: " + eventName;
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

        // lave knapper til all events så man kan behandle dem når man trykker på et event
        private void Start_Events(object sender, RoutedEventArgs e)
        {
            EventName.Text = "";
            //StartDatoBox.Text = "";
            //SlutDatoBox.Text = "";
            DatePickerStart.SelectedDate = null;
            DatePickerSlut.SelectedDate = null;
            NoteBox.Text = "";
            DieselstanderStartBox.Text = "";
            DielselstanderSlutBox.Text = "";
            DefektePærerBox.Text = "";
            TlfNummerBox.Text = "";
            eventVareList.Clear();
            EventVareGrid.Items.Refresh();

            // tjek om knap er trykket og vis eller gem knapper efter behov
            if (ToggleEvent == false)
            {
                try
                {
                    EventButtons.Visibility = Visibility.Collapsed;
                    RedigerGem.Visibility = Visibility.Collapsed;
                    RedigerEventAnnuller.Visibility = Visibility.Collapsed;
                    addEvent.Visibility = Visibility.Collapsed;
                    lokationPanel.Children.Clear();
                    addLokationStack.Visibility = Visibility.Collapsed;


                    AktivLabel.Visibility = Visibility.Collapsed;
                    AktivCB.Visibility = Visibility.Collapsed;
                    searchName.Visibility = Visibility.Collapsed;
                    EventNameBox.Visibility = Visibility.Collapsed;
                    searchStart.Visibility = Visibility.Collapsed;
                    StartBox.Visibility = Visibility.Collapsed;
                    searchSlut.Visibility = Visibility.Collapsed;
                    SlutBox.Visibility = Visibility.Collapsed;
                    searchNote.Visibility = Visibility.Collapsed;
                    NoteSeachBox.Visibility = Visibility.Collapsed;
                    searchDsStart.Visibility = Visibility.Collapsed;
                    DsStartBox.Visibility = Visibility.Collapsed;
                    searchDsSlut.Visibility = Visibility.Collapsed;
                    DsSlutBox.Visibility = Visibility.Collapsed;
                    searchDP.Visibility = Visibility.Collapsed;
                    DPBox.Visibility = Visibility.Collapsed;
                    searchTlf.Visibility = Visibility.Collapsed;
                    TlfBox.Visibility = Visibility.Collapsed;
                    brugerIdLabel.Visibility = Visibility.Collapsed;
                    brugerCB.Visibility = Visibility.Collapsed;

                    aktivCheck.Visibility = Visibility.Visible;
                    eventAktivLabel.Visibility = Visibility.Visible;
                    statusLabel.Visibility = Visibility.Visible;
                    status.Visibility = Visibility.Visible;
                    VareNrLabel.Visibility = Visibility.Visible;
                    VareNrBox.Visibility = Visibility.Visible;
                    beskrivelseLabel.Visibility = Visibility.Visible;
                    beskrivelseBox.Visibility = Visibility.Visible;
                    TilgangLabel.Visibility = Visibility.Visible;
                    TilgangBox.Visibility = Visibility.Visible;
                    AfgangLabel.Visibility = Visibility.Visible;
                    AfgangBox.Visibility = Visibility.Visible;
                    AntalLabel.Visibility = Visibility.Visible;
                    AntalBox.Visibility = Visibility.Visible;
                    LokationLabel.Visibility = Visibility.Visible;
                    LokationBox.Visibility = Visibility.Visible;
                    AmpereLabel.Visibility = Visibility.Visible;
                    AmpereBox.Visibility = Visibility.Visible;
                    PinLabel.Visibility = Visibility.Visible;
                    PinBox.Visibility = Visibility.Visible;
                    LængdeLabel.Visibility = Visibility.Visible;
                    LængdeBox.Visibility = Visibility.Visible;
                    webLabel.Visibility = Visibility.Visible;
                    webBox.Visibility = Visibility.Visible;
                    NoteLabel.Visibility = Visibility.Visible;
                    NoteVareBox.Visibility = Visibility.Visible;
                    TilbageKnapEvent.Visibility = Visibility.Collapsed;
                    selectedEventLabel.Visibility = Visibility.Visible;
                    SearchEventNameEvent.Visibility = Visibility.Visible;
                    SearchEventNameEventLabel.Visibility = Visibility.Visible;



                    Events2.Visibility = Visibility.Visible;
                    EventsStackPanelView.Visibility = Visibility.Visible;
                    Events.Visibility = Visibility.Collapsed;

                    EventVareBorder.Visibility = Visibility.Visible;
                    EventVareGrid.Visibility = Visibility.Visible;

                    EventGrid.Visibility = Visibility.Collapsed;
                    EventGridBorder.Visibility = Visibility.Collapsed;


                    _ = PullEventsToButtons();

                }
                //fejl besked
                catch
                {
                    MessageBox.Show("Der er gået noget galt med forbindelsen til internettet eller databasen.");
                }


            }
            // tjek om knap er trykket og vis eller gem knapper efter behov
            else if (ToggleEvent == true)
            {
                SpecificEventButtons1.Visibility = Visibility.Collapsed;
                SpecificEventButtons2.Visibility = Visibility.Collapsed;

                KopierEventPanel.Visibility = Visibility.Collapsed;
                OmbytVarePanel.Visibility = Visibility.Collapsed;

                EventsStackPanelView.Visibility = Visibility.Collapsed;
                Events2.Visibility = Visibility.Collapsed;

                EventVareBorder.Visibility = Visibility.Collapsed;
                EventVareGrid.Visibility = Visibility.Collapsed;

                StatusPanel.Visibility = Visibility.Collapsed;

                statusLabel.Visibility = Visibility.Collapsed;
                status.Visibility = Visibility.Collapsed;

                EventButtons.Visibility = Visibility.Visible;
                addEvent.Visibility = Visibility.Visible;

                Events.Visibility = Visibility.Visible;
                EventsBoxes.Visibility = Visibility.Visible;

                EventGrid.Visibility = Visibility.Visible;
                EventGridBorder.Visibility = Visibility.Visible;

                aktivCheck.Visibility = Visibility.Collapsed;
                eventAktivLabel.Visibility = Visibility.Collapsed;
                statusLabel.Visibility = Visibility.Collapsed;
                status.Visibility = Visibility.Collapsed;
                VareNrLabel.Visibility = Visibility.Collapsed;
                VareNrBox.Visibility = Visibility.Collapsed;
                beskrivelseLabel.Visibility = Visibility.Collapsed;
                beskrivelseBox.Visibility = Visibility.Collapsed;
                TilgangLabel.Visibility = Visibility.Collapsed;
                TilgangBox.Visibility = Visibility.Collapsed;
                AfgangLabel.Visibility = Visibility.Collapsed;
                AfgangBox.Visibility = Visibility.Collapsed;
                AntalLabel.Visibility = Visibility.Collapsed;
                AntalBox.Visibility = Visibility.Collapsed;
                LokationLabel.Visibility = Visibility.Collapsed;
                LokationBox.Visibility = Visibility.Collapsed;
                AmpereLabel.Visibility = Visibility.Collapsed;
                AmpereBox.Visibility = Visibility.Collapsed;
                PinLabel.Visibility = Visibility.Collapsed;
                PinBox.Visibility = Visibility.Collapsed;
                LængdeLabel.Visibility = Visibility.Collapsed;
                LængdeBox.Visibility = Visibility.Collapsed;
                webLabel.Visibility = Visibility.Collapsed;
                webBox.Visibility = Visibility.Collapsed;
                NoteLabel.Visibility = Visibility.Collapsed;
                NoteVareBox.Visibility = Visibility.Collapsed;



                AktivLabel.Visibility = Visibility.Visible;
                AktivCB.Visibility = Visibility.Visible;
                searchName.Visibility = Visibility.Visible;
                EventNameBox.Visibility = Visibility.Visible;
                searchStart.Visibility = Visibility.Visible;
                StartBox.Visibility = Visibility.Visible;
                searchSlut.Visibility = Visibility.Visible;
                SlutBox.Visibility = Visibility.Visible;
                searchNote.Visibility = Visibility.Visible;
                NoteSeachBox.Visibility = Visibility.Visible;
                searchDsStart.Visibility = Visibility.Visible;
                DsStartBox.Visibility = Visibility.Visible;
                searchDsSlut.Visibility = Visibility.Visible;
                DsSlutBox.Visibility = Visibility.Visible;
                searchDP.Visibility = Visibility.Visible;
                DPBox.Visibility = Visibility.Visible;
                searchTlf.Visibility = Visibility.Visible;
                TlfBox.Visibility = Visibility.Visible;
                brugerIdLabel.Visibility = Visibility.Visible;
                brugerCB.Visibility = Visibility.Visible;
                TilbageKnapEvent.Visibility = Visibility.Visible;
                selectedEventLabel.Visibility = Visibility.Collapsed;
                selectedEventLabel.Content = "Event: ";
                SearchEventNameEvent.Visibility = Visibility.Collapsed;
                SearchEventNameEventLabel.Visibility = Visibility.Collapsed;



                ToggleEvent = false;
                EventGrid.ItemsSource = myList;
                EventGrid.Items.Refresh();

            }

        }
        // kopier event når man double klikker
        private void EventGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // gem den row der er klikket
            var row = (EventClass)EventGrid.SelectedItem;
            EventsBoxes.Visibility = Visibility.Visible;
            lokationPanel.Children.Clear();
            addLokationStack.Visibility = Visibility.Collapsed;

            // udfyld dato felter
            if (row != null)
            {
                if (row.StartDato != null && row.StartDato != "" && row.SlutDato != null && row.SlutDato != "")
                {
                    DateTime startDato;
                    DateTime slutDato;
                    if (DateTime.TryParseExact(row.StartDato, "dd-MM-yyyy", null, DateTimeStyles.None, out startDato) && DateTime.TryParseExact(row.SlutDato, "dd-MM-yyyy", null, DateTimeStyles.None, out slutDato))
                    {
                        //StartDatoBox.Text = startDato.ToString("dd-MM-yyyy HH:mm:ss");
                        //SlutDatoBox.Text = slutDato.ToString("dd-MM-yyyy HH:mm:ss");

                        DatePickerStart.SelectedDate = DateTime.Parse(startDato.ToString("dd-MM-yyyy HH:mm:ss"));
                        DatePickerSlut.SelectedDate = DateTime.Parse(slutDato.ToString("dd-MM-yyyy HH:mm:ss"));
                    }
                    else
                    {
                        if (row.StartDato == "Ingen værdi" && row.SlutDato == "Ingen værdi")
                        {
                            DatePickerStart.SelectedDate = null;
                            DatePickerSlut.SelectedDate = null;
                            //StartDatoBox.Text = "";
                            //SlutDatoBox.Text = "";
                        }
                        else
                        {
                            DatePickerStart.SelectedDate = DateTime.ParseExact(row.StartDato, "dd-MM-yyyy", null);
                            DatePickerSlut.SelectedDate = DateTime.ParseExact(row.SlutDato, "dd-MM-yyyy", null);
                            //StartDatoBox.Text =
                            //SlutDatoBox.Text = DateTime.ParseExact(row.SlutDato, "dd-MM-yyyy", null).ToString("dd-MM-yyyy HH:mm:ss");
                        }

                    }
                }
                else
                {
                    DatePickerStart.SelectedDate = DateTime.ParseExact(row.StartDato, "dd-MM-yyyy", null);
                    DatePickerSlut.SelectedDate = DateTime.ParseExact(row.SlutDato, "dd-MM-yyyy", null);

                    //StartDatoBox.Text = "";
                    //SlutDatoBox.Text = "";
                }
                // udfyld felter med den gemte data og gøre knap syndlig
                redigerId = row.EventId;
                EventName.Text = row.EventName;
                EventName.Text = row.EventName;
                NoteBox.Text = row.Note;
                DieselstanderStartBox.Text = row.DieselstanderStart;
                DielselstanderSlutBox.Text = row.DielselstanderSlut;
                DefektePærerBox.Text = row.DefektePærer;
                TlfNummerBox.Text = row.TlfNummer;

                AktivComboBox.Text = row.Aktiv;
                brugerIdComboBox.Text = row.BrugerId;

                RedigerGem.Visibility = Visibility.Visible;
                RedigerEventAnnuller.Visibility = Visibility.Visible;
                addEvent.Visibility = Visibility.Collapsed;
            }
        }

        private void ExportPdf_Click(object sender, RoutedEventArgs e)
        {
            // sorter vare listen efter WebshopVareNummer
            List<VareClass> vareListe = cvVare.Cast<VareClass>().ToList();
            List<VareClass> order = vareListe.OrderBy(o => o.WebshopVareNummer).ToList();

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




                        int cPos = 0;
                        int rPos = 0;

                        // tilføj indehold til header til tabel
                        while (cPos < EventVareGrid.Columns.Count)
                        {
                            PdfPCell cell = new PdfPCell(new Phrase(EventVareGrid.Columns[cPos].Header.ToString(), Headerfont));
                            headerTable.AddCell(cell);
                            cPos += 1;

                        }

                        // tilføj indehold til tekst til tabel celler
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

                            // tilføj celler til tabel
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
            JArray loadResultTest = await EventApi.EventAndVareQueryAsync(currentEventId); //JArray som indeholder alt der passer på idInt i databasen
            int a = 0;

            string manglendeNummer = " mangler et webshop nummer: \n";

            foreach (JObject itemtest in loadResultTest)
            {
                string exportVarenummerTest = (string)itemtest.GetValue("webshopNummer");
                string vareNummerTest = (string)itemtest.GetValue("varenummer");

                if (exportVarenummerTest == "") // Check column
                {
                    a++;
                    if(a > 1)
                    {
                        manglendeNummer = manglendeNummer + ", " + vareNummerTest;
                    }
                    else
                    {
                        manglendeNummer = manglendeNummer + vareNummerTest;
                    }
                }

            }

            if (a > 0)
            {
                if(a > 1)
                {
                    MessageBox.Show("Disse varer" + manglendeNummer);
                }
                else
                {
                    MessageBox.Show("Denne vare" + manglendeNummer);
                }
            }
            else if(a == 0)
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

            manglendeNummer = "Disse varer mangler et webshop nummer: ";
            exportList.Clear();
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
                eventVareList.Add(new VareClass(item.VareNr, item.Beskrivelse, item.Tilgang, item.Afgang, item.Ampere, item.Status, item.Antal, item.VareLokation, item.PinNr, item.Længde, item.Salgspris, item.Note, item.WebshopVareNummer, item.RFIDNummer, item.QR));
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

                _ = AllEventsListQueryAsync();
                EventGrid.Items.Refresh();

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
            // gem den valgte vare ned
            var row = (VareClass)EventVareGrid.SelectedItem;
            // index variabel
            int pos = 0;
            // variabel til at stoppe loop
            bool done = false;
            // gem combobox værdi ned
            string newStatus = ChangeStatusBox.Text;

            if (row != null && newStatus != "" && row.Status != newStatus)
            {
                //Looper igennem datagrid for at finde positionen på det item du har valgt
                while (done == false && pos < eventVareList.Count())
                {
                    //Hvis position data stemmer overens med det valgte datagrid data -> gå amok
                    if (eventVareList[pos].VareNr == row.VareNr && eventVareList[pos].Status == row.Status)
                    {
                        // api kald til at ændre status på event vare
                        _ = EditEventVareStatusAsync(eventVareList[pos].EventVareId, newStatus);
                        eventVareList[pos] = new VareClass(row.VareNr, row.Beskrivelse, row.Tilgang, row.Afgang, row.Ampere, newStatus, row.Antal, row.VareLokation, row.PinNr, row.Længde, row.Salgspris, row.Note, row.WebshopVareNummer, row.RFIDNummer, row.QR);

                        // åben og luk panneler efter behov
                        EventsStackPanelView.Visibility = Visibility.Visible;
                        StatusPanel.Visibility = Visibility.Collapsed;

                        // opdataer datagrid og luk loop
                        EventVareGrid.Items.Refresh();
                        done = true;
                        // succes besked
                        MessageBox.Show("Status er blevet redigeret");
                    }
                    // gå vider til den næste i listen
                    else
                    {
                        pos += 1;
                    }
                }
            }
            // fejl besked
            else
            {
                MessageBox.Show("Feltet er enten tomt eller også er der ikke blevet lavet en ændring");
            }
        }

        // Gør feltet, hvor man kan tilføjer lokationer, synligt eller usynligt
        private void addLokation_Click(object sender, RoutedEventArgs e)
        {
            // luk scrolpannel ned
            lokationScroll.Visibility = Visibility.Collapsed;

            // gem vare ned
            var row = (EventClass)EventGrid.SelectedItem;

            // tjekker om man har en valgt en vare inden man vil tilføje lokation
            if( row != null)
            {
                // åbner og lukker panneler så det er klart til at tilføje en lokation
                lokationPanel.Children.Clear();
                lokationPanel.Visibility = Visibility.Collapsed;
                addLokationStack.Visibility = Visibility.Visible;
                EventsBoxes.Visibility = Visibility.Collapsed;
                lukRedigeringPannel();
                lokationAdd.Focus();
            }
            // fejl besked
            else
            {
                MessageBox.Show("Du skal vælge et evemt før du kan tilføje en lokation");
            }



        }
        // api kald til at lave en nye lokation til event
        private async Task<List<VareClass>> createEventLokation(int id, string lokation)
        {
            JArray VareResult = await EventApi.CreateEventLokationAsync(id, lokation);
            return eventVareList;
        }
        public string RegexSave(string StrToSafe)
        {
            StrToSafe = Regex.Replace(StrToSafe, @"[^0-9a-zA-ZÆØÅæøå]+", "_");
            StrToSafe = Regex.Replace(StrToSafe, @"\s", "_");
            StrToSafe = Regex.Replace(StrToSafe, @"^\d", "_");
            StrToSafe = Regex.Replace(StrToSafe, "½", "_");
            return StrToSafe;
        }

        private void addLokationButton_Click(object sender, RoutedEventArgs e)
        {
            var row = (EventClass)EventGrid.SelectedItem;
            lokationPanel.Children.Clear();

            // Tilføj lokationer til event

            if (row != null && lokationAdd.Text != "")
            {
                lokationList.Add(new lokationerClass(row.EventId, RegexSave(lokationAdd.Text)));
                _ = createEventLokation(Convert.ToInt32(row.EventId), RegexSave(lokationAdd.Text));
                lokationAdd.Text = "";
            }
            else // Hvis et felt ikke er markeret
            {
                MessageBox.Show("Marker venligst et felt!");
            }

            // Ændre om feltet er synligt

            //addLokationStack.Visibility = Visibility.Collapsed;
            //EventsBoxes.Visibility = Visibility.Visible;



        }



        // åben panel til at ombytte vare
        private async void Ombytte_button(object sender, RoutedEventArgs e)
        {
            if (toogleOmbytte == false) {
                EventRow = (VareClass)EventVareGrid.SelectedItem;
                if (EventRow != null)
                {
                    eventVareLabel.Content = EventRow.Beskrivelse;
                    toogleOmbytte = true;
                    Events.Visibility = Visibility.Collapsed;
                    EventsStackPanelView.Visibility = Visibility.Collapsed;
                    OmbytVarePanel.Visibility = Visibility.Visible;
                    StatusPanel.Visibility = Visibility.Collapsed;
                    KopierEventPanel.Visibility = Visibility.Collapsed;
                    eventComboBox.Items.Clear();

                    JArray eventLocations = await EventApi.GetEventLokation(currentEventId);
                    if (eventLocations != null)
                    {
                        foreach (JObject item in eventLocations)
                        {
                            string eventLokation = (string)item.GetValue("Lokation");
                            if (eventLokation == EventRow.VareLokation)
                            {
                                eventComboBox.Items.Add(eventLokation);
                                eventComboBox.SelectedItem = eventLokation;
                            }
                            else
                            {
                                eventComboBox.Items.Add(eventLokation);
                            }


                        }

                    }
                }
                else
                {
                    MessageBox.Show("Du skal vælge en vare først");
                }
            }
            else
            {
                // åben og luk de forskellige paneler
                toogleOmbytte = false;
                ombytte = false;
                EventsStackPanelView.Visibility = Visibility.Visible;
                OmbytVarePanel.Visibility = Visibility.Collapsed;
            }
        }

        private async Task<JArray> EditEventVareLocation(string eventId, string vareId, string lokation)
        {
            JArray EditEventVareLokationAsync = await EventApi.EditEventVareLokationAsync(eventId, vareId, lokation);
            return EditEventVareLokationAsync;
        }

        // ombyt 2 vares lokationer efter man har valgt 2 vare
        private void ombytteVareButton_Click(object sender, RoutedEventArgs e)
        {
            _ = EditEventVareLocation(currentEventId.ToString(), EventRow.VareNr, eventComboBox.SelectedItem.ToString());
            getEventVareList();
        }

        // fortryd at ombytte vare
        private void ombytteAnnuller_Click(object sender, RoutedEventArgs e)
        {
            // gør klar til næste gang

            // åben og luk de forskellige paneler
            toogleOmbytte = false;
            ombytte = false;
            EventsStackPanelView.Visibility = Visibility.Visible;
            OmbytVarePanel.Visibility = Visibility.Collapsed;

        }



        private void EventTxtSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EventGrid.ItemsSource = myList;
            TextBox textbox = sender as TextBox;
            if (textbox != null)
            {
                searchstr = textbox.Text;
                if (!string.IsNullOrEmpty(searchstr))
                {
                    //EventGrid.Items.Filter = new Predicate<object>(filter_better_mayebe_idk);
                    EventFilter();
                }
                else
                {
                    EventGrid.ItemsSource = myList;
                    EventGrid.Items.Refresh();

                }
            }

        }

        private void EventFilter()
        {
            List<EventClass> data = myList;
            List<EventClass> myTempList = new List<EventClass>();

            int pos = 0;
            while (pos < 8)
            {
                // gem hvad der står i søgefelterne
                switch (pos)
                {
                    case 0:
                        if (!string.IsNullOrEmpty(EventNameBox.Text))
                        {
                            data = data.FindAll(x => x.EventName.ToLower().Contains(EventNameBox.Text.ToLower()));
                            break;
                        }
                        else
                        {
                            break;
                        }

                    case 1:
                        if (!string.IsNullOrEmpty(StartBox.Text))
                        {
                            data = data.FindAll(x => x.StartDato.ToLower().Contains(StartBox.Text.ToLower()));
                            break;
                        }
                        else
                        {
                            break;
                        }

                    case 2:
                        if (!string.IsNullOrEmpty(SlutBox.Text))
                        {
                            data = data.FindAll(x => x.SlutDato.ToLower().Contains(SlutBox.Text.ToLower()));
                            break;
                        }
                        else
                        {
                            break;
                        }

                    case 3:
                        if (!string.IsNullOrEmpty(NoteSeachBox.Text))
                        {
                            data = data.FindAll(x => x.Note.ToLower().Contains(NoteSeachBox.Text.ToLower()));
                            break;
                        }
                        else
                        {
                            break;
                        }

                    case 4:
                        if (!string.IsNullOrEmpty(DsStartBox.Text))
                        {
                            data = data.FindAll(x => x.DieselstanderStart.ToLower().Contains(DsStartBox.Text.ToLower()));
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case 5:
                        if (!string.IsNullOrEmpty(DsSlutBox.Text))
                        {
                            data = data.FindAll(x => x.DielselstanderSlut.ToLower().Contains(DsSlutBox.Text.ToLower()));
                            break;
                        }
                        else
                        {
                            break;
                        }

                    case 6:
                        if (!string.IsNullOrEmpty(DPBox.Text))
                        {
                            data = data.FindAll(x => x.DefektePærer.ToLower().Contains(DPBox.Text.ToLower()));
                            break;
                        }
                        else
                        {
                            break;
                        }

                    case 7:
                        if (!string.IsNullOrEmpty(TlfBox.Text))
                        {
                            data = data.FindAll(x => x.TlfNummer.ToLower().Contains(TlfBox.Text.ToLower()));
                            break;
                        }
                        else
                        {
                            break;
                        }

                    default:
                        break;
                }
                // hvis der er blevet skrevet nået i søgefelterne så skal den filtre
                if (data.Count != 0 && data != myList && data != myTempList)
                {
                    myTempList = new List<EventClass>();

                    foreach (var row in data)
                    {
                        myTempList.Add(new EventClass(row.EventId, row.EventName, row.StartDato, row.SlutDato, row.Note, row.DieselstanderStart, row.DielselstanderSlut, row.DefektePærer, row.TlfNummer, row.Aktiv, row.BrugerId));
                    }

                    data = myTempList;

                    pos += 1;

                }
                else
                {
                    pos += 1;
                }
            }
            // opdater datagrid
            EventGrid.ItemsSource = data;
            EventGrid.Items.Refresh();

        }



        // Tjek om checkbox ikke er trykket og køre den rigtige filtre
        private void EventHandleUnchecked(object sender, RoutedEventArgs e)
        {
            EventGrid.Items.Filter = new Predicate<object>(EventCheckFilter);
        }

        // Tjek om checkbox er trykket og køre den rigtige filtre
        private void EventHandleChecked(object sender, RoutedEventArgs e)
        {
            EventGrid.Items.Filter = new Predicate<object>(EventCheckFilter);
        }



        private bool EventCheckFilter(object o)
        {
            // tjekker om der er valgt en status og gemmer status i en variabel

            if (AktivCB.IsChecked == true && brugerCB.IsChecked == true)
            {
                if (o is EventClass)
                {
                    EventClass v = (o as EventClass);


                    if (v.Aktiv.ToUpper() == "JA" && v.BrugerId.ToUpper() == "JA")
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
            // hvis aktiv er krydset af men bruger id ikke skal kun events der er aktiv komme frem
            else if (AktivCB.IsChecked == true && brugerCB.IsChecked == false)
            {
                if (o is EventClass)
                {
                    EventClass v = (o as EventClass);


                    if (v.Aktiv.ToUpper() == "JA")
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
            // hvis bruger id er krydset af men áktiv id ikke er skal kun events der er bruger id komme frem
            else if (AktivCB.IsChecked == false && brugerCB.IsChecked == true)
            {
                if (o is EventClass)
                {
                    EventClass v = (o as EventClass);


                    if (v.BrugerId.ToUpper() == "JA")
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
                return true;
            }

        }




        private void txtSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            cvVare = CollectionViewSource.GetDefaultView(eventVareList);
            TextBox textbox = sender as TextBox;
            if (textbox != null)
            {
                // Hvis søgebar ikke er tom skal den filtre
                searchstr = textbox.Text;
                if (!string.IsNullOrEmpty(searchstr))
                {
                    Filter();

                    //Filter(textbox.Name, searchstr);
                }
                // hvis søgebare er tom skal den stop med at filtre og opfriske listen
                else
                {
                    EventVareGrid.ItemsSource = eventVareList;
                    EventVareGrid.Items.Refresh();


                }


            }
        }



        //filtre til søge function, favorit checkbox og status dropdown
        private void Filter()
        {

            int pos = 0;


            List<VareClass> data = eventVareList;
            List<VareClass> MyTempList = new List<VareClass>();


            while(pos < 13)
            {
                // gem hvad der står i søgefelterne
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
                        if (!string.IsNullOrEmpty(NoteVareBox.Text))
                        {
                            data = data.FindAll(x => x.Note.ToLower().Contains(NoteVareBox.Text.ToLower()));
                            break;
                        }
                        else
                        {
                            break;
                        }
                    default:
                        break;
                }

                // hvis der er blevet skrevet nået i søgefelterne så skal den filtre
                if(data.Count != 0 && data != eventVareList && data != MyTempList)
                {
                    MyTempList = new List<VareClass>();

                    foreach (var row in data)
                    {
                        MyTempList.Add(new VareClass(row.VareNr, row.Beskrivelse, row.Tilgang, row.Afgang, row.Ampere, row.Status, row.Antal, row.VareLokation, row.PinNr, row.Længde, row.Salgspris, row.Note, row.WebshopVareNummer, row.RFIDNummer, row.QR, row.EventVareId));
                    }

                    data = MyTempList;

                    pos += 1;
                }
                else
                {
                    pos += 1;
                }



            }



            // opdater datagrid
            EventVareGrid.ItemsSource = data;
            EventVareGrid.Items.Refresh();



        }




        //Tjek om checkbox ikke er trykket og køre den rigtige filtre
            private void handleUnchecked(object sender, RoutedEventArgs e)
        {
            // hvis søgebar er tom skal den kun køre filtre til favorit checkbox og status dropdown
            EventVareGrid.Items.Filter = new Predicate<object>(checkFilter);
        }

        // Tjek om checkbox er trykket og køre den rigtige filtre
        private void handleChecked(object sender, RoutedEventArgs e)
        {
             // hvis søgebar er tom skal den kun køre filtre til favorit checkbox og status dropdown
            EventVareGrid.Items.Filter = new Predicate<object>(checkFilter);
        }

        // Tjek om status er valgt og køre den rigtige filtre
        private void status_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // hvis søgebar er tom skal den kun køre filtre til favorit checkbox og status dropdown
            EventVareGrid.Items.Filter = new Predicate<object>(checkFilter);
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

            // hvis en status er valgt skal listen filtres efter hvilken status man har valgt
            if (val != " ")
            {
                if (o is VareClass)
                {
                    VareClass v = (o as VareClass);


                    if ( v.Status == val)
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
            // hvis ingen status er valgt skal der ikke filtres efter status
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

        // tilføj lokation
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
        // tjek om checkBoxKopir er blevet checked
        private void checkBoxKopir_Checked(object sender, RoutedEventArgs e)
        {

            checkBoxKopir_Checked_Bool = true;
            checkBoxKopir_Checked_String = "true";
        }

        // tjek om checkBoxKopir ikke er blevet checked
        private void checkBoxKopir_Unchecked(object sender, RoutedEventArgs e)
        {
            checkBoxKopir_Checked_Bool = false;
            checkBoxKopir_Checked_String = "false";
        }
        // lav en følgeseddel
        private void seddel_button(object sender, RoutedEventArgs e)
        {
            // gem fil dialog
            SaveFileDialog saveFile = new SaveFileDialog()
            {
                Title = "Gem din Fil",
                Filter = "Adobe PDF(*.pdf) | *.pdf",
                FileName = valgteEvent
            };

            // lave indehold til pdf
            if (saveFile.ShowDialog() == true)
            {
                try
                {
                    // lav forbindelse til fil
                    FileStream fs = new FileStream(saveFile.FileName, FileMode.Create);

                    // vælg papir format
                    Document document = new Document(PageSize.A4, 60, 60, 30, 30);

                    //skriv til pdf fil
                    PdfWriter writer = PdfWriter.GetInstance(document, fs);

                    //gem en font
                    BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

                    iTextSharp.text.Font titelFont = new iTextSharp.text.Font(bf, 24, iTextSharp.text.Font.BOLD);
                    iTextSharp.text.Font VareTitelFont = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.BOLD);
                    iTextSharp.text.Font font = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.NORMAL);

                    bool done = false;
                    int pos = 0;

                    // init linjer af tekst til starten af dokumentet
                    iTextSharp.text.Paragraph afhentes = new iTextSharp.text.Paragraph("dato");
                    iTextSharp.text.Paragraph tid = new iTextSharp.text.Paragraph("tid");
                    iTextSharp.text.Paragraph titel = new iTextSharp.text.Paragraph();

                    // gem alle vare ned
                    List<VareClass> vareListe = cvVare.Cast<VareClass>().ToList();

                    // sorter vare
                    List<VareClass> order = vareListe.OrderBy(o => o.Beskrivelse).ToList();
                    string vareNavn = order[0].Beskrivelse;

                    // lave dato
                    while (done == false)
                    {

                        if (myList[pos].EventId == EventId)
                        {
                            string res = myList[pos].SlutDato;
                            var parsedDate = DateTime.Parse(res);
                            afhentes = new iTextSharp.text.Paragraph("Afhentes d. " + parsedDate.ToString("MM/dd/yyyy"), font);
                            tid = new iTextSharp.text.Paragraph("Afleveres d. " + parsedDate.ToString("MM/dd/yyyy") + " Senest kl 12:00\n\n", font);
                            titel = new iTextSharp.text.Paragraph("Følgeseddel for " + myList[pos].EventName + "\n\n", titelFont);
                            titel.Alignment = Element.ALIGN_CENTER;
                            done = true;
                            pos = 0;
                        }
                        else
                        {
                            pos += 1;
                        }

                    }






                    //åben dokument
                    document.Open();

                    // lav indehold

                    //tilføj indehold
                    writer.PageEvent = new Header();
                    writer.PageEvent = new Footer();

                    // tilføj linjer af tekst til dokumentet
                    document.Add(titel);
                    document.Add(afhentes);
                    document.Add(tid);


                    string VareId = "";
                    int vpos = 1;
                    int count = 0;
                    bool vdone = false;
                    while (done == true)
                    {
                        // grup vare sammen efter beskrivelse og tilføj dem til dokument
                        if (pos < myList.Count && pos == 0)
                        {
                            iTextSharp.text.Paragraph VareTitel = new iTextSharp.text.Paragraph(vareNavn, VareTitelFont);
                            VareId = order[0].VareNr;
                            while (vdone == false && vpos < order.Count)
                            {
                                if (vareNavn == order[vpos].Beskrivelse )
                                {
                                    VareId = VareId + "," + order[vpos].VareNr;
                                    vpos += 1;
                                    count += 1;
                                }
                                else
                                {
                                    vdone = true;
                                }

                            }
                            // tjek hvor mange vare er på sidde og hvis der er for mange skal gruppen tilføjedes til en ny sidde
                            if (count >= 120)
                            {
                                document.NewPage();
                                document.Add(VareTitel);
                                iTextSharp.text.Paragraph VareNr = new iTextSharp.text.Paragraph(VareId + "\n\n", font);
                                document.Add(VareNr);
                                pos = vpos;
                                count = 0;
                            }
                            else
                            {
                                document.Add(VareTitel);
                                iTextSharp.text.Paragraph VareNr = new iTextSharp.text.Paragraph("\n" + VareId + "\n\n", font);
                                document.Add(VareNr);
                                pos = vpos;
                            }


                        }
                        // start en nye gruppe af vare
                        else if (pos < order.Count && order[pos].Beskrivelse != vareNavn)
                        {
                            vareNavn = order[pos].Beskrivelse;
                            iTextSharp.text.Paragraph VareTitel = new iTextSharp.text.Paragraph(vareNavn, VareTitelFont);
                            VareId = order[vpos].VareNr;
                            vdone = false;
                            while (vdone == false)
                            {
                                if (vareNavn == order[vpos].Beskrivelse && vpos < (order.Count - 1))
                                {
                                    vpos += 1;
                                    count += 1;
                                    if (vareNavn == order[vpos].Beskrivelse)
                                    {
                                        VareId = VareId + "," + order[vpos].VareNr;
                                    }


                                }
                                else
                                {
                                    vdone = true;
                                }

                            }
                            // tjek hvor mange vare er på sidde og hvis der er for mange skal gruppen tilføjedes til en ny sidde
                            if (count >= 120)
                            {
                                document.NewPage();
                                document.Add(VareTitel);
                                iTextSharp.text.Paragraph VareNr = new iTextSharp.text.Paragraph(VareId + "\n\n", font);
                                document.Add(VareNr);
                                pos = vpos;
                                count = 0;
                            }
                            else
                            {
                                document.Add(VareTitel);
                                iTextSharp.text.Paragraph VareNr = new iTextSharp.text.Paragraph(VareId + "\n\n", font);
                                document.Add(VareNr);
                                pos = vpos;
                            }
                        }
                        else
                        {
                            done = false;
                        }

                    }




                    PdfContentByte cb = writer.DirectContent;


                    // vi fortæller ContentByte, at vi er klar til at tegne tekst
                    cb.BeginText();


                    // vi tegner en tekst på en bestemt position
                    cb.SetTextMatrix(60, 130);
                    cb.SetFontAndSize(bf, 12);
                    cb.ShowText("Dato:_____________________\n\n\n");
                    cb.EndText();

                    // vi tegner en tekst på en bestemt position
                    cb.BeginText();
                    cb.SetTextMatrix(60, 80);
                    cb.SetFontAndSize(bf, 12);
                    cb.ShowText("______________________________\n");
                    cb.EndText();

                    // vi tegner en tekst på en bestemt position
                    cb.BeginText();
                    cb.SetTextMatrix(60, 68);
                    cb.SetFontAndSize(bf, 12);
                    cb.ShowText("For modtagelse kvitteres");
                    cb.EndText();





                    // luk forbindelse til fil og luk fil
                    document.Close();
                    writer.Close();
                    fs.Close();



                }
                //  fejl besked
                catch (IOException)
                {
                    MessageBox.Show("Kunne ikke lave/overskrive pdf filen. \nHvis du har filen åben så luk filen og prøv igen.\nHvis filen ikke er åben så tjek om filen er skrivebeskyttet.");
                }
            }
            // fejl besked
            else
            {
                MessageBox.Show("Din liste er tom prøv igen på med en liste der ikke er tom");
            }
        }

        // footer til følgeseddel
        public partial class Footer : PdfPageEventHelper
        {
            public override void OnEndPage(PdfWriter writer, Document doc)
            {
                // lav en tabel til footer
                PdfPTable footerTbl = new PdfPTable(2);
                footerTbl.TotalWidth = 600;
                footerTbl.HorizontalAlignment = Element.ALIGN_CENTER;

                // gem tidpunkt ned
                DateTime dateTime = DateTime.UtcNow.Date;

                // lav celle med besked til ventre side
                PdfPCell cell = new PdfPCell(new iTextSharp.text.Paragraph("Skive El-Service", FontFactory.GetFont(FontFactory.TIMES, 12, iTextSharp.text.Font.NORMAL)));
                // lav celle med formateret dato til højre side
                PdfPCell cell1 = new PdfPCell(new iTextSharp.text.Paragraph(dateTime.ToString("dd/MM/yyyy"), FontFactory.GetFont(FontFactory.TIMES, 12, iTextSharp.text.Font.NORMAL)));
                // tabel style
                cell.Border = 0;
                cell.PaddingLeft = 0;
                cell1.Border = 0;
                cell1.PaddingLeft = 127;

                // tilføj celler til tabel
                footerTbl.AddCell(cell);
                footerTbl.AddCell(cell1);
                // tabel position
                footerTbl.WriteSelectedRows(0, -1, 60, 18, writer.DirectContent);
            }


        }

        // header til følgeseddel
        public partial class Header : PdfPageEventHelper
        {
            // lave en template til header
            private PdfContentByte cb;
            private List<PdfTemplate> templates;

            //constructor
            public Header()
            {
                this.templates = new List<PdfTemplate>();
            }

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                // lav en tabel
                PdfPTable headerTbl = new PdfPTable(1);
                headerTbl.TotalWidth = 200;
                headerTbl.HorizontalAlignment = Element.ALIGN_CENTER;

                DateTime dateTime = DateTime.UtcNow.Date;


                // lav en celle med tekst til tabel
                PdfPCell cell = new PdfPCell(new iTextSharp.text.Paragraph("Følgeseddel", FontFactory.GetFont(FontFactory.TIMES, 12, iTextSharp.text.Font.NORMAL)));
                cell.Border = 0;
                cell.PaddingLeft = 0;
                headerTbl.AddCell(cell);

                // tegn tabel på en bestemt position på pdf
                headerTbl.WriteSelectedRows(0, -1, 60, 845, writer.DirectContent);
                cb = writer.DirectContentUnder;
                PdfTemplate templateM = cb.CreateTemplate(50, 50);
                templates.Add(templateM);

                //gem siddetal
                int pageN = writer.CurrentPageNumber;


                // lav en tekst og tilføj den til en bestemt position på dokumentet
                string pageText = "Side " + pageN.ToString() + " af ";
                BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb.BeginText();
                cb.SetFontAndSize(bf, 12);
                cb.SetTextMatrix(document.PageSize.GetRight(document.LeftMargin) - 46, document.PageSize.GetTop(document.BottomMargin) + 19);
                cb.ShowText(pageText);
                cb.EndText();

                // tilføj template til dokument
                cb.AddTemplate(templateM, document.PageSize.GetRight(document.LeftMargin), document.PageSize.GetTop(document.BottomMargin) + 19);
            }

            public override void OnCloseDocument(PdfWriter writer, Document document)
            {

                base.OnCloseDocument(writer, document);
                BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                // tilføj den totale sidde tal til på en bestemt position til hver sidde
                foreach (PdfTemplate item in templates)
                {
                    item.BeginText();
                    item.SetFontAndSize(bf, 12);
                    item.SetTextMatrix(0, 0);
                    item.ShowText(writer.PageNumber.ToString());
                    item.EndText();
                }

                // lav en template
                PdfTemplate bla = cb.CreateTemplate(50, 50);
                templates.Add(bla);

                // tegn en tekst på en bestemt position
                string start = "Følgeseddel";
                BaseFont sf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb.BeginText();
                cb.SetFontAndSize(sf, 12);
                cb.SetTextMatrix(document.LeftMargin, document.PageSize.GetTop(document.BottomMargin));
                cb.ShowText(start);
                cb.EndText();
                // til føj template til dokument
                cb.AddTemplate(bla, document.LeftMargin, document.PageSize.GetTop(document.BottomMargin));

            }

        }

        // filtre alle vare fra der ikke har en Tlf Nummer der hedder det samme som i textboxen når man trykker enter
        private void TlfEnter(object sender, KeyEventArgs e)
        {
            List<EventClass> data = myList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.TlfNummer.ToLower().Equals(TlfBox.Text.ToLower()));
            }

            EventGrid.ItemsSource = data;
        }

        // filtre alle vare fra der ikke har en Defekte Pærer der hedder det samme som i textboxen når man trykker enter
        private void DPEnter(object sender, KeyEventArgs e)
        {
            List<EventClass> data = myList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.DefektePærer.ToLower().Equals(DPBox.Text.ToLower()));
            }

            EventGrid.ItemsSource = data;
        }

        // filtre alle vare fra der ikke har en DielselstanderSlut der hedder det samme som i textboxen når man trykker enter
        private void DsSlutEnter(object sender, KeyEventArgs e)
        {
            List<EventClass> data = myList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.DielselstanderSlut.ToLower().Equals(DsSlutBox.Text.ToLower()));
            }

            EventGrid.ItemsSource = data;
        }

        // filtre alle vare fra der ikke har en DieselstanderStart der hedder det samme som i textboxen når man trykker enter
        private void DsStartEnter(object sender, KeyEventArgs e)
        {
            List<EventClass> data = myList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.DieselstanderStart.ToLower().Equals(DsStartBox.Text.ToLower()));
            }

            EventGrid.ItemsSource = data;
        }

        // filtre alle vare fra der ikke har en Note der hedder det samme som i textboxen når man trykker enter
        private void eventNoteEnter(object sender, KeyEventArgs e)
        {
            List<EventClass> data = myList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.Note.ToLower().Equals(NoteSeachBox.Text.ToLower()));
            }

            EventGrid.ItemsSource = data;
        }

        // filtre alle vare fra der ikke har en Slut Dato der hedder det samme som i textboxen når man trykker enter
        private void SlutEnter(object sender, KeyEventArgs e)
        {
            List<EventClass> data = myList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.SlutDato.ToLower().Equals(DatePickerSlut.SelectedDate.ToString().ToLower()));
            }

            EventGrid.ItemsSource = data;
        }

        // filtre alle vare fra der ikke har en Start Dato der hedder det samme som i textboxen når man trykker enter
        private void StartEnter(object sender, KeyEventArgs e)
        {
            List<EventClass> data = myList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.StartDato.ToLower().Equals(DatePickerStart.SelectedDate.ToString().ToLower()));
            }

            EventGrid.ItemsSource = data;
        }

        // filtre alle vare fra der ikke har en event navn der hedder det samme som i textboxen når man trykker enter
        private void NameEnter(object sender, KeyEventArgs e)
        {
            List<EventClass> data = myList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.EventName.ToLower().Equals(EventNameBox.Text.ToLower()));
            }

            EventGrid.ItemsSource = data;
        }


        // filtre alle vare fra der ikke har en vare nr. der hedder det samme som i textboxen når man trykker enter
        private void VareNrEnter(object sender, KeyEventArgs e)
        {
            List<VareClass> data = eventVareList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.VareNr.ToLower().Equals(VareNrBox.Text.ToLower()));
            }

            EventVareGrid.ItemsSource = data;
        }


        // filtre alle vare fra der ikke har en Beskrivelse der hedder det samme som i textboxen når man trykker enter
        private void beskrivelseEnter(object sender, KeyEventArgs e)
        {
            List<VareClass> data = eventVareList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.Beskrivelse.ToLower().Equals(beskrivelseBox.Text.ToLower()));
            }

            EventVareGrid.ItemsSource = data;
        }

        // filtre alle vare fra der ikke har en Antal der hedder det samme som i textboxen når man trykker enter
        private void AntalEnter(object sender, KeyEventArgs e)
        {
            List<VareClass> data = eventVareList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.Antal.Equals(Int32.Parse(AntalBox.Text)));
            }

            EventVareGrid.ItemsSource = data;
        }

        // filtre alle vare fra der ikke har en Lokation der hedder det samme som i textboxen når man trykker enter
        private void LokationEnter(object sender, KeyEventArgs e)
        {
            List<VareClass> data = eventVareList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.VareLokation.ToLower().Equals(LokationBox.Text.ToLower()));
            }

            EventVareGrid.ItemsSource = data;
        }

        // filtre alle vare fra der ikke har en Ampere der hedder det samme som i textboxen når man trykker enter
        private void AmpereEnter(object sender, KeyEventArgs e)
        {
            List<VareClass> data = eventVareList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.Ampere.ToLower().Equals(AmpereBox.Text.ToLower()));
            }

            EventVareGrid.ItemsSource = data;
        }

        // filtre alle vare fra der ikke har en Tilgang der hedder det samme som i textboxen når man trykker enter
        private void TilgangEnter(object sender, KeyEventArgs e)
        {
            List<VareClass> data = eventVareList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.Tilgang.ToLower().Equals(TilgangBox.Text.ToLower()));
            }

            EventVareGrid.ItemsSource = data;
        }

        // filtre alle vare fra der ikke har en Afgang der hedder det samme som i textboxen når man trykker enter
        private void AfgangEnter(object sender, KeyEventArgs e)
        {
            List<VareClass> data = eventVareList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.Afgang.ToLower().Equals(AfgangBox.Text.ToLower()));
            }

            EventVareGrid.ItemsSource = data;
        }

        // filtre alle vare fra der ikke har en PinNr der hedder det samme som i textboxen når man trykker enter
        private void PinEnter(object sender, KeyEventArgs e)
        {
            List<VareClass> data = eventVareList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.PinNr.ToLower().Equals(PinBox.Text.ToLower()));
            }

            EventVareGrid.ItemsSource = data;
        }

        // filtre alle vare fra der ikke har en Længde der hedder det samme som i textboxen når man trykker enter
        private void LængdeEnter(object sender, KeyEventArgs e)
        {
            List<VareClass> data = eventVareList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.Længde.Equals(Int32.Parse(LængdeBox.Text)));
            }

            EventVareGrid.ItemsSource = data;
        }

        // filtre alle vare fra der ikke har en WebshopVareNummer der hedder det samme som i textboxen når man trykker enter
        private void WebEnter(object sender, KeyEventArgs e)
        {
            List<VareClass> data = eventVareList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.WebshopVareNummer.ToLower().Equals(webBox.Text.ToLower()));
            }

            EventVareGrid.ItemsSource = data;
        }

        // filtre alle vare fra der ikke har en Note der hedder det samme som i textboxen når man trykker enter
        private void NoteEnter(object sender, KeyEventArgs e)
        {
            List<VareClass> data = eventVareList;

            if (e.Key == Key.Return)
            {
                data = data.FindAll(x => x.Note.ToLower().Equals(NoteVareBox.Text.ToLower()));
            }

            EventVareGrid.ItemsSource = data;
        }

        private void nextEnter(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Return)
            {
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                request.Wrapped = true;
                ((TextBox)sender).MoveFocus(request);
            }

        }

        private void nextEnterCheckbox(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Return)
            {
                if(checkBoxKopir.IsChecked == true)
                {
                    checkBoxKopir.IsChecked = false;
                }
                else
                {
                    checkBoxKopir.IsChecked = true;
                }
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                request.Wrapped = true;
                ((CheckBox)sender).MoveFocus(request);
            }

        }

        private void nextEnterCombox(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Return)
            {
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                request.Wrapped = true;
                ((ComboBox)sender).MoveFocus(request);
            }

        }

        private void ombytteEnter(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Return)
            {
                _ = EditEventVareLocation(currentEventId.ToString(), EventRow.VareNr, eventComboBox.SelectedItem.ToString());
                getEventVareList();
            }

        }

        private void changeStatusEnter(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Return)
            {
                // gem den valgte vare ned
                var row = (VareClass)EventVareGrid.SelectedItem;
                // index variabel
                int pos = 0;
                // variabel til at stoppe loop
                bool done = false;
                // gem combobox værdi ned
                string newStatus = ChangeStatusBox.Text;

                if (row != null && newStatus != "" && row.Status != newStatus)
                {
                    //Looper igennem datagrid for at finde positionen på det item du har valgt
                    while (done == false && pos < eventVareList.Count())
                    {
                        //Hvis position data stemmer overens med det valgte datagrid data -> gå amok
                        if (eventVareList[pos].VareNr == row.VareNr && eventVareList[pos].Status == row.Status)
                        {
                            // api kald til at ændre status på event vare
                            _ = EditEventVareStatusAsync(eventVareList[pos].EventVareId, newStatus);
                            eventVareList[pos] = new VareClass(row.VareNr, row.Beskrivelse, row.Tilgang, row.Afgang, row.Ampere, newStatus, row.Antal, row.VareLokation, row.PinNr, row.Længde, row.Salgspris, row.Note, row.WebshopVareNummer, row.RFIDNummer, row.QR);

                            // åben og luk panneler efter behov
                            EventsStackPanelView.Visibility = Visibility.Visible;
                            StatusPanel.Visibility = Visibility.Collapsed;

                            // opdataer datagrid og luk loop
                            EventVareGrid.Items.Refresh();
                            done = true;
                            // succes besked
                            MessageBox.Show("Status er blevet redigeret");
                        }
                        // gå vider til den næste i listen
                        else
                        {
                            pos += 1;
                        }
                    }
                }
                // fejl besked
                else
                {
                    MessageBox.Show("Feltet er enten tomt eller også er der ikke blevet lavet en ændring");
                }
            }

        }

        private void tilføjLokationEnter(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Return)
            {
                addLokationButton_Click(sender, e);
            }

        }

        private void redigerLokationEnter(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Return)
            {
                int pos = 0;
                bool done = false;
                NewLokation = lokationRedigerBox.Text;

                // tjek om du har lavet en ændring
                if (NewLokation != null && lokationStr != NewLokation)
                {
                    while (done == false && pos < lokationList.Count())
                    {
                        // rediger lokation
                        if (lokationList[pos].EventId == redigerId && lokationList[pos].Lokation == lokationStr && lokationList[pos].LokationId == lokationId)
                        {
                            _ = editEventLocation(lokationId, redigerId, NewLokation);
                            lokationList[pos] = new lokationerClass(redigerId, NewLokation, lokationId);
                            redigerLokationStack.Visibility = Visibility.Collapsed;
                            EventsBoxes.Visibility = Visibility.Visible;
                            EventGrid.Items.Refresh();
                            // succes besked
                            MessageBox.Show("lokation er blevet redigeret");
                            done = true;
                        }
                        else
                        {
                            pos += 1;
                        }
                    }
                }
                // fejl besked
                else
                {
                    MessageBox.Show("Feltet er inten tom eller også er der ikke blevet gjort nogen ændring");
                }
            }

        }

        private void aktivCheck_Checked(object sender, RoutedEventArgs e)
        {
            EventsStackPanel.Children.Clear();

            List<EventClass> temp = new List<EventClass>();

            foreach(EventClass ev in myList)
            {
                if (ev.Aktiv == "Ja")
                {
                    temp.Add(ev);
                }
            }


            BrushConverter bc = new BrushConverter();
            int i = 1;
            foreach (EventClass item in temp)
            {
                string eventName = item.EventName;
                string eventId = item.EventId;
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

                    EventId = eventId;
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

        private void aktivCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            EventsStackPanel.Children.Clear();

            _ = PullEventsToButtons();
        }

        private void DatePickerSlut_CalendarClosed(object sender, RoutedEventArgs e)
        {
            //SlutDatoBox.Text = DatePickerSlut.SelectedDate.ToString();
        }

        private void DatePickerStart_CalendarClosed(object sender, RoutedEventArgs e)
        {
            //StartDatoBox.Text = DatePickerStart.SelectedDate.ToString();
        }

        private void TilbageKnapEvent_MouseDown(object sender, MouseButtonEventArgs e)
        {
            goBack_Click(sender, e);
        }

        private void TilbageKnapEvent_MouseEnter(object sender, MouseEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            TilbageKnapEvent.Foreground = (Brush)bc.ConvertFrom("#CCC");
        }

        private void TilbageKnapEvent_MouseLeave(object sender, MouseEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            TilbageKnapEvent.Foreground = (Brush)bc.ConvertFrom("#FFF");
        }

        JArray eventResult2 = new JArray();

        private void EventFilter2()
        {
            List<EventClass> data = myList;
            List<EventClass> myTempList = new List<EventClass>();

            Dispatcher.Invoke(() =>
            {
                        if (!string.IsNullOrEmpty(SearchEventNameEvent.Text))
                        {
                            data = data.FindAll(x => x.EventName.ToLower().Contains(SearchEventNameEvent.Text.ToLower()));
                        }
            });
                // hvis der er blevet skrevet nået i søgefelterne så skal den filtre
                if (data.Count != 0 && data != myList && data != myTempList)
                {
                    myTempList = new List<EventClass>();

                    foreach (var row in data)
                    {
                        myTempList.Add(new EventClass(row.EventId, row.EventName, row.StartDato, row.SlutDato, row.Note, row.DieselstanderStart, row.DielselstanderSlut, row.DefektePærer, row.TlfNummer, row.Aktiv, row.BrugerId));
                    }

                    data = myTempList;
                }

            BrushConverter bc = new BrushConverter();
            int i = 1;

            foreach (var item in data)
            {

            // opdater datagrid
            string eventName = (string)item.EventName;

            string eventId = (string)item.EventId;
                Dispatcher.Invoke(() => { 
                
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

                EventId = eventId;
                eventVareList.Clear();
                EventVareGrid.Items.Refresh();
                eventstr = myButton.Name;
                Match idString = re.Match(eventstr);
                selectedEventLabel.Content = "Event: " + eventName;
                if (idString.Success)
                {
                    int id = int.Parse(idString.Value);
                    Event_Click(s, EventArgs, id, eventName);
                }

            };
                EventsStackPanel.Children.Add(myButton);
                });
            i++;
            }
        }

        private async void SearchEventNameEvent_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (eventResult2 != null)
            {
                EventsStackPanel.Children.Clear();
                selectedEventLabel.Content = "Event: ";
                await Task.Run(() =>
                { EventFilter2(); });
            }
        }

        private void DatePickerSlut_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void ryd_Click(object sender, RoutedEventArgs e)
        {

        }

        private void rydpic_MouseEnter(object sender, MouseEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            
        }
    }
    
}