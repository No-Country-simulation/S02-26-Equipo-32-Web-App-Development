using Application.Dtos.Roles;
using Application.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Roles
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    [Tags("Admin - Roles")]  
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var result = await _roleService.GetAllRolesAsync();

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            var result = await _roleService.GetRoleByIdAsync(id);

            if (result.IsSuccess)
                return Ok(result);

            return NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] RoleCreateDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _roleService.CreateRoleAsync(createDto);

            if (result.IsSuccess)
                return StatusCode(201, result);  // 👈 201 Created

            return BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] RoleUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _roleService.UpdateRoleAsync(id, updateDto);

            if (result.IsSuccess)
                return Ok(result);

            return NotFound(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var result = await _roleService.DeleteRoleAsync(id);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleDto assignDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _roleService.AssignRoleToUserAsync(assignDto);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpDelete("user/{userId}/role/{roleId}")]
        public async Task<IActionResult> RemoveRoleFromUser(int userId, int roleId)
        {
            var result = await _roleService.RemoveRoleFromUserAsync(userId, roleId);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetRolesByUserId(int userId)
        {
            var result = await _roleService.GetRolesByUserIdAsync(userId);

            if (result.IsSuccess)
                return Ok(result);

            return NotFound(result);
        }
    }
}