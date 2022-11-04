using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class WeaponEntity : MonoBehaviour
    {
        IDComponent idComponent;
        public IDComponent IDComponent => idComponent;
        public void SetLeagueID(int v) => idComponent.SetLeagueID(v);
        public void SetEntityID(int v) => idComponent.SetEntityID(v);

        [SerializeField] WeaponType weaponType;
        public WeaponType WeaponType => weaponType;

        [SerializeField] int reloadFrame;
        public int ReloadFrame => reloadFrame;

        [SerializeField] int freezeMaintainFrame;
        public int FreezeMaintainFrame => freezeMaintainFrame;

        [SerializeField] int breakFrame;
        public int BreakFrame => breakFrame;

        [SerializeField] int bulletCapacity;
        public int BulletCapacity => bulletCapacity;

        [SerializeField] Transform firePoint;
        public Vector3 ShootPointPos => firePoint.position;

        [SerializeField] float damageCoefficient;
        public float DamageCoefficient => damageCoefficient;

        int curReloadingFrame;
        public int CurReloadingFrame => curReloadingFrame;
        public void ResetCurrentReloadingFrame() => curReloadingFrame = reloadFrame;
        public void ReduceCurReloadingFrame() => curReloadingFrame--;

        int masterEntityID;
        public int MasterID => masterEntityID;

        int bulletNum;
        public int BulletNum => bulletNum;
        public void SetBulletNum(int v) => bulletNum = v;

        bool hasMaster;
        public bool HasMaster => hasMaster;

        [SerializeField] BulletType bulletType;
        public BulletType BulletType => bulletType;

        AudioClip shootAudioClip;

        public void Ctor()
        {
            idComponent = new IDComponent();
            idComponent.SetEntityType(EntityType.Weapon);
            idComponent.SetSubType((byte)weaponType);
            shootAudioClip = transform.Find("audio_clip_shoot").GetComponent<AudioSource>().clip;
        }

        public void Clear()
        {
            hasMaster = false;
        }

        public void SetMaster(int masterWRid)
        {
            this.masterEntityID = masterWRid;
            hasMaster = true;
        }

        public void LoadBullet(int v)
        {
            bulletNum += v;
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

        public void AddDamageCoefficient(float v)
        {
            damageCoefficient += v;
        }

    }

}