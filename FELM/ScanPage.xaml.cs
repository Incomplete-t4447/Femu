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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json.Linq;
using Brush = System.Windows.Media.Brush;
using System.IO;
using System.Data;
using System.ComponentModel;

namespace FELM
{
    /// <summary>
    /// Interaction logic for ScanPage.xaml
    /// </summary>
    public partial class ScanPage : Page
    {
        bool ToggleFavorit = false;
        bool ToggleEvent = false;
        ICollectionView cvVare = CollectionViewSource.GetDefaultView(myList);
        public static List<VareClass> myList = new List<VareClass>();
        API vareApi = new API();
        string eventstr = " ";

        API Api = new API();
        public ScanPage()
        {
            InitializeComponent();
            Vare.ItemsSource = cvVare;
        }

        private void ScanBackBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(Pages.p5);
        }

        void Onload(object sender, RoutedEventArgs e)
        {
            _ = GetVareAsync();
        }

        private async Task<List<VareClass>> GetVareAsync()
        {

            JArray VareResult = await Api.GetAllVare();

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

            Vare.Items.Refresh();

            return myList;
        }


        private async void history_Button(object sender, RoutedEventArgs e)
        {
            JArray stringResult = await Api.AllEventsQueryAsync();

            var bc = new BrushConverter();

            Console.WriteLine(stringResult);
            EventStackPanel.Children.Clear();
            for (int i = 0; i < stringResult.Count(); i++)
            {
                Button newButton = new Button();
                if (i % 2 == 0)
                {
                    newButton.Background = (Brush)bc.ConvertFrom("#00b5a3");
                }
                else
                {
                    newButton.Background = (Brush)bc.ConvertFrom("#017056");
                }

                newButton.Name = $"eventbutton{i}";
                newButton.Content = stringResult[i].First.First;
                newButton.Width = 420;
                newButton.HorizontalAlignment = HorizontalAlignment.Center;
                newButton.VerticalAlignment = VerticalAlignment.Center;
                newButton.FontWeight = FontWeights.Bold;
                newButton.BorderThickness = new Thickness(0);
                newButton.Foreground = (Brush)bc.ConvertFrom("#FFFFFF");
                newButton.Margin = new Thickness(0, 10, 0, 0);


                newButton.Click += (s, se) => {/*API CALL MM. her*/};
                EventStackPanel.Children.Add(newButton);
            }
        }
        Button DesignButton(Button newButton)
        {
            var bc = new BrushConverter();
            newButton.Width = 420;
            newButton.HorizontalAlignment = HorizontalAlignment.Center;
            newButton.VerticalAlignment = VerticalAlignment.Center;
            newButton.FontWeight = FontWeights.Bold;
            newButton.BorderThickness = new Thickness(0);
            newButton.Foreground = (Brush)bc.ConvertFrom("#FFFFFF");
            newButton.Margin = new Thickness(0, 10, 0, 0);
            return newButton;
        }
        private async void Favorit_Button(object sender, RoutedEventArgs e)
        {
            if (ToggleFavorit == false)
            {
                JArray stringResult = await Api.AllFavoritsQuery();
                //string[] stringRArray = new string[] { "hej", "nope", "hypsa" };

                var bc = new BrushConverter();

                //Console.WriteLine(stringResult);
                FavoritBorder.Visibility = Visibility.Visible;
                FavoritStackPanel.Children.Clear();
                for (int i = 0; i < stringResult.Count(); i++)
                {
                    Button newButton = new Button();
                    Tilføj_vare popup = new Tilføj_vare();
                    if (i % 2 == 0)
                    {
                        newButton.Background = (Brush)bc.ConvertFrom("#00b5a3");
                    }
                    else
                    {
                        newButton.Background = (Brush)bc.ConvertFrom("#017056");
                    }

                    newButton.Name = $"FavoritButton{i}";
                    newButton.Content = stringResult[i].First.First;

                    DesignButton(newButton);

                    popup.Vare_Label.Content = stringResult[i].First.First;


                    newButton.Click += (s, se) =>
                    {
                        popup.ShowDialog();
                    };
                    FavoritStackPanel.Children.Add(newButton);



                    ToggleFavorit = true;
                }
            }
            else if (ToggleFavorit == true)
            {
                FavoritBorder.Visibility = Visibility.Hidden;
                ToggleFavorit = false;
            }

        }

        private void Antenne_Button(object sender, RoutedEventArgs e)
        {
            //JArray hypsa = await Api.GetItemsQueryAsync();
        }

        private async void Toggle_Aflevering_Button(object sender, RoutedEventArgs e)
        {
            //henter vores events fra API'en
            JArray eventArray = await Api.AllEventsQueryAsync();

            var bc = new BrushConverter();

            //det er vigtigt at Clear() før man prøver at indsætte ting i stackpanelet fordi ellers kan man tilføje de samme ting flere gange
            EventStackPanel.Children.Clear();
            for (int i = 0; i < eventArray.Count(); i++)
            {
                Button newButton = new Button();
                if (i % 2 == 0)
                {
                    newButton.Background = (Brush)bc.ConvertFrom("#00b5a3");
                }
                else
                {
                    newButton.Background = (Brush)bc.ConvertFrom("#017056");
                }
                //tildeler diverse attributes til event knapperne
                newButton.Tag = i;
                newButton.Name = $"eventbutton{i}";
                newButton.Content = eventArray[i].First.First;
                DesignButton(newButton);

                newButton.Click += async (s, se) => {
                    int eventId = new int();

                    //hvorfor skal graphQL virke som det gør? Last.Last er vores eventID fra vores eventArray men C# ved ikke at det er en INT så....
                    eventId = Int32.Parse(eventArray[newButton.Tag].Last.Last.ToString());

                    //API call her
                    JArray items = await Api.GetItemsQueryAsync(eventId);
                    Console.WriteLine(items);
                    ItemClass itemClass = new ItemClass();

                    //sorterer vores items om til en liste af de individuelle items hvilket gør at vores datagrid kan modtage og vise dem
                    List<ItemClass> itemList = itemClass.Itemize(items);

                    //sætter itemsSource til null så at den fjerner gamle items fra vores grid
                    Vare.ItemsSource = null;
                    Vare.ItemsSource = itemList;
                };
                EventStackPanel.Children.Add(newButton);
            }
        }

        private void Start_Scanner(object sender, RoutedEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Vare_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Start_varelokalition(object sender, RoutedEventArgs e)
        {

            Vare_lokalition popUp = new Vare_lokalition();

            popUp.ShowDialog();



        }

        private async void Start_Events(object sender, RoutedEventArgs e)
        {


            if (ToggleEvent == false && ToggleFavorit == false)
            {
                JArray eventResult = await Api.AllEventsQueryAsync();

                EventsBorder.Visibility = Visibility.Visible;
                
                EventsStackPanel.Children.Clear();

                BrushConverter bc = new BrushConverter();

                for (int i = 0; i < eventResult.Count(); i++)
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

                    myButton.Name = $"Eventsbutton{i}";
                    myButton.Content = eventResult[i].First.First;
                    myButton.Width = 420;
                    myButton.HorizontalAlignment = HorizontalAlignment.Center;
                    myButton.VerticalAlignment = VerticalAlignment.Center;
                    myButton.FontWeight = FontWeights.Bold;
                    myButton.BorderThickness = new Thickness(0);
                    myButton.Foreground = (Brush)bc.ConvertFrom("#FFFFFF");
                    myButton.Margin = new Thickness(0, 10, 0, 0);

                    alertPopup.Visibility = Visibility.Hidden;
                    ToggleEvent = true;

                    myButton.Click += (s, se) =>
                    {
                        eventVare.Visibility = Visibility.Visible;
                        eventstr = myButton.Content.ToString();
                    };

                    EventsStackPanel.Children.Add(myButton);

                }
            }
            else if (ToggleEvent == true && ToggleFavorit == false)
            {

                EventsBorder.Visibility = Visibility.Hidden;
                alertPopup.Visibility = Visibility.Hidden;
                eventVare.Visibility = Visibility.Hidden;
                ToggleEvent = false;

            }
            else if (ToggleEvent == true && ToggleFavorit == true)
            {
                ToggleFavorit = false;
                ToggleEvent = true;
                alertPopup.Visibility = Visibility.Visible;
                alertPopup.Text = "Du kan ikke havde favotitter og events åben";
                EventsBorder.Visibility = Visibility.Hidden;

            }

        }

        private void Start_eventVare(object sender, RoutedEventArgs e)
        {
            EventVare popUp = new EventVare(eventstr);

            popUp.ShowDialog();

            
        }

        private void OpretEvent_Button(object sender, RoutedEventArgs e)
        {

        }

        private void StartUdlejning_Button(object sender, RoutedEventArgs e)
        {

        }

        private void Afleveringsmode_Button(object sender, RoutedEventArgs e)
        {

        }
    }
}