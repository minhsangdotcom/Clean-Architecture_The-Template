namespace Contracts.Extensions.QueryExtensions;
public record PaginationMetadata<T>(IEnumerable<T> Entities, string? NextCursor, string? PreviousCursor);