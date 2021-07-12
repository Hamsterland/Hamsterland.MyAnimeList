using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hamsterland.MyAnimeList.Data;
using Hamsterland.MyAnimeList.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Hamsterland.MyAnimeList.Services.Accounts
{
    public interface IAccountService
    {
        Task<MalAccountDto> GetByUserId(ulong userId);
        Task<MalAccountDto> GetByMalId(int malId);
        Task<List<MalAccountDto>> GetAllAccounts();
        Task Create(ulong userId, int malId);
        Task Create(ulong userId);
        Task Update(ulong userId, int? malId);
        Task Delete(ulong userId);
    }

    public class AccountService : IAccountService
    {
        private readonly IDbContextFactory<HamsterlandContext> _contextFactory;

        public AccountService(IDbContextFactory<HamsterlandContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<MalAccountDto> GetByUserId(ulong userId)
        {
            await using var context = _contextFactory.CreateDbContext();
            
            return (await context
                    .MalAccounts
                    .AsQueryable()
                    .FirstOrDefaultAsync(x => x.UserId == userId))?
                .ToMalAccountDto();
        }

        public async Task<MalAccountDto> GetByMalId(int malId)
        {
            await using var context = _contextFactory.CreateDbContext();
            
            return (await context
                    .MalAccounts
                    .AsQueryable()
                    .FirstOrDefaultAsync(x => x.MalId == malId))?
                .ToMalAccountDto();
        }
        
        public async Task<List<MalAccountDto>> GetAllAccounts()
        {
            await using var context = _contextFactory.CreateDbContext();
            
            return await context
                .MalAccounts
                .AsQueryable()
                .Select(x => x.ToMalAccountDto())
                .ToListAsync();
        }


        public async Task Create(ulong userId, int malId)
        {
            await using var context = _contextFactory.CreateDbContext();
            
            var account = new MalAccount
            {
                UserId = userId,
                MalId = malId
            };

            context.Add(account);
            await context.SaveChangesAsync();
        }
        
        public async Task Create(ulong userId)
        {
            await using var context = _contextFactory.CreateDbContext();
            
            var account = new MalAccount
            {
                UserId = userId,
            };

            context.Add(account);
            await context.SaveChangesAsync();
        }

        public async Task Update(ulong userId, int? malId)
        {
            await using var context = _contextFactory.CreateDbContext();
            
            var account = await context
                .MalAccounts
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (account is null)
            {
                throw new Exception($"Account {userId} was not found in the database.");
            }

            account.MalId = malId;
            await context.SaveChangesAsync();
        }

        public async Task Delete(ulong userId)
        {
            await using var context = _contextFactory.CreateDbContext();
            
            var account = await context
                .MalAccounts
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (account is null)
            {
                throw new Exception($"Account {userId} was not found in the database.");
            }

            context.Remove(account);
            await context.SaveChangesAsync();
        }
    }
}