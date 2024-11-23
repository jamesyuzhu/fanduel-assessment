using System;

namespace DepthChart.Api.Models
{
    public class ChartPositionDepth
    {        
        public string SportCode { get; set; }
        public string TeamCode { get; set; }
        public DateTime ChartDate { get; set; }
        public string PositionCode { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int Depth { get; set; }
    }
}
