using UnityEngine;

namespace Complete
{
    public class UIDirectionControl : MonoBehaviour
    {
        // This class is used to make sure world space UI
        // elements such as the health bar face the correct direction.

		public Vector3 offset = Vector3.zero;
		public Transform target = null;          // The local rotatation at the start of the scene.

        private void Update ()
        {
			transform.rotation = Quaternion.Euler (target.localRotation.eulerAngles.x + offset.x, target.localRotation.eulerAngles.y  + offset.y, target.localRotation.eulerAngles.z  + offset.z);
        }
    }
}