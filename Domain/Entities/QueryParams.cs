namespace ArchieHealthTracker.Domain.Entities;

public record QueryParams(
    int Limit,
    DateTime? From = null,
    DateTime? To = null,
    bool OrderByDescending = true
);