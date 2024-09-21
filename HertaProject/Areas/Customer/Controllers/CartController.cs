using HertaProjectDataAccess.Repository.IRepository;
using HertaProjectModels;
using HertaProjectModels.ViewModel;
using HertaProjectUtility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stripe.Checkout;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace HertaProject.Areas.Customer.Controllers
{
    [Area("customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly string? _LocalBaseUrl;


        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _LocalBaseUrl = _configuration["BaseUrl:Local"];
        }

        public async Task<IActionResult> Index()
        {
            string requestUrl = $"{_LocalBaseUrl}rest/v1/Cart/GetCart";
            var response = await _httpClient.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                ShoppingCartVM = JsonConvert.DeserializeObject<ShoppingCartVM>(jsonResponse);
            }
            else
            {
                // Handle the error, perhaps set a message in TempData
                TempData["Error"] = "Unable to retrieve cart data.";
            }

            return View(ShoppingCartVM);
        }

        public async Task<IActionResult> Summary()
        {
            string requestUrl = $"{_LocalBaseUrl}rest/v1/Cart/Summary";
            var response = await _httpClient.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                ShoppingCartVM = JsonConvert.DeserializeObject<ShoppingCartVM>(jsonResponse);
            }
            else
            {
                // Handle the error, perhaps set a message in TempData
                TempData["Error"] = "Unable to retrieve cart summary.";
            }

            return View(ShoppingCartVM);
        }


        [HttpPost]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPOST()
        {
            var shoppingCartVM = new ShoppingCartVM();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string apiUrl = $"{_LocalBaseUrl}rest/v1/Cart/SummaryPOST";

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var postData = new
            {
                ShoppingCartVM = shoppingCartVM
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(apiUrl, jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                var orderId = (int)result.id;

                return RedirectToAction(nameof(OrderConfirmation), new { id = orderId });
            }
            else
            {
                ModelState.AddModelError("", "An error occurred while processing your order. Please try again.");
                return View("Error");
            }
        }


        public async Task<IActionResult> OrderConfirmation(int id)
        {
          
            string apiUrl = $"{_LocalBaseUrl}rest/v1/Cart/OrderConfirmation/{id}";
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<int>(jsonResponse);

                ViewBag.OrderId = result;

                return View(result);
            }
            else
            {
                ModelState.AddModelError("", "An error occurred while retrieving your order confirmation. Please try again.");
                return View();
            }
        }


        public async Task<IActionResult> Plus(int cartId)
        {
            string apiUrl = $"{_LocalBaseUrl}rest/v1/Cart/Plus";
            var content = new StringContent(JsonConvert.SerializeObject(new { cartId }), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "An error occurred while updating your cart. Please try again.");
                return RedirectToAction(nameof(Index));
            }

        }

        public async Task<IActionResult> Minus(int cartId)
        {
            string apiUrl = $"{_LocalBaseUrl}rest/v1/Cart/Minus";

            var content = new StringContent(JsonConvert.SerializeObject(new { cartId }), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "An error occurred while updating your cart. Please try again.");
                return RedirectToAction(nameof(Index));
            }

        }

        public async Task<IActionResult> Remove(int cartId)
        {
            string apiUrl = $"{_LocalBaseUrl}rest/v1/Cart/Remove";

            var content = new StringContent(JsonConvert.SerializeObject(new { cartId }), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
            
                return RedirectToAction(nameof(Index));
            }
            else
            {
        
                ModelState.AddModelError("", "An error occurred while updating your cart. Please try again.");
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
