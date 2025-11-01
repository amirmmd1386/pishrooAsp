using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pishrooAsp.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

public class WhyUsViewComponent : ViewComponent
{
	private readonly AppDbContext _context;

	public WhyUsViewComponent(AppDbContext context)
	{
		_context = context;
	}

	public async Task<IViewComponentResult> InvokeAsync()
	{
		var culture = RouteData.Values["culture"]?.ToString() ?? CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
		ViewBag.Culture = culture;
		var whyUsItems = await _context.WhyUs
			.Include(a => a.Translations)
				.ThenInclude(t => t.Lang)
			.ToListAsync();

		var model = whyUsItems.Select(w =>
		{
			var tr = w.Translations.FirstOrDefault(t => t.Lang != null && t.Lang.Code.ToLower() == culture.ToLower());
			return new
			{
				Title = tr?.Title ?? "",
				Icon = w.Icon ?? "fas fa-check-circle",  // اگر آیکون تعریف نشده بود آیکون پیش‌فرض
				Description = tr?.Description ?? ""
			};
		}).ToList();

		return View(model);
	}
}
