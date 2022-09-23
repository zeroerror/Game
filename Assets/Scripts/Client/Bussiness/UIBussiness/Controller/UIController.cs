using UnityEngine;
using Game.Client.Bussiness.EventCenter;

namespace Game.Bussiness.UIBussiness
{

    public class UIController
    {

        public UIController()
        {
        }

        public void Tick()
        {
            Tick_Open();
            Tick_TearDown();
        }

        public void TearDown()
        {

        }

        void Tick_Open()
        {
            while (UIEventCenter.TryDequeueOpenQueue(out var openEventModel))
            {
                var uIBehavior = UIManager.OpenUI(openEventModel.uiName, openEventModel.args);
            }
        }

        void Tick_TearDown()
        {
            if (UIEventCenter.TryDequeueTearDownQueue(out string uiName))
            {
                UIManager.CloseUI(uiName);
            }
        }

    }

}