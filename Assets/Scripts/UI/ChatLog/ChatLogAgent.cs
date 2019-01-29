using System.Collections.Generic;
using Settings;
using UI.UserInput;
using UnityEngine;
using Timer = Misc.Timer;

namespace UI.ChatLog {
	public class ChatLogAgent : MonoBehaviour {
		private readonly CommandStatus CommandStatus = AutowireFactory.GetInstanceOf<CommandStatus>();
		
		private const int HistoryLength = 32;
		private const float GalvanizationCooldown = 1.00f;
		private const float DegalvanizationCooldown = 0.5f;
		
		private readonly Vector2 WindowPosition = new Vector2(50f, 50f);
		private readonly Vector2 WindowSize = new Vector2(500f, 600f);
		
		private readonly Timer GalvanizationCooldownTimer = new Timer();
		private readonly Timer DegalvanizationCooldownTimer = new Timer();
		private static bool IsGalvanized;

		private static Rect WindowRect;
		private static GUISkin ChatLogMessageSkin;
		private static readonly List<ChatLogMessage> MessageList = new List<ChatLogMessage>();
		private static readonly Queue<ChatLogMessageQueueEntry> MessageQueue = new Queue<ChatLogMessageQueueEntry>();
		private void Start() {
			WindowRect = new Rect(WindowPosition.x, Screen.height - WindowSize.y - WindowPosition.y, WindowSize.x, WindowSize.y);
			ChatLogMessageSkin = Resources.Load("ChatLogMessageSkin") as GUISkin;
		}

		private void Update() {
			while (MessageQueue.Count > 0) {
				CreateMessage(MessageQueue.Dequeue());
			}
			
			while (MessageList.Count > HistoryLength) {
				MessageList.RemoveAt(0);
			}
			
			GalvanizationCooldownTimer.TickIf(!IsGalvanized);
			if (!IsGalvanized && GalvanizationCooldownTimer.IsDone() && CommandStatus.IsActive(CommandBinding.Command.GalvanizeChatLog)) {
				Galvanize();
			}

			DegalvanizationCooldownTimer.TickIf(IsGalvanized);
			if (IsGalvanized && DegalvanizationCooldownTimer.IsDone() && CommandStatus.IsInactive(CommandBinding.Command.GalvanizeChatLog)) {
				Degalvanize();
			}

			foreach (var message in MessageList) {
				message.Update();
			}
		}

		private void OnGUI() {
			return;
			var toDraw = false;
			foreach (var message in MessageList) {
				if (!message.IsAlive()) {
					continue;
				}
				toDraw = true;
				break;
			}

			if (!toDraw) {
				return;
			}
			
			GUI.BeginGroup(WindowRect);
			GUI.skin = ChatLogMessageSkin;

			foreach (var message in MessageList) {
				message.DrawLabel();
			}
			GUI.EndGroup();
		}

		private void Galvanize() {
			IsGalvanized = true;
			DegalvanizationCooldownTimer.Start(DegalvanizationCooldown);
				
			var visualOffset = 0.00f;
			for (var i = MessageList.Count - 1; i >= 0; i--) {
				visualOffset += 0.02f;
				MessageList[i].Galvanize(visualOffset);
			}
		}

		private void Degalvanize() {
			IsGalvanized = false;
			GalvanizationCooldownTimer.Start(GalvanizationCooldown);
			
			var visualOffset = 0.00f;
			for (var i = MessageList.Count - 1; i >= 0; i--) {
				visualOffset += 0.02f;
				MessageList[i].Degalvanize(visualOffset);
			}
		}

		public void CreateMessage(ChatLogMessageQueueEntry entry) {
			var messageHeight = ChatLogMessage.GetLabelHeight(entry.Text, WindowRect.width, ChatLogMessageSkin);
			foreach (var message in MessageList) {
				message.Move(messageHeight);
			}
			var messageColor = Color.white;
			switch (entry.ChatWindow) {
				case ChatWindow.Info:
					messageColor = new Color(1.00f, 1.00f, 1.00f);
					break;
				case ChatWindow.Error:
					messageColor = new Color(1.00f, 0.00f, 0.00f);
					break;
			}

			MessageList.Add(new ChatLogMessage(entry.Text, messageHeight, WindowRect, messageColor));
		}
		
		public void PostMessage(string text, ChatWindow chatWindow) {
			MessageQueue.Enqueue(new ChatLogMessageQueueEntry(text, chatWindow));
		}
	}
}