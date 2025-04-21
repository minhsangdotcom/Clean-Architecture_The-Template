using System.Collections;
using SharedKernel.Common.Messages;

namespace Application.UnitTest;

public class MessageResultComparer : IEqualityComparer
{
    public new bool Equals(object? x, object? y)
    {
        var a = x as MessageResult;
        var b = y as MessageResult;
        if (ReferenceEquals(a, b))
            return true;
        if (a is null || b is null)
            return false;

        return a.Message == b.Message && a.En == b.En && a.Vi == b.Vi;
    }

    public int GetHashCode(object obj)
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
