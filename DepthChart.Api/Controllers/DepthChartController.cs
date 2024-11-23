using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using DepthChart.Api.Services;
using DepthChart.Api.Dtos.Requests;
using Microsoft.AspNetCore.Http;
using DepthChart.Api.Exceptions;
using DepthChart.Api.Dtos.Responses;
using System.Collections.Generic;

namespace DepthChart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]     
    public class DepthChartController : ControllerBase
    {
        private readonly DepthChartServiceFactory _serviceFactory;
        private readonly ILogger<DepthChartController> _logger;

        public DepthChartController(DepthChartServiceFactory serviceFactory, ILogger<DepthChartController> logger)
        {
            _serviceFactory = serviceFactory;
            _logger = logger;
        }

        [HttpPost("{sportCode}/{teamCode}")]
        public async Task<IActionResult> AddPlayerToDepthChart(string sportCode, string teamCode, [FromBody] AddPlayerToDepthChartRequest request)
        {             
            try
            {
                var service = _serviceFactory.Create(sportCode, teamCode);
                var response = await service.AddPlayerToDepthChartAsync(request, teamCode);
                return Ok(response);
            }
            catch(ArgumentNullException anex)
            {
                _logger.LogError(anex, anex.Message);
                return BadRequest(anex.Message);
            }
            catch (ArgumentOutOfRangeException aorex)
            {
                _logger.LogError(aorex, aorex.Message);
                return BadRequest(aorex.Message);
            }
            catch (InvalidOperationException iex)
            {
                _logger.LogError(iex, iex.Message);
                return NotFound(iex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Problem(statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{sportCode}/{teamCode}")]
        public async Task<IActionResult> RemovePlayerFromDepthChart(string sportCode, string teamCode, [FromQuery] RemovePlayerFromDepthChartRequest request)
        { 
            try
            {
                var service = _serviceFactory.Create(sportCode, teamCode);
                var response = await service.RemovePlayerFromDepthChartAsync(request, teamCode);
                return Ok(response);                 
            }
            catch (ArgumentNullException aex)
            {
                _logger.LogError(aex, aex.Message);
                return BadRequest(aex.Message);
            }
            catch (PlayerNotInPositionException pex)
            {
                _logger.LogInformation(pex, pex.Message);
                return Ok(new PlayerResponse());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpGet("backups/{sportCode}/{teamCode}")]
        public async Task<IActionResult> GetBackUps(string sportCode, string teamCode, [FromQuery] GetBackUpsRequest request)
        {
            try
            {
                var service = _serviceFactory.Create(sportCode, teamCode);
                var response = await service.GetBackupsAsync(request, teamCode);
                return Ok(response);
            }
            catch (ArgumentNullException aex)
            {
                _logger.LogError(aex, aex.Message);
                return BadRequest(aex.Message);
            }
            catch (PlayerNotInPositionException pex)
            {
                _logger.LogInformation(pex, pex.Message);
                return Ok(new List<PlayerResponse>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        //// PUT: api/TodoItems/... 
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutTodoItemAsync(Guid id, Player todoItem)
        //{
        //    try
        //    {
        //        await _service.UpdateTodoItemAsync(id, todoItem);
        //    }
        //    catch (UpdateTodoItemIdNotMatchException uex)
        //    {
        //        _logger.LogError(uex, uex.Message);
        //        return BadRequest("The given Id doesn't match to the id of the given todoItem");
        //    }            
        //    catch (DbUpdateConcurrencyIdNotFoundException updIdEx)
        //    {
        //        _logger.LogError(updIdEx, updIdEx.Message);
        //        return Problem(statusCode: StatusCodes.Status500InternalServerError);
        //    }            
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, ex.Message);
        //        return Problem(statusCode: StatusCodes.Status500InternalServerError);
        //    }

        //    return NoContent();
        //} 

        //// POST: api/TodoItems 
        //[HttpPost]
        //public async Task<IActionResult> PostTodoItemAsync(Player todoItem)
        //{
        //    try
        //    {
        //        await _service.AddTodoItemAsync(todoItem);
        //    }
        //    catch (NewTodoItemMissDescriptionException mex)
        //    {
        //        _logger.LogError(mex, mex.Message);
        //        return BadRequest("Description is required");
        //    }
        //    catch (NewTodoItemDescriptionExistException eex)
        //    {
        //        _logger.LogError(eex, eex.Message);
        //        return BadRequest("Description already exists");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, ex.Message);
        //        return Problem(statusCode: StatusCodes.Status500InternalServerError);
        //    }

        //    return CreatedAtAction(nameof(GetTodoItemAsync), new { id = todoItem.Id }, todoItem);            
        //}         
    }
}
