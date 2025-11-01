using Microsoft.AspNetCore.Mvc;
using pishrooAsp.Data;
using pishrooAsp.Models;
using Microsoft.EntityFrameworkCore;

namespace pishrooAsp.Controllers
{
	[AdminAuthFilter]
	public class BackgroundsController : Controller
	{
		private readonly AppDbContext _context;
		private readonly IWebHostEnvironment _env;

		public BackgroundsController(AppDbContext context, IWebHostEnvironment env)
		{
			_context = context;
			_env = env;
		}

		// 📌 لیست همه بکگراندها
		[AdminAuthFilter]
		public async Task<IActionResult> Index()
		{
			var list = await _context.Backgrounds.OrderByDescending(x => x.CreatedAt).ToListAsync();
			return View(list);
		}

		// 📌 نمایش فرم آپلود
		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

		// 📌 آپلود بکگراند
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(IFormFile UploadFile, string? Title)
		{
			if (UploadFile == null || UploadFile.Length == 0)
			{
				TempData["Error"] = "لطفا یک فایل انتخاب کنید.";
				return RedirectToAction(nameof(Create));
			}

			var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads/backgrounds");
			if (!Directory.Exists(uploadsRoot))
				Directory.CreateDirectory(uploadsRoot);

			var fileName = $"{Guid.NewGuid()}{Path.GetExtension(UploadFile.FileName)}";
			var filePath = Path.Combine(uploadsRoot, fileName);

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await UploadFile.CopyToAsync(stream);
			}

			var bg = new Background
			{
				Title = Title,
				FilePath = $"/uploads/backgrounds/{fileName}",
				IsActive = true
			};

			_context.Add(bg);
			await _context.SaveChangesAsync();

			TempData["Success"] = "بکگراند با موفقیت اضافه شد.";
			return RedirectToAction(nameof(Index));
		}

		// 📌 تغییر وضعیت فعال/غیرفعال
		[HttpPost]
		public async Task<IActionResult> Toggle(Guid id)
		{
			var bg = await _context.Backgrounds.FindAsync(id);
			if (bg == null) return NotFound();

			bg.IsActive = !bg.IsActive;
			_context.Update(bg);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}

		// 📌 حذف بکگراند
		[HttpPost]
		public async Task<IActionResult> Delete(Guid id)
		{
			var bg = await _context.Backgrounds.FindAsync(id);
			if (bg == null) return NotFound();

			_context.Backgrounds.Remove(bg);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}
	}
}
