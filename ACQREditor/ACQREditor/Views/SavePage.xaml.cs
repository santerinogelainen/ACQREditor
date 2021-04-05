using ACQREditor.Class;
using ACQREditor.Models;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ACQREditor.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SavePage : ContentPage
    {
        public SavePage(DesignInfo design)
        {
            InitializeComponent();

            var writer = new QRWriter();

            QRCode.BarcodeOptions.Width = (int)DeviceDisplay.MainDisplayInfo.Width;
            QRCode.BarcodeOptions.Height = (int)DeviceDisplay.MainDisplayInfo.Width;
            QRCode.BarcodeOptions.PureBarcode = true;

            //ISO-8859-1 ????
            QRCode.BarcodeValue = Encoding.GetEncoding("ISO-8859-1").GetString(writer.Write(design));
        }
    }
}