using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xrm.Sdk.Query
{
    public static class ExpressionExtensions
    {
        public static string ExtractPropertyName(this LambdaExpression memberExpression)
        {
            return ((memberExpression.Body as MemberExpression) ?? ((memberExpression.Body as UnaryExpression).Operand as MemberExpression))?.Member?.Name?.ToLower();
        }
    }

    public static class ConditionExpressionExtensions
    {
        public static ConditionExpression FromEntity<TEntity>(this ConditionExpression expression, Expression<Func<TEntity, object>> memberExpression, ConditionOperator conditionoperator, object value) where TEntity : Entity, new()
        {
            return expression.Set(
                v => v.Operator = conditionoperator,
                v => v.EntityName = new TEntity().LogicalName,
                v => v.AttributeName = memberExpression.ExtractPropertyName(),
                v => v.Values.Add(value)
            );
        }
    }

    public static class ColumnSetExtensions
    {
        public static ColumnSet FromEntity<TEntity>(this ColumnSet columnSet, params Expression<Func<TEntity, object>>[] memberExpressions) where TEntity : Entity
        {
            (columnSet = columnSet ?? new ColumnSet()).AllColumns = false;
            foreach (var memberExpression in memberExpressions)
            {
                string columnName = memberExpression.ExtractPropertyName();
                if (!String.IsNullOrWhiteSpace(columnName))
                {
                    columnSet.AddColumn(columnName);
                }
            }
            return columnSet;
        }
    }
}
