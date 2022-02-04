using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interfaces;
using API.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Helpers;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        public LikesController(IUnitOfWork unitOfWork){
            _unitOfWork = unitOfWork;      
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username){
            var sourceUseruserName = User.GetUserName();
            var sourceUserId = await _unitOfWork.userRepository.GetUserByUserNameAsync(sourceUseruserName);
            var likedUser = await _unitOfWork.userRepository.GetUserByUserNameAsync(username);
            var sourceUser = await _unitOfWork.likesRepository.GetUserWithLikes(sourceUserId.Id);

            if(likedUser == null)return NotFound();

            if(sourceUser.UserName == username)return BadRequest("You Cannot Like yourself!");

            var userLike = await _unitOfWork.likesRepository.GetUserLike(sourceUserId.Id, likedUser.Id);

            if(userLike != null)return BadRequest("You Already Liked this User");

            userLike = new Entities.UserLike{
                SourceUserID = sourceUserId.Id,
                LikedUserId = likedUser.Id
            };
            sourceUser.LikedUsers.Add(userLike);
            if(await _unitOfWork.Complete()) return Ok();
            
            return BadRequest("Failed To Like User");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikedDto>>> GetUserLikes([FromQuery]LikesParams likesParams){
            var sourceUseruserName = User.GetUserName();
            var sourceUserId = await _unitOfWork.userRepository.GetUserByUserNameAsync(sourceUseruserName);
            likesParams.UserId = sourceUserId.Id;

            var users =  await _unitOfWork.likesRepository.GetUserLikes(likesParams);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(users);
        }
        
    }
}