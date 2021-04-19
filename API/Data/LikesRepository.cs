using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        public DataContext Context { get; set; }
        public LikesRepository(DataContext context)
        {
            this.Context = context;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await Context.Likes.FindAsync(sourceUserId, likedUserId);
        }

        public async Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int userId)
        {
            var users = Context.Users.OrderBy(user => user.UserName).AsQueryable();
            var likes = Context.Likes.AsQueryable();

            if(predicate == "liked"){   //users that were liked by currently logged user
                likes = likes.Where(like => like.SourceUserId == userId);   //likes that were liked by current user 
                users = likes.Select(like => like.LikedUser);           //users that were liked
            }

             if(predicate == "likedBy"){   //users that have liked currently logged user
                likes = likes.Where(like => like.LikedUserId == userId);   //likes that were given to currently logger user
                users = likes.Select(like => like.SourceUser);           //users that liked someone
            }
            return await users.Select(user => new LikeDto   //in this case we didnt use automapper, we mapped user to LikedDto
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(photo => photo.IsMain).Url,
                City = user.City,
                Id = user.Id
            }).ToListAsync();
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await Context.Users
            .Include(user => user.LikedUsers)
            .FirstOrDefaultAsync(user => user.Id == userId);
        }
    }
}