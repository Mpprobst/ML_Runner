using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedRunner.TerrainGeneration
{
    [System.Serializable]
    public class SaveFileWrapper
    {
        public SaveFileData data;

        public SaveFileWrapper(PlayerStats[] initialdata)
        {
            data = new SaveFileData(initialdata);
        }
    }
}
