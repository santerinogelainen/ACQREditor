﻿using ACQREditor.Models;
using ACQREditor.Resources;
using SkiaSharp;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ACQREditor.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditorPage : ContentPage
    {
        public EditorPage(DesignInfo design)
        {
            InitializeComponent();

            LoadDesignMetadata(design);
            Canvas.LoadDesign(design);
        }

        private void LoadDesignMetadata(DesignInfo design)
        {
            Page.Title = design.Title;

            lblTitle.Text = Labels.Title + ": " + design.Title;
            lblAuthor.Text = Labels.Author + ": " + design.Author;
            lblTown.Text = Labels.Town + ": " + design.Town;
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

        private void btnRotateCounter_Clicked(object sender, System.EventArgs e)
        {
            Canvas.Design.Bitmap = RotateBitmap(Canvas.Design.Bitmap, 90 * 3);
            Canvas.InvalidateSurface();
        }

        private void btnRotate_Clicked(object sender, System.EventArgs e)
        {
            Canvas.Design.Bitmap = RotateBitmap(Canvas.Design.Bitmap, 90);
            Canvas.InvalidateSurface();
        }

        private async void btnSave_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new SavePage(Canvas.Design));
        }
    }
}