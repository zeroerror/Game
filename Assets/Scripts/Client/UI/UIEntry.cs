using Game.UI.Controller;
using Game.UI.Event;

namespace Game.UI
{

    public static class UIEntry
    {

        static UIController uIController;

        public static void Ctor()
        {
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
            // == Event
            UIEventCenter.TearDown();
            // == Controller
            uIController.TearDown();
        }

    }

}