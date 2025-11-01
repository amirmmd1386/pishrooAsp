// SimpleVisitService.cs
using Microsoft.EntityFrameworkCore;
using pishrooAsp.Data;

public class SimpleVisitService : ISimpleVisitService
{
	private readonly AppDbContext _context;

	public SimpleVisitService(AppDbContext context)
	{
		_context = context;
	}

	public async Task LogVisitAsync(HttpContext context)
	{
		try
		{
			var path = context.Request.Path.ToString().ToLower();

			// از ثبت فایل‌های استاتیک جلوگیری کن
			if (ShouldSkipLogging(path))
				return;

			var ipAddress = GetClientIPAddress(context);
			var userAgent = context.Request.Headers["User-Agent"].ToString();

			var visitLog = new VisitLog
			{
				IPAddress = ipAddress,
				Path = path,
				UserAgent = userAgent,
				DeviceType = GetDeviceType(userAgent),
				Timestamp = DateTime.Now,
				IsUniqueVisit = await IsUniqueVisitAsync(ipAddress, userAgent, path)
			};

			_context.VisitLogs.Add(visitLog);
			await _context.SaveChangesAsync();
		}
		catch (Exception ex)
		{
			// خطا را لاگ کن اما برنامه متوقف نشود
			Console.WriteLine($"Error logging visit: {ex.Message}");
			// می‌تونی از ILogger هم استفاده کنی
		}
	}

	private bool ShouldSkipLogging(string path)
	{
		var excludedExtensions = new[] { ".css", ".js", ".png", ".jpg", ".jpeg", ".gif", ".ico", ".svg", ".woff", ".woff2" };
		var excludedPaths = new[] { "/swagger", "/favicon.ico", "/lib/", "/css/", "/js/", "/images/" };

		return excludedExtensions.Any(ext => path.EndsWith(ext)) ||
			   excludedPaths.Any(excluded => path.Contains(excluded));
	}

	private string GetClientIPAddress(HttpContext context)
	{
		// بررسی هدر‌های مختلف برای IP واقعی
		var ip = context.Connection.RemoteIpAddress?.ToString();

		if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
			ip = context.Request.Headers["X-Forwarded-For"].ToString().Split(',')[0];
		else if (context.Request.Headers.ContainsKey("X-Real-IP"))
			ip = context.Request.Headers["X-Real-IP"].ToString();

		return ip ?? "Unknown";
	}

	private string GetDeviceType(string userAgent)
	{
		if (string.IsNullOrEmpty(userAgent))
			return "Unknown";

		userAgent = userAgent.ToLower();

		if (userAgent.Contains("mobile") || userAgent.Contains("android") || userAgent.Contains("iphone"))
			return "Mobile";
		else if (userAgent.Contains("tablet") || userAgent.Contains("ipad"))
			return "Tablet";
		else
			return "Desktop";
	}

	private async Task<bool> IsUniqueVisitAsync(string ipAddress, string userAgent, string path)
	{
		try
		{
			var last24Hours = DateTime.Now.AddHours(-24);

			var existingVisit = await _context.VisitLogs
				.Where(v => v.IPAddress == ipAddress &&
						   v.UserAgent == userAgent &&
						   v.Path == path &&
						   v.Timestamp >= last24Hours)
				.FirstOrDefaultAsync();

			return existingVisit == null;
		}
		catch
		{
			// در صورت خطا، به عنوان بازدید یونیک در نظر بگیر
			return true;
		}
	}
}