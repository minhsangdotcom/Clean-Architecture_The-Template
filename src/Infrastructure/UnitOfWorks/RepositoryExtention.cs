using System.Security.Cryptography;
using System.Text;
using Domain.Common;
using Newtonsoft.Json;
using SharedKernel.Models;

namespace Infrastructure.UnitOfWorks;

public static class RepositoryExtention
{
    public static string GetSort(this string? sort)
    {
        string defaultSort = GetDefaultSort(sort);
        return $"{defaultSort},{nameof(BaseEntity.Id)}";
    }

    public static string GetDefaultSort(this string? sort) =>
        string.IsNullOrWhiteSpace(sort)
            ? $"{nameof(BaseEntity.CreatedAt)}{OrderTerm.DELIMITER}{OrderTerm.DESC}"
            : sort.Trim();

    public static string HashKey(params object?[] parameters)
    {
        StringBuilder text = new();
        foreach (object? param in parameters)
        {
            if (param is null)
            {
                continue;
            }

            if (param is string)
            {
                AppendParameter(text, param.ToString()!);
                continue;
            }

            var result = JsonConvert.SerializeObject(param);
            AppendParameter(text, result);
        }

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(text.ToString()));
        return Convert.ToHexString(bytes);
    }

    private static StringBuilder AppendParameter(StringBuilder text, string param)
    {
        if (!string.IsNullOrWhiteSpace(text.ToString()))
        {
            text.Append('_');
        }
        text.Append(param);

        return text;
    }
}
