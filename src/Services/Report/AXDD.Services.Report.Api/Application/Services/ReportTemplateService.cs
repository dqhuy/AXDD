using AXDD.BuildingBlocks.Common.Results;
using AXDD.BuildingBlocks.Domain.Repositories;
using AXDD.Services.Report.Api.Application.DTOs;
using AXDD.Services.Report.Api.Application.Services.Interfaces;
using AXDD.Services.Report.Api.Domain.Entities;
using AXDD.Services.Report.Api.Domain.Enums;
using AXDD.Services.Report.Api.Domain.Repositories;

namespace AXDD.Services.Report.Api.Application.Services;

/// <summary>
/// Service for managing report templates
/// </summary>
public class ReportTemplateService : IReportTemplateService
{
    private readonly IReportTemplateRepository _templateRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReportTemplateService(IReportTemplateRepository templateRepository, IUnitOfWork unitOfWork)
    {
        _templateRepository = templateRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<ReportTemplateDto>>> GetTemplatesAsync(CancellationToken ct)
    {
        var templates = await _templateRepository.GetActiveTemplatesAsync(ct);
        var templateDtos = templates.Select(MapToDto).ToList();
        return Result<List<ReportTemplateDto>>.Success(templateDtos);
    }

    public async Task<Result<ReportTemplateDto>> GetTemplateByIdAsync(Guid id, CancellationToken ct)
    {
        var template = await _templateRepository.GetByIdAsync(id, ct);
        if (template == null)
        {
            return Result<ReportTemplateDto>.Failure($"Template with ID {id} not found.");
        }

        return Result<ReportTemplateDto>.Success(MapToDto(template));
    }

    public async Task<Result<ReportTemplateDto>> GetTemplateByTypeAsync(ReportType type, CancellationToken ct)
    {
        var template = await _templateRepository.GetByReportTypeAsync(type, ct);
        if (template == null)
        {
            return Result<ReportTemplateDto>.Failure($"No active template found for report type {type}.");
        }

        return Result<ReportTemplateDto>.Success(MapToDto(template));
    }

    public async Task<Result<ReportTemplateDto>> CreateTemplateAsync(
        CreateTemplateRequest request,
        string userId,
        CancellationToken ct)
    {
        // Check if template name already exists
        var nameExists = await _templateRepository.TemplateNameExistsAsync(request.TemplateName, cancellationToken: ct);
        if (nameExists)
        {
            return Result<ReportTemplateDto>.Failure($"Template with name '{request.TemplateName}' already exists.");
        }

        var template = new ReportTemplate
        {
            ReportType = request.ReportType,
            TemplateName = request.TemplateName,
            FieldsJson = request.FieldsJson,
            Description = request.Description,
            Version = request.Version,
            IsActive = true,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _templateRepository.AddAsync(template, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<ReportTemplateDto>.Success(MapToDto(template));
    }

    private static ReportTemplateDto MapToDto(ReportTemplate template)
    {
        return new ReportTemplateDto
        {
            Id = template.Id,
            ReportType = template.ReportType,
            ReportTypeName = template.ReportType.ToString(),
            TemplateName = template.TemplateName,
            FieldsJson = template.FieldsJson,
            IsActive = template.IsActive,
            Description = template.Description,
            Version = template.Version,
            CreatedAt = template.CreatedAt,
            UpdatedAt = template.UpdatedAt
        };
    }
}
