namespace TMSNet.Importer
{
    public class ClassSection : ClassDefinition
    {
        public ClassSection(ClassDefinition def)
        {
            SubjectCode = def.SubjectCode;
            CourseNumber = def.CourseNumber;
            InstructionType = def.InstructionType;
            InstructionMethod = def.InstructionMethod;
            Section = def.Section;
            Crn = def.Crn;
            CourseTitle = def.CourseTitle;
            Instructor = def.Instructor;
            Days = def.Days;
            StartTime = def.StartTime;
            EndTime = def.EndTime;
        }

        public string Term { get; set; }
        public string School { get; set; }
        public string Subject { get; set; }
    }
}
