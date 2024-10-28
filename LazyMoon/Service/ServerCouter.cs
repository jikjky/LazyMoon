using LazyMoon.Model;
using LazyMoon.Service.DBService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace LazyMoon.Service
{
    public sealed class ServerCounterService(DBConnectionHistory _dBConnectionHistory)
    {
        readonly HashSet<string> guids = [];
        public int CounterValue { get; set; }
        private DateTime LastResetDate = GetKoreaTime().Date;

        readonly DBConnectionHistory dBConnectionHistory = _dBConnectionHistory;

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
                await dBConnectionHistory.AddCount(LastResetDate.ToString("yyyyMMdd"));
            }
            guids.Add(guid);

            await GetCounterValue();
        }

        private static DateTime GetKoreaTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time"));
        }
    }
}
