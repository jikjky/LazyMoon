using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace LazyMoon.Service
{
    public sealed class ServerCounterService
    {
        List<string> guids = new List<string>();
        public int CounterValue { get { return guids.Count; } }
        private DateTime LastResetDate;

        public ServerCounterService()
        {
            LastResetDate = GetKoreaTime().Date;
        }

        public void Add(string guid)
        {
            if (!guids.Contains(guid))
            {
                guids.Add(guid);
            }
            DateTime koreaTime = GetKoreaTime();

            // ��¥�� �ٲ�� ī���� �ʱ�ȭ
            if (koreaTime.Date != LastResetDate)
            {
                guids.Clear();
                LastResetDate = koreaTime.Date;
            }

        }

        private DateTime GetKoreaTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time"));
        }
    }
}
