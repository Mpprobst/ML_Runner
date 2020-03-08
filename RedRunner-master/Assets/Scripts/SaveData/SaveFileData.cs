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
        public SaveFileData(PlayerStats[] initialData)
        {
            profileData = initialData;
        }
    }
}
