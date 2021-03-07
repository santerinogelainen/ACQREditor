using ACQREditor.Models;
using SkiaSharp;
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
        }

        private void LoadDesignMetadata()
        {
            lblTitle.Text = "Title: " + Design.Title;
            lblAuthor.Text = "Author: " + Design.Author;
            lblTown.Text = "Town: " + Design.Town;
        }

        private SKBitmap RotateBitmap(SKBitmap bitmap, int degrees)
        {
            using (var surface = new SKCanvas(bitmap))
            {
                surface.RotateDegrees(degrees, bitmap.Width / 2, bitmap.Height / 2);
                surface.DrawBitmap(bitmap.Copy(), 0, 0);
            }

            return bitmap;
        }

        private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            var canvas = args.Surface.Canvas;

            canvas.Clear();

            canvas.Scale((float)DeviceDisplay.MainDisplayInfo.Width / 32);
            canvas.DrawBitmap(Design.Bitmap, 0, 0);
        }

        private void btnRotateCounter_Clicked(object sender, System.EventArgs e)
        {
            Design.Bitmap = RotateBitmap(Design.Bitmap, 90);
            CanvasView.InvalidateSurface();
        }

        private void btnRotate_Clicked(object sender, System.EventArgs e)
        {
            Design.Bitmap = RotateBitmap(Design.Bitmap, 90 * 3);
            CanvasView.InvalidateSurface();
        }
    }
}