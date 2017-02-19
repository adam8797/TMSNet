using System;
using System.Diagnostics;
using System.Linq;
using TMSNet.Importer.Dest;

namespace TMSNet.Importer
{
    class Program
    {
        static void Main(string[] args)
        {
            var scraper = new Scraper();
            var terms = scraper.GetTerms().ToList();

            Console.Clear();
            Console.WriteLine("Select a term to Scrape:");
            int t = ChooseOptions(terms.Select(x => x.Name).ToArray());
            var term = terms[t - 1];

            Console.Clear();
            Console.WriteLine("Will scrape " + term.Name);
            Console.WriteLine("Select a data destination:");
            int d = ChooseOptions(
                "Scrape to Blank SQL Database",
                "Scrape to JSON file",
                "Scrape to XML file",
                "Scrape to CSV");

            ISectionDestination dest;
            string path = null;

            if (d > 1)
            {
                Console.Write("Enter Output File Path: ");
                path = Console.ReadLine();
            }

            switch (d)
            {
                case 1:
                    dest = new DatabaseContext();
                    break;
                case 2:
                    dest = new JsonDestination(path);
                    break;
                case 3:
                    dest = new XmlDestination(path);
                    break;
                case 4:
                    dest = new CsvDestination(path);
                    break;
                default:
                    throw new Exception("Invalid Option");
            }

            Console.Clear();

            var watch = Stopwatch.StartNew();

            double prog = 0;

            var schools = scraper.GetSchools(term).ToList();
            double progPerSchool = 1.0 / schools.Count;
            foreach (var school in schools)
            {
                var subjects = scraper.GetSubjects(school).ToList();
                double progPerSubject = progPerSchool / subjects.Count;
                foreach (var subject in subjects)
                {
                    var crns = scraper.GetCrns(subject).ToList();
                    double progPerCrn = progPerSubject / crns.Count;
                    foreach (var crn in crns)
                    {
                        var cd = scraper.GetDetailedClass(crn);

                        var cs = new ClassSection(cd)
                        {
                            School = school.Name,
                            Term = term.Name,
                            Subject = subject.Name
                        };

                        dest.Add(cs);
                        prog += progPerCrn;

                        Console.Write("{0:P2} ({1:F2}s)\r", prog, watch.ElapsedMilliseconds / 1000.0);
                    }
                    dest.CommitChanges();
                }
            }
            Console.WriteLine("\nClosing...");
            dest.Close();
            watch.Stop();
            Console.WriteLine("Finished ({0}ms)", watch.ElapsedMilliseconds);
        }

        static int ChooseOptions(params string[] options)
        {
            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine("[{0}] {1}", i + 1, options[i]);
            }

            int t = -1;
            while (t < 0)
            {
                Console.Write("~> ");
                if (!int.TryParse(Console.ReadLine(), out t) || t > options.Length || t < 1)
                {
                    Console.WriteLine("Invalid Input");
                }
            }

            return t;
        }

        static void WriteWholeLine(string s)
        {
            var len = Console.BufferWidth - s.Length - 1;
            if (len < 0)
                Console.WriteLine(Truncate(s, Console.BufferWidth));
            else
                Console.WriteLine(s + new string(' ', Console.BufferWidth - s.Length - 1));
        }

        static string Truncate(string s, int length)
        {
            if (s.Length > length)
                return s.Substring(0, length - 4) + "...";
            return s;
        }
    }
}
