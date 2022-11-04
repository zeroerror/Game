using UnityEngine;
using TMPro;
using ZeroUIFrame;
using Game.Client.Bussiness.EventCenter;

namespace Game.Client.Bussiness.UIBussiness.Panel
{

    public class Home_BattleInfoPanel : UIBehavior
    {

        TMP_Text killText;
        TMP_Text damageText;

        void Awake()
        {
            UIEventCenter.KillAndDamageInfoUpdateAction += KillAndDamageInfoUpdate;
            var killTextTF = transform.Find("Info/KillNum");
            killText = killTextTF.GetComponent<TMP_Text>();
            killText.text = "0";
            var damageTextTF = transform.Find("Info/DamageNum");
            damageText = damageTextTF.GetComponent<TMP_Text>();
            damageText.text = "0";
        }

        void KillAndDamageInfoUpdate(int kill, int damage)
        {
            Debug.Log($"kill {kill}  damage {damage}");
            killText.text = kill.ToString();
            damageText.text = damage.ToString();
        }

    }

}