namespace WebApiVersioning.Api.Controllers.Requests
{
    public class PostOrderRequestV2
    {
        public string ProductCode { get; set; }
        public int Quantity { get; set; }
        public int Country { get; set; }
        public int City { get; set; }
        public string AddressDetail { get; set; }
    }
}