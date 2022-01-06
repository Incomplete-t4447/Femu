using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
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

namespace FELM
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        API Api = new API();
        public LoginPage()
        {
            InitializeComponent();
            
        }
        private async void Login_Button(object sender, RoutedEventArgs e)
        {
            String status = "";
            String type = "";
            string[] result = { "false" };
            JObject stringResult = await Api.LoginQueryAsync(LoginTextBox.Text, PasswordTextBox.Password.ToString());

            



            if(Globalvar.NoNet == false)
            {
                status = (string)stringResult.First.First;
                type = (string)stringResult["type"];
            }
            
            try 
            {
                // result = stringResult[0].First.First;
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
            

            if (status == "true")
            {
                if(type == "Admin")
                {
                    LoginTextBox.Text = "";
                    NavigationService.Navigate(Pages.p5);
                } else
                {
                    MessageBox.Show("Den bruger du prøver at logge ind med har ikke adgang til dette program");
                }
            } else
            {
                MessageBox.Show("Forkert brugenavn eller kode");
            }

            //NavigationService.Navigate(Pages.p3);
        }
        // Tobias navngav den her, ikke Michael :) vvv
        private void Button_ClickTryhard(object sender, RoutedEventArgs e)
        {
            LoginTextBox.Text = "";
            NavigationService.Navigate(Pages.p3);
        }
        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoginTextBox.Text = "";
            NavigationService.Navigate(Pages.p5);
        }

        private void Border_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login_Button(sender, e);
            }
        }

        private void Border_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PasswordTextBox.Focus();
            }
        }
    }
   
}
