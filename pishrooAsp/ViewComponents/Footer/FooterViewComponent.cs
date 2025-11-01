using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pishrooAsp.Data;
using pishrooAsp.Models.AboutUs;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace pishrooAsp.ViewComponents
{
	public class FooterViewComponent : ViewComponent
	{
		private readonly AppDbContext _context;

		public FooterViewComponent(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IViewComponentResult> InvokeAsync(int langId = 1)
		{
			// گرفتن اطلاعات اصلی شرکت همراه ترجمه زبان انتخابی
			var aboutUs = await _context.AboutUs
				.Include(a => a.Translations)
				.FirstOrDefaultAsync();

			if (aboutUs == null)
				return View(new AboutUsViewModel());

			var culture = RouteData.Values["culture"]?.ToString() ?? CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

			var translation = aboutUs.Translations?
				.FirstOrDefault(t => t.Lang.Code.ToLower() == culture.ToLower())
				?? aboutUs.Translations?.FirstOrDefault(); // اگر ترجمه زبان نبود، یک ترجمه پیش‌فرض

			ViewBag.about = translation.Description;

			var model = new AboutUsViewModel
			{
				Item = aboutUs,
				Translation = translation
			};

			return View(model);
		}
	}
}
