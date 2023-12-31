using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Base;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Extensions
{
    public static class PaginationExtension
    {
        public static async Task<PaginatedList<TDto>> PaginateAsync<T, TDto>(this IQueryable<T> source, int pageCount, int pageSize) where TDto: class where T : class
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageCount - 1) * pageSize).Take(pageSize).ToListAsync();
            Type typeT = typeof(T);

            return new PaginatedList<TDto>(
                        (!typeT.Equals(typeof(TDto)) ? CustomMapper.Mapper.Map<List<TDto>>(items) : (List<TDto>)(object)items), 
                        count,
                        pageCount, pageSize);
        }

        public static PaginatedList<TResult> Select<T, TResult>(this PaginatedList<T> source, Func<T, TResult> selector) where T : class where TResult : class
        {
            var list = source.Items.Select(selector).ToList();
            return new PaginatedList<TResult>(list, source.TotalCount, source.PageCount, source.TotalPages);
        }
    }
}