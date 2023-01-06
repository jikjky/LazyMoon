using Google.Type;
using LazyMoon.Model;
using LazyMoon.Model.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

#nullable enable
namespace LazyMoon.Service
{
    public class DBTTSService
    {
        private double Rate_ = 1;
        private double Volume_ = 0;

        public bool TTSEnable = true;
        public double Volume
        {
            get
            {
                return Volume_;
            }
            set
            {
                if (value > 16)
                    Volume_ = 16;
                else if (value < -96)
                    Volume_ = -96;
                else
                    Volume_ = value;
            }
        }
        public double Rate
        {
            get
            {
                return Rate_;
            }
            set
            {
                if (value > 4)
                    Rate_ = 4;
                else if (value < 0.25)
                    Rate_ = 0.25;
                else
                    Rate_ = value;
            }
        }

        readonly protected IDbContextFactory<AppDbContext> _contextFactory;

        public DBTTSService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<TTS?> SetTTSOrNullAsync(string chanel, double rate, double volume)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            var user = await context.Users.Include(x => x.TTS).ThenInclude(x => x.Voices).FirstOrDefaultAsync(x => x.Name == chanel);
            if (user == null)
                return null;
            user.TTS = new TTS() { Rate = rate, Volume = volume, TTSEnable = true };
            context.SaveChanges();
            return user.TTS;
        }

        public async Task<TTS?> GetTTSByChanelOrNullAsync(string chanel)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            var user = await context.Users.Include(x => x.TTS).ThenInclude(x => x.Voices).FirstOrDefaultAsync(x => x.Name == chanel);
            if (user == null)
                return null;
            if (user.TTS == null)
            {
                user.TTS = SetTTSDefault(user.TTS);
                await context.SaveChangesAsync();
            }
            return user.TTS;
        }
        public TTS SetTTSDefault(TTS? tts)
        {
            tts = new TTS() { Volume = 0, TTSEnable = true, Rate = 1 };
            return tts;
        }

        public async Task<TTS?> SetTTSEnableOrNullAsync(string chanel, bool enable)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            var user = await context.Users.Include(x => x.TTS).ThenInclude(x => x.Voices).FirstOrDefaultAsync(x => x.Name == chanel);
            if (user == null)
                return null;
            if (user.TTS == null)
            {
                user.TTS = SetTTSDefault(user.TTS);
            }
            user.TTS.TTSEnable = enable;
            await context.SaveChangesAsync();
            return user.TTS;
        }
        public async Task<TTS?> SetTTSRateOrNullAsync(string chanel, double rate)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            Rate = rate;
            var user = await context.Users.Include(x => x.TTS).ThenInclude(x => x.Voices).FirstOrDefaultAsync(x => x.Name == chanel);
            if (user == null)
                return null;
            if (user.TTS == null)
            {
                user.TTS = SetTTSDefault(user.TTS);
            }
            user.TTS.Rate = Rate;
            await context.SaveChangesAsync();
            return user.TTS;
        }

        public async Task<TTS?> SetTTSVolumeOrNullAsync(string chanel, double volume)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            Volume = volume;
            var user = await context.Users.Include(x => x.TTS).ThenInclude(x => x.Voices).FirstOrDefaultAsync(x => x.Name == chanel);
            if (user == null)
                return null;
            if (user.TTS == null)
            {
                user.TTS = SetTTSDefault(user.TTS);
            }
            user.TTS.Volume = Volume;
            await context.SaveChangesAsync();
            return user.TTS;
        }

        public async Task<TTS?> SetTTSDefaultOrNullAsync(string chanel)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            var user = await context.Users.Include(x => x.TTS).ThenInclude(x => x.Voices).FirstOrDefaultAsync(x => x.Name == chanel);
            if (user == null)
                return null;
            if (user.TTS == null)
            {
                user.TTS = SetTTSDefault(user.TTS);
            }
            user.TTS.Rate = 1;
            user.TTS.Volume = 0;
            await context.SaveChangesAsync();
            return user.TTS;
        }
    }
}
