using System;
using System.Linq;
using TMSNet.Importer.Dest;

namespace TMSNet.Importer
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new DatabaseContext();
            var scraper = new Scraper();

            var terms = scraper.GetTerms().ToList();

            Console.WriteLine("Select a term to Scrape:");
            for (int i = 0; i < terms.Count; i++)
            {
                Console.WriteLine("[{0}] {1}", i + 1, terms[i].Name);
            }

            Console.Write("~> ");
            int t;
            if (!int.TryParse(Console.ReadLine(), out t) || t > terms.Count || t < 1)
            {
                Console.WriteLine("Invalid Input");
                return;
            }

            ISectionDestination dest = new DatabaseContext();
            
            var term = terms[t - 1];

            WriteWholeLine(term.Name);
            foreach (var school in scraper.GetSchools(term))
            {
                WriteWholeLine("  " + school.Name);
                foreach (var subject in scraper.GetSubjects(school))
                {
                    WriteWholeLine("    " + subject.Name);
                    foreach (var classDefinition in scraper.GetClasses(subject))
                    {
                        WriteWholeLine("      " + classDefinition);
                        dest.Add(new ClassSection(classDefinition)
                        {
                            School = school.Name,
                            Term = term.Name,
                            Subject = subject.Name
                        });
                        Console.CursorTop--;
                    }
                    WriteWholeLine("      Saving Changes");
                    dest.CommitChanges();
                    Console.CursorTop--;
                    WriteWholeLine("");
                    Console.CursorTop -= 2;

                }
                Console.CursorTop--;
            }
            Console.CursorTop--;
        }

        static void WriteWholeLine(string s)
        {
            Console.WriteLine(s + new string(' ', Console.BufferWidth - s.Length - 1));
        }
    }
}
