using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace QuickEye.WebTools
{
    public class WebLogMessageHandler : DelegatingHandler
    {
        private readonly (string domain, string alias)[] domainAliases;

        public WebLogMessageHandler(HttpMessageHandler innerHandler, string id) : base(innerHandler)
        {
            domainAliases = new (string domain, string alias)[]
            {
                ("http", id)
            };
        }

        public WebLogMessageHandler(HttpMessageHandler innerHandler, IEnumerable<(string domain, string alias)> aliases) :
            base(innerHandler)
        {
            domainAliases = aliases.OrderByDescending(t => t.domain).ToArray();
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            var url = request.RequestUri.OriginalString;
            if (!TryGetId(url, out var id))
                id = url;
            WebLogger.Log(id, response);
            return response;
        }

        private bool TryGetId(string url, out string id)
        {
            foreach (var (domain, alias) in domainAliases)
            {
                if (!url.StartsWith(domain))
                    continue;
                id = alias;
                return true;
            }

            id = default;
            return false;
        }
    }
}