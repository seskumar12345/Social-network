﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reactor.Core.Domain.Users;

namespace Reactor.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly UserManager<User> _userManager;
        private readonly string _currentUserId;

        public UserService(IHttpContextAccessor accessor, UserManager<User> userManager)
        {
            _accessor = accessor;
            _userManager = userManager;
            _currentUserId = GetCurrentUserIdAsync().Result;
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User> GetUserNameAsync(string username)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<string> GetCurrentUserIdAsync()
        {
            var user = await _userManager.GetUserAsync(_accessor.HttpContext.User);

            return await _userManager.GetUserIdAsync(user);
        }

        public async Task<string> GetCurrentUserNameAsync()
        {
            var user = await _userManager.GetUserAsync(_accessor.HttpContext.User);

            return await _userManager.GetUserNameAsync(user);
        }

        public IQueryable<User> GetAllUsers()
        {
            return _userManager.Users;
        }

        public IQueryable<User> GetAllUsersExceptCurrentUser()
        {
            return _userManager.Users.Where(u => u.Id != _currentUserId);
        }

        public Task<User> GetUserWithFriendsAsync()
        {
            return _userManager.Users
                .Include(u => u.SentFriendRequests)
                .ThenInclude(f => f.RequestedTo)
                .Include(u => u.ReceievedFriendRequests)
                .ThenInclude(f => f.RequestedBy)
                .FirstOrDefaultAsync(u => u.Id == _currentUserId);
        }

        public async Task<string> GetUserProfileAsync()
        {
            var user = await GetUserByIdAsync(_currentUserId);

            return user.GetPicture();
        }
    }
}