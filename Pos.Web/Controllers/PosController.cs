using Microsoft.AspNetCore.Mvc;

namespace Pos.Web.Controllers
{
    public sealed class PosController : Controller
    {
        private readonly IConfiguration _config;

        public PosController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Index()
        {
            ViewBag.ApiBaseUrl = _config["Api:BaseUrl"];
            return View();
        }
    }
}
