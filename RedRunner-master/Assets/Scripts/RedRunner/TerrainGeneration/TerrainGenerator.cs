using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RedRunner.Characters;

namespace RedRunner.TerrainGeneration
{

	public abstract class TerrainGenerator : MonoBehaviour
	{

		private static TerrainGenerator m_Singleton;

		public static TerrainGenerator Singleton
		{
			get
			{
				return m_Singleton;
			}
		}

		protected Dictionary<Vector3, Block> m_Blocks;
		protected Dictionary<Vector3, BackgroundBlock> m_BackgroundBlocks;
		protected BackgroundLayer[] m_BackgroundLayers;
		protected float m_PreviousX;
		protected float m_CurrentX;
		protected float m_FathestBackgroundX;
		[SerializeField]
		public TerrainGenerationSettings m_Settings;
		protected int m_GeneratedStartBlocksCount;
		protected int m_GeneratedMiddleBlocksCount;
		protected int m_GeneratedEndBlocksCount;
		[SerializeField]
		protected float m_DestroyRange = 100f;
		[SerializeField]
		protected float m_GenerateRange = 100f;
		[SerializeField]
		protected float m_BackgroundGenerateRange = 200f;
		[SerializeField]
		protected Character m_Character;
		protected Block m_LastBlock;
		protected BackgroundBlock m_LastBackgroundBlock;
		protected float m_RemoveTime = 0f;
		protected bool m_Reset = false;

        // Params for spawning blocks with different separations and heights
        public float m_MinExtraWidth = 1f;
        public float m_MaxExtraWidth = 9f;
        public float m_MinExtraHeight = -4f;
        public float m_MaxExtraHeight = 4f;

        public float m_AbsMinHeight = 1f;
        public float m_AbsMaxHeight = 76f;
        [Range(0.15f, 1f)]
        public float difficultyFactor = 0.15f;

        public float PreviousX
		{
			get
			{
				return m_PreviousX;
			}
		}

		public float CurrentX
		{
			get
			{
				return m_CurrentX;
			}
		}

		public TerrainGenerationSettings Settings
		{
			get
			{
				return m_Settings;
			}
		}

		protected virtual void Awake ()
		{
            m_Character = GameObject.FindObjectOfType<Character>();
			if ( m_Singleton != null )
			{
				Destroy ( gameObject );
				return;
			}
			m_Singleton = this;
			m_Blocks = new Dictionary<Vector3, Block> ();
			m_BackgroundBlocks = new Dictionary<Vector3, BackgroundBlock> ();
			m_BackgroundLayers = new BackgroundLayer[m_Settings.BackgroundLayers.Length];
			for ( int i = 0; i < m_Settings.BackgroundLayers.Length; i++ )
			{
				m_BackgroundLayers [ i ] = m_Settings.BackgroundLayers [ i ];
			}
			GameManager.OnReset += Reset;

            Debug.Log(Settings.MiddleBlocks[0].name);
		}

		protected virtual void Reset ()
		{
			m_Reset = true;
			RemoveAll ();
			m_CurrentX = 0f;
			m_LastBlock = null;
			m_LastBackgroundBlock = null;
			for ( int i = 0; i < m_BackgroundLayers.Length; i++ )
			{
				m_BackgroundLayers [ i ].Reset ();
			}
			m_FathestBackgroundX = 0f;
			m_Blocks.Clear ();
			m_BackgroundBlocks.Clear ();
			m_GeneratedStartBlocksCount = 0;
			m_GeneratedMiddleBlocksCount = 0;
			m_GeneratedEndBlocksCount = 0;
			m_Reset = false;
		}

		protected virtual void OnDestroy ()
		{
			m_Singleton = null;
		}

		protected virtual void Update ()
		{
			if ( m_Reset )
			{
				return;
			}
			if ( m_RemoveTime < Time.time )
			{
				m_RemoveTime = Time.time + 5f;
				Remove ();
			}
			Generate ();
        }

		public virtual void Generate ()
		{
			if ( m_CurrentX < m_Settings.LevelLength || m_Settings.LevelLength <= 0 )
			{
				bool isEnd = false, isStart = false, isMiddle = false;
				Block block = null;
				Vector3 current = new Vector3 ( m_CurrentX, 0f, 0f );
				float newX = 0f;
				if ( m_GeneratedStartBlocksCount < m_Settings.StartBlocksCount || m_Settings.StartBlocksCount <= 0 )
				{
					isStart = true;
					block = ChooseFrom ( m_Settings.StartBlocks );
				}
				else if ( m_GeneratedMiddleBlocksCount < m_Settings.MiddleBlocksCount || m_Settings.MiddleBlocksCount <= 0 )
				{
					isMiddle = true;
					block = ChooseFrom ( m_Settings.MiddleBlocks );
				}
				else if ( m_GeneratedEndBlocksCount < m_Settings.EndBlocksCount || m_Settings.EndBlocksCount <= 0 )
				{
					isEnd = true;
					block = ChooseFrom ( m_Settings.EndBlocks );
				}
				if ( m_LastBlock != null )
				{
					newX = m_CurrentX + m_LastBlock.Width;
				}
				else
				{
					newX = 0f;
				}
				if ( block != null && ( m_LastBlock == null || newX < m_Character.transform.position.x + m_GenerateRange ) )
				{
					if ( isStart )
					{
						if ( m_Settings.StartBlocksCount > 0 )
						{
							m_GeneratedStartBlocksCount++;
						}
					}
					else if ( isMiddle )
					{
						if ( m_Settings.MiddleBlocksCount > 0 )
						{
							m_GeneratedMiddleBlocksCount++;
						}
					}
					else if ( isEnd )
					{
						if ( m_Settings.EndBlocksCount > 0 )
						{
							m_GeneratedEndBlocksCount++;
						}
					}
					CreateBlock ( block, current );
				}
			}
			for ( int i = 0; i < m_BackgroundLayers.Length; i++ )
			{
				int random = Random.Range ( 0, 2 );
				bool generate = random == 1 ? true : false;
				if ( !generate )
				{
					continue;
				}
				Vector3 current = new Vector3 ( m_BackgroundLayers [ i ].CurrentX, 0f, 0f );
				BackgroundBlock block = ( BackgroundBlock )ChooseFrom ( m_BackgroundLayers [ i ].Blocks );
				float newX = 0f;
				if ( m_BackgroundLayers [ i ].LastBlock != null )
				{
					newX = m_BackgroundLayers [ i ].CurrentX + m_BackgroundLayers [ i ].LastBlock.Width;
				}
				else
				{
					newX = 0f;
				}
				if ( block != null && ( m_BackgroundLayers [ i ].LastBlock == null || newX < m_Character.transform.position.x + m_BackgroundGenerateRange ) )
				{
					CreateBackgroundBlock ( block, current, m_BackgroundLayers [ i ], i );
				}
			}
		}

		public virtual void Remove ()
		{
			List<Block> blocksToRemove = new List<Block> ();
			foreach ( KeyValuePair<Vector3, Block> block in m_Blocks )
			{
				if ( block.Value.transform.position.x - m_CurrentX > m_DestroyRange )
				{
					blocksToRemove.Add ( block.Value );
				}
			}
			List<BackgroundBlock> backgroundBlocksToRemove = new List<BackgroundBlock> ();
			foreach ( KeyValuePair<Vector3, BackgroundBlock> block in m_BackgroundBlocks )
			{
				if ( block.Value.transform.position.x - m_FathestBackgroundX > m_DestroyRange )
				{
					backgroundBlocksToRemove.Add ( block.Value );
				}
			}
			for ( int i = 0; i < blocksToRemove.Count; i++ )
			{
				RemoveBlock ( blocksToRemove [ i ] );
			}
			for ( int i = 0; i < backgroundBlocksToRemove.Count; i++ )
			{
				RemoveBackgroundBlock ( backgroundBlocksToRemove [ i ] );
			}
		}

		public virtual void RemoveAll ()
		{
			List<Block> blocksToRemove = new List<Block> ();

            foreach ( KeyValuePair<Vector3, Block> block in m_Blocks )
			{
                blocksToRemove.Add ( block.Value );
			}
            List<BackgroundBlock> backgroundBlocksToRemove = new List<BackgroundBlock> ();
			foreach ( KeyValuePair<Vector3, BackgroundBlock> block in m_BackgroundBlocks )
			{
				backgroundBlocksToRemove.Add ( block.Value );
			}
            for ( int i = 0; i < blocksToRemove.Count; i++ )
			{
				RemoveBlock ( blocksToRemove [ i ] );
			}
            for ( int i = 0; i < backgroundBlocksToRemove.Count; i++ )
			{
				RemoveBackgroundBlock ( backgroundBlocksToRemove [ i ] );
			}
		}

		public virtual void RemoveBlockAt ( Vector3 position )
		{
			RemoveBlock ( m_Blocks [ position ] );
		}

		public virtual void RemoveBlock ( Block block )
		{
			block.OnRemove ( this );
            Destroy( m_Blocks [ block.transform.position ].gameObject );
            m_Blocks.Remove ( block.transform.position );
		}

		public virtual void RemoveBackgroundBlock ( BackgroundBlock block )
		{
			block.OnRemove ( this );
			Destroy ( m_BackgroundBlocks [ block.transform.position ].gameObject );
			m_BackgroundBlocks.Remove ( block.transform.position );
		}

        public float GetDifficultyProbablilty()
        {
            // use a gaussian curve to get a value 0 - 1 based on the difficulty
            float rawRandomFloat = Random.Range(0f, 1f);
            float variance = (difficultyFactor * 5f / 8f) + (3f / 8f);
            float difficultyProbablilty = variance * (Mathf.Exp(-(Mathf.Pow((0.5f - rawRandomFloat), 2) / (variance * variance / (difficultyFactor * 10f)))));
            return difficultyProbablilty;
        }

        public void CheckX(ref float currentMaxWidth, ref float extraWidth, ref float extraHeight)
        {
            // make sure jump is possible
            float xCheck = 0;
            float midpoint = currentMaxWidth / 2f;
            if (extraWidth < midpoint)
            {
                xCheck = extraWidth;
            }
            else
            {
                //Debug.Log("using y equation");
                xCheck = midpoint;
            }

            if (extraWidth > xCheck)
            {
                extraWidth = xCheck - 0.25f;
                Debug.Log("adjusting impossible jump width = " + extraWidth);
            }

        }

        public void CheckY(ref float currentMaxWidth, ref float extraWidth, ref float extraHeight)
        {
            // make sure jump is possible
            float yCheck = 0;
            float midpoint = currentMaxWidth / 2f;
            if (extraWidth < midpoint)
            {
                yCheck = m_MaxExtraHeight;
            }
            else
            {
                //Debug.Log("using y equation");
                yCheck = m_MaxExtraHeight - (m_MaxExtraHeight / midpoint) * extraWidth;
            }

            // Legacy function for specific speed
            //float yCheck = -0.175f * (extraWidth * extraWidth) + 1.75f * extraWidth + 0.14f;
            if (extraHeight > yCheck)
            {
                extraHeight = yCheck - 0.5f;
                Debug.Log("adjusting impossible jump height = " + extraHeight);
            }
        }

        public virtual bool CreateBlock ( Block blockPrefab, Vector3 position )
		{
			if ( blockPrefab == null )
			{
				return false;
			}
			blockPrefab.PreGenerate ( this );
			Block block = Instantiate<Block> ( blockPrefab, position, Quaternion.identity );

            float maxRunningDistance = block.Width;
            block.originalWidth = block.Width;
            if (m_Character.ContinuousDistance > maxRunningDistance)
            {
                Debug.Log("using continuous distance");
                maxRunningDistance = m_Character.ContinuousDistance;
            }

            //Debug.Log("maxRunningDistance = " + maxRunningDistance);
            float potentialSpeed = 0.85f * Mathf.Log(maxRunningDistance) + 3.84f;

            float speed = m_Character.Speed.x;
            float currentMaxWidth = 0.58f * potentialSpeed + 5.56f;
            //Debug.Log("speed of " + potentialSpeed + " gives random width between " + m_MinExtraWidth + " and " + currentMaxWidth);

            float randomWidthFactor = GetDifficultyProbablilty();
            float randomHeightFactor = GetDifficultyProbablilty();
            float extraWidth = randomWidthFactor * (currentMaxWidth - m_MinExtraWidth);

            int randomNeg = Random.Range(0, 2);

            if (randomNeg == 0)
            {
                randomNeg = -1;
            }

            float extraHeight = randomHeightFactor * (m_MaxExtraHeight - m_MinExtraHeight) * randomNeg;

            // randomize x or y dependency
            int random = Random.Range(0, 2);
            if (random == 0)
            {
                CheckY(ref currentMaxWidth, ref extraWidth, ref extraHeight);
            }
            else
            {
                CheckX(ref currentMaxWidth, ref extraWidth, ref extraHeight);
            }

            if (extraWidth < m_MinExtraWidth)
            {
                Debug.Log("was min width");
                extraWidth = m_MinExtraWidth;
            }


            // change height of the new block
            float previousHeight = 0f;
            if (m_LastBlock != null)
            {
                previousHeight = m_LastBlock.transform.position.y;
            }
            block.transform.position = new Vector3(block.transform.position.x, Mathf.Clamp(previousHeight + extraHeight, m_AbsMinHeight, m_AbsMaxHeight), 0f);

            //Debug.Log("extraWidth = " + extraWidth + " extraHeight = " + extraHeight );
            float blockWidth = block.Width;

            block.Width = blockWidth + extraWidth;
            position = block.transform.position;

            if (extraWidth > m_MaxExtraWidth / 2f)
                block.FarJump = true;
            else
                block.FarJump = false;

            if (extraHeight > m_MaxExtraHeight / 2f)
                block.HighJump = true;
            else
                block.HighJump = false;
            if (m_LastBlock != null)
            {
                Debug.Log("last y = " + m_LastBlock.transform.position.y + " new y = " + position.y);
                if (m_LastBlock.transform.position.y - position.y > 0f)
                    block.Below = true;
                else
                    block.Below = false;
            }

            m_PreviousX = m_CurrentX;
			m_CurrentX += block.Width;
			m_Blocks.Add ( position, block );
			blockPrefab.PostGenerate ( this );
			m_LastBlock = block;
			return true;
		}

		public virtual bool CreateBackgroundBlock ( BackgroundBlock blockPrefab, Vector3 position, BackgroundLayer layer, int layerIndex )
		{
			if ( blockPrefab == null )
			{
				return false;
			}
			blockPrefab.PreGenerate ( this );
			position.z = blockPrefab.transform.position.z;
			position.y = blockPrefab.transform.position.y;
			BackgroundBlock block = Instantiate<BackgroundBlock> ( blockPrefab, position, Quaternion.identity );
			float width = Random.Range ( block.MinWidth, block.MaxWidth );
			m_BackgroundLayers [ layerIndex ].PreviousX = m_BackgroundLayers [ layerIndex ].CurrentX;
			m_BackgroundLayers [ layerIndex ].CurrentX += width;
			block.Width = width;
			m_BackgroundLayers [ layerIndex ].LastBlock = block;
			m_BackgroundBlocks.Add ( position, block );
			blockPrefab.PostGenerate ( this );
			if ( m_BackgroundLayers [ layerIndex ].CurrentX > m_FathestBackgroundX )
			{
				m_FathestBackgroundX = m_BackgroundLayers [ layerIndex ].CurrentX;
			}
			return true;
		}

		public Block GetCharacterBlock ()
		{
			Block characterBlock = null;
			foreach ( KeyValuePair<Vector3, Block> block in m_Blocks )
			{
				if ( block.Key.x <= m_Character.transform.position.x && block.Key.x + block.Value.Width > m_Character.transform.position.x )
				{
					characterBlock = block.Value;
					break;
				}
			}
			return characterBlock;
		}

		public static Block ChooseFrom ( Block[] blocks )
		{
			if ( blocks.Length <= 0 )
			{
				return null;
			}
			float total = 0;
			for ( int i = 0; i < blocks.Length; i++ )
			{
				total += blocks [ i ].Probability;
			}
			float randomPoint = Random.value * total;
			for ( int i = 0; i < blocks.Length; i++ )
			{
				if ( randomPoint < blocks [ i ].Probability )
				{
					return blocks [ i ];
				}
				else
				{
					randomPoint -= blocks [ i ].Probability;
				}
			}
			return blocks [ blocks.Length - 1 ];
		}

	}

}