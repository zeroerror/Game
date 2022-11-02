using UnityEngine;

namespace Game.Generic
{

    public static class DebugExtensions
    {

        public static void LogWithColor(string log, string color)
        {
            Debug.Log($"<color={color}>{log}</color>");
        }

    }

}
