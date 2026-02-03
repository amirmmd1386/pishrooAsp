// Models/GroupSms/GroupSmsLog.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pishrooAsp.Models.GroupSms
{
	public class GroupSmsLog
	{
		public int Id { get; set; }

		[Required]
		public int CampaignId { get; set; }

		[Required]
		[MaxLength(20)]
		[Display(Name = "شماره موبایل")]
		public string Mobile { get; set; }

		[Display(Name = "وضعیت")]
		public string Status { get; set; } = "Pending";

		[Display(Name = "پیام خطا")]
		public string? ErrorMessage { get; set; }

		[Display(Name = "کد پیگیری")]
		public long? MessageId { get; set; }

		[Display(Name = "زمان ارسال")]
		public DateTime? SentAt { get; set; }

		[Display(Name = "قیمت")]
		public decimal? Cost { get; set; }

		[ForeignKey("CampaignId")]
		public virtual GroupSmsCampaign Campaign { get; set; }
	}
}