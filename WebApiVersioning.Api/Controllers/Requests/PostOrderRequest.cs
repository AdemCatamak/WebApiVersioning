namespace WebApiVersioning.Api.Controllers.Requests
{
    public class PostOrderRequest
    {
        public string ProductCode { get; set; }
        public int Quantity { get; set; }
        public string Address { get; set; }
    }
}