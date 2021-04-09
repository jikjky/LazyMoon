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
                SaveUserInfo();
            }
            LoadUserInfo();
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

        #region TTSConfig
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

        public TTSSetting ttsSetting = new TTSSetting();

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
                ttsSetting = JsonConvert.DeserializeObject<TTSSetting>(jsonString);

                Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "Load TTS Config : " + jsonString);
            }
            catch (Exception e)
            {
                Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Error, "Load TTS Config Error : " + e.Message);
            }
        }

        public void SetTTSEnable(bool enabled)
        {
            Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "Set TTSEnable : " + enabled.ToString());
            ttsSetting.TTSEnable = enabled;
            SaveTTSConfig();
        }

        public void SetTTSRate(double rate)
        {
            Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "Set TTSRate : " + rate.ToString());
            ttsSetting.Rate = rate;
            SaveTTSConfig();
        }

        public void SetTTSVolume(double volume)
        {
            Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "Set TTSVolume : " + volume.ToString());
            ttsSetting.Volume = volume;
            SaveTTSConfig();
        }

        public void SetTTSDefault()
        {
            Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "SetTTSDefault");
            ttsSetting.SetDefault();
            SaveTTSConfig();
        }

        #endregion

        #region User
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

        public Dictionary<string, VoiceSetting> VoiceSettingDictionary = new Dictionary<string, VoiceSetting>();

        /// <summary>
        /// 유저 목소리 정보 저장
        /// </summary>
        public void SaveUserInfo()
        {
            try
            {
                string jsonString;
                jsonString = JsonConvert.SerializeObject(VoiceSettingDictionary);
                File.WriteAllText("userinfo.json", jsonString);

                Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "Save User Config : " + jsonString);
            }
            catch (Exception e)
            {
                Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Error, "Save User Config Error : " + e.Message);
            }
        }

        /// <summary>
        /// 유저 목소리 정보 불러오기
        /// </summary>
        public void LoadUserInfo()
        {
            try
            {
                string jsonString;
                jsonString = File.ReadAllText("userinfo.json");
                VoiceSettingDictionary = JsonConvert.DeserializeObject<Dictionary<string, VoiceSetting>>(jsonString);

                Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "Load User Config : " + jsonString);
            }
            catch (Exception e)
            {
                Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Error, "Load User Config Error : " + e.Message);
            }
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
            Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "SetVoiceGender User : " + name + " Value : " + gender.ToString());
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
            Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "SetVoicePitch User : " + name + " Value : " + pitch.ToString());
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
            Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "SetTTS User : " + name + " Value : " + on.ToString());
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
            Log.FileLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "GetVoiceInfo User : " + name);
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
