using Microsoft.AspNetCore.Mvc;

namespace pishrooAsp.Controllers
{
	public class Analyze : Controller
	{
		private readonly VisitService _visitService;

		public Analyze(VisitService visitService)
		{
			_visitService = visitService;
		}

		[AdminAuthFilter]
		public async Task<IActionResult> Index()
		{
			var stats = await _visitService.GetVisitStatisticsAsync();
			return View(stats);
		}

	}
}
