namespace FTG12_ReviewsApi.Application.Common.Exceptions;

/// <summary>
/// Thrown when a user does not have permission to perform an action.
/// </summary>
public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message)
    {
    }
}
