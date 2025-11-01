using pishrooAsp.Models.Newses;

namespace pishrooAsp.Models.WhyUss
{
	public class WhyUsItem
	{
		public int Id { get; set; }
	
		public string Icon { get; set; }    
		public ICollection<WhyUsTranslation>? Translations { get; set; }
		// نام کلاس آیکون یا مسیر تصویر
	}

}
