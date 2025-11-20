using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

[Route("Admin/Upload")]
public class UploadController : Controller
{
	private readonly IWebHostEnvironment _env;

	public UploadController(IWebHostEnvironment env)
	{
		_env = env;
	}

	[HttpPost("UploadImage")]
	public async Task<IActionResult> UploadImage(IFormFile file, string folder = "editor")
	{
		try
		{
			if (file == null || file.Length == 0)
				return Json(new { success = false, message = "فایلی انتخاب نشده است" });

			// بررسی نوع فایل
			var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
			var extension = Path.GetExtension(file.FileName).ToLower();
			if (!allowedExtensions.Contains(extension))
				return Json(new { success = false, message = "فقط فایل‌های تصویری مجاز هستند" });

			// ایجاد پوشه
			var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", folder);
			if (!Directory.Exists(uploadsFolder))
				Directory.CreateDirectory(uploadsFolder);

			// ایجاد نام فایل
			var fileName = $"{Guid.NewGuid()}{extension}";
			var filePath = Path.Combine(uploadsFolder, fileName);

			// ذخیره فایل
			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			var imageUrl = $"/uploads/{folder}/{fileName}";

			return Json(new
			{
				success = true,
				url = imageUrl,
				message = "تصویر با موفقیت آپلود شد"
			});
		}
		catch (Exception ex)
		{
			return Json(new { success = false, message = "خطا در آپلود: " + ex.Message });
		}
	}
}