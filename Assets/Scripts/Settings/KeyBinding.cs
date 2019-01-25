using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Settings {
    [UsedImplicitly]
    public class KeyBinding {

        public enum Action {
            GalvanizeChatLog,
        }

        private readonly Dictionary<Action, KeyCode> library = new Dictionary<Action, KeyCode>();
        
        public KeyBinding() {
            library.Add(Action.GalvanizeChatLog, KeyCode.Z);
        }
	
        public KeyCode? Get(Action action) {
            KeyCode keyCode;
            if (library.TryGetValue(action, out keyCode)) {
                return keyCode;
            }

            return null;
        }
    }
}
