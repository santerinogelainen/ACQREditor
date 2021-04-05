
using ACQREditor.Class;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing;
using ZXing.Mobile;

namespace ACQREditor.Views
{
    [DesignTimeVisible(false)]
    public partial class ScanPage : ContentPage
    {

        private readonly QRReader Reader;

        public ScanPage()
        {
            InitializeComponent();

            // remove red line
            var redLine = overlay.Children.FirstOrDefault(x => x.BackgroundColor == Color.Red); 

            if (redLine != null)
                overlay.Children.Remove(redLine);

            scanView.Options.CameraResolutionSelector = SelectLowestResolutionMatchingDisplayAspectRatio;
            scanView.Options.PossibleFormats = new List<BarcodeFormat>
            {
                BarcodeFormat.QR_CODE
            };

            Reader = new QRReader();
        }

        public CameraResolution SelectLowestResolutionMatchingDisplayAspectRatio(List<CameraResolution> availableResolutions)
        {
            // https://dzone.com/articles/how-to-avoid-a-distorted-android-camera-preview-wi

            CameraResolution result = null;

            //a tolerance of 0.1 should not be recognizable for users
            double aspectTolerance = 0.1;

            //calculating our targetRatio
            var targetRatio = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Width;
            var targetHeight = DeviceDisplay.MainDisplayInfo.Height;
            var minDiff = double.MaxValue;

            //camera API lists all available resolutions from highest to lowest, perfect for us
            //making use of this sorting, following code runs some comparisons to select the lowest resolution that matches the screen aspect ratio
            //selecting the lowest makes QR detection actual faster most of the time
            foreach (var r in availableResolutions)
            {
                //if current ratio is bigger than our tolerance, move on
                //camera resolution is provided landscape ...
                if (Math.Abs(((double)r.Width / r.Height) - targetRatio) > aspectTolerance)
                    continue;
                else
                    if (Math.Abs(r.Height - targetHeight) < minDiff)
                    minDiff = Math.Abs(r.Height - targetHeight);

                result = r;
            }

            return result;
        }

        public void scanView_OnScanResult(Result result)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var response = Reader.Read(result.RawBytes);

                if (!response.Success)
                {
                    overlay.BottomText = response.Message;
                    return;
                }

                await Navigation.PushAsync(new EditorPage(response.Design));
                Navigation.RemovePage(this);
            });
        }
    }
}