using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WorkFlow.Application.DTOs;
using WorkFlow.Application.Helpers;
using WorkFlow.Application.Services;
using WorkFlow.Application.Settings;
using WorkFlow.Domain.Entities;
using WorkFlow.Domain.Interfaces.Repositories;

namespace WorkFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly JwtSettings _jwtSettings;
        public AuthController(IUserRepository userRepository, IJwtTokenService jwtTokenService, IOptions<JwtSettings> jwtSettings)
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> Register([FromBody] RegisterUserDto registerDto)
        {
            var emailExists = await _userRepository.EmailExistsAsync(registerDto.Email);
            if(emailExists)
            {
                return BadRequest(ApiResponse<UserResponseDto>.FailureResponse("Email already exist"));
            }

            var hashedPassword = PasswordHelper.HashPassword(registerDto.Password);

            var user = new User
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                PasswordHash = hashedPassword,
                CreatedDate = DateTime.UtcNow
            };

            var createdUser = await _userRepository.AddAsync(user);

            var userDto = new UserResponseDto
            {
                Id = createdUser.Id,
                Name = createdUser.Name,
                Email = createdUser.Email,
                CreatedDate = createdUser.CreatedDate
            };

            return CreatedAtAction(nameof(GetProfile), new { }, ApiResponse<UserResponseDto>.SuccessResponse(userDto,"User registered successful"));
        }
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);

            if(user == null)
            {
                return Unauthorized(ApiResponse<LoginResponseDto>.FailureResponse("Invalid email or password"));
            }

            var isPasswordValid = PasswordHelper.VerifyPassword(loginDto.Password, user.PasswordHash);
            if(!isPasswordValid)
            {
                return Unauthorized(ApiResponse<LoginResponseDto>.FailureResponse("Invalid email or password"));
            }

            var token = _jwtTokenService.GenerateToken(user);

            var loginResponse = new LoginResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes)
            };

            return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(loginResponse, "Login successful"));
        }
        [HttpGet("profile")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> GetProfile()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if(string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(ApiResponse<UserResponseDto>.FailureResponse("Invalid token"));
            }
            var user = await _userRepository.GetByIdAsync(userId);

            if(user == null)
            {
                return NotFound(ApiResponse<UserResponseDto>.FailureResponse("User not found"));
            }

            var userDto = new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedDate = user.CreatedDate
            };
            return Ok(ApiResponse<UserResponseDto>.SuccessResponse(userDto, "Profile retrieved successfully"));
        }
    }
}
