using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pishrooAsp.Data;
using pishrooAsp.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace pishrooAsp.Controllers
{
	
	public class CatalogController : Controller
	{
		private readonly AppDbContext _context;
		private readonly IWebHostEnvironment _environment;

		public CatalogController(AppDbContext context, IWebHostEnvironment environment)
		{
			_context = context;
			_environment = environment;
		}

		// GET: Catalog
		[AdminAuthFilter]

		public async Task<IActionResult> Index()
		{
			var catalogs = await _context.Catalogs
				.OrderByDescending(c => c.UploadDate)
				.ToListAsync();

			return View(catalogs);
		}
	

		public async Task<IActionResult> GetCatalogs()
		{
			var catalogs = await _context.Catalogs
				.OrderByDescending(c => c.UploadDate)
				.ToListAsync();

			return View(catalogs);
		}
		[AdminAuthFilter]

		// GET: Catalog/Create
		public IActionResult Create()
		{
			return View();
		}
		[AdminAuthFilter]

		// POST: Catalog/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(IFormFile catalogFile, string title, string description, bool isActive = true)
		{
			if (ModelState.IsValid)
			{
				if (catalogFile != null && catalogFile.Length > 0)
				{
					// بررسی نوع فایل
					var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };
					var fileExtension = Path.GetExtension(catalogFile.FileName).ToLower();

					if (!allowedExtensions.Contains(fileExtension))
					{
						ModelState.AddModelError("catalogFile", "فرمت فایل مجاز نیست. فقط فایل‌های PDF, Word, JPG, PNG قابل قبول هستند.");
						return View();
					}

					// بررسی حجم فایل (حداکثر 10MB)
					if (catalogFile.Length > 10 * 1024 * 1024)
					{
						ModelState.AddModelError("catalogFile", "حجم فایل نباید بیشتر از 10 مگابایت باشد.");
						return View();
					}

					// ایجاد پوشه اگر وجود ندارد
					var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "catalogs");
					if (!Directory.Exists(uploadsFolder))
					{
						Directory.CreateDirectory(uploadsFolder);
					}

					// تولید نام منحصر به فرد برای فایل
					var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(catalogFile.FileName);
					var filePath = Path.Combine(uploadsFolder, uniqueFileName);

					// ذخیره فایل
					using (var stream = new FileStream(filePath, FileMode.Create))
					{
						await catalogFile.CopyToAsync(stream);
					}

					// ایجاد رکورد در دیتابیس
					var catalog = new Catalog
					{
						Title = title,
						Description = description,
						FileName = uniqueFileName,
						OriginalFileName = catalogFile.FileName,
						FilePath = $"/uploads/catalogs/{uniqueFileName}",
						FileSize = catalogFile.Length,
						UploadDate = DateTime.Now,
						IsActive = isActive
					};

					_context.Catalogs.Add(catalog);
					await _context.SaveChangesAsync();

					TempData["SuccessMessage"] = "کاتالوگ با موفقیت آپلود شد.";
					return RedirectToAction(nameof(Index));
				}
				else
				{
					ModelState.AddModelError("catalogFile", "لطفاً یک فایل انتخاب کنید.");
				}
			}

			return View();
		}
		[AdminAuthFilter]

		// GET: Catalog/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var catalog = await _context.Catalogs.FindAsync(id);
			if (catalog == null)
			{
				return NotFound();
			}
			return View(catalog);
		}
		[AdminAuthFilter]

		// POST: Catalog/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,IsActive")] Catalog catalog, IFormFile? newFile)
		{
			if (id != catalog.Id)
			{
				return NotFound();
			}

			var existingCatalog = await _context.Catalogs.FindAsync(id);
			if (existingCatalog == null)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					// به روزرسانی فیلدها
					existingCatalog.Title = catalog.Title;
					existingCatalog.Description = catalog.Description;
					existingCatalog.IsActive = catalog.IsActive;

					// اگر فایل جدید آپلود شده باشد
					if (newFile != null && newFile.Length > 0)
					{
						// بررسی نوع فایل
						var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };
						var fileExtension = Path.GetExtension(newFile.FileName).ToLower();

						if (!allowedExtensions.Contains(fileExtension))
						{
							ModelState.AddModelError("newFile", "فرمت فایل مجاز نیست.");
							return View(catalog);
						}

						// بررسی حجم فایل
						if (newFile.Length > 10 * 1024 * 1024)
						{
							ModelState.AddModelError("newFile", "حجم فایل نباید بیشتر از 10 مگابایت باشد.");
							return View(catalog);
						}

						// حذف فایل قبلی
						var oldFilePath = Path.Combine(_environment.WebRootPath, "uploads", "catalogs", existingCatalog.FileName);
						if (System.IO.File.Exists(oldFilePath))
						{
							System.IO.File.Delete(oldFilePath);
						}

						// ذخیره فایل جدید
						var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(newFile.FileName);
						var newFilePath = Path.Combine(_environment.WebRootPath, "uploads", "catalogs", uniqueFileName);

						using (var stream = new FileStream(newFilePath, FileMode.Create))
						{
							await newFile.CopyToAsync(stream);
						}

						// به روزرسانی اطلاعات فایل
						existingCatalog.FileName = uniqueFileName;
						existingCatalog.OriginalFileName = newFile.FileName;
						existingCatalog.FilePath = $"/uploads/catalogs/{uniqueFileName}";
						existingCatalog.FileSize = newFile.Length;
					}

					_context.Update(existingCatalog);
					await _context.SaveChangesAsync();

					TempData["SuccessMessage"] = "کاتالوگ با موفقیت ویرایش شد.";
					return RedirectToAction(nameof(Index));
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!CatalogExists(catalog.Id))
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
			return View(catalog);
		}
		[AdminAuthFilter]

		// GET: Catalog/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null) return NotFound();

			var catalog = await _context.Catalogs
				.AsNoTracking()
				.FirstOrDefaultAsync(m => m.Id == id.Value);

			if (catalog == null) return NotFound();

			return View(catalog);
		}
		[AdminAuthFilter]

		// POST: Catalog/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var catalog = await _context.Catalogs.FindAsync(id);
			if (catalog == null)
			{
				TempData["ErrorMessage"] = "کاتالوگ یافت نشد.";
				return RedirectToAction(nameof(Index));
			}

			// بررسی نام فایل - اگر نام فایلت متفاوت است این خط را اصلاح کن (FileName vs OriginalFileName)
			var fileNameToDelete = catalog.FileName ?? catalog.OriginalFileName;

			if (!string.IsNullOrEmpty(fileNameToDelete))
			{
				var uploadsFolder = Path.Combine(_environment.WebRootPath ?? string.Empty, "uploads", "catalogs");
				var filePath = Path.Combine(uploadsFolder, fileNameToDelete);

				try
				{
					if (System.IO.File.Exists(filePath))
					{
						System.IO.File.Delete(filePath);
					}
				}
				catch (Exception ex)
				{
					// اینجا بهتره لاگ بزنی. برای نمایش به کاربر می‌تونی از ModelState یا TempData استفاده کنی:
					TempData["ErrorMessage"] = "خطا در حذف فایل فیزیکی: " + ex.Message;
					return RedirectToAction(nameof(Index));
				}
			}

			try
			{
				_context.Catalogs.Remove(catalog);
				await _context.SaveChangesAsync();
				TempData["SuccessMessage"] = "کاتالوگ با موفقیت حذف شد.";
			}
			catch (Exception ex)
			{
				// لاگ کن و پیام مناسبی نمایش بده
				TempData["ErrorMessage"] = "حذف رکورد با خطا مواجه شد: " + ex.Message;
			}

			return RedirectToAction(nameof(Index));
		}


		[AdminAuthFilter]

		// تغییر وضعیت فعال/غیرفعال
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ToggleStatus(int id)
		{
			var catalog = await _context.Catalogs.FindAsync(id);
			if (catalog != null)
			{
				catalog.IsActive = !catalog.IsActive;
				_context.Update(catalog);
				await _context.SaveChangesAsync();

				TempData["SuccessMessage"] = "وضعیت کاتالوگ با موفقیت تغییر کرد.";
			}
			return RedirectToAction(nameof(Index));
		}
	

		// دانلود فایل
		public async Task<IActionResult> Download(int id)
		{
			var catalog = await _context.Catalogs.FindAsync(id);
			if (catalog == null)
			{
				return NotFound();
			}

			var filePath = Path.Combine(_environment.WebRootPath, "uploads", "catalogs", catalog.FileName);
			if (!System.IO.File.Exists(filePath))
			{
				return NotFound();
			}

			var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
			return File(fileBytes, "application/octet-stream", catalog.OriginalFileName);
		}
		[AdminAuthFilter]

		private bool CatalogExists(int id)
		{
			return _context.Catalogs.Any(e => e.Id == id);
		}
	}
}