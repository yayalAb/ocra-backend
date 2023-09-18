using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
namespace AppDiv.CRVS.Domain.Enums
{
    public enum SqlOperator
    {
        Contain,
        GreaterThan,
        LessThan,
        EqualTo,
        Between,
        NotContain,
        LessThanOrEqual,
        GreaterThanOrEqual,
        NotEqual,
        NotBetween,
        IN,
        ISNULL,
        ISNOTNULL
    }
    public enum SqlAggregate
    {
        Max,
        Min,
        Count,
        Sum,
        Average,             
        GroupBy,
        OrderBy,
        OrderByDesc
    }
}