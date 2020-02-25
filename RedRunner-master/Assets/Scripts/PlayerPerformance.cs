using System.Collections;
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

        float startTime;

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

            if (player)
            {
                player.jumpEvent = new UnityEngine.Events.UnityEvent();
                player.m_GroundCheck.landEvent = new BlockEvent();

                player.jumpEvent.AddListener(PlayerJumped);
                player.m_GroundCheck.landEvent.AddListener(PlayerLanded);

            }

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
            //print("player landed on " + block.name);
            Block landBlock = block.GetComponent<Block>();
            if (landBlock != currentBlock)
            {
                if (blockTimes.Count > 10)
                {
                    blockTimes.Dequeue();
                }

                float newTime = jumpTime - newBlockTime;
                //if (currentBlock != null)
                    //Debug.Log("time on " + currentBlock.name + " was " + newTime);
                blockTimes.Enqueue(newTime);

                newBlockTime = Time.time;
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

    }
}