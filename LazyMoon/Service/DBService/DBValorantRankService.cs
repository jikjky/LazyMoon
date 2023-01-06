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

        public async Task<ValorantRank> GetValorantRankOrNullAsync(string name)
        {
            var user = await _context.Users.Include(x=>x.ValorantRank).FirstOrDefaultAsync(x => x.Name == name);
            if (user == null)
                return null;
            if (user.ValorantRank == null)
                return SetDefaultRank(user);
            return user.ValorantRank;
        }

        public async Task<ValorantRank> SetValorantRankOrNullAsync(string name, int rank, int score)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Name == name);
            if (user == null)
                return null;
            if (user.ValorantRank == null)
                SetDefaultRank(user);
            user.ValorantRank.currentRank = rank;
            user.ValorantRank.currentScore = score;
            _context.SaveChanges();
            return user.ValorantRank;
        }

        private ValorantRank? SetDefaultRank(User user)
        {
            user.ValorantRank = new ValorantRank() { currentRank = 0, currentScore = 0 };
            _context.SaveChanges();
            return user.ValorantRank;
        }
    }
}
