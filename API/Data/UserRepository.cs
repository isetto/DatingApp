

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

        public async Task<IEnumerable<MemberDto>> GetMembersAsync()
        {
                return await context.Users
               .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
               .ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync()
        {
            return await context.Users.FindAsync();
        }
        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await context.Users
            .Include(p => p.Photos)     //to include another table into result (join)
            .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await context.Users
            .Include(p => p.Photos)
            .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            context.Entry(user).State = EntityState.Modified; //lets entity framework info that this was modified
        }
    }
}