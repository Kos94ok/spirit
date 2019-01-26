using System.Collections.Generic;
using System.Threading;
using System.Timers;
using UnityEngine;

namespace UI.ChatLog {
	public class ChatLogAgent : MonoBehaviour {
		private readonly Vector2 windowPosition = new Vector2(50f, 50f);
		private readonly Vector2 windowSize = new Vector2(500f, 600f);
		
		private float galvanizationTimer;
		private float galvanizationCooldown;
		private static bool isGalvanizationActive;

		private static Rect windowRect;
		private static GUISkin chatLogMessageSkin;
		private static readonly List<ChatLogMessage> messageList = new List<ChatLogMessage>();
		private static readonly Queue<ChatLogMessageQueueEntry> messageQueue = new Queue<ChatLogMessageQueueEntry>();
		private void Start() {
			windowRect = new Rect(windowPosition.x, Screen.height - windowSize.y - windowPosition.y, windowSize.x, windowSize.y);
			chatLogMessageSkin = Resources.Load("ChatLogMessageSkin") as GUISkin;
		}

		private void Update() {
			while (messageQueue.Count > 0) {
				CreateMessage(messageQueue.Dequeue());
			}
			
			while (messageList.Count > 32) {
				messageList.RemoveAt(0);
			}

			if (galvanizationCooldown > 0.00f) {
				galvanizationCooldown -= Time.deltaTime;
				if (galvanizationCooldown < 0.00f) { galvanizationCooldown = 0.00f; }
			}
			if (isGalvanizationActive) {
				galvanizationTimer -= Time.deltaTime;
				if (galvanizationTimer < 0.00f) { galvanizationTimer = 0.00f; }
			}
			if (!isGalvanizationActive && Input.GetKey(KeyCode.Z) && galvanizationCooldown < 0.01f) {
				isGalvanizationActive = true;
				galvanizationTimer = 0.50f;
				
				var visualOffset = 0.00f;
				for (var i = messageList.Count - 1; i >= 0; i--) {
					visualOffset += 0.02f;
					messageList[i].Galvanize(visualOffset);
				}
			}
			else if (isGalvanizationActive && galvanizationTimer < 0.01f && !Input.GetKey(KeyCode.Z)) {
				isGalvanizationActive = false;
				galvanizationTimer = 0.00f;
				galvanizationCooldown = 0.50f;
//				foreach (var message in messageList) {
//					message.Degalvanize();
//				}
				var visualOffset = 0.00f;
				for (var i = messageList.Count - 1; i >= 0; i--) {
					visualOffset += 0.02f;
					messageList[i].Degalvanize(visualOffset);
				}
			}

			foreach (var message in messageList) {
				message.Update();
			}
		}

		private void OnGUI() {
			var toDraw = false;
			foreach (var message in messageList) {
				if (!message.IsAlive()) {
					continue;
				}
				toDraw = true;
				break;
			}

			if (!toDraw) {
				return;
			}
			
			GUI.BeginGroup(windowRect);
			GUI.skin = chatLogMessageSkin;

			foreach (var message in messageList) {
				message.DrawLabel();
			}
			GUI.EndGroup();
		}

		public void CreateMessage(ChatLogMessageQueueEntry entry) {
			var messageHeight = ChatLogMessage.GetLabelHeight(entry.Text, windowRect.width, chatLogMessageSkin);
			foreach (var message in messageList) {
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

			messageList.Add(new ChatLogMessage(entry.Text, messageHeight, windowRect, messageColor));
		}
		
		public void PostMessage(string text, ChatWindow chatWindow) {
			messageQueue.Enqueue(new ChatLogMessageQueueEntry(text, chatWindow));
		}
	}
}