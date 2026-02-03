using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pishrooAsp.Data;
using pishrooAsp.Models;
using System.ComponentModel.DataAnnotations;

[AdminAuthFilter]
public class UserController : Controller
{
	private readonly AppDbContext _context;

	public UserController(AppDbContext context)
	{
		_context = context;
	}

	// لیست کاربران
	public async Task<IActionResult> Index()
	{
		return View(await _context.Users.ToListAsync());
	}

	// افزودن کاربر
	public IActionResult Create()
	{
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> Create(string username, string password, string role)
	{
		if (_context.Users.Any(u => u.Username == username))
		{
			ViewBag.Error = "این نام کاربری قبلاً ثبت شده";
			return View();
		}

		var user = new User
		{
			Username = username,
			PasswordHash = PasswordHelper.Hash(password),
			Role = role
		};

		_context.Users.Add(user);
		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

	// ویرایش
	public async Task<IActionResult> Edit(int id)
	{
		var user = await _context.Users.FindAsync(id);
		return user == null ? NotFound() : View(user);
	}

	[HttpPost]
	public async Task<IActionResult> Edit(int id, string role, bool isActive)
	{
		var user = await _context.Users.FindAsync(id);
		if (user == null) return NotFound();
		
		user.Role = role;
		user.IsActive = isActive;
		await _context.SaveChangesAsync();

		return RedirectToAction(nameof(Index));
	}
	// حذف با AJAX
	[HttpPost]
	public async Task<IActionResult> Delete(int id)
	{
		try
		{
			var user = await _context.Users.FindAsync(id);
			if (user == null)
			{
				return Json(new { success = false, message = "کاربر یافت نشد." });
			}

			// نمی‌توانیم خودمان را حذف کنیم
			var currentUsername = User.Identity?.Name;
			if (user.Username == currentUsername)
			{
				return Json(new { success = false, message = "شما نمی‌توانید حساب خود را حذف کنید." });
			}

			_context.Users.Remove(user);
			await _context.SaveChangesAsync();

			return Json(new { success = true, message = "کاربر با موفقیت حذف شد." });
		}
		catch (Exception ex)
		{
			return Json(new { success = false, message = "خطا در حذف کاربر: " + ex.Message });
		}
	}
	// تغییر رمز عبور - GET
	public async Task<IActionResult> ChangePassword(int id)
	{
		var user = await _context.Users.FindAsync(id);
		if (user == null)
		{
			return NotFound();
		}

		var model = new ChangePasswordViewModel
		{
			UserId = user.Id,
			Username = user.Username
		};

		return View(model);
	}

	// تغییر رمز عبور - POST
	[HttpPost]
	public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		try
		{
			var user = await _context.Users.FindAsync(model.UserId);
			if (user == null)
			{
				return NotFound();
			}

			// اعتبارسنجی رمزهای عبور
			if (model.NewPassword != model.ConfirmPassword)
			{
				ModelState.AddModelError("ConfirmPassword", "رمز عبور و تأیید آن یکسان نیستند");
				return View(model);
			}

			// تغییر رمز عبور
			user.PasswordHash = PasswordHelper.Hash(model.NewPassword);

			_context.Users.Update(user);
			await _context.SaveChangesAsync();

			TempData["Success"] = "رمز عبور با موفقیت تغییر یافت";
			return RedirectToAction(nameof(Index));
		}
		catch (Exception ex)
		{
			ModelState.AddModelError("", $"خطا در تغییر رمز عبور: {ex.Message}");
			return View(model);
		}
	}
}
public class ChangePasswordViewModel
{
	public int UserId { get; set; }

	[Display(Name = "نام کاربری")]
	public string Username { get; set; }

	[Required(ErrorMessage = "رمز عبور جدید الزامی است")]
	[StringLength(100, ErrorMessage = "رمز عبور باید حداقل 6 کاراکتر باشد", MinimumLength = 6)]
	[DataType(DataType.Password)]
	[Display(Name = "رمز عبور جدید")]
	public string NewPassword { get; set; }

	[Required(ErrorMessage = "تأیید رمز عبور الزامی است")]
	[DataType(DataType.Password)]
	[Display(Name = "تأیید رمز عبور")]
	[Compare("NewPassword", ErrorMessage = "رمز عبور و تأیید آن یکسان نیستند")]
	public string ConfirmPassword { get; set; }
}
