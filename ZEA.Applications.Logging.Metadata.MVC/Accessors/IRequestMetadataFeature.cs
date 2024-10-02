using ZEA.Applications.Logging.Metadata.Abstractions.Interfaces;

namespace ZEA.Applications.Logging.Metadata.MVC.Accessors;

public interface IRequestMetadataFeature<TMetadata> where TMetadata : IRequestMetadata
{
	TMetadata Metadata { get; set; }
}