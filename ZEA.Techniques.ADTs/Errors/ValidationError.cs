namespace ZEA.Techniques.ADTs.Errors;

/// <summary>
/// Error representing a situation where the provided data fails to meet validation rules.
/// </summary>
public sealed record ValidationError(string Message, string? Details = null)
	: LogicalError(Message, Details)
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ValidationError"/> record with PublicDetails and InternalDetails.
	/// </summary>
	/// <param name="publicDetails">The public details of the error.</param>
	/// <param name="internalDetails">The internal details of the error.</param>
	[Obsolete("We should not expose InternalDetails (the Exception) to the client.")]
	public ValidationError(
		PublicDetails publicDetails,
		InternalDetails? internalDetails = null)
		: this(publicDetails.Message, publicDetails.Details)
	{
		InternalDetails = internalDetails;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ValidationError"/> record with message, details, and an optional exception.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="details">Additional details about the error.</param>
	/// <param name="ex">The exception associated with the error.</param>
	[Obsolete(
		"This constructor is deprecated and will be removed in a future version. Use the constructor without the Exception parameter instead."
	)]
	public ValidationError(
		string message,
		string? details = null,
		Exception? ex = null)
		: this(message, details)
	{
		if (ex != null)
		{
			InternalDetails = new(new(ex, true));
		}
	}
}