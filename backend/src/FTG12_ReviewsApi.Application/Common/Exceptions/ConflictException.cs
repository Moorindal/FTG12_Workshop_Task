namespace FTG12_ReviewsApi.Application.Common.Exceptions;

/// <summary>
/// Thrown when a resource conflict occurs.
/// </summary>
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
}
