using System.ComponentModel.DataAnnotations;

namespace pishrooAsp.Models.MessageTemplate
{
	// Models/TemplateToken.cs
	public class TemplateToken
	{
		public int Id { get; set; }

		[Required]
		[Display(Name = "نام توکن")]
		public string TokenName { get; set; }

		[Display(Name = "نام نمایشی")]
		public string DisplayName { get; set; }

		[Display(Name = "توضیحات")]
		public string Description { get; set; }

		[Display(Name = "الزامی")]
		public bool IsRequired { get; set; }

		[Display(Name = "مقدار پیش‌فرض")]
		public string DefaultValue { get; set; }

		public int TemplateId { get; set; }
		public virtual MessageTemplate Template { get; set; }
	}
}
