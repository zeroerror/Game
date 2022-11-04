using System.Collections.Generic;
using UnityEngine;
using Game.Client.Bussiness.BattleBussiness.Generic;

namespace Game.Client.Bussiness.BattleBussiness
{

    public class BattleLeagueService
    {

        public class League
        {
            public int registId;    // - 联盟注册Id
            public List<IDComponent> members;   // - 联盟盟友
        }

        List<League> leagueList;

        int autoId=10000;

        public BattleLeagueService()
        {
            leagueList = new List<League>();
        }

        // 创建联盟(至少需要2个人)
        public bool TryCreateLeague(IDComponent ally1, IDComponent ally2)
        {
            if (ally1.HasLeague() || ally2.HasLeague())
            {
                Debug.LogWarning("其中一人已经结盟了");
                return false;
            }

            League league = new League();
            league.registId = autoId;
            league.members = new List<IDComponent>();
            league.members.Add(ally1);
            league.members.Add(ally2);

            ally1.SetLeagueID(autoId);
            ally2.SetLeagueID(autoId);

            leagueList.Add(league);

            autoId++;
            return true;
        }

        // 加入联盟
        public bool TryJoinLeague(IDComponent ally, int leagueId)
        {
            if (ally.HasLeague())
            {
                Debug.LogWarning($" {ally.EntityType.ToString()} {ally.EntityID} 已经结盟了");
                return false;
            }

            if (!TryGetLeague(leagueId, out var league))
            {
                return false;
            }
            else
            {
                league.members.Add(ally);
            }

            return true;
        }

        bool TryGetLeague(int leagueId, out League league)
        {
            league = leagueList.Find((league => league.registId == leagueId));
            return league != null;
        }

    }

}