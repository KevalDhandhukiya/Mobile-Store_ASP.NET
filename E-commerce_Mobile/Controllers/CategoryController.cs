using E_commerce_Mobile.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace E_commerce_Mobile.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly HttpClient _client;

        public CategoryController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri("http://localhost:5001/");
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _client.GetAsync("api/CategoryAPI");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var categories = JsonConvert.DeserializeObject<List<CategoryModel>>(jsonString);
                    return View(categories);
                }
                else
                {
                    ViewBag.Error = "Unable to load categories from API.";
                    return View(new List<CategoryModel>());
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(new List<CategoryModel>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _client.GetAsync($"api/CategoryAPI/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var category = JsonConvert.DeserializeObject<CategoryModel>(jsonString);
                    return View(category);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View();
            }
        }

        // GET: Category/Create
        public async Task<IActionResult> Create()
        {
            await LoadUsers();
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Set timestamps
                    category.Created = DateTime.Now;
                    category.Modified = DateTime.Now;
                    
                    var json = JsonConvert.SerializeObject(category);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    
                    var response = await _client.PostAsync("api/CategoryAPI", content);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        ViewBag.Error = $"API Error: {errorContent}";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = $"Error: {ex.Message}";
                }
            }
            await LoadUsers();
            return View(category);
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _client.GetAsync($"api/CategoryAPI/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var category = JsonConvert.DeserializeObject<CategoryModel>(jsonString);
                    await LoadUsers();
                    return View(category);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View();
            }
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryModel category)
        {
            if (id != category.CategoryId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    category.Modified = DateTime.Now;
                    
                    var json = JsonConvert.SerializeObject(category);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    
                    var response = await _client.PutAsync($"api/CategoryAPI/{id}", content);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        ViewBag.Error = $"API Error: {errorContent}";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = $"Error: {ex.Message}";
                }
            }
            await LoadUsers();
            return View(category);
        }

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _client.GetAsync($"api/CategoryAPI/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var category = JsonConvert.DeserializeObject<CategoryModel>(jsonString);
                    return View(category);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View();
            }
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var response = await _client.DeleteAsync($"api/CategoryAPI/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ViewBag.Error = $"API Error: {errorContent}";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        private async Task LoadUsers()
        {
            try
            {
                var response = await _client.GetAsync("api/UserAPI");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var users = JsonConvert.DeserializeObject<List<UserModel>>(jsonString);
                    ViewBag.Users = users?.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = u.UserId.ToString(),
                        Text = u.UserName
                    }).ToList() ?? new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
                }
                else
                {
                    ViewBag.Users = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
                }
            }
            catch (Exception)
            {
                ViewBag.Users = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
            }
        }
    }
}
