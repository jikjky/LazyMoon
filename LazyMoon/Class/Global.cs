using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LazyMoon
{
    public class Global
    {
        private static Global _instance = null;

        Log4NetManager Log = Log4NetManager.GetInstance();

        public static Global GetInstance()
        {
            if (null == _instance)
            {
                _instance = new Global();
            }
            return _instance;
        }


        public Global()
        {
            FileInfo userInfo = new FileInfo("userinfo.json");
            if (userInfo.Exists == false)
            {
                userInfo.Create().Close();
                SaveUserInfoConfig();
            }
            LoadUserInfoConfig();
            FileInfo ttsConfig = new FileInfo("ttsconfig.json");
            if (ttsConfig.Exists == false)
            {
                ttsConfig.Create().Close();
                SaveTTSConfig();
            }
            LoadTTSConfig();
            FileInfo valorantRank = new FileInfo("valorantrank.json");
            if (valorantRank.Exists == false)
            {
                valorantRank.Create().Close();
                SaveRankInfo();
            }
            LoadRankInfo();
        }

        public class TwitchUser
        {
            public string Id;
            public string Name;
        }

        public Dictionary<string, TwitchUser> UserInfo = new Dictionary<string, TwitchUser>(); 

        public void SaveUserInfoConfig()
        {
            try
            {
                string jsonString;
                jsonString = JsonConvert.SerializeObject(UserInfo);
                File.WriteAllText("userinfo.json", jsonString);

                Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "Save UserInfo Config : " + jsonString);
            }
            catch (Exception e)
            {
                Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Error, "Save UserInfo Config Error : " + e.Message);
            }
        }

        public void LoadUserInfoConfig()
        {
            try
            {
                string jsonString;
                jsonString = File.ReadAllText("userinfo.json");
                UserInfo = JsonConvert.DeserializeObject<Dictionary<string, TwitchUser>>(jsonString);

                Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "Load UserInfo Config : " + jsonString);
            }
            catch (Exception e)
            {
                Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Error, "Load UserInfo Config Error : " + e.Message);
            }
        }

        public TwitchUser SetUserInfo(string name, string id)
        {
            if (UserInfo.ContainsKey(name) == false)
            {
                UserInfo[name] = new Global.TwitchUser() { Id = id, Name = name };
                SaveUserInfoConfig();
            }
            return UserInfo[name];
        }

        #region TTSConfig
        public class VoiceSetting
        {
            public EGender Gender = EGender.Female;
            public double Pitch = 0;
            public bool Use = true;
        }

        public enum EGender
        {
            Male = 1,
            Female = 2,
        }

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

            public Dictionary<string, VoiceSetting> VoiceSettingDictionary = new Dictionary<string, VoiceSetting>();

            public void SetDefault()
            {
                Volume = 0;
                Rate = 1;
            }
        }

        public Dictionary<string,TTSSetting> ttsSetting = new Dictionary<string, TTSSetting>();

        public void SaveTTSConfig()
        {
            try
            {
                string jsonString;
                jsonString = JsonConvert.SerializeObject(ttsSetting);
                File.WriteAllText("ttsconfig.json", jsonString);

                Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "Save TTS Config : " + jsonString);
            }
            catch (Exception e)
            {
                Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Error, "Save TTS Config Error : " + e.Message);
            }
        }

        public void LoadTTSConfig()
        {
            try
            {
                string jsonString;
                jsonString = File.ReadAllText("ttsconfig.json");
                ttsSetting = JsonConvert.DeserializeObject<Dictionary<string, TTSSetting>>(jsonString);

                Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "Load TTS Config : " + jsonString);
            }
            catch (Exception e)
            {
                Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Error, "Load TTS Config Error : " + e.Message);
            }
        }

        public void SetTTSEnable(string chanel, bool enabled)
        {
            Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "Set TTSEnable Chanel : " + chanel + " Value : " + enabled.ToString());
            if (ttsSetting.ContainsKey(chanel) == false)
            {
                ttsSetting[chanel] = new TTSSetting();
            }
            ttsSetting[chanel].TTSEnable = enabled;
            SaveTTSConfig();
        }

        public void SetTTSRate(string chanel, double rate)
        {
            Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "Set TTSRate Chanel : " + chanel + " Value : " + rate.ToString());
            if (ttsSetting.ContainsKey(chanel) == false)
            {
                ttsSetting[chanel] = new TTSSetting();
            }
            ttsSetting[chanel].Rate = rate;
            SaveTTSConfig();
        }

        public void SetTTSVolume(string chanel, double volume)
        {
            Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "Set TTSVolume Chanel : " + chanel + " Value : " + volume.ToString());
            if (ttsSetting.ContainsKey(chanel) == false)
            {
                ttsSetting[chanel] = new TTSSetting();
            }
            ttsSetting[chanel].Volume = volume;
            SaveTTSConfig();
        }

        public void SetTTSDefault(string chanel)
        {
            Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "SetTTSDefault Chanel : " + chanel);
            if (ttsSetting.ContainsKey(chanel) == false)
            {
                ttsSetting[chanel] = new TTSSetting();
            }
            ttsSetting[chanel].SetDefault();
            SaveTTSConfig();
        }

        /// <summary>
        /// 유저 정보 변경 (성별)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="gender"></param>
        public void SetVoiceGender(string chanel, string name, EGender gender)
        {
            if (ttsSetting.ContainsKey(chanel) == false)
            {
                ttsSetting[chanel] = new TTSSetting();
            }
            if (ttsSetting[chanel].VoiceSettingDictionary.ContainsKey(name) == false)
            {
                ttsSetting[chanel].VoiceSettingDictionary.Add(name, new VoiceSetting());
            }
            ttsSetting[chanel].VoiceSettingDictionary[name].Gender = gender;
            Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "SetVoiceGender Chanel : " + chanel + "  User : " + name + " Value : " + gender.ToString());
            SaveTTSConfig();
        }

        /// <summary>
        /// 유저 정보 변경 (피치)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pitch"></param>
        public void SetVoicePitch(string chanel, string name, double pitch)
        {
            if (ttsSetting.ContainsKey(chanel) == false)
            {
                ttsSetting[chanel] = new TTSSetting();
            }
            if (ttsSetting[chanel].VoiceSettingDictionary.ContainsKey(name) == false)
            {
                ttsSetting[chanel].VoiceSettingDictionary.Add(name, new VoiceSetting());
            }
            ttsSetting[chanel].VoiceSettingDictionary[name].Pitch = pitch;
            Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "SetVoicePitch Chanel : " + chanel + "  User : " + name + " Value : " + pitch.ToString());
            SaveTTSConfig();
        }

        /// <summary>
        /// 유저 TTS 사용 여부
        /// </summary>
        /// <param name="name"></param>
        /// <param name="on"></param>
        public void SetTTS(string chanel, string name, bool on)
        {
            if (ttsSetting.ContainsKey(chanel) == false)
            {
                ttsSetting[chanel] = new TTSSetting();
            }
            if (ttsSetting[chanel].VoiceSettingDictionary.ContainsKey(name) == false)
            {
                ttsSetting[chanel].VoiceSettingDictionary.Add(name, new VoiceSetting());
            }
            ttsSetting[chanel].VoiceSettingDictionary[name].Use = on;
            Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "SetTTS Chanel : " + chanel + " User : " + name + " Value : " + on.ToString());
            SaveTTSConfig();
        }

        /// <summary>
        /// 유저의 TTS 정보를 가져온다
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public VoiceSetting GetVoiceInfo(string chanel, string name)
        {
            VoiceSetting returnValue;
            if (ttsSetting.ContainsKey(chanel) == false)
            {
                ttsSetting[chanel] = new TTSSetting();
            }
            if (ttsSetting[chanel].VoiceSettingDictionary.ContainsKey(name) == false)
            {
                returnValue = new VoiceSetting();
            }
            returnValue = ttsSetting[chanel].VoiceSettingDictionary[name];
            Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "GetVoiceInfo Chanel : " + chanel + "  User : " + name);
            return returnValue;
        }

        #endregion

        #region ValoranRank
        public class ValorantRankSetting
        {
            public int currentRank;
            public int currentScore;
        }

        public Dictionary<string, ValorantRankSetting> ValorantRankDictionary = new Dictionary<string, ValorantRankSetting>();

        public void SaveRankInfo()
        {
            try
            {
                string jsonString;
                jsonString = JsonConvert.SerializeObject(ValorantRankDictionary);
                File.WriteAllText("valorantrank.json", jsonString);

                Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "Save Rank Config : " + jsonString);
            }
            catch (Exception e)
            {
                Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Error, "Save Rank Config Error : " + e.Message);
            }
        }

        public void LoadRankInfo()
        {
            try
            {
                string jsonString;
                jsonString = File.ReadAllText("valorantrank.json");
                ValorantRankDictionary = JsonConvert.DeserializeObject<Dictionary<string, ValorantRankSetting>>(jsonString);

                Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "Load Rank Config : " + jsonString);
            }
            catch (Exception e)
            {
                Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Error, "Load Rank Config Error : " + e.Message);
            }
        }

        public void SetScore(string name, int score)
        {
            if (ValorantRankDictionary.ContainsKey(name) == true)
            {
                ValorantRankDictionary[name].currentScore = score;
            }
            else
            {
                ValorantRankDictionary.Add(name, new ValorantRankSetting() { currentScore = score });
            }
            Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "SetScore User : " + name + " Value : " + score.ToString());
            SaveRankInfo();
        }

        public void SetRank(string name, int rank)
        {
            if (ValorantRankDictionary.ContainsKey(name) == true)
            {
                ValorantRankDictionary[name].currentRank = rank;
            }
            else
            {
                ValorantRankDictionary.Add(name, new ValorantRankSetting() { currentRank = rank });
            }
            Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "SetRank User : " + name + " Value : " + rank.ToString());
            SaveRankInfo();
        }

        public ValorantRankSetting GetRankInfo(string name)
        {
            ValorantRankSetting returnValue;
            if (ValorantRankDictionary.ContainsKey(name) == true)
            {
                returnValue = ValorantRankDictionary[name];
            }
            else
            {
                returnValue = new ValorantRankSetting();
            }
            Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "GetRankInfo User : " + name);
            return returnValue;
        }
        #endregion
    }
}
