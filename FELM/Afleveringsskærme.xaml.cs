using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Label = System.Windows.Controls.Label;
using MessageBox = System.Windows.MessageBox;

namespace FELM
{
    /// <summary>
    /// Interaction logic for Afleveringsskærme.xaml
    /// </summary>
    public partial class Afleveringsskærme : Window
    {
        List<AfleveringsskærmItems> SkærmList = new List<AfleveringsskærmItems>();

        SolidColorBrush userColor = new SolidColorBrush();

        public bool YesNo = false;

        public bool AddItemBool = false;

       

        public Afleveringsskærme()
        {
            InitializeComponent();
  
        }

        private void NewItemClick(object sender, EventArgs e)
        {
            if (YesNo == false)
            {
                NewItemPopUp.Visibility = Visibility.Visible;
                YesNo = true;
            }
            else if (YesNo == true)
            {
                NewItemPopUp.Visibility = Visibility.Collapsed;
                YesNo = false;
            }
        }

        private void AddItem(object sender, EventArgs e)
        {
            try
            {
                userColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#" + TextBoxFarve.Text);
                AddItemBool = true;
            }
            catch
            {
                MessageBox.Show("Color code was misstyped");
                AddItemBool = false;
            }

            if (String.IsNullOrEmpty(TextBoxNavn.Text) == false && String.IsNullOrEmpty(TextBoxHylde.Text) == false && String.IsNullOrEmpty(TextBoxKrog.Text) == false && String.IsNullOrEmpty(TextBoxFarve.Text) == false && AddItemBool == true)
            {
                Label itemNavn = new Label();
                itemNavn.Content = TextBoxNavn.Text;
                itemNavn.Height = 55;
                itemNavn.Foreground = Brushes.White;
                itemNavn.Background = userColor;
                itemNavn.BorderBrush = Brushes.White;
                itemNavn.FontSize = 30;
                itemNavn.HorizontalContentAlignment = HorizontalAlignment.Center;
                itemNavn.BorderThickness = new Thickness(2.0);

                Label itemHylde = new Label();
                itemHylde.Content = TextBoxHylde.Text;
                itemHylde.Height = 55;
                itemHylde.Foreground = Brushes.White;
                itemHylde.Background = userColor;
                itemHylde.BorderBrush = Brushes.White;
                itemHylde.FontSize = 30;
                itemHylde.HorizontalContentAlignment = HorizontalAlignment.Center;
                itemHylde.BorderThickness = new Thickness(2.0);

                Label itemKrog = new Label();
                itemKrog.Content = TextBoxKrog.Text;
                itemKrog.Height = 55;
                itemKrog.Foreground = Brushes.White;
                itemKrog.Background = userColor;
                itemKrog.BorderBrush = Brushes.White;
                itemKrog.FontSize = 30;
                itemKrog.HorizontalContentAlignment = HorizontalAlignment.Center;
                itemKrog.BorderThickness = new Thickness(2.0);

                UserStackpanelNavn.Children.Insert(0, itemNavn);
                UserStackpanelHylde.Children.Insert(0, itemHylde);
                UserStackpanelKrog.Children.Insert(0, itemKrog);

                TextBoxNavn.Clear();
                TextBoxHylde.Clear();
                TextBoxKrog.Clear();
                TextBoxFarve.Clear();

                NewItemPopUp.Visibility = Visibility.Collapsed;
                YesNo = false;
                AddItemBool = false;
                SkærmList.Add(new AfleveringsskærmItems(TextBoxNavn.Text, TextBoxHylde.Text, TextBoxKrog.Text, TextBoxFarve.Text));

                if (UserStackpanelNavn.Children.Count == 11 && UserStackpanelHylde.Children.Count == 11 && UserStackpanelKrog.Children.Count == 11)
                {
                    UserStackpanelNavn.Children.RemoveAt(10);
                    UserStackpanelHylde.Children.RemoveAt(10);
                    UserStackpanelKrog.Children.RemoveAt(10);
                }
            }
            else if (String.IsNullOrEmpty(TextBoxNavn.Text) == true || String.IsNullOrEmpty(TextBoxHylde.Text) == true || String.IsNullOrEmpty(TextBoxKrog.Text) == true || String.IsNullOrEmpty(TextBoxFarve.Text) == true)
            {
                MessageBox.Show("Et eller flere felter er ikke udfyldt.");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }
    }
}