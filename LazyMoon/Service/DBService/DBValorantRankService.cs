using LazyMoon.Model;
using LazyMoon.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TwitchLib.Client.Models;

#nullable enable
namespace LazyMoon.Service
{
    public class DBValorantRankService
    {
        readonly protected AppDbContext _context;

        public DBValorantRankService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ValorantRank?> GetValorantRankOrNullAsync(string name)
        {
            var user = await _context.Users.Include(x => x.ValorantRank).FirstOrDefaultAsync(x => x.Name == name);
            if (user == null)
                return null;
            if (user.ValorantRank == null)
                return SetDefaultRank(user);
            return user.ValorantRank;
        }

        public async Task<ValorantRank?> SetValorantRankOrNullAsync(string name, string nickName, string tag)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Name == name);

            if (user == null)
                return null;

            var valorantRank = user.ValorantRank;

            if (valorantRank == null)
                valorantRank = SetDefaultRank(user);

            valorantRank.NickName = nickName;
            valorantRank.Tag = tag;

            _context.SaveChanges();

            return valorantRank;
        }

        private ValorantRank SetDefaultRank(User user)
        {
            user.ValorantRank = new ValorantRank() { NickName = string.Empty, Tag = string.Empty };
            _context.SaveChanges();
            return user.ValorantRank;
        }
    }
}
