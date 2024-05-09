using System.Collections;
using SqlKata;
using SqlKata.Compilers;

namespace OptimaJet.DataEngine.Oracle.Implementation;

internal class CustomOracleCompiler : OracleCompiler
{
    public override string CompileTrue()
    {
        return "1";
    }

    public override string CompileFalse()
    {
        return "0";
    }

    protected override string CompileCondition(SqlResult ctx, AbstractCondition clause)
    {
        if (clause is BasicCondition {Value: string} basic)
        {
            string condition = CompileStringColumn(basic.Column) + " " +
                               checkOperator(basic.Operator) + " " +
                               Parameter(ctx, basic.Value);

            return basic.IsNot ? "NOT (" + condition + ")" : condition;
        }

        if (clause is InCondition<object> {Values: IEnumerable enumerable} inCondition)
        {
            if (!inCondition.Values.Any())
            {
                return !inCondition.IsNot ? "1 = 0 /* IN [empty list] */" : "1 = 1 /* NOT IN [empty list] */";
            }
            
            var list = enumerable.Cast<object>().ToList();

            if (list.FirstOrDefault() is string)
            {
                string column = CompileStringColumn(inCondition.Column);

                string inOperator = inCondition.IsNot ? "NOT IN" : "IN";
                string values = Parameterize(ctx, inCondition.Values);
                return column + " " + inOperator + " (" + values + ")";
            }
        }

        return base.CompileCondition(ctx, clause);
    }

    private string CompileStringColumn(string column)
    {
        return $"to_char(substr({Wrap(column)}, 0, 1999))";
    }
}
