using E_commerce_Mobile.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace E_commerce_Mobile.Controllers
{
    public class UsersController : BaseController
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;

        public UsersController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _client = httpClientFactory.CreateClient();
            _configuration = configuration;
            
            string baseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5001/api";
            // Ensure successful parsing or fallback
            if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out Uri? baseUri))
            {
                baseUri = new Uri("http://localhost:5001/api");
            }
            _client.BaseAddress = baseUri;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            try
            {
                // Explicitly construct URL to be sure
                var baseUrl = _client.BaseAddress?.ToString().TrimEnd('/') ?? "http://localhost:5001/api";
                // If BaseAddress already ends with /api, don't append it again if not needed, 
                // but the Controller actions usually expect "api/UserAPI"
                // Actually BaseAddress usually is just the host. 
                // Let's rely on absolute URL construction for safety.
                
                string requestUrl = $"{baseUrl}/UserAPI";
                // Handle case where baseUrl already includes /UserAPI in error
                if (baseUrl.EndsWith("/UserAPI")) requestUrl = baseUrl;
                
                Console.WriteLine($"[Index] Fetching users from: {requestUrl}");
                
                var response = await _client.GetAsync(requestUrl);
                Console.WriteLine($"[Index] Response Status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[Index] Received JSON (len {jsonString.Length})");
                    var users = JsonConvert.DeserializeObject<List<UserModel>>(jsonString) ?? new List<UserModel>();
                    return View(users);
                }
                else
                {
                    Console.WriteLine($"[Index] Failed to load users. Status: {response.StatusCode}");
                    ViewBag.Error = "Unable to load users. Please try again.";
                    return View(new List<UserModel>());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Index] Exception: {ex.Message}");
                ViewBag.Error = $"Error loading users: {ex.Message}";
                return View(new List<UserModel>());
            }
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                 var baseUrl = _client.BaseAddress?.ToString().TrimEnd('/') ?? "http://localhost:5001/api";
                 string requestUrl = $"{baseUrl}/UserAPI/{id}";
                 
                var response = await _client.GetAsync(requestUrl);
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<UserModel>(jsonString);
                    return View(user);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error loading user details: {ex.Message}";
                return View();
            }
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserModel user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    user.Created = DateTime.Now;
                    user.Modified = DateTime.Now;
                    
                    var json = JsonConvert.SerializeObject(user);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    
                    var baseUrl = _client.BaseAddress?.ToString().TrimEnd('/') ?? "http://localhost:5001/api";
                    string requestUrl = $"{baseUrl}/UserAPI";
                    
                    Console.WriteLine($"[Create] Sending to: {requestUrl}");

                    var response = await _client.PostAsync(requestUrl, content);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "User created successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        ViewBag.Error = $"Error creating user: {errorContent}";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = $"Error creating user: {ex.Message}";
                }
            }
            
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var baseUrl = _client.BaseAddress?.ToString().TrimEnd('/') ?? "http://localhost:5001/api";
                string requestUrl = $"{baseUrl}/UserAPI/{id}";

                var response = await _client.GetAsync(requestUrl);
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<UserModel>(jsonString);
                    return View(user);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error loading user: {ex.Message}";
                return View();
            }
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserModel user)
        {
            if (user == null || id != user.UserId)
            {
                return NotFound();
            }

            try
            {
                user.Modified = DateTime.Now;
                
                var json = JsonConvert.SerializeObject(user);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                var baseUrl = _client.BaseAddress?.ToString().TrimEnd('/') ?? "http://localhost:5001/api";
                string requestUrl = $"{baseUrl}/UserAPI/{id}";
                
                var response = await _client.PutAsync(requestUrl, content);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "User updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ViewBag.Error = $"Error updating user: {errorContent}";
                    return View(user);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error updating user: {ex.Message}";
                return View(user);
            }
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var baseUrl = _client.BaseAddress?.ToString().TrimEnd('/') ?? "http://localhost:5001/api";
                string requestUrl = $"{baseUrl}/UserAPI/{id}";
                
                var response = await _client.GetAsync(requestUrl);
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<UserModel>(jsonString);
                    return View(user);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error loading user: {ex.Message}";
                return View();
            }
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var baseUrl = _client.BaseAddress?.ToString().TrimEnd('/') ?? "http://localhost:5001/api";
                string requestUrl = $"{baseUrl}/UserAPI/{id}";
                
                var response = await _client.DeleteAsync(requestUrl);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "User deleted successfully!";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Error deleting user: {errorContent}";
                }
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting user: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
