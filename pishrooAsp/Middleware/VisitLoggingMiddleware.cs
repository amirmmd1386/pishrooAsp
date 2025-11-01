public class VisitLoggingMiddleware
{
	private readonly RequestDelegate _next;

	public VisitLoggingMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task InvokeAsync(HttpContext context, VisitService visitService)
	{
		// از ثبت بازدید برای فایل‌های استاتیک خودداری کن
		if (!IsStaticFile(context.Request.Path))
		{
			await visitService.LogVisitAsync(context);
		}

		await _next(context);
	}

	private bool IsStaticFile(PathString path)
	{
		var staticExtensions = new[] { ".css", ".js", ".png", ".jpg", ".jpeg", ".gif", ".ico", ".svg", ".woff", ".woff2" };
		return staticExtensions.Any(ext => path.Value.EndsWith(ext));
	}
}

