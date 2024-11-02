using UnityEngine;

namespace SupanthaPaul
{
	public class CameraFollow : MonoBehaviour
	{

		public static CameraFollow Instance; 

	    [SerializeField]
		private Transform target;
		[SerializeField]
		private float smoothSpeed = 0.125f;
		public Vector3 offset;
		[Header("Camera bounds")]
		public Vector3 minCamerabounds;
		public Vector3 maxCamerabounds;

		//bool configured = false;

		//private List<PlayerController> playerControllers;

        private void Awake()
        {
			if (Instance == null) Instance = this;
        }


  //      private void Update()
  //      {
		//	if (configured) return;

		//	if(playerControllers.Count < 1)
  //          {
		//		StorePlayers();
		//		return;
  //          }

		//	UpdateCameraTarget();
  //      }

  //      private void UpdateCameraTarget()
  //      {
		//	foreach(PlayerController playerController in playerControllers)
  //          {
		//		if(playerController.OwnerClientId == NetworkManager.Singleton.LocalClientId)
  //              {
		//			configured = true;
		//			SetTarget(playerController.transform);
  //              }
  //          }
		//}

  //      private void StorePlayers()
  //      {
		//	PlayerController[] playerControllerArray = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);

		//	if (playerControllerArray.Length <= 0)
		//		return;

		//	playerControllers = new List<PlayerController>(playerControllerArray);
		//}

        private void FixedUpdate()
		{
			if (target == null) return;

			//if (!configured) return;

			Vector3 desiredPosition = target.localPosition + offset;
			var localPosition = transform.localPosition;
			Vector3 smoothedPosition = Vector3.Lerp(localPosition, desiredPosition, smoothSpeed);
			localPosition = smoothedPosition;

			// clamp camera's position between min and max
			localPosition = new Vector3(
				Mathf.Clamp(localPosition.x, minCamerabounds.x, maxCamerabounds.x),
				Mathf.Clamp(localPosition.y, minCamerabounds.y, maxCamerabounds.y),
				Mathf.Clamp(localPosition.z, minCamerabounds.z, maxCamerabounds.z)
				);
			transform.localPosition = localPosition;
		}

		public void SetTarget(Transform targetToSet)
		{
			target = targetToSet;
		}
	}
}
