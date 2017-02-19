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
            var tds = domObject.Cq().Children("tr").Children("td:last-child").Select(x => x.InnerText.Trim()).ToList();

            var d = domObject.Cq()
                .Parent()
                .Parent()
                .Parent()
                .Siblings()
                .Find(".tableHeader")
                .Siblings()
                .Children()
                .Select(x => x.InnerText).ToList();

            var cd = new ClassDefinition()
            {
                Crn = tds[0],
                SubjectCode = tds[1],
                CourseNumber = tds[2],
                Section = tds[3],
                CourseTitle = tds[5],
                Campus = tds[6],
                Instructor = tds[7],
                InstructionType = tds[8],
                InstructionMethod = tds[9],
                MaxEnroll = int.Parse(tds[10]),

                StartDate = DateTime.Parse(d[0]),
                EndDate = DateTime.Parse(d[1]),
                Days = ParseDays(d[3])
            };

            if (tds[11] == "CLOSED")
                cd.Enroll = cd.MaxEnroll;
            else
                cd.Enroll = int.Parse(tds[11]);


            decimal c;
            if (decimal.TryParse(tds[4], out c))
            {
                cd.MinCredits = cd.MaxCredits = c;
            }
            else
            {
                var creditsRange = tds[4].Split(new[] {"TO", "OR"}, StringSplitOptions.RemoveEmptyEntries);
                
                cd.MinCredits = decimal.Parse(creditsRange[0]);
                cd.MaxCredits = decimal.Parse(creditsRange[1]);
            }



            var times = d[2].Split('-');
            if (d[2] == "TBD")
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

            return cd;
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

            cd.Days = ParseDays(days.InnerText);

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
        public string Campus { get; set; }

        public int MaxEnroll { get; set; }
        public int Enroll { get; set; }
        public decimal MaxCredits { get; set; }
        public decimal MinCredits { get; set; }

        public DayOfWeek Days { get; set; }

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public override string ToString()
        {
            if (Crn != null)
                return CourseTitle + "  -  " + Instructor + "  ( " + Crn + " )";
            return CourseTitle + "  -  " + Instructor;
        }

        private static DayOfWeek ParseDays(string days)
        {
            DayOfWeek day = 0;

            if (days.Contains("M"))
                day |= DayOfWeek.Monday;
            if (days.Contains("T"))
                day |= DayOfWeek.Tuesday;
            if (days.Contains("W"))
                day |= DayOfWeek.Wednesday;
            if (days.Contains("R"))
                day |= DayOfWeek.Thursday;
            if (days.Contains("F"))
                day |= DayOfWeek.Friday;
            if (days.Contains("S"))
                day |= DayOfWeek.Saturday;

            return day;
        }
    }
}