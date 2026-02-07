using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.BuildingBlocks.Domain.Repositories;
using AXDD.Services.Enterprise.Api.Application.DTOs;
using AXDD.Services.Enterprise.Api.Application.Services.Interfaces;
using AXDD.Services.Enterprise.Api.Domain.Entities;
using AXDD.Services.Enterprise.Api.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.Enterprise.Api.Application.Services;

/// <summary>
/// Service implementation for managing enterprise history
/// </summary>
public class EnterpriseHistoryService : IEnterpriseHistoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public EnterpriseHistoryService(IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc/>
    public async Task<Result<IReadOnlyList<EnterpriseHistoryDto>>> GetHistoryAsync(Guid enterpriseId, CancellationToken ct)
    {
        try
        {
            var repository = _unitOfWork.Repository<EnterpriseHistory>();
            var historyRecords = await repository
                .AsQueryable()
                .Where(h => h.EnterpriseId == enterpriseId)
                .OrderByDescending(h => h.ChangedAt)
                .ToListAsync(ct);

            var dtos = historyRecords.Select(MapToDto).ToList();
            return Result<IReadOnlyList<EnterpriseHistoryDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyList<EnterpriseHistoryDto>>.Failure($"Failed to retrieve enterprise history: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<PagedResult<EnterpriseHistoryDto>>> GetHistoryAsync(Guid enterpriseId, int pageNumber, int pageSize, CancellationToken ct)
    {
        try
        {
            var repository = _unitOfWork.Repository<EnterpriseHistory>();
            var query = repository
                .AsQueryable()
                .Where(h => h.EnterpriseId == enterpriseId)
                .OrderByDescending(h => h.ChangedAt);

            var totalCount = await query.CountAsync(ct);

            var historyRecords = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            var dtos = historyRecords.Select(MapToDto).ToList();
            
            var result = new PagedResult<EnterpriseHistoryDto>
            {
                Items = dtos,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Result<PagedResult<EnterpriseHistoryDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<EnterpriseHistoryDto>>.Failure($"Failed to retrieve enterprise history: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task LogCreationAsync(Guid enterpriseId, string userId, string? details, CancellationToken ct)
    {
        var history = new EnterpriseHistory
        {
            EnterpriseId = enterpriseId,
            ChangedAt = DateTime.UtcNow,
            ChangedBy = userId,
            ChangeType = ChangeType.Created,
            Details = details ?? "Enterprise created"
        };

        var repository = _unitOfWork.Repository<EnterpriseHistory>();
        await repository.AddAsync(history, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    /// <inheritdoc/>
    public async Task LogUpdateAsync(
        Guid enterpriseId,
        string userId,
        Dictionary<string, (string? OldValue, string? NewValue)> changes,
        CancellationToken ct)
    {
        if (changes == null || changes.Count == 0)
            return;

        var repository = _unitOfWork.Repository<EnterpriseHistory>();
        var historyRecords = new List<EnterpriseHistory>();

        foreach (var (fieldName, values) in changes)
        {
            var history = new EnterpriseHistory
            {
                EnterpriseId = enterpriseId,
                ChangedAt = DateTime.UtcNow,
                ChangedBy = userId,
                ChangeType = ChangeType.Updated,
                FieldName = fieldName,
                OldValue = values.OldValue,
                NewValue = values.NewValue,
                Details = $"Field '{fieldName}' updated"
            };
            historyRecords.Add(history);
        }

        await repository.AddRangeAsync(historyRecords, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    /// <inheritdoc/>
    public async Task LogStatusChangeAsync(
        Guid enterpriseId,
        string userId,
        EnterpriseStatus oldStatus,
        EnterpriseStatus newStatus,
        string? reason,
        CancellationToken ct)
    {
        var history = new EnterpriseHistory
        {
            EnterpriseId = enterpriseId,
            ChangedAt = DateTime.UtcNow,
            ChangedBy = userId,
            ChangeType = ChangeType.StatusChanged,
            FieldName = "Status",
            OldValue = oldStatus.ToString(),
            NewValue = newStatus.ToString(),
            Reason = reason,
            Details = $"Status changed from {oldStatus} to {newStatus}"
        };

        var repository = _unitOfWork.Repository<EnterpriseHistory>();
        await repository.AddAsync(history, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    /// <inheritdoc/>
    public async Task LogDeletionAsync(Guid enterpriseId, string userId, string? reason, CancellationToken ct)
    {
        var history = new EnterpriseHistory
        {
            EnterpriseId = enterpriseId,
            ChangedAt = DateTime.UtcNow,
            ChangedBy = userId,
            ChangeType = ChangeType.Deleted,
            Reason = reason,
            Details = "Enterprise deleted"
        };

        var repository = _unitOfWork.Repository<EnterpriseHistory>();
        await repository.AddAsync(history, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    /// <inheritdoc/>
    public async Task LogContactChangeAsync(
        Guid enterpriseId,
        string userId,
        ChangeType changeType,
        string contactName,
        string? details,
        CancellationToken ct)
    {
        var history = new EnterpriseHistory
        {
            EnterpriseId = enterpriseId,
            ChangedAt = DateTime.UtcNow,
            ChangedBy = userId,
            ChangeType = changeType,
            FieldName = "Contact Person",
            NewValue = contactName,
            Details = details ?? $"Contact person {changeType}"
        };

        var repository = _unitOfWork.Repository<EnterpriseHistory>();
        await repository.AddAsync(history, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    /// <inheritdoc/>
    public async Task LogLicenseChangeAsync(
        Guid enterpriseId,
        string userId,
        ChangeType changeType,
        string licenseNumber,
        string? details,
        CancellationToken ct)
    {
        var history = new EnterpriseHistory
        {
            EnterpriseId = enterpriseId,
            ChangedAt = DateTime.UtcNow,
            ChangedBy = userId,
            ChangeType = changeType,
            FieldName = "License",
            NewValue = licenseNumber,
            Details = details ?? $"License {changeType}"
        };

        var repository = _unitOfWork.Repository<EnterpriseHistory>();
        await repository.AddAsync(history, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    private static EnterpriseHistoryDto MapToDto(EnterpriseHistory entity)
    {
        return new EnterpriseHistoryDto
        {
            Id = entity.Id,
            EnterpriseId = entity.EnterpriseId,
            ChangedAt = entity.ChangedAt,
            ChangedBy = entity.ChangedBy,
            ChangeType = entity.ChangeType,
            FieldName = entity.FieldName,
            OldValue = entity.OldValue,
            NewValue = entity.NewValue,
            Reason = entity.Reason,
            Details = entity.Details
        };
    }
}
