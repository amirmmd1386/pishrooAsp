using Microsoft.AspNetCore.Mvc;

namespace pishrooAsp.Controllers
{
	[AdminAuthFilter]
	public class Admin_tpl : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
