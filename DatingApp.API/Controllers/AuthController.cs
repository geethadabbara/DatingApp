using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace DatingApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repository;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repository, IConfiguration config)
        {
            _config = config;
            _repository = repository;

        }
        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginUserDto loginUserDto)
        {
            var authUser = await _repository.Login(loginUserDto.UserName, loginUserDto.Password);
            if (authUser == null)
                return Unauthorized();
            var claims = new[]{
                new Claim(ClaimTypes.NameIdentifier,authUser.Id.ToString() ),
                new Claim(ClaimTypes.Name, authUser.UserName)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSetting:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new
            {
                token = tokenHandler.WriteToken(token)
            });
        }

        [HttpPost()]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterUserDto userDto)
        {
            // validate request
            // if(!ModelState.IsValid)
            // return BadRequest(ModelState);

            userDto.UserName = userDto.UserName.ToLower();
            if (await _repository.UserExists(userDto.UserName))
            {
                return BadRequest("username already exists");
            }
            var userTocreate = new User()
            {
                UserName = userDto.UserName
            };
            var createdUser = _repository.Register(userTocreate, userDto.Password);

            return StatusCode(201);

        }
    }
}