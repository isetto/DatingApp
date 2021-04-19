using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly ILikesRepository likesRepository;
        private readonly IUserRepository userRepository;
        public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
        {
            this.userRepository = userRepository;
            this.likesRepository = likesRepository;
        }
        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await userRepository.GetUserByUsernameAsync(username);
            var sourceUser = await likesRepository.GetUserWithLikes(sourceUserId);

            if(likedUser == null) return NotFound();
            if(sourceUser.UserName == username) BadRequest("You cannot like yourself");
            var userLike = await likesRepository.GetUserLike(sourceUserId, likedUser.Id);
            if(userLike != null) BadRequest("You already liked this user");
            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id
            };
            sourceUser.LikedUsers.Add(userLike);
            if(await userRepository.SaveAllAsync()) return Ok();
            return BadRequest("Failed to like the user");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes(string predicate)
        {
            var users = await likesRepository.GetUserLikes(predicate, User.GetUserId());
            return Ok(users);
        }
    }
}