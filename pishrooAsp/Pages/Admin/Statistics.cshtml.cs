using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Threading.Tasks;

public class StatisticsModel : PageModel
{
	private readonly ReportService _reportService;

	public StatisticsModel(ReportService reportService)
	{
		_reportService = reportService;
	}

	public string DeviceStatsJson { get; set; }
	public string DailyStatsJson { get; set; }

	public async Task OnGetAsync()
	{
		var deviceStats = await _reportService.GetVisitsByDeviceTypeAsync();
		DeviceStatsJson = JsonSerializer.Serialize(deviceStats);

		var dailyStats = await _reportService.GetDailyVisitsAsync();
		DailyStatsJson = JsonSerializer.Serialize(dailyStats);
	}
}