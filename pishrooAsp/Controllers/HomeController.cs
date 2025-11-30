using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pishrooAsp.Data;
using pishrooAsp.Models;
using pishrooAsp.Models.AboutUs;
using pishrooAsp.ModelViewer;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;

namespace pishrooAsp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly AppDbContext _context;



		private readonly VisitService _visitService;
		public HomeController(ILogger<HomeController> logger,AppDbContext context, VisitService visitService)
        {
			_visitService = visitService;

			_logger = logger;
			_context = context;
		}
		public async Task<IActionResult> Index1()
		{
			return View();
		}
		public async Task<IActionResult> Index()
		{
			await _visitService.LogVisitAsync(HttpContext, "صفحه اصلی");

			var culture = RouteData.Values["culture"]?.ToString() ?? "fa";

			var translation = _context.AboutUsTranslations
				.Include(p => p.Lang)
				.FirstOrDefault(p => p.Lang.Code.ToLower() == culture.ToLower());

			// بقیه کدهای شما...
			var bg = _context.Backgrounds.FirstOrDefault(x => x.IsActive);
			ViewBag.Background = bg?.FilePath;

			ViewBag.dir = translation?.Lang.dir;
			ViewBag.about = translation?.Description;
			ViewBag.slogan = translation?.Slogan;
			ViewBag.Name = translation?.CompanyName;
			ViewBag.lang = culture.ToLower();
			ViewBag.MetaTitle = "اروین پلیمر | تولید کننده برتر محصولات پلیمری در ایران";
			ViewBag.MetaDescription = translation?.Description;
			ViewBag.MetaKeywords = "پلیمر, لوله پلی اتیلن, اتصالات پلیمری, تولید کننده پلیمر";
			ViewBag.OgImage = "/image/companyLogo.png";

			// اسکیماهای ساده و بدون خطا
			var baseUrl = $"{Request.Scheme}://{Request.Host}";

			// 1. اسکیما سازمان
			var organizationSchema = new
			{
				@context = "https://schema.org",
				@type = "Organization",
				name = "آروین پلیمر",
				url = baseUrl,
				logo = $"{baseUrl}/images/company-logo.png",
				description = "تولید کننده محصولات پلیمری",
				telephone = "+98-21-12345678",
				address = new
				{
					@type = "PostalAddress",
					addressCountry = "IR"
				}
			};

			// 2. اسکیما وبسایت
			var websiteSchema = new
			{
				@context = "https://schema.org",
				@type = "WebSite",
				name = "آروین پلیمر",
				url = baseUrl,
				potentialAction = new
				{
					@type = "SearchAction",
					target = $"{baseUrl}/search?q={{search_term_string}}",
					queryInput = "required name=search_term_string"
				}
			};

			ViewBag.OrganizationSchema = JsonSerializer.Serialize(organizationSchema);
			ViewBag.WebsiteSchema = JsonSerializer.Serialize(websiteSchema);

			return View();
		}

		public string culture()
		{
			var culture = RouteData.Values["culture"]?.ToString() ?? CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
			return culture.ToLower();
		}
		public async Task<IActionResult> ProductDetail(string slug)
		{
			await _visitService.LogVisitAsync(HttpContext, "/جزییات محصول");

			var culture = RouteData.Values["culture"]?.ToString() ?? CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

			// اول با روش ساده‌تر فیلتر کن
			var potentialProducts = await _context.Products
				.Where(p => p.seoWord != null && p.seoWord.Contains(slug))
				.ToListAsync();

			// سپس دقیق فیلتر کن
			var product = potentialProducts
				.FirstOrDefault(p => p.seoWord.Split(',')[0].Trim() == slug);
			if (product == null)
			{
				return NotFound();
			}

			var translation = await _context.ProductTranslations
				.Include(t => t.Lang)
				.FirstOrDefaultAsync(t => t.ProductId == product.Id && t.Lang.Code.ToLower() == culture.ToLower());

			ViewData["Translation"] = translation;
			ViewBag.dir = translation.Lang.dir;

			ViewBag.lang = culture.ToLower();
			ViewBag.MetaTitle = $"{translation.Title} | آروین پلیمر";
			ViewBag.MetaDescription =  product.SeoDiscription;
			ViewBag.MetaKeywords = $"{translation.Title}, محصولات پلیمری,{product.seoWord.Split('|')?[0]}";
			ViewBag.OgImage = product.ImageUrl;
			ViewBag.CanonicalUrl = $"/products/{product.Id}";
			return View(product);


		}
		public async Task<IActionResult> AboutusPage()
		{
			await _visitService.LogVisitAsync(HttpContext, "/درباره ما");

			// دریافت اطلاعات مربوط به AboutUs از پایگاه داده
			var aboutUsItem = _context.AboutUs.Include(p=>p.Translations).ThenInclude(t => t.Lang).FirstOrDefault();

			if (aboutUsItem == null)
			{
				// در صورتی که اطلاعاتی در پایگاه داده وجود ندارد، می‌توانید یک پیغام خطا یا صفحه خالی نمایش دهید.
				// یا به یک صفحه دیگر Redirect کنید.
				return NotFound();
			}
			var culture = RouteData.Values["culture"]?.ToString() ?? CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

			// دریافت ترجمه مربوط به زبان فعلی
			// در این مثال فرض شده است که زبان از طریق یک متغیر یا سرویس مشخص می‌شود.
			// به عنوان مثال، با LangId = 1 به زبان فارسی اشاره می‌کنیم.
			var aboutUsTranslation = aboutUsItem.Translations.FirstOrDefault(p=>  p.Lang != null&&p.Lang.Code.ToLower() == culture.ToLower());

			// اگر ترجمه‌ای برای زبان فعلی پیدا نشد
			if (aboutUsTranslation == null)
			{
				// می‌توانید به یک ترجمه پیش‌فرض (مثلاً انگلیسی) Fallback کنید.
				aboutUsTranslation = _context.AboutUsTranslations
											 .FirstOrDefault(t => t.AboutUsId == aboutUsItem.Id);
			}

			// ایجاد View Model
			var viewModel = new AboutUsViewModel
			{
				Item = aboutUsItem,
				Translation = aboutUsTranslation
			};

			// ارسال ViewModel به View مربوطه
			return View(viewModel);
		}
		public IActionResult Privacy()
		{
			return View();
		}
		public IActionResult AdminPanelLink()
		{
			return View();
		}
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



		public async Task<IActionResult> ProgramList()
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
					Language = culture.ToLower(),
					seoWord = p.seoWord
				};
			}).ToList();

			// متا تگ‌های صفحه لیست محصولات
			// متا تگ‌های صفحه لیست محصولات
			if (culture == "fa")
			{
				ViewBag.MetaTitle = "محصولات پلیمری | تی پی ای، تی پی وی، اس بی اس، کامپاند | آروین پلیمر";
				ViewBag.MetaDescription = "تولید تی پی ای (TPE)، تی پی وی (TPV)، اس بی اس (SBS)، اس بی ای اس (SEBS)، شور، سافت تاچ، کامپاند پلیمری، الاستومر، پلی پروپیلن، پلی اتیلن، همو پلیمر و کو پلیمر با کیفیت بالا";
				ViewBag.MetaKeywords = "تی پی ای, TPE, تی پی وی, TPV, اس بی اس, SBS, اس بی ای اس, SEBS, شور, سافت تاچ, کامپاند, پلیمر, الاستومر, پلی پروپیلن, پلی اتیلن, همو پلیمر, کو پلیمر, آروین پلیمر";
			}
			else
			{
				ViewBag.MetaTitle = "Polymer Products | TPE, TPV, SBS, Compound | Arvin Polymer";
				ViewBag.MetaDescription = "Production of TPE, TPV, SBS, SEBS, Shore, Soft Touch, Polymer Compound, Elastomer, Polypropylene, Polyethylene, Homo Polymer and Co Polymer with high quality";
				ViewBag.MetaKeywords = "TPE, TPV, SBS, SEBS, Shore, Soft Touch, Compound, Polymer, Elastomer, Polypropylene, Polyethylene, Homo Polymer, Co Polymer, Arvin Polymer";
			}

			ViewBag.OgImage = "/images/products-banner.jpg";
			ViewBag.CanonicalUrl = $"/{culture}/products";
			ViewBag.OgUrl = $"https://arvinpolymer.com/{culture}/products";
			ViewBag.OgType = "website";
			return View(model);
		}
		public async Task<IActionResult> NewsList()
		{
			var culture = RouteData.Values["culture"]?.ToString() ?? CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

			var lastNews = await _context.News
				.Include(n => n.Translations)
					.ThenInclude(t => t.Lang)
				.OrderByDescending(n => n.PublishDate)
				.Take(6) // گرفتن 6 خبر آخر
				.ToListAsync();

			if (!lastNews.Any())
			{
				return View(new List<NewsViewModelSecond>());
			}

			var model = lastNews.Select(news =>
			{
				var tr = news.Translations.FirstOrDefault(t =>
					t.Lang != null &&
					t.Lang.Code.ToLower() == culture.ToLower());

				return new NewsViewModelSecond
				{
					Id = news.Id,
					Title = tr?.Title ?? "No Title",
					Summary = tr?.Summary ?? "No Summary",
					Content = tr?.Content ?? "",
					ImageUrl = news.DefaultImageUrl ?? "/images/default-news.jpg",
					PublishDate = news.PublishDate,
					Author = "آروین پلیمر",
					ViewCount = 100,
					Category = news.IsPublished ? "اخبار" : "مقاله"
				};
			}).ToList();

			ViewBag.Language = culture.ToLower();
			return View(model);
		}
	}


	}

