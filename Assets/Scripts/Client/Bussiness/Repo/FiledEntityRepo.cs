using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.Repo
{

    public class FiledEntityRepo
    {

        Dictionary<byte, FieldEntity> dic;

        PhysicsScene curPhysicsScene;
        public PhysicsScene CurPhysicsScene => curPhysicsScene;
        public void SetPhysicsScene(PhysicsScene scene) => this.curPhysicsScene = scene;

        public FieldEntity CurFieldEntity { get; private set; }

        public FiledEntityRepo()
        {
            dic = new Dictionary<byte, FieldEntity>();
        }

        public void Add(FieldEntity entity)
        {
            dic.Add(entity.FieldId, entity);
            CurFieldEntity = entity;
        }

        public FieldEntity Get(byte id)
        {
            dic.TryGetValue(id, out FieldEntity entity);
            return entity;
        }

    }


}