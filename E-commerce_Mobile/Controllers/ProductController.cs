using E_commerce_Mobile.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace E_commerce_Mobile.Controllers
{
    public class ProductController : BaseController
    {
        private readonly HttpClient _client;

        public ProductController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri("http://localhost:5001/");
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _client.GetAsync("api/ProductAPI");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var products = JsonConvert.DeserializeObject<List<ProductModel>>(jsonString);
                    return View("ProductList", products);
                }
                else
                {
                    ViewBag.Error = "Unable to load products from API.";
                    return View("ProductList", new List<ProductModel>());
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View("ProductList", new List<ProductModel>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _client.GetAsync($"api/ProductAPI/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var product = JsonConvert.DeserializeObject<ProductModel>(jsonString);
                    return View(product);
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

        // GET: Product/Create
        public async Task<IActionResult> Create()
        {
            var model = new ProductModel();
            await LoadCategories(model);
            await LoadUsers();
            return View(model);
        }

        // GET: Product/AddProduct - Alternative action name
        public async Task<IActionResult> AddProduct()
        {
            var model = new ProductModel();
            await LoadCategoriesForViewBag();
            await LoadUsers();
            return View(model);
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModel product)
        {
            // Debug: Show ModelState
            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors.Select(y => $"{x.Key}: {y.ErrorMessage}"));
                ViewBag.Error = $"Validation Errors: {string.Join(", ", errors)}";
                await LoadCategories(product);
                await LoadUsers();
                return View(product);
            }
            
            try
            {
                // Set default values for server-side fields
                product.Created = DateTime.Now;
                product.Modified = DateTime.Now;
                // Note: UserId comes from form, IsRemoved comes from checkbox
                
                var json = JsonConvert.SerializeObject(product);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                ViewBag.DebugInfo = $"Sending JSON: {json}";
                
                var response = await _client.PostAsync("api/ProductAPI", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ViewBag.Error = $"API Error ({response.StatusCode}): {errorContent}";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
            }
            
            await LoadCategories(product);
            await LoadUsers();
            return View(product);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _client.GetAsync($"api/ProductAPI/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var product = JsonConvert.DeserializeObject<ProductModel>(jsonString);
                    await LoadCategories(product);
                    return View(product);
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

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductModel product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var json = JsonConvert.SerializeObject(product);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    
                    var response = await _client.PutAsync($"api/ProductAPI/{id}", content);
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
            await LoadCategories(product);
            return View(product);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _client.GetAsync($"api/ProductAPI/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var product = JsonConvert.DeserializeObject<ProductModel>(jsonString);
                    return View(product);
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

        // GET: Product/DeleteProduct/5 - Alternative action name
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var response = await _client.GetAsync($"api/ProductAPI/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var product = JsonConvert.DeserializeObject<ProductModel>(jsonString);
                    return View("Delete", product);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View("Delete");
            }
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var response = await _client.DeleteAsync($"api/ProductAPI/{id}");
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

        // POST: Product/SaveProduct - Alternative action name for AddProduct form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveProduct(ProductModel product)
        {
            // Debug: Show ModelState
            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors.Select(y => $"{x.Key}: {y.ErrorMessage}"));
                ViewBag.Error = $"Validation Errors: {string.Join(", ", errors)}";
                await LoadCategoriesForViewBag();
                await LoadUsers();
                return View("AddProduct", product);
            }
            
            try
            {
                // Set default values for server-side fields
                product.Created = DateTime.Now;
                product.Modified = DateTime.Now;
                // Note: UserId comes from form, IsRemoved comes from checkbox
                
                var json = JsonConvert.SerializeObject(product);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                ViewBag.DebugInfo = $"Sending JSON: {json}";
                
                var response = await _client.PostAsync("api/ProductAPI", content);
                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Success = "Product created successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ViewBag.Error = $"API Error ({response.StatusCode}): {errorContent}";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
            }
            
            await LoadCategoriesForViewBag();
            await LoadUsers();
            return View("AddProduct", product);
        }

        private async Task LoadCategories(ProductModel model)
        {
            try
            {
                var response = await _client.GetAsync("api/CategoryAPI");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var categories = JsonConvert.DeserializeObject<List<CategoryModel>>(jsonString);
                    model.CategoryList = categories?.Select(c => new CategoryDropdownModel
                    {
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName
                    }).ToList() ?? new List<CategoryDropdownModel>();
                }
                else
                {
                    model.CategoryList = new List<CategoryDropdownModel>();
                }
            }
            catch (Exception)
            {
                model.CategoryList = new List<CategoryDropdownModel>();
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

        private async Task LoadCategoriesForViewBag()
        {
            try
            {
                var response = await _client.GetAsync("api/CategoryAPI");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var categories = JsonConvert.DeserializeObject<List<CategoryModel>>(jsonString);
                    ViewBag.Categories = categories?.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.CategoryName
                    }).ToList() ?? new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
                }
                else
                {
                    ViewBag.Categories = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
                }
            }
            catch (Exception)
            {
                ViewBag.Categories = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
            }
        }
    }
}
