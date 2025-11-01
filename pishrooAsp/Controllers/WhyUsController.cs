using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pishrooAsp.Models.WhyUss;
using pishrooAsp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pishrooAsp.Controllers
{
	[AdminAuthFilter]
	public class WhyUsController : Controller
	{
		private readonly AppDbContext _context;

		public WhyUsController(AppDbContext context)
		{
			_context = context;
		}

		// GET: WhyUs/Index
		public async Task<IActionResult> Index()
		{
			var items = await _context.WhyUs
				.Include(w => w.Translations)
				.ToListAsync();
			
			return View(items);
		}

		// GET: WhyUs/Create
		public IActionResult Create()
		{
			ViewBag.Langs = _context.Langs.ToList();
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(WhyUsItem item, List<WhyUsTranslation> translations)
		{
			try
			{
				// وصل کردن ترجمه‌ها به آیتم
				foreach (var t in translations)
				{
					t.Id = 0; // اگر Id اتوماتیک است
				}
				item.Translations = translations;

				// ذخیره یکجا
				_context.Add(item);
				await _context.SaveChangesAsync();

				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", "خطا در ثبت داده: " + ex.Message);
				ViewBag.Langs = _context.Langs.ToList();
				return View(item);
			}
		}




		// GET: WhyUs/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null) return NotFound();

			var item = await _context.WhyUs
				.Include(w => w.Translations)
				.FirstOrDefaultAsync(w => w.Id == id);

			if (item == null) return NotFound();

			ViewBag.Langs = _context.Langs.ToList();
			return View(item);
		}

		// POST: WhyUs/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, WhyUsItem item, List<WhyUsTranslation> translations)
		{
			if (id != item.Id) return NotFound();

			try
			{
				// بروزرسانی WhyUsItem
				_context.Update(item);

				// حذف ترجمه‌های قبلی
				var oldTranslations = await _context.WhyUsTranslations
					.Where(t => t.whyusId == id)
					.ToListAsync();
				_context.WhyUsTranslations.RemoveRange(oldTranslations);

				// اضافه کردن ترجمه‌های جدید
				foreach (var t in translations)
				{
					t.Id = 0;
					t.whyusId = id;
					_context.Add(t);
				}

				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", "خطا در بروزرسانی: " + ex.Message);
				ViewBag.Langs = _context.Langs.ToList();
				return View(item);
			}
		}

		// GET: WhyUs/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null) return NotFound();

			var item = await _context.WhyUs
				.Include(w => w.Translations)
				.FirstOrDefaultAsync(w => w.Id == id);

			if (item == null) return NotFound();

			return View(item);
		}

		// POST: WhyUs/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var item = await _context.WhyUs.FindAsync(id);

			if (item != null)
			{
				// حذف ترجمه‌ها اول
				var translations = _context.WhyUsTranslations.Where(t => t.whyusId == id);
				_context.WhyUsTranslations.RemoveRange(translations);

				// حذف آیتم
				_context.WhyUs.Remove(item);

				await _context.SaveChangesAsync();
			}

			return RedirectToAction(nameof(Index));
		}
	}
}
