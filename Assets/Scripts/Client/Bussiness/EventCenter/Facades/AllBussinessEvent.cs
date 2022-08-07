

namespace Game.Client.Bussiness.EventCenter.Facades
{

    public static class AllBussinessEvent
    {
        public static LoginToWorldEvent LoginToWorldEvent { get; private set; }
        public static EnterWorldEvent EnterWorldEvent { get; private set; }

        public static void Ctor()
        {
            LoginToWorldEvent = new LoginToWorldEvent();
            EnterWorldEvent = new EnterWorldEvent();
        }
    }

}