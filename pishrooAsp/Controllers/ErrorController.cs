using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace pishrooAsp.Controllers
{
	public class ErrorController : Controller
	{
		[Route("/{culture}/404")]
		[Route("/404")]
		public IActionResult NotFound404()
		{
			Response.StatusCode = 404;
			return View();
		}

		[Route("/{culture}/error")]
		[Route("/error")]
		public IActionResult Error()
		{
			return View(new ErrorViewModel
			{
				RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
			});
		}
	}

	public class ErrorViewModel
	{
		public string? RequestId { get; set; }
		public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
	}
}