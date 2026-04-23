using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SchoolJournalApi.Exceptions;
using SchoolJournalApi.Models;
using System.Data.Common;

namespace SchoolJournalApi.Services
{
    public class ContextService : IContextService
    {
        private readonly SchoolJournalDbContext _context;

        public ContextService(SchoolJournalDbContext context)
        {
            _context = context;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            try
            {
                return await _context.Database.BeginTransactionAsync();
            }
            catch (Exception ex) 
            {
                throw new EfDbException("An error occured while beginning a transaction!", ex);
            }
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw;
            }
        }
    }
}
