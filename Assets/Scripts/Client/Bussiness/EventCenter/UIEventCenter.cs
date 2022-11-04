using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Bussiness.EventCenter
{

    public struct OpenEventModel
    {
        public string uiName;
        public object[] args;
    }

    public static class UIEventCenter
    {

        // UI界面打开事件
        static Queue<OpenEventModel> uiOpenQueue;
        public static void AddToOpen(OpenEventModel openEventModel) => uiOpenQueue.Enqueue(openEventModel);
        public static bool TryDequeueOpenQueue(out OpenEventModel openEventModel) => uiOpenQueue.TryDequeue(out openEventModel);

        // UI界面关闭事件
        static Queue<string> uiTearDownQueue;
        public static void AddToTearDown(string uiName) => uiTearDownQueue.Enqueue(uiName);
        public static bool TryDequeueTearDownQueue(out string uiName) => uiTearDownQueue.TryDequeue(out uiName);

        // UI Trigger
        public static Action<string, string> LoginAction;       // 登录
        public static Action<string, string> RegistAction;       // 注册
        public static Action<string, ushort> ConnWorSerAction;// 连接世界服
        public static Action<string> WorldRoomCreateAction;   // 创建世界服内的房间
        public static Action<string, ushort> WorldRoomEnter;   // 进入战斗服房间
        public static Action<Vector2> MoveAction;   // 战斗移动操作
        public static Action PickAction;   // 战斗拾取操作
        public static Action<Vector2> ShootAction;   // 战斗射击操作
        public static Action StopShootAction;   // 战斗射击操作
        public static Action ReloadAction;   // 战斗换弹操作
        public static Action JumpAction;   // 战斗跳跃操作
        public static Action DropWeaponAction;   // 战斗丢弃操作

        // UI Listen
        public static Action<int, int> KillAndDamageInfoUpdateAction;       // 击杀及伤害

        public static void Ctor()
        {
            uiOpenQueue = new Queue<OpenEventModel>();
            uiTearDownQueue = new Queue<string>();
        }

        public static void TearDown()
        {
            uiOpenQueue = null;
            uiTearDownQueue = null;
        }

    }

}