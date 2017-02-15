namespace TMSNet.Importer.Dest
{
    interface ISectionDestination
    {
        void Add(ClassSection cs);

        void CommitChanges();
    }
}
