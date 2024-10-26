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

            // ��¥�� �ٲ�� ī���� �ʱ�ȭ
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
