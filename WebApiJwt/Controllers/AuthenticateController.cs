using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UseCases.API.Authentication;
//using WebApiJwt.Auth;

namespace WebApiJwt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private IdentityUser? user;

        public AuthenticateController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            IdentityUser? user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                IList<string>? userRoles = await _userManager.GetRolesAsync(user);

                List<Claim>? authClaims = new()
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (string? userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                JwtSecurityToken? token = GetToken(authClaims);

                return Ok(new LoginResponse
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }
        private async Task<IActionResult> Register(RegisterModel model)
        {
            IdentityUser? userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new RegisterResponse { Status = "Error", Message = "UserName already exists!" });

            user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            IdentityResult? result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new RegisterResponse { Status = "Error", Message = "User creation failed!" + result.ToString() });
            return Ok();
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            if (await _roleManager.RoleExistsAsync(UserRoles.Admin) && ((List<IdentityUser>)await _userManager.GetUsersInRoleAsync(UserRoles.Admin)).Count > 0)
                return StatusCode(StatusCodes.Status500InternalServerError, new RegisterResponse { Status = "Error", Message = "Admin already exists!" });
            else
                // Если среди ролей нет UserRoles.Admin, то он добавляется в список ролей.
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            IActionResult result = await Register(model);
            if (result is not OkResult || user == null)
                return result;
            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            // Если среди ролей нет UserRoles.User, то он добавляется в список ролей.
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            if (await _roleManager.RoleExistsAsync(UserRoles.User))
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            return Ok(new RegisterResponse { Status = "Success", Message = "Admin, User created successfully!" });
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [Route("register-user")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterModel model)
        {
            IActionResult res = await Register(model);
            if (res is not OkResult || user == null)
                return res;

            // Если среди ролей нет UserRoles.User, то он добавляется в список ролей.
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            if (await _roleManager.RoleExistsAsync(UserRoles.User))
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            return Ok(new RegisterResponse { Status = "Success", Message = "User created successfully!" });
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            SymmetricSecurityKey? authSigningKey = new(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            JwtSecurityToken? token = new(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}
