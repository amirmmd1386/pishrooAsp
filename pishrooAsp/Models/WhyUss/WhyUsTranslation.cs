using pishrooAsp.Migrations;
using pishrooAsp.Models.Newses;
using System.ComponentModel.DataAnnotations;

namespace pishrooAsp.Models.WhyUss
{
	public class WhyUsTranslation
	{
		[Key]
		public int Id { get; set; }
		public string Title { get; set; }           // عنوان
		public string Description { get; set; }     // متن توضیحی
		public int whyusId { get; set; }
		public int LangId { get; set; }
		public WhyUsItem whyus { get; set; }
		public Lang Lang { get; set; }
	}
}
