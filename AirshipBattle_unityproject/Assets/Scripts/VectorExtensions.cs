using UnityEngine;

namespace Assets.Scripts
{
    static class VectorExtensions
    {
        /// <summary>
        /// Gets the angle between a and b after first projecting them onto the x-z plane
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float GetHeadingDelta(Vector3 a, Vector3 b)
        {
            a = Vector3.ProjectOnPlane(a, Vector3.up);
            b = Vector3.ProjectOnPlane(b, Vector3.up);

            return Vector3.Angle(a, b);
        }
        /// <summary>
        /// Gets the pitch difference between two vectors
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static float GetPitchDelta(Vector3 from, Vector3 to)
        {
            from = from.normalized;
            to = to.normalized;

            float aTheta = Mathf.Acos(from.y);
            float bTheta = Mathf.Acos(to.y);

            return (aTheta - bTheta) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// rotates one vector towards another on the x-z plane while maintaining the y component.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="maxAngleDelta"></param>
        /// <param name="maxMagnitudeDelta"></param>
        /// <returns></returns>
        public static Vector3 RotateTowardsFlat(Vector3 from, Vector3 to, float maxDegreesDelta)
        {
            float y = from.y;   //cache the old y value
            Vector3 flat = Vector3.RotateTowards(from.Flatten(), to.Flatten(), maxDegreesDelta * Mathf.Deg2Rad, 0f);
            flat.y = y; //realod cached y value

            return flat;
        }
        /// <summary>
        /// flattens this vector onto the x-z plane
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3 Flatten(this Vector3 vector)
        {
            return Vector3.ProjectOnPlane(vector, Vector3.up);
        }

  
    }
}
