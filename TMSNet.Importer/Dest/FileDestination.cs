using System.Collections.Generic;
using System.IO;

namespace TMSNet.Importer.Dest
{
    public abstract class FileDestination : ISectionDestination
    {
        protected readonly string Path;
        protected List<ClassSection> Sections;

        protected FileDestination(string path)
        {
            Sections = new List<ClassSection>();
            Path = path;
            File.Create(Path).Close();
        }

        public void Add(ClassSection cs)
        {
            Sections.Add(cs);
        }

        public void CommitChanges()
        {
            //Do Nothing, we only want to write on close
        }

        protected string GetDays(DayOfWeek day)
        {
            string s = "";

            if ((day & DayOfWeek.Monday) == DayOfWeek.Monday)
                s += "M";
            if ((day & DayOfWeek.Tuesday) == DayOfWeek.Tuesday)
                s += "T";
            if ((day & DayOfWeek.Wednesday) == DayOfWeek.Wednesday)
                s += "W";
            if ((day & DayOfWeek.Thursday) == DayOfWeek.Thursday)
                s += "R";
            if ((day & DayOfWeek.Friday) == DayOfWeek.Friday)
                s += "F";
            if ((day & DayOfWeek.Saturday) == DayOfWeek.Saturday)
                s += "S";

            return s;
        }

        public abstract void Close();
    }
}