using TouchTracking;

namespace ACQREditor.Models
{
    public struct TouchPosition
    {

        public TouchTrackingPoint StartPoint;
        public TouchTrackingPoint CurrentPoint;
        public TouchActionType CurrentAction;

    }
}
