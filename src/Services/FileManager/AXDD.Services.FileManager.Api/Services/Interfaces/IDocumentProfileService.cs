using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.DTOs;

namespace AXDD.Services.FileManager.Api.Services.Interfaces;

/// <summary>
/// Service interface for document profile operations
/// </summary>
public interface IDocumentProfileService
{
    /// <summary>
    /// Creates a new document profile
    /// </summary>
    /// <param name="request">The create profile request</param>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the created profile</returns>
    Task<Result<DocumentProfileDto>> CreateProfileAsync(
        CreateDocumentProfileRequest request,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a profile by ID
    /// </summary>
    /// <param name="profileId">The profile ID</param>
    /// <param name="includeMetadataFields">Whether to include metadata fields</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the profile</returns>
    Task<Result<DocumentProfileDto>> GetProfileAsync(
        Guid profileId,
        bool includeMetadataFields = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a profile
    /// </summary>
    /// <param name="profileId">The profile ID</param>
    /// <param name="request">The update request</param>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the updated profile</returns>
    Task<Result<DocumentProfileDto>> UpdateProfileAsync(
        Guid profileId,
        UpdateDocumentProfileRequest request,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a profile (soft delete)
    /// </summary>
    /// <param name="profileId">The profile ID</param>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result of the operation</returns>
    Task<Result> DeleteProfileAsync(
        Guid profileId,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists profiles with pagination
    /// </summary>
    /// <param name="query">The list query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing paginated profile list</returns>
    Task<Result<PagedResult<DocumentProfileDto>>> ListProfilesAsync(
        DocumentProfileListQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the profile hierarchy tree
    /// </summary>
    /// <param name="enterpriseCode">The enterprise code</param>
    /// <param name="rootProfileId">Optional root profile ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the profile hierarchy</returns>
    Task<Result<List<DocumentProfileDto>>> GetProfileHierarchyAsync(
        string enterpriseCode,
        Guid? rootProfileId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a profile from a template
    /// </summary>
    /// <param name="templateId">The template profile ID</param>
    /// <param name="enterpriseCode">The enterprise code</param>
    /// <param name="name">The new profile name</param>
    /// <param name="code">The new profile code</param>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the created profile</returns>
    Task<Result<DocumentProfileDto>> CreateFromTemplateAsync(
        Guid templateId,
        string enterpriseCode,
        string name,
        string code,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Opens a profile (sets status to Active and OpenedAt date)
    /// </summary>
    /// <param name="profileId">The profile ID</param>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result of the operation</returns>
    Task<Result> OpenProfileAsync(
        Guid profileId,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Closes a profile (sets status to Closed and ClosedAt date)
    /// </summary>
    /// <param name="profileId">The profile ID</param>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result of the operation</returns>
    Task<Result> CloseProfileAsync(
        Guid profileId,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Archives a profile
    /// </summary>
    /// <param name="profileId">The profile ID</param>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result of the operation</returns>
    Task<Result> ArchiveProfileAsync(
        Guid profileId,
        string userId,
        CancellationToken cancellationToken = default);
}
