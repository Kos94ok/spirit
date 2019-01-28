using Misc;
using UnityEngine;

namespace Units.Common.Lightning {
	public class LightningAgent : MonoBehaviour {

		private readonly Assets Assets = AutowireFactory.GetInstanceOf<Assets>();
		
		private Vector3 startingPoint;
		private Vector3 targetPoint;
		private float fragmentDelay;
		public void Init(Vector3 from, Vector3 to, float delay) {
			startingPoint = from;
			targetPoint = to;
			fragmentDelay = delay;

			AddFragment(from, 150);
		}

		public static void Create(Vector3 from, Vector3 to, float delay) {
			var lightningContainer = new GameObject();
			var lightningAgent = lightningContainer.AddComponent<LightningAgent>();
			lightningContainer.AddComponent<TimedLife>().Timer = 1.00f;
			lightningAgent.Init(from, to, delay);
		}

		public void AddFragment(Vector3 from, int maxDepth) {
			var fragment = (GameObject) Instantiate(Assets.Get(Resource.LightningEffectFragment));
			var fragmentController = fragment.GetComponent<LightningFragment>();
			fragmentController.Init(this, targetPoint, maxDepth, fragmentDelay);
			fragment.transform.position = from;
			fragment.transform.LookAt(targetPoint);
			fragment.transform.Rotate(Vector3.right, 90f);
			
			float xSpread = Random.Range(-60, 60);
			float ySpread = Random.Range(-60, 60);
			//Vector3 spread = new Vector3(xSpread, ySpread, 0.0f).normalized * ConeSize;
			var spread = new Vector3(xSpread, ySpread, 0.0f);
			//Quaternion rotation = Quaternion.Euler(spread) * fragment.transform.rotation;
			fragment.transform.rotation *= Quaternion.Euler(spread);
			
			
			//fragment.transform.position = from;
			//fragment.transform.Rotate(Vector3.right * Random.Range(-90f, 90f));
			//fragment.transform.Rotate(Vector3.up * Random.Range(-90f, 90f));
		}
	}
}