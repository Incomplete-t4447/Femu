using System;
using System.Collections.Generic;
using System.Drawing;
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
using QRCoder;
using System.IO;
using QRCoder.Unity;
using System.Threading;
using System.Security.Cryptography;
using System.Diagnostics;
using System.ComponentModel;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Globalization;
using Microsoft.Win32;
using Paragraph = iTextSharp.text.Paragraph;
using iTextSharp.text.pdf.draw;
using Brush = System.Windows.Media.Brush;

namespace FELM
{
    /// <summary>
    /// Interaction logic for QrPage.xaml
    /// </summary>

    public partial class QrPage : Page
    {
        public static bool runNow = false;

        // Gem QR Koden som fil
        public bool ByteArrayToFile(string fileName, byte[] byteArray)
        {
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return false;
            }
        }

        public QrPage()
        {
            InitializeComponent();
            slValue.Value = numberOfColums;
        }

        // Generer en Random String
        static string RandomString(int length)
        {
            // Definer valide karaktere
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (length-- > 0)
                {
                    // Konverter de tilfældigt generede tal til bogstaver
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(valid[(int)(num % (uint)valid.Length)]);
                }
            }

            return res.ToString();
        }

        // Omdan den tilfældigt generede String ovenfor til Base64
        public static string GenerateNewToken()
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(RandomString(16));
            return System.Convert.ToBase64String(plainTextBytes);
        }

        // Forbind til API
        API QRApi = new API();
        public async Task<JObject> qrKode(string qrkode)
        {
            JObject stuff = await QRApi.qrCode(qrkode);
            return stuff;
        }

        void genQRText(int i, string input)
        {

            // ------------------------ Put QR kodens værdi under QR koden --------------------

            Process ExternalProcess = new Process(); // Definer en ny processe
            ExternalProcess.StartInfo.FileName = "imagemagick\\convert.exe"; // Den skal køre imagemagick
                                                                             // Argumenter til Imagemagick
            ExternalProcess.StartInfo.Arguments = " \"" + DateTime.UtcNow.ToString("yyyy-dd-MM") + " - QR\\qr" + i + ".png\" -gravity South -pointsize 30 -annotate +0+15 \"" + input + "\" \"" + DateTime.UtcNow.ToString("yyyy-dd-MM") + " - QR\\qr" + i + ".png\""; //temp" + i+".png";
            Console.WriteLine(ExternalProcess.StartInfo.Arguments);
            // Vinduet skal være skjult, så det hele køre i baggrunden
            ExternalProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            ExternalProcess.Start(); // Start
            ExternalProcess.WaitForExit();
        }
        void genQRLogo(int i)
        {

            Process LogoProcess = new Process(); // Definer en ny processe
            LogoProcess.StartInfo.FileName = "imagemagick\\convert.exe"; // Den skal køre imagemagick
                                                                         // Argumenter til Imagemagick
            LogoProcess.StartInfo.Arguments = " \"" + DateTime.UtcNow.ToString("yyyy-dd-MM") + " - QR\\qr" + i + ".png\" -gravity North \"imagemagick\\logo_festival-el.png\" -composite \"" + DateTime.UtcNow.ToString("yyyy-dd-MM") + " - QR\\qr" + i + ".png\""; //temp" + i+".png";
            Console.WriteLine(LogoProcess.StartInfo.Arguments);
            // Vinduet skal være skjult, så det hele køre i baggrunden
            LogoProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            LogoProcess.Start(); // Start
            LogoProcess.WaitForExit();
        }

        // ----------------- Sætter Selve generationen igang

        uint numberOfCodesNumber = 0;
        bool noRound = true;

        async void GenerateQR()
        {
            var watch = new System.Diagnostics.Stopwatch(); // Mål tiden brugt på at generere koder

            watch.Start();

            try
            {

                // Sæt Progress barens max værdi til antallet af koder der skal genereres
                Dispatcher.Invoke(() =>
                {
                    qrProgress.Maximum = numberOfCodesNumber;
                });

                // Lav mappe, med navnet dagens dato som QR filerne kan smides i
                Directory.CreateDirectory(DateTime.UtcNow.ToString("yyyy-dd-MM") + " - QR");

                // ------------------------ Progress Bar --------------------

                for (int i = 1; i <= numberOfCodesNumber; i++)
                {
                    // int j = 1; // variablen j bliver brugt til at putte Festival El Logoet over QR koden, og holdes med værdien under i, så de ikke arbejder på dem samme tid
                    //if (i != 1)
                    int j = i - 1;

                    Dispatcher.Invoke(() =>
                    {
                        // Increment Progress baren med 1
                        qrProgress.Value += 1;
                    });


                    // ------------------------ Generer QR string og check om den findes --------------------

                    string input = "";

                    bool qrExist = false;
                    // Check om den genererede QR kode allerede findes, hvis den gør så skrot den og lav en ny
                    while (qrExist == false)
                    {
                        // Generer Koden
                        input = GenerateNewToken();
                        // Afskær alle karaktere efter det 8.
                        input = input.Substring(0, 8);
                        // Kontakt Databasen og se om den allerede findes
                        JObject result = await qrKode(input);
                        JObject test = JObject.Parse(result.ToString());

                        if (test["status"].ToString() == "Not exist")
                        {
                            qrExist = true;
                        }
                        else
                        {
                            // Hvis QR koden allerede var blevet lavet ændre en bool så loopet køre igen
                            qrExist = false;
                            // Vis et popup vindue der siger at koden allerede existerede
                            MessageBox.Show("Samme kode");
                        }

                    }

                    // ------------------------ Den token der blev genereret fandtes ikke og vi kan nu rykke videre, og oprette selve QR koden --------------------

                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    // Lav QR kodens værdi om til en QR kode
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(input, QRCodeGenerator.ECCLevel.Q);
                    PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
                    // Put QR koden ind i et byte array
                    byte[] qrCodeAsPngByteArr = qrCode.GetGraphic(20);

                    using (var ms = new MemoryStream(qrCodeAsPngByteArr))
                    {
                        var qrCodeImage = new Bitmap(ms);
                    }

                    // Gem QR koden i en fil
                    ByteArrayToFile(DateTime.UtcNow.ToString("yyyy-dd-MM") + " - QR\\qr" + i.ToString() + ".png", qrCodeAsPngByteArr);



                    //------------------------ PUDT LOGO OG TEKST PÅ QR KODERNE, KØRES PARRALELT! ---------------------
                    Task t1 = Task.Run(() =>
                    genQRText(i, input)
                    );

                    Task t2 = Task.Run(() =>
                        genQRLogo(j)
                    );

                    await Task.WhenAll(t1, t2);

                    if (i == numberOfCodesNumber) // Ved den sidste kode, så kør logo funktionen så den ikke misser den sidste
                    {
                        await Task.Run(() =>
                            genQRLogo(i)
                            );
                    }
                    //--------------------------------------------------------------------------------------------------

                }

                isGenerating = false;


                // Ændre labelsne tilbage til deres originale værdi
                Dispatcher.Invoke(() =>
                {
                    //createPDF.Visibility = Visibility.Visible;
                    donePopupGrid.Visibility = Visibility.Visible;
                    progessLabelQR.Content = "Inaktiv";
                    progessLabelQR.Foreground = System.Windows.Media.Brushes.Red;
                    qrProgress.Visibility = Visibility.Collapsed;
                    qrProgress.Value = 0;
                    SliderTextBox.IsReadOnly = false;
                    numberOfCodes.IsReadOnly = false;
                });
                watch.Stop();

                Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");



            }
            catch // Hvis der har værret en fejl
            {
                // Vis fejl meddelelse
                MessageBox.Show("Der skete en fejl!");
                isGenerating = false;


                // Og reset labelsne og progress baren
                Dispatcher.Invoke(() =>
                {
                    progessLabelQR.Content = "Inaktiv";
                    progessLabelQR.Foreground = System.Windows.Media.Brushes.Red;
                    qrProgress.Visibility = Visibility.Collapsed;
                    qrProgress.Value = 0;
                    SliderTextBox.IsReadOnly = false;
                    numberOfCodes.IsReadOnly = false;
                });
            }
        }

        // ------------------------ Funktion til at smide QR koderne ind i en PDF fil --------------------

        // Hvor Mange QR Koder der skal værre på en række, at ændre denne variabel ændre også default behavior for slideren
        int numberOfColums = 25;
        void CreatePDFQR()
        {
            // Dialog boks, der spørger hvor PDF filen skal gemmes
            SaveFileDialog saveFile = new SaveFileDialog()
            {
                Title = "Gem din Fil",
                Filter = "Adobe PDF(*.pdf) | *.pdf",
                FileName = "QR Koder"
            };

            // lave indehold til pdf
            if (saveFile.ShowDialog() == true)
            {
                // Definer side størrelse
                var pgSize = new iTextSharp.text.Rectangle(4104, 4000);
                // Lav nyt dokument med pgSize som side størrelse
                var doc1 = new Document(pgSize, -400, -400, 50, 50);

                // Brug den placering valgt i dialog boksen, som filens placering
                PdfWriter.GetInstance(doc1, new FileStream(saveFile.FileName, FileMode.Create));

                doc1.Open();

                try
                {
                    // Ny tabel og celle
                    PdfPTable table = new PdfPTable(numberOfColums); // Hvor mange QR koder der skal værre på 1 række
                    PdfPCell cell = new PdfPCell();

                    // Tæl hvor mange filer der er i mappen med QR koder, så vi ved hvor mange der skal i PDF'en

                    for (int i = 1; i <= numberOfCodesNumber; i++)
                    {
                        // Opret en ny celle
                        cell = new PdfPCell();

                        // Hent qr koden og put dem i variablen addLogo
                        iTextSharp.text.Image addLogo = default(iTextSharp.text.Image);
                        addLogo = iTextSharp.text.Image.GetInstance(DateTime.UtcNow.ToString("yyyy-dd-MM") + " - QR\\qr" + i + ".png");

                        // Tilføj QR koden til cellen
                        cell.AddElement(addLogo);

                        // Tilføj Cellen til Tabellen
                        table.AddCell(cell);

                        uint k = numberOfCodesNumber;
                        if (noRound == true && (numberOfCodesNumber % numberOfColums) != 0 && i == numberOfCodesNumber)
                        {
                            while ((k % 20) != 0)
                            {
                                // Opret en ny celle
                                cell = new PdfPCell();
                                addLogo = iTextSharp.text.Image.GetInstance("whitespace1.png");
                                cell.AddElement(addLogo);
                                cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                                table.AddCell(cell);
                                k++;
                            }
                        }


                    }
                    // Tilføj tabellen til Dokumentet
                    doc1.Add(table);

                    // Færdig med at arbejde i dokumentet
                    doc1.Close();

                    Dispatcher.Invoke(() =>
                    {
                        // Gør Gem som PDF knappen usynlig igen
                        //createPDF.Visibility = Visibility.Collapsed;
                        donePopupGrid.Visibility = Visibility.Collapsed;
                        noRound = true;

                    });

                    //MessageBox.Show("Success!");

                }
                catch
                {
                    // Hvis der sker en fejl
                    MessageBox.Show("Fil ikke fundet!");
                    doc1.Close();
                }
            }
            else
            {
                Console.WriteLine("Canceled");
            }
        }

        bool isGenerating = false; // Sørger for at man ikke kan starte processen hvis den allerede køre

        public void createQR_Click_1(object sender, RoutedEventArgs e)
        {
            if (donePopupGrid.Visibility == Visibility.Visible) { MessageBox.Show("Du kan ikke starte en ny process før du lukker popup vinduet"); return; }
            if (isGenerating == true) { MessageBox.Show("Programmet køre allerede!"); return; }

            // Prøv at hente antallet af QR koder der skal genereres fra tekst boksen
            try { numberOfCodesNumber = Convert.ToUInt32(numberOfCodes.Text); }
            // Hvis det ikke lykkedes pågrund af en ikke gyldig værdi blev givet, vis en fejl meddelelse
            catch { MessageBox.Show("Ugyldig Værdi!\n Afslutter!"); return; }

            isGenerating = true;
            SliderTextBox.IsReadOnly = true;
            numberOfCodes.IsReadOnly = true;

            // Ændre Labelsne til at være running
            progessLabelQR.Content = "Kører!";
            progessLabelQR.Foreground = System.Windows.Media.Brushes.Green;

            // Initialize Progress Baren
            qrProgress.Visibility = Visibility.Visible;
            qrProgress.Value = 0;

            if (numberOfCodesNumber % numberOfColums != 0)
            {
                string sMessageBoxText = "Der kan værre " + numberOfColums.ToString() + " QR koder per række, at oprette noget det ikke går op i vil kun medføre papirspild.\nØnsker du at runde op til nærmeste " + numberOfColums.ToString() + "?";
                string sCaption = "Advarsel";
                MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                MessageBoxImage icnMessageBox = MessageBoxImage.Warning;
                MessageBoxResult rsltMessageBox = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

                switch (rsltMessageBox)
                {
                    case MessageBoxResult.Yes:

                        while ((numberOfCodesNumber % numberOfColums) != 0)
                        {
                            numberOfCodesNumber++;
                        }
                        noRound = false;
                        break;

                    case MessageBoxResult.No:
                        Console.WriteLine("Nope");
                        noRound = true;
                        break;

                    case MessageBoxResult.Cancel:
                        Console.WriteLine("Cancel");
                        break;
                }
            }

            Task.Run(() =>
            GenerateQR()
            );
        }

        private void goBack_Click(object sender, RoutedEventArgs e)
        {
            if (donePopupGrid.Visibility == Visibility.Visible) { return; }
            // Gå tilbage til hovedmenuen
            NavigationService.Navigate(Pages.p5);
        }

        private void popupGemPDF_Click(object sender, RoutedEventArgs e)
        {
            uint numberOfCodesNumber = 0;
            try { numberOfCodesNumber = Convert.ToUInt32(numberOfCodes.Text); }
            catch { MessageBox.Show("Invalid Number"); }

            // Kør Funktionen Async
            Task.Run(() =>
                CreatePDFQR()
            );
        }
        private void gemIkkePDF_Click(object sender, RoutedEventArgs e)
        { donePopupGrid.Visibility = Visibility.Collapsed; }

        private void slValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isGenerating == true) { slValue.Value = numberOfColums; return; }
            if (slValue.Value == 0)
            {
                MessageBox.Show("Du skal have en værdi højere end 0");
                slValue.Value = 5;
            }
            numberOfColums = Convert.ToInt32(slValue.Value);
        }

        private void numberOfCodes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                createQR_Click_1(sender, e);
            }
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
    }
}