using AXDD.BuildingBlocks.Common.Exceptions;

namespace AXDD.Services.MasterData.Api.Exceptions;

/// <summary>
/// Exception thrown when master data is not found
/// </summary>
public class MasterDataNotFoundException : NotFoundException
{
    public MasterDataNotFoundException(string entityType, string key)
        : base($"{entityType} with key '{key}' was not found.")
    {
    }

    public MasterDataNotFoundException(string entityType, Guid id)
        : base($"{entityType} with ID '{id}' was not found.")
    {
    }
}

/// <summary>
/// Exception thrown when a duplicate code is detected
/// </summary>
public class DuplicateCodeException : ConflictException
{
    public DuplicateCodeException(string entityType, string code)
        : base($"{entityType} with code '{code}' already exists.")
    {
    }
}
