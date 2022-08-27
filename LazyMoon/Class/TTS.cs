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
using TwitchLib.Communication.Models;
using Google.Cloud.TextToSpeech.V1;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace LazyMoon.Class
{
    public delegate void TTSSpeakEvent(string memoryStream, string chanel);

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

            return;
        }


        public void Speak(string setText, string name, string chanel)
        {
            SynthesisInput input = new SynthesisInput
            {
                Text = setText
            };

            Random rd = new Random();

            Global.EVoice eVoice = (Global.EVoice)rd.Next(1, 5);
            VoiceSelectionParams voice;

            if (chanel != "jikjky")
                voice = new VoiceSelectionParams
                {
                    LanguageCode = "ko-KR",
                    Name = "ko-KR-Standard-" + eVoice.ToString(),
                };
            else
                voice = new VoiceSelectionParams
                {
                    LanguageCode = "ko-KR",
                    Name = "ko-KR-Standard-" + eVoice.ToString(),
                };

            AudioConfig config = new AudioConfig
            {
                AudioEncoding = AudioEncoding.Mp3,
            };

            if (global.ttsSetting.ContainsKey(chanel) == true && global.ttsSetting[chanel].VoiceSettingDictionary.ContainsKey(name) == true)
            {
                if (global.ttsSetting[chanel].VoiceSettingDictionary[name].Use == false || global.ttsSetting[chanel].TTSEnable == false)
                {
                    return;
                }
                config.Pitch = global.ttsSetting[chanel].VoiceSettingDictionary[name].Pitch;
                config.SpeakingRate = global.ttsSetting[chanel].Rate;
                config.VolumeGainDb = global.ttsSetting[chanel].Volume;
                if (chanel != "jikjky")
                    voice.Name = "ko-KR-Standard-" + global.ttsSetting[chanel].VoiceSettingDictionary[name].Voice.ToString();
                else
                    voice.Name = "ko-KR-Standard-" + global.ttsSetting[chanel].VoiceSettingDictionary[name].Voice.ToString();
            }
            else
            {
                if (global.ttsSetting.ContainsKey(chanel) == false)
                {
                    global.ttsSetting[chanel] = new Global.TTSSetting();
                }
                config.Pitch = defaultPitch;
                config.SpeakingRate = global.ttsSetting[chanel].Rate;
                config.VolumeGainDb = global.ttsSetting[chanel].Volume;
            }

            var response = ttsClient.SynthesizeSpeech(new SynthesizeSpeechRequest
            {
                Input = input,
                Voice = voice,
                AudioConfig = config
            });

            NotifyTTSSpeakEvent(response.AudioContent.ToBase64(), chanel);
        }

        private void NotifyTTSSpeakEvent(string audioBase64,string chanel)
        {
            OnSpeak?.Invoke(audioBase64, chanel);
        }
    }
}
