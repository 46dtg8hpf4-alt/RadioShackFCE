namespace Users.API.Middlewares
{
    public class CorrelationIdDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CorrelationIdDelegatingHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_httpContextAccessor.HttpContext?.Items.TryGetValue("CorrelationId", out var correlationId) == true)
            {
                if (correlationId != null)
                {
                    request.Headers.TryAddWithoutValidation("X-Correlation-Id", correlationId.ToString());
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
