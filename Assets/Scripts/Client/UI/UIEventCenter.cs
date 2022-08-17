using System.Collections.Generic;

namespace Game.UI.Event
{

    public static class UIEventCenter
    {
        static Queue<string> uiOpenQueue;
        public static void EnqueueOpenQueue(string uiName) => uiOpenQueue.Enqueue(uiName);
        public static bool TryDequeueOpenQueue(out string uiName) => uiOpenQueue.TryDequeue(out uiName);

        static Queue<string> uiTearDownQueue;
        public static void EnqueueTearDownQueue(string uiName) => uiTearDownQueue.Enqueue(uiName);
        public static bool TryDequeueTearDownQueue(out string uiName) => uiTearDownQueue.TryDequeue(out uiName);

        public static void Ctor()
        {
            uiOpenQueue = new Queue<string>();
            uiTearDownQueue = new Queue<string>();
        }

        public static void TearDown()
        {
            uiOpenQueue = null;
            uiTearDownQueue = null;
        }

        public static bool TryDequeue<T>(this Queue<T> queue, out T t)
        {
            t = queue.Dequeue();
            return t != null;
        }


    }

}