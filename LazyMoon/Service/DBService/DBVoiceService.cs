using LazyMoon.Model;
using LazyMoon.Model.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

#nullable enable
namespace LazyMoon.Service
{
    public class DBVoiceService
    {
        readonly protected IDbContextFactory<AppDbContext> _contextFactory;

        public DBVoiceService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Voice?> GetVoiceOrNullAsync(string chanel, string name)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            var user = await context.Users.Include(x => x.TTS).ThenInclude(x => x.Voices).FirstOrDefaultAsync(x => x.Name == chanel);
            if (user == null || user.TTS == null)
                return null;
            var voice = user.TTS.Voices.FirstOrDefault(x => x.Name == name);
            if (voice == null)
                return null;
            return voice;
        }

        public async Task<Voice?> GetVoiceOrNullAsync(TTS tts, string name)
        {
            if (tts == null)
                return null;
            var context = await _contextFactory.CreateDbContextAsync();

            Voice? voice = null;
            if (tts.Voices != null)
            {
                voice = tts.Voices.FirstOrDefault(x => x.Name == name);
            }
            if (voice == null)
            {
                voice = SetVoiceDefault(voice, name);
            }
            var existingTTS = context.TTS.FirstOrDefault(x => x.Id == tts.Id);
            if (existingTTS != null)
            {
                if (existingTTS.Voices == null)
                {
                    existingTTS.Voices = new List<Voice>();
                }

                existingTTS.Voices.Add(voice);
                await context.SaveChangesAsync();
            }
            return voice;
        }

        public Voice SetVoiceDefault(Voice? voice, string name)
        {
            Random rd = new Random();

            EVoice eVoice = (EVoice)rd.Next(1, 5);
            voice = new Voice() { Name = name, Pitch = 1, Use = true, VoiceMode = eVoice };
            return voice;
        }

        public async Task<Voice?> SetVoiceModeOrNullAsync(string chanel, string name, EVoice eVoice)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            var user = await context.Users.Include(x => x.TTS).ThenInclude(x => x.Voices).FirstOrDefaultAsync(x => x.Name == chanel);
            if (user == null || user.TTS == null)
                return null;
            var voice = user.TTS.Voices.FirstOrDefault(x => x.Name == name);
            if (voice == null)
                return null;
            voice.VoiceMode = eVoice;
            await context.SaveChangesAsync();
            return voice;
        }

        public async Task<Voice?> SetVoicePitchOrNullAsync(string chanel, string name, double pitch)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            var user = await context.Users.Include(x => x.TTS).ThenInclude(x => x.Voices).FirstOrDefaultAsync(x => x.Name == chanel);
            if (user == null || user.TTS == null)
                return null;
            var voice = user.TTS.Voices.FirstOrDefault(x => x.Name == name);
            if (voice == null)
                return null;
            voice.Pitch = pitch;
            await context.SaveChangesAsync();
            return voice;
        }

        public async Task<Voice?> SetVoiceEnableOrNullAsync(string chanel, string name, bool enable)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            var user = await context.Users.Include(x => x.TTS).ThenInclude(x => x.Voices).FirstOrDefaultAsync(x => x.Name == chanel);
            if (user == null || user.TTS == null)
                return null;
            var voice = user.TTS.Voices.FirstOrDefault(x => x.Name == name);
            if (voice == null)
                return null;
            voice.Use = enable;
            await context.SaveChangesAsync();
            return voice;
        }
    }
}
