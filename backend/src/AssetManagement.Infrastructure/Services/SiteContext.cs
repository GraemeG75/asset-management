using System;
using System.Collections.Generic;
using AssetManagement.Core.Services;
using Microsoft.AspNetCore.Http;

namespace AssetManagement.Infrastructure.Services
{
    public class SiteContext : ISiteContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DateTime RequestTimestamp { get; } = DateTime.UtcNow;
        public IDictionary<string, object?> Items { get; } = new Dictionary<string, object?>();

        public SiteContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string CurrentLocale
        {
            get
            {
                HttpContext? context = _httpContextAccessor.HttpContext;
                if (context != null)
                {
                    string? queryLocale = context.Request.Query["locale"].ToString();
                    if (!string.IsNullOrWhiteSpace(queryLocale))
                    {
                        return queryLocale;
                    }

                    string? headerLocale = context.Request.Headers["Accept-Language"].ToString();
                    if (!string.IsNullOrWhiteSpace(headerLocale))
                    {
                        string[] parts = headerLocale.Split(',');
                        if (parts.Length > 0 && !string.IsNullOrWhiteSpace(parts[0]))
                        {
                            return parts[0].Trim();
                        }
                    }
                }
                return "en-US";
            }
        }

        public string HttpMethod => _httpContextAccessor.HttpContext?.Request.Method ?? "GET";
        public string RequestPath => _httpContextAccessor.HttpContext?.Request.Path.Value ?? string.Empty;
        public string Host => _httpContextAccessor.HttpContext?.Request.Host.Value ?? "localhost";
        public string ClientIpAddress => _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        public string? UserAgent => _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();
    }
}
