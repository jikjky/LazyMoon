using LazyMoon.Model;
using LazyMoon.Service.DBService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace LazyMoon.Service
{
    public sealed class ServerCounterService(DBConnectionHistory dBConnectionHistory)
    {
        readonly HashSet<string> guids = [];
        public int CounterValue { get; set; }
        private DateTime mLastResetDate = GetKoreaTimeNow().Date;

        readonly DBConnectionHistory dBConnectionHistory = dBConnectionHistory;

        public async Task GetCounterValue()
        {
            CounterValue = await dBConnectionHistory.GetCount(mLastResetDate.ToString("yyyyMMdd"));
        }

        public async void Add(string guid)
        {
            DateTime koreaTime = GetKoreaTimeNow();

            // 날짜가 바뀌면 카운터 초기화
            if (koreaTime.Date != mLastResetDate)
            {
                guids.Clear();
                mLastResetDate = koreaTime.Date;
            }
            if (!guids.Contains(guid))
            {
                await dBConnectionHistory.AddCount(mLastResetDate.ToString("yyyyMMdd"));
            }
            guids.Add(guid);

            await GetCounterValue();
        }

        private static DateTime GetKoreaTimeNow()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time"));
        }
    }
}
