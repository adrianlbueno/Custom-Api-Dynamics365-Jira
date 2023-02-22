using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xrm.Sdk
{
    public enum UpsertResponseState
    {
        Updated,
        Created
    }

    public class EntityUpsertResponse<TEntity> where TEntity : Entity
    {
        public UpsertResponseState State { get; }

        public TEntity Entity { get; }

        public EntityUpsertResponse(TEntity entity, UpsertResponseState state)
        {
            State = state;
            Entity = entity;
        }

        public static implicit operator TEntity(EntityUpsertResponse<TEntity> resp)
        {
            return resp.Entity;
        }
    }
}
