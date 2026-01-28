using E_commerce_Mobile.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace E_commerce_Mobile.Controllers
{
    public class CartController : BaseController
    {
        private readonly HttpClient _client;

        public CartController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri("http://localhost:5001/");
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Auth");
                }

                var response = await _client.GetAsync($"api/CartAPI/user/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var cartItems = JsonConvert.DeserializeObject<List<CartModel>>(jsonString);
                    return View(cartItems);
                }
                else
                {
                    ViewBag.Error = "Unable to load cart items from API.";
                    return View(new List<CartModel>());
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(new List<CartModel>());
            }
        }

        // GET: Cart/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _client.GetAsync($"api/CartAPI/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var cartItem = JsonConvert.DeserializeObject<CartModel>(jsonString);
                    return View(cartItem);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error loading cart item: {ex.Message}";
                return View();
            }
        }

        // GET: Cart/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                // Load products for dropdown
                var productsResponse = await _client.GetAsync("api/ProductAPI");
                if (productsResponse.IsSuccessStatusCode)
                {
                    var productsJson = await productsResponse.Content.ReadAsStringAsync();
                    var products = JsonConvert.DeserializeObject<List<ProductModel>>(productsJson);
                    ViewBag.Products = products;
                }

                // Load users for dropdown
                var usersResponse = await _client.GetAsync("api/UserAPI");
                if (usersResponse.IsSuccessStatusCode)
                {
                    var usersJson = await usersResponse.Content.ReadAsStringAsync();
                    var users = JsonConvert.DeserializeObject<List<UserModel>>(usersJson);
                    ViewBag.Users = users;
                }

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error loading form data: {ex.Message}";
                return View();
            }
        }

        // POST: Cart/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CartModel cartItem)
        {
            ViewBag.DebugInfo = $"POST Create called - ModelState.IsValid: {ModelState.IsValid}";
            
            if (cartItem != null)
            {
                ViewBag.DebugInfo += $" | ProductId: {cartItem.ProductId}, UserId: {cartItem.UserId}, Quantity: {cartItem.Quantity}";
            }
            else
            {
                ViewBag.DebugInfo += " | CartItem is NULL!";
            }
            
            if (!ModelState.IsValid)
            {
                ViewBag.DebugInfo += " | ModelState Errors: ";
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    ViewBag.DebugInfo += error.ErrorMessage + "; ";
                }
                
                // Show all ModelState keys and values
                ViewBag.DebugInfo += " | ModelState Details: ";
                foreach (var kvp in ModelState)
                {
                    ViewBag.DebugInfo += $"{kvp.Key}={kvp.Value.AttemptedValue} ";
                }
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    // All dates and Total will be calculated by the API
                    cartItem.Total = 0;
                    cartItem.AddedDate = DateTime.MinValue;
                    cartItem.Created = DateTime.MinValue;
                    cartItem.Modified = DateTime.MinValue;

                    var json = JsonConvert.SerializeObject(cartItem);
                    ViewBag.DebugInfo += $" | JSON: {json}";
                    
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    var response = await _client.PostAsync("api/CartAPI", content);
                    ViewBag.DebugInfo += $" | API Response: {response.StatusCode}";
                    
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Item added to cart successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        ViewBag.Error = $"Error adding item to cart: {response.StatusCode} - {errorContent}";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = $"Error adding item to cart: {ex.Message}";
                }
            }
            else
            {
                ViewBag.Error = "Please fill all required fields correctly.";
            }

            // Reload dropdown data if there's an error
            try
            {
                var productsResponse = await _client.GetAsync("api/ProductAPI");
                if (productsResponse.IsSuccessStatusCode)
                {
                    var productsJson = await productsResponse.Content.ReadAsStringAsync();
                    var products = JsonConvert.DeserializeObject<List<ProductModel>>(productsJson);
                    ViewBag.Products = products;
                }

                var usersResponse = await _client.GetAsync("api/UserAPI");
                if (usersResponse.IsSuccessStatusCode)
                {
                    var usersJson = await usersResponse.Content.ReadAsStringAsync();
                    var users = JsonConvert.DeserializeObject<List<UserModel>>(usersJson);
                    ViewBag.Users = users;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error += $" Also failed to reload form data: {ex.Message}";
            }

            return View(cartItem);
        }

        // GET: Cart/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _client.GetAsync($"api/CartAPI/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var cartItem = JsonConvert.DeserializeObject<CartModel>(jsonString);

                    // Load products for dropdown
                    var productsResponse = await _client.GetAsync("api/ProductAPI");
                    if (productsResponse.IsSuccessStatusCode)
                    {
                        var productsJson = await productsResponse.Content.ReadAsStringAsync();
                        var products = JsonConvert.DeserializeObject<List<ProductModel>>(productsJson);
                        ViewBag.Products = products;
                    }

                    // Load users for dropdown
                    var usersResponse = await _client.GetAsync("api/UserAPI");
                    if (usersResponse.IsSuccessStatusCode)
                    {
                        var usersJson = await usersResponse.Content.ReadAsStringAsync();
                        var users = JsonConvert.DeserializeObject<List<UserModel>>(usersJson);
                        ViewBag.Users = users;
                    }

                    return View(cartItem);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error loading cart item: {ex.Message}";
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CartModel cartItem)
        {
            if (id != cartItem.CartId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get product price to calculate total
                    var productResponse = await _client.GetAsync($"api/ProductAPI/{cartItem.ProductId}");
                    if (productResponse.IsSuccessStatusCode)
                    {
                        var productJson = await productResponse.Content.ReadAsStringAsync();
                        var product = JsonConvert.DeserializeObject<ProductModel>(productJson);
                        cartItem.Total = product.Price * cartItem.Quantity;
                    }
                    else
                    {
                        ViewBag.Error = "Product not found";
                        return View(cartItem);
                    }

                    cartItem.Modified = DateTime.Now;

                    var json = JsonConvert.SerializeObject(cartItem);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    var response = await _client.PutAsync($"api/CartAPI/{id}", content);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Cart updated successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        ViewBag.Error = $"Error updating cart: {errorContent}";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = $"Error updating cart: {ex.Message}";
                }
            }
            else
            {
                ViewBag.Error = "Please fill all required fields correctly.";
            }

            // Reload dropdown data if there's an error
            try
            {
                var productsResponse = await _client.GetAsync("api/ProductAPI");
                if (productsResponse.IsSuccessStatusCode)
                {
                    var productsJson = await productsResponse.Content.ReadAsStringAsync();
                    var products = JsonConvert.DeserializeObject<List<ProductModel>>(productsJson);
                    ViewBag.Products = products;
                }

                var usersResponse = await _client.GetAsync("api/UserAPI");
                if (usersResponse.IsSuccessStatusCode)
                {
                    var usersJson = await usersResponse.Content.ReadAsStringAsync();
                    var users = JsonConvert.DeserializeObject<List<UserModel>>(usersJson);
                    ViewBag.Users = users;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error += $" Also failed to reload form data: {ex.Message}";
            }

            return View(cartItem);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "Please login first" });
                }
                
                var cartItem = new
                {
                    ProductId = productId,
                    UserId = int.Parse(userId),
                    Quantity = quantity,
                    Total = 0, // Will be calculated by API
                    AddedDate = DateTime.Now,
                    Created = DateTime.Now,
                    Modified = DateTime.Now
                };

                var json = JsonConvert.SerializeObject(cartItem);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                var response = await _client.PostAsync("api/CartAPI", content);
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Item added to cart successfully" });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"Failed to add item to cart: {errorContent}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // GET: Cart/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _client.GetAsync($"api/CartAPI/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var cartItem = JsonConvert.DeserializeObject<CartModel>(jsonString);
                    return View(cartItem);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error loading cart item: {ex.Message}";
                return View();
            }
        }

        // POST: Cart/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var response = await _client.DeleteAsync($"api/CartAPI/{id}");
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Item removed from cart successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to remove item from cart.";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
