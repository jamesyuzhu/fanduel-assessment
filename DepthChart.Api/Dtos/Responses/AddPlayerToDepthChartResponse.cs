using System;

namespace DepthChart.Api.Dtos.Responses
{
    public class AddPlayerToDepthChartResponse
    {
        public string SportCode { get; set; }
        public string TeamCode { get; set; }
        public DateTime WeekStartDate { get; set; }
        public string PositionCode { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int Depth { get; set; }                      
    }
}
