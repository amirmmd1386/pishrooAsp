using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

public class AuthController : Controller
{
	private const string AdminUsername = "admin";
	private const string AdminPassword = "123456";
	private const string LimitedUsername = "limited";
	private const string LimitedPassword = "654321";

	[HttpGet]
	public IActionResult Login()
	{
		// فقط اگر لاگین کرده و در صفحه لاگین است، ریدایرکت کن
		// اما اگر از فیلتر آمده، ریدایرکت نکن
		if (Request.Cookies.ContainsKey("AdminAuth"))
		{
			return RedirectToAction("AdminPanelLink", "Home");
		}
		if (Request.Cookies.ContainsKey("LimitedAuth"))
		{
			return RedirectToAction("ShowRequest", "ProductRequests");
		}
		return View();
	}

	[HttpPost]
	public IActionResult Login(string username, string password, bool rememberMe)
	{
		var cookieOptions = new CookieOptions
		{
			HttpOnly = true,
			Secure = true,
			SameSite = SameSiteMode.Strict
		};

		if (rememberMe)
		{
			cookieOptions.Expires = DateTime.Now.AddYears(1);
		}

		if (username == AdminUsername && password == AdminPassword)
		{
			Response.Cookies.Append("AdminAuth", "true", cookieOptions);
			return RedirectToAction("AdminPanelLink", "Home");
		}
		else if (username == LimitedUsername && password == LimitedPassword)
		{
			Response.Cookies.Append("LimitedAuth", "true", cookieOptions);
			return RedirectToAction("ShowRequest", "ProductRequests");
		}

		ViewBag.Error = "نام کاربری یا رمز عبور اشتباه است";
		return View();
	}

	[HttpPost]
	public IActionResult Logout()
	{
		Response.Cookies.Delete("AdminAuth");
		Response.Cookies.Delete("LimitedAuth");
		return RedirectToAction("Login");
	}

	// اضافه کردن این اکشن برای صفحه اصلی بدون فیلتر
	public IActionResult Index()
	{
		return View();
	}
}