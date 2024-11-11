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
namespace LazyMoon.Service.DBService
{
    public class DBConnectionHistory(IDbContextFactory<AppDbContext> contextFactory)
    {
        readonly protected IDbContextFactory<AppDbContext> contextFactory = contextFactory;

        public async Task AddCount(string date)
        {
            var dbContext = await contextFactory.CreateDbContextAsync();
            var dataHistory = dbContext.ConnectionHistorys.Where(x => x.Date == date);
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
                dbContext.ConnectionHistorys.Add(new ConnectionHistory() { Date = date, Count = 1 });
            }
            await dbContext.SaveChangesAsync();
        }

        public async Task<int> GetCount(string date)
        {
            var dbContext = await contextFactory.CreateDbContextAsync();
            var dataHistory = dbContext.ConnectionHistorys.Where(x => x.Date == date);
            if (dataHistory.Any())
            {
                var history = dataHistory.First();
                return history?.Count ?? 0;
            }
            return 0;
        }
    }
}
