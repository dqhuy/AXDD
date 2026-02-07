using AXDD.BuildingBlocks.Common.Results;
using AXDD.BuildingBlocks.Domain.Repositories;
using AXDD.Services.FileManager.Api.Data;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Entities;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using AXDD.Services.FileManager.Api.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AXDD.Services.FileManager.Api.Services.Implementations;

/// <summary>
/// Service implementation for storage quota operations
/// </summary>
public class StorageQuotaService : IStorageQuotaService
{
    private readonly FileManagerDbContext _context;
    private readonly StorageQuotaSettings _settings;
    private readonly ILogger<StorageQuotaService> _logger;

    /// <summary>
    /// Initializes a new instance of StorageQuotaService
    /// </summary>
    public StorageQuotaService(
        FileManagerDbContext context,
        IOptions<StorageQuotaSettings> settings,
        ILogger<StorageQuotaService> logger)
    {
        _context = context;
        _settings = settings.Value;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<StorageQuotaDto>> GetQuotaAsync(
        string enterpriseCode,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(enterpriseCode, nameof(enterpriseCode));

        try
        {
            var quota = await _context.StorageQuotas
                .Where(q => q.EnterpriseCode == enterpriseCode && !q.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (quota == null)
            {
                // Initialize quota if it doesn't exist
                var initResult = await InitializeQuotaAsync(enterpriseCode, null, cancellationToken);
                return initResult;
            }

            var dto = MapToDto(quota);
            return Result<StorageQuotaDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting storage quota for enterprise: {EnterpriseCode}", enterpriseCode);
            return Result<StorageQuotaDto>.Failure($"Failed to get storage quota: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result> UpdateUsageAsync(
        string enterpriseCode,
        long sizeChange,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(enterpriseCode, nameof(enterpriseCode));

        try
        {
            var quota = await _context.StorageQuotas
                .Where(q => q.EnterpriseCode == enterpriseCode && !q.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (quota == null)
            {
                // Initialize quota if it doesn't exist
                await InitializeQuotaAsync(enterpriseCode, null, cancellationToken);
                quota = await _context.StorageQuotas
                    .Where(q => q.EnterpriseCode == enterpriseCode && !q.IsDeleted)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            if (quota == null)
            {
                return Result.Failure("Failed to initialize storage quota");
            }

            quota.UsedBytes += sizeChange;
            quota.UpdatedAt = DateTime.UtcNow;

            _context.StorageQuotas.Update(quota);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Storage usage updated for enterprise {EnterpriseCode}: Change={SizeChange}, NewUsage={UsedBytes}",
                enterpriseCode, sizeChange, quota.UsedBytes);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating storage usage for enterprise: {EnterpriseCode}", enterpriseCode);
            return Result.Failure($"Failed to update storage usage: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<bool>> CheckQuotaAsync(
        string enterpriseCode,
        long requiredSize,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(enterpriseCode, nameof(enterpriseCode));

        try
        {
            var quotaResult = await GetQuotaAsync(enterpriseCode, cancellationToken);

            if (quotaResult.IsFailure)
            {
                return Result<bool>.Failure(quotaResult.Error ?? "Failed to check quota");
            }

            var quota = quotaResult.Value!;
            var hasQuota = quota.AvailableBytes >= requiredSize;

            _logger.LogInformation(
                "Quota check for enterprise {EnterpriseCode}: Required={RequiredSize}, Available={AvailableBytes}, HasQuota={HasQuota}",
                enterpriseCode, requiredSize, quota.AvailableBytes, hasQuota);

            return Result<bool>.Success(hasQuota);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking storage quota for enterprise: {EnterpriseCode}", enterpriseCode);
            return Result<bool>.Failure($"Failed to check storage quota: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<StorageQuotaDto>> InitializeQuotaAsync(
        string enterpriseCode,
        long? quotaBytes = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(enterpriseCode, nameof(enterpriseCode));

        try
        {
            // Check if quota already exists
            var existingQuota = await _context.StorageQuotas
                .Where(q => q.EnterpriseCode == enterpriseCode && !q.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingQuota != null)
            {
                var dto = MapToDto(existingQuota);
                return Result<StorageQuotaDto>.Success(dto);
            }

            var quota = new StorageQuota
            {
                EnterpriseCode = enterpriseCode,
                QuotaBytes = quotaBytes ?? _settings.DefaultQuotaBytes,
                UsedBytes = 0,
                WarningThresholdPercentage = _settings.WarningThresholdPercentage,
                CreatedAt = DateTime.UtcNow
            };

            _context.StorageQuotas.Add(quota);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Storage quota initialized for enterprise {EnterpriseCode}: Quota={QuotaBytes} bytes",
                enterpriseCode, quota.QuotaBytes);

            var resultDto = MapToDto(quota);
            return Result<StorageQuotaDto>.Success(resultDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing storage quota for enterprise: {EnterpriseCode}", enterpriseCode);
            return Result<StorageQuotaDto>.Failure($"Failed to initialize storage quota: {ex.Message}");
        }
    }

    private static StorageQuotaDto MapToDto(StorageQuota quota)
    {
        return new StorageQuotaDto
        {
            EnterpriseCode = quota.EnterpriseCode,
            QuotaBytes = quota.QuotaBytes,
            UsedBytes = quota.UsedBytes,
            AvailableBytes = quota.AvailableBytes,
            UsagePercentage = quota.UsagePercentage,
            IsExceeded = quota.IsExceeded,
            IsWarningThresholdReached = quota.IsWarningThresholdReached
        };
    }
}
