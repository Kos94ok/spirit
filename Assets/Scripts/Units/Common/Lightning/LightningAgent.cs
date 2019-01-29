using System;
using System.Collections;
using Misc;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Units.Common.Lightning {
	public class LightningAgent : MonoBehaviour {

		private readonly Assets Assets = AutowireFactory.GetInstanceOf<Assets>();
		
		private Vector3 StartingPoint;
		private Vector3 TargetPoint;
		private float FragmentDelay;
		private Object FragmentPrefab;
				
		public static void Create(Vector3 from, Vector3 to, float speed) {
			var lightningContainer = new GameObject();
			var lightningAgent = lightningContainer.AddComponent<LightningAgent>();
			lightningContainer.AddComponent<TimedLife>().Timer = 5f;
			lightningAgent.Init(from, to, 1 / speed);
		}
		
		public void Init(Vector3 from, Vector3 to, float delay) {
			StartingPoint = from;
			TargetPoint = to;
			FragmentDelay = delay;
			FragmentPrefab = Assets.Get(Resource.LightningEffectFragment);

			AddFragment(from, 0, 1);
		}

		public void AddFragment(Vector3 from, int depth, int fragmentCount) {
			AddFragment(from, TargetPoint, depth, fragmentCount);
		}
		
		public void AddSideFragment(Vector3 from, int depth) {
			AddFragment(from, from + Random.insideUnitSphere * 10f, depth, Mathf.RoundToInt(Random.Range(1, 4)));
		}
		
		private void AddFragment(Vector3 from, Vector3 to, int depth, int fragmentCount) {
			while (fragmentCount > 0 && Vector3.Distance(from, to) > 0.05f) {
				var fragment = (GameObject) Instantiate(FragmentPrefab);
				var fragmentController = new LightningFragment();
				fragmentController.Init(fragment, depth++);
				fragment.transform.position = from;
				fragment.transform.LookAt(to);
				fragment.transform.Rotate(Vector3.right, 90f);
				
				var xSpread = NextGaussian(0, 60f);
				var ySpread = NextGaussian(0, 60f);
				var spread = new Vector3(xSpread, ySpread, 0.0f);
				fragment.transform.rotation *= Quaternion.Euler(spread);
				
				StartCoroutine(FragmentSelfDestruct(fragment, FragmentDelay));
				if (fragmentCount == 1 && depth < 500) {
					StartCoroutine(FragmentSpawning(fragmentController, FragmentDelay));
				}

				depth += 1;
				from = fragment.transform.GetChild(1).position;
				fragmentCount -= 1;
			}
		}

		private IEnumerator FragmentSelfDestruct(GameObject fragment, float delay) {
			yield return new WaitForSeconds(delay + 0.15f);
			Destroy(fragment.transform.gameObject);
		}

		private IEnumerator FragmentSpawning(LightningFragment fragment, float delay) {
			yield return new WaitForSeconds(delay);
			
			var spawnPosition = fragment.GetGameObject().transform.GetChild(1).position;
			var projectilesToSpawn = Mathf.FloorToInt(Mathf.Clamp((Time.time - fragment.GetTime()) / delay, 1, 150));
			AddFragment(spawnPosition, fragment.GetDepth(), projectilesToSpawn);
		}

		public static float NextGaussian() {
			float v1, v2, s;
			do {
				v1 = 2.0f * Random.Range(0f, 1f) - 1.0f;
				v2 = 2.0f * Random.Range(0f, 1f) - 1.0f;
				s = v1 * v1 + v2 * v2;
			} while (s >= 1.0f || s == 0f);

			s = Mathf.Sqrt((-2.0f * Mathf.Log(s)) / s);
 
			return v1 * s;
		}
		
		public static float NextGaussian(float mean, float standardDeviation) {
			return mean + NextGaussian() * standardDeviation;
		}
		
		public static float NextGaussian (float mean, float standardDeviation, float min, float max) {
			float x;
			do {
				x = NextGaussian(mean, standardDeviation);
			} while (x < min || x > max);
			return x;
		}
	}
}