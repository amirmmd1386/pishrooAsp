using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pishrooAsp.Data;
using pishrooAsp.Models;
using pishrooAsp.Models.Slider;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace pishrooAsp.Controllers
{
	[AdminAuthFilter]
	public class SliderController : Controller
	{
		private readonly AppDbContext _context;
		private readonly IWebHostEnvironment _env;

		public SliderController(AppDbContext context, IWebHostEnvironment env)
		{
			_context = context;
			_env = env;
		}

		// GET: Slider
		public async Task<IActionResult> Index()
		{
			var sliders = await _context.Sliders
				.Include(s => s.Translations)
				.ThenInclude(t => t.Lang)
				.ToListAsync();
			return View(sliders);
		}

		// GET: Slider/Create
		public IActionResult Create()
		{
			ViewBag.Langs = _context.Langs.ToList();
			return View();
		}

		// POST: Slider/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(Slider slider, List<SliderTranslation> translations, IFormFile ImageFile)
		{
			try
			{
				if (ImageFile != null && ImageFile.Length > 0)
				{
					var uploadsPath = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads", "sliders");
					Directory.CreateDirectory(uploadsPath);

					string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
					string filePath = Path.Combine(uploadsPath, fileName);

					using (var stream = new FileStream(filePath, FileMode.Create))
					{
						await ImageFile.CopyToAsync(stream);
					}

					slider.ImageUrl = "/uploads/sliders/" + fileName;
				}

				_context.Sliders.Add(slider);
				await _context.SaveChangesAsync();

				foreach (var tr in translations)
				{
					tr.SliderId = slider.Id;
					_context.SliderTranslations.Add(tr);
				}
				await _context.SaveChangesAsync();

				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				return View();
			}
		}

		// GET: Slider/Edit/5
		public async Task<IActionResult> Edit(int id)
		{
			var slider = await _context.Sliders
				.Include(s => s.Translations)
				.FirstOrDefaultAsync(s => s.Id == id);

			if (slider == null) return NotFound();

			ViewBag.Langs = _context.Langs.ToList();
			return View(slider);
		}

		// POST: Slider/Edit
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, Slider slider, List<SliderTranslation> translations, IFormFile ImageFile)
		{
			try
			{
				if (id != slider.Id) return NotFound();

				var existingSlider = await _context.Sliders
					.Include(s => s.Translations)
					.FirstOrDefaultAsync(s => s.Id == id);

				if (existingSlider == null) return NotFound();

				existingSlider.Link = slider.Link;

				// آپلود عکس جدید
				if (ImageFile != null && ImageFile.Length > 0)
				{
					var uploadsPath = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads", "sliders");
					Directory.CreateDirectory(uploadsPath);

					string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
					string filePath = Path.Combine(uploadsPath, fileName);

					using (var stream = new FileStream(filePath, FileMode.Create))
					{
						await ImageFile.CopyToAsync(stream);
					}

					// حذف عکس قبلی
					if (!string.IsNullOrEmpty(existingSlider.ImageUrl))
					{
						string oldPath = Path.Combine(_env.WebRootPath, existingSlider.ImageUrl.TrimStart('/'));
						if (System.IO.File.Exists(oldPath))
							System.IO.File.Delete(oldPath);
					}

					existingSlider.ImageUrl = "/uploads/sliders/" + fileName;
				}

				// حذف ترجمه‌های قبلی
				_context.SliderTranslations.RemoveRange(existingSlider.Translations);

				// افزودن ترجمه‌های جدید
				foreach (var tr in translations)
				{
					tr.SliderId = existingSlider.Id;
					_context.SliderTranslations.Add(tr);
				}

				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				return View();
			}
		}

		// GET: Slider/Delete/5
		public async Task<IActionResult> Delete(int id)
		{
			var slider = await _context.Sliders
				.Include(s => s.Translations)
				.ThenInclude(t => t.Lang)
				.FirstOrDefaultAsync(s => s.Id == id);

			if (slider == null)
			{
				return NotFound();
			}

			return View(slider);
		}

		// POST: Slider/Delete/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionName("Delete")] // استفاده از ActionName برای تمایز
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var slider = await _context.Sliders
				.Include(s => s.Translations)
				.FirstOrDefaultAsync(s => s.Id == id);

			if (slider != null)
			{
				// حذف عکس از سرور
				if (!string.IsNullOrEmpty(slider.ImageUrl))
				{
					string imagePath = Path.Combine(_env.WebRootPath, slider.ImageUrl.TrimStart('/'));
					if (System.IO.File.Exists(imagePath))
					{
						System.IO.File.Delete(imagePath);
					}
				}

				// حذف ترجمه‌ها
				if (slider.Translations != null && slider.Translations.Any())
				{
					_context.SliderTranslations.RemoveRange(slider.Translations);
				}

				// حذف خود اسلایدر
				_context.Sliders.Remove(slider);
				await _context.SaveChangesAsync();

				TempData["Success"] = "اسلایدر با موفقیت حذف شد.";
			}
			else
			{
				TempData["Error"] = "اسلایدر مورد نظر یافت نشد.";
			}

			return RedirectToAction(nameof(Index));
		}
	}
}