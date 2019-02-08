using System;
using System.Collections;
using System.Collections.Generic;
using Misc;
using Misc.ObjectPool;
using Settings;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Units.Common.Lightning {
	public class LightningAgent : MonoBehaviour {

		public class Builder {
			private readonly Vector3 From;
			private readonly Vector3 To;
			private readonly Maybe<GameObject> TargetUnit;
			private float Speed = Mathf.Infinity;
			private float AngularDeviation = 1f;
			private float BranchingChance;
			private float BranchingFactor = 1f;
			private int MaximumBranchDepth = 3;
			private Prefab FragmentPrefab = Prefab.LightningEffectFragment;
			private float FragmentLifeTime = .1f;
			private float FragmentParticleLifeTime = .5f;
			private float SmoothFactor = .5f;
			private Vector3 StartingOffset = Vector3.zero;
			private Maybe<Action<TargetUnitReachedCallbackPayload>> TargetUnitReachedCallback = Maybe<Action<TargetUnitReachedCallbackPayload>>.None;
			private Maybe<Action<TargetPointReachedCallbackPayload>> TargetPointReachedCallback = Maybe<Action<TargetPointReachedCallbackPayload>>.None;
			
			public Builder(Vector3 from, Vector3 to) {
				From = from;
				To = to;
				TargetUnit = Maybe<GameObject>.None;
			}

			public Builder(Vector3 from, Vector3 to, Maybe<GameObject> target) {
				From = from;
				To = to;
				TargetUnit = target;
			}

			public Builder(Vector3 from, GameObject target) {
				From = from;
				var targetStats = target.GetComponent<UnitStats>();
				To = targetStats.GetHitTargetPosition();
				TargetUnit = Maybe<GameObject>.Some(target);
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

			public Builder SetFragmentResource(Prefab prefab) {
				FragmentPrefab = prefab;
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

			public Builder SetStartingOffset(Vector3 offset) {
				StartingOffset = offset;
				return this;
			}

			public Builder SetTargetUnitReachedCallback(Action<TargetUnitReachedCallbackPayload> callback) {
				TargetUnitReachedCallback = Maybe<Action<TargetUnitReachedCallbackPayload>>.Some(callback);
				return this;
			}
			
			public Builder SetTargetPointReachedCallback(Action<TargetPointReachedCallbackPayload> callback) {
				TargetPointReachedCallback = Maybe<Action<TargetPointReachedCallbackPayload>>.Some(callback);
				return this;
			}

			public LightningAgent Create() {
				var lightningContainer = new GameObject();
				var lightningAgent = lightningContainer.AddComponent<LightningAgent>();
				lightningContainer.name = "LightningAgent";
				lightningAgent.Init(From, To, TargetUnit, 1 / Speed, AngularDeviation, BranchingFactor,
					BranchingChance, MaximumBranchDepth, FragmentPrefab, FragmentLifeTime,
					FragmentParticleLifeTime, SmoothFactor, StartingOffset, TargetUnitReachedCallback, TargetPointReachedCallback);
				return lightningAgent;
			}
		}

		public struct TargetUnitReachedCallbackPayload {
			public GameObject TargetUnit;
			public UnitStats TargetStats;
		}

		public struct TargetPointReachedCallbackPayload {
			public Vector3 TargetPoint;
		}
		
		private readonly ObjectPool ObjectPool = AutowireFactory.GetInstanceOf<ObjectPool>();
		private readonly VisualSettings VisualSettings = AutowireFactory.GetInstanceOf<VisualSettings>();

		private Maybe<GameObject> TargetUnit;
		private Vector3 TargetPoint;
		private float StartingDistance;
		private float FragmentDelay;
		private float AngularDeviation;
		private float BranchingFactor;
		private float BranchingChance;
		private float BranchingChanceReduction;
		private Prefab FragmentPrefab;
		private float FragmentLifeTime;
		private float FragmentParticleLifeTime;
		private float Sharpness;
		private Maybe<Action<TargetUnitReachedCallbackPayload>> TargetUnitReachedCallback;
		private Maybe<Action<TargetPointReachedCallbackPayload>> TargetPointReachedCallback;
		
		private float FloorLevel;
		private float TerminationDistance; 
		
		private readonly List<LightningFragment> RegisteredFragments = new List<LightningFragment>();
		
		private void Init(
				Vector3 from,
				Vector3 to,
				Maybe<GameObject> targetUnit,
				float delay,
				float deviation,
				float branchingFactor,
				float branchingChance,
				int maximumBranchDepth,
				Prefab fragmentPrefab,
				float fragmentLifeTime,
				float fragmentParticleLifeTime,
				float smoothFactor,
				Vector3 startingOffset,
				Maybe<Action<TargetUnitReachedCallbackPayload>> targetUnitReachedCallback,
				Maybe<Action<TargetPointReachedCallbackPayload>> targetPointReachedCallback) {
			TargetUnit = targetUnit;
			TargetPoint = to;
			StartingDistance = Vector3.Distance(from, to);
			FragmentDelay = delay * GetLightningLengthModifier();
			AngularDeviation = deviation;
			BranchingFactor = branchingFactor;
			BranchingChance = branchingChance;
			BranchingChanceReduction = branchingChance / maximumBranchDepth;
			FragmentPrefab = fragmentPrefab;
			FragmentLifeTime = fragmentLifeTime;
			FragmentParticleLifeTime = fragmentParticleLifeTime;
			Sharpness = 1 - Mathf.Clamp(smoothFactor, 0, 1);
			TargetUnitReachedCallback = targetUnitReachedCallback;
			TargetPointReachedCallback = targetPointReachedCallback;
			
			FloorLevel = Mathf.Min(Utility.GetGroundPosition(from).y, Utility.GetGroundPosition(to).y);

			var fragmentObject = ObjectPool.Obtain(FragmentPrefab);
			TerminationDistance = fragmentObject.transform.GetChild(LightningFragment.ConnectingPointChildIndex).transform.localPosition.y;
			ObjectPool.Return(FragmentPrefab, fragmentObject);

			if (!IsLightningBranchingEnabled()) {
				BranchingChance = 0;
			}

			AddFragment(from, to, 0, 0, 1, startingOffset);
			StartCoroutine(AgentSelfDestruct());
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
				var fragment = ObjectPool.Obtain(FragmentPrefab);
				var fragmentController = new LightningFragment();
				fragmentController.Init(fragment, to, linearDepth, branchDepth);
				fragment.transform.position = from;
				fragment.transform.LookAt(to);
				fragment.transform.Rotate(Vector3.right, 90f);
				RegisteredFragments.Add(fragmentController);

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
				fragment.transform.localScale = new Vector3(scaleMod, GetLightningLengthModifier(), scaleMod);
				if (IsLightningLightEnabled()) {
					fragment.GetComponentInChildren<Light>().intensity = 0.5f * scaleMod;
				} else {
					fragment.GetComponentInChildren<Light>().enabled = false;
				}

				StartCoroutine(FragmentHideVisual(fragmentController));
				StartCoroutine(FragmentSelfDestruct(fragmentController));
				if (fragmentsRemaining == 1 && linearDepth < 100) {
					StartCoroutine(FragmentSpawning(fragmentController));
				}
				
				if (Random.Range(0f, 1f) <= BranchingChance - branchDepth * BranchingChanceReduction) {
					AddSideFragment(from, to, linearDepth + 1, branchDepth + 1, fragmentCount, offsetFromTarget);
				}

				from = connectingPointObject.position;
				fragmentsRemaining -= 1;
			}

			if (Vector3.Distance(from, to) > TerminationDistance || branchDepth > 0) {
				return;
			}

			if (TargetUnitReachedCallback.HasValue && TargetUnit.HasValue) {
				var payload = new TargetUnitReachedCallbackPayload {
					TargetUnit = TargetUnit.Value,
					TargetStats = TargetUnit.Value.GetComponent<UnitStats>()
				};
				
				TargetUnitReachedCallback.Value(payload);
				TargetUnitReachedCallback = Maybe<Action<TargetUnitReachedCallbackPayload>>.None;
			} else if (TargetPointReachedCallback.HasValue) {
				var payload = new TargetPointReachedCallbackPayload {
					TargetPoint = TargetPoint
				};

				TargetPointReachedCallback.Value(payload);
				TargetPointReachedCallback = Maybe<Action<TargetPointReachedCallbackPayload>>.None;
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
		
		private IEnumerator FragmentHideVisual(LightningFragment fragment) {
			yield return new WaitForSeconds(FragmentLifeTime);
			fragment.GetGameObject().transform.GetChild(LightningFragment.VisualDataChildIndex).gameObject.SetActive(false);
		}

		private IEnumerator FragmentSelfDestruct(LightningFragment fragment) {
			yield return new WaitForSeconds(FragmentDelay + FragmentParticleLifeTime);

			RegisteredFragments.Remove(fragment);
			ObjectPool.Return(FragmentPrefab, fragment.GetGameObject());
		}
		
		private IEnumerator AgentSelfDestruct() {
			yield return new WaitForSeconds(4);
			
			foreach (var fragment in RegisteredFragments) {
				ObjectPool.Return(FragmentPrefab, fragment.GetGameObject());
			}
			Destroy(transform.gameObject);
		}

		private IEnumerator FragmentSpawning(LightningFragment fragment) {
			yield return new WaitForSeconds(FragmentDelay);
			
			var spawnPosition = fragment.GetGameObject().transform.GetChild(LightningFragment.ConnectingPointChildIndex).position;
			var fragmentCount = Mathf.FloorToInt(Mathf.Clamp((Time.time - fragment.GetTime()) / FragmentDelay, 1, 150));
			AddFragment(spawnPosition, fragment.GetTargetPoint(), fragment.GetLinearDepth() + 1, fragment.GetBranchDepth(), fragmentCount, fragment.GetOffsetFromTarget());
		}

		private bool IsLightningBranchingEnabled() {
			return VisualSettings.GetQuality(VisualSettings.Option.LightningQuality) >= VisualSettings.Quality.Medium;
		}
		
		private bool IsLightningLightEnabled() {
			return VisualSettings.GetQuality(VisualSettings.Option.LightningQuality) >= VisualSettings.Quality.High;
		}

		private float GetLightningLengthModifier() {
			var selectedQuality = VisualSettings.GetQuality(VisualSettings.Option.LightningQuality);
			switch (selectedQuality) {
				case VisualSettings.Quality.Ultra:
					return 0.5f;
				case VisualSettings.Quality.High:
					return 0.8f;
				default:
					return 1f;
			}
		}
	}
}