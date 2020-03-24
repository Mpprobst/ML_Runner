using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RedRunner.Characters;

namespace RedRunner.TerrainGeneration
{
    // m_Settings.MiddleBlocks[0].m_Probability = 1f;

    /// <summary>
    /// This script observes the player and determines how well they are performing
    /// </summary>
    public class PlayerPerformance : MonoBehaviour
    {
        public RedCharacter player;
        private SaveFile saveFile;
        public PlayerStats playerStats;
        private TerrainGenerator terrain;

        float startTime;
        public float difficultyFactor;

        // average speed
        // keeps track of the last 10 player speeds over 10 seconds
        private Queue<float> speedSamples; 
        public float avgSpeed;

        // time per block
        // keeps track of the time spent on the last 10 blocks
        private Block currentBlock;
        private Queue<float> blockTimes;
        private float newBlockTime;
        private float jumpTime;

        public float avgBlockTime;
        // number of pickups

        // score that adds all criteria

        // Start is called before the first frame update
        void Start()
        {
            player = GameObject.FindObjectOfType<RedCharacter>();

            // load values into these stat objects from save file
            saveFile = GameObject.FindObjectOfType<SaveFile>();

            foreach (PlayerStats stat in saveFile.data.profileData)
            {
                if (stat.profileName == player.name)
                {
                    playerStats = stat;
                    break;
                }
            }

            terrain = GameObject.FindObjectOfType<TerrainGenerator>();

            player.jumpEvent = new UnityEngine.Events.UnityEvent();
            player.m_GroundCheck.landEvent = new BlockEvent();

            player.jumpEvent.AddListener(PlayerJumped);
            player.m_GroundCheck.landEvent.AddListener(PlayerLanded);

            player.death.AddListener(PlayerDied);

            speedSamples = new Queue<float>();
            blockTimes = new Queue<float>();

            startTime = Time.time;

            StartCoroutine(SpeedUpdater());

            GameManager.OnReset += Reset;
        }

        protected virtual void Reset()
        {
            speedSamples = new Queue<float>();
            blockTimes = new Queue<float>();

            startTime = Time.time;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void PlayerJumped()
        {
            jumpTime = Time.time;
        }

        private void PlayerLanded(GameObject block)
        {
            Block landBlock = block.GetComponent<Block>();

            if (landBlock != currentBlock)
            {
                player.currBlock = landBlock;
                Debug.Log("landed");
                if (blockTimes.Count > 10)
                {
                    blockTimes.Dequeue();
                }

                float newTime = jumpTime - newBlockTime;
                //if (currentBlock != null)
                //Debug.Log("time on " + currentBlock.name + " was " + newTime);
                blockTimes.Enqueue(newTime);

                newBlockTime = Time.time;

                // player advanced to next block, so it was a success
                string blockName = "";
                if (currentBlock)
                    blockName = currentBlock.name.Replace("(Clone)", "");

                for (int i = 0; i < playerStats.obstacles.Length; i++)
                {
                    if (playerStats.obstacles[i].name == blockName)
                    {
                        playerStats.obstacles[i].successes += 1;
                    }
                }

                UpdateDifficulty();

                UpdateBlockProbabilities();

                currentBlock = landBlock;
            }

            float total = 0;
            foreach (float blockTime in blockTimes)
            {
                total += blockTime;
            }

            avgBlockTime = total / blockTimes.Count;

            Debug.Log("avg time on block = " + avgBlockTime);

        }

        private void UpdateBlockProbabilities()
        {
            foreach (var stat in playerStats.obstacles)
            {
                foreach (var blockData in terrain.m_Settings.m_MiddleBlocks)
                {
                    if (stat.name == blockData.name)
                    {
                        if (stat.successes != 0)
                        {
                            //blockData.m_Probability = (stat.successes - (stat.failures * (0.4f - terrain.difficultyFactor)) ) / stat.successes;
                            blockData.m_Probability = Mathf.Abs( (stat.successes - (stat.failures * (0.5f - terrain.difficultyFactor))) / stat.successes ) ;
                        }
                        else
                        {
                            blockData.m_Probability = 1f;
                        }
                        Debug.Log("block: " + blockData.name + " has probability: " + blockData.m_Probability);
                        break;
                    }
                }
            }
        }

        private void UpdateDifficulty()
        {
            int distance = Mathf.FloorToInt(player.transform.position.x - player.StartingPos);
            float distanceFactor = distance / 500f;

            float speedFactor = avgSpeed / 14f;

            float timeFactor = (avgBlockTime - 1.25f) / 4;

            difficultyFactor = 0.75f * distanceFactor + (speedFactor - timeFactor) * 0.25f;

            if (difficultyFactor > 1f)
            {
                difficultyFactor = 1f;
            }

            if (difficultyFactor < 0.15f)
            {
                difficultyFactor = 0.15f;
            }

            terrain.difficultyFactor = difficultyFactor;
        }

        IEnumerator SpeedUpdater()
        {
            while (true)
            {
                if (speedSamples.Count > 10)
                {
                    speedSamples.Dequeue();
                }

                speedSamples.Enqueue(player.Speed.x);

                float total = 0;
                foreach (float sample in speedSamples)
                {
                    total += sample;
                }

                avgSpeed = total / speedSamples.Count;

                //avgSpeed = (player.transform.position.x - player.StartingPos) / (Time.time - startTime);
                Debug.Log("average speed " + avgSpeed);
                yield return new WaitForSeconds(1f);
            }
        }

        private void PlayerDied()
        {

            playerStats.scores.Add( Mathf.FloorToInt(player.transform.position.x - player.StartingPos) );
            if (playerStats.scores.Count >= 100)
            {
                Debug.Break();
            }
            Block[] allBlocks = GameObject.FindObjectsOfType<Block>();
            Block closestBlock = null;
            float closestDistance = 1000f;
            foreach (Block b in allBlocks)
            {
                float dist = Mathf.Abs(b.transform.position.x - player.transform.position.x);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestBlock = b;
                }
            }

            string blockName = "";

            if (currentBlock)
                blockName = closestBlock.name.Replace("(Clone)", "");

            for (int i = 0; i < playerStats.obstacles.Length; i++)
            {
                if (playerStats.obstacles[i].name == blockName)
                {
                    playerStats.obstacles[i].failures += 1;
                    Debug.Log("Died on " + blockName);
                    SaveStats();
                }
            }
        }

        private void SaveStats()
        {
            for (int i = 0; i < saveFile.data.profileData.Length; i++)
            {
                if (saveFile.data.profileData[i].profileName == playerStats.profileName)
                    saveFile.data.profileData[i] = playerStats;
            }
            saveFile.SaveData();
        }

        private void OnApplicationQuit()
        {
            SaveStats();
        }

    }
}