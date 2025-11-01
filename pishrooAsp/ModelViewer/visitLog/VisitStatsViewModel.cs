namespace pishrooAsp.ModelViewer.visitLog
{
	public class VisitStatsViewModel
	{
		public List<VisitDataPoint> DailyVisits { get; set; }
		public List<VisitDataPoint> DeviceTypeStats { get; set; }
		public int TotalVisits { get; set; }
		public int UniqueVisits { get; set; }
	}
}
