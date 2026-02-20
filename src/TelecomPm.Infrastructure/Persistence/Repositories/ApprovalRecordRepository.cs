namespace TelecomPM.Infrastructure.Persistence.Repositories;

using TelecomPM.Domain.Entities.ApprovalRecords;
using TelecomPM.Domain.Interfaces.Repositories;

public class ApprovalRecordRepository : Repository<ApprovalRecord, Guid>, IApprovalRecordRepository
{
    public ApprovalRecordRepository(ApplicationDbContext context) : base(context)
    {
    }
}
