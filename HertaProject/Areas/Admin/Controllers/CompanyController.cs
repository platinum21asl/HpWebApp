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
    public class CompanyController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly string? _LocalBaseUrl;

        public CompanyController(HttpClient httpClient, IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _LocalBaseUrl = _configuration["BaseUrl:Local"];
        }
        public async Task<IActionResult> Index()
        {
            string requestUrl = $"{_LocalBaseUrl}rest/v1/Company/GetAllCompany";
            var response = await _httpClient.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var listObjCompany = JsonConvert.DeserializeObject<List<Company>>(jsonResponse);
                return View(listObjCompany);
            }

            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                return View(new Company());
            }
            else
            {
                string requestUrl = $"{_LocalBaseUrl}rest/v1/Company/GetCompanyById/{id}";
                var response = await _httpClient.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var companyObj = JsonConvert.DeserializeObject<Company>(jsonResponse);

                    return View(companyObj);
                }
                else
                {
                    return NotFound();
                }
            }
        }


        [HttpPost]
        public async Task<IActionResult> Upsert(Company companyObj)
        {
            if (ModelState.IsValid)
            {
                string requestUrl = $"{_LocalBaseUrl}rest/v1/Company/Upsert";
                var companyJson = JsonConvert.SerializeObject(companyObj);
                var content = new StringContent(companyJson, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(requestUrl, content);

                if (companyObj.Id == 0)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["success"] = "Company Create successfully";
                        return RedirectToAction("Index", "Company");
                    }
                    else
                    {
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError(string.Empty, errorResponse);
                        return View(companyObj);
                    }
                }
                else
                {
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["success"] = "Company Update successfully";
                        return RedirectToAction("Index", "Company");
                    }
                    else
                    {
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError(string.Empty, errorResponse);
                        return View(companyObj);
                    }
                }
            }
            else
            {
                return View(companyObj);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            string requestUrl = $"{_LocalBaseUrl}rest/v1/Company/GetAllCompany";
            var response = await _httpClient.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var listObjCompany = JsonConvert.DeserializeObject<List<Company>>(jsonResponse);
                return Json(listObjCompany);
            }

            return Json(new { success = true, message = "Delete Successful" });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            string requestUrl = $"{_LocalBaseUrl}rest/v1/Company/Delete/{id}";
            var response = await _httpClient.DeleteAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Delete Successful" });
            }
            else
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
        }
    }
}
