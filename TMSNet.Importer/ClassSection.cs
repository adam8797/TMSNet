namespace TMSNet.Importer
{
    public class ClassSection : ClassDefinition
    {
        public ClassSection() { }

        public ClassSection(ClassDefinition def)
        {
            Utils.CopyCast(def, this);
        }

        public string Term { get; set; }
        public string School { get; set; }
        public string Subject { get; set; }
    }
}
