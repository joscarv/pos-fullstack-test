using Microsoft.AspNetCore.Mvc;
using Pos.Web.Models.Api;

namespace Pos.Web.Controllers
{
    public sealed class ProductsController : Controller
    {
        private readonly IHttpClientFactory _http;

        public ProductsController(IHttpClientFactory http)
        {
            _http = http;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var api = _http.CreateClient("PosApi");
            var url = string.IsNullOrWhiteSpace(search)
                ? "/api/products"
                : $"/api/products?search={Uri.EscapeDataString(search)}";

            var products = await api.GetFromJsonAsync<List<ProductDto>>(url) ?? new();
            ViewBag.Search = search ?? "";
            return View(products);
        }

        public IActionResult Create() => View(new CreateProductDto());

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var api = _http.CreateClient("PosApi");
            var resp = await api.PostAsJsonAsync("/api/products", dto);

            if (!resp.IsSuccessStatusCode)
            {
                ViewBag.Error = await resp.Content.ReadAsStringAsync();
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var api = _http.CreateClient("PosApi");

            var resp = await api.GetAsync($"/api/products/{id}");
            if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                return NotFound();

            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync();
                return Problem($"API error getting product {id}: {body}");
            }

            var p = await resp.Content.ReadFromJsonAsync<ProductDto>();
            if (p is null) return Problem("API returned empty product.");

            var dto = new CreateProductDto
            {
                Sku = p.Sku,
                Name = p.Name,
                Price = p.Price,
                IepsRate = p.IepsRate,
                Stock = p.Stock
            };

            ViewBag.Id = id;
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CreateProductDto dto)
        {
            var api = _http.CreateClient("PosApi");
            var resp = await api.PutAsJsonAsync($"/api/products/{id}", dto);

            if (!resp.IsSuccessStatusCode)
            {
                ViewBag.Error = await resp.Content.ReadAsStringAsync();
                ViewBag.Id = id;
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var api = _http.CreateClient("PosApi");
            await api.DeleteAsync($"/api/products/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
