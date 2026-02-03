// Services/GroupSmsService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kavenegar;
using Kavenegar.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using pishrooAsp.Data;
using pishrooAsp.Models.GroupSms;

namespace pishrooAsp.Services
{
	public interface IGroupSmsService
	{
		Task<SendGroupSmsResult> SendGroupSmsAsync(int campaignId);
		Task<SendGroupSmsResult> SendImmediateAsync(List<string> mobiles, string message, string currentUser, string campaignTitle = null);
		Task<List<string>> ParseMobiles(string mobileInput);
	}

	public class SendGroupSmsResult
	{
		public bool Success { get; set; }
		public string Message { get; set; }
		public int TotalCount { get; set; }
		public int SentCount { get; set; }
		public int FailedCount { get; set; }
		public List<string> Errors { get; set; } = new List<string>();
		public decimal? TotalCost { get; set; }
	}

	public class GroupSmsService : IGroupSmsService
	{
		private readonly AppDbContext _context;
		private readonly IConfiguration _configuration;
		private readonly ILogger<GroupSmsService> _logger; // اضافه شد

		public GroupSmsService(
			AppDbContext context,
			IConfiguration configuration,
			ILogger<GroupSmsService> logger) // اضافه شد
		{
			_context = context;
			_configuration = configuration;
			_logger = logger; // اضافه شد
		}

		public async Task<List<string>> ParseMobiles(string mobileInput)
		{
			if (string.IsNullOrWhiteSpace(mobileInput))
				return new List<string>();

			// جدا کردن با کاما، خط جدید، فاصله یا نقطه ویرگول
			var separators = new[] { ',', ';', '\n', '\r', '\t', ' ' };
			var mobiles = mobileInput
				.Split(separators, StringSplitOptions.RemoveEmptyEntries)
				.Select(m => m.Trim())
				.Where(m => !string.IsNullOrWhiteSpace(m))
				.Distinct()
				.ToList();

			// حذف مواردی که شماره موبایل نیستند
			var validMobiles = new List<string>();
			foreach (var mobile in mobiles)
			{
				var cleanMobile = CleanMobileNumber(mobile);
				if (IsValidMobile(cleanMobile))
				{
					validMobiles.Add(cleanMobile);
				}
			}

			return validMobiles;
		}

		public async Task<SendGroupSmsResult> SendImmediateAsync(List<string> mobiles, string message,  string currentUser,string campaignTitle = null)
		{
			var result = new SendGroupSmsResult
			{
				TotalCount = mobiles.Count
			};

			try
			{
				// ایجاد کمپین جدید
				var campaign = new GroupSmsCampaign
				{
					Title = campaignTitle ?? $"ارسال گروهی {DateTime.Now:yyyy/MM/dd HH:mm}",
					Message = message,
					Mobiles = string.Join(",", mobiles),
					TotalCount = mobiles.Count,
					Status = SmsStatus.Sending,
					SentAt = DateTime.UtcNow,
					CreatedBy = currentUser
				};

				_context.GroupSmsCampaigns.Add(campaign);
				await _context.SaveChangesAsync();

				// ارسال پیامک‌ها
				return await SendGroupSmsAsync(campaign.Id);
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = $"خطا در ایجاد کمپین: {ex.Message}";
				_logger.LogError(ex, "خطا در ارسال فوری پیامک گروهی");
				return result;
			}
		}

		public async Task<SendGroupSmsResult> SendGroupSmsAsync(int campaignId)
		{
			var result = new SendGroupSmsResult();

			try
			{
				var campaign = await _context.GroupSmsCampaigns.FindAsync(campaignId);
				if (campaign == null)
				{
					result.Success = false;
					result.Message = "کمپین یافت نشد";
					return result;
				}

				// پارس کردن شماره‌ها
				var mobiles = await ParseMobiles(campaign.Mobiles);
				campaign.TotalCount = mobiles.Count;
				campaign.Status = SmsStatus.Sending;
				await _context.SaveChangesAsync();

				var apiKey = _configuration["Kavenegar:ApiKey"] ?? "5269784E645955614833434A474C6667694B425832794D746A4235596969434B457353706B72722B79746F3D";
				var sender = _configuration["Kavenegar:Sender"] ?? "90006210";

				var api = new KavenegarApi(apiKey);
				decimal totalCost = 0;

				foreach (var mobile in mobiles)
				{
					try
					{
						var smsResult = api.Send(sender, mobile, campaign.Message);

						var log = new GroupSmsLog
						{
							CampaignId = campaignId,
							Mobile = mobile,
							Status = "Sent",
							MessageId = smsResult.Messageid, // Messageid با حرف کوچک
							SentAt = DateTime.UtcNow,
							Cost = (decimal?)smsResult.Cost // تبدیل به decimal?
						};

						_context.GroupSmsLogs.Add(log);
						result.SentCount++;

						// بررسی هزینه
						if (smsResult.Cost != 0) // اگر هزینه بیشتر از صفر بود
						{
							totalCost += smsResult.Cost;
						}

						await Task.Delay(100); // تأخیر برای جلوگیری از Rate Limit
					}
					catch (Exception ex)
					{
						result.FailedCount++;
						result.Errors.Add($"خطا برای {mobile}: {ex.Message}");

						var log = new GroupSmsLog
						{
							CampaignId = campaignId,
							Mobile = mobile,
							Status = "Failed",
							ErrorMessage = ex.Message,
							SentAt = DateTime.UtcNow
						};

						_context.GroupSmsLogs.Add(log);
						_logger.LogError(ex, "خطا در ارسال به {Mobile}", mobile);
					}
				}

				// بروزرسانی وضعیت کمپین
				campaign.SentCount = result.SentCount;
				campaign.Status = result.FailedCount == 0 ? SmsStatus.Completed :
								 (result.SentCount > 0 ? SmsStatus.Completed : SmsStatus.Failed);
				campaign.SentAt = DateTime.UtcNow;

				result.Success = result.SentCount > 0;
				result.Message = result.Success ?
					$"تعداد {result.SentCount} از {result.TotalCount} پیامک ارسال شد" :
					"ارسال ناموفق بود";
				result.TotalCost = totalCost > 0 ? (decimal?)totalCost : null;

				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = $"خطای سیستمی: {ex.Message}";
				_logger.LogError(ex, "خطا در ارسال گروهی پیامک");
			}

			return result;
		}

		private string CleanMobileNumber(string mobile)
		{
			if (string.IsNullOrWhiteSpace(mobile))
				return mobile;

			// حذف همه کاراکترهای غیرعددی
			var clean = new string(mobile.Where(char.IsDigit).ToArray());

			// اگر با ۹۸ شروع شده، به ۰۹ تبدیل کن
			if (clean.StartsWith("98") && clean.Length == 12)
			{
				clean = "0" + clean.Substring(2);
			}

			// اگر با +۹۸ شروع شده
			if (clean.StartsWith("98") && clean.Length == 12)
			{
				clean = "0" + clean.Substring(2);
			}

			return clean;
		}

		private bool IsValidMobile(string mobile)
		{
			if (string.IsNullOrWhiteSpace(mobile) || mobile.Length != 11)
				return false;

			return mobile.StartsWith("09") && mobile.All(char.IsDigit);
		}
	}
}