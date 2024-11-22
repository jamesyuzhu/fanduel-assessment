using System;

namespace DepthChart.Api.Dtos.Responses
{
    public class AddPlayerToDepthChartResponse
    {
        public string SportCode { get; set; }
        public string TeamCode { get; set; }
        public DateTime WeekStartDate { get; set; }
        public string PositionCode { get; set; }
        public int PlayId { get; set; }
        public string PlayName { get; set; }
        public int Depth { get; set; }                      
    }
}
