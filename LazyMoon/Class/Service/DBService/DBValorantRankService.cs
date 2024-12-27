using LazyMoon.Model;
using LazyMoon.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

#nullable enable
namespace LazyMoon.Class.Service.DBService
{
    public class DBValorantRankService(AppDbContext context)
    {
        readonly protected AppDbContext context = context;

        public async Task<ValorantRank?> GetValorantRankOrNullAsync(string name)
        {
            var user = await context.Users.Include(x => x.ValorantRank).FirstOrDefaultAsync(x => x.Name == name);
            if (user == null)
                return null;
            if (user.ValorantRank == null)
                return SetDefaultRank(user);
            return user.ValorantRank;
        }

        public async Task<ValorantRank?> SetValorantRankOrNullAsync(string name, string nickName, string tag)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Name == name);

            if (user == null)
                return null;

            var valorantRank = user.ValorantRank;

            valorantRank ??= SetDefaultRank(user);

            valorantRank.NickName = nickName;
            valorantRank.Tag = tag;

            context.SaveChanges();

            return valorantRank;
        }

        private ValorantRank SetDefaultRank(User user)
        {
            user.ValorantRank = new ValorantRank() { NickName = string.Empty, Tag = string.Empty };
            context.SaveChanges();
            return user.ValorantRank;
        }
    }
}
