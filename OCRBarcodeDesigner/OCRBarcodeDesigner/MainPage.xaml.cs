using OCRBarcodeDesigner.PrintTemplates;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZXing.Mobile;

namespace OCRBarcodeDesigner
{
    public partial class MainPage : ContentPage
    {
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
    }
}
