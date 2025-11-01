using Microsoft.EntityFrameworkCore;
using pishrooAsp.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ReportService
{
	private readonly AppDbContext _context;

	public ReportService(AppDbContext context)
	{
		_context = context;
	}

	// آمار بازدید بر اساس نوع دستگاه
	public async Task<Dictionary<string, int>> GetVisitsByDeviceTypeAsync()
	{
		return await _context.VisitLogs
			.GroupBy(l => l.DeviceType)
			.Select(g => new { Device = g.Key, Count = g.Count() })
			.ToDictionaryAsync(x => x.Device, x => x.Count);
	}

	// آمار بازدید در 7 روز گذشته
	public async Task<Dictionary<string, int>> GetDailyVisitsAsync()
	{
		var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);
		return await _context.VisitLogs
			.Where(l => l.Timestamp >= sevenDaysAgo)
			.GroupBy(l => l.Timestamp.Date)
			.Select(g => new { Date = g.Key.ToString("yyyy-MM-dd"), Count = g.Count() })
			.OrderBy(x => x.Date)
			.ToDictionaryAsync(x => x.Date, x => x.Count);
	}

	// ... می‌توانید متدهای دیگری برای آمار IP، مرورگر و ... بنویسید.
}