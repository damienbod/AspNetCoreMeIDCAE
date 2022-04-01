using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RazorPageCae;

/// <summary>
/// Applications that use enhanced security features like Continuous Access Evaluation (CAE) 
/// and Conditional Access authentication context must be prepared to handle claims challenges.
/// </summary>
public class CAEAdminServices
{
    private readonly GraphAuthContextAdmin _graphAuthContextAdmin;

    public CAEAdminServices(GraphAuthContextAdmin graphAuthContextAdmin)
    {
        _graphAuthContextAdmin = graphAuthContextAdmin;
       
    }

    public Dictionary<string, string> GetSupportedAuthContextValues()
    {
        return new Dictionary<string, string>()
        {
            { AuthContextId.C1,"Require strong authentication" },
            { AuthContextId.C2,"Require compliant devices" },
            { AuthContextId.C3,"Require trusted locations" }
        };
    }

    public async Task<Dictionary<string, string>> GetAuthContextValuesViaGraph()
    {
        var acrDict = new Dictionary<string, string>();

        var existingAuthContexts = await _graphAuthContextAdmin
            .ListAuthContextClassReferencesAsync();

        if (existingAuthContexts.Count > 0)
        {
            foreach (var authContext in existingAuthContexts)
            {
                acrDict.Add(authContext.Id, authContext.DisplayName);
            }
        }

        return acrDict;
    }

    public async Task CreateAuthContextViaGraph(string acrKey, string acrValue)
    {
        await _graphAuthContextAdmin.CreateAuthContextClassReferenceAsync(
            acrKey, 
            acrValue, 
            $"A new Authentication Context Class Reference created at {DateTime.UtcNow}", 
            true);
    }
}