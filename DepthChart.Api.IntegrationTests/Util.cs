using DepthChart.Api.Dtos.Responses;
using DepthChart.Api.Models;
using DepthChart.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DepthChart.Api.IntegrationTests
{
    public class Util
    {
        private readonly string _sportCode;
        private readonly string _teamCode;      
         
        public Util(string sportCode, string teamCode)
        {
            _sportCode = sportCode;
            _teamCode = teamCode;                          
        }

        public async Task<ChartPositionDepth> CreateChartPositionDepthRecord(string positionCode, int playerId, int depth, DepthChartDbContext context, string playerName = "Tester")
        {
            var today = DateTime.Today;

            var record = context.ChartPositionDepths.Add(new Models.ChartPositionDepth
            {
                PositionCode = positionCode,
                PlayerId = playerId,
                PlayerName = playerName,
                Depth = depth,
                WeekStartDate = today.AddDays(-(int)today.DayOfWeek),
                TeamCode = _teamCode,
                SportCode = _sportCode
            });
            await context.SaveChangesAsync();
            return record.Entity;             
        }

        public async Task<ChartPositionDepth?> GetExistingPlayer(string positionCode, DateTime weekStartDate, int playerId, DepthChartDbContext context)
        {
            var existingPlayer = await context.ChartPositionDepths.FirstOrDefaultAsync(
                x => x.SportCode == _sportCode
                && x.TeamCode == _teamCode
                && x.WeekStartDate == weekStartDate
                && x.PositionCode == positionCode
                && x.PlayerId == playerId);

            if (existingPlayer != null)
            {
                await context.Entry(existingPlayer).ReloadAsync();
            }
            
            return existingPlayer;
        }

        public async Task<T> GetResponseData<T>(HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<T>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return responseData;
        }
    }
}
