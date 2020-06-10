using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/user/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IBaseRepository _baseRepository;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary cloudinary;

        public PhotosController(IBaseRepository baseRepository,
        IMapper mapper,
         IOptions<CloudinarySettings> options)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
            _cloudinaryConfig = options;

            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            cloudinary = new Cloudinary(acc);

        }
        [HttpPost]
        public async Task<IActionResult> AddPhoto(int userId, [FromForm] CreatePhotoDto createPhotoDto)
        {

            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            User userFromRepo = await _baseRepository.GetUser(userId);
            //createPhotoDto.File = formFile;
            IFormFile file = createPhotoDto.File;
            ImageUploadResult uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };
                    uploadResult = cloudinary.Upload(uploadParams);
                }
            }
            createPhotoDto.Url = uploadResult.Uri.ToString();
            createPhotoDto.PublicId = uploadResult.PublicId;

            Photo photo = _mapper.Map<Photo>(createPhotoDto);

            if (!userFromRepo.Photos.Any(x => x.IsMain))
                photo.IsMain = true;


            userFromRepo.Photos.Add(photo);
            if (await _baseRepository.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotosDto>(photo);
                return CreatedAtRoute("GetPhoto", new { userId = userId, id = photo.Id }, photoToReturn);
            }
            return BadRequest("Couldn't add the photo");

        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            Photo photoFromRepo = await _baseRepository.GetPhoto(id);

            var photo = _mapper.Map<PhotosDto>(photoFromRepo);

            return Ok(photo);
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            User userFromRepo = await _baseRepository.GetUser(userId);
            if (!userFromRepo.Photos.Any(x => x.Id == id))
                return Unauthorized();

            Photo photo = await _baseRepository.GetPhoto(id);
            if (photo.IsMain)
                return BadRequest("This is already main photo");

            Photo currentMainPhoto = await _baseRepository.GetMainPhotoForUser(userId);
            currentMainPhoto.IsMain = false;

            photo.IsMain = true;
            if (await _baseRepository.SaveAll())
                return NoContent();

            return BadRequest("Could not set the main photo");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int userId, int id)
        {

            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            User userFromRepo = await _baseRepository.GetUser(userId);
            if (!userFromRepo.Photos.Any(p => p.Id == id))
                return Unauthorized();
            Photo photoFromRepo = await _baseRepository.GetPhoto(id);

            if (photoFromRepo.IsMain)
                return BadRequest("You cannot delete your main");
            if (photoFromRepo.PublicId != null)
            {
                var deletionParams = new DeletionParams(photoFromRepo.PublicId);
                var deletionResult = cloudinary.Destroy(deletionParams);
                if (deletionResult.Result == "ok")
                {
                    _baseRepository.Delete(photoFromRepo);
                }
            }
            else
            {
                _baseRepository.Delete(photoFromRepo);
            }
            if (await _baseRepository.SaveAll())
                return Ok();
            return BadRequest("Failed to delete the photo");

        }
    }
}