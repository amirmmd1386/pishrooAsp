using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pishrooAsp.Data;
using pishrooAsp.Models.ProductRequest;
using System.Linq;
using System.Threading.Tasks;

namespace pishrooAsp.Controllers
{
	[AdminAuthFilter]
	public class MessageController : Controller
	{
		private readonly AppDbContext _context;

		public MessageController(AppDbContext context)
		{
			_context = context;
		}

		// GET: Message
		[AdminAuthFilter]
		public async Task<IActionResult> Index()
		{
			return View(await _context.Message.ToListAsync());
		}

		// GET: Message/Details/5
		[AdminAuthFilter]
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var message = await _context.Message
				.FirstOrDefaultAsync(m => m.Id == id);
			if (message == null)
			{
				return NotFound();
			}

			return View(message);
		}

		// GET: Message/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: Message/Create
		[HttpPost]
		//[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create( message MessageText)
		{
			try
			{
				
					_context.Add(MessageText);
					await _context.SaveChangesAsync();

					// اضافه کردن TempData برای پیام موفقیت
					TempData["Success"] = "پیام با موفقیت ثبت شد.";
					return RedirectToAction(nameof(Index));
				
				
			}
			catch (Exception ex)
			{
				// لاگ کردن خطا (در صورت نیاز)
				// _logger.LogError(ex, "Error creating message");

				// نمایش پیام خطا به کاربر
				TempData["Error"] = "خطایی در ثبت پیام رخ داده است. لطفاً مجدداً تلاش کنید.";
				return View(MessageText);
			}
		}

		// GET: Message/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var message = await _context.Message.FindAsync(id);
			if (message == null)
			{
				return NotFound();
			}
			return View(message);
		}

		// POST: Message/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Message")] message MessageText)
		{
			if (id != MessageText.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(MessageText);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!MessageExists(MessageText.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			return View(MessageText);
		}

		// GET: Message/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var message = await _context.Message
				.FirstOrDefaultAsync(m => m.Id == id);
			if (message == null)
			{
				return NotFound();
			}

			return View(message);
		}

		// POST: Message/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var message = await _context.Message.FindAsync(id);
			_context.Message.Remove(message);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool MessageExists(int id)
		{
			return _context.Message.Any(e => e.Id == id);
		}
	}
}