using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedRunner.TerrainGeneration
{
    [CreateAssetMenu(fileName = "PlayerStats", menuName = "ScriptableObjects/PlayerStats", order = 2)]
    public class PlayerStats : ScriptableObject
    {
        public Obstacle[] obstacles;

        [System.Serializable]
        public class Obstacle
        {
            public string name;
            public int successes;
            public int failures;
        }
    }
}
