using System.Text;
using System.Text.Json.Serialization;
using Contracts.Extensions;

namespace Contracts.Dtos.Responses;

public class PaginationResponse<T>
{
    public IEnumerable<T>? Data { get; private set; }

    public Paging<T>? Paging { get; private set; }

    public PaginationResponse(IEnumerable<T> data, int totalPage, int currentPage, int pageSize)
    {
        Data = data;
        Paging = new Paging<T>(totalPage, currentPage, pageSize);
    }

    public PaginationResponse(
        IEnumerable<T> data,
        int totalPage,
        int pageSize,
        T? FirstPage = default,
        T? LastPage = default,
        string? PreviousCursor = null,
        string? NextCursor = null)
    {
        Data = data;
        Paging = new Paging<T>(totalPage, pageSize, FirstPage, LastPage, PreviousCursor, NextCursor);
    }
}

public class Paging<T>
{
    public int? CurrentPage { get; set; }

    public int PageSize { get; set; }

    public int TotalPage { get; set; }

    public bool? HasNextPage { get; set; }

    public bool? HasPreviousPage { get; set; }

    [JsonIgnore]
    public T? FirstPage { get; set; }

    [JsonIgnore]
    public T? LastPage { get; set; }

    public string? Previous { get; set; }

    public string? Next { get; set; }

    public Paging(int totalPage, int currentPage = 1, int pageSize = 10)
    {
        CurrentPage = currentPage;
        PageSize = pageSize;
        TotalPage = totalPage;

        HasNextPage = (currentPage + 1) * pageSize <= totalPage;
        HasPreviousPage = currentPage > 1;
    }

    public Paging(
        int totalPage,
        int pageSize = 10,
        T? FirstPage = default,
        T? LastPage = default,
        string? PreviousCursor = null,
        string? NextCursor = null)
    {
        PageSize = pageSize;
        TotalPage = totalPage;

        this.FirstPage = FirstPage;
        this.LastPage = LastPage;


        bool isNext = Convert.ToBase64String(Encoding.UTF8.GetBytes(SerializerExtension.Serialize(LastPage!))) == NextCursor;

        if (!isNext)
        {
            Next = NextCursor;
        }

        HasNextPage = !isNext;

        bool isPrevious = Convert.ToBase64String(Encoding.UTF8.GetBytes(SerializerExtension.Serialize(FirstPage!))) == PreviousCursor;

        if (!isPrevious)
        {
            Previous = PreviousCursor;
        }

        HasPreviousPage = !isPrevious;
    }
}