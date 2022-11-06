using System.Threading;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Generic;
using Game.Infrastructure.Generic;
using Game.Infrastructure.Network.Server;
using Game.Server.Bussiness.BattleBussiness.Facades;
using Game.Server.Bussiness.EventCenter;

namespace Game.Server.Bussiness.BattleBussiness
{

    public class BattleEntry
    {
        BattleServerFacades serverFacades;

        BattleController battleController;
        BattlePhysicsController battlePhysicsController;
        BattleNetworkController battleNetworkController;
        BattleWeaponController battleWeaponController;
        BattleLifeController battleLifeController;
        BattleArmorController battleArmorController;
        BattleBulletController battleBulletController;

        Thread _battleServerThread;

        public BattleEntry()
        {
            serverFacades = new BattleServerFacades();
            battleController = new BattleController();
            battlePhysicsController = new BattlePhysicsController();
            battleNetworkController = new BattleNetworkController();
            battleWeaponController = new BattleWeaponController();
            battleLifeController = new BattleLifeController();
            battleArmorController = new BattleArmorController();
            battleBulletController = new BattleBulletController();

            ServerNetworkEventCenter.Regist_BattleServerNeedCreate(StartBattleServer);
        }

        public void Inject(NetworkServer server)
        {
            // Facades
            serverFacades.Inject(server);

            // Conntroller
            battleController.Inject(serverFacades);
            battlePhysicsController.Inject(serverFacades);
            battleNetworkController.Inject(serverFacades);
            battleWeaponController.Inject(serverFacades);
            battleLifeController.Inject(serverFacades);
            battleArmorController.Inject(serverFacades);
            battleBulletController.Inject(serverFacades);
        }

        public void Tick(float fixedDeltaTime)
        {
            battleNetworkController.Tick();

            battleController.Tick(fixedDeltaTime);
            battlePhysicsController.Tick(fixedDeltaTime);
            battleWeaponController.Tick(fixedDeltaTime);
            battleLifeController.Tick(fixedDeltaTime);
            battleArmorController.Tick(fixedDeltaTime);
            battleBulletController.Tick(fixedDeltaTime);
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
            var battleServer = serverFacades.Network.BattleServer;
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

                // - Server Battle Load
                var battleFacades = serverFacades.BattleFacades;
                var gameEntity = battleFacades.GameEntity;
                var gameStage = gameEntity.Stage;
                var fsm = gameEntity.FSMComponent;
                var gameState = fsm.State;
                if (!gameStage.HasStage(BattleStage.LoadedLevel1) && gameState != BattleState.Loading)
                {
                    fsm.EnterGameState_BattleLoading(BattleStage.LoadedLevel1);
                }
            };
        }

    }

}