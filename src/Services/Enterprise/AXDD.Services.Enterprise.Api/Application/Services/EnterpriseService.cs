using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.BuildingBlocks.Domain.Repositories;
using AXDD.Services.Enterprise.Api.Application.DTOs;
using AXDD.Services.Enterprise.Api.Application.Services.Interfaces;
using AXDD.Services.Enterprise.Api.Domain.Entities;
using AXDD.Services.Enterprise.Api.Domain.Enums;
using AXDD.Services.Enterprise.Api.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.Enterprise.Api.Application.Services;

/// <summary>
/// Service implementation for managing enterprises
/// </summary>
public class EnterpriseService : IEnterpriseService
{
    private readonly IEnterpriseRepository _enterpriseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEnterpriseHistoryService _historyService;

    public EnterpriseService(
        IEnterpriseRepository enterpriseRepository,
        IUnitOfWork unitOfWork,
        IEnterpriseHistoryService historyService)
    {
        ArgumentNullException.ThrowIfNull(enterpriseRepository);
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(historyService);

        _enterpriseRepository = enterpriseRepository;
        _unitOfWork = unitOfWork;
        _historyService = historyService;
    }

    /// <inheritdoc/>
    public async Task<Result<PagedResult<EnterpriseListDto>>> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm,
        EnterpriseStatus? status,
        Guid? zoneId,
        string? industryCode,
        string? sortBy,
        bool descending,
        CancellationToken ct)
    {
        try
        {
            var query = _enterpriseRepository.AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.ToLower();
                query = query.Where(e =>
                    e.Code.ToLower().Contains(search) ||
                    e.Name.ToLower().Contains(search) ||
                    e.TaxCode.ToLower().Contains(search));
            }

            if (status.HasValue)
            {
                query = query.Where(e => e.Status == status.Value);
            }

            if (zoneId.HasValue)
            {
                query = query.Where(e => e.IndustrialZoneId == zoneId.Value);
            }

            if (!string.IsNullOrWhiteSpace(industryCode))
            {
                query = query.Where(e => e.IndustryCode == industryCode);
            }

            // Apply sorting
            query = ApplySorting(query, sortBy, descending);

            // Get total count
            var totalCount = await query.CountAsync(ct);

            // Apply pagination
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            var result = new PagedResult<EnterpriseListDto>
            {
                Items = items.Select(MapToListDto),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Result<PagedResult<EnterpriseListDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<EnterpriseListDto>>.Failure($"Failed to retrieve enterprises: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<EnterpriseDto>> GetByIdAsync(Guid id, CancellationToken ct)
    {
        try
        {
            var enterprise = await _enterpriseRepository.GetByIdAsync(
                id,
                ct,
                e => e.Contacts,
                e => e.Licenses);

            if (enterprise == null)
                return Result<EnterpriseDto>.Failure("Enterprise not found");

            return Result<EnterpriseDto>.Success(MapToDto(enterprise));
        }
        catch (Exception ex)
        {
            return Result<EnterpriseDto>.Failure($"Failed to retrieve enterprise: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<EnterpriseDto>> GetByCodeAsync(string code, CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        try
        {
            var enterprise = await _enterpriseRepository.GetByCodeAsync(code, ct);

            if (enterprise == null)
                return Result<EnterpriseDto>.Failure($"Enterprise with code '{code}' not found");

            // Load related entities
            var contactsRepo = _unitOfWork.Repository<ContactPerson>();
            var licensesRepo = _unitOfWork.Repository<EnterpriseLicense>();

            var contacts = await contactsRepo
                .AsQueryable()
                .Where(c => c.EnterpriseId == enterprise.Id)
                .ToListAsync(ct);

            var licenses = await licensesRepo
                .AsQueryable()
                .Where(l => l.EnterpriseId == enterprise.Id)
                .ToListAsync(ct);

            enterprise.Contacts = contacts;
            enterprise.Licenses = licenses;

            return Result<EnterpriseDto>.Success(MapToDto(enterprise));
        }
        catch (Exception ex)
        {
            return Result<EnterpriseDto>.Failure($"Failed to retrieve enterprise: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<EnterpriseDto>> GetByTaxCodeAsync(string taxCode, CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(taxCode);

        try
        {
            var enterprise = await _enterpriseRepository.GetByTaxCodeAsync(taxCode, ct);

            if (enterprise == null)
                return Result<EnterpriseDto>.Failure($"Enterprise with tax code '{taxCode}' not found");

            // Load related entities
            var contactsRepo = _unitOfWork.Repository<ContactPerson>();
            var licensesRepo = _unitOfWork.Repository<EnterpriseLicense>();

            var contacts = await contactsRepo
                .AsQueryable()
                .Where(c => c.EnterpriseId == enterprise.Id)
                .ToListAsync(ct);

            var licenses = await licensesRepo
                .AsQueryable()
                .Where(l => l.EnterpriseId == enterprise.Id)
                .ToListAsync(ct);

            enterprise.Contacts = contacts;
            enterprise.Licenses = licenses;

            return Result<EnterpriseDto>.Success(MapToDto(enterprise));
        }
        catch (Exception ex)
        {
            return Result<EnterpriseDto>.Failure($"Failed to retrieve enterprise: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<EnterpriseDto>> CreateAsync(CreateEnterpriseRequest request, string userId, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        try
        {
            // Validate business rules
            var validationResult = await ValidateCreateRequestAsync(request, ct);
            if (validationResult.IsFailure)
                return Result<EnterpriseDto>.Failure(validationResult.Error!);

            var enterprise = new EnterpriseEntity
            {
                Code = request.Code,
                Name = request.Name,
                TaxCode = request.TaxCode,
                EnglishName = request.EnglishName,
                ShortName = request.ShortName,
                IndustryCode = request.IndustryCode,
                IndustryName = request.IndustryName,
                IndustrialZoneId = request.IndustrialZoneId,
                IndustrialZoneName = request.IndustrialZoneName,
                Status = request.Status,
                LegalRepresentative = request.LegalRepresentative,
                Position = request.Position,
                Address = request.Address,
                Ward = request.Ward,
                District = request.District,
                Province = request.Province,
                Phone = request.Phone,
                Fax = request.Fax,
                Email = request.Email,
                Website = request.Website,
                RegisteredDate = request.RegisteredDate,
                RegisteredCapital = request.RegisteredCapital,
                CharterCapital = request.CharterCapital,
                TotalEmployees = request.TotalEmployees,
                VietnamEmployees = request.VietnamEmployees,
                ForeignEmployees = request.ForeignEmployees,
                ProductionCapacity = request.ProductionCapacity,
                AnnualRevenue = request.AnnualRevenue,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            await _enterpriseRepository.AddAsync(enterprise, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            // Log the creation
            await _historyService.LogCreationAsync(
                enterprise.Id,
                userId,
                $"Enterprise created: {enterprise.Name}",
                ct);

            return Result<EnterpriseDto>.Success(MapToDto(enterprise));
        }
        catch (Exception ex)
        {
            return Result<EnterpriseDto>.Failure($"Failed to create enterprise: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<EnterpriseDto>> UpdateAsync(Guid id, UpdateEnterpriseRequest request, string userId, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        try
        {
            var enterprise = await _enterpriseRepository.GetByIdAsync(id, ct);

            if (enterprise == null)
                return Result<EnterpriseDto>.Failure("Enterprise not found");

            // Track changes for history
            var changes = TrackChanges(enterprise, request);

            // Update properties
            enterprise.Name = request.Name;
            enterprise.EnglishName = request.EnglishName;
            enterprise.ShortName = request.ShortName;
            enterprise.IndustryCode = request.IndustryCode;
            enterprise.IndustryName = request.IndustryName;
            enterprise.IndustrialZoneId = request.IndustrialZoneId;
            enterprise.IndustrialZoneName = request.IndustrialZoneName;
            enterprise.LegalRepresentative = request.LegalRepresentative;
            enterprise.Position = request.Position;
            enterprise.Address = request.Address;
            enterprise.Ward = request.Ward;
            enterprise.District = request.District;
            enterprise.Province = request.Province;
            enterprise.Phone = request.Phone;
            enterprise.Fax = request.Fax;
            enterprise.Email = request.Email;
            enterprise.Website = request.Website;
            enterprise.RegisteredDate = request.RegisteredDate;
            enterprise.RegisteredCapital = request.RegisteredCapital;
            enterprise.CharterCapital = request.CharterCapital;
            enterprise.TotalEmployees = request.TotalEmployees;
            enterprise.VietnamEmployees = request.VietnamEmployees;
            enterprise.ForeignEmployees = request.ForeignEmployees;
            enterprise.ProductionCapacity = request.ProductionCapacity;
            enterprise.AnnualRevenue = request.AnnualRevenue;
            enterprise.Description = request.Description;
            enterprise.UpdatedAt = DateTime.UtcNow;
            enterprise.UpdatedBy = userId;

            _enterpriseRepository.Update(enterprise);
            await _unitOfWork.SaveChangesAsync(ct);

            // Log the changes
            if (changes.Count > 0)
            {
                await _historyService.LogUpdateAsync(id, userId, changes, ct);
            }

            // Load related entities
            var contactsRepo = _unitOfWork.Repository<ContactPerson>();
            var licensesRepo = _unitOfWork.Repository<EnterpriseLicense>();

            var contacts = await contactsRepo
                .AsQueryable()
                .Where(c => c.EnterpriseId == enterprise.Id)
                .ToListAsync(ct);

            var licenses = await licensesRepo
                .AsQueryable()
                .Where(l => l.EnterpriseId == enterprise.Id)
                .ToListAsync(ct);

            enterprise.Contacts = contacts;
            enterprise.Licenses = licenses;

            return Result<EnterpriseDto>.Success(MapToDto(enterprise));
        }
        catch (Exception ex)
        {
            return Result<EnterpriseDto>.Failure($"Failed to update enterprise: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<bool>> DeleteAsync(Guid id, string userId, CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        try
        {
            var enterprise = await _enterpriseRepository.GetByIdAsync(id, ct);

            if (enterprise == null)
                return Result<bool>.Failure("Enterprise not found");

            // Soft delete
            _enterpriseRepository.Delete(enterprise);
            await _unitOfWork.SaveChangesAsync(ct);

            // Log the deletion
            await _historyService.LogDeletionAsync(id, userId, "Enterprise deleted", ct);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to delete enterprise: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<EnterpriseDto>> ChangeStatusAsync(
        Guid id,
        EnterpriseStatus newStatus,
        string? reason,
        string userId,
        CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        try
        {
            var enterprise = await _enterpriseRepository.GetByIdAsync(id, ct);

            if (enterprise == null)
                return Result<EnterpriseDto>.Failure("Enterprise not found");

            var oldStatus = enterprise.Status;

            // Validate status transition
            var validationResult = ValidateStatusTransition(oldStatus, newStatus);
            if (validationResult.IsFailure)
                return Result<EnterpriseDto>.Failure(validationResult.Error!);

            enterprise.Status = newStatus;
            enterprise.UpdatedAt = DateTime.UtcNow;
            enterprise.UpdatedBy = userId;

            _enterpriseRepository.Update(enterprise);
            await _unitOfWork.SaveChangesAsync(ct);

            // Log the status change
            await _historyService.LogStatusChangeAsync(id, userId, oldStatus, newStatus, reason, ct);

            // Load related entities
            var contactsRepo = _unitOfWork.Repository<ContactPerson>();
            var licensesRepo = _unitOfWork.Repository<EnterpriseLicense>();

            var contacts = await contactsRepo
                .AsQueryable()
                .Where(c => c.EnterpriseId == enterprise.Id)
                .ToListAsync(ct);

            var licenses = await licensesRepo
                .AsQueryable()
                .Where(l => l.EnterpriseId == enterprise.Id)
                .ToListAsync(ct);

            enterprise.Contacts = contacts;
            enterprise.Licenses = licenses;

            return Result<EnterpriseDto>.Success(MapToDto(enterprise));
        }
        catch (Exception ex)
        {
            return Result<EnterpriseDto>.Failure($"Failed to change enterprise status: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<EnterpriseStatisticsDto>> GetStatisticsAsync(CancellationToken ct)
    {
        try
        {
            var enterprises = await _enterpriseRepository.GetAllAsync(ct);

            var statistics = new EnterpriseStatisticsDto
            {
                TotalCount = enterprises.Count,
                ByStatus = enterprises
                    .GroupBy(e => e.Status.ToString())
                    .ToDictionary(g => g.Key, g => g.Count()),
                ByZone = enterprises
                    .Where(e => e.IndustrialZoneName != null)
                    .GroupBy(e => e.IndustrialZoneName!)
                    .ToDictionary(g => g.Key, g => g.Count()),
                ByIndustry = enterprises
                    .GroupBy(e => e.IndustryName)
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            return Result<EnterpriseStatisticsDto>.Success(statistics);
        }
        catch (Exception ex)
        {
            return Result<EnterpriseStatisticsDto>.Failure($"Failed to retrieve statistics: {ex.Message}");
        }
    }

    private async Task<Result> ValidateCreateRequestAsync(CreateEnterpriseRequest request, CancellationToken ct)
    {
        // Check for duplicate code
        if (await _enterpriseRepository.CodeExistsAsync(request.Code, null, ct))
        {
            return Result.Failure($"Enterprise code '{request.Code}' already exists");
        }

        // Check for duplicate tax code
        if (await _enterpriseRepository.TaxCodeExistsAsync(request.TaxCode, null, ct))
        {
            return Result.Failure($"Tax code '{request.TaxCode}' already exists");
        }

        return Result.Success();
    }

    private static Result ValidateStatusTransition(EnterpriseStatus oldStatus, EnterpriseStatus newStatus)
    {
        // Define valid status transitions
        var validTransitions = new Dictionary<EnterpriseStatus, EnterpriseStatus[]>
        {
            [EnterpriseStatus.UnderConstruction] = [EnterpriseStatus.Active, EnterpriseStatus.Closed],
            [EnterpriseStatus.Active] = [EnterpriseStatus.Suspended, EnterpriseStatus.Closed, EnterpriseStatus.Liquidated],
            [EnterpriseStatus.Suspended] = [EnterpriseStatus.Active, EnterpriseStatus.Closed, EnterpriseStatus.Liquidated],
            [EnterpriseStatus.Closed] = [EnterpriseStatus.Active, EnterpriseStatus.Liquidated],
            [EnterpriseStatus.Liquidated] = []
        };

        if (oldStatus == newStatus)
        {
            return Result.Failure("New status must be different from current status");
        }

        if (validTransitions.TryGetValue(oldStatus, out var allowedStatuses) &&
            !allowedStatuses.Contains(newStatus))
        {
            return Result.Failure($"Cannot change status from {oldStatus} to {newStatus}");
        }

        return Result.Success();
    }

    private static Dictionary<string, (string? OldValue, string? NewValue)> TrackChanges(
        EnterpriseEntity current,
        UpdateEnterpriseRequest updated)
    {
        var changes = new Dictionary<string, (string?, string?)>();

        if (current.Name != updated.Name)
            changes["Name"] = (current.Name, updated.Name);

        if (current.EnglishName != updated.EnglishName)
            changes["EnglishName"] = (current.EnglishName, updated.EnglishName);

        if (current.ShortName != updated.ShortName)
            changes["ShortName"] = (current.ShortName, updated.ShortName);

        if (current.IndustryCode != updated.IndustryCode)
            changes["IndustryCode"] = (current.IndustryCode, updated.IndustryCode);

        if (current.Address != updated.Address)
            changes["Address"] = (current.Address, updated.Address);

        if (current.Phone != updated.Phone)
            changes["Phone"] = (current.Phone, updated.Phone);

        if (current.Email != updated.Email)
            changes["Email"] = (current.Email, updated.Email);

        return changes;
    }

    private static IQueryable<EnterpriseEntity> ApplySorting(
        IQueryable<EnterpriseEntity> query,
        string? sortBy,
        bool descending)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return descending
                ? query.OrderByDescending(e => e.CreatedAt)
                : query.OrderBy(e => e.CreatedAt);
        }

        return sortBy.ToLower() switch
        {
            "code" => descending ? query.OrderByDescending(e => e.Code) : query.OrderBy(e => e.Code),
            "name" => descending ? query.OrderByDescending(e => e.Name) : query.OrderBy(e => e.Name),
            "taxcode" => descending ? query.OrderByDescending(e => e.TaxCode) : query.OrderBy(e => e.TaxCode),
            "status" => descending ? query.OrderByDescending(e => e.Status) : query.OrderBy(e => e.Status),
            "createdat" => descending ? query.OrderByDescending(e => e.CreatedAt) : query.OrderBy(e => e.CreatedAt),
            _ => descending ? query.OrderByDescending(e => e.CreatedAt) : query.OrderBy(e => e.CreatedAt)
        };
    }

    private static EnterpriseDto MapToDto(EnterpriseEntity entity)
    {
        return new EnterpriseDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            TaxCode = entity.TaxCode,
            EnglishName = entity.EnglishName,
            ShortName = entity.ShortName,
            IndustryCode = entity.IndustryCode,
            IndustryName = entity.IndustryName,
            IndustrialZoneId = entity.IndustrialZoneId,
            IndustrialZoneName = entity.IndustrialZoneName,
            Status = entity.Status,
            LegalRepresentative = entity.LegalRepresentative,
            Position = entity.Position,
            Address = entity.Address,
            Ward = entity.Ward,
            District = entity.District,
            Province = entity.Province,
            Phone = entity.Phone,
            Fax = entity.Fax,
            Email = entity.Email,
            Website = entity.Website,
            RegisteredDate = entity.RegisteredDate,
            RegisteredCapital = entity.RegisteredCapital,
            CharterCapital = entity.CharterCapital,
            TotalEmployees = entity.TotalEmployees,
            VietnamEmployees = entity.VietnamEmployees,
            ForeignEmployees = entity.ForeignEmployees,
            ProductionCapacity = entity.ProductionCapacity,
            AnnualRevenue = entity.AnnualRevenue,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            CreatedBy = entity.CreatedBy,
            UpdatedAt = entity.UpdatedAt,
            UpdatedBy = entity.UpdatedBy,
            Contacts = entity.Contacts.Select(c => new ContactPersonDto
            {
                Id = c.Id,
                EnterpriseId = c.EnterpriseId,
                FullName = c.FullName,
                Position = c.Position,
                Department = c.Department,
                Phone = c.Phone,
                Email = c.Email,
                IsMain = c.IsMain,
                Notes = c.Notes,
                CreatedAt = c.CreatedAt,
                CreatedBy = c.CreatedBy
            }).ToList(),
            Licenses = entity.Licenses.Select(l => new EnterpriseLicenseDto
            {
                Id = l.Id,
                EnterpriseId = l.EnterpriseId,
                LicenseType = l.LicenseType,
                LicenseNumber = l.LicenseNumber,
                IssuedDate = l.IssuedDate,
                ExpiryDate = l.ExpiryDate,
                IssuingAuthority = l.IssuingAuthority,
                Status = l.Status,
                FileId = l.FileId,
                Notes = l.Notes,
                CreatedAt = l.CreatedAt,
                CreatedBy = l.CreatedBy
            }).ToList()
        };
    }

    private static EnterpriseListDto MapToListDto(EnterpriseEntity entity)
    {
        return new EnterpriseListDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            TaxCode = entity.TaxCode,
            ShortName = entity.ShortName,
            IndustryCode = entity.IndustryCode,
            IndustryName = entity.IndustryName,
            IndustrialZoneId = entity.IndustrialZoneId,
            IndustrialZoneName = entity.IndustrialZoneName,
            Status = entity.Status,
            Address = entity.Address,
            District = entity.District,
            Province = entity.Province,
            Phone = entity.Phone,
            Email = entity.Email,
            TotalEmployees = entity.TotalEmployees,
            RegisteredCapital = entity.RegisteredCapital,
            CreatedAt = entity.CreatedAt
        };
    }
}
