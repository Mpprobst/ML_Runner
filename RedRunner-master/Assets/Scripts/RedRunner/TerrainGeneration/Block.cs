using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedRunner.TerrainGeneration
{

	public abstract class Block : MonoBehaviour
	{

        [Header("Classifiers")]
        public bool FarJump; // False indicates the jump is < 1/2 the max jump distance. True is the latter
        public bool HighJump; // True indicates the jump is > 1/2 the max jump height. False is the latter
        public bool Below; // true if the block is lower than the previous
        public bool Narrow; // false for blocks with 3 and 4 tiles. true for blocks with 1 and 2 tiles. set in inspector
        public bool HasEnemy; // self-explanatory. set in inspector
        public string classString;

		[SerializeField]
		protected float m_Width;
        public float originalWidth;
		[SerializeField]
		public float m_Probability = 1f;

		public virtual float Width {
			get {
				return m_Width;
			}
			set {
				m_Width = value;
			}
		}

		public virtual float Probability {
			get {
				return m_Probability;
			}
		}

		public virtual void OnRemove (TerrainGenerator generator)
		{
			
		}

		public virtual void PreGenerate (TerrainGenerator generator)
		{
			
		}

		public virtual void PostGenerate (TerrainGenerator generator)
		{
			
		}

        public string GetName()
        {
            name = "";
            if (FarJump)
                name += "F";
            else
                name += "S";

            if (HighJump)
                name += "H";
            else
                name += "L";

            if (Below)
                name += "B";
            else
                name += "A";

            if (Narrow)
                name += "N";
            else
                name += "W";

            if (HasEnemy)
                name += "E";
            else
                name += "_";

            return name;
        }

	}

}