using System.ComponentModel.DataAnnotations;

namespace pishrooAsp.Models.MessageTemplate
{
	// ViewModel برای ارسال پیام
	public class SendMessageViewModel
	{
		public int TemplateId { get; set; }
		public int CompanyId { get; set; }

		[Display(Name = "قالب")]
		public string TemplateName { get; set; }

		[Display(Name = "شرکت")]
		public string CompanyName { get; set; }

		[Required(ErrorMessage = "گیرنده الزامی است")]
		[Display(Name = "گیرنده")]
		public string Recipient { get; set; }

		[Display(Name = "موضوع")]
		public string Subject { get; set; }

		[Display(Name = "نوع پیام")]
		public string MessageType { get; set; } = "Email";

		public List<TokenViewModel> Tokens { get; set; } = new();

		[Display(Name = "پیش‌نمایش")]
		public string Preview { get; set; }
	}
}
