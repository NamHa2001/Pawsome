using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pawsome.API.Common;
using Pawsome.API.DTOs.DonHang;
using Pawsome.API.Services.DonHang;

namespace Pawsome.API.Controllers.DonHang;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PawPointsController : ControllerBase
{
    private readonly IPawPointsService _pawPointsService;

    public PawPointsController(IPawPointsService pawPointsService)
    {
        _pawPointsService = pawPointsService;
    }

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

    [HttpGet("balance")]
    public async Task<IActionResult> GetBalance()
    {
        var result = await _pawPointsService.GetBalanceAsync(CurrentUserId);
        return Ok(ApiResponse<PawPointsBalanceDto>.Ok(result));
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory([FromQuery] PawPointsHistoryFilterDto filter)
    {
        var result = await _pawPointsService.GetHistoryAsync(CurrentUserId, filter);
        return Ok(ApiResponse<PagedResult<PawPointsTransactionDto>>.Ok(result));
    }
}