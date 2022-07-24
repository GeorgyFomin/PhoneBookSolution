using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace WebAppClaims.Models
{
    public class AgeHandler : AuthorizationHandler<AgeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            AgeRequirement requirement)
        {
            if (context.User.HasClaim(c => c.Type == ClaimTypes.DateOfBirth))
            {
                int year = 0;
                if (Int32.TryParse(context.User.FindFirst(c => c.Type == ClaimTypes.DateOfBirth)?.Value, out year))
                {
                    if ((DateTime.Now.Year - year) >= requirement.Age)
                    {
                        context.Succeed(requirement);
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
