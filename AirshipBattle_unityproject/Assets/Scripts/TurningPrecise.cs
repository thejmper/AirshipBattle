using UnityEngine;

namespace Assets.Scripts
{
    class TurningPrecise : MonoBehaviour
    {
        //used in lieu of time.deltaTime.
        //public const float TIME_FRACTION = 0.1f;

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
        /// how fast (in degrees/second) our heading can change.
        /// </summary>
        public float maxHeadingSteerSpeed;
        /// <summary>
        /// how fast we can change our angular velocity (in degrees/second^2)
        /// </summary>
        public float headingSteerAccel;

        //--private variables--//        
        ///<summary>
        ///direction we're moving
        /// </summary>
        private Vector3 velocity = Vector3.forward;
        /// <summary>
        /// direction we'd like to face!
        /// </summary>
        private Vector3 facing = Vector3.forward;

        //--monomethods--//
        private void Update()
        {
            Vector3 desiredVelocity = target.position - transform.position;
            SteeringModule(desiredVelocity);
        }

        //--modules--//
        /// <summary>
        /// determines the required heading delta to pull onto the desired course
        /// </summary>
        /// <param name="desiredVelocity"></param>
        /// <returns>the direction our FACING should point</returns>
        private float HeadingModule(Vector3 desiredVelocity)
        {
            //flatten things so we don't get stray y-axis stuff causing trouble.
            Vector3 desiredHeading = desiredVelocity.Flatten();
            Vector3 currentHeading = velocity.Flatten();

            //get the normal vector, but don't allow it to rotate all the way, as that'll cause problems if our desired bearing
            //is *exactly* behind us.
            Vector3 normal = Vector3.Cross(currentHeading, Vector3.RotateTowards(currentHeading, desiredHeading, 1.57f, 0f)).normalized;

            //get the heading delta (in degrees)
            float headingDelta = Vector3.Angle(currentHeading, desiredHeading) * headingSteerConstant;
            if (normal.y < 0)
                headingDelta = -headingDelta;
            return headingDelta;
        }
        /// <summary>
        /// applies a change to our facing.
        /// </summary>
        /// <param name="headingDelta"></param>
        private void TurnFacingModule(float headingDelta)
        {
            Vector3 desiredFacing = Quaternion.AngleAxis(headingDelta, Vector3.up) * this.velocity;

            this.facing = Vector3.RotateTowards(this.velocity, desiredFacing, maxHeadingSteerSpeed * Time.deltaTime, 0f);
        }
        /// <summary>
        /// applies forces to the object. Should happen at the very end.
        /// </summary>
        private void AppliedForcesModule()
        {
            float headingAoA = Vector3.Angle(facing.Flatten(), velocity.Flatten()); //velocity off-bore compared to facing
            float rotateAmount = (headingAoA / headingSteerConstant);               //angular velocity in deg/sec

            this.velocity = VectorExtensions.RotateTowardsFlat(velocity, facing, rotateAmount * Time.deltaTime);

        }

        /// <summary>
        /// master module that runs all the steering sub-modules in the proper order.
        /// </summary>
        /// <param name="desiredVelocity"></param>
        private void SteeringModule(Vector3 desiredVelocity)
        {
            float headingDelta = HeadingModule(desiredVelocity);
            TurnFacingModule(headingDelta);
            AppliedForcesModule();
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