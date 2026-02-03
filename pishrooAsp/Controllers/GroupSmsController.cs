// Controllers/GroupSmsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // اضافه شد
using pishrooAsp.Data;
using pishrooAsp.Models.GroupSms;
using pishrooAsp.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace pishrooAsp.Controllers
{
	[SmartAuthFilter]
	public class GroupSmsController : Controller
	{
		private readonly AppDbContext _context;
		private readonly IGroupSmsService _smsService;
		private readonly ILogger<GroupSmsController> _logger; // اضافه شد
		private readonly IHttpContextAccessor _httpContextAccessor;

		public GroupSmsController(
			AppDbContext context,
			IGroupSmsService smsService,
			ILogger<GroupSmsController> logger,
		IHttpContextAccessor httpContextAccessor) // اضافه شد
		{
			_context = context;
			_smsService = smsService;
			_logger = logger; // اضافه شد
			_httpContextAccessor = httpContextAccessor;

		}




		private string GetCurrentUsername()
		{
			// اول از Authentication بگیر
			if (User.Identity?.IsAuthenticated == true)
			{
				return User.Identity.Name;
			}

			// اگر Authentication کار نکرد، از کوکی‌های قدیمی بگیر
			var usernameCookie = Request.Cookies["UserName"];
			if (!string.IsNullOrEmpty(usernameCookie))
			{
				return usernameCookie;
			}

			// تشخیص از روی نوع کوکی
			if (Request.Cookies.ContainsKey("AdminAuth"))
			{
				return "AdminUser";
			}
			else if (Request.Cookies.ContainsKey("LimitedAuth"))
			{
				return "LimitedUser";
			}

			return "System";
		}

		private string GetCurrentUserRole()
		{
			// از Claims
			var roleClaim = User.FindFirst(ClaimTypes.Role);
			if (roleClaim != null)
			{
				return roleClaim.Value;
			}

			// از کوکی
			return Request.Cookies["UserRole"] ?? "Unknown";
		}

		private int GetCurrentUserId()
		{
			var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
			if (idClaim != null && int.TryParse(idClaim.Value, out var userId))
			{
				return userId;
			}

			var userIdCookie = Request.Cookies["UserId"];
			if (!string.IsNullOrEmpty(userIdCookie) && int.TryParse(userIdCookie, out var id))
			{
				return id;
			}

			return 0;
		}





		private bool IsAdmin()
		{
			return User.IsInRole("Admin") ||
				   Request.Cookies.ContainsKey("AdminAuth") ||
				   User.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
		}

		private bool IsLimitedUser()
		{
			return User.IsInRole("Limited") ||
				   Request.Cookies.ContainsKey("LimitedAuth") ||
				   User.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Limited");
		}

		// GET: لیست کمپین‌ها
		public async Task<IActionResult> Index()
		{
			IQueryable<GroupSmsCampaign> query = _context.GroupSmsCampaigns;

			// اگر ادمین نیست، فقط کمپین‌های حذف نشده رو نشون بده
			if (!IsAdmin())
			{
				query = query.Where(c => !c.IsDeleted);
			}
			// اگر ادمین هست، همه رو نشون بده
			else
			{
				// همه کمپین‌ها (حذف شده و نشده)
			}

			var campaigns = await query
				.OrderByDescending(c => c.CreatedAt)
				.ToListAsync();

			ViewBag.IsAdmin = IsAdmin();
			return View(campaigns);
		}

		// GET: ایجاد کمپین جدید
		public IActionResult Create()
		{
			return View();
		}

		// POST: ایجاد کمپین جدید
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(GroupSmsCampaign campaign)
		{
			if (ModelState.IsValid)
			{
				try
				{

					var currentUser = GetCurrentUsername();
					var currentUserId = GetCurrentUserId();
					var currentUserRole = GetCurrentUserRole();

					// لاگ برای دیباگ
					_logger.LogInformation("User: {User}, ID: {Id}, Role: {Role}",
						currentUser, currentUserId, currentUserRole);

					campaign.CreatedBy = currentUser;
					campaign.CreatedAt = DateTime.UtcNow;
					campaign.IsDeleted = false;

					// پارس کردن شماره‌ها و اعتبارسنجی
					var mobiles = await _smsService.ParseMobiles(campaign.Mobiles);
					if (mobiles.Count == 0)
					{
						ModelState.AddModelError("Mobiles", "شماره موبایل معتبری وارد نشده است");
						return View(campaign);
					}

					campaign.TotalCount = mobiles.Count;
					campaign.Mobiles = string.Join(",", mobiles);
					campaign.Status = SmsStatus.Draft;
					campaign.CreatedAt = DateTime.UtcNow;

					_context.Add(campaign);
					await _context.SaveChangesAsync();

					TempData["Success"] = "کمپین با موفقیت ایجاد شد. آماده ارسال است.";
					return RedirectToAction(nameof(Details), new { id = campaign.Id });
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", $"خطا در ایجاد کمپین: {ex.Message}");
					_logger.LogError(ex, "Error creating SMS campaign");
				}
			}

			return View(campaign);
		}

		// GET: جزئیات کمپین
		public async Task<IActionResult> Details(int id)
		{
			var campaign = await _context.GroupSmsCampaigns
				.Include(c => c.Logs)
				.FirstOrDefaultAsync(c => c.Id == id);

			if (campaign == null)
			{
				return NotFound();
			}

			return View(campaign);
		}

		// POST: ارسال کمپین
		
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Send(int id)
		{
			try
			{
				var result = await _smsService.SendGroupSmsAsync(id);

				if (result.Success)
				{
					TempData["Success"] = result.Message;
				}
				else
				{
					TempData["Error"] = result.Message;

					if (result.Errors.Any())
					{
						TempData["Errors"] = string.Join("<br/>", result.Errors.Take(5));
					}
				}
			}
			catch (Exception ex)
			{
				TempData["Error"] = $"خطا در ارسال: {ex.Message}";
				_logger.LogError(ex, "Error sending SMS campaign {CampaignId}", id);
			}

			return RedirectToAction(nameof(Index));
		}

		// GET: ارسال فوری (ساده)
		public IActionResult SendQuick()
		{
			return View();
		}

		// POST: ارسال فوری
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SendQuick(QuickSendModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{

					string currentUser = GetCurrentUsername().ToString();
					var currentUserId = GetCurrentUserId();
					var currentUserRole = GetCurrentUserRole();

					// لاگ برای دیباگ
					_logger.LogInformation("User: {User}, ID: {Id}, Role: {Role}",
						currentUser, currentUserId, currentUserRole);

					model.CreatedBy = currentUser;
					model.IsDeleted = false;
					var mobiles = await _smsService.ParseMobiles(model.Mobiles);
					if (mobiles.Count == 0)
					{
						ModelState.AddModelError("Mobiles", "شماره موبایل معتبری وارد نشده است");
						return View(model);
					}

					var result = await _smsService.SendImmediateAsync(mobiles, model.Message, currentUser , model.Title);

					if (result.Success)
					{
						TempData["Success"] = result.Message;
						// برای پیدا کردن ID کمپین جدید
						var latestCampaign = await _context.GroupSmsCampaigns
							.Where(c => c.Title == model.Title || (model.Title == null && c.Title.Contains("ارسال گروهی")))
							.OrderByDescending(c => c.Id)
							.FirstOrDefaultAsync();

						if (latestCampaign != null)
						{
							return RedirectToAction(nameof(Details), new { id = latestCampaign.Id });
						}
						return RedirectToAction(nameof(Index));
					}
					else
					{
						TempData["Error"] = result.Message;
					}
				}
				catch (Exception ex)
				{
					TempData["Error"] = $"خطا در ارسال: {ex.Message}";
					_logger.LogError(ex, "Error in quick send");
				}
			}

			return View(model);
		}












		// GET: حذف کمپین
		public async Task<IActionResult> Delete(int id)
		{
			var campaign = await _context.GroupSmsCampaigns.FindAsync(id);
			if (campaign == null)
			{
				return NotFound();
			}

			return View(campaign);
		}

		// POST: حذف کمپین
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var campaign = await _context.GroupSmsCampaigns.FindAsync(id);
			if (!IsAdmin())
			{
				// کاربر عادی: Soft Delete
				campaign.IsDeleted = true;
				campaign.Status = SmsStatus.Deleted;
				campaign.CreatedAt = DateTime.UtcNow;
				campaign.CreatedBy = GetCurrentUsername();
				_context.Update(campaign);

			}
			// اگر ادمین هست، همه رو نشون بده
			else
			{
				_context.GroupSmsCampaigns.Remove(campaign);
			}
			if (campaign != null)
			{
				
				await _context.SaveChangesAsync();

				TempData["Success"] = "کمپین با موفقیت حذف شد";
			}

			return RedirectToAction(nameof(Index));
		}


















		// API برای ارسال سریع (AJAX)
		[HttpPost]
		public async Task<IActionResult> SendQuickApi([FromBody] QuickSendApiModel model)
		{
			try
			{
				var mobiles = await _smsService.ParseMobiles(model.Mobiles);
				if (mobiles.Count == 0)
				{
					return Json(new { success = false, message = "شماره موبایل معتبری وارد نشده است" });
				}

				var result = await _smsService.SendImmediateAsync(mobiles, model.Message, model.Title);

				return Json(new
				{
					success = result.Success,
					message = result.Message,
					total = result.TotalCount,
					sent = result.SentCount,
					failed = result.FailedCount,
					errors = result.Errors.Take(10).ToList() // فقط ۱۰ خطای اول
				});
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = $"خطا: {ex.Message}" });
			}
		}


		// در GroupSmsController.cs اضافه کنید:

		// GET: صفحه اصلی مدیریت گروه‌های تلفن
		public async Task<IActionResult> PhoneGroups()
		{
			var groups = await _context.PhoneGroups
				.Include(g => g.PhoneNumbers)
				.OrderByDescending(g => g.CreatedAt)
				.ToListAsync();

			return View(groups);
		}

		// GET: صفحه ایجاد گروه جدید
		public IActionResult CreatePhoneGroup()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreatePhoneGroup(PhoneGroup model)
		{
			// اینو اول کار چک کن
			model.Description ??= string.Empty;
			model.Name = model.Name?.Trim() ?? string.Empty;

			if (string.IsNullOrEmpty(model.Name))
			{
				ModelState.AddModelError("Name", "نام گروه الزامی است");
				return View(model);
			}

			try
			{
				using var transaction = await _context.Database.BeginTransactionAsync();

				// 1. اول گروه رو ذخیره کن (بدون شماره‌ها)
				var group = new PhoneGroup
				{
					Name = model.Name,
					Description = model.Description,
					CreatedAt = DateTime.UtcNow,
					UpdatedAt = DateTime.UtcNow
				};

				_context.PhoneGroups.Add(group);
				await _context.SaveChangesAsync(); // Id تولید می‌شه

				// 2. حالا شماره‌ها رو ذخیره کن
				if (model.PhoneNumbers != null && model.PhoneNumbers.Any())
				{
					foreach (var phone in model.PhoneNumbers)
					{
						if (!string.IsNullOrWhiteSpace(phone.Number))
						{
							var phoneNumber = new PhoneNumber
							{
								Number = phone.Number.Trim(),
								Name = phone.Name?.Trim() ?? string.Empty,
								PhoneGroupId = group.Id, // اینجا مهمه
								IsActive = true
							};
							_context.PhoneNumber.Add(phoneNumber);
						}
					}
					await _context.SaveChangesAsync();
				}

				await transaction.CommitAsync();

				TempData["Success"] = "گروه تلفن با موفقیت ایجاد شد";
				return RedirectToAction(nameof(PhoneGroups));
			}
			catch (Exception ex)
			{
				// لاگ خطای کامل
				_logger.LogError(ex, "Full error: {Message}, Inner: {InnerException}",
					ex.Message, ex.InnerException?.Message);

				ModelState.AddModelError("", $"خطا: {ex.Message}");
				if (ex.InnerException != null)
				{
					ModelState.AddModelError("", $"جزئیات: {ex.InnerException.Message}");
				}

				return View(model);
			}
		}

		// GET: ویرایش گروه
		public async Task<IActionResult> EditPhoneGroup(int id)
		{
			var group = await _context.PhoneGroups
				.Include(g => g.PhoneNumbers)
				.FirstOrDefaultAsync(g => g.Id == id);

			if (group == null)
			{
				return NotFound();
			}

			return View(group);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditPhoneGroup(int id, PhoneGroup model)
		{
			if (id != model.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					var existingGroup = await _context.PhoneGroups
						.Include(g => g.PhoneNumbers)
						.FirstOrDefaultAsync(g => g.Id == id);

					if (existingGroup == null)
					{
						return NotFound();
					}

					// بررسی تکراری نبودن نام (به جز خودش)
					var duplicateName = await _context.PhoneGroups
						.AnyAsync(g => g.Name == model.Name && g.Id != id);

					if (duplicateName)
					{
						ModelState.AddModelError("Name", "گروه با این نام قبلاً ایجاد شده است");
						return View(model);
					}

					// آپدیت فیلدها
					existingGroup.Name = model.Name;
					existingGroup.Description = model.Description;
					existingGroup.UpdatedAt = DateTime.UtcNow;

					// مدیریت شماره‌ها - پاک کردن شماره‌های حذف شده
					var existingNumbers = existingGroup.PhoneNumbers.ToList();

					// شماره‌های جدید از مدل
					var newNumbers = model.PhoneNumbers ?? new List<PhoneNumber>();

					// پاک کردن شماره‌هایی که در مدل جدید نیستند
					var numbersToRemove = existingNumbers
						.Where(en => !newNumbers.Any(nn => nn.Id == en.Id))
						.ToList();

					foreach (var number in numbersToRemove)
					{
						_context.PhoneNumber.Remove(number);
					}

					// آپدیت یا اضافه کردن شماره‌های جدید
					if (newNumbers != null)
					{
						foreach (var newNumber in newNumbers)
						{
							if (newNumber.Id > 0)
							{
								// شماره موجود - آپدیت شود
								var existingNumber = existingNumbers
									.FirstOrDefault(en => en.Id == newNumber.Id);

								if (existingNumber != null)
								{
									existingNumber.Name = newNumber.Name;
									existingNumber.Number = newNumber.Number;
								}
							}
							else
							{
								// شماره جدید
								newNumber.PhoneGroupId = existingGroup.Id;
							
								_context.PhoneNumber.Add(newNumber);
							}
						}
					}

					_context.Update(existingGroup);
					await _context.SaveChangesAsync();

					TempData["Success"] = "گروه تلفن با موفقیت ویرایش شد";
					return RedirectToAction(nameof(PhoneGroups));
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", $"خطا در ویرایش گروه: {ex.Message}");
					_logger.LogError(ex, "Error editing phone group {Id}", id);
				}
			}

			return View(model);
		}

		// API: دریافت شماره‌های یک گروه برای کپی
		[HttpGet]
		public async Task<IActionResult> GetGroupNumbersJson(int id)
		{
			var group = await _context.PhoneGroups
				.Include(g => g.PhoneNumbers)
				.FirstOrDefaultAsync(g => g.Id == id);

			if (group == null)
			{
				return Json(new { success = false, message = "گروه یافت نشد" });
			}

			var numbers = group.PhoneNumbers
				.Where(p => p.IsActive)
				.Select(p => p.Number)
				.ToList();

			return Json(new
			{
				success = true,
				numbers = numbers,
				count = numbers.Count
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeletePhoneGroup(int id)
		{
			try
			{
				// ابتدا شماره‌های گروه را پیدا کنید
				var group = await _context.PhoneGroups
					.Include(g => g.PhoneNumbers)
					.FirstOrDefaultAsync(g => g.Id == id);

				if (group == null)
				{
					return NotFound();
				}

				// حذف شماره‌ها
				_context.PhoneNumber.RemoveRange(group.PhoneNumbers);

				// حذف گروه
				_context.PhoneGroups.Remove(group);

				await _context.SaveChangesAsync();

				TempData["Success"] = "گروه تلفن با موفقیت حذف شد";
			}
			catch (Exception ex)
			{
				TempData["Error"] = $"خطا در حذف گروه: {ex.Message}";
				_logger.LogError(ex, "Error deleting phone group {Id}", id);
			}

			return RedirectToAction(nameof(PhoneGroups));
		}



		// API: دریافت همه شماره‌های همه گروه‌ها
		[HttpGet]
		public async Task<IActionResult> GetAllPhoneNumbersJson()
		{
			try
			{
				// همه گروه‌ها با شماره‌هاشون رو بگیر
				var groups = await _context.PhoneGroups
					.Include(g => g.PhoneNumbers)
					.Where(g => g.PhoneNumbers.Any())
					.ToListAsync();

				// همه شماره‌ها رو جمع کن
				var allNumbers = new List<string>();
				foreach (var group in groups)
				{
					var numbers = group.PhoneNumbers
						.Where(p => p.IsActive && !string.IsNullOrWhiteSpace(p.Number))
						.Select(p => p.Number.Trim())
						.Distinct(); // شماره‌های تکراری رو حذف کن

					allNumbers.AddRange(numbers);
				}

				return Json(new
				{
					success = true,
					numbers = allNumbers,
					count = allNumbers.Count,
					groupCount = groups.Count
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting all phone numbers");
				return Json(new
				{
					success = false,
					message = ex.Message
				});
			}
		}
	}


	// مدل‌های View
	public class QuickSendModel
	{
		[Display(Name = "عنوان (اختیاری)")]
		public string? Title { get; set; }

		[Required(ErrorMessage = "شماره موبایل‌ها الزامی است")]
		[Display(Name = "شماره موبایل‌ها")]
		[DataType(DataType.MultilineText)]
		public string Mobiles { get; set; }

		[Required(ErrorMessage = "متن پیامک الزامی است")]
		[Display(Name = "متن پیامک")]
		[DataType(DataType.MultilineText)]
		[MaxLength(500, ErrorMessage = "حداکثر ۵۰۰ کاراکتر")]
		public string Message { get; set; }

		// 👇 اضافه کردن فیلد IsDeleted
		[Display(Name = "حذف شده")]
		public bool IsDeleted { get; set; } = false;

		// 👇 اضافه کردن فیلد برای کاربر ایجاد کننده
		[Display(Name = "ایجاد کننده")]
		public string CreatedBy { get; set; } = "System"; // یا از UserId استفاده کنید
	}

	public class QuickSendApiModel
	{
		public string? Title { get; set; }
		public string Mobiles { get; set; }
		public string Message { get; set; }
	}
}