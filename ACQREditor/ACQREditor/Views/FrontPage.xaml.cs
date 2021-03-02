using System.ComponentModel;
using Xamarin.Forms;

namespace ACQREditor.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class FrontPage : ContentPage
    {
        public FrontPage()
        {
            InitializeComponent();
        }

        private async void btnScan_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new ScanPage());
        }
    }
}