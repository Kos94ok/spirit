using System.Collections.Generic;
using JetBrains.Annotations;

namespace Settings {
	[UsedImplicitly]
	public class VisualSettings {
		public static class Quality {
			public const int Low = 1;
			public const int Medium = 2;
			public const int High = 3;
			public const int Ultra = 4;
		}

		public enum Option {
			LightningQuality,
		}

		private readonly Dictionary<Option, int> Presets = new Dictionary<Option, int>();

		public VisualSettings() {
			Presets.Add(Option.LightningQuality, Quality.Ultra);
		}
		
		public int GetQuality(Option option) {
			int level;
			if (Presets.TryGetValue(option, out level)) {
				return level;
			}

			return Quality.Medium;
		}
	}
}