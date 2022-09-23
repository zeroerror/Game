using UnityEngine;
using Game.Bussiness.UIBussiness;

namespace Game.Client.Bussiness.EventCenter
{

    public static class UIEntry
    {

        static UIController uIController;

        public static void Ctor()
        {
            // == EventCenter
            UIEventCenter.Ctor();
            UIEventCenter.EnqueueOpenQueue(new OpenEventModel { uiName = "Home_LoginPanel" });
            
            // == Controller
            uIController = new UIController();
        }

        public static void Tick()
        {

            // == Controller
            uIController.Tick();

        }

        public static void TearDown()
        {
            // == EventCenter
            UIEventCenter.TearDown();

            // == Controller
            uIController.TearDown();
        }

    }

}