using UnityEngine;

namespace Game.Library
{

    [CreateAssetMenu(fileName = "evolve_template_model", menuName = "Game/Template/" + nameof(EvolveTM))]
    public class EvolveTM : ScriptableObject
    {

        public int modelID;
        public string modelName;

        public int addHealth;
        public float addDamageCoefficient;
        public int addSpeed;

    }

}