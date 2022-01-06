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

namespace FELM
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(Pages.p3);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(Pages.p4);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(Pages.p2);
        }

        private void Button_Click3(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(Pages.p6);
        }

        private void qrSideButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(Pages.p8);
        }
        
        // Gå tilbage til loginsiden
        private void logOut_Popup(object sender, RoutedEventArgs e)
        {
            LogOutPopup.Visibility = Visibility.Visible;
        }

        private void logOut_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(Pages.p1);
            LogOutPopup.Visibility = Visibility.Collapsed;
        }

        private void noLogOut_Click(object sender, RoutedEventArgs e)
        {
            LogOutPopup.Visibility = Visibility.Collapsed;
        }

        private void TilbageKnapEvent_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LogOutPopup.Visibility = Visibility.Visible;
        }

        private void TilbageKnapEvent_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            logOutButton.Foreground = (Brush)bc.ConvertFrom("#CCC");
        }

        private void TilbageKnapEvent_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            logOutButton.Foreground = (Brush)bc.ConvertFrom("#FFF");
        }
    }
}