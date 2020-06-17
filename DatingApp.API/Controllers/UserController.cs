using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using DatingApp.API.Helpers;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IBaseRepository _repository;
        private readonly IMapper _mapper;
        public UserController(IBaseRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
        {
            int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            User currentUser = await _repository.GetUser(currentUserId);
            userParams.UserId = currentUserId;
            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = currentUser.Gender == "male" ? "female" : "male";
            }
            var users = await _repository.GetUsers(userParams);
            var result = _mapper.Map<IEnumerable<ListUserDto>>(users);
            Response.AddPagination(users.CurrentPageIndex, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(result);
        }
        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repository.GetUser(id);
            var result = _mapper.Map<DetailedUserDto>(user);
            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, UpdateUserDto userDto)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            var user = await _repository.GetUser(id);
            _mapper.Map(userDto, user);
            if (await _repository.SaveAll())
                return NoContent();
            throw new Exception("updating user {id} failed on save");
        }

        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var like = await _repository.GetLike(id, recipientId);
            if (like != null)
                return BadRequest("You already liked this user");

            if (await _repository.GetUser(recipientId) == null)
                return NotFound();

            like = new Like
            {
                LikerId = id,
                LikeeId = recipientId
            };
            _repository.Add<Like>(like);
            if (await _repository.SaveAll())
                return Ok();

            return BadRequest("Failed to like user");


        }
    }
}