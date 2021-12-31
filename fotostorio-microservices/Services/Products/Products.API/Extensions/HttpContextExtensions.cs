using Microsoft.AspNetCore.Http;
using Products.API.Helpers;
using System;
using System.Text.Json;

namespace Products.API.Extensions
{
    public static class HttpContextExtensions
    {
        public static void AddPaginationResponseHeaders(
                this HttpContext httpContext,
                int recordCount,
                int pageSize,
                int pageIndex
            )
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            // calculate total number of pages
            int totalPages = (int)Math.Ceiling(recordCount / (double)pageSize);

            // are there previous or next pages?
            bool hasPrevious = pageIndex > 1;
            bool hasNext = pageIndex < totalPages;

            var paginationHeader = new PaginationResponseHeader
            {
                TotalCount = recordCount,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = totalPages,
                HasPreviousPage = hasPrevious,
                HasNextPage = hasNext
            };

            httpContext.Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader));
        }
    }
}
