using System;
using System.ComponentModel.DataAnnotations;

public class VisitLog
{
	public int Id { get; set; }

	// آدرس IP بازدیدکننده
	[StringLength(45)] // برای IPv6
	public string IPAddress { get; set; }

	// مسیر (URL) صفحه بازدید شده
	[StringLength(255)]
	public string Path { get; set; }

	// جزئیات مرورگر و دستگاه (User-Agent)
	public string UserAgent { get; set; }

	// نوع دستگاه (مثلاً Mobile, Desktop, Tablet)
	[StringLength(50)]
	public string DeviceType { get; set; }

	// تاریخ و زمان بازدید
	public DateTime Timestamp { get; set; }

	// پرچم برای مشخص کردن بازدید یونیک (اختیاری)
	public bool IsUniqueVisit { get; set; }
}