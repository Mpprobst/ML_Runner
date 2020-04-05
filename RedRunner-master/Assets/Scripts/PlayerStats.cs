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
                /*bool far = features[0];
                bool high = features[1];
                bool below = features[2];
                bool wide = features[3];
                bool enemy = features[4];*/

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
            /*
            obList.Add(new Obstacle("1_SingleBlock"));
            obList.Add(new Obstacle("2_DoubleBlock"));
            obList.Add(new Obstacle("2_DoubleBlockSpike"));
            obList.Add(new Obstacle("3_TripleBlock"));
            obList.Add(new Obstacle("4_QuadBlock"));*/

            // Far <-> High
            // Below -> !High
            obList.Add(new Obstacle(true,false,false,false,true));
            obList.Add(new Obstacle(true,false,false,true,true));
            obList.Add(new Obstacle(true,false,true,false,true));
            obList.Add(new Obstacle(true,false,true,true,true));

            obList.Add(new Obstacle(true,false,false,false,false));
            obList.Add(new Obstacle(true,false,false,true,false));
            obList.Add(new Obstacle(true,false,true, false, false));
            obList.Add(new Obstacle(true,false,true,true, false));

            obList.Add(new Obstacle(false, false, false, false, true));
            obList.Add(new Obstacle(false, false, false, true,true));

            obList.Add(new Obstacle(false, false, false, false, false));
            obList.Add(new Obstacle(false, false, false, true, false));

            obList.Add(new Obstacle(false, false, true,true,true));
            obList.Add(new Obstacle(false, false, true, false, true));

            obList.Add(new Obstacle(false, false, true,true, false));
            obList.Add(new Obstacle(false, false, true, false, false));

            obList.Add(new Obstacle(false, true, false, true,true));
            obList.Add(new Obstacle(false, true, false, false, true));

            obList.Add(new Obstacle(false, true, false, true, false));
            obList.Add(new Obstacle(false, true, false, false, false));

            obstacles = new Obstacle[obList.Count];

            for (int i = 0; i < obList.Count; i++)
            {
                obstacles[i] = obList[i];
            }

            scores = new List<float>();
        }
    }
}
