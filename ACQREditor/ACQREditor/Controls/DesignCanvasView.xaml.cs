
using ACQREditor.Models;
using SkiaSharp.Views.Forms;
using System.Collections.Generic;
using System.Linq;
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

        private DesignPosition Position;
        private DesignPosition EndPosition;

        private Dictionary<long, TouchPosition> Touches;

        public DesignCanvasView()
        {
            InitializeComponent();

            Touches = new Dictionary<long, TouchPosition>();
            CanvasView.PaintSurface += OnCanvasViewPaintSurface;
        }

        public void LoadDesign(DesignInfo design)
        {
            Design = design;

            Position.X = 0;
            Position.Y = 0;

            Position.Scale = (float)DeviceDisplay.MainDisplayInfo.Width / Design.Bitmap.Width;

            EndPosition = Position;

            InvalidateSurface();
        }  

        private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            if (Design == null)
                return;

            var canvas = args.Surface.Canvas;

            canvas.Clear();

            if (Touches.Count == 1)
            {
                var touch = Touches.First().Value; // TODO: Multitouch (up to 2) moving

                if (touch.CurrentAction == TouchActionType.Moved)
                {
                    Position.X = EndPosition.X - ((touch.StartPoint.X - touch.CurrentPoint.X) / (Position.Scale / 2));
                    Position.Y = EndPosition.Y - ((touch.StartPoint.Y - touch.CurrentPoint.Y) / (Position.Scale / 2));
                }
                else
                {
                    Position = EndPosition;
                }
            }

            canvas.Scale(Position.Scale);
            canvas.DrawBitmap(Design.Bitmap, Position.X, Position.Y);
        }

        public void InvalidateSurface()
        {
            CanvasView.InvalidateSurface();
        }

        private void ImageControls_TouchAction(object sender, TouchActionEventArgs args)
        {
            if (!Touches.TryGetValue(args.Id, out var touch))
                touch = default;

            touch.CurrentAction = args.Type;

            switch (args.Type)
            {
                case TouchActionType.Pressed:
                    touch.StartPoint = args.Location;
                    break;

                case TouchActionType.Moved:
                    touch.CurrentPoint = args.Location;
                    InvalidateSurface();
                    break;

                case TouchActionType.Released:
                case TouchActionType.Exited:
                case TouchActionType.Cancelled:
                    EndPosition = Position;
                    Touches.Remove(args.Id);
                    return;
            }

            Touches[args.Id] = touch;
        }
    }
}