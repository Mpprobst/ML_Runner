using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedRunner.TerrainGeneration
{
    [System.Serializable]
    public class PlayerStats
    {
        public string profileName;
        public Obstacle[] obstacles;
        public List<float> scores;

        [System.Serializable]
        public class Obstacle
        {
            public string name;
            public int successes;
            public int failures;

            public Obstacle(string obstacleName)
            {
                name = obstacleName;
                successes = 0;
                failures = 0;
            }
        }

        public PlayerStats(string characterName)
        {
            profileName = characterName;

            List<Obstacle> obList = new List<Obstacle>();
            // IF NEW OBSTACLE NEEDS TO BE TRACKED, ADD IT HERE
            
            obList.Add(new Obstacle("1_SingleBlock"));
            obList.Add(new Obstacle("2_DoubleBlock"));
            obList.Add(new Obstacle("2_DoubleBlockSpike"));
            obList.Add(new Obstacle("3_TripleBlock"));
            obList.Add(new Obstacle("4_QuadBlock"));

            obstacles = new Obstacle[obList.Count];

            for (int i = 0; i < obList.Count; i++)
            {
                obstacles[i] = obList[i];
            }

            scores = new List<float>();
        }
    }
}
