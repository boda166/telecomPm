using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.Commands.AuditLogs.LogAuditEntry;
using TelecomPM.Application.DTOs.WorkOrders;
using TelecomPM.Domain.Entities.WorkOrders;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Commands.WorkOrders.CreateWorkOrder;

public class CreateWorkOrderCommandHandler : IRequestHandler<CreateWorkOrderCommand, Result<WorkOrderDto>>
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ISender _sender;

    public CreateWorkOrderCommandHandler(
        IWorkOrderRepository workOrderRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ISender sender)
    {
        _workOrderRepository = workOrderRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _sender = sender;
    }

    public async Task<Result<WorkOrderDto>> Handle(CreateWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var existing = await _workOrderRepository.GetByWoNumberAsync(request.WoNumber, cancellationToken);
        if (existing != null)
            return Result.Failure<WorkOrderDto>($"Work order with number {request.WoNumber} already exists");

        var workOrder = WorkOrder.Create(
            request.WoNumber,
            request.SiteCode,
            request.OfficeCode,
            request.SlaClass,
            request.IssueDescription);

        await _workOrderRepository.AddAsync(workOrder, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _sender.Send(new LogAuditEntryCommand
        {
            EntityType = "WorkOrder",
            EntityId = workOrder.Id,
            Action = "Created",
            ActorId = Guid.Empty,
            ActorRole = "System",
            NewState = workOrder.Status.ToString(),
            Notes = request.IssueDescription
        }, cancellationToken);

        return Result.Success(_mapper.Map<WorkOrderDto>(workOrder));
    }
}
