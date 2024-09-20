using HertaProjectModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace HertaProject.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly string? _LocalBaseUrl;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, HttpClient httpClient)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClient;
            _LocalBaseUrl = _configuration["BaseUrl:Local"];
        }

        public async  Task<IActionResult> Index()
        {
            string requestUrl = $"{_LocalBaseUrl}rest/v1/Home/GetAll";
            var response = await _httpClient.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var listObjProduct = JsonConvert.DeserializeObject<List<Product>>(jsonResponse);
                return View(listObjProduct);
            }

            return View();
        }

        public async Task<IActionResult> Details(int productId)
        {
            string requestUrl = $"{_LocalBaseUrl}/rest/v1/Home/Details/{productId}";
            var response = await _httpClient.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                ShoppingCart cart = JsonConvert.DeserializeObject<ShoppingCart>(responseBody);

                return View(cart);
            }

            return BadRequest(new { status = "400", message = "Failed to load product details" });
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
        {
            string requestUrl = $"{_LocalBaseUrl}/rest/v1/Home/Details";
            var jsonContent = JsonConvert.SerializeObject(shoppingCart);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(requestUrl, content);

            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = "Error updating cart";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
