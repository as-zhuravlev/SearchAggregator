using System;
using System.Collections.Generic;
using System.Text;

namespace SearchAggregator.Core.Entities
{
    public class UrlSearchResult : BaseEntity
    {
        public UrlSearchResult()
        {
            UrlSearchResultSearchEngines = new List<UrlSearchResultSearchEngine>();
        }

        public Guid UrlId { get; set; }

        public Url Url{ get; set; }

        public Guid SearchResultId { get; set; }

        public SearchResult SearchResult { get; set; }
        
        public ICollection<UrlSearchResultSearchEngine> UrlSearchResultSearchEngines { get; set; }
    }
}
