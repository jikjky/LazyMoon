using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogManager;

namespace LazyMoon
{
    public class Log4NetManager
    {
        FileInfo mSettingFilePath;

        private int mDeleteDayPeriod = 90;

        private List<Log4NetBase> mManagedeList = new List<Log4NetBase>();

        private static Log4NetManager _instance = null;

        public int isDeleteDayPeriod
        {
            get
            {
                return mDeleteDayPeriod;
            }
            set
            {
                mDeleteDayPeriod = value > 0 ? value : 0;
                DeleteDayPeriodSetting();
            }
        }

        public static string SetInstance
        {
            set
            {
                if (null == _instance)
                {
                    _instance = new Log4NetManager(new FileInfo(value));
                }
            }
        }

        public static Log4NetManager GetInstance()
        {
            if (null == _instance)
            {
                return null;
            }
            return _instance;
        }

        public enum eUseLog
        {
            TTSLog,
            TwitchBotLog,
            ValorantRankLog,
        }

        public Log4NetBase TTSLog;
        public Log4NetBase TwitchBotLog;
        public Log4NetBase ValorantRankLog;

        private Log4NetManager(FileInfo path)
        {
            mSettingFilePath = path;

            TTSLog = new Log4NetBase("TTSLog", mSettingFilePath);
            mManagedeList.Add(TTSLog);

            TwitchBotLog = new Log4NetBase("TwitchBotLog", mSettingFilePath);
            mManagedeList.Add(TwitchBotLog);

            ValorantRankLog = new Log4NetBase("ValorantRankLog", mSettingFilePath);
            mManagedeList.Add(ValorantRankLog);

            DeleteDayPeriodSetting();
        }

        private void DeleteDayPeriodSetting()
        {
            foreach (var item in mManagedeList)
            {
                item.isDeleteDayPeriod = mDeleteDayPeriod;
            }
        }

        public void DeleteLog()
        {
            foreach (var item in mManagedeList)
            {
                item.DeleteLog();
            }
        }

        public void SetLog(eUseLog logName, Log4NetBase.eLogType logType, string logText)
        {
            var useLog = mManagedeList.Find(p=>p.LogName == logName.ToString());
            useLog.SetLog(logType, logText);
        }
    }
}
