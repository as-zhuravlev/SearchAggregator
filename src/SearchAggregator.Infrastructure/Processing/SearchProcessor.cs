using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using Microsoft.Extensions.Logging;

using SearchAggregator.Core.Entities;
using SearchAggregator.Core.Interfaces;
using SearchAggregator.Core.Shared;

namespace SearchAggregator.Infrastructure.Processing
{
    public class SearchProcessor : ISearchProcessor
    {
        private static readonly Regex whiteSpaceRegex = new Regex(@"\s");

        private readonly Dictionary<Guid, ISearchEngine> _engines;
        private readonly IRepository _repository;
        private readonly ILogger _logger;
        
        public SearchProcessor(IEnumerable<ISearchEngine> engines, IRepository repository, ILogger<SearchProcessor> logger)
        {
            _logger = logger;
            _repository = repository;
            _engines = new Dictionary<Guid, ISearchEngine>();

            var dbEngines = _repository.List<SearchEngine>().ToDictionary(x => x.Name);
            
            foreach (var e in engines)
            {
                if (!dbEngines.ContainsKey(e.Name))
                {
                    _logger.LogInformation($"Adding new SearchEngine to Db: {e.Name}");
                    var se = _repository.Add<SearchEngine>(new SearchEngine() { Name = e.Name });
                    _engines.Add(se.Id, e);
                }
                else
                {
                    _engines.Add(dbEngines[e.Name].Id, e);
                }
            }
        }

        public SearchResult Search(string request, bool onlyInDatabase)
        {
            request = NormalizeSearchRequest(request);

            if (string.IsNullOrEmpty(request))
            {
                _logger.LogInformation($"Empty search request");
                return new SearchResult();
            }

            var result = _repository.FindSearchResult(request);

            if (result == null)
            {
                _logger.LogInformation($"Adding new SearchResult to Db: {request}");
                result = new SearchResult()
                {
                    SearchRequest = request,
                    UrlSearchResults = new List<UrlSearchResult>(),
                };
                result = _repository.Add<SearchResult>(result);
            }

            if (onlyInDatabase)
                return result;

            return SearchInSearchEngines(request, result);
        }

        private SearchResult SearchInSearchEngines(string request, SearchResult result)
        {
            var urls = new Dictionary<string, (UrlSearchResult, HashSet<Guid>)>(); // create dict with hashsets for avoiding searching in lists
            foreach (var usr in result.UrlSearchResults)
                urls.Add(usr.Url.UrlString, ValueTuple.Create(usr, new HashSet<Guid>(usr.UrlSearchResultSearchEngines.Select(x => x.SearchEngineId))));
            
            foreach (var engine in _engines)
            {
                try
                {
                    AddNewUrlsFromWebSearchToResult(engine, request, result, urls);
                }
                catch (BaseException ex)
                {
                    _logger.LogError(ex, $"Can not search with SearchEngine: {engine.Value.Name}");
                }
            }
            return result;
        }

        private void AddNewUrlsFromWebSearchToResult(KeyValuePair<Guid, ISearchEngine> engine, string request, SearchResult result, Dictionary<string, (UrlSearchResult, HashSet<Guid>)> urls)
        {
            IReadOnlyCollection<Url> urlsFromWebSearch = engine.Value.Search(request);

            foreach (var u in urlsFromWebSearch)    
            {
                if (!urls.ContainsKey(u.UrlString)) // new url has found!
                {
                    var url = _repository.FirstOrDefault<Url>(x => x.UrlString == u.UrlString);

                    if (url == null)
                    {
                        _logger.LogInformation($"Adding Url to Db: {u.UrlString}");
                        url = _repository.Add(u);
                    }

                    var usr = new UrlSearchResult()
                    {
                        UrlId = url.Id,
                        SearchResultId = result.Id,
                    };

                    _logger.LogInformation($"Adding UrlSearchResult to Db: ({u.UrlString})-({request})");
                    usr = _repository.Add<UrlSearchResult>(usr);
                    
                    var usrse = new UrlSearchResultSearchEngine()
                    {
                        IsNew = true,
                        SearchEngineId = engine.Key,
                        SearchEngine = new SearchEngine() { Id = engine.Key, Name = engine.Value.Name },
                        UrlSearchResultId = usr.Id,
                    };

                    _logger.LogInformation("Adding UrlSearchResultSearchEngine to Db: " +
                                           $"({u.UrlString})-({request})-({engine.Value.Name})");

                    usrse = _repository.Add<UrlSearchResultSearchEngine>(usrse);

                    urls.Add(u.UrlString,
                             ValueTuple.Create(usr, new HashSet<Guid>(new[] { engine.Key })));
                }
                else // existed url
                {
                    var usr = urls[u.UrlString];
                    if (!usr.Item2.Contains(engine.Key)) //  if we found it in another search engine 
                    {
                        var usrse = new UrlSearchResultSearchEngine()  // add this information to db
                        {
                            IsNew = true,
                            SearchEngineId = engine.Key,
                            SearchEngine = new SearchEngine() { Id = engine.Key, Name = engine.Value.Name },
                            UrlSearchResultId = usr.Item1.Id,
                        };

                        _logger.LogInformation("Adding UrlSearchResultSearchEngine to Db: " +
                                               $"({u.UrlString})-({request})-({engine.Value.Name})");

                        usrse = _repository.Add<UrlSearchResultSearchEngine>(usrse);

                        usr.Item2.Add(engine.Key);
                    }
                }
            }
        }

        public static string NormalizeSearchRequest(string r)
        {
            if (string.IsNullOrWhiteSpace(r))
                throw new ArgumentException("Emty search request");

            var sb = new StringBuilder(r.Length);
            foreach (var s in whiteSpaceRegex.Split(r))
            {
                sb.Append(s.ToLower());
                sb.Append(' ');
            }
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }
    }
}
