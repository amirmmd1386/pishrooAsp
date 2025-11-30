using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pishrooAsp.Data;
using pishrooAsp.Models;
using pishrooAsp.Models.Newses;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

[ViewComponent(Name = "RelatedArticles")]
public class RelatedArticlesViewComponent : ViewComponent
{
	private readonly AppDbContext _context;

	public RelatedArticlesViewComponent(AppDbContext context)
	{
		_context = context;
	}

	public async Task<IViewComponentResult> InvokeAsync(string productKeywords, int count = 3)
	{
		var culture = RouteData.Values["culture"]?.ToString() ?? CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

		// تقسیم کلمات کلیدی محصول
		var keywords = productKeywords?.Split(',')
			.Select(k => k.Trim())
			.Where(k => !string.IsNullOrEmpty(k))
			.ToList() ?? new List<string>();

		IQueryable<News> query = _context.News
			.Include(n => n.Translations)
				.ThenInclude(t => t.Lang);

		// پیدا کردن مقالاتی که در Title آنها کلمات کلیدی محصول وجود دارد
		if (keywords.Any())
		{
			query = query.Where(n =>
				n.Translations.Any(t =>
					t.Lang.Code.ToLower() == culture.ToLower() &&
					keywords.Any(keyword =>
						t.Title.Contains(keyword) ||
						t.Summary.Contains(keyword)
					)
				)
			);
		}

		var articles = await query
			.OrderByDescending(n => n.PublishDate)
			.Take(count)
			.ToListAsync();

		// تبدیل به List<object>
		var model = articles.Select(article =>
		{
			var tr = article.Translations.FirstOrDefault(t => t.Lang != null && t.Lang.Code.ToLower() == culture.ToLower());
			return new
			{
				Id = article.Id,
				Title = tr?.Title ?? "",
				Summary = tr?.Summary ?? "",
				ImageUrl = article.DefaultImageUrl,
				PublishDate = article.PublishDate,
				Language = culture.ToLower()
			} as object; // این خط مهم است
		}).ToList();

		ViewBag.Language = culture.ToLower();
		return View(model);
	}
}