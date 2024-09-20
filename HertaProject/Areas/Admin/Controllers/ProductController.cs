using HertaProjectModels;
using HertaProjectModels.ViewModel;
using HertaProjectUtility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace HertaProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly string? _LocalBaseUrl;

   
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(HttpClient httpClient, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _LocalBaseUrl = _configuration["BaseUrl:Local"];
            _webHostEnvironment = webHostEnvironment;

        }
        public async Task<IActionResult> Index()
        {
            string requestUrl = $"{_LocalBaseUrl}rest/v1/Product/GetAllProduct";
            var response = await _httpClient.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var listObjProduct = JsonConvert.DeserializeObject<List<Product>>(jsonResponse);
                return View(listObjProduct);
            }

            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                return View(new Product());
            }
            else
            {
                string requestUrl = $"{_LocalBaseUrl}rest/v1/Product/GetProductById/{id}";
                var response = await _httpClient.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var ProductObj = JsonConvert.DeserializeObject<Product>(jsonResponse);

                    return View(ProductObj);
                }
                else
                {
                    return NotFound();
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(ProductVM productVM, IFormFile file)
        {
            string requestUrl = $"{_LocalBaseUrl}/rest/v1/Product/Upsert";
            using (var multipartContent = new MultipartFormDataContent())
            {
                multipartContent.Add(new StringContent(JsonConvert.SerializeObject(productVM), Encoding.UTF8, "application/json"), "productVM");
                if (file != null)
                {
                    var fileContent = new StreamContent(file.OpenReadStream());
                    multipartContent.Add(fileContent, "file", file.FileName);
                }
                var response = await _httpClient.PostAsync(requestUrl, multipartContent);
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Product created/updated successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to create/update product" });
                }
            }
        }


        #region API CALLS
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            string requestUrl = $"{_LocalBaseUrl}rest/v1/Product/GetAllProduct";
            var response = await _httpClient.GetAsync(requestUrl);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var listObjProduct = JsonConvert.DeserializeObject<List<Product>>(jsonResponse);
                return Json(listObjProduct);
            }

            return Json(new { success = false, message = "Error while getting" });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            string requestUrl = $"{_LocalBaseUrl}/rest/v1/Product/Delete/{id}";

            // Mengirim permintaan delete ke API
            var response = await _httpClient.DeleteAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Product deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // Handle jika terjadi error saat penghapusan
                TempData["error"] = "Error deleting product";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion
    }
}
