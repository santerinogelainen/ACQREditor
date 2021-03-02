
using System.ComponentModel;
using Xamarin.Forms;

namespace ACQREditor.Views
{
    [DesignTimeVisible(false)]
    public partial class ScanPage : ContentPage
    {
        public ScanPage()
        {
            InitializeComponent();

            scanView.Options.PossibleFormats = new System.Collections.Generic.List<ZXing.BarcodeFormat>
            {
                ZXing.BarcodeFormat.QR_CODE
            };
        }

        private void scanView_OnScanResult(ZXing.Result result)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Scanned result", "The barcode's text is " + result.Text + ". The barcode's format is ", "OK");
            });
        }
    }
}