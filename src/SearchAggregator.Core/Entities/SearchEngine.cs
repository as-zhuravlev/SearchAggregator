using System;
using System.Collections.Generic;
using System.Text;

namespace SearchAggregator.Core.Entities
{
    public class SearchEngine : BaseEntity
    {
        public string Name { get; set; }

        //public ICollection<UrlSearchResultSearchEngine> UrlSearchResultSearchEngines { get; set; }
    }
}
