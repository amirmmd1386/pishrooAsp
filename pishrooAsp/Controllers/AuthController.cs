using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pishrooAsp.Data;
using System.Security.Claims;

public class AuthController : Controller
{
	private readonly AppDbContext _context;

	public AuthController(AppDbContext context)
	{
		_context = context;
	}

	[HttpGet]
	public IActionResult Login()
	{
		if (User.Identity?.IsAuthenticated == true)
			return RedirectToAction("AdminPanelLink", "Home");

		return View();
	}

	[HttpPost]
	public async Task<IActionResult> Login(string username, string password, bool rememberMe)
	{
		var hash = PasswordHelper.Hash(password);
		var user = await _context.Users
			.FirstOrDefaultAsync(u =>
				u.Username == username &&
				u.PasswordHash == hash &&
				u.IsActive);

		if (user == null)
		{
			ViewBag.Error = "نام کاربری یا رمز عبور اشتباه است";
			return View();
		}

		// ۱. ایجاد Claims
		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.Name, user.Username),
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new Claim(ClaimTypes.Role, user.Role),
			new Claim("UserId", user.Id.ToString()),
			new Claim("FullName", user.Username)
		};

		var identity = new ClaimsIdentity(claims, "CookieAuth");
		var principal = new ClaimsPrincipal(identity);

		// ۲. تنظیمات Authentication
		var authProperties = new AuthenticationProperties
		{
			IsPersistent = rememberMe,
			ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddYears(1) : null
		};

		// ۳. Sign in
		await HttpContext.SignInAsync("CookieAuth", principal, authProperties);

		// ۴. **اینجا کوکی‌های اصلی رو هم ست کن**
		var cookieOptions = new CookieOptions
		{
			HttpOnly = true,
			Secure = true,
			SameSite = SameSiteMode.Strict,
			Expires = rememberMe ? DateTime.Now.AddYears(1) : (DateTime?)null
		};

		// بسته به نقش کاربر، کوکی مناسب رو ست کن
		if (user.Role == "Admin")
		{
			Response.Cookies.Append("AdminAuth", "true", cookieOptions);
		}
		else if (user.Role == "Limited")
		{
			Response.Cookies.Append("LimitedAuth", "true", cookieOptions);
		}

		// کوکی‌های اضافی
		Response.Cookies.Append("UserRole", user.Role, cookieOptions);
		Response.Cookies.Append("UserName", user.Username, cookieOptions);

		// ۵. لاگ لاگین
		_context.UserLoginLogs.Add(new UserLoginLog
		{
			UserId = user.Id,
			Ip = HttpContext.Connection.RemoteIpAddress?.ToString(),
		});
		await _context.SaveChangesAsync();

		return RedirectToAction("AdminPanelLink", "Home");
	}

	public async Task<IActionResult> Logout()
	{
		// ۱. Sign out از Authentication
		await HttpContext.SignOutAsync("CookieAuth");

		// ۲. پاک کردن تمام کوکی‌ها
		Response.Cookies.Delete("AdminAuth");
		Response.Cookies.Delete("LimitedAuth");
		Response.Cookies.Delete("UserRole");
		Response.Cookies.Delete("UserName");

		TempData["SuccessMessage"] = "با موفقیت از حساب خارج شدید.";
		return RedirectToAction("Login");
	}
}