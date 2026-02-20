using TelecomPM.Domain.Entities.AuditLogs;

namespace TelecomPM.Domain.Interfaces.Repositories;

public interface IAuditLogRepository : IRepository<AuditLog, Guid>
{
}
