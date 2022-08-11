using System;

namespace Game.Client.Bussiness.EventCenter
{

    public static class NetworkEventCenter
    {
        // == LoginBussiness ==
        static bool loginSuccessHandlerTrigger = false;
        static Action loginSuccessHandler;
        public static Action RegistLoginSuccess(Action action) => loginSuccessHandler += action;
        public static void InvokeLoginSuccessHandler() => loginSuccessHandlerTrigger = true;

        public static void Ctor()
        {

        }

        public static void Tick()
        {
            Tick_loginSuccessHandlerTrigger();
        }

        static void Tick_loginSuccessHandlerTrigger()
        {
            if (loginSuccessHandlerTrigger)
            {
                loginSuccessHandlerTrigger = false;
                InvokeLoginSuccessHandler(loginSuccessHandler);
            }
        }

        static void InvokeLoginSuccessHandler(Action handler)
        {
            var list = handler.GetInvocationList();
            for (int i = 0; i < list.Length; i++)
            {
                var func = list[i];
                func.DynamicInvoke();
            }
        }

    }

}