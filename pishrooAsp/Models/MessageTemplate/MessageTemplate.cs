using System.ComponentModel.DataAnnotations;
using pishrooAsp.Models.Company;

namespace pishrooAsp.Models.MessageTemplate
{
	// Models/MessageTemplate.cs
	public class MessageTemplate
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "نام قالب الزامی است")]
		[Display(Name = "نام قالب")]
		public string Name { get; set; }

		[Required(ErrorMessage = "متن قالب الزامی است")]
		[Display(Name = "متن قالب")]
		[DataType(DataType.MultilineText)]
		public string TemplateText { get; set; }

		[Display(Name = "توضیحات")]
		public string Description { get; set; }

		[Display(Name = "دسته‌بندی")]
		public string Category { get; set; }

		[Display(Name = "تاریخ ایجاد")]
		public DateTime CreatedDate { get; set; } = DateTime.Now;

		[Display(Name = "آخرین ویرایش")]
		public DateTime? LastModified { get; set; }

		// کلید خارجی
		public int? CompanyId { get; set; }

		[Display(Name = "شرکت")]
		public virtual pishrooAsp.Models.Company.Company Company { get; set; } = new Company.Company();

		public virtual ICollection<TemplateToken> Tokens { get; set; } = new List<TemplateToken>();
	}

}
