
using System.Collections.Generic;
using Game.Infrastructure.Input;
using UnityEngine;

namespace Game.Client.Bussiness.Interfaces
{

    public interface ICameraTrackObj
    {

        Vector3 SelfPos { get; }
        Transform CamTrackObjTrans { get; }

    }

}