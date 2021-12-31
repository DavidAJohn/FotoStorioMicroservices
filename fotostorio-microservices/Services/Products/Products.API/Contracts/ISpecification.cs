﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Products.API.Contracts
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; } // .Where
        List<Expression<Func<T, object>>> Includes { get; }
        Expression<Func<T, object>> OrderBy { get; }
        Expression<Func<T, object>> OrderByDescending { get; }
        int Take { get; }
        int Skip { get; }
        bool IsPagingEnabled { get; }
    }
}
