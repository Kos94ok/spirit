using System;

namespace Units {
	public static class UnitRelationship {
		public const int Ally = 1 << 1;
		public const int Enemy = 1 << 2;
		public const int Neutral = 1 << 3;

		public static int GetRelationship(UnitAlliance first, UnitAlliance second) {
			switch (first) {
				case UnitAlliance.Player when second == UnitAlliance.Player:
					return Ally;
				case UnitAlliance.Player when second == UnitAlliance.Forgotten:
					return Neutral;
				case UnitAlliance.Player when second == UnitAlliance.Corruption:
					return Enemy;
				case UnitAlliance.Forgotten when second == UnitAlliance.Player:
					return Neutral;
				case UnitAlliance.Forgotten when second == UnitAlliance.Forgotten:
					return Neutral;
				case UnitAlliance.Forgotten when second == UnitAlliance.Corruption:
					return Neutral;
				case UnitAlliance.Corruption when second == UnitAlliance.Player:
					return Enemy;
				case UnitAlliance.Corruption when second == UnitAlliance.Forgotten:
					return Neutral;
				case UnitAlliance.Corruption when second == UnitAlliance.Corruption:
					return Ally;
				default:
					return 0;
			}
		}
	}
}