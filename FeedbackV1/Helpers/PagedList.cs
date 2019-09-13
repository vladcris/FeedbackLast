using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FeedbackV1 {

    public class PagedList<T> : List<T> //make it generic
    {
        public int CurrentPage {get; set;}
        public int TotalPages {get; set;}
        public int PageSize {get; set;}
        public int TotalCount {get; set;}

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }
        public static PagedList<T> Create(IQueryable<T> source,
             int pageNumber, int pageSize)
        {
            var count =  source.Count();
            var items =  source.Skip((pageNumber-1) * pageSize)
                                    .Take(pageSize).ToList();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}