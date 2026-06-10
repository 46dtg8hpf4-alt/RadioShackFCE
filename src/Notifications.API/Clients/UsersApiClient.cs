namespace Notifications.API.Clients
{
    public class UsersApiClient
    {
        private readonly HttpClient _httpClient;

        public UsersApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> UserExists(Guid userId)
        {
            HttpResponseMessage response =
                await _httpClient.GetAsync(
                    $"http://localhost:5166/api/Users/{userId}");

            return response.IsSuccessStatusCode;
        }
    }
}