using Microsoft.AspNetCore.Mvc;
using ProductionBoardApi.Models;
using ProductionBoardApi.Services;

namespace ProductionBoardApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductionBoardController : ControllerBase
{
    private readonly IProductionBoardService _service;

    public ProductionBoardController(IProductionBoardService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductionBoardResponse>>> GetProductionBoardAsync([FromQuery] ProductionBoardRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var results = await _service.GetProductionBoardAsync(request, cancellationToken);
        return Ok(results);
    }
}
