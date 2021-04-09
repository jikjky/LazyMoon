using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Users;
using TwitchLib.Api.V5.Models.Subscriptions;
using TwitchLib.Communication.Models;
using Google.Cloud.TextToSpeech.V1;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace LazyMoon.Class
{
    public delegate void TTSSpeakEvent(string memoryStream);

    public class TTS
    {
        Log4NetManager Log = Log4NetManager.GetInstance();
        Global global = Global.GetInstance();

        //TTS
        TextToSpeechClient ttsClient;

        public event TTSSpeakEvent OnSpeak;

        public double defaultPitch = 0;

        //생성자
        public TTS()
        {
            Log.TTSLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "Create TTS Instance");

            //TTS
            FileInfo file = new FileInfo(@"wwwroot/google.json");
            Init("civil-sprite-288916", file.FullName);
        }

        

        public void Init(string projectId, string jsonPath)
        {
            Log.TTSLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "Set Google");
            TextToSpeechClientBuilder builder = new TextToSpeechClientBuilder
            {
                CredentialsPath = jsonPath
            };

            ttsClient = builder.Build();

            return ;
        }


        public void Speak(string setText, string name)
        {
            int gender = 3;
            if (setText[setText.Length - 1] == '`')
            {
                gender = 1;
            }

            SynthesisInput input = new SynthesisInput
            {
                Text = setText
            };

            VoiceSelectionParams voice = new VoiceSelectionParams
            {
                LanguageCode = "ko-KR",
                SsmlGender = (SsmlVoiceGender)gender
            };
            AudioConfig config = new AudioConfig
            {
                AudioEncoding = AudioEncoding.Mp3,
            };

            if (global.VoiceSettingDictionary.ContainsKey(name) == true)
            {
                if (global.VoiceSettingDictionary[name].Use == false || global.ttsSetting.TTSEnable == false)
                {
                    return;
                }
                config.Pitch = global.VoiceSettingDictionary[name].Pitch;
                config.SpeakingRate = global.ttsSetting.Rate;
                config.VolumeGainDb = global.ttsSetting.Volume;
                voice.SsmlGender = (SsmlVoiceGender)global.VoiceSettingDictionary[name].Gender;
            }
            else
            {
                config.Pitch = defaultPitch;
                config.SpeakingRate = global.ttsSetting.Rate;
                config.VolumeGainDb = global.ttsSetting.Volume;
            }

            var response = ttsClient.SynthesizeSpeech(new SynthesizeSpeechRequest
            {
                Input = input,
                Voice = voice,
                AudioConfig = config
            });

            NotifyTTSSpeakEvent(response.AudioContent.ToBase64());
        }

        private void NotifyTTSSpeakEvent(string audioBase64)
        {
            OnSpeak?.Invoke(audioBase64);
        }
    }
}
