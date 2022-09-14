using System.Collections.Generic;
using System.Data;
using Game.Infrastructure.Generic;
using Game.Protocol.Login;
using Game.Server.Bussiness.LoginBussiness.Facades;
using MySql.Data.MySqlClient;
using UnityEngine;

namespace Game.Server.Bussiness.LoginBussiness
{

    public class LoginController
    {
        public class LoginEvent
        {
            public int connID;
            public LoginReqMessage msg;
        }

        public class RegistEvent
        {
            public int connID;
            public string name;
            public string pwd;
        }

        LoginFacades loginFacades;
        List<LoginEvent> loginEventList;
        List<RegistEvent> registEventList;

        public LoginController()
        {
            loginEventList = new List<LoginEvent>();
            registEventList = new List<RegistEvent>();
        }

        public void Inject(LoginFacades loginFacades)
        {
            this.loginFacades = loginFacades;

            LoginListener();
            RegisterAccountListener();
        }

        public void Tick()
        {
            Tick_Login();

            Tick_RegistAccount();
        }

        void Tick_Login()
        {
            var e1 = loginEventList.GetEnumerator();
            if (e1.MoveNext())
            {
                var ev = e1.Current;
                loginEventList.Remove(ev);
                var msg = ev.msg;

                if (msg.name == null) return;
                if (msg.pwd == null) return;

                byte status = 0;
                // MYSQL OPARATION
                try
                {
                    // var reader = MySqlHelper.ExecuteReader(DatabaseConfig.ConnStr, "select * from account");
                    // while (reader.GetEnumerator().MoveNext())
                    // {
                    //     string name = reader.GetString("name");
                    //     string pwd = reader.GetString("pwd");
                    //     if (name == msg.name && pwd == msg.pwd)
                    //     {
                    //         status = 1;
                    //         break;
                    //     }
                    // }
                }
                catch (MySqlException exc)
                {
                    Debug.Log(exc.ToString());
                }

                status = 1;// Temp
                var networkServer = loginFacades.NetworkServer;
                networkServer.SendMsg<LoginResMessage>(ev.connID, new LoginResMessage
                {
                    status = status,
                    userToken = status != 0 ? System.DateTime.Now.ToString() : string.Empty
                });
            }
        }

        void Tick_RegistAccount()
        {
            var e = registEventList.GetEnumerator();
            if (e.MoveNext())
            {
                var ev = e.Current;
                registEventList.Remove(ev);
                if (ev.name == string.Empty) return;
                if (ev.pwd == string.Empty) return;

                byte status = 1;
                // MYSQL OPARATION
                try
                {
                    var reader = MySqlHelper.ExecuteReader(DatabaseConfig.ConnStr, "select * from account");
                    while (reader.GetEnumerator().MoveNext())
                    {
                        string name = reader.GetString("name");
                        string pwd = reader.GetString("pwd");
                        if (name == ev.name)
                        {
                            status = 0;
                            break;
                        }
                    }
                    if (status == 1)
                    {
                        Debug.Log($"MySQL插入game_account表 value('{ev.name}','{ev.pwd}')============================================");
                        reader = MySqlHelper.ExecuteReader(DatabaseConfig.ConnStr, $"insert into account value('{ev.name}','{ev.pwd}')");
                    }
                }
                catch (MySqlException exc)
                {
                    Debug.Log(exc.ToString());
                }

                // RESPONSE TO CLIENT
                var networkServer = loginFacades.NetworkServer;
                networkServer.SendMsg<RegisterAccountResMessage>(ev.connID, new RegisterAccountResMessage
                {
                    status = status
                });
            }
        }

        #region [Server Listener]
        void LoginListener()
        {
            var networkServer = loginFacades.NetworkServer;
            networkServer.AddRegister<LoginReqMessage>((connId, msg) =>
            {
                Debug.Log($"服务端: 账户登录请求 connId:{connId}  account:{msg.name}  pwd:{msg.pwd}");
                lock (loginEventList)
                {
                    loginEventList.Add(new LoginEvent
                    {
                        connID = connId,
                        msg = msg
                    });
                }
            });
        }

        void RegisterAccountListener()
        {
            var networkServer = loginFacades.NetworkServer;
            networkServer.AddRegister<RegisterAccountReqMessage>((connId, msg) =>
            {
                Debug.Log($"服务端: 账户注册请求 name:{msg.name}  pwd:{msg.pwd} ");
                lock (registEventList)
                {
                    registEventList.Add(new RegistEvent
                    {
                        connID = connId,
                        name = msg.name,
                        pwd = msg.pwd
                    });
                }
            });
        }

        #endregion

    }


}