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
using AutoMapper;

namespace DatingApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repository;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        public AuthController(IAuthRepository repository, IConfiguration config, IMapper mapper)
        {
            _mapper = mapper;
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
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var user = _mapper.Map<ListUserDto>(authUser);
            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                user
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
                return BadRequest("username already exists");
            var userTocreate = _mapper.Map<User>(userDto);
            var createdUser = await _repository.Register(userTocreate, userDto.Password);
            var userToReturn = _mapper.Map<DetailedUserDto>(createdUser);
            return CreatedAtRoute("GetUser", new { Controller = "User", id = createdUser.Id }, userToReturn);

        }
    }
}