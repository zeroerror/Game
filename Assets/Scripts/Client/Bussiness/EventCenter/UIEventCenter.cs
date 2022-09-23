using System;
using System.Collections.Generic;

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
        public static void EnqueueOpenQueue(OpenEventModel openEventModel) => uiOpenQueue.Enqueue(openEventModel);
        public static bool TryDequeueOpenQueue(out OpenEventModel openEventModel) => uiOpenQueue.TryDequeue(out openEventModel);

        // UI界面关闭事件
        static Queue<string> uiTearDownQueue;
        public static void EnqueueTearDownQueue(string uiName) => uiTearDownQueue.Enqueue(uiName);
        public static bool TryDequeueTearDownQueue(out string uiName) => uiTearDownQueue.TryDequeue(out uiName);

        // 打开登录界面
        public static Action<string, string> LoginAction;
        public static Action<string, string> RegistAction;

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