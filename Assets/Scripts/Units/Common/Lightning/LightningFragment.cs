using System;
using System.Collections;
using Misc;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Units.Common.Lightning {
	public class LightningFragment {
		private GameObject FragmentObject;
		private int Depth;
		private float CreatedTime;
		private bool SideBranch;

		public void Init(GameObject fragmentObject, int depth) {
			FragmentObject = fragmentObject;
			Depth = depth;
			SideBranch = false;
			CreatedTime = Time.time;
		}
		
		public void InitSideBranch(GameObject fragmentObject, int depth) {
			FragmentObject = fragmentObject;
			Depth = depth;
			SideBranch = true;
			CreatedTime = Time.time;
		}

		public GameObject GetGameObject() {
			return FragmentObject;
		}
		
		public int GetDepth() {
			return Depth;
		}
		
		public bool IsSideBranch() {
			return SideBranch;
		}

		public float GetTime() {
			return CreatedTime;
		}
	}
}