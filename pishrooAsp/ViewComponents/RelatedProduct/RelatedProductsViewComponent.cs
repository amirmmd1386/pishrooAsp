// RelatedProductsViewComponent.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pishrooAsp.Data;
using pishrooAsp.Models;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

public class RelatedProductsViewComponent : ViewComponent
{
	private readonly AppDbContext _context;

	public RelatedProductsViewComponent(AppDbContext context)
	{
		_context = context;
	}

	public async Task<IViewComponentResult> InvokeAsync(Guid currentProductId, int count = 3)
	{
		var culture = RouteData.Values["culture"]?.ToString() ?? CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

		// دریافت محصولات مرتبط (به جز محصول جاری)
		var products = await _context.Products
			.Where(p => p.Id != currentProductId)
			.Include(p => p.Translations)
				.ThenInclude(t => t.Lang)
			.OrderByDescending(p => p.CreatedAt) // یا هر منطق دیگری برای مرتبط بودن
			.Take(count)
			.ToListAsync();

		var model = products.Select(p =>
		{
			var tr = p.Translations.FirstOrDefault(t => t.Lang != null && t.Lang.Code.ToLower() == culture.ToLower());
			return new ProductViewModel
			{
				Id = p.Id,
				ImageUrl = p.ImageUrl,
				Title = tr?.Title ?? "",
				Language = culture.ToLower(),
				seoWord = p.seoWord?.Split(new char[] { ',' }, StringSplitOptions.None)?[0].Trim() ?? "none"
			};
		}).ToList();

		return View(model);
	}
}