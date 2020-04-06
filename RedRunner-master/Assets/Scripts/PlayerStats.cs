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

            public Obstacle(bool far, bool high, bool below, bool wide, bool enemy)
            {
                name = "";
                if (far)
                    name += "F";
                else
                    name += "S";

                if (high)
                    name += "H";
                else
                    name += "L";

                if (below)
                    name += "B";
                else
                    name += "A";

                if (wide)
                    name += "W";
                else
                    name += "N";

                if (enemy)
                    name += "E";
                else
                    name += "_";

                //Debug.Log("new block added: " + name);
                successes = 0;
                failures = 0;

            }
        }

        public PlayerStats(string characterName)
        {
            profileName = characterName;

            List<Obstacle> obList = new List<Obstacle>();
            // IF NEW OBSTACLE NEEDS TO BE TRACKED, ADD IT HERE
   
            obList.Add(new Obstacle("far"));
            obList.Add(new Obstacle("high"));
            obList.Add(new Obstacle("below"));
            obList.Add(new Obstacle("wide"));
            obList.Add(new Obstacle("enemy"));
            obstacles = new Obstacle[obList.Count];

            for (int i = 0; i < obList.Count; i++)
            {
                obstacles[i] = obList[i];
            }

            scores = new List<float>();
        }
    }
}
