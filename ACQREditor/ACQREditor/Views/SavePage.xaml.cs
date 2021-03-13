using ACQREditor.Models;
using System.Text;
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

            GenerateQRCode(design);
        }

        private void GenerateQRCode(DesignInfo design)
        {
            //ISO-8859-1

            // TODO

            //QRCode.BarcodeValue = Encoding.GetEncoding("ISO-8859-1").GetString(design.Bitmap.Bytes);
        }
    }
}