using System.Collections;
using Misc;
using Misc.ObjectPool;
using UnityEngine;

namespace Units.Common.Lightning {
	public class LightningAgent : MonoBehaviour {

		public class Builder {
			private readonly Vector3 From;
			private readonly Vector3 To;
			private float Speed = Mathf.Infinity;
			private float AngularDeviation = 1f;
			private float BranchingChance;
			private float BranchingFactor = 1f;
			private int MaximumBranchDepth = 3;
			
			public Builder(Vector3 from, Vector3 to) {
				From = from;
				To = to;
			}

			public Builder SetSpeed(float speed) {
				Speed = speed;
				return this;
			}

			public Builder SetAngularDeviation(float deviation) {
				AngularDeviation = deviation;
				return this;
			}
			
			public Builder SetBranchingChance(float chance) {
				BranchingChance = chance;
				return this;
			}

			public Builder SetBranchingFactor(float factor) {
				BranchingFactor = factor;
				return this;
			}

			public Builder SetMaximumBranchDepth(int maximumDepth) {
				MaximumBranchDepth = maximumDepth;
				return this;
			}

			public LightningAgent Create() {
				var lightningContainer = new GameObject();
				var lightningAgent = lightningContainer.AddComponent<LightningAgent>();
				lightningContainer.name = "LightningAgent";
				lightningContainer.AddComponent<TimedLife>().Timer = 5f;
				lightningAgent.Init(From, To, 1 / Speed, AngularDeviation, BranchingFactor, BranchingChance, MaximumBranchDepth);
				return lightningAgent;
			}
		}
		private readonly Assets Assets = AutowireFactory.GetInstanceOf<Assets>();

		private readonly ObjectPool ObjectPool = AutowireFactory.GetInstanceOf<ObjectPool>();
		
		private float FragmentDelay;
		private float AngularDeviation;
		private float BranchingFactor;
		private float BranchingChance;
		private float BranchingChanceReduction;
		
		private void Init(
				Vector3 from,
				Vector3 to,
				float delay,
				float deviation,
				float branchingFactor,
				float branchingChance,
				int maximumBranchDepth) {
			FragmentDelay = delay;
			AngularDeviation = deviation;
			BranchingFactor = branchingFactor;
			BranchingChance = branchingChance;
			BranchingChanceReduction = branchingChance / maximumBranchDepth;

			AddFragment(from, to, 0, 0, 1);
		}
		
		private void AddSideFragment(Vector3 from, Vector3 to, int linearDepth, int branchDepth) {
			AddFragment(from, from + Random.insideUnitSphere * Random.Range(0.1f, 0.3f * BranchingFactor), linearDepth, branchDepth, Mathf.RoundToInt(Random.Range(1, 4)));
		}
		
		private void AddFragment(Vector3 from, Vector3 to, int linearDepth, int branchDepth, int fragmentCount) {
			while (fragmentCount > 0 && Vector3.Distance(from, to) > 0.1f) {
				var fragment = ObjectPool.Obtain(Resource.LightningEffectFragment);
				var fragmentController = new LightningFragment();
				fragmentController.Init(fragment, to, linearDepth, branchDepth);
				fragment.transform.position = from;
				fragment.transform.LookAt(to);
				fragment.transform.Rotate(Vector3.right, 90f);
				
				var xSpread = RandomExtensions.NextGaussian(0, AngularDeviation, -90, 90);
				var ySpread = RandomExtensions.NextGaussian(0, AngularDeviation, -90, 90);
				var spread = new Vector3(xSpread, ySpread, 0.0f);
				fragment.transform.rotation *= Quaternion.Euler(spread);
				
				StartCoroutine(FragmentHideVisual(fragment));
				StartCoroutine(FragmentSelfDestruct(fragment));
				if (fragmentCount == 1 && linearDepth < 500) {
					StartCoroutine(FragmentSpawning(fragmentController));
				}
				
				if (Random.Range(0f, 1f) <= BranchingChance - branchDepth * BranchingChanceReduction) {
					AddSideFragment(from, to, linearDepth + 1, branchDepth + 1);
				}

				from = fragment.transform.GetChild(LightningFragment.ConnectingPointChildIndex).position;
				fragmentCount -= 1;
			}
		}
		
		private IEnumerator FragmentHideVisual(GameObject fragment) {
			yield return new WaitForSeconds(.1f);
			fragment.transform.GetChild(LightningFragment.VisualDataChildIndex).gameObject.SetActive(false);
		}

		private IEnumerator FragmentSelfDestruct(GameObject fragment) {
			yield return new WaitForSeconds(FragmentDelay + .5f);
			fragment.SetActive(false);
			ObjectPool.Return(Resource.LightningEffectFragment, fragment);
		}

		private IEnumerator FragmentSpawning(LightningFragment fragment) {
			yield return new WaitForSeconds(FragmentDelay);
			
			var spawnPosition = fragment.GetGameObject().transform.GetChild(LightningFragment.ConnectingPointChildIndex).position;
			var projectilesToSpawn = Mathf.FloorToInt(Mathf.Clamp((Time.time - fragment.GetTime()) / FragmentDelay, 1, 150));
			AddFragment(spawnPosition, fragment.GetTargetPoint(), fragment.GetLinearDepth()+1, fragment.GetBranchDepth(), projectilesToSpawn);
		}

	}
}