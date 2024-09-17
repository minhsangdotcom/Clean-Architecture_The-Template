namespace Domain.Common;

public static class FilterBaseEntity
{
    public static void IsValidBaseType(this Type type)
    {
        if(type.BaseType != typeof(BaseEntity) && type.BaseType != typeof(AggregateRoot))
        {
            throw new Exception("type is not valid in constrain");
        }
    }
        
}
