using System.Threading.Tasks;
using webGDPR.Data;
using webGDPR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace webGDPR.Authorization
{
    public class UserIsOwnerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, User>
	{
		UserManager<ApplicationUser> _userManager;

		public UserIsOwnerAuthorizationHandler(UserManager<ApplicationUser>
			userManager)
		{
			_userManager = userManager;
		}

		protected override Task
			HandleRequirementAsync(AuthorizationHandlerContext context,
								   OperationAuthorizationRequirement requirement,
								   User resource)
		{
			/*
			 Authorization handlers generally:
			 - Return context.Succeed when the requirements are met.
			 - Return Task.CompletedTask when requirements aren't met. Task.CompletedTask is neither success or failure—it allows other authorization handlers to run.
			 If you need to explicitly fail, return context.Fail.
			 */

			if (context.User == null || resource == null)
			{
				// Return Task.FromResult(0) if targeting a version of
				// .NET Framework older than 4.6:
				return Task.CompletedTask;
			}

			// If we're not asking for CRUD permission, return.

			if (requirement.Name != Constants.CreateOperationName &&
				requirement.Name != Constants.ReadOperationName &&
				requirement.Name != Constants.UpdateOperationName &&
				requirement.Name != Constants.DeleteOperationName)
			{
				return Task.CompletedTask;
			}

			if (resource.OwnerID == _userManager.GetUserId(context.User))
			{
				context.Succeed(requirement);
			}

			return Task.CompletedTask;
		}
	}
}
