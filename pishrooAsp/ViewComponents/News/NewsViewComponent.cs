using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pishrooAsp.Data;
using pishrooAsp.Models;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

public class NewsViewComponent : ViewComponent
{
	private readonly AppDbContext _context;

	public NewsViewComponent(AppDbContext context)
	{
		_context = context;
	}

	public async Task<IViewComponentResult> InvokeAsync()
	{
		var culture = RouteData.Values["culture"]?.ToString() ?? CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

		var lastNews = await _context.News
			.Include(n => n.Translations)
				.ThenInclude(t => t.Lang)
			.OrderByDescending(n => n.PublishDate)  // مرتب‌سازی نزولی بر اساس تاریخ انتشار
			.FirstOrDefaultAsync(p=>p.IsPublished);

		if (lastNews == null)
		{
			return View(null); // یا ویو با مدل خالی
		}

		var tr = lastNews.Translations.FirstOrDefault(t => t.Lang != null && t.Lang.Code.ToLower() == culture.ToLower());
		ViewBag.lang=culture.ToLower();
		
		var model = new
		{
			Id = lastNews.Id,
			Title = tr?.Title ?? "",
			Summary = tr?.Summary ?? "",
			ImageUrl = lastNews.DefaultImageUrl,
			PublishDate = lastNews.PublishDate
		};

		return View(new List<object> { model }); // چون ویو شما انتظار IEnumerable داره، یک لیست با یک عضو بفرست
	}

}
