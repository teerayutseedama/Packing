namespace Packing.Views
{
    public class ResponseMessage
    {
        public bool Status { get; set; }
        public string? Message { get; set; }
        public string? Error { set; get; }   
    }
}
