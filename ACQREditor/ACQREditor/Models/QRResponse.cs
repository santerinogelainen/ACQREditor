namespace ACQREditor.Models
{
    public class QRResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public DesignInfo Design { get; set; }
    }
}
