using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Connectiv.XrmCommon.Core.EarlyBound
{
    [Obsolete]
    public class EarlyBoundEntityBase<TEntity, TFields> : EarlyBoundEntityBase<TFields> where TEntity : Entity where TFields : Enum
    {
        public new String LogicalName { get; set; }

        public EarlyBoundEntityBase() : base(EntityLogicalName)
        {
            LogicalName = EntityLogicalName;
        }

        public EarlyBoundEntityBase(Guid id) : base(EntityLogicalName, id)
        {
            LogicalName = EntityLogicalName;
        }

        new public Guid Id
        {
            get
            {
                return GetAttributeValue<Guid>(primaryIdAttributeName);
            }
            set
            {
                SetAttributeValue(value, primaryIdAttributeName);
            }
        }

        public static String EntityLogicalName
        {
            get
            {
                return typeof(TEntity).Name.ToLower();
            }
        }

        protected override string primaryIdAttributeName { get { return $"{EntityLogicalName.ToLower()}id"; } }
    }


    public abstract class EarlyBoundEntityBase<TFields> : EarlyBoundEntityBase where TFields : Enum
    {
        public EarlyBoundEntityBase(String logicalName) : base(logicalName)
        {

        }

        public EarlyBoundEntityBase(String logicalName, Guid id) : base(logicalName, id)
        {

        }

        public T GetAttributeValue<T>(TFields key)
        {
            return GetAttributeValue<T>(key.ToString());
        }

        public EarlyBoundEntityBase<TFields> AddAttribute(TFields key, Object value)
        {
            this[key] = value;
            return this;
        }

        public EarlyBoundEntityBase<TFields> RemoveAttribute(TFields key)
        {
            this.RemoveAttribute(key.ToString().ToLower());

            return this;
        }

        public Boolean Contains(TFields key)
        {
            return base.Contains(key.ToString().ToLower());
        }

        public Object this[TFields key]
        {
            get
            {
                return this[key.ToString().ToLower()];
            }
            set
            {
                this[key.ToString().ToLower()] = value;
            }
        }
    }

    public abstract class EarlyBoundEntityBase : Entity
    {
        protected abstract String primaryIdAttributeName { get; }

        public EarlyBoundEntityBase(String logicalname) : base(logicalname)
        {

        }

        public EarlyBoundEntityBase(String logicalname, Guid id) : base(logicalname, id)
        {

        }

        protected static string logicalname = null;

        new public String LogicalName
        {
            get
            {
                return logicalname ?? (String.IsNullOrWhiteSpace((logicalname = GetType().Name.ToString().ToLower())) ? base.LogicalName : logicalname);
            }
        }

        new public T GetAttributeValue<T>([CallerMemberName] String attributeName = null)
        {
            if (String.IsNullOrWhiteSpace(attributeName)) throw new ArgumentNullException(nameof(attributeName));

            return base.GetAttributeValue<T>(attributeName.ToLower());
        }

        public void SetAttributeValue(Object value, [CallerMemberName] String attributeName = null)
        {
            if (String.IsNullOrWhiteSpace(attributeName)) throw new ArgumentNullException(nameof(attributeName));

            this[attributeName.ToLower()] = value;
        }
    }
}
