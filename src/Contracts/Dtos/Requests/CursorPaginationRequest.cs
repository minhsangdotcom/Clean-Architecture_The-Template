namespace Contracts.Dtos.Requests;

public class CursorPaginationRequest(string? beforeCursor, string? afterCursor, int size, string? order, string uniqueOrdering)
{
    public string? BeforeCursor { get; private set; } = beforeCursor;

    public string? AfterCursor { get; private set; } = afterCursor;

    public int Size { get; private set; } = size;

    public string? Order { get; private set; } = order;

    public string UniqueOrdering { get; private set; } = uniqueOrdering;
}