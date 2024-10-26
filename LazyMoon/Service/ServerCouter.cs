using System;
using System.Threading.Tasks;
namespace LazyMoon.Service
{
    public sealed class ServerCounterService
    {
        public int CounterValue { get; private set; }
        private DateTime LastResetDate;

        public ServerCounterService()
        {
            CounterValue = 0;
            LastResetDate = GetKoreaTime().Date;
        }

        public void Add()
        {
            DateTime koreaTime = GetKoreaTime();

            // 날짜가 바뀌면 카운터 초기화
            if (koreaTime.Date != LastResetDate)
            {
                CounterValue = 0;
                LastResetDate = koreaTime.Date;
            }

            CounterValue++;
        }

        private DateTime GetKoreaTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time"));
        }
    }
}
