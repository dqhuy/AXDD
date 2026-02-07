using AXDD.BuildingBlocks.Common.Results;
using AXDD.BuildingBlocks.Domain.Repositories;
using AXDD.Services.Enterprise.Api.Application.DTOs;
using AXDD.Services.Enterprise.Api.Application.Services.Interfaces;
using AXDD.Services.Enterprise.Api.Domain.Entities;
using AXDD.Services.Enterprise.Api.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.Enterprise.Api.Application.Services;

/// <summary>
/// Service implementation for managing enterprise licenses
/// </summary>
public class EnterpriseLicenseService : IEnterpriseLicenseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEnterpriseHistoryService _historyService;

    public EnterpriseLicenseService(IUnitOfWork unitOfWork, IEnterpriseHistoryService historyService)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(historyService);
        _unitOfWork = unitOfWork;
        _historyService = historyService;
    }

    /// <inheritdoc/>
    public async Task<Result<IReadOnlyList<EnterpriseLicenseDto>>> GetByEnterpriseIdAsync(Guid enterpriseId, CancellationToken ct)
    {
        try
        {
            var repository = _unitOfWork.Repository<EnterpriseLicense>();
            var licenses = await repository
                .AsQueryable()
                .Where(l => l.EnterpriseId == enterpriseId)
                .OrderBy(l => l.LicenseType)
                .ThenByDescending(l => l.IssuedDate)
                .ToListAsync(ct);

            var dtos = licenses.Select(MapToDto).ToList();
            return Result<IReadOnlyList<EnterpriseLicenseDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyList<EnterpriseLicenseDto>>.Failure($"Failed to retrieve licenses: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<EnterpriseLicenseDto>> GetByIdAsync(Guid id, CancellationToken ct)
    {
        try
        {
            var repository = _unitOfWork.Repository<EnterpriseLicense>();
            var license = await repository.GetByIdAsync(id, ct);

            if (license == null)
                return Result<EnterpriseLicenseDto>.Failure("License not found");

            return Result<EnterpriseLicenseDto>.Success(MapToDto(license));
        }
        catch (Exception ex)
        {
            return Result<EnterpriseLicenseDto>.Failure($"Failed to retrieve license: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<EnterpriseLicenseDto>> CreateAsync(CreateLicenseRequest request, string userId, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        try
        {
            // Validate enterprise exists
            var enterpriseRepo = _unitOfWork.Repository<EnterpriseEntity>();
            var enterpriseExists = await enterpriseRepo.AnyAsync(e => e.Id == request.EnterpriseId, ct);
            if (!enterpriseExists)
                return Result<EnterpriseLicenseDto>.Failure("Enterprise not found");

            // Check for duplicate license number
            var licenseRepo = _unitOfWork.Repository<EnterpriseLicense>();
            var duplicateExists = await licenseRepo.AnyAsync(
                l => l.EnterpriseId == request.EnterpriseId && l.LicenseNumber == request.LicenseNumber,
                ct);

            if (duplicateExists)
                return Result<EnterpriseLicenseDto>.Failure($"License number '{request.LicenseNumber}' already exists for this enterprise");

            var license = new EnterpriseLicense
            {
                EnterpriseId = request.EnterpriseId,
                LicenseType = request.LicenseType,
                LicenseNumber = request.LicenseNumber,
                IssuedDate = request.IssuedDate,
                ExpiryDate = request.ExpiryDate,
                IssuingAuthority = request.IssuingAuthority,
                Status = request.Status,
                FileId = request.FileId,
                Notes = request.Notes
            };

            await licenseRepo.AddAsync(license, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            // Log the change
            await _historyService.LogLicenseChangeAsync(
                request.EnterpriseId,
                userId,
                ChangeType.LicenseAdded,
                request.LicenseNumber,
                $"License added: {request.LicenseType} - {request.LicenseNumber}",
                ct);

            return Result<EnterpriseLicenseDto>.Success(MapToDto(license));
        }
        catch (Exception ex)
        {
            return Result<EnterpriseLicenseDto>.Failure($"Failed to create license: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<EnterpriseLicenseDto>> UpdateAsync(Guid id, UpdateLicenseRequest request, string userId, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        try
        {
            var repository = _unitOfWork.Repository<EnterpriseLicense>();
            var license = await repository.GetByIdAsync(id, ct);

            if (license == null)
                return Result<EnterpriseLicenseDto>.Failure("License not found");

            // Check for duplicate license number (excluding current)
            var duplicateExists = await repository.AnyAsync(
                l => l.EnterpriseId == license.EnterpriseId &&
                     l.LicenseNumber == request.LicenseNumber &&
                     l.Id != id,
                ct);

            if (duplicateExists)
                return Result<EnterpriseLicenseDto>.Failure($"License number '{request.LicenseNumber}' already exists for this enterprise");

            license.LicenseType = request.LicenseType;
            license.LicenseNumber = request.LicenseNumber;
            license.IssuedDate = request.IssuedDate;
            license.ExpiryDate = request.ExpiryDate;
            license.IssuingAuthority = request.IssuingAuthority;
            license.Status = request.Status;
            license.FileId = request.FileId;
            license.Notes = request.Notes;

            repository.Update(license);
            await _unitOfWork.SaveChangesAsync(ct);

            // Log the change
            await _historyService.LogLicenseChangeAsync(
                license.EnterpriseId,
                userId,
                ChangeType.LicenseUpdated,
                license.LicenseNumber,
                $"License updated: {license.LicenseType} - {license.LicenseNumber}",
                ct);

            return Result<EnterpriseLicenseDto>.Success(MapToDto(license));
        }
        catch (Exception ex)
        {
            return Result<EnterpriseLicenseDto>.Failure($"Failed to update license: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<bool>> DeleteAsync(Guid id, string userId, CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        try
        {
            var repository = _unitOfWork.Repository<EnterpriseLicense>();
            var license = await repository.GetByIdAsync(id, ct);

            if (license == null)
                return Result<bool>.Failure("License not found");

            var enterpriseId = license.EnterpriseId;
            var licenseNumber = license.LicenseNumber;
            var licenseType = license.LicenseType;

            repository.Delete(license);
            await _unitOfWork.SaveChangesAsync(ct);

            // Log the change
            await _historyService.LogLicenseChangeAsync(
                enterpriseId,
                userId,
                ChangeType.LicenseRemoved,
                licenseNumber,
                $"License removed: {licenseType} - {licenseNumber}",
                ct);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to delete license: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<IReadOnlyList<EnterpriseLicenseDto>>> GetExpiringSoonAsync(int days, CancellationToken ct)
    {
        try
        {
            var repository = _unitOfWork.Repository<EnterpriseLicense>();
            var targetDate = DateTime.UtcNow.AddDays(days);

            var licenses = await repository
                .AsQueryable()
                .Where(l => l.ExpiryDate.HasValue &&
                           l.ExpiryDate.Value <= targetDate &&
                           l.ExpiryDate.Value >= DateTime.UtcNow &&
                           l.Status == "Active")
                .OrderBy(l => l.ExpiryDate)
                .ToListAsync(ct);

            var dtos = licenses.Select(MapToDto).ToList();
            return Result<IReadOnlyList<EnterpriseLicenseDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyList<EnterpriseLicenseDto>>.Failure($"Failed to retrieve expiring licenses: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public Task<Result<IReadOnlyList<EnterpriseLicenseDto>>> GetByEnterpriseAsync(Guid enterpriseId, CancellationToken ct)
    {
        return GetByEnterpriseIdAsync(enterpriseId, ct);
    }

    /// <inheritdoc/>
    public Task<Result<IReadOnlyList<EnterpriseLicenseDto>>> GetExpiringLicensesAsync(int days, CancellationToken ct)
    {
        return GetExpiringSoonAsync(days, ct);
    }

    private static EnterpriseLicenseDto MapToDto(EnterpriseLicense entity)
    {
        return new EnterpriseLicenseDto
        {
            Id = entity.Id,
            EnterpriseId = entity.EnterpriseId,
            LicenseType = entity.LicenseType,
            LicenseNumber = entity.LicenseNumber,
            IssuedDate = entity.IssuedDate,
            ExpiryDate = entity.ExpiryDate,
            IssuingAuthority = entity.IssuingAuthority,
            Status = entity.Status,
            FileId = entity.FileId,
            Notes = entity.Notes,
            CreatedAt = entity.CreatedAt,
            CreatedBy = entity.CreatedBy
        };
    }
}
