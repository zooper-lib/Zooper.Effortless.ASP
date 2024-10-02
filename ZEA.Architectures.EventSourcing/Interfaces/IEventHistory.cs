using ZEA.Applications.Logging.Metadata.Abstractions.Interfaces;
using ZEA.Architectures.DDD.Abstractions.Interfaces;

namespace ZEA.Architectures.EventSourcing.Interfaces;

/// <summary>
/// 
/// </summary>
public interface IEventHistory;

public sealed record EventHistory(IDomainEvent Data, IMetadata Metadata);