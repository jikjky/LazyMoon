using LazyMoon.Model;
using LazyMoon.Model.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

#nullable enable
namespace LazyMoon.Class.Service.DBService
{
    public class DBTTSService(IDbContextFactory<AppDbContext> contextFactory)
    {
        private double rate_ = 1;
        private double volume_ = 0;

        public bool tTSEnable = true;
        public double Volume
        {
            get
            {
                return volume_;
            }
            set
            {
                if (value > 16)
                    volume_ = 16;
                else if (value < -96)
                    volume_ = -96;
                else
                    volume_ = value;
            }
        }
        public double Rate
        {
            get
            {
                return rate_;
            }
            set
            {
                if (value > 4)
                    rate_ = 4;
                else if (value < 0.25)
                    rate_ = 0.25;
                else
                    rate_ = value;
            }
        }

        readonly protected IDbContextFactory<AppDbContext> contextFactory = contextFactory;

        public async Task<TTS?> SetTTSOrNullAsync(string chanel, double rate, double volume)
        {
            var context = await contextFactory.CreateDbContextAsync();
            var user = await context.Users.Include(x => x.TTS).ThenInclude(x => x!.Voices).FirstOrDefaultAsync(x => x.Name == chanel);
            if (user == null)
                return null;
            user.TTS = new TTS() { Rate = rate, Volume = volume, TTSEnable = true };
            context.SaveChanges();
            return user.TTS;
        }

        public async Task<TTS?> GetTTSByChanelOrNullAsync(string chanel)
        {
            var context = await contextFactory.CreateDbContextAsync();
            var user = await context.Users.Include(x => x.TTS).ThenInclude(x => x!.Voices).FirstOrDefaultAsync(x => x.Name == chanel);
            if (user == null)
                return null;
            if (user.TTS == null)
            {
                user.TTS = SetTTSDefault();
                await context.SaveChangesAsync();
            }
            return user.TTS;
        }
        public TTS SetTTSDefault()
        {
            var tts = new TTS() { Volume = 0, TTSEnable = true, Rate = 1 };
            return tts;
        }

        public async Task<TTS?> SetTTSEnableOrNullAsync(string chanel, bool enable)
        {
            var context = await contextFactory.CreateDbContextAsync();
            var user = await context.Users.Include(x => x.TTS).ThenInclude(x => x!.Voices).FirstOrDefaultAsync(x => x.Name == chanel);
            if (user == null)
                return null;
            user.TTS ??= SetTTSDefault();
            user.TTS.TTSEnable = enable;
            await context.SaveChangesAsync();
            return user.TTS;
        }
        public async Task<TTS?> SetTTSRateOrNullAsync(string chanel, double rate)
        {
            var context = await contextFactory.CreateDbContextAsync();
            Rate = rate;
            var user = await context.Users.Include(x => x.TTS).ThenInclude(x => x!.Voices).FirstOrDefaultAsync(x => x.Name == chanel);
            if (user == null)
                return null;
            user.TTS ??= SetTTSDefault();
            user.TTS.Rate = Rate;
            await context.SaveChangesAsync();
            return user.TTS;
        }

        public async Task<TTS?> SetTTSVolumeOrNullAsync(string chanel, double volume)
        {
            var context = await contextFactory.CreateDbContextAsync();
            Volume = volume;
            var user = await context.Users.Include(x => x.TTS).ThenInclude(x => x!.Voices).FirstOrDefaultAsync(x => x.Name == chanel);
            if (user == null)
                return null;
            user.TTS ??= SetTTSDefault();
            user.TTS.Volume = Volume;
            await context.SaveChangesAsync();
            return user.TTS;
        }

        public async Task<TTS?> SetTTSDefaultOrNullAsync(string chanel)
        {
            var context = await contextFactory.CreateDbContextAsync();
            var user = await context.Users.Include(x => x.TTS).ThenInclude(x => x!.Voices).FirstOrDefaultAsync(x => x.Name == chanel);
            if (user == null)
                return null;
            user.TTS ??= SetTTSDefault();
            user.TTS.Rate = 1;
            user.TTS.Volume = 0;
            await context.SaveChangesAsync();
            return user.TTS;
        }
    }
}
