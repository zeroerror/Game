using UnityEngine;
using Game.Client.Bussiness.UIBussiness;

namespace Game.Client.Bussiness.EventCenter
{

    public class UIEntry
    {

        UIController uIController;

        public UIEntry() { }
        
        public void Ctor()
        {
            // == EventCenter
            UIEventCenter.Ctor();
            UIEventCenter.AddToOpen(new OpenEventModel { uiName = "Home_LoginPanel" });

            // == Controller
            uIController = new UIController();
        }

        public void Tick()
        {

            // == Controller
            uIController.Tick();

        }

        public void TearDown()
        {
            // == EventCenter
            UIEventCenter.TearDown();

            // == Controller
            uIController.TearDown();
        }

    }

}