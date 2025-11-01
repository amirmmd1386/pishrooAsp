using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pishrooAsp.Models.Products;
using pishrooAsp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pishrooAsp.Data;

namespace pishrooAsp.Controllers
{
	[AdminAuthFilter]
	public class ProductController : Controller
	{
		private readonly AppDbContext _context;
		private readonly IWebHostEnvironment _env;

		public ProductController(AppDbContext context, IWebHostEnvironment env)
		{
			_context = context;
			_env = env;
		}

		// GET: Product/Manage
		public async Task<IActionResult> Index()
		{
			var products = await _context.Products
				.Include(p => p.Translations)
				.ThenInclude(t => t.Lang)
				.ToListAsync();
			return View(products);
		}

		

		// GET: Product/Create
		public IActionResult Create()
		{
			ViewBag.Languages = _context.Langs.ToList();
			return View();
		}

		// POST: Product/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(Product product, List<ProductTranslation> translations,
			IFormFile ImageFile, IFormFile AttachmentFile,
			IFormFile Usage1File, string Usage1Title,
			IFormFile Usage2File, string Usage2Title,
			IFormFile Usage3File, string Usage3Title,
			IFormFile Usage4File, string Usage4Title,
			IFormFile Usage5File, string Usage5Title,
			  string SeoWord, string SeoDescription)
		{
			try
			{
				product.Id = Guid.NewGuid();
				product.CreatedAt = DateTime.UtcNow;

				product.seoWord = SeoWord;
				product.SeoDiscription = SeoDescription;
				// ذخیره تصویر اصلی
				if (ImageFile != null && ImageFile.Length > 0)
				{
					product.ImageUrl = await SaveFile(ImageFile, "products");
				}

				// ذخیره فایل ضمیمه
				if (AttachmentFile != null && AttachmentFile.Length > 0)
				{
					product.AttachmentUrl = await SaveFile(AttachmentFile, "attachments");
				}

				// ذخیره کاربردها
				await ProcessUsageFile(Usage1File, Usage1Title, product, 1);
				await ProcessUsageFile(Usage2File, Usage2Title, product, 2);
				await ProcessUsageFile(Usage3File, Usage3Title, product, 3);
				await ProcessUsageFile(Usage4File, Usage4Title, product, 4);
				await ProcessUsageFile(Usage5File, Usage5Title, product, 5);

				// ذخیره ترجمه‌ها
				foreach (var translation in translations)
				{
					translation.Id = Guid.NewGuid();
					translation.ProductId = product.Id;
					_context.Add(translation);
				}

				_context.Add(product);
				await _context.SaveChangesAsync();

				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", "خطایی رخ داده: " + ex.Message);
				ViewBag.Languages = _context.Langs.ToList();
				return View(product);
			}
		}

		// GET: Product/Edit/5
		public async Task<IActionResult> Edit(Guid? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var product = await _context.Products
				.Include(p => p.Translations)
				.FirstOrDefaultAsync(p => p.Id == id);

			if (product == null)
			{
				return NotFound();
			}

			ViewBag.Languages = _context.Langs.ToList();
			return View(product);
		}

		// POST: Product/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Guid id, Product product, List<ProductTranslation> translations,
			IFormFile ImageFile, IFormFile AttachmentFile,
			IFormFile Usage1File, string Usage1Title,
			IFormFile Usage2File, string Usage2Title,
			IFormFile Usage3File, string Usage3Title,
			IFormFile Usage4File, string Usage4Title,
			IFormFile Usage5File, string Usage5Title)
		{
			if (id != product.Id)
			{
				return NotFound();
			}

			try
			{
				var productInDb = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
				if (productInDb == null)
				{
					return NotFound();
				}

				// مدیریت تصویر اصلی
				product.ImageUrl = await HandleFileUpdate(ImageFile, productInDb.ImageUrl, "products");

				// مدیریت فایل ضمیمه
				product.AttachmentUrl = await HandleFileUpdate(AttachmentFile, productInDb.AttachmentUrl, "attachments");

				// مدیریت کاربردها
				await ProcessUsageFile(Usage1File, Usage1Title, product, 1, productInDb.Usage1FileUrl);
				await ProcessUsageFile(Usage2File, Usage2Title, product, 2, productInDb.Usage2FileUrl);
				await ProcessUsageFile(Usage3File, Usage3Title, product, 3, productInDb.Usage3FileUrl);
				await ProcessUsageFile(Usage4File, Usage4Title, product, 4, productInDb.Usage4FileUrl);
				await ProcessUsageFile(Usage5File, Usage5Title, product, 5, productInDb.Usage5FileUrl);

				// به روز رسانی محصول
				_context.Update(product);

				// مدیریت ترجمه‌ها
				var existingTranslations = await _context.ProductTranslations
					.Where(t => t.ProductId == id)
					.ToListAsync();
				_context.ProductTranslations.RemoveRange(existingTranslations);

				foreach (var translation in translations)
				{
					translation.Id = Guid.NewGuid();
					translation.ProductId = product.Id;
					_context.Add(translation);
				}

				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ProductExists(product.Id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
		}

		// GET: Product/Usage/5
		public async Task<IActionResult> Usage(Guid? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var product = await _context.Products.FindAsync(id);
			if (product == null)
			{
				return NotFound();
			}

			return View(product);
		}

		// POST: Product/Usage/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Usage(Guid id,
			IFormFile Usage1File, string Usage1Title,
			IFormFile Usage2File, string Usage2Title,
			IFormFile Usage3File, string Usage3Title,
			IFormFile Usage4File, string Usage4Title,
			IFormFile Usage5File, string Usage5Title)
		{
			var product = await _context.Products.FindAsync(id);
			if (product == null)
			{
				return NotFound();
			}

			// مدیریت کاربردها
			await ProcessUsageFile(Usage1File, Usage1Title, product, 1, product.Usage1FileUrl);
			await ProcessUsageFile(Usage2File, Usage2Title, product, 2, product.Usage2FileUrl);
			await ProcessUsageFile(Usage3File, Usage3Title, product, 3, product.Usage3FileUrl);
			await ProcessUsageFile(Usage4File, Usage4Title, product, 4, product.Usage4FileUrl);
			await ProcessUsageFile(Usage5File, Usage5Title, product, 5, product.Usage5FileUrl);

			_context.Update(product);
			await _context.SaveChangesAsync();

			TempData["SuccessMessage"] = "کاربردهای محصول با موفقیت به روز شدند.";
			return RedirectToAction(nameof(Index));
		}

		// سایر متدها (Delete, DeleteConfirmed, ProductExists) بدون تغییر می‌مانند

		// متدهای کمکی
		private async Task<string> SaveFile(IFormFile file, string folderName)
		{
			if (file == null || file.Length == 0) return null;

			var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", folderName);
			if (!Directory.Exists(uploadsFolder))
				Directory.CreateDirectory(uploadsFolder);

			var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
			var filePath = Path.Combine(uploadsFolder, fileName);

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			return $"/uploads/{folderName}/{fileName}";
		}

		private async Task<string> HandleFileUpdate(IFormFile newFile, string existingFileUrl, string folderName)
		{
			if (newFile != null && newFile.Length > 0)
			{
				// حذف فایل قبلی اگر وجود دارد
				if (!string.IsNullOrEmpty(existingFileUrl))
				{
					var oldFilePath = Path.Combine(_env.WebRootPath, existingFileUrl.TrimStart('/'));
					if (System.IO.File.Exists(oldFilePath))
					{
						System.IO.File.Delete(oldFilePath);
					}
				}

				return await SaveFile(newFile, folderName);
			}

			return existingFileUrl;
		}

		private async Task ProcessUsageFile(IFormFile file, string title, Product product, int usageNumber, string existingFileUrl = null)
		{
			var fileUrlProperty = $"Usage{usageNumber}FileUrl";
			var titleProperty = $"Usage{usageNumber}Title";
			var typeProperty = $"Usage{usageNumber}FileType";

			if (file != null && file.Length > 0)
			{
				// حذف فایل قبلی اگر وجود دارد
				if (!string.IsNullOrEmpty(existingFileUrl))
				{
					var oldFilePath = Path.Combine(_env.WebRootPath, existingFileUrl.TrimStart('/'));
					if (System.IO.File.Exists(oldFilePath))
					{
						System.IO.File.Delete(oldFilePath);
					}
				}

				// ذخیره فایل جدید
				var fileUrl = await SaveFile(file, "usages");
				var fileType = Path.GetExtension(file.FileName).ToLower() == ".pdf" ? "pdf" : "image";

				product.GetType().GetProperty(fileUrlProperty)?.SetValue(product, fileUrl);
				product.GetType().GetProperty(typeProperty)?.SetValue(product, fileType);
			}

			if (!string.IsNullOrEmpty(title))
			{
				product.GetType().GetProperty(titleProperty)?.SetValue(product, title);
			}
		}

		private bool ProductExists(Guid id)
		{
			return _context.Products.Any(e => e.Id == id);
		}
	}
}