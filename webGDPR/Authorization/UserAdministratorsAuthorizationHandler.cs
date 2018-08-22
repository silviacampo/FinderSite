using System.Threading.Tasks;
using webGDPR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace webGDPR.Authorization
{
    public class UserAdministratorsAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, User>
	{
		protected override Task HandleRequirementAsync(
											  AuthorizationHandlerContext context,
									OperationAuthorizationRequirement requirement,
									 User resource)
		{
			if (context.User == null)
			{
				return Task.CompletedTask;
			}

			// Administrators can do anything.
			if (context.User.IsInRole(Constants.ContactAdministratorsRole))
			{
				context.Succeed(requirement);
			}

			return Task.CompletedTask;
		}
	}
}
