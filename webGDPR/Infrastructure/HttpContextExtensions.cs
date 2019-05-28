using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;

namespace webGDPR.Infrastructure
{
	public static class HttpContextExtensions
	{
		public static string GetAntiforgeryToken(this HttpContext httpContext)
		{
			var antiforgery = (IAntiforgery)httpContext.RequestServices.GetService(typeof(IAntiforgery));
			var tokenSet = antiforgery.GetAndStoreTokens(httpContext);
			string fieldName = tokenSet.FormFieldName;
			string requestToken = tokenSet.RequestToken;
			return requestToken;
		}
	}
}
