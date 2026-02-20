using TelecomPM.Domain.Entities.WorkOrders;
using TelecomPM.Domain.Enums;

namespace TelecomPM.Domain.Services;

public interface ISlaClockService
{
    bool IsBreached(DateTime createdAtUtc, int responseMinutes, SlaClass slaClass);
    DateTime CalculateDeadline(DateTime createdAtUtc, SlaClass slaClass);
    SlaStatus EvaluateStatus(WorkOrder workOrder);
}
