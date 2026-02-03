using pishrooAsp.Data;
using pishrooAsp.Models.MessageTemplate;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace pishrooAsp.Services
{
	// Services/TemplateService.cs
	public class TemplateService : ITemplateService
	{
		private readonly AppDbContext _context;

		public TemplateService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<string> GenerateMessageAsync(int templateId, Dictionary<string, string> tokens)
		{
			var template = await _context.MessageTemplates
				.Include(t => t.Tokens)
				.FirstOrDefaultAsync(t => t.Id == templateId);

			if (template == null)
				throw new ArgumentException("قالب یافت نشد");

			string result = template.TemplateText;

			// جایگزینی توکن‌ها
			foreach (var token in tokens)
			{
				result = result.Replace($"{{{token.Key}}}", token.Value ?? string.Empty);
			}

			// بررسی توکن‌های ضروری
			var requiredTokens = template.Tokens
				.Where(t => t.IsRequired)
				.Select(t => t.TokenName)
				.ToList();

			var missingTokens = requiredTokens
				.Where(rt => !tokens.ContainsKey(rt) || string.IsNullOrEmpty(tokens[rt]))
				.ToList();

			if (missingTokens.Any())
			{
				throw new ArgumentException($"توکن‌های ضروری پر نشده‌اند: {string.Join(", ", missingTokens)}");
			}

			return result;
		}

		public async Task<List<string>> ExtractTokensFromTemplate(string templateText)
		{
			var pattern = @"\{(\w+)\}";
			var matches = Regex.Matches(templateText, pattern);

			return matches
				.Cast<Match>()
				.Select(m => m.Groups[1].Value)
				.Distinct()
				.OrderBy(t => t)
				.ToList();
		}

		public async Task<List<TokenViewModel>> GetTokenViewModelsAsync(int templateId)
		{
			var template = await GetTemplateWithTokensAsync(templateId);
			if (template == null) return new List<TokenViewModel>();

			var tokens = await ExtractTokensFromTemplate(template.TemplateText);

			return tokens.Select(tokenName => new TokenViewModel
			{
				TokenName = tokenName,
				DisplayName = template.Tokens?.FirstOrDefault(t => t.TokenName == tokenName)?.DisplayName ?? tokenName,
				IsRequired = template.Tokens?.FirstOrDefault(t => t.TokenName == tokenName)?.IsRequired ?? false,
				Value = template.Tokens?.FirstOrDefault(t => t.TokenName == tokenName)?.DefaultValue ?? string.Empty
			}).ToList();
		}

		public Task<List<MessageTemplate>> GetTemplatesByCompanyAsync(int companyId)
		{
			throw new NotImplementedException();
		}

		public Task<MessageTemplate> GetTemplateWithTokensAsync(int templateId)
		{
			throw new NotImplementedException();
		}
	}
}
