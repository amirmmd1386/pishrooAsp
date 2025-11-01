namespace pishrooAsp.Services
{
	public class UserAgentService
	{
		public string GetDeviceType(string userAgent)
		{
			if (string.IsNullOrEmpty(userAgent))
				return "Unknown";

			// ساده‌سازی برای تشخیص سریع
			if (userAgent.Contains("Mobile") || userAgent.Contains("Android") || userAgent.Contains("iPhone"))
				return "Mobile";

			if (userAgent.Contains("Tablet") || userAgent.Contains("iPad"))
				return "Tablet";

			return "Desktop";
		}
	}
}
