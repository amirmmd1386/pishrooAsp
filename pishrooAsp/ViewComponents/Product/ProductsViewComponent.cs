using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pishrooAsp.Data;
using pishrooAsp.Models;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

public class ProductsViewComponent : ViewComponent
{
	private readonly AppDbContext _context;

	public ProductsViewComponent(AppDbContext context)
	{
		_context = context;
	}

	public async Task<IViewComponentResult> InvokeAsync()
	{
		var culture = RouteData.Values["culture"]?.ToString() ?? CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

		var products = await _context.Products
			.Include(p => p.Translations)
				.ThenInclude(t => t.Lang)
				
			.ToListAsync();

		var model = products.Select(p =>
		{
			var tr = p.Translations.FirstOrDefault(t => t.Lang != null && t.Lang.Code.ToLower() == culture.ToLower());
			return new ProductViewModel
			{
				Id = p.Id,
				ImageUrl = p.ImageUrl,
				Title = tr?.Title ?? "",
				Description = tr?.Description ?? "",
				Language = culture.ToLower(),
			};
		}).ToList();
	

		return View(model);
	}

}

public class ProductViewModel
{
	public Guid Id { get; set; }
	public string ImageUrl { get; set; }
	public string Title { get; set; }
	public string Description { get; set; }
	public string Language { get; set; }
}
