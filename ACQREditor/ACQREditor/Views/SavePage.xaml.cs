using ACQREditor.Class;
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

            var writer = new QRWriter();

            //ISO-8859-1 ????
            QRCode.BarcodeValue = Encoding.GetEncoding("ISO-8859-1").GetString(writer.Write(design));
        }
    }
}