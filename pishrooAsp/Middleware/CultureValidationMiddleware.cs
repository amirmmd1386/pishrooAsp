// Middlewares/CultureValidationMiddleware.cs
using pishrooAsp.Services;

namespace pishrooAsp.Middlewares;

public class CultureValidationMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ICultureService _cultureService;

	public CultureValidationMiddleware(RequestDelegate next, ICultureService cultureService)
	{
		_next = next;
		_cultureService = cultureService;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		// ۱. گرفتن culture از Route
		var routeCulture = context.GetRouteValue("culture")?.ToString();

		// ۲. اگر culture در Route وجود داشت
		if (!string.IsNullOrEmpty(routeCulture))
		{
			// ۳. بررسی وجود culture در دیتابیس
			bool isValidCulture = _cultureService.IsCultureExists(routeCulture);

			// ۴. اگر culture معتبر نبود
			if (!isValidCulture)
			{
				// ۵. ساخت URL جدید با culture=fa
				var path = context.Request.Path.ToString();

				// حذف culture نامعتبر از اول مسیر
				var newPath = path;
				if (path.StartsWith($"/{routeCulture}"))
				{
					newPath = path.Substring(routeCulture.Length + 1); // +1 برای /
				}

				// ۶. اضافه کردن fa به ابتدای مسیر
				if (!newPath.StartsWith("/fa"))
				{
					newPath = $"/fa{newPath}";
				}

				// ۷. اضافه کردن query string
				var query = context.Request.QueryString;
				newPath = $"{newPath}{query}";

				// ۸. هدایت به URL جدید
				context.Response.Redirect(newPath);
				return;
			}
		}

		await _next(context);
	}
}