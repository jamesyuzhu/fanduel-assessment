using System;

namespace DepthChart.Api.Models
{
    public class DepthChart
    {
        public Guid Id { get; set; }
        // Assume week starts from Sunday
        public DateTime WeekStartDate { get; set; }
        public string TeamCode { get; set; }        
    }
}
