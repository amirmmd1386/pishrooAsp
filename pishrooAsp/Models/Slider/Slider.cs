namespace pishrooAsp.Models.Slider
{
	// جدول اصلی اسلایدر
	public class Slider
	{
		public int Id { get; set; }

		// آدرس یا نام فایل تصویر
		public string ImageUrl { get; set; }

		// لینک به صفحه یا آدرس خارجی
		public string Link { get; set; }

		// ارتباط با ترجمه‌ها
		public ICollection<SliderTranslation> Translations { get; set; }
	}
	
}
