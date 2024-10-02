using ZEA.Applications.Logging.Metadata.Abstractions.Interfaces;

namespace ZEA.Applications.Logging.Metadata.MVC.Accessors;

/// <summary>
/// Accessor for request metadata stored in the current HTTP context.
/// </summary>
/// <typeparam name="TMetadata">The type of request metadata.</typeparam>
public interface IRequestMetadataAccessor<TMetadata> where TMetadata : IRequestMetadata
{
	/// <summary>
	/// Gets or sets the request metadata.
	/// </summary>
	TMetadata? Metadata { get; set; }
}