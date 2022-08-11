using Game.UI.Controller;
using Game.UI.Event;

namespace Game.UI
{

    public static class UIEntry
    {
        public static void Ctor()
        {
            // == Event
            UIEventCenter.Ctor();

            // == Controller
            UIController.Ctor();
        }

        public static void Tick()
        {
            // == Controller
            UIController.Tick();
        }

        public static void TearDown()
        {
            // == Event
            UIEventCenter.TearDown();
            // == Controller
            UIController.TearDown();
        }

    }

}