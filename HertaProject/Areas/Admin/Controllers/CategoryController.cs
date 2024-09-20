using HertaProjectModels;
using HertaProjectUtility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace HertaProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly string? _LocalBaseUrl;

        public CategoryController(HttpClient httpClient, IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _LocalBaseUrl = _configuration["BaseUrl:Local"];

        }
        public async Task<IActionResult> Index()
        {
            string requestUrl = $"{_LocalBaseUrl}rest/v1/Category/GetAllCategory";
            var response = await _httpClient.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var listObjCategory = JsonConvert.DeserializeObject<List<Category>>(jsonResponse);
                return View(listObjCategory);
            }

            return View(); 
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            string requestUrl = $"{_LocalBaseUrl}rest/v1/Category/Create";
            var categoryJson = JsonConvert.SerializeObject(category);
            var content = new StringContent(categoryJson, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(requestUrl, content);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();

                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index", "Category");
            }
            else
            {
                return View();
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            string requestUrl = $"{_LocalBaseUrl}rest/v1/Category/GetCategoryById/{id}";
            var response = await _httpClient.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var category = JsonConvert.DeserializeObject<Category>(jsonResponse);

                return View(category); 
            }
            else
            {
                return NotFound();
            }
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            string requestUrl = $"{_LocalBaseUrl}rest/v1/Category/Update";
            var categoryJson = JsonConvert.SerializeObject(category);
            var content = new StringContent(categoryJson, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(requestUrl, content);

            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index", "Category");
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, errorResponse);
                return View(category);
            }
        }


        public async Task<IActionResult> Delete(int? id)
        {
            string requestUrl = $"{_LocalBaseUrl}rest/v1/Category/GetCategoryById/{id}";
            var response = await _httpClient.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var category = JsonConvert.DeserializeObject<Category>(jsonResponse);

                return View(category);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            string requestUrl = $"{_LocalBaseUrl}rest/v1/Category/DeleteCategory/{id}";
            var response = await _httpClient.PostAsync(requestUrl, null);
            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Category deleted successfully";
                return RedirectToAction("Index", "Category");
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, errorResponse);
                return RedirectToAction("Index");
            }
        }
    }
}
