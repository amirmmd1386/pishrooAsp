namespace pishrooAsp.Models.MessageTemplate
{
	// ViewModel برای نمایش توکن‌ها
	public class TokenViewModel
	{
		public string TokenName { get; set; }
		public string DisplayName { get; set; }
		public string Value { get; set; }
		public bool IsRequired { get; set; }
	}
}
