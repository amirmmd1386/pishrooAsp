using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class VisitController : ControllerBase
{
	private readonly VisitService _visitService;

	public VisitController(VisitService visitService)
	{
		_visitService = visitService;
	}

	[HttpPost("log")]
	public async Task<IActionResult> LogVisit([FromBody] VisitRequest request)
	{
		await _visitService.LogVisitAsync(HttpContext, request.Path);
		return Ok(new { message = "بازدید ثبت شد" });
	}

	[HttpGet("stats")]
	public async Task<IActionResult> GetStats()
	{
		var stats = await _visitService.GetVisitStatisticsAsync();
		return Ok(stats);
	}

	[HttpGet("stats/daily")]
	public async Task<IActionResult> GetDailyStats([FromQuery] int days = 30)
	{
		var stats = await _visitService.GetDailyStatsAsync(days);
		return Ok(stats);
	}

	[HttpGet("stats/range")]
	public async Task<IActionResult> GetStatsByDateRange(
		[FromQuery] DateTime startDate,
		[FromQuery] DateTime endDate)
	{
		var stats = await _visitService.GetVisitStatisticsByDateRangeAsync(startDate, endDate);
		return Ok(stats);
	}

	//[HttpGet("recent")]
	//public async Task<IActionResult> GetRecentVisits([FromQuery] int count = 20)
	//{
	//	var recentVisits = await _visitService.get(count);
	//	return Ok(recentVisits);
	//}
}

public class VisitRequest
{
	public string Path { get; set; }
}