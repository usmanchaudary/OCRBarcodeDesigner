using OCRBarcodeDesigner.PrintTemplates;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing.Mobile;

namespace OCRBarcodeDesigner
{
    public partial class MainPage : ContentPage
    {
        public string PhotoPath { get; private set; }

        public MainPage()
        {
            InitializeComponent();
        }

        private async void BarcodeGenerateBtn_Clicked(object sender, EventArgs e)
        {
            var scanner = new MobileBarcodeScanner();
            scanner.TopText = "Hold the camera up to the barcode\nAbout 6 inches away";
            scanner.BottomText = "Wait for the barcode to automatically scan!";

            //This will start scanning
            ZXing.Result result = await scanner.Scan();

            //Show the result returned.
            HandleResult(result);

            var printTemplate = new ListPrintTemplate();

            // Set the model property (ViewModel is a custom property within containing view - FYI)
            var invoiceNumber = Guid.NewGuid().ToString().Substring(0, 6);

            // Generate the HTML
            var htmlString = printTemplate.GenerateString();

            // Create a source for the webview
            var htmlSource = new HtmlWebViewSource();
            htmlSource.Html = htmlString;

            // Create and populate the Xamarin.Forms.WebView
            var browser = new WebView();

            browser.Source = htmlSource;

            _ = await browser.EvaluateJavaScriptAsync("updatetextonwebview()");

            var printService = DependencyService.Get<IPrintService>();
            printService.Print(browser);
        }
        void HandleResult(ZXing.Result result)
        {
            var msg = "No Barcode!";
            if (result != null)
            {
                msg = "Barcode: " + result.Text + " (" + result.BarcodeFormat + ")";
            }

            DisplayAlert("", msg, "Ok");
        }

        private async void OCR_Test_Clicked(object sender, EventArgs e)
        {
            TakePhotoAsync();

        }
        async Task TakePhotoAsync()
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                await LoadPhotoAsync(photo);
                Console.WriteLine($"CapturePhotoAsync COMPLETED: {PhotoPath}");
                using (WebClient wc = new WebClient())
                {
                    wc.Headers["apikey"] = "a4028b8a1188957";
                    var resp = wc.UploadFile("https://api.ocr.space/parse/image", PhotoPath);
                    var str = System.Text.Encoding.Default.GetString(resp);
                    var text = OcrResult.FromJson(str).ParsedResults.First().ParsedText;
                    Console.WriteLine(text);


                   await DisplayAlert("", text, "Ok");
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature is not supported on the device
            }
            catch (PermissionException pEx)
            {
                // Permissions not granted
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CapturePhotoAsync THREW: {ex.Message}");
            }
        }
        async Task LoadPhotoAsync(FileResult photo)
        {
            // canceled
            if (photo == null)
            {
                PhotoPath = null;
                return;
            }
            // save the file into local storage
            var newFile = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
            using (var stream = await photo.OpenReadAsync())
            using (var newStream = File.OpenWrite(newFile))
                await stream.CopyToAsync(newStream);

            PhotoPath = newFile;
        }

    }
}
