// SmartAuthFilter.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

public class SmartAuthFilter : ActionFilterAttribute
{
	// لیست کنترلرهایی که برای هر دو گروه قابل دسترسی هستند
	private readonly List<string> _sharedControllers = new List<string>
	{
		"ProductRequests",
		"Company",
		"CompanySms",
		"SmsTemplate",
		"GroupSms",
		"Message"
	};

	public override void OnActionExecuting(ActionExecutingContext context)
	{
		var actionName = context.RouteData.Values["action"]?.ToString();
		var controllerName = context.RouteData.Values["controller"]?.ToString();

		if (controllerName == "Auth" && actionName == "Login")
		{
			base.OnActionExecuting(context);
			return;
		}

		bool hasAdminAuth = context.HttpContext.Request.Cookies.ContainsKey("AdminAuth");
		bool hasLimitedAuth = context.HttpContext.Request.Cookies.ContainsKey("LimitedAuth");

		// اگر ادمین است، اجازه دسترسی به همه دارد
		if (hasAdminAuth)
		{
			base.OnActionExecuting(context);
			return;
		}

		// اگر کاربر محدود است، فقط به کنترلرهای مشخص شده اجازه دسترسی دارد
		if (hasLimitedAuth && _sharedControllers.Contains(controllerName))
		{
			base.OnActionExecuting(context);
			return;
		}

		// اگر به اینجا رسید، یعنی دسترسی ندارد
		context.Result = new RedirectToActionResult("Login", "Auth", null);
	}
}