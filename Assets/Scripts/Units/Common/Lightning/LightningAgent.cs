using System.Collections;
using Misc;
using Misc.ObjectPool;
using Unity.Collections;
using Unity.Jobs;
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
			private Resource FragmentResource = Resource.LightningEffectFragment;
			private float FragmentLifeTime = .1f;
			private float FragmentParticleLifeTime = .5f;
			private float SmoothFactor = .5f;
			
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

			public Builder SetFragmentResource(Resource resource) {
				FragmentResource = resource;
				return this;
			}

			public Builder SetFragmentLifeTime(float lifeTime) {
				FragmentLifeTime = lifeTime;
				return this;
			}

			public Builder SetFragmentParticleLifeTime(float lifeTime) {
				FragmentParticleLifeTime = lifeTime;
				return this;
			}

			public Builder SetSmoothFactor(float smoothFactor) {
				SmoothFactor = smoothFactor;
				return this;
			}

			public LightningAgent Create() {
				var lightningContainer = new GameObject();
				var lightningAgent = lightningContainer.AddComponent<LightningAgent>();
				lightningContainer.name = "LightningAgent";
				lightningContainer.AddComponent<TimedLife>().Timer = 5f;
				lightningAgent.Init(From, To, 1 / Speed, AngularDeviation, BranchingFactor, BranchingChance, MaximumBranchDepth, FragmentResource, FragmentLifeTime, FragmentParticleLifeTime, SmoothFactor);
				return lightningAgent;
			}
		}
		
		private readonly ObjectPool ObjectPool = AutowireFactory.GetInstanceOf<ObjectPool>();

		private float StartingDistance;
		private float FragmentDelay;
		private float AngularDeviation;
		private float BranchingFactor;
		private float BranchingChance;
		private float BranchingChanceReduction;
		private Resource FragmentResource;
		private float FragmentLifeTime;
		private float FragmentParticleLifeTime;
		private float Sharpness;
		
		private float FloorLevel;
		private float TerminationDistance; 
		
		private void Init(
				Vector3 from,
				Vector3 to,
				float delay,
				float deviation,
				float branchingFactor,
				float branchingChance,
				int maximumBranchDepth,
				Resource fragmentResource,
				float fragmentLifeTime,
				float fragmentParticleLifeTime,
				float smoothFactor) {
			StartingDistance = Vector3.Distance(from, to);
			FragmentDelay = delay;
			AngularDeviation = deviation;
			BranchingFactor = branchingFactor;
			BranchingChance = branchingChance;
			BranchingChanceReduction = branchingChance / maximumBranchDepth;
			FragmentResource = fragmentResource;
			FragmentLifeTime = fragmentLifeTime;
			FragmentParticleLifeTime = fragmentParticleLifeTime;
			Sharpness = 1 - Mathf.Clamp(smoothFactor, 0, 1);
			
			FloorLevel = Mathf.Min(Utility.GetGroundPosition(from).y, Utility.GetGroundPosition(to).y);

			var fragmentObject = ObjectPool.Obtain(FragmentResource);
			TerminationDistance = fragmentObject.transform.GetChild(LightningFragment.ConnectingPointChildIndex).transform.localPosition.y;
			ObjectPool.Return(FragmentResource, fragmentObject);

			AddFragment(from, to, 0, 0, 1, Vector3.zero);
		}
		
		private void AddSideFragment(Vector3 from, Vector3 to, int linearDepth, int branchDepth, int fragmentCount, Vector3 previousOffset) {
			Vector3 targetPoint;
			do {
				targetPoint = from + Random.insideUnitSphere * Random.Range(0.1f, 1.0f * BranchingFactor);
			} while (targetPoint.y < FloorLevel);

			AddFragment(from, targetPoint, linearDepth, branchDepth, fragmentCount, previousOffset);
		}
		
		private void AddFragment(Vector3 from, Vector3 to, int linearDepth, int branchDepth, int fragmentCount, Vector3 previousOffset) {
			var fragmentsRemaining = fragmentCount;
			while (fragmentsRemaining > 0 && Vector3.Distance(from, to) > TerminationDistance) {
				var fragment = ObjectPool.Obtain(FragmentResource);
				var fragmentController = new LightningFragment();
				fragmentController.Init(fragment, to, linearDepth, branchDepth);
				fragment.transform.position = from;
				fragment.transform.LookAt(to);
				fragment.transform.Rotate(Vector3.right, 90f);

				var vectorToTarget = to - from;
				var distanceToTarget = vectorToTarget.magnitude;
				var preOffsetRotation = fragment.transform.rotation;
				var connectingPointObject = fragment.transform.GetChild(LightningFragment.ConnectingPointChildIndex);
				var offsetFromTarget = RotateToRandomGaussianOffset(fragment, preOffsetRotation, -90, 90, previousOffset, Sharpness);
				while (fragment.transform.GetChild(LightningFragment.ConnectingPointChildIndex).transform.position.y < FloorLevel) {
					offsetFromTarget = RotateToRandomGaussianOffset(fragment, preOffsetRotation, -90, 90, previousOffset, 1);
				}
				fragmentController.SetOffsetFromTarget(offsetFromTarget);

				var scaleMod = Mathf.Max(0.20f, distanceToTarget / StartingDistance);
				fragment.transform.localScale = new Vector3(scaleMod, 1f, scaleMod);
				fragment.GetComponentInChildren<Light>().intensity = 0.5f * scaleMod;
				
				StartCoroutine(FragmentHideVisual(fragment));
				StartCoroutine(FragmentSelfDestruct(fragment));
				if (fragmentsRemaining == 1 && linearDepth < 500) {
					StartCoroutine(FragmentSpawning(fragmentController));
				}
				
				if (Random.Range(0f, 1f) <= BranchingChance - branchDepth * BranchingChanceReduction) {
					AddSideFragment(from, to, linearDepth + 1, branchDepth + 1, fragmentCount, offsetFromTarget);
				}

				from = connectingPointObject.position;
				fragmentsRemaining -= 1;
			}
		}

		private Vector3 RotateToRandomGaussianOffset(GameObject fragment, Quaternion preOffsetRotation, float minAngle, float maxAngle, Vector3 previousOffset, float sharpness) {
			var xOffset = RandomExtensions.NextGaussian(0, AngularDeviation, minAngle, maxAngle);
			var yOffset = RandomExtensions.NextGaussian(0, AngularDeviation, minAngle, maxAngle);
			var zOffset = RandomExtensions.NextGaussian(0, AngularDeviation, minAngle, maxAngle);

			var offset = Vector3.Lerp(previousOffset, new Vector3(xOffset, yOffset, zOffset), sharpness);
			fragment.transform.rotation = preOffsetRotation * Quaternion.Euler(offset);
			return offset;
		}
		
		private IEnumerator FragmentHideVisual(GameObject fragment) {
			yield return new WaitForSeconds(FragmentLifeTime);
			fragment.transform.GetChild(LightningFragment.VisualDataChildIndex).gameObject.SetActive(false);
		}

		private IEnumerator FragmentSelfDestruct(GameObject fragment) {
			yield return new WaitForSeconds(FragmentDelay + FragmentParticleLifeTime);
			
			ObjectPool.Return(FragmentResource, fragment);
		}

		private IEnumerator FragmentSpawning(LightningFragment fragment) {
			yield return new WaitForSeconds(FragmentDelay);
			
			var spawnPosition = fragment.GetGameObject().transform.GetChild(LightningFragment.ConnectingPointChildIndex).position;
			var fragmentCount = Mathf.FloorToInt(Mathf.Clamp((Time.time - fragment.GetTime()) / FragmentDelay, 1, 150));
			AddFragment(spawnPosition, fragment.GetTargetPoint(), fragment.GetLinearDepth() + 1, fragment.GetBranchDepth(), fragmentCount, fragment.GetOffsetFromTarget());
		}
		
	}
}