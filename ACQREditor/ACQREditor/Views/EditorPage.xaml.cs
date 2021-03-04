using ACQREditor.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ACQREditor.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditorPage : ContentPage
    {
        private DesignInfo Design { get;}


        public EditorPage(DesignInfo design)
        {
            Design = design;

            InitializeComponent();
        }
    }
}