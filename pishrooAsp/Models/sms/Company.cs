namespace pishrooAsp.Models.Sms
{
	public class Company
	{
		public int Id { get; set; }

		public string Title { get; set; } = null!;

		// ❗ فقط یک شماره
		public string Mobile { get; set; } = null!;

		public ICollection<CompanySmsLog> SmsLogs { get; set; } = new List<CompanySmsLog>();

	}
}
