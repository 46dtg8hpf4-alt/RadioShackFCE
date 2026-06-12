using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Cart.API.Middleware
{
    public class CorrelationIdHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CorrelationIdHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null && context.Items.TryGetValue("CorrelationId", out var correlationId))
            {
                if (correlationId != null)
                {
                    request.Headers.Add("X-Correlation-Id", correlationId.ToString());
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
