﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        /// <summary>
        /// Scrapes the list of Terms from the drop down menu on the TMS Home page
        /// </summary>
        /// <returns>An IEnumerable of Term Pages</returns>
        public IEnumerable<Page> GetTerms()
        {
            var html = _client.DownloadString(new Uri(BaseUri, "/webtms_du/app"));

            return GetTermsFromHtml(html);
        }

        /// <summary>
        /// Scrapes the list of Terms from the drop down menu on the TMS Home page asynchronously.
        /// </summary>
        /// <returns>An awaitable Task for an IEnumerable of Term Pages</returns>
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

        /// <summary>
        /// Get a list of schools from the Term Page.
        /// </summary>
        /// <param name="term">The Term page generated from GetTerms()</param>
        /// <returns>An IEnumerable of School Pages</returns>
        public IEnumerable<Page> GetSchools(Page term)
        {
            var html = _client.DownloadString(new Uri(BaseUri, term.Url));

            return GetSchoolsFromHtml(html);
        }

        /// <summary>
        /// Get a list of schools from the Term Page.
        /// </summary>
        /// <param name="term">The Term page generated from GetTerms()</param>
        /// <returns>An awaitable task for an IEnumerable of School Pages</returns>
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

        /// <summary>
        /// Scrapes a list of subjects from the given school page.
        /// </summary>
        /// <param name="school">Page that represents a school. Generated by GetSchools()</param>
        /// <returns>An IEnumerable of Subject Pages</returns>
        public IEnumerable<Page> GetSubjects(Page school)
        {
            var html = _client.DownloadString(new Uri(BaseUri, school.Url));

            return GetSubjectsFromHtml(html);
        }

        /// <summary>
        /// Scrapes a list of subjects from the given school page.
        /// </summary>
        /// <param name="school">Page that represents a school. Generated by GetSchools()</param>
        /// <returns>An awaitable Task for an IEnumerable of Subject Pages</returns>
        public async Task<IEnumerable<Page>> GetSubjectsAsync(Page school)
        {
            var html = await _client.DownloadStringTaskAsync(new Uri(BaseUri, school.Url));

            return GetSchoolsFromHtml(html);
        }

        #endregion

        #region GetClasses

        private IEnumerable<ClassDefinition> GetClassesFromHtml(string html)
        {
            var sel = (CQ) html;
            var rows = sel[".tableHeader"].Siblings(".odd, .even");
            return rows.Select(ClassDefinition.FromTr);
        }

        /// <summary>
        /// Scrapes the list of class sections for a given subject page. 
        /// </summary>
        /// <param name="subject">Page for the subject generated by GetSubjects()</param>
        /// <returns>An IEnumerable of ClassDefinitions</returns>
        public IEnumerable<ClassDefinition> GetClasses(Page subject)
        {
            var html = _client.DownloadString(new Uri(BaseUri, subject.Url));

            return GetClassesFromHtml(html);
        }

        /// <summary>
        /// Scrapes the list of class sections for a given subject page. 
        /// </summary>
        /// <param name="subject">Page for the subject generated by GetSubjects()</param>
        /// <returns>An awaitable Task for an IEnumerable of ClassDefinitions</returns>
        public async Task<IEnumerable<ClassDefinition>> GetClassesAsync(Page subject)
        {
            var html = await _client.DownloadStringTaskAsync(new Uri(BaseUri, subject.Url));

            return GetClassesFromHtml(html);
        }

        #endregion

        #region GetCrns

        private IEnumerable<Page> GetCrnsFromHtml(string html)
        {
            var sel = (CQ)html;
            var rows = sel[".tableHeader"].Siblings(".odd, .even");
            return rows.Select(x =>
            {
                var crn = x.ChildElements.ElementAt(5).FirstElementChild.FirstElementChild;

                return new Page()
                {
                    Name = crn.InnerText,
                    Url = crn.GetAttribute("href")
                };
            });
        }

        /// <summary>
        /// Get a list of CRNs from a subject page
        /// </summary>
        /// <param name="subject">Page to scrape</param>
        /// <returns>An IEnumerable of Detailed Class Pages</returns>
        public IEnumerable<Page> GetCrns(Page subject)
        {
            var html = _client.DownloadString(new Uri(BaseUri, subject.Url));

            return GetCrnsFromHtml(html);

        }

        /// <summary>
        /// Get a list of CRNs from a subject page, async.
        /// </summary>
        /// <param name="subject">Page to scrape</param>
        /// <returns>An awaitable task for an IEnumerable of Detailed Class Pages</returns>
        public async Task<IEnumerable<Page>> GetCrnsAsync(Page subject)
        {
            var html = await _client.DownloadStringTaskAsync(new Uri(BaseUri, subject.Url));

            return GetCrnsFromHtml(html);
        }

        #endregion

        #region GetDetailedClasses

        private ClassDefinition GetDetailedClassFromHtml(string html)
        {
            var sel = (CQ)html;
            var table = sel["td.tableHeader"].First().Parent().Parent().Selection.FirstOrDefault();

            if (table == null)
                return null;

            return ClassDefinition.FromDetailTable(table);
        }

        /// <summary>
        /// Get detailed class information from a Detailed Class page
        /// </summary>
        /// <param name="crn">Detailed Class page to scrape</param>
        /// <returns>A single ClassDefinition that contains more information than the GetClasses() function.</returns>
        public ClassDefinition GetDetailedClass(Page crn)
        {
            var html = _client.DownloadString(new Uri(BaseUri, crn.Url));
            return GetDetailedClassFromHtml(html);
        }
        /// <summary>
        /// Get detailed class information from a Detailed Class page
        /// </summary>
        /// <param name="crn">Detailed Class page to scrape</param>
        /// <returns>An awaitable Task for a single ClassDefinition that contains more information than the GetClasses() function.</returns>
        public async Task<ClassDefinition> GetDetailedClassAsync(Page crn)
        {
            var html = await _client.DownloadStringTaskAsync(new Uri(BaseUri, crn.Url));
            return GetDetailedClassFromHtml(html);
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
