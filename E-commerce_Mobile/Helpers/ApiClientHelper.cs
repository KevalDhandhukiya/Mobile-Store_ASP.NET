using System.Net.Http.Headers;

namespace E_commerce_Mobile.Helpers
{
    public static class ApiClientHelper
    {
        public static void SetAuthorizationHeader(HttpClient client, string? jwtToken)
        {
            if (!string.IsNullOrEmpty(jwtToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            }
            else
            {
                client.DefaultRequestHeaders.Authorization = null;
            }
        }

        public static void SetAuthorizationHeader(HttpClient client, HttpContext httpContext)
        {
            var jwtToken = httpContext.Session.GetString("JwtToken");
            SetAuthorizationHeader(client, jwtToken);
        }
    }
}
