using System;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness;

namespace Game.Client.Bussiness.EventCenter
{

    /// <summary>
    /// 跨Bussiness的事件通信中心
    /// </summary>
    public static class LocalEventCenter
    {
        // == BattleBussiness ==
        // BattleChoose Scene Loaded
        // static Action sceneLoaded_Action;
        // static Action<string> sceneLoaded_Handler;
        // public static void Regist_SceneLoadedHandler(Action<string> action) => sceneLoaded_Handler += action;
        // public static void Invoke_SceneLoadedHandler(string name) => sceneLoaded_Action = () =>
        //     {
        //         if (sceneLoaded_Handler == null) return;
        //         var list = sceneLoaded_Handler.GetInvocationList();
        //         for (int i = 0; i < list.Length; i++)
        //         {
        //             var action = list[i];
        //             action.DynamicInvoke(name);
        //         }
        //     };

        // static Action battleRoleSpawn_Action;
        // static Action<BattleRoleLogicEntity> battleRoleSpawn_Handler;
        // public static void Regist_BattleRoleSpawnHandler(Action<BattleRoleLogicEntity> action) => battleRoleSpawn_Handler += action;
        // public static void Invoke_BattleRoleSpawnHandler(BattleRoleLogicEntity entity) => battleRoleSpawn_Action = () =>
        //     {
        //         if (battleRoleSpawn_Handler == null) return;
        //         var list = battleRoleSpawn_Handler.GetInvocationList();
        //         for (int i = 0; i < list.Length; i++)
        //         {
        //             var action = list[i];
        //             action.DynamicInvoke(entity);
        //         }
        //     };

        public static void Ctor()
        {
        }

        public static void Tick()
        {
            // if (sceneLoaded_Action != null)
            // {
            //     Debug.Log("场景加载------------------------------");
            //     sceneLoaded_Action.Invoke();
            //     sceneLoaded_Action = null;
            // }

            // if (battleRoleSpawn_Action != null)
            // {
            //     Debug.Log("战斗角色生成-----------------------------");
            //     battleRoleSpawn_Action.Invoke();
            //     battleRoleSpawn_Action = null;
            // }
        }

        public static void TearDown()
        {

        }

    }

}