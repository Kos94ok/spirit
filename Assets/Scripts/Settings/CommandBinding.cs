using System.Collections.Generic;
using JetBrains.Annotations;
using UI;
using UI.UserInput;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace Settings {
	[UsedImplicitly]
	public class CommandBinding {

		public enum Command {
			MoveToMouse,
			ForceMoveToMouse,
			MoveUp,
			MoveDown,
			MoveLeft,
			MoveRight,
			Sprint,
			Blink,
			GalvanizeChatLog,
			AbilityQ,
			AbilityW,
			AbilityE,
			AbilityR,
		}

		private readonly Dictionary<Command, CommandMapping> Library = new Dictionary<Command, CommandMapping>();
		
		public CommandBinding() {
			Library.Add(Command.MoveToMouse, CommandMapping.Mouse(MouseStatus.Button.Left));
			Library.Add(Command.ForceMoveToMouse, CommandMapping.Mouse(MouseStatus.Button.Middle));
			Library.Add(Command.MoveUp, CommandMapping.Keyboard(KeyCode.W));
			Library.Add(Command.MoveDown, CommandMapping.Keyboard(KeyCode.S));
			Library.Add(Command.MoveLeft, CommandMapping.Keyboard(KeyCode.A));
			Library.Add(Command.MoveRight, CommandMapping.Keyboard(KeyCode.D));
			Library.Add(Command.Sprint, CommandMapping.Keyboard(KeyCode.LeftShift));
			Library.Add(Command.Blink, CommandMapping.Keyboard(KeyCode.Space));
			Library.Add(Command.GalvanizeChatLog, CommandMapping.Keyboard(KeyCode.Z));
			Library.Add(Command.AbilityQ, CommandMapping.Keyboard(KeyCode.Q));
			Library.Add(Command.AbilityW, CommandMapping.Keyboard(KeyCode.W));
			Library.Add(Command.AbilityE, CommandMapping.Keyboard(KeyCode.E));
			Library.Add(Command.AbilityR, CommandMapping.Keyboard(KeyCode.R));
		}
	
		public CommandMapping Get(Command command) {
			CommandMapping commandMapping;
			if (Library.TryGetValue(command, out commandMapping)) {
				return commandMapping;
			}
			return null;
		}
	}
}
