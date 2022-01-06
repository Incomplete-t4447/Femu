using CsvHelper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
using Excel = Microsoft.Office.Interop.Excel;
using System.Text.RegularExpressions;

namespace FELM
{
    /// <summary>
    /// Interaction logic for ExportPage.xaml
    /// </summary>
    public partial class ExportPage : Page
    {
        API Api = new API();
        public List<object> eventList = new List<object>();
        public List<object> exportList = new List<object>();
        string currentEventName;
        public ExportPage()
        {
            InitializeComponent();
            exportDataGrid.ItemsSource = exportList;
        }

        void Onload(object sender, RoutedEventArgs e)
        {
            _ = GetEventsAsync();
        }

        //Henter og indsætter Event navn og id i en Dropdownliste på Exportsiden
        private async Task<List<object>> GetEventsAsync()
        {
            eventList.Clear();
            CmbBox.Items.Clear();
            JArray EventResult = await Api.AllEventsExportQueryAsync();
            foreach (JObject item in EventResult)
            {
                string eventName = (string)item.GetValue("EventName");
                string eventId = (string)item.GetValue("EventId");
                eventList.Add(eventName + "-" + eventId);
            }
            eventList.Reverse();
            CmbBox.Items.Insert(0, "Vælg event");
            CmbBox.SelectedIndex = 0;
            foreach (var item in eventList)//Foreach som tilføjer eventName og eventId til en dropdown på export siden
            {
                CmbBox.Items.Add(item);
            }
            return eventList;
        }


        //Henter data som passer på Id'et fra det valgte event i dropdownlisten og viser det i et DataGridView 
        async void OnDropDownSelect(object sender, EventArgs e)
        {
            exportList.Clear();
            Regex re = new Regex("([-])([^-]*)$");
            string CmbBoxResult = CmbBox.Text;
            Match idString = re.Match(CmbBoxResult);
            if (idString.Success)
            {
                int id = int.Parse(idString.Groups[2].Value);
                JArray loadResult = await Api.EventAndVareQueryAsync(id); //JArray som indeholder alt der passer på idInt i databasen

                if (loadResult != null)
                {

                    foreach (JObject i in loadResult)
                    {
                        string exportStartDato = (string)i.GetValue("EventStartDato");
                        string exportSlutDato = (string)i.GetValue("EventSlutDato");
                        string exportVarenummer = (string)i.GetValue("varenummer");
                        string exportAntal = (string)i.GetValue("antal");
                        EventData exportData = new EventData(exportStartDato, exportSlutDato, exportVarenummer, exportAntal);
                        exportList.Add(exportData);
                    }

                    exportDataGrid.ItemsSource = exportList;
                    exportDataGrid.Items.Refresh();

                }
                else
                {
                    MessageBox.Show("Der er ikke noget indhold at vise.");
                }
            }
            re = new Regex(".+?(?=-)");
            Match name = re.Match(CmbBoxResult);
            currentEventName = name.Value;

        }
        //Exportere dataen fra DataGridViewet på siden
        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            string path = currentEventName + "_csv.csv";
            path = path.Replace(" ", "_");
            Excel.Application xlexcel;
            object misValue = System.Reflection.Missing.Value;
            xlexcel = new Excel.Application();
            xlexcel.Visible = true;

            xlexcel.Workbooks.Add();
            Excel._Worksheet workSheet = (Excel.Worksheet)xlexcel.ActiveSheet;

            //Laver Excell headers
            workSheet.Cells[1, "A"] = "fromDate";
            workSheet.Cells[1, "B"] = "toDate";
            workSheet.Cells[1, "C"] = "number";
            workSheet.Cells[1, "D"] = "count";
            workSheet.Columns[1].AutoFit();
            workSheet.Columns[2].AutoFit();
            workSheet.Columns[3].AutoFit();
            workSheet.Columns[4].AutoFit();
            ((Excel.Range)workSheet.Columns[1]).AutoFit();
            ((Excel.Range)workSheet.Columns[2]).AutoFit();
            ((Excel.Range)workSheet.Columns[3]).AutoFit();
            ((Excel.Range)workSheet.Columns[4]).AutoFit();

            //Fylder cells ud med data fra grid
            int i = 2;
            foreach (EventData rows in exportDataGrid.ItemsSource)
            {
                workSheet.Cells[i, "A"] = rows.StartDato;
                workSheet.Cells[i, "B"] = rows.SlutDato;
                workSheet.Cells[i, "C"] = rows.Varenummer;
                workSheet.Cells[i, "D"] = rows.Antal;
                i++;
            }
            workSheet.SaveAs(path, Excel.XlFileFormat.xlCSV);
        }
        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(Pages.p5);
        }

        private void CmbBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private async void LoadBtn_Click(object sender, RoutedEventArgs e)
        {
            string CmbBoxResult = CmbBox.Text;
            string idString = CmbBoxResult.Substring(CmbBoxResult.IndexOf("-") + 1); //Splitter CmbBoxResult op og fjerner alt tekst foran id'et
            int idInt = int.Parse(idString);
            JArray loadResult = await Api.EventAndVareQueryAsync(idInt); //JArray som indeholder alt der passer på idInt i databasen
            foreach (JObject i in loadResult)
            {
                string exportStartDato = (string)i.GetValue("EventStartDato");
                string exportSlutDato = (string)i.GetValue("EventSlutDato");
                string exportVarenummer = (string)i.GetValue("varenummer");
                string exportAntal = (string)i.GetValue("antal");
                ExportData exportData = new ExportData(exportStartDato, exportSlutDato, exportVarenummer, exportAntal);
                exportList.Add(exportData);
            }
            exportDataGrid.ItemsSource = exportList;
            exportDataGrid.Items.Refresh();
        }

    }
}