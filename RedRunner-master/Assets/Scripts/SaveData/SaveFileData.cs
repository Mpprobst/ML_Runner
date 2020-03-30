using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace RedRunner.TerrainGeneration
{
    [System.Serializable]
    public class SaveFileData
    {
        [Tooltip("array of statistics for all agents and player profiles")]
        public PlayerStats[] profileData;

        // initial save file with all agent data, but with values set to 0

        public SaveFileData()
        {
            List<PlayerStats> statList = new List<PlayerStats>();
            // IF NEW AGENT IS ADDED, ADD IT TO THIS LIST IN ORDER TO SAVE ITS DATA
            statList.Add(new PlayerStats("RedRunner"));
            statList.Add(new PlayerStats("Agent_EPP"));
            statList.Add(new PlayerStats("Agent_EPF"));
            statList.Add(new PlayerStats("Agent_EXP"));
            statList.Add(new PlayerStats("Agent_EXF"));
            statList.Add(new PlayerStats("Agent_XPP"));
            statList.Add(new PlayerStats("Agent_XPF"));
            statList.Add(new PlayerStats("Agent_XXP"));
            statList.Add(new PlayerStats("Agent_XXF"));

            profileData = new PlayerStats[statList.Count];

            for (int i = 0; i < statList.Count; i++)
            {
                profileData[i] = statList[i];
            }
        }
    }
}
