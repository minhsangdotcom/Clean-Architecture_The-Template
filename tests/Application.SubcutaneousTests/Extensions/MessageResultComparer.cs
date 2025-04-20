using System.Diagnostics.CodeAnalysis;
using SharedKernel.Common.Messages;

namespace Application.SubcutaneousTests.Extensions;

public class MessageResultComparer : IEqualityComparer<MessageResult>
{
    public bool Equals(MessageResult? x, MessageResult? y)
    {
        var a = x;
        var b = y;
        if (ReferenceEquals(a, b))
            return true;
        if (a is null || b is null)
            return false;

        return a.Message == b.Message && a.En == b.En && a.Vi == b.Vi;
    }

    public int GetHashCode([DisallowNull] MessageResult obj)
    {
        if (obj is not MessageResult mr)
            return 0;
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + (mr.Message?.GetHashCode() ?? 0);
            hash = hash * 23 + mr.En?.GetHashCode() ?? 0;
            hash = hash * 23 + mr.Vi?.GetHashCode() ?? 0;
            return hash;
        }
    }
}
