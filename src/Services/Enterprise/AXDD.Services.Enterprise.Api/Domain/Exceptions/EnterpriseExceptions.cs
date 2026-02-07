namespace AXDD.Services.Enterprise.Api.Domain.Exceptions;

/// <summary>
/// Exception thrown when an enterprise is not found
/// </summary>
public class EnterpriseNotFoundException : Exception
{
    public EnterpriseNotFoundException(Guid id) 
        : base($"Enterprise with ID '{id}' was not found.")
    {
    }

    public EnterpriseNotFoundException(string code) 
        : base($"Enterprise with code '{code}' was not found.")
    {
    }
}

/// <summary>
/// Exception thrown when a duplicate tax code is detected
/// </summary>
public class DuplicateTaxCodeException : Exception
{
    public DuplicateTaxCodeException(string taxCode) 
        : base($"An enterprise with tax code '{taxCode}' already exists.")
    {
    }
}

/// <summary>
/// Exception thrown when a duplicate enterprise code is detected
/// </summary>
public class DuplicateCodeException : Exception
{
    public DuplicateCodeException(string code) 
        : base($"An enterprise with code '{code}' already exists.")
    {
    }
}

/// <summary>
/// Exception thrown when an invalid status transition is attempted
/// </summary>
public class InvalidStatusTransitionException : Exception
{
    public InvalidStatusTransitionException(string currentStatus, string newStatus) 
        : base($"Cannot change status from '{currentStatus}' to '{newStatus}'.")
    {
    }
}

/// <summary>
/// Exception thrown when trying to delete an enterprise with active licenses
/// </summary>
public class CannotDeleteEnterpriseException : Exception
{
    public CannotDeleteEnterpriseException(Guid enterpriseId) 
        : base($"Cannot delete enterprise '{enterpriseId}' because it has active licenses.")
    {
    }
}
