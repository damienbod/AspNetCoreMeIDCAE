using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Web;

namespace RazorPageCae;

public class GraphAuthContextAdmin
{
    private readonly GraphServiceClient _graphServiceClient;
    private readonly ILogger<GraphAuthContextAdmin> _logger;

    public GraphAuthContextAdmin(
        ILogger<GraphAuthContextAdmin> logger, 
        GraphServiceClient graphServiceClient)
    {
        _graphServiceClient = graphServiceClient;
        _logger = logger;
    }

    public async Task<List<AuthenticationContextClassReference>> ListAuthContextClassReferencesAsync()
    {
        var allAuthenticationContextClassReferences = new List<AuthenticationContextClassReference>();

        try
        {
            var authenticationContextClassreferences = await _graphServiceClient
                .Identity
                .ConditionalAccess
                .AuthenticationContextClassReferences
                .Request()
                .GetAsync();

            if (authenticationContextClassreferences != null)
            {
                allAuthenticationContextClassReferences = await ProcessIAuthenticationContextClassReferenceRootPoliciesCollectionPage(authenticationContextClassreferences);
            }
        }
        catch (ServiceException e)
        {
            _logger.LogWarning("We could not retrieve the existing ACRs: {exception}", e);

            if (e.InnerException != null)
            {
                var exp = (MicrosoftIdentityWebChallengeUserException)e.InnerException;
                throw exp;
            }

            throw e;
        }

        return allAuthenticationContextClassReferences;
    }

    public async Task<AuthenticationContextClassReference?> GetAuthContextClassReferenceByIdAsync(string acrId)
    {
        try
        {
            var acr = await _graphServiceClient
                .Identity
                .ConditionalAccess
                .AuthenticationContextClassReferences[acrId]
                .Request()
                .GetAsync();

            return acr;
        }
        catch (ServiceException gex)
        {
            if (gex.StatusCode != HttpStatusCode.NotFound)
            {
                throw;
            }
        }

        return null;
    }

    public async Task<AuthenticationContextClassReference?> CreateAuthContextClassReferenceAsync(string id, string displayName, string description, bool IsAvailable)
    {
        try
        {
            var acr = await _graphServiceClient
                .Identity
                .ConditionalAccess
                .AuthenticationContextClassReferences
                .Request()
                .AddAsync(new AuthenticationContextClassReference
                {
                    Id = id,
                    DisplayName = displayName,
                    Description = description,
                    IsAvailable = IsAvailable,
                    ODataType = null
                });

            return acr;
        }
        catch (ServiceException e)
        {
            _logger.LogWarning("We could not add a new ACR: {exception}",  e.Error.Message);

            return null;
        }
    }

    public async Task<AuthenticationContextClassReference?> UpdateAuthenticationContextClassReferenceAsync(string acrId, bool IsAvailable, string? displayName = null, string? description = null)
    {
        var acr = await GetAuthContextClassReferenceByIdAsync(acrId);

        if (acr == null)
        {
            throw new ArgumentNullException(nameof(acrId), $"No ACR matching '{acrId}' exists");
        }

        try
        {
            acr = await _graphServiceClient
                .Identity
                .ConditionalAccess
                .AuthenticationContextClassReferences[acrId]
                .Request()
                .UpdateAsync(new AuthenticationContextClassReference
                {
                    Id = acrId,
                    DisplayName = displayName ?? acr.DisplayName,
                    Description = description ?? acr.Description,
                    IsAvailable = IsAvailable,
                    ODataType = null
                });
            return acr;
        }
        catch (ServiceException e)
        {
            _logger.LogWarning("We could not update the ACR: {exception}",  e.Error.Message);

            return null;
        }
    }

    public async Task DeleteAuthenticationContextClassReferenceAsync(string acrId)
    {
        try
        {
            await _graphServiceClient.Identity.ConditionalAccess.AuthenticationContextClassReferences[acrId].Request().DeleteAsync();
        }
        catch (ServiceException e)
        {
            _logger.LogWarning("We could not delete the ACR with Id-{acrId}: {exception}", acrId, e);
        }
    }

    private async Task<List<AuthenticationContextClassReference>> ProcessIAuthenticationContextClassReferenceRootPoliciesCollectionPage(IConditionalAccessRootAuthenticationContextClassReferencesCollectionPage authenticationContextClassreferencesPage)
    {
        var allAuthenticationContextClassReferences = new List<AuthenticationContextClassReference>();

        try
        {
            if (authenticationContextClassreferencesPage != null)
            {
                var pageIterator = PageIterator<AuthenticationContextClassReference>.CreatePageIterator(_graphServiceClient, authenticationContextClassreferencesPage, (authenticationContextClassreference) =>
                {
                    var message = PrintAuthenticationContextClassReference(authenticationContextClassreference);
                    _logger.LogInformation("{message}", message);
                    allAuthenticationContextClassReferences.Add(authenticationContextClassreference);
                    return true;
                });

                await pageIterator.IterateAsync();

                while (pageIterator.State != PagingState.Complete)
                {
                    await pageIterator.ResumeAsync();
                }
            }
        }
        catch (ServiceException e)
        {
            _logger.LogWarning("We could not process the authentication context class references list: {exception}", e);
        }

        return allAuthenticationContextClassReferences;
    }

    private string PrintAuthenticationContextClassReference(AuthenticationContextClassReference authenticationContextClassReference, bool verbose = false)
    {
        var sb = new StringBuilder();

        if (authenticationContextClassReference != null)
        {
            sb.AppendLine($"DisplayName-{authenticationContextClassReference.DisplayName}, IsAvailable-{authenticationContextClassReference.IsAvailable}, Id- '{authenticationContextClassReference.Id}'");

            if (verbose)
            {
                sb.AppendLine($", Description-'{authenticationContextClassReference.Description}'");
            }
        }
        else
        {
            _logger.LogInformation("The provided authenticationContextClassReference is null!");
        }

        return sb.ToString();
    }
}