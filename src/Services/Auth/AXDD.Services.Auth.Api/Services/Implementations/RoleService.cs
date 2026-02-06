using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.Auth.Api.DTOs;
using AXDD.Services.Auth.Api.Entities;
using AXDD.Services.Auth.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.Auth.Api.Services.Implementations;

/// <summary>
/// Role management service implementation
/// </summary>
public class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ILogger<RoleService> _logger;

    public RoleService(
        RoleManager<ApplicationRole> roleManager,
        ILogger<RoleService> logger)
    {
        _roleManager = roleManager;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result<List<RoleDto>>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _roleManager.Roles
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);

        var roleDtos = roles.Select(r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name ?? string.Empty,
            Description = r.Description,
            CreatedAt = r.CreatedAt
        }).ToList();

        return Result<List<RoleDto>>.Success(roleDtos);
    }

    /// <inheritdoc />
    public async Task<Result<RoleDto>> GetRoleByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await _roleManager.FindByIdAsync(roleId.ToString());
        if (role == null)
        {
            return Result<RoleDto>.Failure("Role not found");
        }

        var roleDto = new RoleDto
        {
            Id = role.Id,
            Name = role.Name ?? string.Empty,
            Description = role.Description,
            CreatedAt = role.CreatedAt
        };

        return Result<RoleDto>.Success(roleDto);
    }

    /// <inheritdoc />
    public async Task<Result<RoleDto>> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Check if role already exists
        var existingRole = await _roleManager.FindByNameAsync(request.Name);
        if (existingRole != null)
        {
            return Result<RoleDto>.Failure("Role already exists");
        }

        var role = new ApplicationRole
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description
        };

        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            _logger.LogWarning("Role creation failed: {Errors}", string.Join(", ", errors));
            return Result<RoleDto>.Failure(errors);
        }

        _logger.LogInformation("Role created successfully - {RoleId}", role.Id);

        var roleDto = new RoleDto
        {
            Id = role.Id,
            Name = role.Name ?? string.Empty,
            Description = role.Description,
            CreatedAt = role.CreatedAt
        };

        return Result<RoleDto>.Success(roleDto);
    }

    /// <inheritdoc />
    public async Task<Result<RoleDto>> UpdateRoleAsync(
        Guid roleId,
        UpdateRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var role = await _roleManager.FindByIdAsync(roleId.ToString());
        if (role == null)
        {
            return Result<RoleDto>.Failure("Role not found");
        }

        // Update name if provided
        if (!string.IsNullOrWhiteSpace(request.Name) && request.Name != role.Name)
        {
            var existingRole = await _roleManager.FindByNameAsync(request.Name);
            if (existingRole != null && existingRole.Id != roleId)
            {
                return Result<RoleDto>.Failure("Role name already exists");
            }

            role.Name = request.Name;
        }

        // Update description
        if (request.Description != null)
        {
            role.Description = request.Description;
        }

        var result = await _roleManager.UpdateAsync(role);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            _logger.LogWarning("Role update failed - {RoleId}: {Errors}", roleId, string.Join(", ", errors));
            return Result<RoleDto>.Failure(errors);
        }

        _logger.LogInformation("Role updated successfully - {RoleId}", roleId);

        var roleDto = new RoleDto
        {
            Id = role.Id,
            Name = role.Name ?? string.Empty,
            Description = role.Description,
            CreatedAt = role.CreatedAt
        };

        return Result<RoleDto>.Success(roleDto);
    }

    /// <inheritdoc />
    public async Task<Result> DeleteRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await _roleManager.FindByIdAsync(roleId.ToString());
        if (role == null)
        {
            return Result.Failure("Role not found");
        }

        var result = await _roleManager.DeleteAsync(role);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            _logger.LogWarning("Role deletion failed - {RoleId}: {Errors}", roleId, string.Join(", ", errors));
            return Result.Failure(errors);
        }

        _logger.LogInformation("Role deleted successfully - {RoleId}", roleId);

        return Result.Success();
    }
}
