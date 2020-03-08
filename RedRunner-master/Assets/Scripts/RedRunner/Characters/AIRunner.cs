using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedRunner.Characters
{
    public class AIRunner : RedCharacter
    {
        public const string GROUND_TAG = "Ground";
        public const string ENEMY_TAG = "Enemy";
        public const string GROUND_LAYER_NAME = "Ground";

        private float maxInputVal = 1f;
        public float reactionTime = 0.001f; // used by coroutines to simulate think speed
        public bool enemyAware = true;
        private float lookDistance = 2f;

        protected float inputVal = 0f;  // modified by other functions, but its value is constantly fed to Move()
        protected GameObject nextBlock; //

        protected override void Awake()
        {
            base.Awake();
        }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            Vector2 edgeSensor = new Vector2(0f, -1f);
            StartCoroutine(FindEdge(edgeSensor));
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            Move(inputVal);
        }

        protected IEnumerator FindEdge(Vector2 sensorVector)
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

        protected GameObject FindBlock()
        {
            GameObject foundBlock = null;


            return foundBlock;

        }

        public override void Die(bool blood)
        {
            StopCoroutine(FindEdge(Vector2.down));
            base.Die(blood);

        }
    }
}
