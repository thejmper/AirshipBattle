using UnityEngine;

namespace Assets.Scripts
{
    class TurningTest: MonoBehaviour
    {
        //used in lieu of time.deltaTime.
        public const float TIME_FRACTION = 0.1f;

        //--public tweakables--//
        /// <summary>
        /// target we want to point towards
        /// </summary>
        public Transform target;

        /// <summary>
        /// how much our heading changes per degree of heading AoA.
        /// Must be greater than 0
        /// Less than 1 means we steer faster than we turn. 
        /// </summary>
        public float headingSteerConstant;
        /// <summary>
        /// how much our altitude changes per degree of pitch
        /// </summary>
        public float pitchSteerConstant;

        /// <summary>
        /// how fast (in degrees per second) we change heading at max rotational speed.
        /// </summary>
        public float maxHeadingChangeSpeed;
        /// <summary>
        /// max speed (in degrees per second) that we change our pitch at max rotational speed.
        /// </summary>
        public float maxPichChangeSpeed;

        //--private variables--//        
        ///<summary>
        ///direction we're moving
        /// </summary>
        private Vector3 velocity = Vector3.forward;
        /// <summary>
        /// direction we'd like to face!
        /// </summary>
        private Vector3 facing = Vector3.forward;

        /// <summary>
        /// update that handles movement.
        /// Each "module" should be split into its own method soon, but
        /// for testing, we're keeping them all here to mimimize the refactoring should
        /// something go horribly horribly wrong.
        /// </summary>
        private void Update()
        {
            
            Vector3 desiredVelocity = target.position - this.transform.position;
            
            //modules should be executed in this order.
            //first find the relevant deltas,
            //then compute forces applied given the various AoAs
            //only THEN do you apply some change to the heading.

            float headingDelta = 0f;
            float pitchDelta = 0f;

            //heading module.
            {
                //flatten things so we don't get stray y-axis stuff causing trouble.
                Vector3 desiredHeading = desiredVelocity.Flatten();
                Vector3 currentHeading = velocity.Flatten();

                //get the normal vector, but don't allow it to rotate all the way, as that'll cause problems if our desired bearing
                //is *exactly* behind us.
                Vector3 normal = Vector3.Cross(currentHeading, Vector3.RotateTowards(currentHeading, desiredHeading, 1.57f, 0f)).normalized;

                //get the heading delta (in degrees)
                headingDelta = Vector3.Angle(currentHeading, desiredHeading);
                if (normal.y < 0)
                    headingDelta = -headingDelta;

                //in reality, return headingDelta
            }
            //end heading module.     

            //pitch module
            {
                pitchDelta = VectorExtensions.GetPitchDelta(this.velocity, desiredVelocity);
            }
            //end pitch module

            //applied forces module
            {
                //heading!
                float headingAoA = Vector3.Angle(facing.Flatten(), velocity.Flatten());
                this.velocity = VectorExtensions.RotateTowardsFlat(velocity, facing, (headingAoA / headingSteerConstant));

                float pitchAoA = VectorExtensions.GetPitchDelta(this.velocity, this.facing);
               
                {
                    this.velocity.y = (pitchAoA * pitchSteerConstant);
                }

                //this.velocity += Physics.gravity * Time.deltaTime;
            }
            //end applied forces module

            //apply to facing
            {
                //heading delta
                float headingSpeed = Mathf.Clamp(headingDelta, -maxHeadingChangeSpeed, maxHeadingChangeSpeed);
                this.facing = Quaternion.AngleAxis(headingSpeed * Time.deltaTime, Vector3.up) * this.facing;

                //pitch delta
                //float pitchSpeed = Mathf.Clamp(pitchDelta, -maxPichChangeSpeed, maxPichChangeSpeed);
                //this.facing = Quaternion.AngleAxis(pitchDelta, transform.right) * this.facing.Flatten().normalized;
                
            }
            
        }


        //float headingDelta = 0f;
        private void OnGUI()
        {
            /*
            Vector3 desiredVelDir = target.position - this.transform.position;
            
            //modules should be executed in this order.
            //first find the relevant deltas,
            //then compute forces applied given the various AoAs
            //only THEN do you apply some change to the heading.


            //heading module.
            if(GUI.Button(new Rect(10, 10, 150, 35), "heading"))
            {
                //flatten things so we don't get stray y-axis stuff causing trouble.
                Vector3 desiredHeading = desiredVelDir.Flatten();
                Vector3 currentHeading = velocity.Flatten();

                //get the normal vector, but don't allow it to rotate all the way, as that'll cause problems if our desired bearing
                //is *exactly* behind us.
                Vector3 normal = Vector3.Cross(currentHeading, Vector3.RotateTowards(currentHeading, desiredHeading, 1.57f, 0f)).normalized;

                //get the heading delta (in degrees)
                headingDelta = Vector3.Angle(currentHeading, desiredHeading);
                if (normal.y < 0)
                    headingDelta = -headingDelta;
            }
            //end heading module.     

            //applied forces module
            if (GUI.Button(new Rect(10, 55, 150, 35), "velocity"))
            {
                float headingAoA = Vector3.Angle(facing.Flatten(), velocity.Flatten());
                this.velocity = VectorExtensions.RotateTowardsFlat(velocity, facing, headingAoA / headingSteerConstant);
            }

            //apply to facing
            if (GUI.Button(new Rect(10, 100, 150, 35), "facing"))
            {
                Debug.Log(headingDelta);

                float headingSpeed = Mathf.Clamp(headingDelta, -maxHeadingChangeSpeed, maxHeadingChangeSpeed);

                this.facing = Quaternion.AngleAxis(headingSpeed * TIME_FRACTION, Vector3.up) * this.facing;

            }
            */
        }

        //--debugging/testing--//
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, target.position);

            //draw velocity
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, velocity);
            Gizmos.DrawWireSphere(transform.position + velocity, 0.1f);

            //draw facing
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, facing);
            Gizmos.DrawWireSphere(transform.position + facing, 0.15f);
        }
    }
}

//how the update loop should go
//get the offset vector between where our velocity is pointing and where we want it to point

//get the heading delta and pitch/altitude delta components

//from those deltas, compute how much heading and pitch we'd need to apply to our facing
//to get the right velocity change.
//we do that by taking the inverse of the pitch and heading steering functions. (functions that
//determine how much velocity deflection we get per degree of AoA)

//issue those heading/pitch deltas to the object, and have it start to turn towards where we want
//it to be. Obviously we want to use a rotateToward here to keep things from happening instantly.

//now that our facing has changed, work backwards and apply a turn to our velocity.