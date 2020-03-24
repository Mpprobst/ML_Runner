using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedRunner.TerrainGeneration;

namespace RedRunner.Characters
{
    public class AIRunner : RedCharacter
    {
        public const string GROUND_TAG = "Ground";
        public const string ENEMY_TAG = "Enemy";
        public const string GROUND_LAYER_NAME = "Ground";

        [Header("Agent Types")]
        public bool enemyAware = true;
        public bool predictive = false;
        public bool precise = false;

        private float maxInputVal = 1f;
        public float reactionTime = 0.001f; // used by coroutines to simulate think speed
        private float lookDistance = 2f;

        // used to find next block
        public BlockFinder blockFinder;
        private float blockDist;
        private float blockHeight;

        private bool preciseStarted = false;
        private float jumpPosition = 0;


        public float inputVal = 0f;  // modified by other functions, but its value is constantly fed to Move()
        protected GameObject nextBlock; // 

        public override void Reset()
        {
            base.Reset();
            blockFinder.Reset();
            inputVal = 1;
            Vector2 edgeSensor = new Vector2(0f, -1f);
            StartCoroutine(FindEdge(edgeSensor));
            StartCoroutine(FindNextBlock());
        }

        protected override void Awake()
        {
            base.Awake();
        }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            Reset();
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();

            if (predictive)
            {
                CalculateJump();
            }

            if (precise && !IsDead.Value)
            {
                if (!m_GroundCheck.IsGrounded)
                {
                    if (!preciseStarted)
                    {
                        jumpPosition = transform.position.x;
                        preciseStarted = true;
                    }
                    //StartCoroutine(PreciseMovementRoutine());
                    PreciseMovement();
                }
                else
                {
                    Debug.Log("stopping precise movement");
                    preciseStarted = false;
                    inputVal = 1f;
                    //StopCoroutine(PreciseMovementRoutine());
                }
            }

            Move(inputVal);

        }

        /// <summary>
        /// Inherent to all agents.
        /// Finds the next block the agent will jump to.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator FindNextBlock()
        {
            while (true)
            {
                float minDist = 100f;
                if (blockFinder.blocks != null)
                {
                    foreach (Block block in blockFinder.blocks)
                    {
                        float distance = Vector3.Distance(transform.localPosition, block.transform.localPosition);
                        if (distance < minDist)
                        {
                            nextBlock = block.gameObject;
                        }
                    }
                }
                yield return new WaitForSeconds(reactionTime);
            }
        }

        /// <summary>
        /// Inherent to all agents.
        /// Detects if the agent is about to fall off a platform and jumps if so.
        /// </summary>
        /// <param name="sensorVector"></param>
        /// <returns></returns>
        protected virtual IEnumerator FindEdge(Vector2 sensorVector)
        {
            while (true)
            {
                Vector2 rayStart = new Vector2(gameObject.transform.position.x + lookDistance, gameObject.transform.position.y);
                RaycastHit2D hit = Physics2D.Raycast(rayStart, sensorVector, 10f, LayerMask.GetMask(GROUND_LAYER_NAME));
                Debug.DrawRay(rayStart, sensorVector * 10f);
                bool edgeDetected = hit != null && hit.collider != null && hit.collider.CompareTag(GROUND_TAG);

                bool enemyDetected = hit != null && hit.collider != null && hit.collider.CompareTag(ENEMY_TAG);

                if (enemyAware && enemyDetected)
                {
                    Jump();
                }

                if (!edgeDetected)
                {
                    Jump();
                }

                yield return new WaitForSeconds(reactionTime);
            }
        }

        /// <summary>
        /// Specific to predictive agents.
        /// Uses the distance to next block and potential distance to determine when to jump
        /// </summary>
        protected virtual void CalculateJump()
        {
            // consider making this a coroutine
            //float potentialDist = 0.58f * Speed.x + 5.56f;
            float potentialDist = 2f * Mathf.Log(Speed.x) + 5.342f;

            // find the distance to next block
            if (nextBlock && currBlock)
            {
                blockDist = nextBlock.transform.position.x - transform.position.x;
                blockHeight = nextBlock.transform.position.y - currBlock.transform.position.y;
            }
            // make sure block isnt too high
            //Debug.Log(potentialDist);
            if (blockDist > 0f)
            {
                // height = maxheight - (maxheight / midpoint) * xDist
                // xDist = (maxheight - height) * (midpoint/maxheight)
                float midpoint = potentialDist / 2f;
                potentialDist = (4.5f - blockHeight) * (midpoint / 4.5f) + midpoint;
                //Debug.Log("calculated x = " + potentialDist);
                if (blockDist < potentialDist)
                {
                    Jump();
                }
            }
        }

        /// <summary>
        /// Specific to precise agents.
        /// Predicts where it will land and adjusts input accordingly
        /// </summary>
        /// <returns></returns>
        protected virtual void PreciseMovement()
        {
            Debug.Log("starting precisely moving");
            float traveled = 0f;

            //while (true)
            //{
            traveled = transform.position.x - jumpPosition;

            //float potentialDist = 0.58f * Speed.x + 5.56f - traveled;
            float potentialDist = 2f * Mathf.Log(Speed.x) + 5.342f - traveled;

            //Debug.Log("traveled " + traveled + " from jump. Can move " + potentialDist + " more.");

            Vector2 rayStart = new Vector2(jumpPosition + traveled + potentialDist, gameObject.transform.position.y);
            RaycastHit2D hit = Physics2D.Raycast(rayStart, new Vector2(0, -1), 10f, LayerMask.GetMask(GROUND_LAYER_NAME));
            Debug.DrawRay(rayStart, new Vector2(0, -1) * 10f);
            bool edgeDetected = hit != null && hit.collider != null && hit.collider.CompareTag(GROUND_TAG);
            bool enemyDetected = hit != null && hit.collider != null && hit.collider.CompareTag(ENEMY_TAG);

            GameObject collidedBlock = null;
            if (hit.collider)
            {
                collidedBlock = hit.collider.gameObject;
                print("\tprecise movement found: " + collidedBlock);
            }

            if (!edgeDetected || (enemyAware && enemyDetected) )
            {
                inputVal -= 0.01f;
            }
            else if (inputVal < 1)
            {
                inputVal += 0.01f;
            }

            if (inputVal < 0)
                inputVal = 0;


            //yield return new WaitForSeconds(reactionTime);
            //}
        }

        protected GameObject FindBlock()
        {
            GameObject foundBlock = null;

            return foundBlock;

        }

        public override void Die()
        {
            Debug.Log("dead");
            StopAllCoroutines();
            base.Die();
        }
    }
}
