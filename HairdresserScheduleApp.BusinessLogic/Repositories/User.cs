using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HairdresserScheduleApp.BusinessLogic.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HairdresserScheduleApp.BusinessLogic.Repositories
{
    public class User:IUser
    {
        private readonly AppDbContext context;
        private readonly ILogger<User> logger;
        public User(AppDbContext context, ILogger<User> logger)
        {
            this.context = context;
            this.logger = logger;
        }
        public Task<IQueryable<Models.User>> GetAll(CancellationToken cancellationToken = default)
        {
            return ExecuteInTryCatch<IQueryable<Models.User>>(async () =>
            {
                return  this.context.Users;
            }, "GetAll Users");
        }

        public Task<IQueryable<Models.User>> GetAllByUserName(string username = default, CancellationToken cancellationToken = default)
        {
            return ExecuteInTryCatch<IQueryable<Models.User>>(async () =>
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    return await GetAll(cancellationToken);
                }
                return this.context.Users.Where(x=>x.Username.Contains(username));
            }, "GetAllByUserName Users");
        }

        public Task<Models.User> GetUserByUserName(string username, CancellationToken cancellationToken = default)
        {
            return ExecuteInTryCatch<Models.User>(async () =>
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    throw new ArgumentNullException(nameof(username));
                }
                return await this.context.Users.Include(x=>x.UserRole)
                    .SingleOrDefaultAsync(x => x.Username==username);
            }, "GetAllByUserName Users");
        }

        public Task<Models.User> GetUserById(int userId, CancellationToken cancellationToken = default)
        {
            return ExecuteInTryCatch<Models.User>(async () =>
            {
                if (userId<=0)
                {
                    throw new ArgumentNullException(nameof(userId));
                }
                return await this.context.Users.SingleOrDefaultAsync(x => x.Id == userId);
            }, "GetUserById Users");
        }

        public Task<bool> RegisterUser(Models.User user, CancellationToken cancellationToken = default)
        {
            return ExecuteInTryCatch<bool>(async () =>
            {
                var userRole =await this.context.Roles.SingleOrDefaultAsync(x => x.Id == user.UserRoleId);
                if (userRole == null)
                {
                    throw new ArgumentNullException(nameof(user.UserRole));
                }

                user.UserRole = userRole;

                await this.context.Users.AddAsync(user);

                return true;
            }, "GetAllByUserName Users");
        }


        private Task<T> ExecuteInTryCatch<T>(Func<Task<T>> databaseFunction, string errorMessage)
        {
            try
            {
                return databaseFunction();
            }
            catch (Exception e)
            {
                logger.LogError(e, errorMessage);
                throw;
            }
        }
    }

    public interface IUser
    {
        Task<IQueryable<Models.User>> GetAll(CancellationToken cancellationToken=default);
        Task<IQueryable<Models.User>> GetAllByUserName(string username=default,CancellationToken cancellationToken=default);
        Task<Models.User> GetUserByUserName(string username,CancellationToken cancellationToken=default);
        Task<Models.User> GetUserById(int userId,CancellationToken cancellationToken=default);
        Task<bool> RegisterUser(Models.User user,CancellationToken cancellationToken=default);
    }
}
