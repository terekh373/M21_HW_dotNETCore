using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Shop_app.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Shop_app.Controllers.API
{
    //Error Auth 401
    [Route("api/[controller]")]
    [ApiController]
    public class APIUserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        public APIUserController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }
        /*
         * POST
         * http://localhost:5247/api/user/Register
         {
                "email": "admin@gmail.com",
                "password": "12345"
            }
         */
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var newUser = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true
                };
                //Assign role
                IdentityRole? existRole = await _roleManager.FindByNameAsync("user");
                if (existRole == null)
                {
                    return BadRequest("Not found Role ...");
                }
                //Create user
                var result_create_user = await _userManager.CreateAsync(newUser, model.Password);
                if (!result_create_user.Succeeded)
                {
                    return BadRequest(result_create_user.Errors);
                }
                IdentityUser? existUser = await _userManager.FindByNameAsync(newUser.UserName);
                if (existUser == null)
                {
                    return BadRequest("Not found User ...");
                }
                var result = await _userManager.AddToRoleAsync(existUser, existRole.Name);
                if (!result.Succeeded)
                {
                    return BadRequest("Asssign role error ...");
                }
                return Ok(new { status = 200, message = "User register successfully", role = "user" });
            }
            return BadRequest("Error validation model ...");
        }
        /*
         * POST
         * http://localhost:5247/api/user/Register
         {
                "email": "admin@gmail.com",
                "password": "12345"
            }
         */
        [HttpPost("auth")]
        public async Task<IActionResult> Auth([FromBody] LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if(user == null)
                {
                    return Unauthorized(new { message = "Invalid login attempt 1" }); 
                }
                var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                if(result.Succeeded)
                {
                    var token_jwt = GenerateJwtToken(user);
                    return Ok(new { 
                        message = "User logged in",
                        status = 200,
                        token = token_jwt
                    });
                }
                return Unauthorized(new { message = "Invalid login attempt 2" });
            }
            else
            {
                return BadRequest(new { message = "Error model" });
            }
        }
        private async Task<string> GenerateJwtToken(IdentityUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user); // 🟢 Отримуємо ролі

            var authClaims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email)
    };

            // 🟢 Додаємо ролі в claims
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:DurationInMinutes"])),
                claims: authClaims,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /*
         {
            "RoleId": "",
            "UserId": "",
            "RoleName": "user"
        }
         */
        //Roles create only admin
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "admin")]
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] Role role)
        {
            if (string.IsNullOrEmpty(role.RoleName))
            {
                return BadRequest("Error RoleName ...");
            }
            var existRoleName = await _roleManager.RoleExistsAsync(role.RoleName);
            if (existRoleName)
            {
                return BadRequest("This RoleName alredy exist ...");
            }
            var result = await _roleManager.CreateAsync(new IdentityRole(role.RoleName));
            if (result.Succeeded)
            {
                return Ok($"Role: {role.RoleName} created ...");
            }
            return BadRequest(result.Errors);
        }
        //Roles create only admin
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "admin")]
        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole(Role role)
        {
            if (
                string.IsNullOrEmpty(role.UserId) &&
                string.IsNullOrEmpty(role.RoleId) &&
                string.IsNullOrEmpty(role.RoleName)
                )
            {
                return BadRequest("Error assign role ...");
            }
            var existRole = await _roleManager.FindByIdAsync(role.RoleId);
            if (existRole == null)
            {
                return BadRequest("Not found Role ...");
            }
            var existUser = await _userManager.FindByIdAsync(role.UserId);
            if (existUser == null)
            {
                return BadRequest("Not found User ...");
            }
            var result = await _userManager.AddToRoleAsync(existUser, role.RoleName);
            if (result.Succeeded)
            {
                return Ok("Roles assigned ...");
            }
            return BadRequest(result.Errors);
        }
        [HttpPost("logout")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Logout()
        {
            return Ok(new { message = "Користувач успішно вийшов" });
        }
    }
}
