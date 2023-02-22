using Microsoft.Xrm.Sdk.Query;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xrm.Sdk
{
    public class QueryDefinition
    {
        public List<String> Columns { get; set; } = new List<String>();
        public FilterExpression Criteria { get; set; } = new FilterExpression();
        public ConditionList Conditions { get; set; } = new ConditionList();
        public List<LinkEntity> LinkEntities { get; set; } = new List<LinkEntity>();
        public OrderList Orders { get; set; } = new OrderList();
        public int? TopCount { get; set; } = null;
        public bool AllColumns { get; set; } = false;

        public static implicit operator QueryExpression(QueryDefinition queryDefinition)
        {
            QueryExpression retVal = new QueryExpression
            {
                Criteria = queryDefinition.Criteria,
                ColumnSet = queryDefinition.AllColumns ? new ColumnSet(true) : new ColumnSet(queryDefinition.Columns.ToArray()),
                TopCount = queryDefinition.TopCount
            };

            if (queryDefinition.Conditions.Count > 0)
            {
                retVal.Criteria.FilterOperator = queryDefinition.Conditions.FilterOperator;
                retVal.Criteria.Conditions.AddRange(queryDefinition.Conditions);
            }
            retVal.Orders.AddRange(queryDefinition.Orders);
            retVal.LinkEntities.AddRange(queryDefinition.LinkEntities);

            return retVal;
        }
    }

    public class ConditionList : List<ConditionExpression>
    {
        public ConditionList()
        {

        }

        public ConditionList(LogicalOperator filteroperator)
        {
            FilterOperator = filteroperator;
        }

        public LogicalOperator FilterOperator { get; set; } = LogicalOperator.And;

        public void Add(string attributename, ConditionOperator conditionOperator)
        {
            Add(new ConditionExpression(attributename, conditionOperator));
        }

        public void Add(string attributename, ConditionOperator conditionOperator, object value)
        {
            Add(new ConditionExpression(attributename, conditionOperator, value));
        }

        public void Add(string attributename, ConditionOperator conditionOperator, params object[] values)
        {
            Add(new ConditionExpression(attributename, conditionOperator, values));
        }
    }

    public class OrderList : List<OrderExpression>
    {
        public void Add(string attributename, OrderType orderType)
        {
            Add(new OrderExpression(attributename, orderType));
        }

        public void Add(string attributename, OrderType orderType, string alias)
        {
            Add(new OrderExpression(attributename, orderType, alias));
        }
    }
}
