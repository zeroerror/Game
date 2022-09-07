using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.WorldBussiness
{

    public class HookerEntity : BulletEntity
    {

        Transform masterTrans;
        WorldRoleEntity masterEntity;
        public WorldRoleEntity MasterEntity => masterEntity;
        public void SetMaster(WorldRoleEntity roleEntity)
        {
            masterEntity = roleEntity;
            this.masterTrans = roleEntity.transform;
        }

        GameObject masterGrabEnd;//钩爪持有者一端
        public void SetMasterGrabPoint(Transform grabPoint) => this.masterGrabEnd.transform.SetParent(grabPoint, false);

        GameObject shootEnd => this.gameObject;//钩爪持有者一端
        public GameObject ShootEnd => shootEnd;//发射出去的一端

        public GameObject GrabPoint { get; private set; }

        LineRenderer lineRenderer;

        protected override void Init()
        {
            moveComponent.SetSpeed(30f);//Hooker速度
            moveComponent.SetGravity(0f);//重力加速度
            SetLifeTime(2f); //Hooker生命周期
            masterGrabEnd = new GameObject();
            lineRenderer = new LineRenderer();
        }

        public bool TickHooker(out float curForce)
        {
            curForce = 0;
            // lineRenderer.positionCount = 2;
            // lineRenderer.SetPositions(new Vector3[] { masterGrabEnd.transform.position, shootEnd.transform.position });
            if (GrabPoint != null)
            {
                shootEnd.transform.position = GrabPoint.transform.position;
                var grabPos = GrabPoint.transform.position;
                var masterGrabEndPos = masterGrabEnd.transform.position;
                var dis = Vector3.Distance(grabPos, masterGrabEndPos);
                if (dis < 5f)
                {
                    Debug.Log("钩爪断开");
                    SetLifeTime(0f);

                    return false;
                }
                curForce = dis > 10f ? 10f : dis;
                return true;
            }

            return false;
        }

        public override void TearDown()
        {
            Destroy(shootEnd);
            Destroy(masterGrabEnd);
            Destroy(GrabPoint);
        }

        public override void EnterTrigger(Collider collision)
        {
            var go = collision.gameObject;
            var layer = go.layer;
            if (layer == LayerMask.NameToLayer("Player"))
            {
                Debug.Log($"plaer: {go.name}");
                var roleEntity = go.GetComponent<WorldRoleEntity>();
                if (roleEntity.WRid == masterEntity.WRid) return;
            }

            Debug.Log("钩爪击中！");
            moveComponent.isPersistentMove = false;
            moveComponent.SetVelocity(Vector3.zero);

            var shootEndTrans = this.shootEnd.transform;

            var targetTrans = collision.gameObject.transform;

            this.shootEnd.transform.localScale = new Vector3(2, 2, 2);
            GrabPoint = new GameObject("爪钩击中点");
            GrabPoint.transform.position = this.shootEnd.transform.position;
            GrabPoint.transform.SetParent(targetTrans, true);

            SetLifeTime(5f);
        }

    }

}