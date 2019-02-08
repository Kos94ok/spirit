using Misc;
using UnityEngine;

namespace Units.Enemies {
	public interface IEnemyAI {
		void OnHit(float damage, Maybe<GameObject> source);
	}
}
