// Services/CultureService.cs
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using pishrooAsp.Data;

namespace pishrooAsp.Services;

public class CultureService : ICultureService
{
	private readonly AppDbContext _context;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private List<string> _cultureCodesCache;

	public CultureService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
	{
		_context = context;
		_httpContextAccessor = httpContextAccessor;
	}

	// دریافت culture فعلی از Route
	public string GetCurrentCulture()
	{
		var httpContext = _httpContextAccessor.HttpContext;
		if (httpContext == null)
			return "fa";

		var routeCulture = httpContext.GetRouteValue("culture")?.ToString();
		if (!string.IsNullOrEmpty(routeCulture))
		{
			// بررسی معتبر بودن culture
			return ValidateAndGetCulture(routeCulture);
		}

		var requestCulture = httpContext.Features.Get<IRequestCultureFeature>();
		var culture = requestCulture?.RequestCulture.Culture.Name ?? "fa";
		return ValidateAndGetCulture(culture);
	}

	// بررسی و بازگشت culture معتبر
	public string ValidateAndGetCulture(string requestedCulture)
	{
		if (string.IsNullOrWhiteSpace(requestedCulture))
			return "fa";

		// نرمال‌سازی
		requestedCulture = requestedCulture.Trim().ToLower();

		// بررسی وجود در دیتابیس
		if (IsCultureExists(requestedCulture))
		{
			return requestedCulture;
		}

		// اگر وجود نداشت، پیش‌فرض
		return "fa";
	}

	// بررسی وجود culture در دیتابیس
	public bool IsCultureExists(string cultureCode)
	{
		if (string.IsNullOrWhiteSpace(cultureCode))
			return false;

		// از کش استفاده کن
		if (_cultureCodesCache == null)
		{
			_cultureCodesCache = _context.Langs
				.Select(l => l.Code.ToLower())
				.ToList();
		}

		return _cultureCodesCache.Contains(cultureCode.ToLower());
	}

	// بررسی تکراری نبودن کد culture
	public async Task<bool> IsCultureCodeUnique(string code)
	{
		if (string.IsNullOrWhiteSpace(code))
			return false;

		return !await _context.Langs.AnyAsync(l =>
			l.Code.ToLower() == code.ToLower());
	}

	// بررسی تکراری نبودن کد culture با حذف یک آیتم
	public async Task<bool> IsCultureCodeUnique(string code, int excludeId)
	{
		if (string.IsNullOrWhiteSpace(code))
			return false;

		return !await _context.Langs.AnyAsync(l =>
			l.Code.ToLower() == code.ToLower() && l.Id != excludeId);
	}

	// دریافت همه کدهای culture
	public List<string> GetAllCultureCodes()
	{
		if (_cultureCodesCache == null)
		{
			_cultureCodesCache = _context.Langs
				.Select(l => l.Code)
				.ToList();
		}
		return _cultureCodesCache;
	}
}