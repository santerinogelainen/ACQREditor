using ACQREditor.Models;
using SkiaSharp.Views.Forms;
using Xamarin.Essentials;
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
            InitializeComponent();

            Design = design;
            LoadDesignMetadata();

            CanvasView.PaintSurface += OnCanvasViewPaintSurface;
            CanvasView.WidthRequest = DeviceDisplay.MainDisplayInfo.Width;
            CanvasView.HeightRequest = DeviceDisplay.MainDisplayInfo.Width;
        }

        private void LoadDesignMetadata()
        {
            lblTitle.Text = "Title: " + Design.Title;
            lblAuthor.Text = "Author: " + Design.Author;
            lblTown.Text = "Town: " + Design.Town;
        }

        private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            var canvas = args.Surface.Canvas;

            canvas.Clear();

            canvas.Scale((float)DeviceDisplay.MainDisplayInfo.Width / 32);
            canvas.DrawBitmap(Design.Bitmap, 0, 0);
        }
    }
}