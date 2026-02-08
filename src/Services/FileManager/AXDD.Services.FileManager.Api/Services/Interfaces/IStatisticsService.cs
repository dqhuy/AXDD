using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.DTOs;

namespace AXDD.Services.FileManager.Api.Services.Interfaces;

/// <summary>
/// Service interface for document management statistics
/// </summary>
public interface IStatisticsService
{
    /// <summary>
    /// Gets document statistics
    /// </summary>
    Task<Result<DocumentStatisticsDto>> GetDocumentStatisticsAsync(string? enterpriseCode = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets storage statistics
    /// </summary>
    Task<Result<StorageStatisticsDto>> GetStorageStatisticsAsync(string? enterpriseCode = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets loan statistics
    /// </summary>
    Task<Result<LoanStatisticsDto>> GetLoanStatisticsAsync(string? enterpriseCode = null, CancellationToken cancellationToken = default);
}
