using JetBrains.Annotations;
using UnityEngine;

namespace UI.ChatLog {
	public enum ChatWindow {
		Info,
		Error,
	}

	[UsedImplicitly]
	public class ChatLog {
		private ChatLogAgent chatLogAgent;
		private readonly Localization localization = AutowireFactory.GetInstanceOf<Localization>();

		public void PostRaw(string text, ChatWindow window = ChatWindow.Info) {
			if (chatLogAgent == null) {
				var chatLogAgentHolder = new GameObject();
				chatLogAgent = (ChatLogAgent) chatLogAgentHolder.AddComponent(typeof(ChatLogAgent));
			}

			chatLogAgent.PostMessage(text, window);
		}
		public void Post(string link, ChatWindow window = ChatWindow.Info) {
			PostRaw(localization.Get(link));
		}
	}
}