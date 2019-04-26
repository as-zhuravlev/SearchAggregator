using System;
using System.Collections.Generic;
using System.Text;
using SearchAggregator.Core.Entities;

namespace SearchAggregator.Core.Interfaces
{
    public interface ISearchEngine
    {
        string Name { get; }
        IReadOnlyCollection<Url> Search(string request);
    }
}
