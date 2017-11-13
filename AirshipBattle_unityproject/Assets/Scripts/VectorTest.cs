using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// quick and dirty class used to test behavior of a vector extention method
    /// </summary>
    class VectorTest: MonoBehaviour
    {
        public Transform A;
        public Transform B;

        private void OnGUI()
        {
            Vector3 offsetA = A.position - transform.position;
            Vector3 offsetB = B.position - transform.position;

            GUI.Box(new Rect(10, 10, 200, 45), "pitch diff: " + VectorExtensions.GetPitchDelta(offsetA, offsetB));
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(this.transform.position, A.position);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(this.transform.position, B.position);
        }
    }
}
