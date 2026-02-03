using System.ComponentModel.DataAnnotations;
using pishrooAsp.Models.MessageTemplate;
namespace pishrooAsp.Models.Company
{
	// Models/Company.cs
	public class Company
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "نام شرکت الزامی است")]
		[Display(Name = "نام شرکت")]
		public string Name { get; set; }

		[EmailAddress(ErrorMessage = "فرمت ایمیل نامعتبر است")]
		[Display(Name = "ایمیل")]
		public string Email { get; set; }

		[Display(Name = "تلفن")]
		public string Phone { get; set; }

		[Display(Name = "شخص تماس")]
		public string ContactPerson { get; set; }

		[Display(Name = "تاریخ ایجاد")]
		public DateTime CreatedDate { get; set; } = DateTime.Now;

		[Display(Name = "فعال")]
		public bool IsActive { get; set; } = true;


		public virtual ICollection<pishrooAsp.Models.MessageTemplate.MessageTemplate> Templates { get; set; } = new List<pishrooAsp.Models.MessageTemplate.MessageTemplate>();
		public virtual ICollection<SentMessage> SentMessages { get; set; } = new List<SentMessage>();
	}
}
