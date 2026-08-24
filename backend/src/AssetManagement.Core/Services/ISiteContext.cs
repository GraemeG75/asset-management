using System;
using System.Collections.Generic;

namespace AssetManagement.Core.Services
{
    public interface ISiteContext
    {
        string CurrentLocale { get; }
        string HttpMethod { get; }
        string RequestPath { get; }
        string Host { get; }
        string ClientIpAddress { get; }
        string? UserAgent { get; }
        DateTime RequestTimestamp { get; }
        IDictionary<string, object?> Items { get; }
    }
}
