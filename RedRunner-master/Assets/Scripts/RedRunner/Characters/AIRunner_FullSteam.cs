using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedRunner.Characters
{
    public class AIRunner_FullSteam : AIRunner
    {

        // Start is called before the first frame update
        protected override void Start()
        {
            inputVal = 1f;
            base.Start();
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }
    }
}
