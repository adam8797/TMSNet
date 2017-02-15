using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CsQuery;

namespace TMSNet
{
    public class Scraper
    {
        public Uri BaseUri;
        private readonly WebClient _client;

        public Scraper()
        {
            BaseUri = new Uri("https://duapp2.drexel.edu");
            _client = new WebClient();
        }

        public Scraper(string baseUri)
        {
            BaseUri = new Uri(baseUri);
            _client = new WebClient();
        }


        #region GetTerms

        private IEnumerable<Page> GetTermsFromHtml(string html)
        {
            var sel = (CQ) html;
            var children = sel["div.term"];

            return children.Skip(1).Select(x =>
            {
                var a = x.LastChild;

                return new Page()
                {
                    Name = ClassDefinition.Amp(a.InnerText),
                    Url = a.GetAttribute("href")
                };
            });
        }

        public IEnumerable<Page> GetTerms()
        {
            var html = _client.DownloadString(new Uri(BaseUri, "/webtms_du/app"));

            return GetTermsFromHtml(html);
        }

        public async Task<IEnumerable<Page>> GetTermsAsync()
        {
            var html = await _client.DownloadStringTaskAsync(new Uri(BaseUri, "/webtms_du/app"));

            return GetTermsFromHtml(html);
        }

        #endregion

        #region GetSchools

        private IEnumerable<Page> GetSchoolsFromHtml(string html)
        {
            var sel = (CQ) html;
            var links = sel["div#sideLeft"].Children("a");

            return links.Select(x => new Page()
            {
                Name = ClassDefinition.Amp(x.InnerText),
                Url = x.GetAttribute("href")
            });
        }

        public IEnumerable<Page> GetSchools(Page term)
        {
            var html = _client.DownloadString(new Uri(BaseUri, term.Url));

            return GetSchoolsFromHtml(html);
        }

        public async Task<IEnumerable<Page>> GetSchoolsAsync(Page term)
        {
            var html = await _client.DownloadStringTaskAsync(new Uri(BaseUri, term.Url));

            return GetSchoolsFromHtml(html);
        }

        #endregion

        #region GetSubjects

        private IEnumerable<Page> GetSubjectsFromHtml(string html)
        {
            var sel = (CQ) html;
            var odds = sel["div.odd"].Children("a");
            var evens = sel["div.even"].Children("a");

            foreach (var odd in odds)
            {
                yield return new Page()
                {
                    Name = ClassDefinition.Amp(odd.InnerText),
                    Url = odd.GetAttribute("href")
                };
            }

            foreach (var even in evens)
            {
                yield return new Page()
                {
                    Name = ClassDefinition.Amp(even.InnerText),
                    Url = even.GetAttribute("href")
                };
            }
        }

        public IEnumerable<Page> GetSubjects(Page school)
        {
            var html = _client.DownloadString(new Uri(BaseUri, school.Url));

            return GetSubjectsFromHtml(html);
        }

        public async Task<IEnumerable<Page>> GetSubjectsAsync(Page school)
        {
            var html = await _client.DownloadStringTaskAsync(new Uri(BaseUri, school.Url));

            return GetSchoolsFromHtml(html);
        }

        #endregion

        #region GetClasses

        public IEnumerable<ClassDefinition> GetClassesFromHtml(string html)
        {
            var sel = (CQ) html;
            var rows = sel[".tableHeader"].Siblings(".odd, .even");
            return rows.Select(x => new ClassDefinition(x));
        }

        public IEnumerable<ClassDefinition> GetClasses(Page subject)
        {
            var html = _client.DownloadString(new Uri(BaseUri, subject.Url));

            return GetClassesFromHtml(html);
        }

        public async Task<IEnumerable<ClassDefinition>> GetClassesAsync(Page subject)
        {
            var html = await _client.DownloadStringTaskAsync(new Uri(BaseUri, subject.Url));

            return GetClassesFromHtml(html);
        }

        #endregion
    }

    public class Page
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

    [Flags]
    public enum DayOfWeek
    {
        Monday = 1,
        Tuesday = 2,
        Wednesday = 4,
        Thursday = 8,
        Friday = 16,
        Saturday = 32
    }
}
