using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Misc {
	public enum Resource {
		HeightIndicator,
		TargetIndicatorAlly,
		TargetIndicatorEnemy,
		TargetIndicatorNeutral,
		TargetIndicatorPosition,
		LightningEffectFragment,
	}

	[UsedImplicitly]
	public class Assets {
		private readonly Dictionary<Resource, string> Library = new Dictionary<Resource, string>();

		public Assets() {
			Library.Add(Resource.HeightIndicator, "Indicators/HeightIndicator");
			Library.Add(Resource.TargetIndicatorAlly, "Indicators/TargetIndicatorAlly");
			Library.Add(Resource.TargetIndicatorEnemy, "Indicators/TargetIndicatorEnemy");
			Library.Add(Resource.TargetIndicatorNeutral, "Indicators/TargetIndicatorNeutral");
			Library.Add(Resource.TargetIndicatorPosition, "Indicators/TargetIndicatorPosition");
			Library.Add(Resource.LightningEffectFragment, "EffectFragments/LightningEffectFragment");
		}

		public Object Get(Resource resource) {
			return Resources.Load(GetPath(resource));
		}
		
		public string GetPath(Resource resource) {
			string path;
			if (Library.TryGetValue(resource, out path)) {
				return path;
			}

			return resource.ToString();
		}
	}
}