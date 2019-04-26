using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace SearchAggregator.Core.Entities
{
    public class SearchResult : BaseEntity
    {
        public SearchResult()
        {
            UrlSearchResults = new List<UrlSearchResult>(); 
        }

        public string SearchRequest { get; set; }

        public virtual ICollection<UrlSearchResult> UrlSearchResults { get; set; }
    }
}
