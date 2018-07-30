using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ShopApi.Data;
using ShopApi.Dtos;
using ShopApi.Filters;
using ShopApi.Models;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ValidateModel]
    public class AuthController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IAuthRepository _authRepo;
        public AuthController(IAuthRepository authRepo, IConfiguration config)
        {
            _config = config;
            _authRepo = authRepo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserForRegisterDto userForRegisterDto)
        {
            if (!string.IsNullOrEmpty(userForRegisterDto.UserName))
                userForRegisterDto.UserName = userForRegisterDto.UserName.ToLower();

            if (await _authRepo.UserExists(userForRegisterDto.UserName))
            {
                ModelState.AddModelError("UserName", "User exists");
            }

            var newUser = new User
            {
                UserName = userForRegisterDto.UserName
            };

            var createUser = await _authRepo.Register(newUser, userForRegisterDto.Password);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserForLoginDto userForLoginDto)
        {
            var user = await _authRepo.Login(userForLoginDto.UserName.ToLower(), userForLoginDto.Password);

            if (user == null)
            {
                return Unauthorized();
            }

            //generate toke
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config.GetSection("AppSettings:Token").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]{
                    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                    new Claim(ClaimTypes.Name ,user.UserName)
                }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)

            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { tokenString });
        }
    }
}