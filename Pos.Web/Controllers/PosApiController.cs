using Microsoft.AspNetCore.Mvc;
using Pos.Web.Models.Api;

namespace Pos.Web.Controllers
{
    [ApiController]
    [Route("posapi")]
    public sealed class PosApiController : ControllerBase
    {
        private readonly IHttpClientFactory _http;

        public PosApiController(IHttpClientFactory http)
        {
            _http = http;
        }

        [HttpGet("products")]
        public async Task<IActionResult> Products([FromQuery] string? search)
        {
            var api = _http.CreateClient("PosApi");

            var url = string.IsNullOrWhiteSpace(search)
                ? "/api/products"
                : $"/api/products?search={Uri.EscapeDataString(search)}";

            var resp = await api.GetAsync(url);
            var body = await resp.Content.ReadAsStringAsync();

            return new ContentResult
            {
                StatusCode = (int)resp.StatusCode,
                ContentType = "application/json",
                Content = body
            };
        }

        [HttpPost("sales")]
        public async Task<IActionResult> CreateSale([FromBody] CreateSaleDto dto)
        {
            var api = _http.CreateClient("PosApi");

            var resp = await api.PostAsJsonAsync("/api/sales", dto);
            var body = await resp.Content.ReadAsStringAsync();

            return new ContentResult
            {
                StatusCode = (int)resp.StatusCode,
                ContentType = "application/json",
                Content = body
            };
        }

        [HttpPost("sales/{id:int}/payments")]
        public async Task<IActionResult> PaySale(int id, [FromBody] CreatePaymentDto dto)
        {
            var api = _http.CreateClient("PosApi");

            var resp = await api.PostAsJsonAsync($"/api/sales/{id}/payments", dto);
            var body = await resp.Content.ReadAsStringAsync();

            return new ContentResult
            {
                StatusCode = (int)resp.StatusCode,
                ContentType = "application/json",
                Content = body
            };
        }
    }
}
