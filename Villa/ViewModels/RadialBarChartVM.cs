namespace Villa.ViewModels
{
    public class RadialBarChartVM
    {
        public decimal TotalC {  get; set; }
        public decimal CountInCurrentMonth { get; set; }

        public bool IsIncreased { get; set; }

        public int[] Series { get; set; }
    }
}
