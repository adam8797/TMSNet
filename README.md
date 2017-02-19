# TMS.Net
A .NET Library to Scrape the Drexel Term Master Schedule. The process is not quick, as it loads every page that the TMS can produce once.

The process takes about 5 mins per term in quick mode, and up to 30 mins in slow mode.

## NuGet

TMS.Net is available on [NuGet](https://www.nuget.org/packages/TMS.Net/1.0.0)!

```
PM> Install-Package TMS.Net
```

This project contains two segments, `TMSNet` and `TMSNet.Importer`

## TMSNet

TMSNet contains the core scraping functionality. It loads and parses the pages, pulling out data as it goes.


### Quick Mode
A basic TMSNet example, that prints out every class, for every term.

```C#
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
```

### Slow Mode
A slightly more complicated example, that retrieves full information for each CRN.

```C#
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
	            foreach (var crn in scraper.GetCrns(subject))
	            {
                    var cd = scraper.GetDetailedClass(crn);
                    Console.WriteLine("{0} Enroll: {1}; MaxEnroll: {2}; Credits: {3}", 
                        cd.CourseTitle, cd.Enroll, cd.MaxEnroll, cd.Credits);
	            }
	        }
	    }
    }
```

### Public Methods

```C#
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

        public IEnumerable<Page> GetCrns(Page subject) { ... }
        public async Task<IEnumerable<Page>> GetCrnsAsync(Page subject) { ... }

        public ClassDefinition GetDetailedClass(Page crn) { ... }
        public async Task<ClassDefinition> GetDetailedClassAsync(Page crn) { ... }
    }
```

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
