using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;

using SearchAggregator.Core.Interfaces;
using SearchAggregator.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace SearchAggregator.Infrastructure.Data
{
    public class EfRepository : IRepository
    {
        private readonly AppDbContext _dbContext;

        public EfRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public T GetById<T>(Guid id) where T : BaseEntity
        {
            return _dbContext.Set<T>().SingleOrDefault(e => e.Id == id);
        }

        public List<T> List<T>() where T : BaseEntity
        {
            return _dbContext.Set<T>().ToList();
        }

        public T Add<T>(T entity) where T : BaseEntity
        {
            _dbContext.Set<T>().Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public void Delete<T>(T entity) where T : BaseEntity
        {
            _dbContext.Set<T>().Remove(entity);
            _dbContext.SaveChanges();
        }

        public void Update<T>(T entity) where T : BaseEntity
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public T FirstOrDefault<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity
        {
            return _dbContext.Set<T>().FirstOrDefault(predicate);
        }
        
        public SearchResult FindSearchResult(string request)
        {
             return _dbContext.SearchResults.Where(sr => sr.SearchRequest == request)
                                            .Include(x => x.UrlSearchResults)
                                                .ThenInclude(usr => usr.UrlSearchResultSearchEngines)
                                            .Include(x => x.UrlSearchResults)
                                                .ThenInclude(usr => usr.Url)
                                            .FirstOrDefault();
        }
    }
}
