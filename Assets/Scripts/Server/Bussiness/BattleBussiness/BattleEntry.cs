using System.Threading;
using Game.Infrastructure.Generic;
using Game.Infrastructure.Network.Server;
using Game.Server.Bussiness.BattleBussiness.Facades;
using Game.Server.Bussiness.EventCenter;
using UnityEngine;

namespace Game.Server.Bussiness.BattleBussiness
{

    public class BattleEntry
    {
        BattleServerFacades battleFacades;

        BattleController battleController;
        BattlePhysicsController battlePhysicsController;
        BattleNetworkController battleNetworkController;
        BattleWeaponController battleWeaponController;
        BattleLifeController battleLifeController;

        Thread _battleServerThread;

        public BattleEntry()
        {
            battleFacades = new BattleServerFacades();
            battleController = new BattleController();
            battlePhysicsController = new BattlePhysicsController();
            battleNetworkController = new BattleNetworkController();
            battleWeaponController = new BattleWeaponController();
            battleLifeController = new BattleLifeController();

            ServerNetworkEventCenter.Regist_BattleServerNeedCreate(StartBattleServer);
        }

        public void Inject(NetworkServer server)
        {
            // Facades
            battleFacades.Inject(server);

            // Conntroller
            battleController.Inject(battleFacades);
            battlePhysicsController.Inject(battleFacades);
            battleNetworkController.Inject(battleFacades);
            battleWeaponController.Inject(battleFacades);
            battleLifeController.Inject(battleFacades);
        }

        public void Tick(float fixedDeltaTime)
        {
            battleNetworkController.Tick();

            battleController.Tick(fixedDeltaTime);
            battlePhysicsController.Tick(fixedDeltaTime);
            battleWeaponController.Tick();
            battleLifeController.Tick();
        }

        void StartBattleServer()
        {
            if (_battleServerThread != null)
            {
                Debug.Log($"战斗服已经启动了！！！");
                return;
            }

            var port = NetworkConfig.BATTLESERVER_PORT[0];
            Debug.Log($"战斗服启动！端口:{port}");
            var battleServer = battleFacades.Network.BattleServer;
            battleServer.StartListen(port);
            _battleServerThread = new Thread(() =>
            {
                while (true)
                {
                    battleServer.Tick();
                }
            });
            _battleServerThread.Start();

            battleServer.OnConnectedHandle += (connID) =>
            {
                Debug.Log($"[战斗服]: connID:{connID} 客户端连接成功-------------------------");
                ServerNetworkEventCenter.battleSerConnect.Invoke(connID);
            };
        }

    }

}