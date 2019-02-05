using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Misc {
	public enum Prefab {
		HeightIndicator,
		TargetIndicatorAlly,
		TargetIndicatorEnemy,
		TargetIndicatorNeutral,
		TargetIndicatorPosition,
		LightningEffectFragment,
		LongLightningEffectFragment,
		PinkLongLightningEffectFragment,
		RunningLightningEffectFragment,
		GenericProjectile,
		ProjectileArrow,
	}

	public enum Texture {
		HealthBarEmpty,
		HealthBarFull,
	}

	[UsedImplicitly]
	public class Assets {
		private readonly Dictionary<Prefab, string> Library = new Dictionary<Prefab, string>();
		private readonly Dictionary<Texture, string> TextureLibrary = new Dictionary<Texture, string>();

		public Assets() {
			Library.Add(Prefab.HeightIndicator, "Indicators/HeightIndicator");
			Library.Add(Prefab.TargetIndicatorAlly, "Indicators/TargetIndicatorAlly");
			Library.Add(Prefab.TargetIndicatorEnemy, "Indicators/TargetIndicatorEnemy");
			Library.Add(Prefab.TargetIndicatorNeutral, "Indicators/TargetIndicatorNeutral");
			Library.Add(Prefab.TargetIndicatorPosition, "Indicators/TargetIndicatorPosition");
			Library.Add(Prefab.LightningEffectFragment, "EffectFragments/LightningEffectFragment");
			Library.Add(Prefab.LongLightningEffectFragment, "EffectFragments/LongLightningEffectFragment");
			Library.Add(Prefab.PinkLongLightningEffectFragment, "EffectFragments/PinkLongLightningEffectFragment");
			Library.Add(Prefab.RunningLightningEffectFragment, "EffectFragments/RunningLightningEffectFragment");
			Library.Add(Prefab.GenericProjectile, "Projectiles/GenericProjectile");
			Library.Add(Prefab.ProjectileArrow, "Projectiles/ProjectileArrow");
			
			TextureLibrary.Add(Texture.HealthBarEmpty, "HealthBarBackground");
			TextureLibrary.Add(Texture.HealthBarFull, "HealthBarForeground");
		}

		public Object Get(Prefab prefab) {
			return Resources.Load(GetPath(prefab));
		}
		
		public string GetPath(Prefab prefab) {
			string path;
			if (Library.TryGetValue(prefab, out path)) {
				return path;
			}

			return prefab.ToString();
		}
		
		public Texture2D Get(Texture texture) {
			return Resources.Load(GetPath(texture)) as Texture2D;
		}

		public string GetPath(Texture texture) {
			string path;
			if (TextureLibrary.TryGetValue(texture, out path)) {
				return path;
			}

			return texture.ToString();
		}
	}
}