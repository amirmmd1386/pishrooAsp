using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using pishrooAsp.Data;
using pishrooAsp.ModelViewer.visitLog;

public class VisitService
{
	private readonly AppDbContext _context;

	public VisitService(AppDbContext context)
	{
		_context = context;
	}

	// متد اصلی ثبت بازدید
	public async Task LogVisitAsync(HttpContext httpContext, string path = "")
	{
		try
		{
			var visit = new VisitLog
			{
				IPAddress = GetClientIPAddress(httpContext),
				UserAgent = httpContext.Request.Headers["User-Agent"].ToString(),
				Path = string.IsNullOrEmpty(path) ? httpContext.Request.Path : path,
				DeviceType = GetDeviceType(httpContext.Request.Headers["User-Agent"].ToString()),
				Timestamp = DateTime.Now,
				IsUniqueVisit = await CheckUniqueVisitAsync(httpContext, path)
			};

			_context.VisitLogs.Add(visit);
			await _context.SaveChangesAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine($"خطا در ثبت بازدید: {ex.Message}");
		}
	}

	// متد جدید برای گرفتن آمار
	public async Task<VisitStatistics> GetVisitStatisticsAsync()
	{
		var now = DateTime.Now;

		return new VisitStatistics
		{
			TotalVisits = await _context.VisitLogs.CountAsync(),
			UniqueVisits = await _context.VisitLogs.CountAsync(v => v.IsUniqueVisit),
			TodayVisits = await _context.VisitLogs
				.Where(v => v.Timestamp.Date == now.Date)
				.CountAsync(),
			TodayUniqueVisits = await _context.VisitLogs
				.Where(v => v.Timestamp.Date == now.Date && v.IsUniqueVisit)
				.CountAsync(),
			ThisMonthVisits = await _context.VisitLogs
				.Where(v => v.Timestamp.Year == now.Year && v.Timestamp.Month == now.Month)
				.CountAsync(),
			DeviceStats = await _context.VisitLogs
				.GroupBy(v => v.DeviceType)
				.Select(g => new DeviceStat
				{
					DeviceType = g.Key ?? "نامشخص",
					Count = g.Count(),
					Percentage = (g.Count() * 100.0) / _context.VisitLogs.Count()
				})
				.ToListAsync(),
			PopularPages = await _context.VisitLogs
				.GroupBy(v => v.Path)
				.Select(g => new PageStat
				{
					Path = g.Key,
					VisitCount = g.Count(),
					UniqueVisitCount = g.Count(v => v.IsUniqueVisit)
				})
				.OrderByDescending(p => p.VisitCount)
				.Take(10)
				.ToListAsync()
		};
	}

	// متد برای گرفتن آمار در بازه زمانی خاص
	public async Task<VisitStatistics> GetVisitStatisticsByDateRangeAsync(DateTime startDate, DateTime endDate)
	{
		var visitsInRange = _context.VisitLogs
			.Where(v => v.Timestamp >= startDate && v.Timestamp <= endDate);

		return new VisitStatistics
		{
			TotalVisits = await visitsInRange.CountAsync(),
			UniqueVisits = await visitsInRange.CountAsync(v => v.IsUniqueVisit),
			DeviceStats = await visitsInRange
				.GroupBy(v => v.DeviceType)
				.Select(g => new DeviceStat
				{
					DeviceType = g.Key ?? "نامشخص",
					Count = g.Count(),
					Percentage = (g.Count() * 100.0) / visitsInRange.Count()
				})
				.ToListAsync(),
			PopularPages = await visitsInRange
				.GroupBy(v => v.Path)
				.Select(g => new PageStat
				{
					Path = g.Key,
					VisitCount = g.Count(),
					UniqueVisitCount = g.Count(v => v.IsUniqueVisit)
				})
				.OrderByDescending(p => p.VisitCount)
				.Take(10)
				.ToListAsync(),
			StartDate = startDate,
			EndDate = endDate
		};
	}

	// متد برای گرفتن آمار روزانه (برای نمودار)
	public async Task<List<DailyStat>> GetDailyStatsAsync(int days = 30)
	{
		var startDate = DateTime.Now.AddDays(-days);

		return await _context.VisitLogs
			.Where(v => v.Timestamp >= startDate)
			.GroupBy(v => v.Timestamp.Date)
			.Select(g => new DailyStat
			{
				Date = g.Key,
				TotalVisits = g.Count(),
				UniqueVisits = g.Count(v => v.IsUniqueVisit)
			})
			.OrderBy(d => d.Date)
			.ToListAsync();
	}

	private string GetClientIPAddress(HttpContext httpContext)
	{
		var ipAddress = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();

		if (string.IsNullOrEmpty(ipAddress))
		{
			ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
		}

		return ipAddress ?? "Unknown";
	}

	private string GetDeviceType(string userAgent)
	{
		if (string.IsNullOrEmpty(userAgent))
			return "Unknown";

		userAgent = userAgent.ToLower();

		if (userAgent.Contains("mobile") || userAgent.Contains("android") || userAgent.Contains("iphone"))
			return "Mobile";


		if (userAgent.Contains("tablet") || userAgent.Contains("ipad"))
			return "Tablet";

		return "Desktop";
	}

	private async Task<bool> CheckUniqueVisitAsync(HttpContext httpContext, string path)
	{
		var ipAddress = GetClientIPAddress(httpContext);
		var userAgent = httpContext.Request.Headers["User-Agent"].ToString();

		var twentyFourHoursAgo = DateTime.Now.AddHours(-24);

		var existingVisit = await _context.VisitLogs
			.Where(v => v.IPAddress == ipAddress &&
					   v.UserAgent == userAgent &&
					   v.Path == path &&
					   v.Timestamp >= twentyFourHoursAgo)
			.FirstOrDefaultAsync();

		return existingVisit == null;
	}
}