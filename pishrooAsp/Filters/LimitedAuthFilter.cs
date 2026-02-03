using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

public class LimitedAuthFilter : ActionFilterAttribute
{
	public override void OnActionExecuting(ActionExecutingContext context)
	{
		// اگر در حال حاضر در صفحه Login هستیم، چک نکن
		var actionName = context.RouteData.Values["action"]?.ToString();
		var controllerName = context.RouteData.Values["controller"]?.ToString();

		if (controllerName == "Auth" && actionName == "Login")
		{
			base.OnActionExecuting(context);
			return;
		}

		// هم کوکی AdminAuth و هم LimitedAuth قابل قبول هستند
		if (!context.HttpContext.Request.Cookies.ContainsKey("AdminAuth") &&
			!context.HttpContext.Request.Cookies.ContainsKey("LimitedAuth"))
		{
			context.Result = new RedirectToActionResult("Login", "Auth", null);
			return;
		}
		base.OnActionExecuting(context);
	}
}