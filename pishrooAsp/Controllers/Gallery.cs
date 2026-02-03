using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pishrooAsp.Data;
using pishrooAsp.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace pishrooAsp.Controllers
{
	public class GalleryController : Controller
	{
		private readonly AppDbContext _context;
		private readonly IWebHostEnvironment _environment;

		public GalleryController(AppDbContext context, IWebHostEnvironment environment)
		{
			_context = context;
			_environment = environment;
		}

		// GET: Gallery
		[AdminAuthFilter]

		public async Task<IActionResult> Index()
		{
			return View(await _context.GalleryImages.ToListAsync());
		}


		// به کنترلر GalleryController این متد را اضافه کنید
		[HttpGet]
		public async Task<IActionResult> getImage(int page = 1, int pageSize = 12)
		{
			try
			{
				var images = await _context.GalleryImages
					.OrderByDescending(g => g.UploadDate)
					.Skip((page - 1) * pageSize)
					.Take(pageSize)
					
					.ToListAsync();
				ViewBag.dir = true;

				return View(images);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { error = "خطا در دریافت تصاویر" });
			}
		}
		[AdminAuthFilter]

		// GET: Gallery/Create
		public IActionResult Create()
		{
			return View();
		}
		[AdminAuthFilter]

		// POST: Gallery/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(IFormFile imageFile, string title, string description)
		{
			if (ModelState.IsValid)
			{
				if (imageFile != null && imageFile.Length > 0)
				{
					// بررسی نوع فایل
					var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
					var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();

					if (!allowedExtensions.Contains(fileExtension))
					{
						ModelState.AddModelError("imageFile", "فرمت فایل مجاز نیست. فقط فایل‌های jpg, jpeg, png, gif قابل قبول هستند.");
						return View();
					}

					// بررسی حجم فایل (حداکثر 5MB)
					if (imageFile.Length > 5 * 1024 * 1024)
					{
						ModelState.AddModelError("imageFile", "حجم فایل نباید بیشتر از 5 مگابایت باشد.");
						return View();
					}

					// ایجاد پوشه اگر وجود ندارد
					var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "gallery");
					if (!Directory.Exists(uploadsFolder))
					{
						Directory.CreateDirectory(uploadsFolder);
					}

					// تولید نام منحصر به فرد برای فایل
					var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
					var filePath = Path.Combine(uploadsFolder, uniqueFileName);

					// ذخیره فایل
					using (var stream = new FileStream(filePath, FileMode.Create))
					{
						await imageFile.CopyToAsync(stream);
					}

					// ایجاد رکورد در دیتابیس
					var galleryImage = new GalleryImage
					{
						Title = title,
						Description = description,
						FileName = uniqueFileName,
						OriginalFileName = imageFile.FileName,
						FilePath = $"/uploads/gallery/{uniqueFileName}",
						FileSize = imageFile.Length,
						UploadDate = DateTime.Now
					};

					_context.GalleryImages.Add(galleryImage);
					await _context.SaveChangesAsync();

					return RedirectToAction(nameof(Index));
				}
				else
				{
					ModelState.AddModelError("imageFile", "لطفاً یک تصویر انتخاب کنید.");
				}
			}

			return View();
		}
		[AdminAuthFilter]

		// GET: Gallery/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var galleryImage = await _context.GalleryImages.FindAsync(id);
			if (galleryImage == null)
			{
				return NotFound();
			}
			return View(galleryImage);
		}
		[AdminAuthFilter]

		// POST: Gallery/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,FileName,OriginalFileName,FilePath,FileSize,UploadDate")] GalleryImage galleryImage)
		{
			if (id != galleryImage.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(galleryImage);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!GalleryImageExists(galleryImage.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			return View(galleryImage);
		}
		[AdminAuthFilter]

		// GET: Gallery/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var galleryImage = await _context.GalleryImages
				.FirstOrDefaultAsync(m => m.Id == id);
			if (galleryImage == null)
			{
				return NotFound();
			}

			return View(galleryImage);
		}
		[AdminAuthFilter]

		// POST: Gallery/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var galleryImage = await _context.GalleryImages.FindAsync(id);
			if (galleryImage != null)
			{
				// حذف فایل فیزیکی
				var filePath = Path.Combine(_environment.WebRootPath, "uploads", "gallery", galleryImage.FileName);
				if (System.IO.File.Exists(filePath))
				{
					System.IO.File.Delete(filePath);
				}

				// حذف رکورد از دیتابیس
				_context.GalleryImages.Remove(galleryImage);
				await _context.SaveChangesAsync();
			}
			return RedirectToAction(nameof(Index));
		}
		[AdminAuthFilter]

		private bool GalleryImageExists(int id)
		{
			return _context.GalleryImages.Any(e => e.Id == id);
		}
	}
}