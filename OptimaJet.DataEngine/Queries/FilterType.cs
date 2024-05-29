namespace OptimaJet.DataEngine.Queries;

public enum FilterType
{
    Unspecified,
    Property,
    Constant,
    True,
    False,
    Null,
    And,
    Or,
    Not,
    Equal,
    NotEqual,
    Greater,
    GreaterEqual,
    Less,
    LessEqual,
    IsNull,
    IsNotNull,
    IsTrue,
    IsFalse,
    Like,
    LikePattern,
    In,
    Array,
}