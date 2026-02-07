using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.BuildingBlocks.Domain.Repositories;
using AXDD.Services.Report.Api.Application.DTOs;
using AXDD.Services.Report.Api.Application.Services.Interfaces;
using AXDD.Services.Report.Api.Domain.Entities;
using AXDD.Services.Report.Api.Domain.Enums;
using AXDD.Services.Report.Api.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.Report.Api.Application.Services;

/// <summary>
/// Service for managing enterprise reports
/// </summary>
public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReportService(IReportRepository reportRepository, IUnitOfWork unitOfWork)
    {
        _reportRepository = reportRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ReportDto>> SubmitReportAsync(
        CreateReportRequest request,
        string userId,
        CancellationToken ct)
    {
        // Check if report already exists for this enterprise, type, period, and year
        var exists = await _reportRepository.ExistsAsync(
            request.EnterpriseId,
            request.ReportType,
            request.ReportPeriod,
            request.Year,
            request.Month,
            cancellationToken: ct);

        if (exists)
        {
            return Result<ReportDto>.Failure(
                "A report for this enterprise, type, period, and year already exists.");
        }

        var report = new EnterpriseReport
        {
            EnterpriseId = request.EnterpriseId,
            EnterpriseName = request.EnterpriseName,
            EnterpriseCode = request.EnterpriseCode,
            ReportType = request.ReportType,
            ReportPeriod = request.ReportPeriod,
            Year = request.Year,
            Month = request.Month,
            SubmittedDate = DateTime.UtcNow,
            SubmittedBy = userId,
            Status = ReportStatus.Pending,
            DataJson = request.DataJson,
            Attachments = request.Attachments,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _reportRepository.AddAsync(report, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<ReportDto>.Success(MapToDto(report));
    }

    public async Task<Result<PagedResult<ReportListDto>>> GetReportsAsync(
        ReportFilterParams filterParams,
        CancellationToken ct)
    {
        var query = ((IReadRepository<EnterpriseReport>)_reportRepository).AsQueryable();

        // Apply filters
        if (filterParams.Status.HasValue)
        {
            query = query.Where(r => r.Status == filterParams.Status.Value);
        }

        if (filterParams.ReportType.HasValue)
        {
            query = query.Where(r => r.ReportType == filterParams.ReportType.Value);
        }

        if (filterParams.EnterpriseId.HasValue)
        {
            query = query.Where(r => r.EnterpriseId == filterParams.EnterpriseId.Value);
        }

        if (filterParams.DateFrom.HasValue)
        {
            query = query.Where(r => r.SubmittedDate >= filterParams.DateFrom.Value);
        }

        if (filterParams.DateTo.HasValue)
        {
            query = query.Where(r => r.SubmittedDate <= filterParams.DateTo.Value);
        }

        if (filterParams.Year.HasValue)
        {
            query = query.Where(r => r.Year == filterParams.Year.Value);
        }

        if (filterParams.ReportPeriod.HasValue)
        {
            query = query.Where(r => r.ReportPeriod == filterParams.ReportPeriod.Value);
        }

        if (!string.IsNullOrWhiteSpace(filterParams.SearchTerm))
        {
            var searchTerm = filterParams.SearchTerm.ToLower();
            query = query.Where(r =>
                r.EnterpriseName.ToLower().Contains(searchTerm) ||
                r.EnterpriseCode.ToLower().Contains(searchTerm));
        }

        // Apply sorting
        query = ApplySorting(query, filterParams.SortBy, filterParams.Descending);

        // Get total count
        var totalCount = await query.CountAsync(ct);

        // Apply pagination
        var reports = await query
            .Skip((filterParams.PageNumber - 1) * filterParams.PageSize)
            .Take(filterParams.PageSize)
            .ToListAsync(ct);

        var reportDtos = reports.Select(MapToListDto).ToList();

        var pagedResult = new PagedResult<ReportListDto>
        {
            Items = reportDtos,
            TotalCount = totalCount,
            PageNumber = filterParams.PageNumber,
            PageSize = filterParams.PageSize
        };

        return Result<PagedResult<ReportListDto>>.Success(pagedResult);
    }

    public async Task<Result<ReportDto>> GetReportByIdAsync(Guid id, CancellationToken ct)
    {
        var report = await _reportRepository.GetByIdAsync(id, ct);
        if (report == null)
        {
            return Result<ReportDto>.Failure($"Report with ID {id} not found.");
        }

        return Result<ReportDto>.Success(MapToDto(report));
    }

    public async Task<Result<ReportDto>> ApproveReportAsync(
        Guid id,
        ApproveReportRequest request,
        Guid reviewerId,
        CancellationToken ct)
    {
        var report = await _reportRepository.GetByIdAsync(id, ct);
        if (report == null)
        {
            return Result<ReportDto>.Failure($"Report with ID {id} not found.");
        }

        if (report.Status != ReportStatus.Pending && report.Status != ReportStatus.UnderReview)
        {
            return Result<ReportDto>.Failure(
                $"Cannot approve report with status {report.Status}. Only Pending or UnderReview reports can be approved.");
        }

        report.Status = ReportStatus.Approved;
        report.ReviewedBy = reviewerId;
        report.ReviewedDate = DateTime.UtcNow;
        report.ReviewerNotes = request.ReviewerNotes;
        report.UpdatedAt = DateTime.UtcNow;

        _reportRepository.Update(report);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<ReportDto>.Success(MapToDto(report));
    }

    public async Task<Result<ReportDto>> RejectReportAsync(
        Guid id,
        RejectReportRequest request,
        Guid reviewerId,
        CancellationToken ct)
    {
        var report = await _reportRepository.GetByIdAsync(id, ct);
        if (report == null)
        {
            return Result<ReportDto>.Failure($"Report with ID {id} not found.");
        }

        if (report.Status != ReportStatus.Pending && report.Status != ReportStatus.UnderReview)
        {
            return Result<ReportDto>.Failure(
                $"Cannot reject report with status {report.Status}. Only Pending or UnderReview reports can be rejected.");
        }

        report.Status = ReportStatus.Rejected;
        report.ReviewedBy = reviewerId;
        report.ReviewedDate = DateTime.UtcNow;
        report.RejectionReason = request.RejectionReason;
        report.ReviewerNotes = request.ReviewerNotes;
        report.UpdatedAt = DateTime.UtcNow;

        _reportRepository.Update(report);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<ReportDto>.Success(MapToDto(report));
    }

    public async Task<Result<List<ReportListDto>>> GetPendingReportsAsync(CancellationToken ct)
    {
        var reports = await _reportRepository.GetPendingReportsAsync(ct);
        var reportDtos = reports.Select(MapToListDto).ToList();
        return Result<List<ReportListDto>>.Success(reportDtos);
    }

    public async Task<Result<PagedResult<ReportListDto>>> GetMyReportsAsync(
        string username,
        int pageNumber,
        int pageSize,
        CancellationToken ct)
    {
        var query = ((IReadRepository<EnterpriseReport>)_reportRepository).AsQueryable()
            .Where(r => r.SubmittedBy == username)
            .OrderByDescending(r => r.SubmittedDate);

        var totalCount = await query.CountAsync(ct);

        var reports = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var reportDtos = reports.Select(MapToListDto).ToList();

        var pagedResult = new PagedResult<ReportListDto>
        {
            Items = reportDtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return Result<PagedResult<ReportListDto>>.Success(pagedResult);
    }

    private static IQueryable<EnterpriseReport> ApplySorting(
        IQueryable<EnterpriseReport> query,
        string? sortBy,
        bool descending)
    {
        return sortBy?.ToLower() switch
        {
            "enterprisename" => descending
                ? query.OrderByDescending(r => r.EnterpriseName)
                : query.OrderBy(r => r.EnterpriseName),
            "reporttype" => descending
                ? query.OrderByDescending(r => r.ReportType)
                : query.OrderBy(r => r.ReportType),
            "status" => descending
                ? query.OrderByDescending(r => r.Status)
                : query.OrderBy(r => r.Status),
            "year" => descending
                ? query.OrderByDescending(r => r.Year)
                : query.OrderBy(r => r.Year),
            "submitteddate" or _ => descending
                ? query.OrderByDescending(r => r.SubmittedDate)
                : query.OrderBy(r => r.SubmittedDate)
        };
    }

    private static ReportDto MapToDto(EnterpriseReport report)
    {
        return new ReportDto
        {
            Id = report.Id,
            EnterpriseId = report.EnterpriseId,
            EnterpriseName = report.EnterpriseName,
            EnterpriseCode = report.EnterpriseCode,
            ReportType = report.ReportType,
            ReportTypeName = report.ReportType.ToString(),
            ReportPeriod = report.ReportPeriod,
            ReportPeriodName = report.ReportPeriod.ToString(),
            Year = report.Year,
            Month = report.Month,
            SubmittedDate = report.SubmittedDate,
            SubmittedBy = report.SubmittedBy,
            Status = report.Status,
            StatusName = report.Status.ToString(),
            DataJson = report.DataJson,
            Attachments = report.Attachments,
            ReviewedBy = report.ReviewedBy,
            ReviewedDate = report.ReviewedDate,
            ReviewerNotes = report.ReviewerNotes,
            RejectionReason = report.RejectionReason,
            CreatedAt = report.CreatedAt,
            UpdatedAt = report.UpdatedAt
        };
    }

    private static ReportListDto MapToListDto(EnterpriseReport report)
    {
        return new ReportListDto
        {
            Id = report.Id,
            EnterpriseId = report.EnterpriseId,
            EnterpriseName = report.EnterpriseName,
            EnterpriseCode = report.EnterpriseCode,
            ReportType = report.ReportType,
            ReportTypeName = report.ReportType.ToString(),
            ReportPeriod = report.ReportPeriod,
            ReportPeriodName = report.ReportPeriod.ToString(),
            Year = report.Year,
            Month = report.Month,
            SubmittedDate = report.SubmittedDate,
            SubmittedBy = report.SubmittedBy,
            Status = report.Status,
            StatusName = report.Status.ToString(),
            AttachmentCount = report.Attachments.Count,
            CreatedAt = report.CreatedAt
        };
    }
}
