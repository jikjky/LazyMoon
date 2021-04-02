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
        public class TTSSetting
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

            public void SetDefault()
            {
                Volume = 0;
                Rate = 1;
            }
        }
        //TTS
        TextToSpeechClient ttsClient;

        public event TTSSpeakEvent OnSpeak;

        public class VoiceSetting
        {
            public EGender Gender = EGender.Female;
            public double Pitch = 0;
            public bool Use = true;
        }

        Dictionary<string, VoiceSetting> VoiceSettingDictionary = new Dictionary<string, VoiceSetting>();
        TTSSetting ttsSetting = new TTSSetting();

        public double defaultPitch = 0;


        public enum EGender
        {
            Male = 1,
            Female = 2,
        }

        //생성자
        public TTS()
        {
            //TTS
            FileInfo file = new FileInfo(@"wwwroot/google.json");
            Init("civil-sprite-288916", file.FullName);

            FileInfo userinfo = new FileInfo("userinfo.json");
            if (userinfo.Exists == false)
            {
                userinfo.Create().Close();
                SaveUserInfo();
            }
            LoadUserInfo();
            FileInfo config = new FileInfo("config.json");
            if (config.Exists == false)
            {
                config.Create().Close();
                SaveConfig();
            }
            LoadConfig();
        }

        public void SaveConfig()
        {
            string jsonString;
            jsonString = JsonConvert.SerializeObject(ttsSetting);
            File.WriteAllText("config.json", jsonString);
        }

        public void LoadConfig()
        {
            string jsonString;
            jsonString = File.ReadAllText("config.json");
            ttsSetting = JsonConvert.DeserializeObject<TTSSetting>(jsonString);
        }

        public void SetTTSEnable(bool enabled)
        {
            ttsSetting.TTSEnable = enabled;
            SaveConfig();
        }

        public void SetTTSRate(double rate)
        {
            ttsSetting.Rate = rate;
            SaveConfig();
        }

        public void SetTTSVolume(double volume)
        {
            ttsSetting.Volume = volume;
            SaveConfig();
        }

        public void SetTTSDefault()
        {
            ttsSetting.SetDefault();
            SaveConfig();
        }

        /// <summary>
        /// 유저 목소리 정보 저장
        /// </summary>
        public void SaveUserInfo()
        {
            string jsonString;
            jsonString = JsonConvert.SerializeObject(VoiceSettingDictionary);
            File.WriteAllText("userinfo.json", jsonString);
        }

        /// <summary>
        /// 유저 목소리 정보 불러오기
        /// </summary>
        public void LoadUserInfo()
        {
            string jsonString;
            jsonString = File.ReadAllText("userinfo.json");
            VoiceSettingDictionary = JsonConvert.DeserializeObject<Dictionary<string, VoiceSetting>>(jsonString);
        }

        /// <summary>
        /// 유저 정보 변경 (성별)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="gender"></param>
        public void SetVoiceGender(string name, EGender gender)
        {
            if (VoiceSettingDictionary.ContainsKey(name) == true)
            {
                VoiceSettingDictionary[name].Gender = gender;
            }
            else
            {
                VoiceSettingDictionary.Add(name, new VoiceSetting() { Gender = gender });
            }
            SaveUserInfo();
        }

        /// <summary>
        /// 유저 정보 변경 (피치)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pitch"></param>
        public void SetVoicePitch(string name, double pitch)
        {
            if (VoiceSettingDictionary.ContainsKey(name) == true)
            {
                VoiceSettingDictionary[name].Pitch = pitch;
            }
            else
            {
                VoiceSettingDictionary.Add(name, new VoiceSetting() { Pitch = pitch });
            }
            SaveUserInfo();
        }

        /// <summary>
        /// 유저 TTS 사용 여부
        /// </summary>
        /// <param name="name"></param>
        /// <param name="on"></param>
        public void SetTTS(string name, bool on)
        {
            if (VoiceSettingDictionary.ContainsKey(name) == true)
            {
                VoiceSettingDictionary[name].Use = on;
            }
            else
            {
                VoiceSettingDictionary.Add(name, new VoiceSetting() { Use = on });
            }
            SaveUserInfo();
        }

        /// <summary>
        /// 유저의 TTS 정보를 가져온다
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public VoiceSetting GetVoiceInfo(string name)
        {
            VoiceSetting returnValue;
            if (VoiceSettingDictionary.ContainsKey(name) == true)
            {
                returnValue = VoiceSettingDictionary[name];
            }
            else
            {
                returnValue = new VoiceSetting();
            }
            return returnValue;
        }

        public void Init(string projectId, string jsonPath)
        {
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

            if (VoiceSettingDictionary.ContainsKey(name) == true)
            {
                if (VoiceSettingDictionary[name].Use == false || ttsSetting.TTSEnable == false)
                {
                    return;
                }
                config.Pitch = VoiceSettingDictionary[name].Pitch;
                config.SpeakingRate = ttsSetting.Rate;
                config.VolumeGainDb = ttsSetting.Volume;
                voice.SsmlGender = (SsmlVoiceGender)VoiceSettingDictionary[name].Gender;
            }
            else
            {
                config.Pitch = defaultPitch;
                config.SpeakingRate = ttsSetting.Rate;
                config.VolumeGainDb = ttsSetting.Volume;
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
