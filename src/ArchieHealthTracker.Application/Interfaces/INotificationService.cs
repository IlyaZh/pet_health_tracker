using ArchieHealthTracker.Domain.Entities;

namespace ArchieHealthTracker.Application.Interfaces;

public interface INotificationService
{
    Task SendAsync(long chatId, ReportResult reportResult, CancellationToken ct);
}