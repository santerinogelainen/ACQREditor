using ACQREditor.Extensions;
using ACQREditor.Models;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Collections.Generic;
using TouchTracking;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ACQREditor.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DesignCanvasView : ContentView
    {
        public DesignInfo Design { get; private set; }

        private SKMatrix Matrix;
        private readonly Dictionary<long, SKPoint> Touches;

        private float MaxScale;
        private float MinScale;

        public DesignCanvasView()
        {
            InitializeComponent();

            Matrix = SKMatrix.CreateIdentity();
            Touches = new Dictionary<long, SKPoint>();
            CanvasView.PaintSurface += OnCanvasViewPaintSurface;
        }

        public void LoadDesign(DesignInfo design)
        {
            Design = design;

            var scale = (float)DeviceDisplay.MainDisplayInfo.Width / Design.Bitmap.Width;

            Matrix.ScaleX = scale;
            Matrix.ScaleY = scale;

            MaxScale = scale * 3;
            MinScale = scale / 3;

            InvalidateSurface();
        }  

        private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            if (Design == null)
                return;

            var canvas = args.Surface.Canvas;

            canvas.Clear();

            canvas.SetMatrix(Matrix);
            canvas.DrawBitmap(Design.Bitmap, new SKPoint());
        }

        public void InvalidateSurface()
        {
            CanvasView.InvalidateSurface();
        }

        private void MoveOrScaleCanvas(long id, SKPoint point)
        {
            if (!Touches.ContainsKey(id))
                return;

            // Single-finger drag
            if (Touches.Count == 1)
            {
                var prevPoint = Touches[id];

                // Adjust the matrix for the new position
                Matrix.TransX += point.X - prevPoint.X;
                Matrix.TransY += point.Y - prevPoint.Y;

                var height = Matrix.ScaleY * Design.Bitmap.Height;
                var width = Matrix.ScaleX * Design.Bitmap.Width;

                Matrix.TransX = Matrix.TransX.Clamp(-(width - (width / 4f)), (float)CanvasView.CanvasSize.Width - (width / 4f));
                Matrix.TransY = Matrix.TransY.Clamp(-(height - (height / 4f)), (float)CanvasView.CanvasSize.Height - (height / 4f));

                InvalidateSurface();
            }

            // Double-finger scale and drag
            else if (Touches.Count >= 2)
            {
                // Copy two dictionary keys into array
                long[] keys = new long[Touches.Count];
                Touches.Keys.CopyTo(keys, 0);

                // Find index of non-moving (pivot) finger
                int pivotIndex = (keys[0] == id) ? 1 : 0;

                // Get the three points involved in the transform
                var pivotPoint = Touches[keys[pivotIndex]];
                var prevPoint = Touches[id];
                var newPoint = point;

                // Calculate two vectors
                var oldVector = prevPoint - pivotPoint;
                var newVector = newPoint - pivotPoint;

                // isotropic scaling
                float scale = newVector.Magnitude() / oldVector.Magnitude();

                if (!float.IsNaN(scale) && !float.IsInfinity(scale) &&
                    scale * Matrix.ScaleX <= MaxScale &&
                    scale * Matrix.ScaleX >= MinScale &&
                    scale * Matrix.ScaleY <= MaxScale &&
                    scale * Matrix.ScaleY >= MinScale)
                {
                    // If something bad hasn't happened, calculate a scale and translation matrix
                    var scaleMatrix = SKMatrix.CreateScale(scale, scale, pivotPoint.X, pivotPoint.Y);

                    SKMatrix.PostConcat(ref Matrix, scaleMatrix);
                    InvalidateSurface();
                }
            }

            Touches[id] = point;
        }

        private void ImageControls_TouchAction(object sender, TouchActionEventArgs args)
        {
            // https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/transforms/touch

            var point = new SKPoint((float)(CanvasView.CanvasSize.Width * args.Location.X / CanvasView.Width),
                            (float)(CanvasView.CanvasSize.Height * args.Location.Y / CanvasView.Height));

            switch (args.Type)
            {
                case TouchActionType.Pressed:
                    var rect = new SKRect(0, 0, Design.Bitmap.Width, Design.Bitmap.Height);
                    rect = Matrix.MapRect(rect);

                    if (rect.Contains(point) && !Touches.ContainsKey(args.Id))
                        Touches.Add(args.Id, point);
                    break;

                case TouchActionType.Moved:
                    MoveOrScaleCanvas(args.Id, point);
                    break;

                case TouchActionType.Released:
                case TouchActionType.Exited:
                case TouchActionType.Cancelled:
                    if (Touches.ContainsKey(args.Id))
                        Touches.Remove(args.Id);
                    return;
            }
        }
    }
}