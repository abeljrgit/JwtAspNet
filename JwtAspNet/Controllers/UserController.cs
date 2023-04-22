using JwtAspNet.Dto;
using JwtAspNet.Services;
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
        public async Task registerUser(RegisterDto registerDto)
        {
            await _userService.RegisterUser(registerDto);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> loginUser(LoginDto loginDto)
        {
            Tuple<UserDto?, string> user = await _userService.LoginUser(loginDto);

            if (user.Item1 == null)
                return BadRequest(user.Item2);

            return Ok(user.Item1);
        }
    }
}
