namespace Contracts.Common.Messages;

public enum MessageType
{
    MaximumLength = 1,
    MinumumLength = 2,
    ValidFormat = 3,
    Found = 4,
    Existence = 5,
    Correct = 6,
    Active = 7,
    OuttaOption = 8,
    GreaterThan = 9,
    GreaterThanEqual = 10,
    LessThan = 11,
    LessThanEqual = 12,
    Empty = 13,
    Null = 14,
    Unique = 15,
    Strong = 16,
    Expired = 17,
    Redundant = 18,

    //* Custom message
    LackOfArrayOperatorIndex = 19,
    LackOfOperator = 20,
    LackOfArrayOperatorElement = 21,
    MustBeInteger = 22,
    MustBeDatetime = 23,
    MustBeUlid = 24,
}
