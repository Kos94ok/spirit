using System;
using Settings;
using UnityEngine;

namespace UI.ChatLog {
	public class ChatLogMessageQueueEntry {
		private readonly string text;
		private readonly ChatWindow chatWindow;

		public ChatLogMessageQueueEntry(string text, ChatWindow chatWindow) {
			this.text = text;
			this.chatWindow = chatWindow;
		}

		public string Text {
			get { return text; }
		}

		public ChatWindow ChatWindow {
			get { return chatWindow; }
		}
	}
}
