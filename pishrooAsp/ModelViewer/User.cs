namespace pishrooAsp.ModelViewer
{
	public class User
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public string Role { get; set; } // "Admin" یا "LimitedUser"
		public List<string> AllowedPages { get; set; }
	}
}
