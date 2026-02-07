using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.Data;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Entities;
using AXDD.Services.FileManager.Api.Exceptions;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.FileManager.Api.Services.Implementations;

/// <summary>
/// Service implementation for folder operations
/// </summary>
public class FolderService : IFolderService
{
    private readonly FileManagerDbContext _context;
    private readonly ILogger<FolderService> _logger;

    /// <summary>
    /// Initializes a new instance of FolderService
    /// </summary>
    public FolderService(
        FileManagerDbContext context,
        ILogger<FolderService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<FolderDto>> CreateFolderAsync(
        string name,
        string enterpriseCode,
        Guid? parentFolderId,
        string userId,
        string? description = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        ArgumentException.ThrowIfNullOrWhiteSpace(enterpriseCode, nameof(enterpriseCode));
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        try
        {
            // Validate parent folder if specified
            Folder? parentFolder = null;
            if (parentFolderId.HasValue)
            {
                parentFolder = await _context.Folders
                    .Where(f => f.Id == parentFolderId.Value && !f.IsDeleted)
                    .FirstOrDefaultAsync(cancellationToken);

                if (parentFolder == null)
                {
                    return Result<FolderDto>.Failure($"Parent folder with ID '{parentFolderId}' not found");
                }

                // Ensure parent folder belongs to the same enterprise
                if (parentFolder.EnterpriseCode != enterpriseCode)
                {
                    return Result<FolderDto>.Failure("Parent folder does not belong to the specified enterprise");
                }
            }

            // Check if folder with same name exists in parent
            var existingFolder = await _context.Folders
                .Where(f => f.Name == name
                    && f.EnterpriseCode == enterpriseCode
                    && f.ParentFolderId == parentFolderId
                    && !f.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingFolder != null)
            {
                return Result<FolderDto>.Failure($"Folder with name '{name}' already exists in this location");
            }

            // Build the full path
            var path = parentFolder != null
                ? $"{parentFolder.Path}/{name}"
                : $"/{enterpriseCode}/{name}";

            var folder = new Folder
            {
                Name = name,
                EnterpriseCode = enterpriseCode,
                ParentFolderId = parentFolderId,
                Path = path,
                Description = description,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.Folders.Add(folder);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Folder created: Id={FolderId}, Name={Name}, Enterprise={EnterpriseCode}, Path={Path}",
                folder.Id, folder.Name, folder.EnterpriseCode, folder.Path);

            var dto = await MapToDto(folder, cancellationToken);
            return Result<FolderDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating folder: Name={Name}, Enterprise={EnterpriseCode}", name, enterpriseCode);
            return Result<FolderDto>.Failure($"Failed to create folder: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<FolderDto>> GetFolderAsync(Guid folderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var folder = await _context.Folders
                .Where(f => f.Id == folderId && !f.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (folder == null)
            {
                return Result<FolderDto>.Failure($"Folder with ID '{folderId}' not found");
            }

            var dto = await MapToDto(folder, cancellationToken);
            return Result<FolderDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting folder: FolderId={FolderId}", folderId);
            return Result<FolderDto>.Failure($"Failed to get folder: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<FolderDto>> GetRootFolderAsync(
        string enterpriseCode,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(enterpriseCode, nameof(enterpriseCode));

        try
        {
            var rootFolder = await _context.Folders
                .Where(f => f.EnterpriseCode == enterpriseCode
                    && f.ParentFolderId == null
                    && !f.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (rootFolder == null)
            {
                // Create root folder if it doesn't exist
                rootFolder = new Folder
                {
                    Name = enterpriseCode,
                    EnterpriseCode = enterpriseCode,
                    ParentFolderId = null,
                    Path = $"/{enterpriseCode}",
                    Description = $"Root folder for {enterpriseCode}",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system"
                };

                _context.Folders.Add(rootFolder);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Root folder created for enterprise: {EnterpriseCode}", enterpriseCode);
            }

            var dto = await MapToDto(rootFolder, cancellationToken);
            return Result<FolderDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting root folder for enterprise: {EnterpriseCode}", enterpriseCode);
            return Result<FolderDto>.Failure($"Failed to get root folder: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<PagedResult<FolderDto>>> ListFoldersAsync(
        string enterpriseCode,
        Guid? parentFolderId,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(enterpriseCode, nameof(enterpriseCode));

        try
        {
            var query = _context.Folders
                .Where(f => f.EnterpriseCode == enterpriseCode
                    && f.ParentFolderId == parentFolderId
                    && !f.IsDeleted);

            var totalCount = await query.CountAsync(cancellationToken);

            var folders = await query
                .OrderBy(f => f.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var dtos = new List<FolderDto>();
            foreach (var folder in folders)
            {
                dtos.Add(await MapToDto(folder, cancellationToken));
            }

            var pagedResult = new PagedResult<FolderDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Result<PagedResult<FolderDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing folders for enterprise: {EnterpriseCode}", enterpriseCode);
            return Result<PagedResult<FolderDto>>.Failure($"Failed to list folders: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result> DeleteFolderAsync(
        Guid folderId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        try
        {
            var folder = await _context.Folders
                .Where(f => f.Id == folderId && !f.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (folder == null)
            {
                return Result.Failure($"Folder with ID '{folderId}' not found");
            }

            // Check if folder has files
            var hasFiles = await _context.FileMetadata
                .AnyAsync(f => f.FolderId == folderId && !f.IsDeleted, cancellationToken);

            if (hasFiles)
            {
                return Result.Failure("Cannot delete folder that contains files");
            }

            // Check if folder has subfolders
            var hasSubfolders = await _context.Folders
                .AnyAsync(f => f.ParentFolderId == folderId && !f.IsDeleted, cancellationToken);

            if (hasSubfolders)
            {
                return Result.Failure("Cannot delete folder that contains subfolders");
            }

            // Soft delete
            folder.IsDeleted = true;
            folder.DeletedAt = DateTime.UtcNow;
            folder.DeletedBy = userId;

            _context.Folders.Update(folder);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Folder deleted: Id={FolderId}, Name={Name}", folderId, folder.Name);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting folder: FolderId={FolderId}", folderId);
            return Result.Failure($"Failed to delete folder: {ex.Message}");
        }
    }

    private async Task<FolderDto> MapToDto(Folder folder, CancellationToken cancellationToken)
    {
        var fileCount = await _context.FileMetadata
            .CountAsync(f => f.FolderId == folder.Id && !f.IsDeleted, cancellationToken);

        var subfolderCount = await _context.Folders
            .CountAsync(f => f.ParentFolderId == folder.Id && !f.IsDeleted, cancellationToken);

        string? parentFolderName = null;
        if (folder.ParentFolderId.HasValue)
        {
            parentFolderName = await _context.Folders
                .Where(f => f.Id == folder.ParentFolderId.Value)
                .Select(f => f.Name)
                .FirstOrDefaultAsync(cancellationToken);
        }

        return new FolderDto
        {
            Id = folder.Id,
            Name = folder.Name,
            ParentFolderId = folder.ParentFolderId,
            ParentFolderName = parentFolderName,
            EnterpriseCode = folder.EnterpriseCode,
            Path = folder.Path,
            Description = folder.Description,
            FileCount = fileCount,
            SubfolderCount = subfolderCount,
            CreatedAt = folder.CreatedAt,
            UpdatedAt = folder.UpdatedAt
        };
    }
}
