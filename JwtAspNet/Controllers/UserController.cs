using JwtAspNet.Dto;
using JwtAspNet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAspNet.Controllers
{
    [Route("/api/jwt")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponseDto>> registerUser(RegisterRequestDto registerDto)
        {
            RegisterResponseDto? user = await _userService.RegisterUser(registerDto);

            if (user == null)
                return BadRequest("Something bad happened.");

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> loginUser(LoginRequestDto loginDto)
        {
            Tuple<LoginResponseDto?, string> user = await _userService.LoginUser(loginDto);

            if (user.Item1 == null)
                return BadRequest(user.Item2);

            return Ok(user.Item1);
        }

        [HttpGet("test"), Authorize]
        public string test()
        {
            return "THIS IS TEST CONTROLLER --ABEL";
        }
    }
}
