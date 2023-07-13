using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Enums;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class Filter
    {
        public string? PropertyName { get; set; }
        public SqlOperator? Operator { get; set; }
        public string? Value { get; set; }
        public string? Value2 { get; set; }

    }
    public class Aggregate
    {
        public string? PropertyName { get; set; }
        public SqlAggregate? AggregateMethod { get; set; }

    }
}