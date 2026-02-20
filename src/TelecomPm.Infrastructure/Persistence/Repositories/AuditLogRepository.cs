namespace TelecomPM.Infrastructure.Persistence.Repositories;

using TelecomPM.Domain.Entities.AuditLogs;
using TelecomPM.Domain.Interfaces.Repositories;

public class AuditLogRepository : Repository<AuditLog, Guid>, IAuditLogRepository
{
    public AuditLogRepository(ApplicationDbContext context) : base(context)
    {
    }
}
