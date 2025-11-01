namespace pishrooAsp.Models.Newses
{
	public class NewsAttachment
	{
		public int Id { get; set; }
		public int NewsId { get; set; }
		public string FileUrl { get; set; }
		public string FileName { get; set; }
		public string FileType { get; set; } // مثال: "PDF", "DOCX"
		public long FileSize { get; set; } // اندازه فایل (بر حسب بایت)

		// روابط
		public News News { get; set; }
	}
}
