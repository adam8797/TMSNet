using System;
using System.IO;

namespace TMSNet.Importer.Dest
{
    public class CsvDestination : FileDestination
    {
        public CsvDestination(string path)
            : base(path)
        {
        }

        public override void Close()
        {
            using (var writer = new StreamWriter(Path))
            {
                writer.WriteLine("Term,School,Subject,SubjectCode,CourseNumber,InstructionType,InstructionMethod,Section,Crn,CourseTitle,Instructor,Campus,Enroll,MaxEnrollMaxCredits,MinCredits,StartDate,EndDate,Days,StartTime,EndTime");

                foreach (var classSection in Sections)
                {
                    writer.Write(QuoteIfNeeded(classSection.Term) + ",");
                    writer.Write(QuoteIfNeeded(classSection.School) + ",");
                    writer.Write(QuoteIfNeeded(classSection.Subject) + ",");
                    writer.Write(QuoteIfNeeded(classSection.SubjectCode) + ",");
                    writer.Write(QuoteIfNeeded(classSection.CourseNumber) + ",");
                    writer.Write(QuoteIfNeeded(classSection.InstructionType) + ",");
                    writer.Write(QuoteIfNeeded(classSection.InstructionMethod) + ",");
                    writer.Write(QuoteIfNeeded(classSection.Section) + ",");
                    writer.Write(QuoteIfNeeded(classSection.Crn) + ",");
                    writer.Write(QuoteIfNeeded(classSection.CourseTitle) + ",");
                    writer.Write(QuoteIfNeeded(classSection.Instructor) + ",");
                    writer.Write(QuoteIfNeeded(classSection.Campus) + ",");
                    writer.Write(QuoteIfNeeded(classSection.Enroll.ToString()) + ",");
                    writer.Write(QuoteIfNeeded(classSection.MaxEnroll.ToString()) + ",");
                    writer.Write(QuoteIfNeeded(classSection.MaxCredits.ToString()) + ",");
                    writer.Write(QuoteIfNeeded(classSection.MinCredits.ToString()) + ",");
                    writer.Write(QuoteIfNeeded(classSection.StartDate.ToString("d")) + ",");
                    writer.Write(QuoteIfNeeded(classSection.EndDate.ToString("d")) + ",");
                    writer.Write(GetDays(classSection.Days) + ",");
                    writer.Write(DateTime.Today.Add(classSection.StartTime).ToString("hh:mm tt") + ",");
                    writer.WriteLine(DateTime.Today.Add(classSection.EndTime).ToString("hh:mm tt"));
                }
            }
        }

        private string QuoteIfNeeded(string s)
        {
            if (s.Contains(","))
                return '\"' + s + '\"';
            return s;
        }
    }
}