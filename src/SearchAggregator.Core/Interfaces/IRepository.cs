using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

using SearchAggregator.Core.Entities;

namespace SearchAggregator.Core.Interfaces
{
    public interface IRepository
    {
        T GetById<T>(Guid id) where T : BaseEntity;

        List<T> List<T>() where T : BaseEntity;

        T Add<T>(T entity) where T : BaseEntity;

        void Update<T>(T entity) where T : BaseEntity;

        void Delete<T>(T entity) where T : BaseEntity;

        T FirstOrDefault<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity;

        SearchResult FindSearchResult (string request);
    }
}
