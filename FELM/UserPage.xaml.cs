using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json.Linq;
using SAHB.GraphQLClient.QueryGenerator;
using DataGrid = System.Windows.Controls.DataGrid;
using DataGridCell = System.Windows.Controls.DataGridCell;
using MessageBox = System.Windows.MessageBox;
using SAHB.GraphQL;
using SAHB.GraphQLClient;
using SAHB.GraphQLClient.Extentions;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using SAHB.GraphQLClient.QueryGenerator;
using TextBox = System.Windows.Controls.TextBox;
using Button = System.Windows.Controls.Button;
using ComboBox = System.Windows.Controls.ComboBox;
using Label = System.Windows.Controls.Label;
using CheckBox = System.Windows.Controls.CheckBox;
using StackPanel = System.Windows.Controls.StackPanel;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using CS203Engine;
using System.Windows.Threading;

namespace FELM
{
    /// <summary>
    /// Interaction logic for UserPage.xaml
    /// </summary>
    ///
    public class Colors
    {
        private string _color;
        public Colors(string color)
        {
            this._color = color;
        }

        public string color
        {
            get { return _color; }
            set { _color = value; }
        }
    }

    public class UserEvents
    {
        private string _events;
        public UserEvents(string events)
        {
            this._events = events;
        }

        public string events
        {
            get { return _events; }
            set { _events = value; }
        }
    }

    public partial class UserPage : Page
    {
        private List<AddUserData> userdummy = new List<AddUserData>();
        private List<Colors> colors = new List<Colors>();
        private List<string> tempEvents = new List<string>();
        private List<UserEvents> userEvents = new List<UserEvents>();
        private List<UserEvents> oldEvents = new List<UserEvents>();
        private API Api = new API();
        Button MultiPurposButton = new Button();
        Button DeleteUserButton = new Button();
        Button RfidUserButton = new Button();
        Button CancelEditButton = new Button();
        int TelefonVal;
        private int PostNrVal;
        private int RowIndex;

        private string UserType;

        private int UserEventPanels = 1;

        public UserPage()
        {
            InitializeComponent();
            RfidButtonFunction();
            MultipurposButtonFunction();
            DeleteButtonFunction();
            userClickFunction();
            CancelEditButtonFunction();
            UserDataGrid.ItemsSource = userdummy;
            ColorComboBox.SelectedIndex = -1;

            HjælperCheckbox.IsChecked = true;

        }

        //Funktion som køre når siden bliver loaded
        void Onload(object sender, RoutedEventArgs e)
        {
            userdummy.Clear();
            _ = GetUsers();
        }

        //Funktion til at hente bruger fra api'en
        private async Task<List<AddUserData>> GetUsers()
        {
            userdummy.Clear();
            //api kald hvor resultatet bliver gemt som et JArray som vi kalder useres
            JArray Users = await Api.getUsers();

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


                // Tilføj users til liste
                userdummy.Add(new AddUserData(username, name, password, mail, color, type, adresse, nr, postnr, telefonnummer, rfidNummer));
            }
            //Refresher vores datagrid
            UserDataGrid.Items.Refresh();

            //Genere de manglende farver som ikke er i brug og sætter den første ledige til ColorComboBox content
            GenerateColors();
            var result = colors.Where(v => userdummy.All(x => v.color != x.Color));
            if (result != null)
            {
                string color = result.First().color;
                ComboBoxItem cbItem = new ComboBoxItem();
                cbItem.Background = (Brush)new BrushConverter().ConvertFrom(color);
                cbItem.Content = color;
                ColorComboBox.Items.Add(cbItem);
                ColorComboBox.SelectedIndex = 0;
                ColorComboBox.Resources.Clear();
                ColorComboBox.Resources.Add(SystemColors.WindowBrushKey, (Brush)new BrushConverter().ConvertFrom(color));
            }

            //Retunere vores liste
            return userdummy;
        }

        //Adder farver til listen....
        private void GenerateColors()
        {
            colors.Clear();
            ColorComboBox.Items.Clear();

            colors.Add(new Colors("#FF0000"));
            colors.Add(new Colors("#0000FF"));
            colors.Add(new Colors("#00FF00"));
            colors.Add(new Colors("#000000"));
            colors.Add(new Colors("#FFFFC0CB"));
            colors.Add(new Colors("#FFFF1493"));
            colors.Add(new Colors("#FFADD8E6"));
            colors.Add(new Colors("#FFA52A2A"));
            colors.Add(new Colors("#FFF4A460"));
            colors.Add(new Colors("#FFFFA500"));
            colors.Add(new Colors("#FFFFD700"));
            colors.Add(new Colors("#FFC0C0C0"));
            colors.Add(new Colors("#FF9ACD32"));
            colors.Add(new Colors("#FF8A2BE2"));
            colors.Add(new Colors("#FF00FFFF"));
            colors.Add(new Colors("#FF856F4F"));
            colors.Add(new Colors("#FF040D68"));
            colors.Add(new Colors("#FFFBFB01"));
            colors.Add(new Colors("#FFB05E26"));
            colors.Add(new Colors("#FF00FB67"));
            colors.Add(new Colors("#FF0097FF"));
            colors.Add(new Colors("#FF3E2711"));
            colors.Add(new Colors("#FF595B34"));
            colors.Add(new Colors("#FF4D7459"));
            colors.Add(new Colors("#FF08471B"));
            colors.Add(new Colors("#FF0C737E"));
            colors.Add(new Colors("#FF49696C"));
            colors.Add(new Colors("#FF7E97EA"));
            colors.Add(new Colors("#FF45027E"));
            colors.Add(new Colors("#FF815A85"));
            colors.Add(new Colors("#FF7A0E4D"));

        }

        //Funktion til at hente events fra api'en
        private async Task<List<UserEvents>> GetEvents()
        {
            //api kald hvor resultatet bliver gemt som et JArray som vi kalder events
            JArray events = await Api.getEvents();

            //For hver item i events henter vi det information vi vil have
            foreach (JObject item in events)
            {
                string name = (string)item.GetValue("EventName");

                // Tilføj events til liste
                userEvents.Add(new UserEvents(name));
            }

            //Så længe i er mindre end længden på vores Events liste så tilføjer vi et nyt item til vores combobox
            for (int i = 0; i < userEvents.Count; i++)
            {
                Label label = new Label();
                label.Content = userEvents[i].events;
                label.Foreground = Brushes.White;
                label.HorizontalContentAlignment = HorizontalAlignment.Right;

                CheckBox checkbox = new CheckBox();
                checkbox.HorizontalAlignment = HorizontalAlignment.Right;
                checkbox.VerticalAlignment = VerticalAlignment.Center;
                checkbox.Margin = new Thickness(15,0,10,0);
                checkbox.Tag = userEvents[i].events; //Ligger et tag på checkboxes med event name
                checkbox.Height = 15.6;

                //click event til hver checkbox der adder event name ind i tempEvents list


                checkbox.Checked += (s, se) =>
                {
                    Console.WriteLine(checkbox.Tag);
                    tempEvents.Add(checkbox.Tag.ToString());
                };

                checkbox.Unchecked += (s, se) =>
                {


                    Console.WriteLine(checkbox.Tag);

                    tempEvents.RemoveAll(n => n == checkbox.Tag.ToString());
                };

StackPanel stackPanel = new StackPanel
                {
                    Name = "childStacks" + i,
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                stackPanel.Children.Add(checkbox);
                stackPanel.Children.Add(label);

                switch (UserEventPanels)
                {
                    case 1:
                        EventStackPanelLabel1.Children.Add(stackPanel);

                        UserEventPanels = 2;
                    break;

                    case 2:
                        EventStackPanelLabel2.Children.Add(stackPanel);

                        UserEventPanels = 3;
                    break;

                    case 3:
                        EventStackPanelLabel3.Children.Add(stackPanel);

                        UserEventPanels = 1;
                    break;
                }


                if(oldEvents.Any())
                {
                    bool done = false;
                    int count = 0;

                    while (count < oldEvents.Count)
                    {
                        if(userEvents[i].events == oldEvents[count].events)
                        {
                            checkbox.IsChecked = true;
                            count += 1;
                        }
                        else
                        {
                            count += 1;
                        }
                    }
                }
                /*EventComboBox.Items.Add(Events[i]);*/
            }
            //Retunere vores liste
            return userEvents;
        }

        private void TestStackPanelAdd()
        {
            //Så længe i er mindre end længden på vores Events liste så tilføjer vi et nyt item til vores combobox
            for (int i = 0; i < 500; i++)
            {
                Label label = new Label();
                label.Content = i;
                label.Foreground = Brushes.White;
                label.HorizontalContentAlignment = HorizontalAlignment.Right;
                label.BorderThickness = new Thickness(1);
                label.BorderBrush = Brushes.White;

                CheckBox checkbox = new CheckBox();
                checkbox.HorizontalAlignment = HorizontalAlignment.Left;
                checkbox.VerticalAlignment = VerticalAlignment.Center;
                checkbox.Margin = new Thickness(3, 6.2, 0, 6.2);
                // checkbox.Tag = userEvents[i].events; //Ligger et tag på checkboxes med event name
                checkbox.Height = 15.6;

                //click event til hver checkbox der adder event name ind i tempEvents list


                StackPanel stackPanel = new StackPanel
                {

                    Name = "childStacks" + i,
                    Orientation = Orientation.Horizontal
                };
                stackPanel.Children.Add(label);
                stackPanel.Children.Add(checkbox);

                switch (UserEventPanels)
                {
                    case 1:
                        EventStackPanelLabel1.Children.Add(stackPanel);

                        UserEventPanels = 2;
                        break;

                    case 2:

                        EventStackPanelLabel2.Children.Add(stackPanel);

                        UserEventPanels = 3;
                        break;

                    case 3:

                        EventStackPanelLabel3.Children.Add(stackPanel);

                        UserEventPanels = 1;
                        break;
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(Pages.p5);
            ClearTxtimputs();
            ClearEvents();

            DeleteUserButton.Visibility = Visibility.Collapsed;
            CancelEditButton.Visibility = Visibility.Collapsed;
            MultiPurposButton.Content = "Tilføj ny bruger";
            MultiPurposButton.Click += AddNewUser_Click;
        }

        private void EditUserCancel_Click(object sender, RoutedEventArgs e)
        {
            ClearTxtimputs();
            ClearEvents();

            AdminCheckbox.IsChecked = false;
            DeleteUserButton.Visibility = Visibility.Collapsed;
            CancelEditButton.Visibility = Visibility.Collapsed;
            MultiPurposButton.Content = "Tilføj ny bruger";
            MultiPurposButton.Click += AddNewUser_Click;
        }

        private void TelefonText_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void NumberValidation(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("^[^0-9\\s]+$");
            e.Handled = re.IsMatch(e.Text);

        }

        //public void addEvents()
        //{
        //    Events.Add("Rosilde");
        //    Events.Add("Skive");
        //    Events.Add("Grøn");
        //    Events.Add("Thyrock");
        //    Events.Add("MidtFyn");
        //    Events.Add("numsejuice");
        //    Events.Add("COBENHELL");
        //}

        public void MultipurposButtonFunction()
        {
            var bc = new BrushConverter();
            MultiPurposButton.Background = (Brush)bc.ConvertFrom("#FF009B88");
            MultiPurposButton.Margin = new Thickness(0, 10, 0, 0);
            MultiPurposButton.Height = 20;
            MultiPurposButton.Foreground = Brushes.White;
            MultiPurposButton.FontSize = 12;

            AddUserStack.Children.Add(MultiPurposButton); //Adding to Stackpanel
        }

        private async Task<List<AddUserData>> deleteUser(string username)
        {
            JArray userDelete = await Api.DeleteUserAsync(username);

            return userdummy;
        }

        public void DeleteButtonFunction()
        {
            var bc = new BrushConverter();
            DeleteUserButton.Background = (Brush)bc.ConvertFrom("#FF009B88");
            DeleteUserButton.Margin = new Thickness(0, 10, 0, 0);
            DeleteUserButton.Height = 20;
            DeleteUserButton.Foreground = Brushes.White;
            DeleteUserButton.FontSize = 12;
            DeleteUserButton.Click += DeleteUser_Click;
            DeleteUserButton.Content = "Slet Bruger";

            AddUserStack.Children.Add(DeleteUserButton); //Adding to Stackpanel
            DeleteUserButton.Visibility = Visibility.Collapsed;
        }

        public void RfidButtonFunction()
        {
            var bc = new BrushConverter();
            RfidUserButton.Background = (Brush)bc.ConvertFrom("#FF009B88");
            RfidUserButton.Margin = new Thickness(0, 10, 0, 0);
            RfidUserButton.Height = 20;
            RfidUserButton.Foreground = Brushes.White;
            RfidUserButton.FontSize = 12;
            RfidUserButton.Click += rfid_Click;
            RfidUserButton.Content = "Tilføj Rfid til bruger";

            AddUserStack.Children.Add(RfidUserButton); //Adding to Stackpanel
            RfidUserButton.Visibility = Visibility.Visible;
        }

        public void CancelEditButtonFunction()
        {
            var bc = new BrushConverter();
            CancelEditButton.Background = (Brush)bc.ConvertFrom("#FF009B88");
            CancelEditButton.Margin = new Thickness(0, 10, 0, 0);
            CancelEditButton.Height = 20;
            CancelEditButton.Foreground = Brushes.White;
            CancelEditButton.FontSize = 12;
            CancelEditButton.Click += EditUserCancel_Click;
            CancelEditButton.Content = "Annuller Ændring";

            AddUserStack.Children.Add(CancelEditButton); //Adding to Stackpanel
            CancelEditButton.Visibility = Visibility.Collapsed;
        }

        private void rfid_Click(object sender, RoutedEventArgs e)
        {
            rfidBorder.Visibility = Visibility.Visible;
            scanText.Focus();
        }


            public void ClearTxtimputs()
        {
            UsernameText.Clear();
            UserName.Clear();
            PasswordText.Clear();
            MailText.Clear();
            AdresseText.Clear();
            NrText.Clear();
            PostNrText.Clear();
            TelefonText.Clear();
            rfidnummer.Clear();
            ColorComboBox.SelectedIndex = -1;

            HjælperCheckbox.IsChecked = true;
        }

        public void ClearEvents()/// Removes Clickevent
        {
            MultiPurposButton.Click -= AddNewUser_Click;
            MultiPurposButton.Click -= EditUser_Click;
        }


        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            userdummy.RemoveAt(RowIndex);/// måske ikke vær
            _ = deleteUser(UsernameText.Text);
            ClearTxtimputs();

            MessageBox.Show("Bruger Slettet!");
            DeleteUserButton.Visibility = Visibility.Collapsed;
            UserDataGrid.Items.Refresh();
            userClickFunction();

            userdummy.Clear();
            _ = GetUsers();

            CancelEditButton.Visibility = Visibility.Collapsed;
        }


        private void UserDatagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ClearEvents();

            try
            {
                var SI = UserDataGrid.SelectedItem as AddUserData;

            _ = GetEventUsers(SI.Username);

                DeleteUserButton.Visibility = Visibility.Visible;
                CancelEditButton.Visibility = Visibility.Visible;

                if (SI.Type == "Admin")
                {
                    AdminCheckbox.IsChecked = true;
                    HjælperCheckbox.IsChecked = false;
                }
                else
                {
                    AdminCheckbox.IsChecked = false;
                    HjælperCheckbox.IsChecked = true;
                }

                GenerateColors();
                var result = colors.Where(v => userdummy.All(x => v.color != x.Color));
                if (result != null)
                {
                    foreach (Colors i in result)
                    {
                        ComboBoxItem cbItem = new ComboBoxItem();
                        cbItem.Background = (Brush)new BrushConverter().ConvertFrom(i.color);
                        cbItem.Content = i.color;
                        ColorComboBox.Items.Add(cbItem);
                    }
                }

                MultiPurposButton.Click += EditUser_Click;
                MultiPurposButton.Content = "Bekræft Ændring";
                UserHeaderLabel.Content = "Bekræft Ændring";
                //AddUserStack.Visibility = Visibility.Visible;
                if (UserDataGrid.SelectedItem == null) return;



                //Sætter ColorComboBox item til brugerens hexcolor
                ComboBoxItem cbItem2 = new ComboBoxItem();
                ColorComboBox.Resources.Clear();
                ColorComboBox.Resources.Add(SystemColors.WindowBrushKey, (Brush)new BrushConverter().ConvertFrom(SI.Color));
                cbItem2.Background = (Brush)new BrushConverter().ConvertFromString(SI.Color);
                cbItem2.Content = SI.Color;
                ColorComboBox.Items.Add(cbItem2);

                RowIndex = UserDataGrid.Items.IndexOf(UserDataGrid.CurrentItem);
                UsernameText.Text = SI.Username;
                UserName.Text     = SI.Name;
                
                //PasswordText.Text = SI.Password;
                MailText.Text = SI.Mail;
                ColorComboBox.Text = SI.Color;
                UserType          = SI.Type;
                AdresseText.Text  = SI.Adresse;
                NrText.Text       = SI.Nr;
                PostNrText.Text   = SI.PostNr.ToString();
                TelefonText.Text  = SI.TelefonNummer.ToString();
                rfidnummer.Text = SI.Rfidnummer;
                /*EventComboBox.Text = SI.Events;*/
                userEditInfo(SI.Username);
            }
            catch (Exception)
            {
            }
        }
        string checkUsername = "";
        private void userEditInfo(string username)
        {
            checkUsername = username;
        }

        private async Task<List<AddUserData>> editUser(string username, string newUsername, string name, string mail, string color, string type, string adresse, string nr, int postnr, int telefonnummer, string rfidNummer, string password = null)
        {
            JArray userEdit = await Api.EditUserAsync(username,
                newUsername,
                name,
                mail,
                color,
                type,
                adresse,
                nr,
                postnr,
                telefonnummer,
                rfidNummer,
                password);
            return userdummy;
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(PostNrText.Text, out PostNrVal);
            int.TryParse(TelefonText.Text, out TelefonVal);

            if (AdminCheckbox.IsChecked == true)
            {
                UserType = "Admin";
            }
            else if (HjælperCheckbox.IsChecked == true)
            {
                UserType = "Hjælper";
            }

            if (UsernameText.Text == "")
            {

                MessageBox.Show("Du skal have både et Brugernavn og en Adgangskode");
                /*_ = editUser(checkUsername, UsernameText.Text, UserName.Text,
                    MailText.Text, ColorComboBox.Text, UserComboBox.Text,
                    AdresseText.Text, NrText.Text, PostNrVal,
                    TelefonVal);*/
            } else if (UsernameText.Text != "")
            {
                _ = editUser(checkUsername, UsernameText.Text, UserName.Text,
                    MailText.Text, ColorComboBox.Text, UserType,
                    AdresseText.Text, NrText.Text, PostNrVal, TelefonVal, rfidnummer.Text,
                    PasswordText.Password.ToString());

                string userName = UsernameText.Text;

                if (tempEvents.Count() != 0)
                {
                    _ = DeleteUserEvent(userName);
                }

                oldEvents.Clear();

                if (tempEvents.Count() != 0)
                {
                    foreach (string i in tempEvents)
                    {
                        _ = createUserEvent(userName, i);
                    }
                }

                tempEvents.Clear();
                userEvents.Clear();

                string password = null;
                userdummy.Add(new AddUserData(UsernameText.Text, UserName.Text, password, MailText.Text, ColorComboBox.Text, UserType,
                AdresseText.Text, NrText.Text, PostNrVal, TelefonVal, rfidnummer.Text));
                ClearTxtimputs();

                userdummy.RemoveAt(RowIndex);
                UserDataGrid.Items.Refresh();

                userdummy.Clear();
                _ = GetUsers();

                ClearEvents();
                userClickFunction();
                DeleteUserButton.Visibility = Visibility.Collapsed;
                CancelEditButton.Visibility = Visibility.Collapsed;
                AdminCheckbox.IsChecked = false;
            }

        }

        private void newUser_Click(object sender, RoutedEventArgs e)
        {
            //MultiPurposButton.Click -= EditUser_Click;
            MultiPurposButton.Click += AddNewUser_Click;
            MultiPurposButton.Content = "Tilføj ny bruger";
            UserHeaderLabel.Content = "Tilføj ny bruger";


            //AddUserStack.Visibility = Visibility.Visible;

        }

        public void userClickFunction()
        {
            //MultiPurposButton.Click -= EditUser_Click;
            MultiPurposButton.Click += AddNewUser_Click;
            MultiPurposButton.Content = "Tilføj ny bruger";
            UserHeaderLabel.Content = "Tilføj ny bruger";


            //AddUserStack.Visibility = Visibility.Visible;
        }

        private async Task<List<UserEvents>> GetEventUsers(string username)
        {
            //api kald hvor resultatet bliver gemt som et JArray som vi kalder useres
            JArray events = await Api.getEventUser(username);

            //For hveritem i Users henter vi det information vi vil have
            if(events != null)
            {
                foreach (JObject item in events)
                {
                    string eventName = (string)item.GetValue("event");


                    // Tilføj users til liste
                    oldEvents.Add(new UserEvents(eventName));
                }
            }



            //Retunere vores liste
            return userEvents;
        }

        private async Task<List<UserEvents>> createUserEvent(string username, string eventName)
        {
            JArray addUser = await Api.createEventUser(username, eventName);

            return userEvents;
        }

        private async Task<List<UserEvents>> DeleteUserEvent(string username)
        {
            JArray addUser = await Api.deleteEventUser(username);

            return userEvents;
        }

        private async Task<List<AddUserData>> createUser(string username, string name, string password, string mail, string color, string type, string adresse, string nr, int postnr, int telefonnummer, string rfidNummer)
        {
            JArray addUser = await Api.CreateUserAsync(username, name, password, mail, color, type, adresse, nr, postnr, telefonnummer, rfidNummer);

            return userdummy;
        }

        private void AdminCheckIsActive(object sender, RoutedEventArgs e)
        {
            if (AdminCheckbox.IsChecked == true)
            {
                HjælperCheckbox.IsChecked = false;
                
            }
        }

        private void HjælperCheckIsActive(object sender, RoutedEventArgs e)
        {
            if (HjælperCheckbox.IsChecked == true)
            {
                AdminCheckbox.IsChecked = false;
            }
        }

        private void AddNewUser_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem color = (ComboBoxItem)ColorComboBox.SelectedItem;
            var result = userdummy.Find(v => v.Username == UsernameText.Text && v.Color == color.Content.ToString());
            if(result != null)
            {
                MessageBox.Show("Brugernavnet eksistere allerede. Vælg et andet.");
            }
            else
            {
                if (PostNrText.Text == ""){ PostNrText.Text = "0"; }
                if (TelefonText.Text == "") { TelefonText.Text = "0"; }

                int.TryParse(PostNrText.Text, out PostNrVal);
                int.TryParse(TelefonText.Text, out TelefonVal);

                //TelefonVal = int.Parse(TelefonText.Text);
                Event();
                if (MailText.Text == "") { MailText.Text = " "; }

                if(UsernameText.Text.Contains("'") || UserName.Text.Contains("'") || PasswordText.Password.ToString().Contains("'") || MailText.Text.Contains("'") || ColorComboBox.Text.Contains("'") || AdresseText.Text.Contains("'") || NrText.Text.Contains("'") || rfidnummer.Text.Contains("'") ||
                    UsernameText.Text.Contains('"') || UserName.Text.Contains('"') || PasswordText.Password.ToString().Contains('"') || MailText.Text.Contains('"') || ColorComboBox.Text.Contains('"') || AdresseText.Text.Contains('"') || NrText.Text.Contains('"') || rfidnummer.Text.Contains('"'))
                {
                    MessageBox.Show("Bruger information kan ikke indeholde ' eller \". Bruger blev ikke tilføjet.");
                }
                else if (UsernameText.Text == "" || UserName.Text == "" || PasswordText.Password.ToString() == "" || ColorComboBox.Text == ""
                    || AdminCheckbox.IsChecked == false && HjælperCheckbox.IsChecked == false)
                     {
                            MessageBox.Show("FEJL: Du skal udfylde alle felter med (*).");
                     }
                else
                {
                    if (AdminCheckbox.IsChecked == true)
                    {
                        UserType = "Admin";
                    }
                    else if (HjælperCheckbox.IsChecked == true)
                    {
                        UserType = "Hjælper";
                    }
                    string password = null;

                    string username = UsernameText.Text;

                    _ = createUser(UsernameText.Text, UserName.Text, PasswordText.Password.ToString(), MailText.Text, ColorComboBox.Text, UserType, AdresseText.Text, NrText.Text, PostNrVal, TelefonVal, rfidnummer.Text);
                    userdummy.Add(new AddUserData(UsernameText.Text, UserName.Text, password, MailText.Text, ColorComboBox.Text, UserType, AdresseText.Text, NrText.Text, PostNrVal, TelefonVal, rfidnummer.Text));
                    ClearTxtimputs();
                    //looper igennem tempEvents for at smide user events ind i databasen
                    //createUserEvent bliver kun kørt en gang ligemeget hvor meget den looper pt og bliver ikke smidt ind i databasen

                    foreach (string i in tempEvents)
                    {
                        _ = createUserEvent(username, i);
                    }


                    userdummy.Clear();
                    _ = GetUsers();
                    UserDataGrid.Items.Refresh();
                    tempEvents.Clear();
                    userEvents.Clear();
                    //AddUserStack.Visibility = Visibility.Collapsed;
                }

                ClearEvents();
                userClickFunction();
                AdminCheckbox.IsChecked = false;
                HjælperCheckbox.IsChecked = true;

            }

        }

        private void Event()
        {
            /*switch(asd.Content.ToString().ToLower())
            {
                userdummy.Add(new AddUserData(UsernameText.Text, UserName.Text, PasswordText.Text, ColorText.Text, UserComboBox.Text));
                ClearTxtimputs();

                UserDataGrid.Items.Refresh();
                //AddUserStack.Visibility = Visibility.Collapsed;
                //AddNewUserButton.Visibility = Visibility.Collapsed;
                MultiPurposButton.Click -= AddNewUser_Click; /// Removes Clickevent
            }

            //Api kaldet for newuser*/

        }

        private void LukGrid_Click(object sender, RoutedEventArgs e)
        {
            userEvents.Clear();
            EventStackPanelLabel1.Children.Clear();
            EventStackPanelLabel2.Children.Clear();
            EventStackPanelLabel3.Children.Clear();
            MultiGrid.Visibility = Visibility.Collapsed;

            UserEventPanels = 1;
        }

        private void TilføjUserEvent_Click(object sender, RoutedEventArgs e)
        {
            userEvents.Clear();
            EventStackPanelLabel1.Children.Clear();
            EventStackPanelLabel2.Children.Clear();
            EventStackPanelLabel3.Children.Clear();
            MultiGrid.Visibility = Visibility.Collapsed;

            UserEventPanels = 1;
        }

        private void VælgEvents_Click(object sender, RoutedEventArgs e)
        {
            _ = GetEvents();

            MultiGrid.Visibility = Visibility.Visible;
            TilføjUserEvent.Focus();
        }

        /* private void EventComboBox_DropDownClosed(object sender, EventArgs e)
         {
             if(EventComboBox.Text != "")
             {
                 string message = "Brugeren er tilføjet til: ";

                 tempEvents.Add(EventComboBox.Text);
                 foreach (var i in tempEvents)
                 {
                     message += i + ", ";
                 }
                 MessageBox.Show(message);
             }
         }*/

         private void UserDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e) {
             if (e.PropertyName == "Password")
            {
                e.Cancel = true;
            }
         }

        private ConnectionInterface connectionInterface;
        private string scannedID = null;
        private bool scannerOnOff = false;
        private void annulerRfid_Click(object sender, RoutedEventArgs e)
        {
            if (scannerOnOff == true)
            {
                connectionInterface.disconnect();
                connectionInterface.Dispose();
                rfidBorder.Visibility = Visibility.Collapsed;

                scannerOnOff = false;
            }
            else
            {
                //MessageBox.Show("Crash prevented");
                rfidBorder.Visibility = Visibility.Collapsed;
            }

            // Når man forlader vinduet og slukker scanneren skal muligheden for at ændre scanner IP og scanner styrke igen
            ChangeScannerIPKnap.Visibility = Visibility.Visible;
            SetPowerKnap.Visibility = Visibility.Visible;
        }

        private void gemRfid_Click(object sender, RoutedEventArgs e)
        {
            // Når man forlader vinduet og slukker scanneren skal muligheden for at ændre scanner IP og scanner styrke igen
            ChangeScannerIPKnap.Visibility = Visibility.Visible;
            SetPowerKnap.Visibility = Visibility.Visible;

            if (scannedID == null)
            {
                _ = MessageBox.Show("Intet ID scannet");
                if (connectionInterface == null) { return; }
                if (scannerOnOff == true)
                {
                    connectionInterface.disconnect();
                    connectionInterface.Dispose();

                    scannerOnOff = false;
                }
            }
            else
            {
                //smid scannedID til api eller hvad end det skal bruges til her
                rfidnummer.Text = scannedID;
                if (scannerOnOff == true)
                {
                    connectionInterface.disconnect();
                    connectionInterface.Dispose();
                    rfidBorder.Visibility = Visibility.Collapsed;

                    scannerOnOff = false;
                }
                else
                {
                    //MessageBox.Show("Crash prevented");
                    rfidBorder.Visibility = Visibility.Collapsed;
                }
                rfidBorder.Visibility = Visibility.Collapsed;
            }
        }
        // scanPowerValue = dBm * 10 (Scanner Styrke)
        // https://en.wikipedia.org/wiki/DBm
        // Må have en maks værdi på 300 (30 dBm) og en mindste værdi på 0, (unsigned integer) men det ville ikke give mening at gå under 10 da det bliver divideret alligevel
        // så det mindste vil altid værre 1 dBm
        uint scanPowerValue = 200;
        // IP Til scanner, så den nemt kan ændres af brugeren skulle det blive nødvendigt
        string scannerIP = "192.168.2.60";

        void ScanKnapAsync()
        {
            // Dispatcher så vi kan ænder GUI elementer selvom vi køre op en anden tråd
            // Skjuler knapperne Ændre IP og Ændre styrke, og starer loading skærm
            Dispatcher.Invoke(() =>
            {
            ChangeScannerIPKnap.Visibility = Visibility.Collapsed;
            SetPowerKnap.Visibility = Visibility.Collapsed;
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
                Dispatcher.Invoke(() =>
                {
                StarterScannerBorder.Visibility = Visibility.Collapsed;
                ChangeScannerIPKnap.Visibility = Visibility.Visible;
                SetPowerKnap.Visibility = Visibility.Visible;
                });
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
                //MessageBox.Show("klar");
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

        private void scanButton_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
                ScanKnapAsync() // Køre hele skan funktionen async
            );
        }
        private void ConnectionInterface_OnTagRead(object sender, TagReadEventArgs e)
        {
            scannedID = e.tagID;
            this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                scanText.Text = scannedID;
            }));
            // Beep ved Skan
            CSLibrary.Tools.Sound.Beep(700, 200);
        }
        // UI
        private void userAntennaPower_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Hvis man ændre slideren ændre man værdien i scanPowerValue variablen, som er den der styre antenne styrken
            userAntennaPowerLabel.Content = userAntennaPower.Value.ToString();
            scanPowerValue = Convert.ToUInt32(userAntennaPower.Value);
        }

        private void SetPowerKnap_Click(object sender, RoutedEventArgs e)
        {
            changeStyrkeBorder.Visibility = Visibility.Visible;
            SkjulStyrkeBorder.Focus();
        }

        private void SkjulStyrkeBorder_Click(object sender, RoutedEventArgs e)
        {
            changeStyrkeBorder.Visibility = Visibility.Collapsed;
            scanText.Focus();
        }

        private void GemChangedIP_Click(object sender, RoutedEventArgs e)
        {
            scannerIP = scannerIPTextBox.Text;
            changeScannerIP.Visibility = Visibility.Collapsed;
        }

        private void SkjulIPChanger_Click(object sender, RoutedEventArgs e)
        {
            changeScannerIP.Visibility = Visibility.Collapsed;
            scanText.Focus();
        }

        private void ChangeScannerIPKnap_Click(object sender, RoutedEventArgs e)
        {
            changeScannerIP.Visibility = Visibility.Visible;
            scannerIPTextBox.Focus();
        }

        private void TilbageKnapEvent_MouseDown(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(Pages.p5);
            ClearTxtimputs();
            ClearEvents();

            DeleteUserButton.Visibility = Visibility.Collapsed;
            CancelEditButton.Visibility = Visibility.Collapsed;
            MultiPurposButton.Content = "Tilføj ny bruger";
            MultiPurposButton.Click += AddNewUser_Click;
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
    }
}