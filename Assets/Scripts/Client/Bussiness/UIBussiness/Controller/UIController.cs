using Game.Bussiness.UIBussiness;
using UnityEngine;

namespace Game.Bussiness.UIBussiness
{

    public class UIController
    {

        UIEventCenter uIEventCenter;

        public UIController()
        {
        }

        public void Inject(UIEventCenter uIEventCenter)
        {
            this.uIEventCenter = uIEventCenter;
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
            while (uIEventCenter.TryDequeueOpenQueue(out var openEventModel))
            {
                var uIBehavior = UIManager.OpenUI(openEventModel.uiName, uIEventCenter, openEventModel.args);
            }
        }

        void Tick_TearDown()
        {
            if (uIEventCenter.TryDequeueTearDownQueue(out string uiName))
            {
                UIManager.CloseUI(uiName);
            }
        }

    }

}