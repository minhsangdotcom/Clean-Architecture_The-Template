namespace Domain.Common;

public static class FilterBaseEntity
{
    public static void IsValidBaseType(this Type type)
    {
        if (!Validate(type))
        {
            throw new Exception("type is not valid in constrain");
        }
    }

    private static bool Validate(Type type)
    {
        if (type.BaseType == typeof(BaseEntity) || type.BaseType == typeof(AggregateRoot))
        {
            return true;
        }

        Type? currentBaseType = type.BaseType;

        while (
            currentBaseType != null
            && currentBaseType != typeof(BaseEntity)
            && currentBaseType != typeof(AggregateRoot)
        )
        {
            if (
                currentBaseType.BaseType == typeof(BaseEntity)
                || currentBaseType.BaseType == typeof(AggregateRoot)
            )
            {
                return true;
            }
            currentBaseType = currentBaseType.BaseType;
        }

        return false;
    }
}
