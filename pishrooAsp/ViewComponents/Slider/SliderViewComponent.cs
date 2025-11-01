using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pishrooAsp.Data;
using pishrooAsp.Models.Slider;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

public class SliderViewComponent : ViewComponent
{
	private readonly AppDbContext _context;

	public SliderViewComponent(AppDbContext context)
	{
		_context = context;
	}

	public async Task<IViewComponentResult> InvokeAsync()
	{
		var culture = RouteData.Values["culture"]?.ToString() ?? CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

		var sliders = await _context.Sliders
			.Include(s => s.Translations)
				.ThenInclude(t => t.Lang)
			.ToListAsync();

		var model = sliders.Select(s =>
		{
			var tr = s.Translations.FirstOrDefault(t => t.Lang != null && t.Lang.Code.ToLower() == culture.ToLower());
			return new SliderViewModel
			{
				ImageUrl = s.ImageUrl,
				Link = s.Link,
				Title = tr?.Title ?? "",
				ShortDescription = tr?.ShortDescription ?? ""
			};
		}).ToList();

		return View(model);
	}
}

public class SliderViewModel
{
	public string ImageUrl { get; set; }
	public string Link { get; set; }
	public string Title { get; set; }
	public string ShortDescription { get; set; }
}
