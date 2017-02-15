# TMS.Net
A .NET Library to Scrape the Drexel Term Master Schedule. The process is not quick, as it loads every page that the TMS can produce once. This usually takes about five minutes per term.

This project contains two segments, `TMSNet` and `TMSNet.Importer`

## TMSNet

TMSNet contains the core scraping functionality. It loads and parses the pages, pulling out data as it goes.

A basic TMSNet example, that prints out every class, for every term.

    var scraper = new TMSNet.Scraper();

    foreach (var term in scraper.GetTerms())
    {
        Console.WriteLine(term.Name);
	    foreach (var school in scraper.GetSchools(term))
	    {
	        Console.WriteLine("  " + school.Name);
	        foreach (var subject in scraper.GetSubjects(school))
	        {
	            Console.WriteLine("    " + subject.Name);
	            foreach (var classDefinition in scraper.GetClasses(subject))
	            {
	                Console.WriteLine("      " + classDefinition);
	            }
	        }
	    }
    }

### Public Methods

    public class Scraper
    {
        public IEnumerable<Page> GetTerms() { ... }
        public async Task<IEnumerable<Page>> GetTermsAsync() { ... }

        public IEnumerable<Page> GetSchools(Page term) { ... }
        public async Task<IEnumerable<Page>> GetSchoolsAsync(Page term) { ... }

        public IEnumerable<Page> GetSubjects(Page school) { ... }
        public async Task<IEnumerable<Page>> GetSubjectsAsync(Page school) { ... }

        public IEnumerable<ClassDefinition> GetClasses(Page subject) { ... }
        public async Task<IEnumerable<ClassDefinition>> GetClassesAsync(Page subject) { ... }
    }

## TMSNet.Importer

A quick console application that I put together. It uses `TMSNet` to read the WebTMS, and saves all records to a user selectable destination. There are four built-in destinations:

- An SQL Database using the Entity Framework
- A JSON file
- An XML file
- A CSV file

It only does one term at a time, but this is by design and it is capable to scraping all terms in one execution.

There are no parameters, instead the user is prompted for input. This will change in future versions.

## Dependencies

### TMSNet

- CsQuery (https://github.com/jamietre/CsQuery/)

### TMSNet.Importer

- Newtonsoft.Json
- EntityFramework