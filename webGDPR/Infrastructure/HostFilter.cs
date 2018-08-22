//https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters?view=aspnetcore-2.1
//https://docs.microsoft.com/en-us/aspnet/core/razor-pages/filter?view=aspnetcore-2.1

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace webGDPR.Infrastructure
{
    public class HostFilter: IAsyncActionFilter, IAsyncPageFilter
	{
			public async Task OnActionExecutionAsync(
				ActionExecutingContext context,
				ActionExecutionDelegate next)
			{
			// do something before the action executes
			var controller = context.Controller as Controller;
			if (controller == null) return;
			controller.ViewData["Host"] = context.HttpContext.Request.Host.Host;
			var resultContext = await next();
				// do something after the action executes; resultContext.Result will be set
			}

		public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
		{
			// Called asynchronously before the handler method is invoked, after model binding is complete.
			var page = context.HandlerInstance as PageModel;
			if (page == null) return;
			page.ViewData["Host"] = context.HttpContext.Request.Host.Host;
			var resultContext = await next();
		}

		public async Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
		{
			//Called asynchronously after the handler method has been selected, but before model binding occurs.
			await Task.CompletedTask;
		}
	}
	}

