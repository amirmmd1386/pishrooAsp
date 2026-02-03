using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pishrooAsp.Data;
using pishrooAsp.Models;
using pishrooAsp.Models.Sms;
using System.Collections.Generic;

namespace pishrooAsp.Controllers
{
[SmartAuthFilter]
	public class CompanySmsController : Controller
	{
		private readonly AppDbContext _context;
		private readonly ISmsSender _smsSender;

		public CompanySmsController(AppDbContext context, ISmsSender smsSender)
		{
			_context = context;
			_smsSender = smsSender;
		}


		// GET: SelectTemplate
		public IActionResult SelectTemplate(int companyId)
		{
			var company = _context.Companies.Find(companyId);
			if (company == null)
				return NotFound();

			ViewBag.CompanyId = companyId;
			var templates = _context.SmsTemplates.ToList();
			return View(templates);
		}

		// GET: Create (پر کردن توکن‌ها)
		public IActionResult Create(int companyId, int templateId)
		{
			var template = _context.SmsTemplates.Find(templateId);
			var company = _context.Companies.Find(companyId);

			if (template == null || company == null)
				return NotFound();

			var tokens = TokenHelper.ExtractTokens(template.Body);
			ViewBag.Tokens = tokens;
			ViewBag.TemplateBody = template.Body;
			ViewBag.CompanyMobile = company.Mobile;
			ViewBag.CompanyName = company.Title;

			return View();
		}

		// POST: ارسال پیامک
		[HttpPost]
		public async Task<IActionResult> Create(int companyId, int templateId, Dictionary<string, string> Tokens)
		{
			var template = await _context.SmsTemplates.FindAsync(templateId);
			var company = await _context.Companies.FindAsync(companyId);

			if (template == null || company == null)
				return NotFound();

			string message = template.Body;
			foreach (var t in Tokens)
			{
				message = message.Replace($"{{{t.Key}}}", t.Value);
			}

			var sends = await _smsSender.SendAsync(company.Mobile, message);
			var log = new CompanySmsLog
			{
				CompanyId = company.Id,
				SmsTemplateId = template.Id,
				FinalMessage = message,
				Mobile = company.Mobile,
				IsSent = true,
				SentAt = DateTime.UtcNow
			};

			_context.CompanySmsLogs.Add(log);
			await _context.SaveChangesAsync();

			TempData["Success"] = "پیامک با موفقیت ارسال شد!";
			return RedirectToAction("Index", "Company");
		}



		// GET: /CompanySms/History/5
		public async Task<IActionResult> History(int companyId)
		{
			var company = await _context.Companies
				.Include(c => c.SmsLogs) // فرض شده یک رابطه Company -> SmsLogs دارید
				.FirstOrDefaultAsync(c => c.Id == companyId);

			if (company == null)
				return NotFound();

			var model = company.SmsLogs
				.Select(s => new CompanySmsHistoryViewModel
				{
					Id = s.Id,
					Message = s.FinalMessage,
					SentAt = s.SentAt,
					Status = s.IsSent
				})
				.OrderByDescending(s => s.SentAt)
				.ToList();

			ViewBag.CompanyName = company.Title;

			return View(model);
		}
	}

}