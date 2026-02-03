using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using pishrooAsp.Data;
using pishrooAsp.Models.ProductRequest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class ProductRequestsController : Controller
{
	private readonly AppDbContext _context;
	private readonly IWebHostEnvironment _env;
	private readonly IWebHostEnvironment _environment;


	public ProductRequestsController(AppDbContext context, IWebHostEnvironment env, IWebHostEnvironment environment)
	{
		_context = context;
		_env = env;
		_environment = environment;

	}

	[SmartAuthFilter]
	public async Task<IActionResult> Index(int page = 1)
	{
		int pageSize = 10;
		var requests = await _context.ProductRequests
			.Include(r => r.Files) // اضافه کردن این خط برای لود کردن فایل‌ها
			.OrderByDescending(r => r.CreatedAt)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();

		int totalItems = await _context.ProductRequests.CountAsync();
		ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
		ViewBag.CurrentPage = page;

		var message = await _context.Message.ToListAsync();
		ViewBag.Message = message;
		return View(requests);
	}


	[LimitedAuthFilter]

	public async Task<IActionResult> ShowRequest(int page = 1)
	{
		int pageSize = 10;
		var requests = await _context.ProductRequests
			.Include(r => r.Files) // اضافه کردن این خط برای لود کردن فایل‌ها
			.OrderByDescending(r => r.CreatedAt)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();

		int totalItems = await _context.ProductRequests.CountAsync();
		ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
		ViewBag.CurrentPage = page;

		var message = await _context.Message.ToListAsync();
		ViewBag.Message = message;
		return View(requests);
	}

	// GET: نمایش فرم
	[HttpGet]
	public IActionResult Create()
	{
		var culture = RouteData.Values["culture"]?.ToString() ?? "fa";
		var isRtl = culture.StartsWith("fa");

		// دریافت محصولات بر اساس زبان
		var products = _context.ProductTranslations
		   .Where(p => p.Lang.Code.ToLower() == culture.ToLower())
		   .Select(p => p.Title)
		   .Distinct()
		   .ToList();

		ViewBag.Product = products;
		ViewBag.IsRtl = isRtl;
		ViewBag.Culture = culture;

		// لیبل‌های چندزبانه
		var labels = new Dictionary<string, string>
	{
		{"FormTitle", isRtl ? "فرم ثبت درخواست" : "Product Request Form"},
		{"RequiredField", isRtl ? "فیلدهای ضروری" : "Required fields"},
		{"FullName", isRtl ? "نام و نام خانوادگی" : "Full Name"},
		{"PhoneNumber", isRtl ? "شماره تماس" : "Phone Number"},
		{"CompanyName", isRtl ? "نام شرکت/محل کار" : "Company Name"},
		{"CompanyPhone", isRtl ? "تلفن محل کار" : "Company Phone"},
		{"InjectionMethod", isRtl ? "روش تزریق" : "Injection Method"},
		{"ProductType", isRtl ? "نوع محصول" : "Product Type"},
		{"RequiredAmount", isRtl ? "مقدار مورد نیاز (کیلوگرم)" : "Required Amount (kg)"},
		{"SoftnessHardness", isRtl ? "نرمی یا سفتی" : "Softness or Hardness"},
		{"SoftnessA", isRtl ? "(A) — 0 تا 90" : "(A) — 0 to 90"},
		{"HardnessB", isRtl ? "(D) — 10 تا 50" : "(D) — 10 to 50"},
		{"FileUpload", isRtl ? "ارسال فایل" : "File Upload"},
		{"Description", isRtl ? "توضیحات" : "Description"},
		{"BackToList", isRtl ? "بازگشت به لیست" : "Back to List"},
		{"Submit", isRtl ? "ثبت درخواست" : "Submit Request"},
		{"SelectOption", isRtl ? "انتخاب کنید" : "Select an option"},
		{"NoProductFound", isRtl ? "هیچ محصولی یافت نشد" : "No products found"},
		{"FileRequirements", isRtl ? "فایل‌های مجاز: pdf, jpg, png (حداکثر 10MB)" : "Allowed files: pdf, jpg, png (max 10MB)"},
		{"SoftnessHardnessError", isRtl ? "لطفا فقط یکی از فیلدها را پر کنید" : "Please fill only one field"},
		{"ExampleFullName", isRtl ? "مثال: علی رضایی" : "Example: John Doe"},
		{"ExamplePhone", isRtl ? "0912xxxxxxx" : "0912xxxxxxx"},
		{"ExampleCompany", isRtl ? "در صورت وجود" : "If applicable"},
		{"ExampleAmount", isRtl ? "مثال: 12.5" : "Example: 12.5"}
	};

		ViewBag.Labels = labels;
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> ChangeStatus(ChangeStatusModel model)
	{
		if (model == null) return BadRequest();

		var request = await _context.ProductRequests.FindAsync(model.Id);
		if (request == null) return NotFound();

		request.Status = model.Status;
		_context.Update(request);
		await _context.SaveChangesAsync();

		TempData["Success"] = "وضعیت با موفقیت تغییر کرد";
		return RedirectToAction("Index");
	}

	public class ChangeStatusModel
	{
		public Guid Id { get; set; }
		public RequestStatus Status { get; set; }
	}






	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(ProductRequest model, List<IFormFile> UploadFiles)
	{
		try
		{
			// First save the main entity to get an ID
			model.CreatedAt = DateTime.UtcNow;
			_context.Add(model);
			await _context.SaveChangesAsync(); // This generates the ID

			// Now handle file uploads
			if (UploadFiles != null && UploadFiles.Count > 0)
			{
				var uploadsRoot = Path.Combine(_environment.WebRootPath, "uploads");
				if (!Directory.Exists(uploadsRoot))
					Directory.CreateDirectory(uploadsRoot);

				foreach (var file in UploadFiles)
				{
					if (file.Length > 0)
					{
						var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
						var filePath = Path.Combine(uploadsRoot, fileName);

						using (var stream = new FileStream(filePath, FileMode.Create))
						{
							await file.CopyToAsync(stream);
						}

						// Add file record to database
						var requestFile = new ProductRequestFile
						{
							ProductRequestId = model.Id, // Now this ID exists
							FilePath = $"/uploads/{fileName}"
						};
						_context.ProductRequestFiles.Add(requestFile);
					}
				}

				await _context.SaveChangesAsync();
			}
			var culture = RouteData.Values["culture"]?.ToString() ?? "fa";
			var isRtl = culture.StartsWith("fa");
			ViewBag.IsRtl = isRtl;
			ViewBag.Culture = "fa";
			TempData["Success"] = "اطلاعات با موفقیت ثبت شد.";
			try
			{
				// کلید API که از کاوه نگار گرفتی
				var api = new Kavenegar.KavenegarApi("5269784E645955614833434A474C6667694B425832794D746A4235596969434B457353706B72722B79746F3D");

				// شماره خط فرستنده (مثلاً خط اختصاصی یا خدماتی شما)
				string sender = "90006210";
				string phoneNumber = model.PhoneNumber;
				phoneNumber = phoneNumber.Trim().Replace(" ", "").Replace("-", "");

				// اگر با صفر شروع شد، صفر رو حذف و 98 رو اضافه کن
				if (phoneNumber.StartsWith("0"))
				{
					phoneNumber = "98" + phoneNumber.Substring(1);
				}

				// حالا phoneNumber در قالب صحیح بین‌المللیه
				string receptor = phoneNumber;

				// متن پیام
				string template = $"مشتری گرامی درخواست شما ثبت گردید با شما تماس گرفته میشود";
				string message = $"آروین پلیمر{Environment.NewLine} {template}{Environment.NewLine}با تشکر 🌹";



				var result = api.Send(sender, receptor, message);

			}
			catch (Kavenegar.Exceptions.ApiException ex)
			{
				// خطای API
				Console.WriteLine("Message : " + ex.Message);
			}
			catch (Kavenegar.Exceptions.HttpException ex)
			{
				// خطای شبکه یا ارتباط
				Console.WriteLine("Message : " + ex.Message);
			}
			try
			{
				var apiKey = "5269784E645955614833434A474C6667694B425832794D746A4235596969434B457353706B72722B79746F3D";
				var receptor = "989123633723";

				var api = new Kavenegar.KavenegarApi(apiKey);

				// استفاده از Template آماده در پنل
				var templateName = "ArvinPolymer"; // همون اسم تمپلیتی که در کاوه نگار ساختی
				var res = api.VerifyLookup(
					receptor,
					model.FullName, // این میشه {token} اول
					model.PhoneNumber,        // token2
					null,        // token3
					null,        // token10
					templateName,
					Kavenegar.Models.Enums.VerifyLookupType.Sms
				);
			}
			catch (Kavenegar.Exceptions.ApiException ex)
			{
				Console.WriteLine("Message : " + ex.Message);
			}
			catch (Kavenegar.Exceptions.HttpException ex)
			{
				Console.WriteLine("Message : " + ex.Message);
			}
			return RedirectToAction("Index", "Home");
		}
		catch (Exception ex)
		{
			TempData["Error"] = "خطا در ثبت اطلاعات: " + ex.Message;
			return View(model);
		}
	}


	[SmartAuthFilter]

	// نمایش جزئیات
	public async Task<IActionResult> Details(Guid id)
	{
		var request = await _context.ProductRequests.FindAsync(id);
		if (request == null) return NotFound();

		var files = await _context.ProductRequestFiles
	.Where(p => p.ProductRequestId == id) // نه p.Id == id
	.ToListAsync();

		ViewBag.Files = files; // با حرف بزرگ
		return View(request);
	}



	[LimitedAuthFilter]
	// نمایش جزئیات
	public async Task<IActionResult> DetailsShow(Guid id)
	{
		var request = await _context.ProductRequests.FindAsync(id);
		if (request == null) return NotFound();

		var files = await _context.ProductRequestFiles
	.Where(p => p.ProductRequestId == id) // نه p.Id == id
	.ToListAsync();

		ViewBag.Files = files; // با حرف بزرگ
		return View(request);
	}


	// حذف
	[HttpPost]
	public async Task<IActionResult> Delete(Guid id)
	{
		var request = await _context.ProductRequests.FindAsync(id);
		if (request == null) return NotFound();

		_context.ProductRequests.Remove(request);
		await _context.SaveChangesAsync();

		TempData["Success"] = "درخواست حذف شد";
		return RedirectToAction(nameof(Index));
	}



	// اضافه کردن این متدها به کنترلر ProductRequestsController
	//[HttpPost]
	//public async Task<IActionResult> SendTemplateMessage([FromBody] SendMessageModel model)
	//{
	//	try
	//	{
	//		var request = await _context.ProductRequests
	//			.FirstOrDefaultAsync(r => r.Id == model.requestId);

	//		if (request == null)
	//		{
	//			return Json(new { success = false, message = "درخواست یافت نشد" });
	//		}

	//		var messageTemplate = await _context.Message.FindAsync(model.messageId);
	//		if (messageTemplate == null)
	//		{
	//			return Json(new { success = false, message = "پیام مورد نظر یافت نشد" });
	//		}

	//		string phoneNumber = request.PhoneNumber;
	//		phoneNumber = phoneNumber.Trim().Replace(" ", "").Replace("-", "");

	//		// فرمت شماره تلفن
	//		if (phoneNumber.StartsWith("0"))
	//		{
	//			phoneNumber = "98" + phoneNumber.Substring(1);
	//		}

	//		// ارسال پیامک
	//		var api = new Kavenegar.KavenegarApi("717043746A4B6E44666F66445639547A414334526674515A794A55583168383962773977554F56774977733D");

	//		string sender = "10000099990090";
	//		string receptor = phoneNumber;
	//		string template = $"آروین پلیمر{Environment.NewLine}سفارش شما در مرحله: {messageTemplate.Message}{Environment.NewLine}با تشکر 🌹"; var result = api.Send(sender, receptor, template);

	//		// ذخیره تاریخ آخرین پیام ارسالی
	//		request.LastMessageSentAt = DateTime.UtcNow;
	//		_context.Update(request);
	//		await _context.SaveChangesAsync();

	//		return Json(new { success = true, message = "پیام با موفقیت ارسال شد" });
	//	}
	//	catch (Kavenegar.Exceptions.ApiException ex)
	//	{
	//		return Json(new { success = false, message = "خطا در ارسال پیامک: " + ex.Message });
	//	}
	//	catch (Kavenegar.Exceptions.HttpException ex)
	//	{
	//		return Json(new { success = false, message = "خطای شبکه در ارسال پیامک: " + ex.Message });
	//	}
	//	catch (Exception ex)
	//	{
	//		return Json(new { success = false, message = "خطا: " + ex.Message });
	//	}
	//}


	[HttpPost]
	public async Task<IActionResult> SendTemplateMessage([FromBody] SendMessageModel model)
	{
		try
		{
			var request = await _context.ProductRequests
				.FirstOrDefaultAsync(r => r.Id == model.requestId);

			if (request == null)
			{
				return Json(new { success = false, message = "درخواست یافت نشد" });
			}

			var messageTemplate = await _context.Message.FindAsync(model.messageId);
			if (messageTemplate == null)
			{
				return Json(new { success = false, message = "پیام مورد نظر یافت نشد" });
			}

			string phoneNumber = request.PhoneNumber;
			phoneNumber = phoneNumber.Trim().Replace(" ", "").Replace("-", "");

			// فرمت شماره تلفن
			if (phoneNumber.StartsWith("0"))
			{
				phoneNumber = "98" + phoneNumber.Substring(1);
			}

			// ارسال پیامک
			var api = new Kavenegar.KavenegarApi("5269784E645955614833434A474C6667694B425832794D746A4235596969434B457353706B72722B79746F3D");

			string sender = "10000099990090";
			string receptor = phoneNumber;
			string template = $"آروین پلیمر{Environment.NewLine} {messageTemplate.Message}{Environment.NewLine}با تشکر 🌹";

			string status = "Sent";
			try
			{
				var result = api.Send(sender, receptor, template);
				status = "Sent";
			}
			catch (Exception)
			{
				status = "Failed";
				throw; // خطا به کتچ بالاتر منتقل می‌شود
			}

			// ✅ ثبت تاریخچه پیام در جدول رابط
			var productRequestMessage = new ProductRequestMessage
			{
				ProductRequestId = model.requestId,
				MessageId = model.messageId,
				SentAt = DateTime.UtcNow,
				Status = status
			};

			_context.ProductRequestMessages.Add(productRequestMessage);

			// ذخیره تاریخ آخرین پیام ارسالی
			request.LastMessageSentAt = DateTime.UtcNow;
			_context.Update(request);

			await _context.SaveChangesAsync();

			return Json(new { success = true, message = "پیام با موفقیت ارسال شد" });
		}
		catch (Kavenegar.Exceptions.ApiException ex)
		{
			return Json(new { success = false, message = "خطا در ارسال پیامک: " + ex.Message });
		}
		catch (Kavenegar.Exceptions.HttpException ex)
		{
			return Json(new { success = false, message = "خطای شبکه در ارسال پیامک: " + ex.Message });
		}
		catch (Exception ex)
		{
			return Json(new { success = false, message = "خطا: " + ex.Message });
		}
	}

	public class SendCustomMessageModel
	{
		public Guid requestId { get; set; }
		public string messageContent { get; set; }
	}

	// نمایش تاریخچه پیام‌های یک درخواست
	public async Task<IActionResult> MessageHistory(Guid id)
	{
		var messageHistory = await _context.ProductRequestMessages
			.Include(p => p.ProductRequest)
			.Include(p => p.Message)
			.Where(p => p.ProductRequestId == id)
			.OrderByDescending(p => p.SentAt)
			.ToListAsync();

		ViewBag.RequestId = id;
		return View(messageHistory);
	}
	public class SendMessageModel
	{
		public Guid requestId { get; set; }
		public int messageId { get; set; }
	}

	[HttpPost]
	public async Task<IActionResult> SendCustomMessage(Guid requestId, string messageContent)
	{
		try
		{
			var request = await _context.ProductRequests
				.FirstOrDefaultAsync(r => r.Id == requestId);

			if (request == null)
			{
				return Json(new { success = false, message = "درخواست یافت نشد" });
			}

			string phoneNumber = request.PhoneNumber;
			phoneNumber = phoneNumber.Trim().Replace(" ", "").Replace("-", "");

			// فرمت شماره تلفن
			if (phoneNumber.StartsWith("0"))
			{
				phoneNumber = "98" + phoneNumber.Substring(1);
			}

			// ارسال پیامک
			var api = new Kavenegar.KavenegarApi("5269784E645955614833434A474C6667694B425832794D746A4235596969434B457353706B72722B79746F3D");

			string sender = "10000099990090";
			string receptor = phoneNumber;

			var result = api.Send(sender, receptor, messageContent);

			// ذخیره تاریخ آخرین پیام ارسالی
			request.LastMessageSentAt = DateTime.UtcNow;
			_context.Update(request);
			await _context.SaveChangesAsync();

			return Json(new { success = true, message = "پیام با موفقیت ارسال شد" });
		}
		catch (Kavenegar.Exceptions.ApiException ex)
		{
			return Json(new { success = false, message = "خطا در ارسال پیامک: " + ex.Message });
		}
		catch (Kavenegar.Exceptions.HttpException ex)
		{
			return Json(new { success = false, message = "خطای شبکه در ارسال پیامک: " + ex.Message });
		}
		catch (Exception ex)
		{
			return Json(new { success = false, message = "خطا: " + ex.Message });
		}
	}

	public class SendMessageModels
	{
		public Guid requestId { get; set; }
		public int messageId { get; set; }
	}

}
