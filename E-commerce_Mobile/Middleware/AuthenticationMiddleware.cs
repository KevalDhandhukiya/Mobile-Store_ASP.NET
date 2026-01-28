using Microsoft.AspNetCore.Mvc;

namespace E_commerce_Mobile.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly List<string> _allowedPaths;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
            _allowedPaths = new List<string>
            {
                "/auth/login",
                "/auth/logout", 
                "/auth/accessdenied",
                "/css/",
                "/js/",
                "/lib/",
                "/img/",
                "/vendor/",
                "/wwwroot/",
                "/swagger",
                "/api/",
                "/favicon.ico"
            };
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";

            // Allow access to login page, static files, and API endpoints
            if (_allowedPaths.Any(allowedPath => path.StartsWith(allowedPath.ToLower())))
            {
                await _next(context);
                return;
            }

            // Check if user is authenticated (has session)
            var userId = context.Session.GetString("UserId");
            
            if (string.IsNullOrEmpty(userId))
            {
                // User is not authenticated - redirect to login
                if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    // For AJAX requests, return 401
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }
                else
                {
                    // For regular requests, redirect to login
                    context.Response.Redirect("/Auth/Login");
                    return;
                }
            }

            // User is authenticated, continue to next middleware
            await _next(context);
        }
    }

    // Extension method to easily add the middleware
    public static class AuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticationMiddleware>();
        }
    }
}
