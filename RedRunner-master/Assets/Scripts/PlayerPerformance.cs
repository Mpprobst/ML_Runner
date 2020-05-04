﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RedRunner.Characters;

namespace RedRunner.TerrainGeneration
{
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
                {
                    blockName = currentBlock.name.Replace("(Clone)", "");
                    Debug.Log(blockName + currentBlock.FarJump.ToString() + currentBlock.HighJump.ToString() + currentBlock.FarJump.ToString() + currentBlock.Below.ToString() + currentBlock.HasEnemy.ToString());
                    blockName = currentBlock.GetName();
                    if (currentBlock.FarJump)
                        playerStats.obstacles[0].successes += 1;
                    if (currentBlock.HighJump && !currentBlock.Below)
                        playerStats.obstacles[1].successes += 1;
                    if (currentBlock.Below)
                        playerStats.obstacles[2].successes += 1;
                    if (currentBlock.Narrow)
                        playerStats.obstacles[3].successes += 1;
                    if (currentBlock.HasEnemy)
                        playerStats.obstacles[4].successes += 1;

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

            //Debug.Log("avg time on block = " + avgBlockTime);

        }

        private void UpdateBlockProbabilities()
        {

            for (int i = 0; i < 3; i++)
            {
                var stat = playerStats.obstacles[i];
                if (stat.successes != 0)
                    terrain.blockProbs[i] = Mathf.Abs(terrain.difficultyFactor * (stat.successes) / (stat.successes + stat.failures));
                else
                    terrain.blockProbs[i] = 1;
            }

            foreach (var blockData in terrain.m_Settings.m_MiddleBlocks)
            {
                int totalSuccess = 0;
                int totalFail = 0;

                if (blockData.Narrow)
                {
                    totalSuccess += playerStats.obstacles[3].successes;
                    totalFail += playerStats.obstacles[3].failures;
                }
                if (blockData.HasEnemy)
                {
                    totalSuccess += playerStats.obstacles[4].successes;
                    totalFail += playerStats.obstacles[4].failures;
                }

                if (totalSuccess != 0)
                {
                    //blockData.m_Probability = (stat.successes - (stat.failures * (0.4f - terrain.difficultyFactor)) ) / stat.successes;
                    blockData.m_Probability = Mathf.Abs((totalSuccess - (totalFail * (0.5f - terrain.difficultyFactor))) / totalSuccess);
                }
                else
                {
                    blockData.m_Probability = 1f;
                }
                Debug.Log("block: " + blockData.name + " has probability: " + blockData.m_Probability);
                break;
            }

        }
        

        private void UpdateDifficulty()
        {
            int distance = Mathf.FloorToInt(player.transform.position.x - player.StartingPos);
            float distanceFactor = distance / 500f;

            float speedFactor = avgSpeed / 14f;

            float timeFactor = (avgBlockTime - 1.25f) / 4;

            difficultyFactor = 0.75f * distanceFactor + (speedFactor - timeFactor) * 0.25f;
            //difficultyFactor = distanceFactor;
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
                //Debug.Log("average speed " + avgSpeed);
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
            {
                blockName = closestBlock.name.Replace("(Clone)", "");
                Debug.Log("died on "+ blockName + closestBlock.FarJump.ToString() + closestBlock.HighJump.ToString() + closestBlock.Below.ToString() + closestBlock.Narrow.ToString() + closestBlock.HasEnemy.ToString());

                if (closestBlock.FarJump)
                    playerStats.obstacles[0].failures += 1;
                if (closestBlock.HighJump && !closestBlock.Below)
                    playerStats.obstacles[1].failures += 1;
                if (closestBlock.Below)
                    playerStats.obstacles[2].failures += 1;
                if (closestBlock.Narrow)
                    playerStats.obstacles[3].failures += 1;
                if (closestBlock.HasEnemy)
                    playerStats.obstacles[4].failures += 1;
            }

            for (int i = 0; i < 50; i++)
            {
                SimulateRuns();
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

        private void SimulateRuns()
        {
            float p_far = (float)playerStats.obstacles[0].successes / (playerStats.obstacles[0].failures + playerStats.obstacles[0].successes);
            float p_high = (float)playerStats.obstacles[1].successes / (playerStats.obstacles[1].failures + playerStats.obstacles[1].successes);
            float p_below = (float)playerStats.obstacles[2].successes / (playerStats.obstacles[2].failures + playerStats.obstacles[2].successes);
            float p_narrow = (float)playerStats.obstacles[3].successes / (playerStats.obstacles[3].failures + playerStats.obstacles[3].successes);
            float p_enemy = (float)playerStats.obstacles[4].successes / (playerStats.obstacles[4].failures + playerStats.obstacles[4].successes);

            bool success = true;
            float totalProb = 0;

            Block nextBlock = TerrainGenerator.ChooseFrom(terrain.m_Settings.MiddleBlocks);
            terrain.CreateBlock(nextBlock, new Vector3(0, 0, 0));
            nextBlock = terrain.m_LastBlock;

            if (nextBlock.FarJump) totalProb += p_far;
            else totalProb += 1;

            if (nextBlock.HighJump) totalProb += p_high;
            else totalProb += 1;

            if (nextBlock.Below) totalProb += p_below;
            else totalProb += 1;

            if (nextBlock.Narrow) totalProb += p_narrow;
            else totalProb += 1;

            if (nextBlock.HasEnemy) totalProb += p_enemy;
            else totalProb += 1;

            totalProb = totalProb / 5;
            float random = Random.Range(0f, 1f);
            if (random > totalProb)
            {
                success = false;
            }

            if (nextBlock.FarJump)
            {
                if (success) playerStats.obstacles[0].successes++;
                else playerStats.obstacles[0].failures++;
            }
            if (nextBlock.HighJump)
            {
                if (success) playerStats.obstacles[1].successes++;
                else playerStats.obstacles[1].failures++;
            }
            if (nextBlock.Below)
            {
                if (success) playerStats.obstacles[2].successes++;
                else playerStats.obstacles[2].failures++;
            }
            if (nextBlock.Narrow)
            {
                if (success) playerStats.obstacles[3].successes++;
                else playerStats.obstacles[3].failures++;
            }
            if (nextBlock.HasEnemy)
            {
                if (success) playerStats.obstacles[4].successes++;
                else playerStats.obstacles[4].failures++;
            }

            //Destroy(nextBlock.gameObject);

        }

    }
}