

namespace Game.Client.Bussiness.EventCenter
{

    public class EnterWorldEvent
    {
        // Trigger
        bool isTrigger;
        public bool IsTrigger => this.isTrigger;
        public void SetIsTrigger(bool flag) => isTrigger = flag;

        // Field
    }

}