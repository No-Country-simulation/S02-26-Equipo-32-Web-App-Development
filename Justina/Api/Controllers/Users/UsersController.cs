using Application.Dtos.Users;
using Application.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Users
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]  // Solo Admin
    [Tags("Admin - Usuarios")]    
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAllUsersAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.CreateUserAsync(createDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserAdminUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.UpdateUserAsync(id, updateDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPatch("{id}/enable")]
        public async Task<IActionResult> EnableUser(int id)
        {
            var result = await _userService.EnableUserAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPatch("{id}/disable")]
        public async Task<IActionResult> DisableUser(int id)
        {
            var result = await _userService.DisableUserAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}