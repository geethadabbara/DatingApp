using System.Collections.Generic;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DatingApp.API.Helpers;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [ApiController]
    [Route("api/user/{userId}/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IBaseRepository _baseRepository;
        private readonly IMapper _mapper;
        public MessagesController(IBaseRepository baseRepository, IMapper mapper)
        {
            _mapper = mapper;
            _baseRepository = baseRepository;
        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            var message = await _baseRepository.GetMessage(id);
            if (message == null)
                return NotFound();
            return Ok(message);
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            var messages = await _baseRepository.GetMessagesThread(userId, recipientId);

            var messageThread = _mapper.Map<IEnumerable<MessageResponseDto>>(messages);

            return Ok(messageThread);
        }
        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery] MessageParams messageParams)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            messageParams.UserId = userId;

            var messages = await _baseRepository.GetMessagesForUser(messageParams);
            var messagesResponse = _mapper.Map<IEnumerable<MessageResponseDto>>(messages);
            Response.AddPagination(messages.CurrentPageIndex, messages.PageSize, messages.TotalCount, messages.TotalPages);

            return Ok(messagesResponse);
        }



        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, CreateMessageDto messageDto)
        {
            var sender = await _baseRepository.GetUser(userId);
            if (sender.Id != userId)
                return Unauthorized();

            messageDto.SenderId = userId;

            var recipient = await _baseRepository.GetUser(messageDto.Recipientid);
            if (recipient == null)
                return BadRequest("Could not find user");

            Message message = _mapper.Map<Message>(messageDto);
            _baseRepository.Add(message);
            if (await _baseRepository.SaveAll())
            {
                var messageToReturn = _mapper.Map<MessageResponseDto>(message);
                return CreatedAtRoute("GetMessage", new { userId, id = message.Id }, messageToReturn);
            }
            throw new Exception("Creating message failed on Save");
            //return BadRequest("Failed to send message");

        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int id, int userId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            var message = await _baseRepository.GetMessage(id);
            if (message.SenderId == userId)
                message.SenderDeleted = true;

            if (message.RecipientId == userId)
                message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted)
                _baseRepository.Delete(message);
            if (await _baseRepository.SaveAll())
                return NoContent();
            throw new Exception("error deleting the message");
        }
        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var message = await _baseRepository.GetMessage(id);
            if (message.RecipientId != userId)
                return Unauthorized();

            message.IsRead = true;
            message.DateRead = DateTime.Now;
            if (await _baseRepository.SaveAll())
                return NoContent();

            throw new Exception("error updating the message as read");
        }
    }
}