// Services/ICultureService.cs
namespace pishrooAsp.Services;

public interface ICultureService
{
	string GetCurrentCulture();
	bool IsCultureExists(string cultureCode);
	Task<bool> IsCultureCodeUnique(string code);
	Task<bool> IsCultureCodeUnique(string code, int excludeId);
	List<string> GetAllCultureCodes();
	string ValidateAndGetCulture(string requestedCulture); // متد جدید
}