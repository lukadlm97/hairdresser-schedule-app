using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairdresserScheduleApp.BusinessLogic.Services
{
    public class User:IUser
    {
        private readonly UnitOfWorks.IUsersUoW usersUoW;
        private readonly IJwtService jwtService;

        public User(UnitOfWorks.IUsersUoW usersUoW,IJwtService jwtService)
        {
            this.usersUoW = usersUoW;
            this.jwtService = jwtService;
        }
        public async Task<IQueryable<Models.User>> GetAll(CancellationToken cancellationToken = default)
        {
            return await  this.usersUoW.Users.GetAll(cancellationToken);
        }

        public async Task<bool> Register(string email,string username, string password, CancellationToken cancellationToken = default)
        {
            var userToVerify = await this.usersUoW.Users.GetUserByUserName(username.ToLower(), cancellationToken);

            if (userToVerify != null)
            {
                return false;
            }

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            var newUser = new Models.User()
            {
                Email = email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Username = username.ToLower(),
                UserRoleId = 2
            };

            await this.usersUoW.Users.RegisterUser(newUser, cancellationToken);

            return await this.usersUoW.Commit() == 1;
        }

        public async Task<string> LogIn(string username, string password, CancellationToken cancellationToken = default)
        {
            var userToVerify = (await this.usersUoW.Users.GetUserByUserName(username.ToLower(), cancellationToken));

            if (userToVerify == null)
            {
                return null;
            }

            if (!VerifyPasswordHash(password, userToVerify.PasswordHash, userToVerify.PasswordSalt))
            {
                return null;
            }

            return jwtService.GenerateJwt(userToVerify, userToVerify.UserRole.RoleName == "admin");

        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                        return false;
                }
                return true;
            }
        }
    }

    public interface IUser
    {
        Task<IQueryable<Models.User>> GetAll(CancellationToken cancellationToken=default);
        Task<bool> Register(string email,string username, string password,CancellationToken cancellationToken=default);
        Task<string> LogIn(string username, string password,CancellationToken cancellationToken=default);
    }
}
