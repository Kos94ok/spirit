using JetBrains.Annotations;
using UnityEngine;

namespace UI.ChatLog {
	public enum ChatWindow {
		Info,
		Error,
	}

	[UsedImplicitly]
	public class ChatLog {
		private ChatLogAgent ChatLogAgent;
		private readonly Localization Localization = AutowireFactory.GetInstanceOf<Localization>();

		public void PostRaw(string text, ChatWindow window = ChatWindow.Info) {
			if (ChatLogAgent == null) {
				var chatLogAgentHolder = new GameObject();
				ChatLogAgent = (ChatLogAgent) chatLogAgentHolder.AddComponent(typeof(ChatLogAgent));
			}

			ChatLogAgent.PostMessage(text, window);
		}
		public void Post(string link, ChatWindow window = ChatWindow.Info) {
			PostRaw(Localization.Get(link));
		}
	}
}