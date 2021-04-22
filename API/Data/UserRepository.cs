

using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities;
using API.Data;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using System.Linq;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using API.Helpers;
using System;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;
        public UserRepository(DataContext context, IMapper mapper)
        {
            this.mapper = mapper;
            this.context = context;
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
            return await context.Users      //we need mapper to map entity to DTO. Also using DTO prevents from loop that we had in entite user has photo and photo has user => infinity
               .Where(x => x.UserName == username)  //Automapper is mapping from entity to DTO automatically
               .ProjectTo<MemberDto>(mapper.ConfigurationProvider)  //When we use ProjectTo we dont need to use Include that was making join
               .SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
               var query = context.Users.AsQueryable();

                query = query.Where(user => user.UserName != userParams.CurrentUsername);   //show users that are not me
                query = query.Where(user => user.Gender == userParams.Gender); 

                var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge);
                query = query.Where(user => user.DateOfBirth >= minDob && user.DateOfBirth <= maxDob); 
                query = userParams.OrderBy switch                               //modern switch statement
                {
                    "created" => query.OrderByDescending(user => user.Created), //this is for string "created"
                    _ => query.OrderByDescending(user => user.LastActive)       //_ means default option
                };

                return await PagedList<MemberDto>.CreateAsync(
                    query.ProjectTo<MemberDto>(mapper.ConfigurationProvider).AsNoTracking(), //we dont need to track because we wont modify these objects only download them
                    userParams.PageNumber, userParams.PageSize);
        }

        public async Task<AppUser> GetUserByIdAsync(int userId)
        {
            return await context.Users.FindAsync(userId);
        }
        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await context.Users
            .Include(p => p.Photos)     //to include another table into result (join)
            .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<string> GetUserGender(string username)
        {
            return await context.Users
            .Where(x => x.UserName == username)
            .Select(x => x.Gender)
            .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await context.Users
            .Include(p => p.Photos)
            .ToListAsync();
        }

        public void Update(AppUser user)
        {
            context.Entry(user).State = EntityState.Modified; //lets entity framework info that this was modified
        }
    }
}