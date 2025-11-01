namespace pishrooAsp.ModelViewer.visitLog
{
	public class VisitStatistics
	{
		public int TotalVisits { get; set; }
		public int UniqueVisits { get; set; }
		public int TodayVisits { get; set; }
		public int TodayUniqueVisits { get; set; }
		public int ThisMonthVisits { get; set; }
		public List<DeviceStat> DeviceStats { get; set; } = new List<DeviceStat>();
		public List<PageStat> PopularPages { get; set; } = new List<PageStat>();
		public List<DailyStat> DailyStats { get; set; } = new List<DailyStat>();
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
	}

	public class DeviceStat
	{
		public string DeviceType { get; set; }
		public int Count { get; set; }
		public double Percentage { get; set; }
	}

	public class PageStat
	{
		public string Path { get; set; }
		public int VisitCount { get; set; }
		public int UniqueVisitCount { get; set; }
	}

	public class DailyStat
	{
		public DateTime Date { get; set; }
		public int TotalVisits { get; set; }
		public int UniqueVisits { get; set; }
	}
}
