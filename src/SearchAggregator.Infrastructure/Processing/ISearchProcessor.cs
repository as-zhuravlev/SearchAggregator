using System;
using System.Collections.Generic;
using System.Text;

using SearchAggregator.Core.Entities;

namespace SearchAggregator.Infrastructure.Processing
{
    public interface ISearchProcessor
    {
        SearchResult Search(string request, bool onlyInDatabase);
    }
}
