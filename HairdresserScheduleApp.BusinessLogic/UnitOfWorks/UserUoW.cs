using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HairdresserScheduleApp.BusinessLogic.Models;
using Microsoft.Extensions.Logging;

namespace HairdresserScheduleApp.BusinessLogic.UnitOfWorks
{
    public class UsersUoW : IUsersUoW
    {
        private readonly AppDbContext context;

        public UsersUoW(AppDbContext dbContext, Repositories.IUser user)
        {
            this.context = dbContext;
            Users = user;
        }

        public Repositories.IUser Users { get; set; }


        public void Dispose()
        {
            context.Dispose();
        }

        public async Task<int> Commit()
        {
            return await context.SaveChangesAsync();
        }
    }
    public interface IUsersUoW : IDisposable
    {
        Repositories.IUser Users { get; set; }

        Task<int> Commit();
    }

}
