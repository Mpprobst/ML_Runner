using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedRunner.TerrainGeneration
{
    [System.Serializable]
    public class SaveFileWrapper
    {
        public SaveFileData data;

        public SaveFileWrapper()
        {
            data = new SaveFileData();
        }
    }
}
