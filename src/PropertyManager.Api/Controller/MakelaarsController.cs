using Microsoft.AspNetCore.Mvc;
using PropertyManager.Domain.Ports.UseCases;
using PropertyManager.Models;

namespace PropertyManager.Controller;

[ApiController]
[Route("/makelaars")]
public class MakelaarsController : ControllerBase
{
    private readonly IMakelaarsUsecase _makelaarsUsecase;
        
    public MakelaarsController(IMakelaarsUsecase makelaarsUsecase)
    {
        _makelaarsUsecase = makelaarsUsecase;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(List<MakelaarResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get([FromQuery] GetMakelaarsQuery query)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var result = await _makelaarsUsecase.GetMakelaars(query.ToDomain());
        return Ok(result.Select(MakelaarResponseDto.FromDomain));
    }
}