using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pishrooAsp.Data;
using pishrooAsp.Models.Sms;

[SmartAuthFilter]
public class SmsTemplateController : Controller
{
	private readonly AppDbContext _context;
	public SmsTemplateController(AppDbContext context) => _context = context;

	public async Task<IActionResult> Index()
		=> View(await _context.SmsTemplates.ToListAsync());

	public IActionResult Create() => View();

	[HttpPost]
	public async Task<IActionResult> Create(SmsTemplate model)
	{
		if (!ModelState.IsValid) return View(model);
		_context.SmsTemplates.Add(model);
		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

	public async Task<IActionResult> Edit(int id)
	{
		var template = await _context.SmsTemplates.FindAsync(id);
		if (template == null) return NotFound();
		return View(template);
	}

	[HttpPost]
	public async Task<IActionResult> Edit(int id, SmsTemplate model)
	{
		if (id != model.Id) return NotFound();

		if (!ModelState.IsValid) return View(model);

		var template = await _context.SmsTemplates.FindAsync(id);
		if (template == null) return NotFound();

		template.Title = model.Title;
		template.Body = model.Body;

		_context.Update(template);
		await _context.SaveChangesAsync();

		TempData["SuccessMessage"] = "قالب با موفقیت ویرایش شد.";
		return RedirectToAction(nameof(Index));
	}

	public async Task<IActionResult> Delete(int id)
	{
		var template = await _context.SmsTemplates.FindAsync(id);
		if (template == null) return NotFound();

		return View(template);
	}

	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(int id)
	{
		var template = await _context.SmsTemplates.FindAsync(id);
		if (template == null) return NotFound();

		_context.SmsTemplates.Remove(template);
		await _context.SaveChangesAsync();

		TempData["SuccessMessage"] = "قالب با موفقیت حذف شد.";
		return RedirectToAction(nameof(Index));
	}
}