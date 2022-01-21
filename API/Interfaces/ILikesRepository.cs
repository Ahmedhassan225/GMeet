using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int LikedUserId);

        Task<AppUser> GetUserWithLikes(int userId);
        Task<IEnumerable<LikedDto>> GetUserLikes(string predicate, int userId);
    }
}