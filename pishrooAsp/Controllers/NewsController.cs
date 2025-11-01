using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pishrooAsp.Data;
using pishrooAsp.Models.Newses;
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

		public async Task<IActionResult> Details(int id)
		{
			var news = await _context.News
				.Include(n => n.Translations)
				.Include(n => n.Images)
				.Include(n => n.Attachments)
				.FirstOrDefaultAsync(n => n.Id == id);

			if (news == null)
				return NotFound();

			return View(news);
		}

	}
}
