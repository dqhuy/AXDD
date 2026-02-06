namespace AXDD.Services.GIS.Api.Exceptions;

/// <summary>
/// Exception thrown when coordinates are invalid
/// </summary>
public class InvalidCoordinatesException : Exception
{
    public InvalidCoordinatesException(string message) : base(message)
    {
    }

    public InvalidCoordinatesException(double latitude, double longitude) 
        : base($"Invalid coordinates: Latitude={latitude}, Longitude={longitude}")
    {
    }

    public InvalidCoordinatesException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown when a location is not found
/// </summary>
public class LocationNotFoundException : Exception
{
    public LocationNotFoundException(string message) : base(message)
    {
    }

    public LocationNotFoundException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
    
    /// <summary>
    /// Create exception for enterprise code not found
    /// </summary>
    public static LocationNotFoundException ForEnterpriseCode(string enterpriseCode)
    {
        return new LocationNotFoundException($"Location not found for enterprise: {enterpriseCode}");
    }
}

/// <summary>
/// Exception thrown when an industrial zone is not found
/// </summary>
public class IndustrialZoneNotFoundException : Exception
{
    public IndustrialZoneNotFoundException(string message) : base(message)
    {
    }

    public IndustrialZoneNotFoundException(Guid id) 
        : base($"Industrial zone not found: {id}")
    {
    }

    public IndustrialZoneNotFoundException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown when a spatial query fails
/// </summary>
public class SpatialQueryException : Exception
{
    public SpatialQueryException(string message) : base(message)
    {
    }

    public SpatialQueryException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
