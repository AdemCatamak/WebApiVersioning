using Microsoft.AspNetCore.Mvc;
using WebApiVersioning.Api.Controllers.Requests;
using WebApiVersioning.Api.Controllers.Responses;

namespace WebApiVersioning.Api.Controllers
{
    [ApiController]
    [Route("orders")]
    public class OrderController : ControllerBase
    {
        [HttpPost]
        [ApiVersion("1")]
        public IActionResult Post([FromBody] PostOrderRequest postOrderRequest)
        {
            if (string.IsNullOrEmpty(postOrderRequest?.ProductCode))
            {
                return BadRequest("ProductCode should not be empty");
            }

            if ((postOrderRequest?.Quantity ?? 0) < 0)
            {
                return BadRequest("Quantity should be greater than zero");
            }

            if (string.IsNullOrEmpty(postOrderRequest.Address))
            {
                return BadRequest("Address should not be empty");
            }

            var orderResponse = new OrderResponse
                                {
                                    ProductCode = postOrderRequest.ProductCode,
                                    Quantity = postOrderRequest.Quantity,
                                    Address = postOrderRequest.Address
                                };
            return Ok(orderResponse);
        }

        [HttpPost]
        [ApiVersion("2")]
        public IActionResult Post([FromBody] PostOrderRequestV2 postOrderRequest)
        {
            if (string.IsNullOrEmpty(postOrderRequest?.ProductCode))
            {
                return BadRequest("ProductCode should not be empty");
            }

            if ((postOrderRequest?.Quantity ?? 0) < 0)
            {
                return BadRequest("Quantity should be greater than zero");
            }

            if ((postOrderRequest?.Country ?? 0) < 0)
            {
                return BadRequest("Country should be greater than zero");
            }

            if ((postOrderRequest?.City ?? 0) < 0)
            {
                return BadRequest("City should be greater than zero");
            }

            if (string.IsNullOrEmpty(postOrderRequest.AddressDetail))
            {
                return BadRequest("AddressDetail should not be empty");
            }

            var orderResponse = new OrderResponse
                                {
                                    ProductCode = postOrderRequest.ProductCode,
                                    Quantity = postOrderRequest.Quantity,
                                    Address = $"{postOrderRequest.Country} - {postOrderRequest.City} | {postOrderRequest.AddressDetail}"
                                };
            return Ok(orderResponse);
        }
    }
}