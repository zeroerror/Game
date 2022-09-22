using UnityEngine;

namespace Game.Bussiness.UIBussiness
{

    public static class UIEntry
    {

        static UIEventCenter uIEventCenter;
        static UIController uIController;

        public static void Ctor()
        {
            // == Event
            uIEventCenter = new UIEventCenter();
            uIEventCenter.EnqueueOpenQueue(new OpenEventModel { uiName = "Home_LoginPanel" });

            // == Controller
            uIController = new UIController();
            uIController.Inject(uIEventCenter);
        }

        public static void Tick()
        {

            // == Controller
            uIController.Tick();

        }

        public static void TearDown()
        {
            // == Event
            uIEventCenter.TearDown();
            // == Controller
            uIController.TearDown();
        }

    }

}