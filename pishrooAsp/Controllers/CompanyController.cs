using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pishrooAsp.Data;
using pishrooAsp.Models.Sms;

namespace pishrooAsp.Controllers
{
	[SmartAuthFilter]
	public class CompanyController : Controller
	{
		private readonly AppDbContext _context;
		public CompanyController(AppDbContext context) => _context = context;

		public async Task<IActionResult> Index()
			=> View(await _context.Companies.ToListAsync());

		public IActionResult Create() => View();

		[HttpPost]
		public async Task<IActionResult> Create(Company model)
		{
			if (!ModelState.IsValid) return View(model);
			_context.Companies.Add(model);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
		// اکشن Delete
		public async Task<IActionResult> Delete(int id)
		{
			var company = await _context.Companies.FindAsync(id);
			if (company == null)
				return NotFound();

			return View(company);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var company = await _context.Companies.FindAsync(id);
			if (company == null)
				return NotFound();

			_context.Companies.Remove(company);
			await _context.SaveChangesAsync();

			TempData["SuccessMessage"] = "شرکت با موفقیت حذف شد.";
			return RedirectToAction(nameof(Index));
		}
	}
}