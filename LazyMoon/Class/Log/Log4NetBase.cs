using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogManager
{
    using log4net;
    using log4net.Appender;
    using log4net.Core;
    using log4net.Layout;
    using log4net.Repository;
    using log4net.Repository.Hierarchy;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    /// <summary>
    /// Log 관리 Class
    /// Nuget Version : Log4Net 2.0.8
    /// </summary>
    public class Log4NetBase
    {
        private ILog ILog;

        //LogSetting에 저장된 BaseFolder 경로
        private string mBaseFolder;

        //오늘
        private int mDate;

        private double mDeleteDayPeriod = 90;

        public string LogName { get; set; }

        /// <summary>
        /// 오늘기준 삭제 일 지정
        /// </summary>
        public int isDeleteDayPeriod
        {
            get
            {
                return Convert.ToInt32(mDeleteDayPeriod);
            }
            set
            {
                mDeleteDayPeriod = value;
            }
        }

        /// <summary>
        /// LogType 이넘
        /// </summary>
        public enum eLogType
        {
            Info = 0,   //Information
            Warn,       //Warning
            Error,        //Error
            Fatal,        //Fatal Error
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="logName">저장할 파일 이름(경로수정은 ConfigFile내에서 해야됩니다.)</param>
        /// <param name="log4netConfigPath">ConfigFile 위치</param>
        public Log4NetBase(string logName, FileInfo log4netConfigPath)
        {
            LogName = logName;
            mDate = DateTime.Today.DayOfYear;

            var hierarchy = (Hierarchy)LogManager.GetRepository();
            var logger = hierarchy.LoggerFactory.CreateLogger((ILoggerRepository)hierarchy, logName);
            logger.Hierarchy = hierarchy;
            logger.AddAppender(CreateFileAppender(logName, log4netConfigPath));
            logger.Repository.Configured = true;


            // alternative use the LevelMap to set the Log level based on a string
            // hierarchy.LevelMap["ERROR"]
            hierarchy.Threshold = Level.Debug;
            logger.Level = Level.Debug;

            ILog = new LogImpl(logger);
        }

        private IAppender CreateFileAppender(string logName, FileInfo log4netConfigPath)
        {
            mBaseFolder = log4netConfigPath.FullName + "//";

            var fileAppender = new RollingFileAppender
            {
                Name = logName,
                //로그 저장 베이스 폴더
                File = mBaseFolder,
                //로그 저장 날짜/시간 패턴 설정
                DatePattern = $"yyyy/MM/dd/'{logName}'_yyyy-MM-dd'.log'",
                //파일이 이미 존재 할 때 추가 할지 덮어 쓸지 선택하는 옵션
                AppendToFile = true,
                //Once: 프로그램 실행 시 한번 파일을 롤링한다.
                //Size: 설정한 크기 보다 커지면 파일을 롤링한다.
                //Date: datePattern에 설정을 기준으로 파일을 롤링한다.
                //Composite: Size와 Date를 둘다 확인 하여 파일을 롤링한다.
                RollingStyle = RollingFileAppender.RollingMode.Composite,
                //백업 파일의 개수 (-1 = 무한, Other = 파일 개수)
                MaxSizeRollBackups = 10,
                //파일 크기: 로그 파일 크기를 설정한다. (단위로 KB, MB, GB를 쓸 수 있다.)
                MaximumFileSize = "20MB",
                //false: datePattern에 맞춰서 백업 할 때 이름이 바뀐다.[필수! true로 설정시 로그가 남지 않음.] log.txt.1, log.txt.2, log.txt.3, etc...
                StaticLogFileName = false,
                //로그 파일에 출력 할 포맷 설정
                Layout = new log4net.Layout.PatternLayout("%d [%t] %-5p - %m%n")
            };
            fileAppender.ActivateOptions();
            return fileAppender;
        }

        /// <summary>
        /// SetLog
        /// </summary>
        /// <param name="logType">LogType</param>
        /// <param name="writeLogString">Log Text</param>
        public void SetLog(eLogType logType, string writeLogString, [CallerMemberName] string memberName = "",
        [CallerFilePath] string fileName = "",
        [CallerLineNumber] int lineNumber = 0, bool bCheck = false)
        {
            fileName = Path.GetFileName(fileName);
            if (bCheck == false)
            {
                writeLogString = string.Format("[Line : {0}] [Member : {1}] [FileName : {2}]{3}", lineNumber, memberName, fileName, writeLogString);
            }
            switch (logType)
            {
                case eLogType.Info:
                    ILog.Info(writeLogString);
                    break;

                case eLogType.Warn:
                    ILog.Warn(writeLogString);
                    break;

                case eLogType.Error:
                    ILog.Error(writeLogString);
                    break;

                case eLogType.Fatal:
                    ILog.Fatal(writeLogString);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// DeleteLog
        /// </summary>
        /// 예제>
        /// 금일이 1월7일이고  mDeleteDayPeriod = 1 이라고 한다면
        /// 1월 5일(포함)이전 Log가 삭제된다.
        public void DeleteLog()
        {
            //하루에 한번씩만 동작되게
            if (mDate != DateTime.Today.DayOfYear)
            {
                mDate = DateTime.Today.DayOfYear;
                new Thread(() =>
                {
                    var compareTime = DateTime.Now.AddDays( - mDeleteDayPeriod ).Date;
                    var dirTime = default(DateTime);

                    var MainDirInfo = new System.IO.DirectoryInfo(mBaseFolder);
                    if (MainDirInfo.Exists)
                    {
                        foreach (var yearDirInfo in MainDirInfo.GetDirectories())
                        {
                            foreach (var monDirInfo in yearDirInfo.GetDirectories())
                            {
                                foreach (var dayDirInfo in monDirInfo.GetDirectories())
                                {
                                    if (DateTime.TryParse($"{yearDirInfo.Name}/{monDirInfo.Name}/{dayDirInfo.Name}", out dirTime))
                                    {
                                        try
                                        {
                                            if (dirTime.Date < compareTime.Date)
                                            {
                                                Directory.Delete(dayDirInfo.FullName, true);
                                            }
                                        }
                                        //열려있는 파일은 지울수 없다.
                                        catch (IOException e)
                                        {
                                            Debug.WriteLine(e.Message);
                                        }
                                    }
                                }
                            }
                        }
                    }
                })
                { IsBackground = true }.Start();
            }
        }
    }
}