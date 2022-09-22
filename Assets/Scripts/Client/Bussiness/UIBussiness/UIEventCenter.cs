using System.Collections.Generic;

namespace Game.Bussiness.UIBussiness
{

    public struct OpenEventModel
    {
        public string uiName;
        public object[] args;
    }

    public class UIEventCenter
    {

        Queue<OpenEventModel> uiOpenQueue;
        public void EnqueueOpenQueue(OpenEventModel openEventModel) => uiOpenQueue.Enqueue(openEventModel);
        public bool TryDequeueOpenQueue(out OpenEventModel openEventModel) => uiOpenQueue.TryDequeue(out openEventModel);

        Queue<string> uiTearDownQueue;
        public void EnqueueTearDownQueue(string uiName) => uiTearDownQueue.Enqueue(uiName);
        public bool TryDequeueTearDownQueue(out string uiName) => uiTearDownQueue.TryDequeue(out uiName);

        public UIEventCenter()
        {
            uiOpenQueue = new Queue<OpenEventModel>();
            uiTearDownQueue = new Queue<string>();
        }

        public void TearDown()
        {
            uiOpenQueue = null;
            uiTearDownQueue = null;
        }

    }

}