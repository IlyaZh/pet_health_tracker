using ArchieHealthTracker.Entities;
using ArchieHealthTracker.Extensions.Interfaces;

namespace ArchieHealthTracker.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplyBaseParams<T>(this IQueryable<T> query, QueryParams? parameters)
        where T : class, IHasCreatedAt
    {
        if (parameters == null) return query;
        query = query.Take(parameters.Limit);

        if (parameters.From.HasValue)
        {
            query = query.Where(x => x.CreatedAt >= parameters.From.Value);
        }

        if (parameters.To.HasValue)
        {
            query = query.Where(x => x.CreatedAt <= parameters.To.Value);
        }

        if (parameters.OrderByDescending)
        {
            query = query.OrderByDescending(x => x.CreatedAt);
        }
        else
        {
            query = query.OrderBy(x => x.CreatedAt);
        }

        return query;

    }
}