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
    /// Interaction logic for OverlapRFID.xaml
    /// </summary>
    public partial class OverlapRFID : Window
    {
        public string vareNummer;
        public OverlapRFID()
        {
            InitializeComponent();
            AcceptButton.Visibility = Visibility.Hidden;
        }

        private void VareTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AcceptButton.Visibility = Visibility.Visible;
        }
        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            vareNummer = VareTextBox.Text;
            DialogResult = true;
            Close();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}