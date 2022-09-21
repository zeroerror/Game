using System;
using Game.Protocol.Login;

namespace Game.Client.Bussiness.EventCenter
{

    public static class NetworkEventCenter
    {
        // == LoginBussiness ==
        static bool loginSuccessHandlerTrigger = false;
        static Action<LoginResMessage> loginSuccessHandler;
        public static LoginResMessage loginResMessage;
        public static void RegistLoginSuccess(Action<LoginResMessage> action) => loginSuccessHandler += action;
        public static void SetLoginSuccess(LoginResMessage msg)
        {
            loginSuccessHandlerTrigger = true;
            loginResMessage = msg;
        }

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

        static void InvokeLoginSuccessHandler(Action<LoginResMessage> handler)
        {
            var list = handler.GetInvocationList();
            for (int i = 0; i < list.Length; i++)
            {
                var func = list[i];
                func.DynamicInvoke(loginResMessage);
            }
        }

    }

}