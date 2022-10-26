using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class WeaponEntity : MonoBehaviour
    {
        IDComponent idComponent;
        public IDComponent IDComponent => idComponent;

        [SerializeField]
        WeaponType weaponType;
        public WeaponType WeaponType => weaponType;

        [SerializeField]
        int reloadFrame;
        public int ReloadFrame => reloadFrame;

        [SerializeField]
        int bulletCapacity;
        public int BulletCapacity => bulletCapacity;

        [SerializeField]
        Transform firePoint;
        public Vector3 ShootPointPos => firePoint.position;

        int curReloadingFrame;
        public int CurReloadingFrame => curReloadingFrame;
        public void ResetCurrentReloadingFrame() => curReloadingFrame = reloadFrame;
        public void ReduceCurReloadingFrame() => curReloadingFrame--;

        int masterEntityID;
        public int MasterEntityID => masterEntityID;

        public int bulletNum { get; private set; }
        public void LoadBullet(int bulletNum) => this.bulletNum += bulletNum;

        bool hasMaster;
        public bool HasMaster => hasMaster;

        public BulletType bulletType;

        AudioClip shootAudioClip;

        public void Ctor()
        {
            idComponent = new IDComponent();
            idComponent.SetEntityType(EntityType.Weapon);
            idComponent.SetSubType((byte)weaponType);
            shootAudioClip = transform.Find("audio_clip_shoot").GetComponent<AudioSource>().clip;
        }

        public void SetMaster(int masterWRid)
        {
            this.masterEntityID = masterWRid;
            hasMaster = true;
        }

        public void Clear()
        {
            var colliders = transform.GetComponentsInChildren<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                var c = colliders[i];
                c.enabled = true;
                c.isTrigger = true;
            }
            hasMaster = false;
        }

        public int TryShootBullet(int num)
        {
            // Debug.Log($"武器射击，所需子弹：{num} 当前拥有子弹：{bulletNum}");
            if (bulletNum >= num)
            {
                bulletNum -= num;
                return num;
            }
            else
            {
                return bulletNum;
            }
        }

        public int GetReloadBulletNum()
        {
            return bulletCapacity - bulletNum;
        }

        public void PlayShootAudio()
        {
            AudioSource.PlayClipAtPoint(shootAudioClip, ShootPointPos);
        }

    }

}