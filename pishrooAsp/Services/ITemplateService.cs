using pishrooAsp.Models.MessageTemplate;

namespace pishrooAsp.Services
{
	// Services/ITemplateService.cs
	public interface ITemplateService
	{
		Task<string> GenerateMessageAsync(int templateId, Dictionary<string, string> tokens);
		Task<List<MessageTemplate>> GetTemplatesByCompanyAsync(int companyId);
		Task<MessageTemplate> GetTemplateWithTokensAsync(int templateId);
		Task<List<string>> ExtractTokensFromTemplate(string templateText);
		Task<List<TokenViewModel>> GetTokenViewModelsAsync(int templateId);
	}
}
