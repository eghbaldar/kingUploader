using KingUploader.Core.Domain.File;
using Microsoft.EntityFrameworkCore;

namespace KingUploader.Core.Application.Interfaces.Context
{
    public interface IDataBaseContext
    {
        DbSet<Files> Files { get; set; }

        //SaveChanges
        int SaveChanges(bool acceptAllChangesOnSuccess);
        int SaveChanges();
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken());
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
    }
}
