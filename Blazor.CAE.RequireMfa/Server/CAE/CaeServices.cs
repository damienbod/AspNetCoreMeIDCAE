using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blazor.CAE.RequireMfa.Server;

public class CaeServices
{
    private readonly AuthContextClassAdmin _authContextClassReferencesOperations;
    private readonly IDistributedCache _cache;

    private static readonly object _lock = new();
    private const int cacheExpirationInDays = 1;

    public CaeServices(IDistributedCache cache, 
        AuthContextClassAdmin authContextClassReferencesOperations)
    {
        _authContextClassReferencesOperations = authContextClassReferencesOperations;
        _cache = cache;
    }

    public async Task<Dictionary<string, string>> GetAuthContextValues()
    {
        var acrDict = new Dictionary<string, string>()
        {
            {"C1","Require strong authentication" },
            {"C2","Require compliant devices" },
            {"C3","Require trusted locations" }
        };

        var cacheKey = "ACRS";
        var cachedData = GetFromCache(cacheKey);

        if (cachedData != null)
        {
            acrDict = cachedData;
        }
        else
        {
            var existingAuthContexts = await _authContextClassReferencesOperations
                .ListAuthContextClassReferencesAsync();

            if (existingAuthContexts.Count > 0)
            {
                acrDict.Clear();

                foreach (var authContext in existingAuthContexts)
                {
                    acrDict.Add(authContext.Id, authContext.DisplayName);
                }

                AddToCache(cacheKey, acrDict);
            }
        }

        return acrDict;
    }

    public async Task CreateAuthContextViaGraph()
    {
        var dictACRValues = await GetAuthContextValues();

        foreach (KeyValuePair<string, string> acr in dictACRValues)
        {
            await _authContextClassReferencesOperations.CreateAuthContextClassReferenceAsync(
                acr.Key, acr.Value, $"A new Authentication Context Class Reference created at {DateTime.UtcNow}", true);
        }
    }

    private void AddToCache(string key, Dictionary<string, string> dict)
    {
        var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(cacheExpirationInDays));

        lock (_lock)
        {
            _cache.SetString(key, System.Text.Json.JsonSerializer.Serialize(dict), options);
        }
    }

    private Dictionary<string, string>? GetFromCache(string key)
    {
        var item = _cache.GetString(key);
        if (item != null)
        {
            return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(item);
        }

        return null;
    }
}