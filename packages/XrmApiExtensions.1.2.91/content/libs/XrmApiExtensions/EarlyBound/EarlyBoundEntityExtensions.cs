using Connectiv.XrmCommon.Core.EarlyBound;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xrm.Sdk
{
    [Obsolete]
    public static class EarlyBoundEntityExtensions
    {
        public static ColumnSet FromEntity<TEntity, TFields>(this ColumnSet columnSet, params TFields[] attributes) where TEntity : EarlyBoundEntityBase<TFields> where TFields : Enum
        {
            return new ColumnSet(attributes.Select(v => v.ToString().ToLower()).ToArray());
        }

        public static OrderExpression FromEntity<TEntity, TFields>(this OrderExpression orderExpression, TFields attribute, OrderType orderType) where TEntity : EarlyBoundEntityBase<TFields> where TFields : Enum
        {
            return new OrderExpression(attribute.ToString().ToLower(), orderType);
        }
    }

    [Obsolete]
    public class QueryExpression<TEntity, TFields> where TEntity : EarlyBoundEntityBase<TFields> where TFields : Enum
    {
        public String EntityName { get; set; } = typeof(TEntity).Name.ToLower();
        public List<OrderExpression<TFields>> Orders { get; set; } = new List<OrderExpression<TFields>>();
        public ColumnSet<TFields> ColumnSet { get; set; } = new ColumnSet<TFields>();
        public FilterExpression<TFields> Criteria { get; set; } = new FilterExpression<TFields>();
        public List<LinkEntity> LinkEntities = new List<LinkEntity>();
        public static implicit operator QueryExpression(QueryExpression<TEntity, TFields> queryExpression)
        {
            var qEx = new QueryExpression
            {
                ColumnSet = queryExpression.ColumnSet,
                Criteria = queryExpression.Criteria,
                EntityName = queryExpression.EntityName,
                Orders = { }
            };
            qEx.Orders.AddRange(queryExpression.Orders.Select(v => (OrderExpression)v));
            qEx.LinkEntities.AddRange(queryExpression.LinkEntities);
            return qEx;
        }
    }

    public class FilterExpression<TFields> : IEnumerable<ConditionExpression<TFields>> where TFields : Enum
    {
        public LogicalOperator FilterOperator { get; set; }
        public ConditionCollection<TFields> Conditions { get; } = new ConditionCollection<TFields>();

        public void Add(TFields field, ConditionOperator conditionoperator, object value)
        {
            Conditions.Add(field, conditionoperator, value);
        }

        public IEnumerator<ConditionExpression<TFields>> GetEnumerator()
        {
            return Conditions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Conditions.GetEnumerator();
        }

        public static implicit operator FilterExpression(FilterExpression<TFields> filterExpression)
        {
            var filt = new FilterExpression
            {
                FilterOperator = filterExpression.FilterOperator,
            };
            filt.Conditions.AddRange(filterExpression.Conditions.Select(v => (ConditionExpression)v));

            return filt;
        }
    }

    public class ConditionCollection<TFields> : List<ConditionExpression<TFields>> where TFields : Enum
    {
        public void Add(TFields field, ConditionOperator conditionoperator, object value)
        {
            this.Add(new ConditionExpression<TFields>(field, conditionoperator, value));
        }
    }

    public class ConditionExpression<TFields> where TFields : Enum
    {
        public TFields Field { get; }
        public ConditionOperator Conditionoperator { get; }
        public object Value { get; }

        public ConditionExpression(TFields field, ConditionOperator conditionoperator, Object value)
        {
            Field = field;
            Conditionoperator = conditionoperator;
            Value = value;
        }

        public static implicit operator ConditionExpression(ConditionExpression<TFields> condition)
        {
            return new ConditionExpression(condition.Field.ToString().ToLower(), condition.Conditionoperator, condition.Value);
        }
    }

    public class OrderExpression<TFields> where TFields : Enum
    {
        TFields Field { get; set; }
        OrderType OrderType { get; set; }

        public OrderExpression(TFields field, OrderType orderType)
        {
            Field = field;
            OrderType = orderType;
        }

        public static implicit operator OrderExpression(OrderExpression<TFields> order)
        {
            return new OrderExpression(order.Field.ToString().ToLower(), order.OrderType);
        }
    }

    public class ColumnSet<TFields> : IEnumerable<String> where TFields : Enum
    {
        protected List<String> attributes = new List<string>();

        public ColumnSet()
        {

        }

        public ColumnSet(IEnumerable<String> fields)
        {
            attributes.AddRange(fields);
        }

        public ColumnSet(params TFields[] cols)
        {
            attributes.AddRange(attributes.Select(v => v.ToString().ToLower()));
        }

        public void Add(TFields field)
        {
            String fieldname = field.ToString().ToLower();
            if (!attributes.Contains(fieldname))
                attributes.Add(fieldname);
        }

        public static implicit operator ColumnSet(ColumnSet<TFields> cols)
        {
            return new ColumnSet(cols.attributes.ToArray());
        }

        public IEnumerator<String> GetEnumerator()
        {
            return attributes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return attributes.GetEnumerator();
        }

        public ColumnSet<TFields> AddColumns(IEnumerable<string> additionalcolumns)
        {
            attributes.AddRange(additionalcolumns);
            return this;
        }
    }
}
