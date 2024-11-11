using LazyMoon.Model;
using LazyMoon.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

#nullable enable
namespace LazyMoon.Service.DBService
{
    public class DBUserService(AppDbContext context)
    {
        readonly protected AppDbContext context = context;

        public async Task<User?> GetUserOrNullAsync(string name)
        {
            return await context.Users.FirstOrDefaultAsync(x => x.Name == name);
        }

        public async Task<User?> SetUserOrNullAsync(UserDTO userDTO)
        {
            if (await context.Users.AnyAsync(x => x.Name == userDTO.Name && x.UserId == userDTO.UserId))
                return null;
            var addObject = await context.Users.AddAsync(new User() { UserId = userDTO.UserId, Name = userDTO.Name, Key = userDTO.Key });
            context.SaveChanges();
            return addObject.Entity;

        }

    }
}
