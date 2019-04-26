using System;
using System.Collections.Generic;
using System.Text;

namespace SearchAggregator.Core.Entities
{
    public class Url : BaseEntity
    {
        public Url()
        {
            UrlSearchResults = new List<UrlSearchResult>();
        }

        public string UrlString { get; set; }

        public string Title { get; set; }

        public string Snippet { get; set; }
        
        public ICollection<UrlSearchResult> UrlSearchResults { get; set; }

        public override string ToString() //for debug
        {
            return UrlString ?? "";
        }
    }
}
