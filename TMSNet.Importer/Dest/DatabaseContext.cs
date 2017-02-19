using System.Data.Entity;

namespace TMSNet.Importer.Dest
{
    public class DatabaseContext : DbContext, ISectionDestination
    {
        public DatabaseContext() : base("Data Source=localhost;Initial Catalog=DragonSchedule;Integrated Security=True")
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

            modelBuilder.Entity<ClassSection>().Property(x => x.StartDate).HasColumnType("datetime2");
            modelBuilder.Entity<ClassSection>().Property(x => x.EndDate).HasColumnType("datetime2");
        }

        public void Close()
        {
            // Does Nothing
        }
    }
}
