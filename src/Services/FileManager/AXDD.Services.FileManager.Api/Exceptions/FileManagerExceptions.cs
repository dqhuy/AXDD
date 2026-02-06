namespace AXDD.Services.FileManager.Api.Exceptions;

/// <summary>
/// Exception thrown when a file is not found
/// </summary>
public class FileNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of FileNotFoundException
    /// </summary>
    /// <param name="fileId">The file ID that was not found</param>
    public FileNotFoundException(Guid fileId)
        : base($"File with ID '{fileId}' was not found")
    {
        FileId = fileId;
    }

    /// <summary>
    /// Gets the file ID
    /// </summary>
    public Guid FileId { get; }
}

/// <summary>
/// Exception thrown when storage quota is exceeded
/// </summary>
public class QuotaExceededException : Exception
{
    /// <summary>
    /// Initializes a new instance of QuotaExceededException
    /// </summary>
    /// <param name="enterpriseCode">The enterprise code</param>
    /// <param name="requiredBytes">Required bytes</param>
    /// <param name="availableBytes">Available bytes</param>
    public QuotaExceededException(string enterpriseCode, long requiredBytes, long availableBytes)
        : base($"Storage quota exceeded for enterprise '{enterpriseCode}'. Required: {requiredBytes:N0} bytes, Available: {availableBytes:N0} bytes")
    {
        EnterpriseCode = enterpriseCode;
        RequiredBytes = requiredBytes;
        AvailableBytes = availableBytes;
    }

    /// <summary>
    /// Gets the enterprise code
    /// </summary>
    public string EnterpriseCode { get; }

    /// <summary>
    /// Gets the required bytes
    /// </summary>
    public long RequiredBytes { get; }

    /// <summary>
    /// Gets the available bytes
    /// </summary>
    public long AvailableBytes { get; }
}

/// <summary>
/// Exception thrown when an invalid file type is uploaded
/// </summary>
public class InvalidFileTypeException : Exception
{
    /// <summary>
    /// Initializes a new instance of InvalidFileTypeException
    /// </summary>
    /// <param name="fileExtension">The invalid file extension</param>
    /// <param name="allowedExtensions">The allowed extensions</param>
    public InvalidFileTypeException(string fileExtension, IEnumerable<string> allowedExtensions)
        : base($"File type '{fileExtension}' is not allowed. Allowed types: {string.Join(", ", allowedExtensions)}")
    {
        FileExtension = fileExtension;
        AllowedExtensions = allowedExtensions.ToList();
    }

    /// <summary>
    /// Gets the file extension
    /// </summary>
    public string FileExtension { get; }

    /// <summary>
    /// Gets the allowed extensions
    /// </summary>
    public List<string> AllowedExtensions { get; }
}

/// <summary>
/// Exception thrown when file size exceeds the maximum allowed
/// </summary>
public class FileSizeTooLargeException : Exception
{
    /// <summary>
    /// Initializes a new instance of FileSizeTooLargeException
    /// </summary>
    /// <param name="fileSize">The file size</param>
    /// <param name="maxFileSize">The maximum allowed file size</param>
    public FileSizeTooLargeException(long fileSize, long maxFileSize)
        : base($"File size {fileSize:N0} bytes exceeds the maximum allowed size of {maxFileSize:N0} bytes")
    {
        FileSize = fileSize;
        MaxFileSize = maxFileSize;
    }

    /// <summary>
    /// Gets the file size
    /// </summary>
    public long FileSize { get; }

    /// <summary>
    /// Gets the maximum file size
    /// </summary>
    public long MaxFileSize { get; }
}

/// <summary>
/// Exception thrown when a folder is not found
/// </summary>
public class FolderNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of FolderNotFoundException
    /// </summary>
    /// <param name="folderId">The folder ID that was not found</param>
    public FolderNotFoundException(Guid folderId)
        : base($"Folder with ID '{folderId}' was not found")
    {
        FolderId = folderId;
    }

    /// <summary>
    /// Gets the folder ID
    /// </summary>
    public Guid FolderId { get; }
}
