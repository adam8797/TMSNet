using System.Data.Entity;

namespace TMSNet.Importer.Dest
{
    public class DatabaseContext : DbContext, ISectionDestination
    {
        public DatabaseContext() : base("<< Connection String >>")
        {}

        public DbSet<ClassSection> Classes { get; set; }

        public void Add(ClassSection cs)
        {
            Classes.Add(cs);
        }

        public void CommitChanges()
        {
            SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClassSection>().HasKey(x => x.Crn);
        }
    }
}
