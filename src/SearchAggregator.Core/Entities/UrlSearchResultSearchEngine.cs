using System;
using System.Collections.Generic;
using System.Text;

namespace SearchAggregator.Core.Entities
{
    public class UrlSearchResultSearchEngine : BaseEntity
    {
        public Guid UrlSearchResultId { get; set; }

        public UrlSearchResult UrlSearchResult { get; set; }

        public Guid SearchEngineId { get; set; }

        public SearchEngine SearchEngine { get; set; }

        public bool IsNew { get; set; }
    }
}
