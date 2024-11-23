using System;
using System.ComponentModel.DataAnnotations;

namespace DepthChart.Api.Dtos.Requests
{
    public class AddPlayerToDepthChartRequest
    {
        [Required]
        public string PositionCode { get; set; }
        [Required]
        public int PlayerId { get; set; }
        [Required]
        public string PlayerName { get; set; }                
        public int? Depth { get; set; }
        /// <summary>
        /// The date that a chart is associated, e.g., a weekly chart may use the first week day.
        /// It is optional. If it is not present, the system will default to the current
        /// period, e.g., the first day of the current week
        /// </summary>
        public DateTime? ChartDate { get; set; }
    }
}
