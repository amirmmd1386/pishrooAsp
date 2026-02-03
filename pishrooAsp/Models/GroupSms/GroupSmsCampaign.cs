// Models/GroupSms/GroupSmsCampaign.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace pishrooAsp.Models.GroupSms
{
	public class GroupSmsCampaign
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "عنوان کمپین الزامی است")]
		[Display(Name = "عنوان کمپین")]
		public string Title { get; set; }

		[Required(ErrorMessage = "متن پیامک الزامی است")]
		[Display(Name = "متن پیامک")]
		[MaxLength(500, ErrorMessage = "متن پیامک نباید بیشتر از 500 کاراکتر باشد")]
		public string Message { get; set; }

		[Display(Name = "شماره‌های موبایل")]
		public string Mobiles { get; set; } // ذخیره به صورت CSV: 0912,0913

		[Display(Name = "تعداد ارسال")]
		public int TotalCount { get; set; }

		[Display(Name = "ارسال موفق")]
		public int SentCount { get; set; }

		[Display(Name = "وضعیت")]
		public SmsStatus Status { get; set; } = SmsStatus.Draft;

		[Display(Name = "زمان ایجاد")]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		[Display(Name = "زمان ارسال")]
		public DateTime? SentAt { get; set; }

		[Display(Name = "یادداشت")]
		public string? Notes { get; set; }

		// 👇 اضافه کردن فیلد IsDeleted
		[Display(Name = "حذف شده")]
		public bool IsDeleted { get; set; } = false;

		// 👇 اضافه کردن فیلد برای کاربر ایجاد کننده
		[Display(Name = "ایجاد کننده")]
		public string CreatedBy { get; set; } = "System"; // یا از UserId استفاده کنید

		// تاریخچه ارسال
		public virtual ICollection<GroupSmsLog> Logs { get; set; } = new List<GroupSmsLog>();
	}

	public enum SmsStatus
	{
		Draft = 0,      // پیش‌نویس
		Scheduled = 1,  // زمان‌بندی شده
		Sending = 2,    // در حال ارسال
		Completed = 3,  // تکمیل شده
		Failed = 4,
		// ناموفق
		Deleted = 5
	}
}