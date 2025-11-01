using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pishrooAsp.Data;
using pishrooAsp.Models;

namespace pishrooAsp.Controllers;
[AdminAuthFilter]
public class LangController : Controller
{
	private readonly AppDbContext _context;
	public LangController(AppDbContext context) => _context = context;

	public async Task<IActionResult> Index() =>
		View(await _context.Langs.ToListAsync());

	[HttpPost]
	public async Task<IActionResult> Create(Lang lang)
	{
		if (ModelState.IsValid)
		{
			_context.Langs.Add(lang);
			await _context.SaveChangesAsync();
		}
		return RedirectToAction(nameof(Index));
	}

	[HttpPost]
	public async Task<IActionResult> Delete(int id)
	{
		var lang = await _context.Langs.FindAsync(id);
		if (lang != null)
		{
			_context.Langs.Remove(lang);
			await _context.SaveChangesAsync();
		}
		return RedirectToAction(nameof(Index));
	}
}
