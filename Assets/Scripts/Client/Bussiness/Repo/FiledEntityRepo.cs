using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.Repo
{

    public class FiledEntityRepo
    {

        Dictionary<byte, FieldEntity> dic;

        public FiledEntityRepo()
        {
            dic = new Dictionary<byte, FieldEntity>();
        }

        public void Add(FieldEntity entity)
        {
            dic.Add(entity.FieldId, entity);
        }

        public FieldEntity Get(byte id)
        {
            dic.TryGetValue(id, out FieldEntity entity);
            return entity;
        }

    }


}