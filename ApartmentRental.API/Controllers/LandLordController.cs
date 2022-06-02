using ApartmentRental.Core.DTO;
using ApartmentRental.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApartmentRental.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LandLordController : ControllerBase
{
    private readonly ILandLordService _landLordService;

    public LandLordController(ILandLordService landLordService)
    {
        _landLordService = landLordService;
    }

    [HttpPost("Create")]
    public async Task<IActionResult> CreateNewLandLordAccount([FromBody] LandLordCreationRequestDto dto)
    {
        await _landLordService.AddNewLandLordAccount(dto);
        return NoContent();
    }
}