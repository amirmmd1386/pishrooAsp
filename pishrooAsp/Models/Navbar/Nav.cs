namespace pishrooAsp.Models.Navbar
{
	public class NavbarItem
	{
		public string Title { get; set; }
		public string Url { get; set; }
	}

	public class NavbarViewModel
	{
		public List<NavbarItem> Items { get; set; }
		public string CurrentCulture { get; set; }
	}
}
