namespace AirlineTicketsAppWebApi.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

public class SubAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly List<string> _expectedSubs;

    // Accept multiple sub values via params
    public SubAuthorizeAttribute(params string[] expectedSubs)
    {
        _expectedSubs = expectedSubs.ToList();
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        var subClaim = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        // Check if the sub claim exists and matches any of the expected values
        if (subClaim == null || !_expectedSubs.Contains(subClaim))
        {
            context.Result = new ForbidResult();
        }
    }
}
