using UnityEngine;

namespace Game.Generic
{

    public static class VectorExtension
    {

        public static bool Equals(this Vector3 v, Vector3 compareV, int decimalCount)
        {
            int num = 1;
            for (int i = 0; i < decimalCount; i++)
            {
                num *= 10;
            }

            v *= num;
            compareV *= num;

            int posX1 = (int)v.x;
            int posY1 = (int)v.y;
            int posZ1 = (int)v.z;
            int posX2 = (int)compareV.x;
            int posY2 = (int)compareV.y;
            int posZ2 = (int)compareV.z;
            return posX1 == posX2 && posY1 == posY2 && posZ1 == posZ2;
        }

    }

}