namespace pishrooAsp.Models.Slider
{
	public class SliderTranslation
	{
		public int Id { get; set; }

		public int SliderId { get; set; }
		public Slider Slider { get; set; }

		public int LangId { get; set; }
		public Lang Lang { get; set; } // همون جدول زبان که قبلاً داری

		public string Title { get; set; }
		public string ShortDescription { get; set; }
	}
}
