using KingUploader.Core.Application.Interfaces.Context;
using KingUploader.Core.Domain.File;
using Microsoft.EntityFrameworkCore;

namespace KingUploader.Core.Persistence
{
    public class DataBaseContext:DbContext, IDataBaseContext
    {
        public DataBaseContext(DbContextOptions options): base(options)
        {

        }
        public DbSet<Files> Files { get; set; }
        public DbSet<MultiFiles> MultiFiles { get; set; }
    }
}
