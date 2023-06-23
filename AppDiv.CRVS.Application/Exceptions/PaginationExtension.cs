using System;
using System.Collections.Generic;
using System.Linq;
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
            return new PaginatedList<TDto>(
                        CustomMapper.Mapper.Map<List<TDto>>(items), 
                        count,
                        pageCount, pageSize);
        }
    }
}