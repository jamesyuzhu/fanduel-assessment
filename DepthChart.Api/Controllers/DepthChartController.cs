using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using DepthChart.Api.Services;
using DepthChart.Api.Dtos.Requests;
using Microsoft.AspNetCore.Http;

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

        [HttpPost("add-player-to-depth-chart/{sportCode}/{teamCode}")]
        public async Task<IActionResult> AddPlayerToDepthChart(string sportCode, string teamCode, [FromBody] AddPlayerToDepthChartRequest request)
        {
            if (string.IsNullOrEmpty(sportCode))
            {
                return BadRequest("SportCode is required");
            }
            else if(string.IsNullOrEmpty(teamCode))
            {
                return BadRequest("TeamCode is required");
            }
            if (request == null)
            {
                return BadRequest("Invalid request data.");
            }

            try
            {
                var service = _serviceFactory.Create(sportCode, teamCode);
                var response = await service.AddPlayerToDepthChart(request, teamCode);
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

        //// GET: api/TodoItems/...
        //[HttpGet("{id}")]
        //[ActionName(nameof(GetTodoItemAsync))]
        //public async Task<IActionResult> GetTodoItemAsync(Guid id)
        //{
        //    var result = await _service.GetTodoItemAsync(id);

        //    if (result == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(result);
        //}

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
