using LazyMoon.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace LazyMoon.Service
{
    public sealed class ServerCounterService
    {
        HashSet<string> guids = new HashSet<string>();
        public int CounterValue { get; set; }
        private DateTime LastResetDate;

        DBConnectionHistory dBConnectionHistory;

        public ServerCounterService(DBConnectionHistory _dBConnectionHistory)
        {
            dBConnectionHistory = _dBConnectionHistory;
            GetCounterValue();
            LastResetDate = GetKoreaTime().Date;
        }

        public async Task GetCounterValue()
        {
            CounterValue = await dBConnectionHistory.GetCount(LastResetDate.ToString("yyyyMMdd"));
        }

        public async void Add(string guid)
        {
            DateTime koreaTime = GetKoreaTime();

            // 날짜가 바뀌면 카운터 초기화
            if (koreaTime.Date != LastResetDate)
            {
                guids.Clear();
                LastResetDate = koreaTime.Date;
            }

            if (!guids.Contains(guid))
            {
                guids.Add(guid);
                await dBConnectionHistory.AddCount(LastResetDate.ToString("yyyyMMdd"));
            }
            await GetCounterValue();
        }

        private DateTime GetKoreaTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time"));
        }
    }
}
