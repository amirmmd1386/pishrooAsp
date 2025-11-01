using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pishrooAsp.Models
{
	public class AboutUsTranslation
	{
		[Key]
		public int Id { get; set; }

		// عنوان یا شعار به زبان خاص
		public string? Title { get; set; }

		// متن توضیح درباره شرکت (ترجمه شده)
		public string? Description { get; set; }

		public string? CompanyName { get; set; }
		public string? Slogan { get; set; }
		public string? Address { get; set; }
		public string? Address2 { get; set; }
		public string? CEOName { get; set; }

		// کلید خارجی به جدول اصلی AboutUsItem
		[ForeignKey("AboutUsItem")]
		public int AboutUsId { get; set; }
		public AboutUsItem? AboutUsItem { get; set; }

		// کلید خارجی به جدول زبان‌ها (Lang)
		[ForeignKey("Lang")]
		public int LangId { get; set; }
		public Lang? Lang { get; set; }
	}
}
