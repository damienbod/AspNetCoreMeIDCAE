using Microsoft.Graph;
using Microsoft.Identity.Web;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RazorPageCae;

public class MsGraphService
{
    private readonly GraphServiceClient _graphServiceClient;

    public MsGraphService(GraphServiceClient graphServiceClient)
    {
        _graphServiceClient = graphServiceClient;
    }

    public async Task<User> GetGraphApiUser()
    {
        return await _graphServiceClient
            .Me
            .Request()
            .WithScopes("User.ReadBasic.All", "user.read")
            .GetAsync();
    }
}

