using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pishrooAsp.Data;
using pishrooAsp.Models;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

public class BlogViewComponent : ViewComponent
{
	private readonly AppDbContext _context;

	public BlogViewComponent(AppDbContext context)
	{
		_context = context;
	}

	public async Task<IViewComponentResult> InvokeAsync(int count = 3)
	{
		var culture = RouteData.Values["culture"]?.ToString() ?? CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

		var latestNews = await _context.News
			.Include(n => n.Translations)
				.ThenInclude(t => t.Lang)
			.Where(n => !n.IsPublished) // فقط اخبار منتشر شده
			.OrderByDescending(n => n.PublishDate)
			.Take(count) // تعداد اخبار درخواستی
			.ToListAsync();

		if (!latestNews.Any())
		{
			return View(null);
		}

		var newsModels = latestNews.Select(news =>
		{
			var tr = news.Translations.FirstOrDefault(t =>
				t.Lang != null &&
				t.Lang.Code.ToLower() == culture.ToLower());

			return new
			{
				Id = news.Id,
				Title = tr?.Title ?? "No title available",
				Summary = tr?.Summary ?? "",
				ImageUrl = news.DefaultImageUrl,
				PublishDate = news.PublishDate,
				Category ="General",
				TimeAgo = GetTimeAgo(news.PublishDate, culture)
			};
		}).ToList();

		ViewBag.lang = culture.ToLower();
		return View(newsModels);
	}

	private string GetTimeAgo(DateTime publishDate, string culture)
	{
		var timeSpan = DateTime.Now - publishDate;

		if (culture == "fa")
		{
			if (timeSpan.TotalMinutes < 60)
				return $"{(int)timeSpan.TotalMinutes} دقیقه پیش";
			if (timeSpan.TotalHours < 24)
				return $"{(int)timeSpan.TotalHours} ساعت پیش";
			return $"{(int)timeSpan.TotalDays} روز پیش";
		}
		else
		{
			if (timeSpan.TotalMinutes < 60)
				return $"{(int)timeSpan.TotalMinutes} minutes ago";
			if (timeSpan.TotalHours < 24)
				return $"{(int)timeSpan.TotalHours} hours ago";
			return $"{(int)timeSpan.TotalDays} days ago";
		}
	}
}