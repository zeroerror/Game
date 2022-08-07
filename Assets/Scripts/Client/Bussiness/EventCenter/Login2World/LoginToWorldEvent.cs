

namespace Game.Client.Bussiness.EventCenter
{

    public class LoginToWorldEvent
    {
        // Trigger
        bool isTrigger;
        public bool IsTrigger => this.isTrigger;
        public void SetIsTrigger(bool flag) => isTrigger = flag;

        // Field
    }

}