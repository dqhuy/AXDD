using AXDD.BuildingBlocks.Domain.Repositories;
using AXDD.Services.Enterprise.Api.Domain.Entities;
using AXDD.Services.Enterprise.Api.Domain.Enums;

namespace AXDD.Services.Enterprise.Api.Domain.Repositories;

/// <summary>
/// Repository interface for enterprise-specific queries
/// </summary>
public interface IEnterpriseRepository : IRepository<EnterpriseEntity>
{
    /// <summary>
    /// Gets an enterprise by its code
    /// </summary>
    Task<EnterpriseEntity?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an enterprise by its tax code
    /// </summary>
    Task<EnterpriseEntity?> GetByTaxCodeAsync(string taxCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets enterprises by industrial zone
    /// </summary>
    Task<IReadOnlyList<EnterpriseEntity>> GetByIndustrialZoneAsync(Guid zoneId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets enterprises by industry code
    /// </summary>
    Task<IReadOnlyList<EnterpriseEntity>> GetByIndustryCodeAsync(string industryCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets enterprises by status
    /// </summary>
    Task<IReadOnlyList<EnterpriseEntity>> GetByStatusAsync(EnterpriseStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a code exists
    /// </summary>
    Task<bool> CodeExistsAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a tax code exists
    /// </summary>
    Task<bool> TaxCodeExistsAsync(string taxCode, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
