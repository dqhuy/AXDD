using AXDD.BuildingBlocks.Common.Results;
using AXDD.BuildingBlocks.Domain.Repositories;
using AXDD.Services.Enterprise.Api.Application.DTOs;
using AXDD.Services.Enterprise.Api.Application.Services.Interfaces;
using AXDD.Services.Enterprise.Api.Domain.Entities;
using AXDD.Services.Enterprise.Api.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.Enterprise.Api.Application.Services;

/// <summary>
/// Service implementation for managing enterprise contact persons
/// </summary>
public class ContactPersonService : IContactPersonService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEnterpriseHistoryService _historyService;

    public ContactPersonService(IUnitOfWork unitOfWork, IEnterpriseHistoryService historyService)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(historyService);
        _unitOfWork = unitOfWork;
        _historyService = historyService;
    }

    /// <inheritdoc/>
    public async Task<Result<IReadOnlyList<ContactPersonDto>>> GetByEnterpriseIdAsync(Guid enterpriseId, CancellationToken ct)
    {
        try
        {
            var repository = _unitOfWork.Repository<ContactPerson>();
            var contacts = await repository
                .AsQueryable()
                .Where(c => c.EnterpriseId == enterpriseId)
                .OrderByDescending(c => c.IsMain)
                .ThenBy(c => c.FullName)
                .ToListAsync(ct);

            var dtos = contacts.Select(MapToDto).ToList();
            return Result<IReadOnlyList<ContactPersonDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyList<ContactPersonDto>>.Failure($"Failed to retrieve contact persons: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<ContactPersonDto>> GetByIdAsync(Guid id, CancellationToken ct)
    {
        try
        {
            var repository = _unitOfWork.Repository<ContactPerson>();
            var contact = await repository.GetByIdAsync(id, ct);

            if (contact == null)
                return Result<ContactPersonDto>.Failure("Contact person not found");

            return Result<ContactPersonDto>.Success(MapToDto(contact));
        }
        catch (Exception ex)
        {
            return Result<ContactPersonDto>.Failure($"Failed to retrieve contact person: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<ContactPersonDto>> CreateAsync(CreateContactRequest request, string userId, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        try
        {
            // Validate enterprise exists
            var enterpriseRepo = _unitOfWork.Repository<EnterpriseEntity>();
            var enterpriseExists = await enterpriseRepo.AnyAsync(e => e.Id == request.EnterpriseId, ct);
            if (!enterpriseExists)
                return Result<ContactPersonDto>.Failure("Enterprise not found");

            // If setting as main contact, unset existing main contact
            if (request.IsMain)
            {
                await UnsetMainContactAsync(request.EnterpriseId, ct);
            }

            var contact = new ContactPerson
            {
                EnterpriseId = request.EnterpriseId,
                FullName = request.FullName,
                Position = request.Position,
                Department = request.Department,
                Phone = request.Phone,
                Email = request.Email,
                IsMain = request.IsMain,
                Notes = request.Notes
            };

            var repository = _unitOfWork.Repository<ContactPerson>();
            await repository.AddAsync(contact, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            // Log the change
            await _historyService.LogContactChangeAsync(
                request.EnterpriseId,
                userId,
                ChangeType.ContactAdded,
                request.FullName,
                $"Contact person added: {request.FullName}",
                ct);

            return Result<ContactPersonDto>.Success(MapToDto(contact));
        }
        catch (Exception ex)
        {
            return Result<ContactPersonDto>.Failure($"Failed to create contact person: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<ContactPersonDto>> UpdateAsync(Guid id, UpdateContactRequest request, string userId, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        try
        {
            var repository = _unitOfWork.Repository<ContactPerson>();
            var contact = await repository.GetByIdAsync(id, ct);

            if (contact == null)
                return Result<ContactPersonDto>.Failure("Contact person not found");

            // If setting as main contact, unset existing main contact
            if (request.IsMain && !contact.IsMain)
            {
                await UnsetMainContactAsync(contact.EnterpriseId, ct);
            }

            contact.FullName = request.FullName;
            contact.Position = request.Position;
            contact.Department = request.Department;
            contact.Phone = request.Phone;
            contact.Email = request.Email;
            contact.IsMain = request.IsMain;
            contact.Notes = request.Notes;

            repository.Update(contact);
            await _unitOfWork.SaveChangesAsync(ct);

            // Log the change
            await _historyService.LogContactChangeAsync(
                contact.EnterpriseId,
                userId,
                ChangeType.ContactUpdated,
                contact.FullName,
                $"Contact person updated: {contact.FullName}",
                ct);

            return Result<ContactPersonDto>.Success(MapToDto(contact));
        }
        catch (Exception ex)
        {
            return Result<ContactPersonDto>.Failure($"Failed to update contact person: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<bool>> DeleteAsync(Guid id, string userId, CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        try
        {
            var repository = _unitOfWork.Repository<ContactPerson>();
            var contact = await repository.GetByIdAsync(id, ct);

            if (contact == null)
                return Result<bool>.Failure("Contact person not found");

            var enterpriseId = contact.EnterpriseId;
            var contactName = contact.FullName;

            repository.Delete(contact);
            await _unitOfWork.SaveChangesAsync(ct);

            // Log the change
            await _historyService.LogContactChangeAsync(
                enterpriseId,
                userId,
                ChangeType.ContactRemoved,
                contactName,
                $"Contact person removed: {contactName}",
                ct);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to delete contact person: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<ContactPersonDto>> SetAsMainContactAsync(Guid id, string userId, CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        try
        {
            var repository = _unitOfWork.Repository<ContactPerson>();
            var contact = await repository.GetByIdAsync(id, ct);

            if (contact == null)
                return Result<ContactPersonDto>.Failure("Contact person not found");

            if (contact.IsMain)
                return Result<ContactPersonDto>.Success(MapToDto(contact));

            // Unset existing main contact
            await UnsetMainContactAsync(contact.EnterpriseId, ct);

            contact.IsMain = true;
            repository.Update(contact);
            await _unitOfWork.SaveChangesAsync(ct);

            // Log the change
            await _historyService.LogContactChangeAsync(
                contact.EnterpriseId,
                userId,
                ChangeType.MainContactChanged,
                contact.FullName,
                $"Main contact changed to: {contact.FullName}",
                ct);

            return Result<ContactPersonDto>.Success(MapToDto(contact));
        }
        catch (Exception ex)
        {
            return Result<ContactPersonDto>.Failure($"Failed to set main contact: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public Task<Result<IReadOnlyList<ContactPersonDto>>> GetByEnterpriseAsync(Guid enterpriseId, CancellationToken ct)
    {
        return GetByEnterpriseIdAsync(enterpriseId, ct);
    }

    /// <inheritdoc/>
    public Task<Result<ContactPersonDto>> SetMainContactAsync(Guid contactId, string userId, CancellationToken ct)
    {
        return SetAsMainContactAsync(contactId, userId, ct);
    }

    private async Task UnsetMainContactAsync(Guid enterpriseId, CancellationToken ct)
    {
        var repository = _unitOfWork.Repository<ContactPerson>();
        var mainContacts = await repository
            .AsQueryable()
            .Where(c => c.EnterpriseId == enterpriseId && c.IsMain)
            .ToListAsync(ct);

        foreach (var contact in mainContacts)
        {
            contact.IsMain = false;
            repository.Update(contact);
        }
    }

    private static ContactPersonDto MapToDto(ContactPerson entity)
    {
        return new ContactPersonDto
        {
            Id = entity.Id,
            EnterpriseId = entity.EnterpriseId,
            FullName = entity.FullName,
            Position = entity.Position,
            Department = entity.Department,
            Phone = entity.Phone,
            Email = entity.Email,
            IsMain = entity.IsMain,
            Notes = entity.Notes,
            CreatedAt = entity.CreatedAt,
            CreatedBy = entity.CreatedBy
        };
    }
}
