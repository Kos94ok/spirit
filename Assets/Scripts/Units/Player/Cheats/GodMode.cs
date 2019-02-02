using Settings;
using UI.ChatLog;
using UI.UserInput;
using Units.Buffs;
using UnityEngine;

namespace Units.Player.Cheats {
	public class GodMode : MonoBehaviour {
		private UnitStats Stats;
		private readonly ChatLog ChatLog = AutowireFactory.GetInstanceOf<ChatLog>();
		private readonly CommandStatus CommandStatus = AutowireFactory.GetInstanceOf<CommandStatus>();

		private void Start() {
			Stats = GetComponent<UnitStats>();
		}
		
		private void Update() {
			if (CommandStatus.IsIssuedThisFrame(CommandBinding.Command.CheatGodMode) && !Stats.Buffs.Has(Buff.GodMode)) {
				Stats.Buffs.Add(Buff.GodMode);
				ChatLog.PostRaw("God mode enabled");
			} else if (CommandStatus.IsIssuedThisFrame(CommandBinding.Command.CheatGodMode) && Stats.Buffs.Has(Buff.GodMode)) {
				Stats.Buffs.Remove(Buff.GodMode);
				ChatLog.PostRaw("God mode disabled");
			}
		}
	}
}