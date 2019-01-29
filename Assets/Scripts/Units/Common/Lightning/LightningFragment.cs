using UnityEngine;

namespace Units.Common.Lightning {
	public class LightningFragment {
		public const int VisualDataChildIndex = 0;
		public const int ConnectingPointChildIndex = 1;
		
		private GameObject FragmentObject;
		private Vector3 TargetPoint;
		private int LinearDepth;
		private int BranchDepth;
		private float CreatedTime;

		public void Init(GameObject fragmentObject, Vector3 targetPoint, int linearDepth, int branchDepth) {
			FragmentObject = fragmentObject;
			TargetPoint = targetPoint;
			LinearDepth = linearDepth;
			BranchDepth = branchDepth;
			CreatedTime = Time.time;
			fragmentObject.SetActive(true);
			fragmentObject.transform.GetChild(VisualDataChildIndex).gameObject.SetActive(true);
		}

		public GameObject GetGameObject() {
			return FragmentObject;
		}

		public Vector3 GetTargetPoint() {
			return TargetPoint;
		}

		public int GetLinearDepth() {
			return LinearDepth;
		}

		public int GetBranchDepth() {
			return BranchDepth;
		}

		public float GetTime() {
			return CreatedTime;
		}
	}
}