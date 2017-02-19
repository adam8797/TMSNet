using System;
using System.Globalization;
using System.Linq;
using CsQuery;

namespace TMSNet
{
    public class ClassDefinition
    {
        public ClassDefinition() {}

        public static ClassDefinition FromDetailTable(IDomObject domObject)
        {
            return new ClassDefinition();
        }

        public static ClassDefinition FromTr(IDomObject domObject)
        {
            var tds = domObject.Cq().Children("td").ToList();

            var cd = new ClassDefinition
            {
                SubjectCode = Amp(tds[0].InnerText),
                CourseNumber = Amp(tds[1].InnerText),
                InstructionType = Amp(tds[2].InnerText),
                InstructionMethod = Amp(tds[3].InnerText),
                Section = Amp(tds[4].InnerText),
                CourseTitle = Amp(tds[6].InnerText),
                Instructor = Amp(tds[8].InnerText)
            };

            var t = domObject.ChildElements.ElementAt(7);
            var rows = t.FirstElementChild.FirstElementChild.FirstElementChild.ChildElements.ToList();

            var days = rows.ElementAt(0);
            var time = rows.ElementAt(1);

            if (days.InnerText.Contains("M"))
                cd.Days |= DayOfWeek.Monday;
            if (days.InnerText.Contains("T"))
                cd.Days |= DayOfWeek.Tuesday;
            if (days.InnerText.Contains("W"))
                cd.Days |= DayOfWeek.Wednesday;
            if (days.InnerText.Contains("R"))
                cd.Days |= DayOfWeek.Thursday;
            if (days.InnerText.Contains("F"))
                cd.Days |= DayOfWeek.Friday;
            if (days.InnerText.Contains("S"))
                cd.Days |= DayOfWeek.Saturday;

            var times = time.InnerText.Split('-');
            if (time.InnerText == "TBD")
            {
                cd.StartTime = TimeSpan.Zero;
                cd.EndTime = TimeSpan.Zero;
            }
            else
            {
                var start = times[0].Trim();
                var end = times[1].Trim();

                cd.StartTime = DateTime.ParseExact(start, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
                cd.EndTime = DateTime.ParseExact(end, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
            }

            var crn = domObject.ChildElements.ElementAt(5).FirstElementChild.FirstElementChild;

            cd.Crn = crn.InnerText;

            return cd;
        }

        public static string Amp(string s)
        {
            return s.Replace("&amp;", "&");
        }

        public string SubjectCode { get; set; }
        public string CourseNumber { get; set; }
        public string InstructionType { get; set; }
        public string InstructionMethod { get; set; }
        public string Section { get; set; }
        public string Crn { get; set; }
        public string CourseTitle { get; set; }
        public string Instructor { get; set; }

        public int MaxEnroll { get; set; }
        public int Enroll { get; set; }
        public int Credits { get; set; }

        public DayOfWeek Days { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public override string ToString()
        {
            if (Crn != null)
                return CourseTitle + "  -  " + Instructor + "  ( " + Crn + " )";
            return CourseTitle + "  -  " + Instructor;
        }
    }
}