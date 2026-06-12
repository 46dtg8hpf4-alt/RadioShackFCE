using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Orders.API.Clients
{
    public class UsersApiClient
    {
        private readonly HttpClient _httpClient;

        public UsersApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> UserExistsAsync(Guid userId)
        {
            var response = await _httpClient.GetAsync($"/api/users/{userId}");
            return response.IsSuccessStatusCode;
        }
    }
}
