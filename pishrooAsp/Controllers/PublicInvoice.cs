// Controllers/PublicInvoiceController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pishrooAsp.Data;
using pishrooAsp.Models.Invoice;
using pishrooAsp.ModelViewer.Invoices;
using System.Linq;
using System.Threading.Tasks;

public class PublicInvoiceController : Controller
{
	private readonly AppDbContext _context;
	private readonly IWebHostEnvironment _env;

	public PublicInvoiceController(AppDbContext context, IWebHostEnvironment env)
	{
		_context = context;
		_env = env;
	}

	// صفحه اصلی - GET
	public IActionResult Index()
	{
		var model = new TrackingViewModel();
		return View(model);
	}

	// بررسی کد پیگیری - POST
	[HttpPost]
	public async Task<IActionResult> Index(TrackingViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		// جستجوی فاکتورها بر اساس کد پیگیری
		var invoices = await _context.Invoices
			.Where(i => i.TrackingCode == model.TrackingCode && i.IsActive)
			.OrderByDescending(i => i.InvoiceDate)
			.ToListAsync();

		if (!invoices.Any())
		{
			ModelState.AddModelError("TrackingCode", "فاکتوری با این کد پیگیری یافت نشد");
			model.ShowResults = false;
			return View(model);
		}

		model.Invoices = invoices;
		model.ShowResults = true;

		// ذخیره در TempData برای استفاده در صورت نیاز
		TempData["TrackingCode"] = model.TrackingCode;

		return View(model);
	}

	// دانلود فاکتور
	public async Task<IActionResult> Download(int id, string trackingCode)
	{
		// اعتبارسنجی: فقط فاکتورهای مربوط به این کد پیگیری قابل دانلود باشند
		var invoice = await _context.Invoices
			.FirstOrDefaultAsync(i => i.Id == id && i.TrackingCode == trackingCode && i.IsActive);

		if (invoice == null)
		{
			return NotFound();
		}

		var filePath = Path.Combine(_env.WebRootPath, "uploads", "invoices", invoice.FileName);

		if (!System.IO.File.Exists(filePath))
		{
			return NotFound("فایل فاکتور یافت نشد");
		}

		// خواندن فایل
		var memory = new MemoryStream();
		using (var stream = new FileStream(filePath, FileMode.Open))
		{
			await stream.CopyToAsync(memory);
		}
		memory.Position = 0;

		// تعیین ContentType
		var contentType = GetContentType(filePath);

		return File(memory, contentType, invoice.OriginalFileName ?? invoice.FileName);
	}

	// پیش نمایش فاکتور در مرورگر
	public async Task<IActionResult> Preview(int id, string trackingCode)
	{
		var invoice = await _context.Invoices
			.FirstOrDefaultAsync(i => i.Id == id && i.TrackingCode == trackingCode && i.IsActive);

		if (invoice == null)
		{
			return NotFound();
		}

		var filePath = Path.Combine(_env.WebRootPath, "uploads", "invoices", invoice.FileName);

		if (!System.IO.File.Exists(filePath))
		{
			return NotFound();
		}

		// برای PDF می‌توانیم مستقیماً نمایش دهیم
		if (Path.GetExtension(filePath).ToLower() == ".pdf")
		{
			var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
			return File(bytes, "application/pdf");
		}

		// برای سایر فرمت‌ها دانلود شود
		return RedirectToAction("Download", new { id, trackingCode });
	}

	private string GetContentType(string path)
	{
		var ext = Path.GetExtension(path).ToLowerInvariant();
		return ext switch
		{
			".pdf" => "application/pdf",
			".jpg" or ".jpeg" => "image/jpeg",
			".png" => "image/png",
			".doc" => "application/msword",
			".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
			".xls" => "application/vnd.ms-excel",
			".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
			_ => "application/octet-stream",
		};
	}



	// اضافه کردن این Action به PublicInvoiceController
	public async Task<IActionResult> Track(string code)
	{
		if (string.IsNullOrEmpty(code))
		{
			return RedirectToAction("Index");
		}

		var invoices = await _context.Invoices
			.Where(i => i.TrackingCode == code && i.IsActive)
			.OrderByDescending(i => i.InvoiceDate)
			.ToListAsync();

		if (!invoices.Any())
		{
			TempData["Error"] = "فاکتوری با این کد پیگیری یافت نشد";
			return RedirectToAction("Index");
		}

		var model = new TrackingViewModel
		{
			TrackingCode = code,
			Invoices = invoices,
			ShowResults = true
		};

		return View("Index", model);
	}

	// لینک امن با توکن
	public async Task<IActionResult> View(int id, string token)
	{
		var invoice = await _context.Invoices
			.FirstOrDefaultAsync(i => i.Id == id && i.AccessToken == token && i.IsActive);

		if (invoice == null)
		{
			return NotFound();
		}

		// نمایش مستقیم فاکتور
		var filePath = Path.Combine(_env.WebRootPath, "uploads", "invoices", invoice.FileName);

		if (!System.IO.File.Exists(filePath))
		{
			return NotFound();
		}

		var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

		// تشخیص نوع فایل
		var ext = Path.GetExtension(filePath).ToLower();
		var contentType = ext == ".pdf" ? "application/pdf" :
						 ext == ".jpg" || ext == ".jpeg" ? "image/jpeg" :
						 ext == ".png" ? "image/png" : "application/octet-stream";

		return File(bytes, contentType);
	}

	[HttpGet]
	public async Task<IActionResult> Detail(string code)
	{
		if (string.IsNullOrEmpty(code))
		{
			return NotFound();
		}

		// پیدا کردن فاکتورهای مربوط به این کد پیگیری
		var invoices = await _context.Invoices
			.Where(i => i.TrackingCode == code && i.IsActive)
			.OrderByDescending(i => i.InvoiceDate)
			.ToListAsync();

		if (!invoices.Any())
		{
			return View("InvoiceNotFound", code);
		}

		// ایجاد مدل TrackingViewModel
		var model = new TrackingViewModel
		{
			TrackingCode = code,
			Invoices = invoices,
			ShowResults = true // چون مستقیماً از لینک آمده، نتایج را نشان بده
		};

		return View(model);
	}

}