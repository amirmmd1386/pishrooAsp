using System;

namespace pishrooAsp.Models.Sms
{
	public class CompanySmsLog
	{
		public int Id { get; set; }

		public int CompanyId { get; set; }
		public Company Company { get; set; }

		public int SmsTemplateId { get; set; }
		public SmsTemplate SmsTemplate { get; set; }

		public string Mobile { get; set; }
		public string FinalMessage { get; set; }

		public bool IsSent { get; set; }
		public string? ProviderResult { get; set; }

		public DateTime SentAt { get; set; } = DateTime.Now;
	}
}
