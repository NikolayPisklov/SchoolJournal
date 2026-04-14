using Microsoft.EntityFrameworkCore.Storage;

namespace SchoolJournalApi.Services
{
    public interface IContextService
    {
        Task SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
