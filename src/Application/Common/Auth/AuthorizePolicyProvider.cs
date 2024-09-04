using Application.Common.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
namespace Application.Common.Auth;

public class AuthorizePolicyProvider(IOptions<AuthorizationOptions> options) : IAuthorizationPolicyProvider
{
    public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; } = new DefaultAuthorizationPolicyProvider(options);

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
        FallbackPolicyProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() =>
        Task.FromResult<AuthorizationPolicy>(null!)!;

    public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(AuthorizePolicy.POLICY_PREFIX, StringComparison.OrdinalIgnoreCase))
        {
            var policy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
            policy.AddRequirements(new AuthorizationRequirement(policyName));

            return await Task.FromResult(policy.Build());
        }

        return await FallbackPolicyProvider.GetPolicyAsync(policyName);
    }
}