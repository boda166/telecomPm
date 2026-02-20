using TelecomPM.Domain.Entities.WorkOrders;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Services;

namespace TelecomPM.Infrastructure.Services;

public class SlaClockService : ISlaClockService
{
    public bool IsBreached(DateTime createdAtUtc, int responseMinutes, SlaClass slaClass)
    {
        if (slaClass == SlaClass.P4)
            return false;

        var respondedAtUtc = createdAtUtc.AddMinutes(responseMinutes);
        return respondedAtUtc > CalculateDeadline(createdAtUtc, slaClass);
    }

    public DateTime CalculateDeadline(DateTime createdAtUtc, SlaClass slaClass)
    {
        return slaClass switch
        {
            SlaClass.P1 => createdAtUtc.AddHours(1),
            SlaClass.P2 => createdAtUtc.AddHours(4),
            SlaClass.P3 => createdAtUtc.AddHours(24),
            SlaClass.P4 => DateTime.MaxValue,
            _ => createdAtUtc.AddHours(24)
        };
    }

    public SlaStatus EvaluateStatus(WorkOrder workOrder)
    {
        if (workOrder.SlaClass == SlaClass.P4)
        {
            workOrder.ApplySlaStatus(SlaStatus.OnTime, DateTime.UtcNow);
            return SlaStatus.OnTime;
        }

        var nowUtc = DateTime.UtcNow;
        var deadlineUtc = CalculateDeadline(workOrder.CreatedAt, workOrder.SlaClass);

        var status = nowUtc > deadlineUtc
            ? SlaStatus.Breached
            : nowUtc >= deadlineUtc.AddMinutes(-30)
                ? SlaStatus.AtRisk
                : SlaStatus.OnTime;

        workOrder.ApplySlaStatus(status, nowUtc);
        return status;
    }
}
