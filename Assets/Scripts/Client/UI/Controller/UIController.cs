using Game.UI.Event;
using Game.UI.Manager;
using Game.Client.Bussiness.EventCenter;

namespace Game.UI.Controller
{

    public static class UIController
    {

        public static void Ctor()
        {
            NetworkEventCenter.RegistLoginSuccess(CloseLoginPanel);
        }

        public static void Tick()
        {
            Tick_Open();
            Tick_TearDown();
        }

        public static void TearDown()
        {

        }

        static void Tick_Open()
        {
            if (UIEventCenter.TryDequeueOpenQueue(out string uiName))
            {
                UIManager.OpenUI(uiName);
            }
        }

        static void Tick_TearDown()
        {
            if (UIEventCenter.TryDequeueTearDownQueue(out string uiName))
            {
                UIManager.CloseUI(uiName);
            }
        }

        static void CloseLoginPanel()
        {
            UIManager.CloseUI("Home_LoginPanel");
        }

    }

}