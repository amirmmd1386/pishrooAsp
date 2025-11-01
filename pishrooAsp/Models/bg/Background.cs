using System;
using System.ComponentModel.DataAnnotations;

namespace pishrooAsp.Models
{
	public class Background
	{
		[Key]
		public Guid Id { get; set; }

		[Required(ErrorMessage = "آدرس فایل الزامی است")]
		[MaxLength(500)]
		public string FilePath { get; set; } // مسیر ذخیره فایل در wwwroot/uploads

		[MaxLength(200)]
		public string Title { get; set; } // توضیح یا نام بکگراند (اختیاری)

		public bool IsActive { get; set; } = true; // فعال/غیرفعال کردن بکگراند

		public DateTime CreatedAt { get; set; } = DateTime.Now; // تاریخ آپلود
	}
}
