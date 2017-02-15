using System;
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

                        try
                        {
                            var cs = new ClassSection(classDefinition)
                            {
                                School = school.Name,
                                Term = term.Name,
                                Subject = subject.Name
                            };

                            dest.Add(cs);
                        }
                        catch (Exception e)
                        {
                            var p = Console.CursorTop;
                            Console.CursorTop = Console.WindowHeight - 1;
                            Console.WriteLine(e);
                            Console.CursorTop = p;
                        }
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
            Console.WriteLine("Closing...");
            dest.Close();
            Console.WriteLine("Finished");
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
            Console.WriteLine(s + new string(' ', Console.BufferWidth - s.Length - 1));
        }
    }
}
