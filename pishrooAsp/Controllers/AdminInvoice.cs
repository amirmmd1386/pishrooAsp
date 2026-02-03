using Kavenegar;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pishrooAsp.Data;
using pishrooAsp.Models.Invoice;
using pishrooAsp.Services;

namespace pishrooAsp.Controllers
{
	[AdminAuthFilter]
	public class AdminInvoiceController : Controller
	{
		private readonly AppDbContext _context;
		private readonly IWebHostEnvironment _env;
		private readonly IConfiguration _configuration;
		private readonly IGroupSmsService _smsService;


		public AdminInvoiceController(AppDbContext context, IWebHostEnvironment env, IConfiguration configuration,
			IGroupSmsService smsService)
		{
			_context = context;
			_env = env;
			_configuration = configuration;
			_smsService = smsService;

		}

		// آپلود فاکتور جدید
		public IActionResult Upload()
		{
			// تولید کد پیگیری پیش‌فرض
			ViewBag.DefaultTrackingCode = GenerateTrackingCode();
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Upload(
			string trackingCode,
			string customerName,
			string customerPhone,
			decimal amount,
			string description,
			IFormFile invoiceFile)
		{
			if (invoiceFile == null || invoiceFile.Length == 0)
			{
				ModelState.AddModelError("invoiceFile", "فایل فاکتور را انتخاب کنید");
				return View();
			}

			// بررسی تکراری نبودن کد پیگیری
			if (await _context.Invoices.AnyAsync(p => p.TrackingCode == trackingCode))
			{
				ModelState.AddModelError("trackingCode", "این کد پیگیری قبلاً استفاده شده است");
				ViewBag.DefaultTrackingCode = GenerateTrackingCode();
				return View();
			}

			// بررسی فرمت فایل
			var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
			var extension = Path.GetExtension(invoiceFile.FileName).ToLower();

			if (!allowedExtensions.Contains(extension))
			{
				ModelState.AddModelError("invoiceFile", "فرمت فایل مجاز نیست. فقط PDF و تصویر قابل قبول است");
				ViewBag.DefaultTrackingCode = GenerateTrackingCode();
				return View();
			}

			// تولید نام فایل و توکن
			var fileName = Guid.NewGuid().ToString() + extension;
			var accessToken = GenerateAccessToken();
			var uploadsPath = Path.Combine(_env.WebRootPath, "uploads", "invoices");

			Directory.CreateDirectory(uploadsPath);
			var filePath = Path.Combine(uploadsPath, fileName);

			// ذخیره فایل
			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await invoiceFile.CopyToAsync(stream);
			}

			// ذخیره در دیتابیس
			var invoice = new Invoice
			{
				TrackingCode = trackingCode,
				CustomerName = customerName,
				CustomerPhone = customerPhone,
				Amount = amount,
				Description = description,
				FileName = fileName,
				OriginalFileName = invoiceFile.FileName,
				InvoiceDate = DateTime.Now,
				IsActive = true,
				AccessToken = accessToken
			};

			_context.Invoices.Add(invoice);
			await _context.SaveChangesAsync();

			// تولید لینک پیگیری
			//var trackingUrl = GenerateTrackingUrl(trackingCode);
			//var secureTrackingUrl = GenerateSecureTrackingUrl(invoice.Id, accessToken);

			//// ذخیره در ViewBag برای نمایش
			//ViewBag.TrackingUrl = trackingUrl;
			//ViewBag.SecureTrackingUrl = secureTrackingUrl;
			//ViewBag.TrackingCode = trackingCode;
			//ViewBag.InvoiceId = invoice.Id;
			//ViewBag.SuccessMessage = "فاکتور با موفقیت آپلود شد";

			return RedirectToAction("Index");
		}

		// صفحه موفقیت آپلود
		public IActionResult UploadSuccess()
		{
			return View();
		}

		// ارسال پیامک و کپی لینک
		[HttpPost]
		public async Task<IActionResult> SendSms(int invoiceId, string message = null)
		{
			var invoice = await _context.Invoices.FindAsync(invoiceId);
			if (invoice == null)
			{
				return Json(new { success = false, message = "فاکتور یافت نشد" });
			}

			if (string.IsNullOrEmpty(invoice.CustomerPhone))
			{
				return Json(new { success = false, message = "شماره تلفن مشتری ثبت نشده است" });
			}

			// تولید لینک
			var trackingUrl = GenerateTrackingUrl(invoice.TrackingCode);

			// متن پیش‌فرض پیامک
			var defaultMessage = $"فاکتور شما آماده است\nکد پیگیری: {invoice.TrackingCode}\nمشاهده: {trackingUrl}\nمبلغ: {invoice.Amount.ToString("N0")} تومان";

			var finalMessage =  $"آروین پلیمر{Environment.NewLine}" +
					 $"فاکتور شما آماده است{Environment.NewLine}" +
					 $"کد پیگیری: {invoice.TrackingCode}{Environment.NewLine}" +
					 $"مشاهده: {trackingUrl}{Environment.NewLine}" +
					 $"مبلغ: {invoice.Amount.ToString("N0")} تومان{Environment.NewLine}" +
					 $"با تشکر 🌹";

			try
			{
				var apiKey = "5269784E645955614833434A474C6667694B425832794D746A4235596969434B457353706B72722B79746F3D";
				var sender = "90006210";

				var api = new KavenegarApi(apiKey);
		
				var result = api.Send(sender, invoice.CustomerPhone, finalMessage);

				// بروزرسانی تاریخ ارسال
				invoice.SentDate = DateTime.Now;
				await _context.SaveChangesAsync();

				return Json(new
				{
					success = true,
					message = "پیامک با موفقیت ارسال شد",
					trackingUrl = trackingUrl,
					trackingCode = invoice.TrackingCode
				});
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = $"خطا در ارسال پیامک: {ex.Message}" });
			}
		}

		// دریافت اطلاعات برای نمایش در مدال
		public async Task<IActionResult> GetInvoiceInfo(int id)
		{
			var invoice = await _context.Invoices.FindAsync(id);
			if (invoice == null)
			{
				return Json(new { success = false });
			}

			var trackingUrl = GenerateTrackingUrl(invoice.TrackingCode);
			var secureUrl = GenerateSecureTrackingUrl(invoice.Id, invoice.AccessToken);

			return Json(new
			{
				success = true,
				trackingCode = invoice.TrackingCode,
				trackingUrl = trackingUrl,
				secureUrl = secureUrl,
				customerName = invoice.CustomerName,
				customerPhone = invoice.CustomerPhone,
				amount = invoice.Amount.ToString("N0"),
				sentDate = invoice.SentDate?.ToString("yyyy/MM/dd HH:mm")
			});
		}

		// لیست فاکتورها با امکان ارسال پیامک
		public async Task<IActionResult> Index()
		{
			var invoices = await _context.Invoices
				.OrderByDescending(i => i.InvoiceDate)
				.ToListAsync();

			// تولید لینک برای هر فاکتور
			foreach (var invoice in invoices)
			{
				invoice.TrackingCode = invoice.TrackingCode; // فقط برای اطمینان
				ViewData[$"Url_{invoice.Id}"] = GenerateTrackingUrl(invoice.TrackingCode);
			}

			return View(invoices);
		}

		// متدهای کمکی
		private string GenerateTrackingCode()
		{
			// ترکیبی از حروف و اعداد
			var chars = "ABCDEFGHJKLMNPQRSTUVWXYZ0123456789";
			var random = new Random();
			return new string(Enumerable.Repeat(chars, 8)
				.Select(s => s[random.Next(s.Length)]).ToArray());
		}

		private string GenerateAccessToken()
		{
			var random = new Random();
			return random.Next(100000, 999999).ToString();
		}

		private string GenerateTrackingUrl(string trackingCode)
		{
			var baseUrl = _configuration["AppSettings:BaseUrl"] ?? $"{Request.Scheme}://{Request.Host}";
			return $"{baseUrl}/fa/PublicInvoice/Detail?code={trackingCode}";
		}

		private string GenerateSecureTrackingUrl(int invoiceId, string accessToken)
		{
			var baseUrl = _configuration["AppSettings:BaseUrl"] ?? $"{Request.Scheme}://{Request.Host}";
			return $"{baseUrl}/invoice/view/{invoiceId}?token={accessToken}";
		}




	}
}
