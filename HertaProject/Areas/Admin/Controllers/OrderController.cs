using HertaProjectDataAccess.Repository.IRepository;
using HertaProjectModels;
using HertaProjectModels.ViewModel;
using HertaProjectUtility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using System.Text;

namespace HertaProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly string? _LocalBaseUrl;


        public OrderVM OrderVM { get; set; }
        public OrderController(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _LocalBaseUrl = _configuration["BaseUrl:Local"];
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Details(int orderId)
        {
            string apiUrl = $"{_LocalBaseUrl}rest/v1/Order/Details/{orderId}";
            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                var orderVM = JsonConvert.DeserializeObject<OrderVM>(result);

                return View(orderVM);
            }
            else
            {
                return NotFound();
            }

        }


        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> UpdateOrderDetails(int orderId, OrderVM orderVM)
        {
            string requestUrl = $"{_LocalBaseUrl}rest/v1/Order/updateOrderDetails/{orderId}";

            var orderDetails = new
            {
                orderVM.OrderHeader.Name,
                orderVM.OrderHeader.PhoneNumber,
                orderVM.OrderHeader.StreetAddress,
                orderVM.OrderHeader.City,
                orderVM.OrderHeader.State,
                orderVM.OrderHeader.PostalCode,
                Carrier = string.IsNullOrEmpty(orderVM.OrderHeader.Carrier) ? null : orderVM.OrderHeader.Carrier,
                TrackingNumber = string.IsNullOrEmpty(orderVM.OrderHeader.TrackingNumber) ? null : orderVM.OrderHeader.TrackingNumber
            };

            var response = await _httpClient.PostAsJsonAsync(requestUrl, orderDetails);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Order Details Updated Successfully.";
                return RedirectToAction(nameof(Details), new { orderId });
            }
            else
            {
                TempData["Error"] = "Error occurred while updating order details.";
                return RedirectToAction(nameof(Details), new { orderId });
            }
        }


        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> StartProcessing(int orderId)
        {
            string requestUrl = $"{_LocalBaseUrl}rest/v1/Order/StartProcessing/{orderId}";

            var response = await _httpClient.PostAsync(requestUrl, null);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Order is now In Process.";
                return RedirectToAction(nameof(Details), new { orderId });
            }
            else
            {
                TempData["Error"] = "Error occurred while processing the order.";
                return RedirectToAction(nameof(Details), new { orderId });
            }
        }


        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> ShipOrder(int orderId, string trackingNumber, string carrier)
        {
            string requestUrl = $"{_LocalBaseUrl}rest/v1/Order/ShipOrder";

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("orderId", orderId.ToString()),
                new KeyValuePair<string, string>("trackingNumber", trackingNumber),
                new KeyValuePair<string, string>("carrier", carrier)
            });

            var response = await _httpClient.PostAsync(requestUrl, formContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Order Shipped Successfully.";
                return RedirectToAction(nameof(Details), new { orderId });
            }
            else
            {
                TempData["Error"] = "Error occurred while shipping the order.";
                return RedirectToAction(nameof(Details), new { orderId });
            }
        }


        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            string requestUrl = $"{_LocalBaseUrl}rest/v1/Order/CancelOrder";
      
            var content = new StringContent(JsonConvert.SerializeObject(orderId), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(requestUrl, content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Order Cancelled Successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to cancel the order.";
            }

            return RedirectToAction(nameof(Details), new { orderId = orderId });
        }


        [ActionName("Details")]
        [HttpPost]
        public async Task<IActionResult> Details_PAY_NOW(int orderId)
        {
            string apiUrl = $"{_LocalBaseUrl}rest/v1/Order/Details_PAY_NOW";

            var content = new StringContent(JsonConvert.SerializeObject(new { orderId }), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var locationHeader = response.Headers.Location.ToString();
                return Redirect(locationHeader); // Mengarahkan pengguna ke Stripe
            }
            else
            {
                TempData["Error"] = "Failed to initiate payment.";
                return RedirectToAction(nameof(Details), new { orderId });
            }

        }

        public async Task<IActionResult> PaymentConfirmation(int orderHeaderId)
        {
            var apiUrl = $"{_LocalBaseUrl}rest/v1/Order/paymentConfirmation/{orderHeaderId}";
            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(content);

                var orderNewHeaderId = result.orderHeaderId;
                ViewBag.Message = "Payment Confirmation Successful.";
                return View(orderNewHeaderId);
            }
            else
            {
                ViewBag.Message = "Failed to confirm payment. Please try again later.";
            }

            return View();
        }

        #region API CALLS
        [HttpGet]
        public async Task<IActionResult> GetAll(string status)
        {
            var apiUrl = $"{_LocalBaseUrl}rest/v1/Order/Order/GetAll?status={status}";
            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(content);

                var orderHeaders = result.data.ToObject<List<OrderHeader>>();
                return Json(new { data = orderHeaders });
            }
            else
            {
                return Json(new { error = "Failed to retrieve data. Please try again later." });
            }

        }


        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Invalid ID" });
            }
            var apiUrl = $"{_LocalBaseUrl}rest/v1/Order/Order/Delete/{id}";
            var response = await _httpClient.DeleteAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(content);

                if (result.success)
                {
                    return Json(new { success = true, message = "Delete Successful" });
                }
                else
                {
                    return Json(new { success = false, message = result.message ?? "Error while deleting" });
                }
            }
            else
            {
                return Json(new { success = false, message = "Failed to delete. Please try again later." });
            }
        }

        #endregion
    }
}
