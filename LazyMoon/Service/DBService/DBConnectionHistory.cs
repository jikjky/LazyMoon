using LazyMoon.Model;
using LazyMoon.Model.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

#nullable enable
namespace LazyMoon.Service
{
    public class DBConnectionHistory
    {
        readonly protected IDbContextFactory<AppDbContext> _contextFactory;

        public DBConnectionHistory(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task AddCount(string Date)
        {
            var dbContext = await _contextFactory.CreateDbContextAsync();
            var dataHistory = dbContext.ConnectionHistorys.Where(x => x.Date == Date);
            if (dataHistory.Any())
            {
                var history = dataHistory.First();
                if (history != null)
                {
                    history.Count++;
                }
            }
            else
            {
                dbContext.ConnectionHistorys.Add(new Model.ConnectionHistory() { Date = Date, Count = 1 });
            }
            await dbContext.SaveChangesAsync();
        }

        public async Task<int> GetCount(string Date)
        {
            var dbContext = await _contextFactory.CreateDbContextAsync();
            var dataHistory = dbContext.ConnectionHistorys.Where(x => x.Date == Date);
            if (dataHistory.Any())
            {
                var history = dataHistory.First();
                return history?.Count ?? 0;
            }
            return 0;
        }
    }
}
