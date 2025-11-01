using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pishrooAsp.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using pishrooAsp.Data;

namespace pishrooAsp.Controllers
{
	[AdminAuthFilter]
	public class AboutUsController : Controller
	{
		private readonly AppDbContext _context;

		public AboutUsController(AppDbContext context)
		{
			_context = context;
		}

		// GET: AboutUs
		public async Task<IActionResult> Index()
		{
			var data = await _context.AboutUs
				.Include(a => a.Translations)
				.ToListAsync();
			return View(data);
		}

		// GET: AboutUs/Create
		public IActionResult Create()
		{
			ViewBag.Langs = _context.Langs.ToList();
			return View();
		}

		// POST: AboutUs/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(AboutUsItem item, List<AboutUsTranslation> translations)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.Langs = _context.Langs.ToList();
				return View(item);
			}

			try
			{
				// اول ذخیره آیتم اصلی
				_context.AboutUs.Add(item);
				await _context.SaveChangesAsync();

				// ذخیره ترجمه‌ها
				foreach (var tr in translations)
				{
					tr.Id = 0;
					tr.AboutUsId = item.Id;
					_context.AboutUsTranslations.Add(tr);
				}

				await _context.SaveChangesAsync();

				return RedirectToAction(nameof(Index));
			}
			catch (System.Exception ex)
			{
				ModelState.AddModelError("", "خطا در ثبت داده: " + ex.Message);
				ViewBag.Langs = _context.Langs.ToList();
				return View(item);
			}
		}

		// GET: AboutUs/Edit/5
		public async Task<IActionResult> Edit(int id)
		{
			var item = await _context.AboutUs
				.Include(a => a.Translations)
				.FirstOrDefaultAsync(a => a.Id == id);

			if (item == null)
				return NotFound();

			ViewBag.Langs = _context.Langs.ToList();
			ViewBag.translations = _context.AboutUsTranslations.ToList();
			return View(item);
		}

		// POST: AboutUs/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, AboutUsItem item, List<AboutUsTranslation> translations)
		{
			if (id != item.Id)
				return NotFound();

			//if (!ModelState.IsValid)
			//{
			//	ViewBag.Langs = _context.Langs.ToList();
			//	return View(item);
			//}

			try
			{
				// آپدیت آیتم اصلی
				_context.Update(item);

				// حذف ترجمه‌های قبلی
				var oldTranslations = _context.AboutUsTranslations.Where(t => t.AboutUsId == id);
				_context.AboutUsTranslations.RemoveRange(oldTranslations);

				// افزودن ترجمه‌های جدید
				foreach (var tr in translations)
				{
					//tr.Id = 1;
					tr.AboutUsId = item.Id;
					_context.AboutUsTranslations.Add(tr);
				}

				await _context.SaveChangesAsync();

				return RedirectToAction(nameof(Index));
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!_context.AboutUs.Any(e => e.Id == item.Id))
					return NotFound();
				else
					return RedirectToAction(nameof(Create));
			}
		}

		[HttpGet]
		public async Task<IActionResult> Delete(int id)
		{
			var item = await _context.AboutUs
				.Include(a => a.Translations)
				.FirstOrDefaultAsync(a => a.Id == id);

			if (item != null)
			{
				_context.AboutUsTranslations.RemoveRange(item.Translations);
				_context.AboutUs.Remove(item);
				await _context.SaveChangesAsync();
			}

			return RedirectToAction(nameof(Index));
		}


	}
}
