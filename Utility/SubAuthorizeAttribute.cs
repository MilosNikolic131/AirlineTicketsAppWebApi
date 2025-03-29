namespace AirlineTicketsAppWebApi.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

public class SubAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly string _expectedSub;

    public SubAuthorizeAttribute(string expectedSub)
    {
        _expectedSub = expectedSub;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        var subClaim = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (subClaim != _expectedSub)
        {
            context.Result = new ForbidResult();
        }
    }
}
