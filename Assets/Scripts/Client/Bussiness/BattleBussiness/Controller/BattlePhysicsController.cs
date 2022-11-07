using Game.Client.Bussiness.BattleBussiness.Facades;

namespace Game.Client.Bussiness.BattleBussiness.Controller
{

    public class BattlePhysicsController
    {

        BattleFacades battleFacades;

        public BattlePhysicsController() { }

        public void Inject(BattleFacades battleFacades)
        {
            this.battleFacades = battleFacades;
        }

        public void Tick(float fixedDeltaTime)
        {
            // == Physics Simulation
            Tick_Physics_AllPhysicsEntity(fixedDeltaTime);
            var physicsScene = battleFacades.Repo.FieldRepo.CurPhysicsScene;
            physicsScene.Simulate(fixedDeltaTime);

            // == Physics Collision(Only For Client Performances Like Hit Effect,etc.)
            Tick_Physics_AllCollisions(fixedDeltaTime);
        }

        void Tick_Physics_AllPhysicsEntity(float fixedDeltaTime)
        {
            var roleDomain = battleFacades.Domain.RoleDomain;
            roleDomain.Tick_Physics_AllRoles(fixedDeltaTime);

            var bulletLogicDomain = battleFacades.Domain.BulletLogicDomain;
            bulletLogicDomain.Tick_Physics_AllBullets(fixedDeltaTime);

            var airdropDomain = battleFacades.Domain.AirdropDomain;
            airdropDomain.Tick_Physics_AllAirdrops(fixedDeltaTime);
        }

        void Tick_Physics_AllCollisions(float fixedDeltaTime)
        {
            var physicsDomain = battleFacades.Domain.PhysicsDomain;
            physicsDomain.Tick_Physics_Collections_Role_Field();
            physicsDomain.Tick_Physics_Collections_Bullet_Field();
            physicsDomain.Tick_Physics_Collections_Airdrop_Field();
        }

    }


}