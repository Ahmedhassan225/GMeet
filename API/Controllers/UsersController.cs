using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extentions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository; 
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
            _mapper = mapper;
            _userRepository = userRepository;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GETUsers(){
            return Ok(await _userRepository.GetMembersAsync());
        }

        [Authorize]
        [HttpGet("{username}", Name = "GETUser")]
        public async Task<ActionResult<MemberDto>> GETUser(string username){
            return await _userRepository.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.GetUserName();
            var user = await _userRepository.GetUserByUserNameAsync(username);

            _mapper.Map(memberUpdateDto, user);
    
            _userRepository.Update(user);

            if (await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file){

            var user = await _userRepository.GetUserByUserNameAsync(User.GetUserName());
            var result = await _photoService.AddPhotoAsync(file);

            if(result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo{
                Url = result.SecureUrl.AbsoluteUri,
                publicId = result.PublicId
            };

            if(user.Photos.Count == 0)
                photo.IsMain = true;

            user.Photos.Add(photo);
            if(await _userRepository.SaveAllAsync()){

                return CreatedAtRoute("GetUser",new {username = user.UserName}, _mapper.Map<PhotoDto>(photo));
            }
                
            return BadRequest("Problem adding a Photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> setMainPhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUserName());

            var newPhoto = user.Photos.FirstOrDefault<Photo>(x => x.Id == photoId);
            if(newPhoto.IsMain) return BadRequest("This Photo Already is the Main Photo!");

            var currentPhoto = user.Photos.FirstOrDefault<Photo>(x => x.IsMain);
            if(currentPhoto != null) currentPhoto.IsMain = false;

            newPhoto.IsMain = true;
            
            if (await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to User's Main Photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> deletePhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUserName());

            var deletePhoto = user.Photos.FirstOrDefault(x => x.Id == photoId);
            
            if(deletePhoto == null) return NotFound();
            if(deletePhoto.IsMain) return BadRequest("Can't Delete the Main Photo!");

            if(deletePhoto.publicId != null){
                var result = await _photoService.DeletePhotoAsync(deletePhoto.publicId);
                if(result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(deletePhoto);
            
            if (await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to Delete the Photo");
        }
    }
}