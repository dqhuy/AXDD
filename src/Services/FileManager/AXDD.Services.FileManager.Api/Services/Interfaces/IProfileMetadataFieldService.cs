using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.DTOs;

namespace AXDD.Services.FileManager.Api.Services.Interfaces;

/// <summary>
/// Service interface for profile metadata field operations
/// </summary>
public interface IProfileMetadataFieldService
{
    /// <summary>
    /// Creates a new metadata field for a profile
    /// </summary>
    /// <param name="request">The create field request</param>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the created field</returns>
    Task<Result<ProfileMetadataFieldDto>> CreateFieldAsync(
        CreateProfileMetadataFieldRequest request,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a metadata field by ID
    /// </summary>
    /// <param name="fieldId">The field ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the field</returns>
    Task<Result<ProfileMetadataFieldDto>> GetFieldAsync(
        Guid fieldId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a metadata field
    /// </summary>
    /// <param name="fieldId">The field ID</param>
    /// <param name="request">The update request</param>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the updated field</returns>
    Task<Result<ProfileMetadataFieldDto>> UpdateFieldAsync(
        Guid fieldId,
        UpdateProfileMetadataFieldRequest request,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a metadata field (soft delete)
    /// </summary>
    /// <param name="fieldId">The field ID</param>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result of the operation</returns>
    Task<Result> DeleteFieldAsync(
        Guid fieldId,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists metadata fields for a profile
    /// </summary>
    /// <param name="profileId">The profile ID</param>
    /// <param name="includeDisabled">Whether to include disabled fields</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the field list</returns>
    Task<Result<List<ProfileMetadataFieldDto>>> ListFieldsAsync(
        Guid profileId,
        bool includeDisabled = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Reorders metadata fields
    /// </summary>
    /// <param name="profileId">The profile ID</param>
    /// <param name="fieldOrders">Dictionary of field ID to display order</param>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result of the operation</returns>
    Task<Result> ReorderFieldsAsync(
        Guid profileId,
        Dictionary<Guid, int> fieldOrders,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Copies metadata fields from one profile to another
    /// </summary>
    /// <param name="sourceProfileId">The source profile ID</param>
    /// <param name="targetProfileId">The target profile ID</param>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the copied fields</returns>
    Task<Result<List<ProfileMetadataFieldDto>>> CopyFieldsAsync(
        Guid sourceProfileId,
        Guid targetProfileId,
        string userId,
        CancellationToken cancellationToken = default);
}
