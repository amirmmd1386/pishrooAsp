using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using pishrooAsp.Models.Navbar;
using System.Threading.Tasks;
using pishrooAsp.Data;
using Microsoft.EntityFrameworkCore;

public class NavbarViewComponent : ViewComponent
{
	private readonly AppDbContext _context;

	public NavbarViewComponent(AppDbContext context)
	{
		_context = context;
	}

	public async Task<IViewComponentResult> InvokeAsync()
	{
		var culture = RouteData.Values["culture"]?.ToString()?.ToLower()
					  ?? CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower();

		var items = new List<NavbarItem>();

		if (culture == "fa")
		{
			items.Add(new NavbarItem { Title = "خانه", Url = "/fa" });
			items.Add(new NavbarItem { Title = "محصولات", Url = "/fa/products" });
			items.Add(new NavbarItem { Title = "اخبار و مقالات", Url = "/fa/home/newsList" });
			items.Add(new NavbarItem { Title = "سفارش محصولات", Url = "/fa/ProductRequests/create" });
			items.Add(new NavbarItem { Title = "درباره ما", Url = "/fa/home/aboutuspage" });
			items.Add(new NavbarItem { Title = "تماس با ما", Url = "/fa/home/aboutuspage" });
			items.Add(new NavbarItem { Title = "تصاویر", Url = "/fa/gallery/getImage" });
			items.Add(new NavbarItem { Title = "کاتالوگ ها", Url = "/fa/catalog/GetCatalogs" });
		}
		else
		{
			items.Add(new NavbarItem { Title = "Home", Url = "/" + culture });
			items.Add(new NavbarItem { Title = "Products", Url = "/" + culture + "/products" });
			items.Add(new NavbarItem { Title = "News & Articles", Url = "/" + culture + "/home/newsList" });
			items.Add(new NavbarItem { Title = "ProductRequests", Url = "/" + culture + "/ProductRequests/create" });
			items.Add(new NavbarItem { Title = "About Us", Url = "/" + culture + "/home/aboutuspage" });
			items.Add(new NavbarItem { Title = "Contact Us", Url = "/" + culture + "/home/aboutuspage" });
		}

		var model = new NavbarViewModel
		{
			Items = items,
			CurrentCulture = culture
		};

		return View(model);
	}
}