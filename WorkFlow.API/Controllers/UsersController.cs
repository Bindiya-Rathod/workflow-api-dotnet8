using Microsoft.AspNetCore.Mvc;
using WorkFlow.Application.DTOs;
using WorkFlow.Application.Helpers;
using WorkFlow.Domain.Entities;
using WorkFlow.Domain.Interfaces.Repositories;

namespace WorkFlow.API.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> Register([FromBody] RegisterUserDto  registerDto)
        {
            var emailExists = await _userRepository.EmailExistsAsync(registerDto.Email);
            if(emailExists)
            {
                return BadRequest(ApiResponse<UserResponseDto>.FailureResponse("Email already exists"));
            }

            var hashedPassward = PasswordHelper.HashPassword(registerDto.Password);

            var User = new User
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                PasswordHash = hashedPassward,
                CreatedDate = DateTime.UtcNow
            };
            var createdUser = await _userRepository.AddAsync(User);

            var userDto = new UserResponseDto
            {
                Id = createdUser.Id,
                Name = createdUser.Name,
                Email = createdUser.Email,
                CreatedDate = createdUser.CreatedDate
            };

            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id},ApiResponse<UserResponseDto>.SuccessResponse(userDto, "User registered successfully"));
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> GetUserById(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
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

            return Ok(ApiResponse<UserResponseDto>.SuccessResponse(userDto, "User retrieved successfully"));
        }
        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserResponseDto>>>> GetAllUsers()
        {
            var users = await _userRepository.GetAllAsync();

            var userDtos = users.Select(u => new UserResponseDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                CreatedDate = u.CreatedDate
            }).ToList();

            return Ok(ApiResponse<IEnumerable<UserResponseDto>>.SuccessResponse(userDtos, "Users retrieved successfully"));
        }
    }
}
