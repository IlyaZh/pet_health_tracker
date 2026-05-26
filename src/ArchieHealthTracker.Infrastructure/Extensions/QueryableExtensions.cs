using ArchieHealthTracker.Domain.Entities;

namespace ArchieHealthTracker.Infrastructure.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplyBaseParams<T>(this IQueryable<T> query, QueryParams? parameters)
        where T : class, IHasCreatedAt
    {
        if (parameters == null) return query;

        if (parameters.From.HasValue) query = query.Where(x => x.CreatedAt >= parameters.From.Value);

        if (parameters.To.HasValue) query = query.Where(x => x.CreatedAt <= parameters.To.Value);

        if (parameters.OrderByDescending)
            query = query.OrderByDescending(x => x.CreatedAt);
        else
            query = query.OrderBy(x => x.CreatedAt);

        query = query.Take(parameters.Limit);

        return query;
    }
}