﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Print;
using Android.Content;
using OCRBarcodeDesigner.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(DroidPrintService))]
namespace OCRBarcodeDesigner.Droid
{
    public class DroidPrintService : IPrintService
    {
        public DroidPrintService()
        {
        }

        public void Print(WebView viewToPrint)
        {
            var droidViewToPrint = Platform.CreateRenderer(viewToPrint).ViewGroup.GetChildAt(0) as Android.Webkit.WebView;

            if (droidViewToPrint != null)
            {
                // Only valid for API 19+
                var version = Android.OS.Build.VERSION.SdkInt;

                if (version >= Android.OS.BuildVersionCodes.Kitkat)
                {
                    var printMgr = (PrintManager)Forms.Context.GetSystemService(Context.PrintService);

                    printMgr.Print("Forms-EZ-Print", droidViewToPrint.CreatePrintDocumentAdapter(), null);
                }
            }
        }
    }
}
