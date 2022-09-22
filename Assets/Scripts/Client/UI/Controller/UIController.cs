using Game.UI.Event;
using Game.UI.Manager;
using Game.Client.Bussiness.EventCenter;
using UnityEngine;

namespace Game.UI.Controller
{

    public class UIController
    {

        public UIController(){

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
            while (UIEventCenter.TryDequeueOpenQueue(out string open_uiName))
            {
                UIManager.OpenUI(open_uiName);
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