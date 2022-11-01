using UnityEngine;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class HookerEntity : BulletEntity
    {

        GameObject masterGrabEnd;//钩爪持有者一端
        public void SetMasterGrabPoint(Transform grabPoint) => this.masterGrabEnd.transform.SetParent(grabPoint, false);

        GameObject shootEnd => this.gameObject;//钩爪持有者一端
        public GameObject ShootEnd => shootEnd;//发射出去的一端

        public GameObject GrabPoint { get; private set; }

        LineRenderer lineRenderer;

        protected override void Init()
        {
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

        public void TryGrabSomthing(Transform hitTrans)
        {
            if (hitTrans == null) return;
            if (GrabPoint != null) return;

            Debug.Log("钩爪击中！");
            locomotionComponent.SetPersistentMove(false);
            locomotionComponent.SetVelocity(Vector3.zero);

            var shootEndTrans = ShootEnd.transform;
            shootEndTrans.localScale = new Vector3(2, 2, 2);
            GrabPoint = new GameObject("爪钩击中点");
            GrabPoint.transform.position = shootEndTrans.position;
            GrabPoint.transform.SetParent(hitTrans, true);

            SetLifeTime(5f);
        }

        public void TryGrabPosition(Vector3 pos)
        {
            Debug.Log("钩爪击中！");
            if (GrabPoint != null) return;

            locomotionComponent.SetPersistentMove(false);
            locomotionComponent.SetVelocity(Vector3.zero);

            var shootEndTrans = ShootEnd.transform;
            shootEndTrans.position = pos;
            shootEndTrans.localScale = new Vector3(2, 2, 2);
            GrabPoint = new GameObject("爪钩击中点");
            GrabPoint.transform.position = shootEndTrans.position;

            SetLifeTime(5f);
        }

        public override void TearDown()
        {
            Destroy(shootEnd);
            Destroy(masterGrabEnd);
            Destroy(GrabPoint);
            Debug.Log($"爪钩断开！！！");
        }

    }

}