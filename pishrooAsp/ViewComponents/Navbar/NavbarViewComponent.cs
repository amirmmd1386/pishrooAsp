using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using pishrooAsp.Models.Navbar;
using System.Threading.Tasks;
using pishrooAsp.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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

		// تنظیم منو بر اساس زبان
		switch (culture)
		{
			case "fa":
				ConfigurePersianMenu(items, culture);
				break;
			case "ru":
				ConfigureRussianMenu(items, culture);
				break;
			case "tr":
				ConfigureTurkishMenu(items, culture);
				break;
			case "ar":
				ConfigureArabicMenu(items, culture);
				break;
			default:
				ConfigureDefaultMenu(items, culture);
				break;
		}

		var model = new NavbarViewModel
		{
			Items = items,
			CurrentCulture = culture
		};

		return View(model);
	}

	private void ConfigurePersianMenu(List<NavbarItem> items, string culture)
	{
		items.AddRange(new[]
		{
			new NavbarItem { Title = "خانه", Url = $"/{culture}" },
			new NavbarItem { Title = "محصولات", Url = $"/{culture}/products" },
			new NavbarItem { Title = "اخبار و مقالات", Url = $"/{culture}/newses" },
			new NavbarItem { Title = "سفارش محصولات", Url = $"/{culture}/ProductRequests/create" },
			new NavbarItem { Title = "درباره ما", Url = $"/{culture}/home/aboutuspage" },
			new NavbarItem { Title = "تماس با ما", Url = $"/{culture}/home/aboutuspage" },
			new NavbarItem { Title = "تصاویر", Url = $"/{culture}/gallery/getImage" },
			new NavbarItem { Title = "کاتالوگ ها", Url = $"/{culture}/catalog/GetCatalogs" }
		});
	}

	private void ConfigureRussianMenu(List<NavbarItem> items, string culture)
	{
		items.AddRange(new[]
		{
			new NavbarItem { Title = "Главная", Url = $"/{culture}" },
			new NavbarItem { Title = "Продукты", Url = $"/{culture}/products" },
			new NavbarItem { Title = "Новости и статьи", Url = $"/{culture}/newses" },
			new NavbarItem { Title = "Заказ продукции", Url = $"/{culture}/ProductRequests/create" },
			new NavbarItem { Title = "О нас", Url = $"/{culture}/home/aboutuspage" },

		});
	}

	private void ConfigureTurkishMenu(List<NavbarItem> items, string culture)
	{
		items.AddRange(new[]
		{
			new NavbarItem { Title = "Ana Sayfa", Url = $"/{culture}" },
			new NavbarItem { Title = "Ürünler", Url = $"/{culture}/products" },
			new NavbarItem { Title = "Haberler ve Makaleler", Url = $"/{culture}/newses" },
			new NavbarItem { Title = "Ürün Siparişi", Url = $"/{culture}/ProductRequests/create" },
			new NavbarItem { Title = "Hakkımızda", Url = $"/{culture}/home/aboutuspage" },

		});
	}

	private void ConfigureArabicMenu(List<NavbarItem> items, string culture)
	{
		items.AddRange(new[]
		{
			new NavbarItem { Title = "الرئيسية", Url = $"/{culture}" },
			new NavbarItem { Title = "المنتجات", Url = $"/{culture}/products" },
			new NavbarItem { Title = "الأخبار والمقالات", Url = $"/{culture}/newses" },
			new NavbarItem { Title = "طلب المنتجات", Url = $"/{culture}/ProductRequests/create" },
			new NavbarItem { Title = "من نحن", Url = $"/{culture}/home/aboutuspage" },

		});
	}

	private void ConfigureDefaultMenu(List<NavbarItem> items, string culture)
	{
		// منوی پیش‌فرض (مثلاً انگلیسی یا فارسی)
		items.AddRange(new[]
		{
			new NavbarItem { Title = "Home", Url = $"/{culture}" },
			new NavbarItem { Title = "Products", Url = $"/{culture}/products" },
			new NavbarItem { Title = "News & Articles", Url = $"/{culture}/newses" },
			new NavbarItem { Title = "Product Order", Url = $"/{culture}/ProductRequests/create" },
			new NavbarItem { Title = "About Us", Url = $"/{culture}/home/aboutuspage" },

		});
	}
}