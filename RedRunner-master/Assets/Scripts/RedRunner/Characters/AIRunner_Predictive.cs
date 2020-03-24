using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedRunner.TerrainGeneration;
namespace RedRunner.Characters
{
    public class AIRunner_Predictive : AIRunner
    { 
        protected override void Awake()
        {
            base.Awake();
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            m_GroundCheck.landEvent = new BlockEvent();
            m_GroundCheck.landEvent.AddListener(Land);

            StartCoroutine(FindNextBlock());
            //StartCoroutine(JumpToBlock());
            inputVal = 1f;
        }

        // Update is called once per frame
        protected override void Update()
        {

            base.Update();

            CalculateJump();
        }

        protected override void CalculateJump()
        {
            float potentialDist = 0.58f * Speed.x + 5.56f;
            // find the distance to next block
            float blockDist = 0;
            float blockHeight = 0;
            if (nextBlock && currBlock)
            {
                blockDist = nextBlock.transform.position.x - transform.position.x;
                blockHeight = nextBlock.transform.position.y - currBlock.transform.position.y;
            }
            // make sure block isnt too high
            Debug.Log(potentialDist);
            if (blockDist > 0f)
            {
                // height = maxheight - (maxheight / midpoint) * xDist
                // xDist = (maxheight - height) * (midpoint/maxheight)
                float midpoint = potentialDist / 2f;
                potentialDist = (4.5f - blockHeight) * (midpoint / 4.5f) + midpoint;
                Debug.Log("calculated x = " + potentialDist);
                if (blockDist < potentialDist)
                {
                    Jump();
                }
            }
        }

        protected override IEnumerator FindNextBlock()
        {
            while (true)
            {
                float minDist = 100f;
                foreach (Block block in blockFinder.blocks)
                {

                    float distance = Vector3.Distance(transform.localPosition, block.transform.localPosition);
                    if (distance < minDist)
                    {
                        nextBlock = block.gameObject;
                    }
                }
                yield return new WaitForSeconds(reactionTime);
            }
        }

        private void Land(GameObject block)
        {
            Debug.Log("land");
            currBlock = block.GetComponent<Block>();
        }
        
    }
    
}