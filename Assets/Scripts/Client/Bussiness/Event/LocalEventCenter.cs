using System;
using UnityEngine;
using Game.Client.Bussiness.WorldBussiness;

namespace Game.Client.Bussiness.EventCenter
{

    /// <summary>
    /// 跨Bussiness的事件通信中心
    /// </summary>
    public static class LocalEventCenter
    {
        // == WorldBussiness ==
        // WorldChoose Scene Loaded
        static Action sceneLoaded_Action;
        static Action<string> sceneLoaded_Handler;
        public static void Regist_SceneLoadedHandler(Action<string> action) => sceneLoaded_Handler += action;
        public static void Invoke_SceneLoadedHandler(string name) => sceneLoaded_Action = () =>
            {
                if (sceneLoaded_Handler == null) return;
                var list = sceneLoaded_Handler.GetInvocationList();
                for (int i = 0; i < list.Length; i++)
                {
                    var action = list[i];
                    action.DynamicInvoke(name);
                }
            };

        static Action worldRoleSpawn_Action;
        static Action<WorldRoleEntity> worldRoleSpawn_Handler;
        public static void Regist_WorldRoleSpawnHandler(Action<WorldRoleEntity> action) => worldRoleSpawn_Handler += action;
        public static void Invoke_WorldRoleSpawnHandler(WorldRoleEntity entity) => worldRoleSpawn_Action = () =>
            {
                if (worldRoleSpawn_Handler == null) return;
                var list = worldRoleSpawn_Handler.GetInvocationList();
                for (int i = 0; i < list.Length; i++)
                {
                    var action = list[i];
                    action.DynamicInvoke(entity);
                }
            };



        public static void Ctor()
        {
        }

        public static void Tick()
        {
            if (sceneLoaded_Action != null)
            {
                Debug.Log("sceneLoaded_Action");
                sceneLoaded_Action.Invoke();
                sceneLoaded_Action = null;
            }

            if (worldRoleSpawn_Action != null)
            {
                Debug.Log("worldRoleSpawn_Action");
                worldRoleSpawn_Action.Invoke();
                worldRoleSpawn_Action = null;
            }
        }

        public static void TearDown()
        {

        }

    }

}