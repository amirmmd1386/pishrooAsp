using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pishrooAsp.Data;
using pishrooAsp.Models.Newses;
using pishrooAsp.Models.Newes; // استفاده از namespace فایل شما

using pishrooAsp.ModelViewer;

namespace pishrooAsp.Controllers
{
	public class NewsController : Controller
	{
		private readonly AppDbContext _context;
		private readonly IWebHostEnvironment _env;

		public NewsController(AppDbContext context, IWebHostEnvironment env)
		{
			_context = context;
			_env = env;
		}

		// لیست اخبار
		[AdminAuthFilter]

		public async Task<IActionResult> Index()
		{
			var langs = await _context.Langs.ToListAsync();
			ViewBag.Langs = langs;

			var news = await _context.News
				.Include(n => n.Translations)
				.Include(n => n.Images)
				.Include(n => n.Attachments)
				.OrderByDescending(n => n.Id)
				.ToListAsync();

			return View(news);
		}


		[AdminAuthFilter]

		public IActionResult Create()
		{
			ViewBag.Languages = _context.Langs.ToList();
			return View();
		}
		[AdminAuthFilter]

		// صفحه ایجاد
		[HttpPost]
		public async Task<IActionResult> Create(NewsDto model)
		{
			try
			{
				var news = new News
				{
					PublishDate = DateTime.Now,
					IsPublished = model.IsPublished,
					AuthorId = model.AuthorId,
					DefaultImageUrl = model.DefaultImage != null ? await SaveFile(model.DefaultImage) : null,
					Translations = new List<NewsTranslation>(), // اینجا مقدار اولیه رو بزنید
					Images = new List<NewsImage>(),
					Attachments = new List<NewsAttachment>()
				};

				if (model.Translations != null)
				{
					foreach (var t in model.Translations)
					{
						news.Translations.Add(new NewsTranslation
						{
							LangId = t.LangId,
							Title = t.Title,
							Summary = t.Summary,
							Content = t.Content,
							MetaKeywords = t.MetaKeywords,
							MetaDescription = t.MetaDescription
						});
					}
				}

				if (model.Images != null)
				{
					foreach (var file in model.Images)
					{
						var url = await SaveFile(file);
						news.Images.Add(new NewsImage { ImageUrl = url, AltText = file.FileName, DisplayOrder = 0 });
					}
				}

				if (model.Attachments != null)
				{
					foreach (var file in model.Attachments)
					{
						var url = await SaveFile(file);
						news.Attachments.Add(new NewsAttachment
						{
							FileUrl = url,
							FileName = file.FileName,
							FileType = Path.GetExtension(file.FileName),
							FileSize = file.Length
						});
					}
				}

				_context.News.Add(news);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}
		[AdminAuthFilter]

		public async Task<IActionResult> Edit(int id)
		{
			try
			{
				var dto = await _context.News
				.Include(n => n.Translations)
				.Include(n => n.Images)
				.Include(n => n.Attachments)
				.FirstOrDefaultAsync(n => n.Id == id);

				if (dto == null)
					return NotFound();

				var vm = new NewsViewModel
				{
					PublishDate = dto.PublishDate,
					IsPublished = dto.IsPublished,
					AuthorId = dto.AuthorId ?? 0,
					DefaultImageUrl = dto.DefaultImageUrl,

					Translations = dto.Translations.Select(t => new NewsTranslationViewModel
					{
						LangId = t.LangId,
						Title = t.Title,
						Summary = t.Summary,
						Content = t.Content,
						MetaDescription = t.MetaDescription,
						MetaKeywords = t.MetaKeywords,
					}).ToList(),
					Images = dto.Images.Select(i => new NewsImageViewModel
					{
						ImageUrl = i.ImageUrl
					}).ToList(),
					Attachments = dto.Attachments.Select(a => new NewsAttachmentViewModel
					{
						FileUrl = a.FileUrl
					}).ToList()
				};
				return View(vm);

			}
			catch
			{ 

			}
			

			return View();
		}


		[AdminAuthFilter]

		[HttpPost]
		public async Task<IActionResult> Edit(int id, NewsDto model)
		{
			try
			{
				if (id != model.Id) return NotFound();

				var news = await _context.News
					.Include(n => n.Translations)
					.Include(n => n.Images)
					.Include(n => n.Attachments)
					.FirstOrDefaultAsync(n => n.Id == id);

				if (news == null) return NotFound();

				news.IsPublished = model.IsPublished;
				news.AuthorId = model.AuthorId;

				if (model.DefaultImage != null)
					news.DefaultImageUrl = await SaveFile(model.DefaultImage);

				_context.NewsTranslations.RemoveRange(news.Translations);
				news.Translations = model.Translations;

				if (model.Images != null)
				{
					foreach (var file in model.Images)
					{
						var url = await SaveFile(file);
						news.Images.Add(new NewsImage { ImageUrl = url, AltText = file.FileName, DisplayOrder = 0 });
					}
				}

				if (model.Attachments != null)
				{
					foreach (var file in model.Attachments)
					{
						var url = await SaveFile(file);
						news.Attachments.Add(new NewsAttachment
						{
							FileUrl = url,
							FileName = file.FileName,
							FileType = Path.GetExtension(file.FileName),
							FileSize = file.Length
						});
					}
				}

				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			catch {
				return View(model);
			}
			
			return RedirectToAction(nameof(Index));
		}

		// حذف خبر	[AdminAuthFilter]
		[AdminAuthFilter]

		public async Task<IActionResult> Delete(int id)
		{
			var news = await _context.News.FindAsync(id);
			if (news == null) return NotFound();

			_context.News.Remove(news);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
		[AdminAuthFilter]

		private async Task<string> SaveFile(IFormFile file)
		{
			var folder = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads");
			Directory.CreateDirectory(folder);
			var path = Path.Combine(folder, Path.GetFileName(file.FileName));
			using var stream = new FileStream(path, FileMode.Create);
			await file.CopyToAsync(stream);
			return $"/uploads/{file.FileName}";
		}

		
		public async Task<IActionResult> Details(int id, string title)
		{
			var news = await _context.News
				.Include(n => n.Translations)
				.Include(n => n.Images)
				.Include(n => n.Attachments)
				.FirstOrDefaultAsync(n => n.Id == id);

			if (news == null)
				return NotFound();

			var translation = news.Translations.FirstOrDefault();
			var culture = RouteData.Values["culture"]?.ToString() ?? "fa";

			// آخرین اخبار (5 تای آخر)
			var latestNews = await _context.News
				.Include(n => n.Translations)
				.Where(n => n.IsPublished && n.Id != id)
				.OrderByDescending(n => n.PublishDate)
				.Take(5)
				.Select(n => new NewsSidebarViewModel
				{
					Id = n.Id,
					Title = n.Translations.FirstOrDefault(t => t.Lang.Code == culture).Title ??
						   n.Translations.First().Title,
					DefaultImageUrl = n.DefaultImageUrl,
					PublishDate = n.PublishDate
				})
				.ToListAsync();

			var latestArticle = await _context.News
				.Include(n => n.Translations)
				.Where(n => !n.IsPublished && n.Id != id)
				.OrderByDescending(n => n.PublishDate)
				.Take(5)
				.Select(n => new NewsSidebarViewModel
				{
					Id = n.Id,
					Title = n.Translations.FirstOrDefault(t => t.Lang.Code == culture).Title ??
						   n.Translations.First().Title,
					DefaultImageUrl = n.DefaultImageUrl,
					PublishDate = n.PublishDate
				})
				.ToListAsync();
			// آخرین محصولات (4 تای آخر)
			var latestProducts = await _context.Products
				.Include(p => p.Translations)
				.OrderByDescending(p => p.CreatedAt)
				.Take(4)
				.Select(p => new ProductSidebarViewModel
				{
					Id = p.Id,
					Title = p.Translations.FirstOrDefault(t => t.Lang.Code == culture).Title ??
						   p.Translations.First().Title,
					ImageUrl = p.ImageUrl,
					SeoWord = p.seoWord
				})
				.ToListAsync();

			// دسته‌بندی‌های اخبار
			var categories = await _context.News
				.Include(n => n.Translations)
				.Where(n => n.IsPublished)
				.SelectMany(n => n.Translations)
				.Where(t => !string.IsNullOrEmpty(t.MetaKeywords))
				.Select(t => t.MetaKeywords)
				.ToListAsync();

			// متا تگ‌ها
			ViewBag.MetaTitle = $"{translation?.Title} | آروین پلیمر";
			ViewBag.MetaDescription = translation?.Summary?.Length > 160 ?
				translation.Summary.Substring(0, 160) + "..." :
				translation?.Summary;
			ViewBag.MetaKeywords = $"{translation?.Title}, اخبار پلیمر, صنعت پلیمر, تی پی ای, تی پی وی, کامپاند";
			ViewBag.OgImage = news.DefaultImageUrl;
			ViewBag.CanonicalUrl = $"/{culture}/news/details/{id}";

			// داده‌های سایدبار
			ViewBag.LatestNews = latestNews;
			ViewBag.LatestProducts = latestProducts;
			ViewBag.latestArticle = latestArticle;
			ViewBag.Categories = ProcessCategories(categories);
			ViewBag.Tags = GetPopularTags(categories);

			return View(news);
		}
		// متد کمکی برای پردازش دسته‌بندی‌ها
		private Dictionary<string, int> ProcessCategories(List<string> metaKeywordsList)
		{
			var categories = new Dictionary<string, int>();

			foreach (var keywords in metaKeywordsList)
			{
				if (!string.IsNullOrEmpty(keywords))
				{
					var keywordArray = keywords.Split(',', StringSplitOptions.RemoveEmptyEntries);
					foreach (var keyword in keywordArray)
					{
						var trimmedKeyword = keyword.Trim();
						if (categories.ContainsKey(trimmedKeyword))
						{
							categories[trimmedKeyword]++;
						}
						else
						{
							categories[trimmedKeyword] = 1;
						}
					}
				}
			}

			return categories.OrderByDescending(x => x.Value).Take(8).ToDictionary(x => x.Key, x => x.Value);
		}

		// متد کمکی برای برچسب‌های محبوب
		private List<string> GetPopularTags(List<string> metaKeywordsList)
		{
			var allTags = new List<string>();

			foreach (var keywords in metaKeywordsList)
			{
				if (!string.IsNullOrEmpty(keywords))
				{
					var tags = keywords.Split(',', StringSplitOptions.RemoveEmptyEntries)
									 .Select(t => t.Trim())
									 .Where(t => t.Length > 2);
					allTags.AddRange(tags);
				}
			}

			return allTags.GroupBy(t => t)
						 .OrderByDescending(g => g.Count())
						 .Select(g => g.Key)
						 .Take(12)
						 .ToList();
		}


		//public async Task<IActionResult> Details(int id, string title)
		//{
		//	var news = await _context.News
		//		.Include(n => n.Translations)
		//		.Include(n => n.Images)
		//		.Include(n => n.Attachments)
		//		.FirstOrDefaultAsync(n => n.Id == id);

		//	if (news == null)
		//		return NotFound();

		//	var translation = news.Translations.FirstOrDefault();
		//	var culture = RouteData.Values["culture"]?.ToString() ?? "fa";

		//	// متا تگ‌ها
		//	ViewBag.MetaTitle = $"{translation?.Title} | آروین پلیمر";
		//	ViewBag.MetaDescription = translation?.Summary?.Length > 160 ?
		//		translation.Summary.Substring(0, 160) + "..." :
		//		translation?.Summary;
		//	ViewBag.MetaKeywords = $"{translation?.Title}, اخبار پلیمر, صنعت پلیمر, تی پی ای, تی پی وی, کامپاند";
		//	ViewBag.OgImage = news.DefaultImageUrl;
		//	ViewBag.CanonicalUrl = $"/{culture}/news/{id}/{System.Web.HttpUtility.UrlEncode(translation?.Title?.Replace(" ", "-"))}";
		//	ViewBag.OgUrl = $"https://arvinpolymer.com/{culture}/news/{id}/{System.Web.HttpUtility.UrlEncode(translation?.Title?.Replace(" ", "-"))}";
		//	 ViewBag.product = _context

		//	return View(news);
		//}
	}
}
