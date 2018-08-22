using System.Threading.Tasks;
using webGDPR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace webGDPR.Authorization
{
    public class UserManagerAuthorizationHandler :
		AuthorizationHandler<OperationAuthorizationRequirement, User>
	{
		protected override Task
			HandleRequirementAsync(AuthorizationHandlerContext context,
								   OperationAuthorizationRequirement requirement,
								   User resource)
		{
			if (context.User == null || resource == null)
			{
				return Task.CompletedTask;
			}

			// If not asking for approval/reject, return.
			if (requirement.Name != Constants.ApproveOperationName &&
				requirement.Name != Constants.RejectOperationName)
			{
				return Task.CompletedTask;
			}

			// Managers can approve or reject.
			if (context.User.IsInRole(Constants.ContactManagersRole))
			{
				context.Succeed(requirement);
			}

			return Task.CompletedTask;
		}
	}
}
