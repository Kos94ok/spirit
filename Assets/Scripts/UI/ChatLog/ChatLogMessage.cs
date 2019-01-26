using System;
using System.Collections.Generic;
using Settings;
using UI.UserInput;
using UnityEngine;

namespace UI.ChatLog {
	public class ChatLogMessage {
		private readonly CommandStatus CommandStatus = AutowireFactory.GetInstanceOf<CommandStatus>();
		
		private bool IsGalvanized;
		private bool IsFadingIn = true;
		private bool IsFadingOut;
		private const float FadeInTime = 0.50f;
		private const float FadeOutTime = 0.50f;
		private const float MovementTime = 0.50f;

		private float FadeInCounter;
		private float LifeTimer = 5.00f;
		private float GalvanizeDelay;
		private float DegalvanizeDelay;
		private List<ChatLogMessageMovement> Movements = new List<ChatLogMessageMovement>();
		
		private const float SpawnOffset = 30.00f;

		private readonly string Text;
		private readonly Rect WindowRect;
		private readonly Color Color;

		private readonly float LabelHeight;
		private float Position;
		private float AnchorPosition;

		public ChatLogMessage(string text, float labelHeight, Rect windowRect, Color color) {
			Text = Utility.ConvertColorStrings(text);
			LabelHeight = labelHeight;
			Color = color;
			WindowRect = windowRect;
			Position = -SpawnOffset;
			AnchorPosition = 0.00f;
			if (CommandStatus.IsActive(CommandBinding.Command.GalvanizeChatLog)) {
				Galvanize(0.00f);
			}
		}

		public void Move(float distance) {
			AnchorPosition += distance;
			var movement = new ChatLogMessageMovement(distance, MovementTime);
			Movements.Add(movement);
		}
		
		public void DrawLabel() {
			if (!IsAlive())
				return;

			var fadeValue = 1f;
			if (IsFadingIn) {
				fadeValue = FadeInCounter / FadeInTime;
			} else if (IsFadingOut) {
				fadeValue = LifeTimer / FadeOutTime;
			}

			/* No idea what that is, but it's probably here for a reason */
			var displayText = FixColorCodes(Text, fadeValue);

			var oldColor = GUI.skin.label.normal.textColor;
			GUI.skin.label.normal.textColor = new Color(Color.r, Color.g, Color.b, fadeValue);
			GUI.Label(new Rect(0.00f, WindowRect.height - LabelHeight - Position, WindowRect.width, LabelHeight), displayText);
			GUI.skin.label.normal.textColor = oldColor;
		}

		private string FixColorCodes(string text, float fadeValue) {
			if (!text.Contains("<color")) {
				return text;
			}
			var lastAlphaTagPos = 0;
			var alphaTagPos = text.IndexOf("<color", lastAlphaTagPos, StringComparison.Ordinal);
			while (alphaTagPos != -1) {
				lastAlphaTagPos = alphaTagPos + 1;
				text = text.Remove(alphaTagPos + 14, 2);
				var newColor = ((int)(fadeValue * 255f)).ToString("x");
				if (newColor.Length == 1) { newColor = "0" + newColor; }
				text = text.Insert(alphaTagPos + 14, newColor);
				alphaTagPos = text.IndexOf("<color", lastAlphaTagPos, StringComparison.Ordinal);
			}
			return text;
		}

		public void Galvanize(float visualOffset) {
			IsGalvanized = true;
			LifeTimer = -1.00f;
			if (IsAlive() && !IsFadingOut) {
				return;
			}

			if (!IsFadingIn) {
				GalvanizeDelay = visualOffset;
			}
			
			IsFadingIn = true;
			IsFadingOut = false;
			FadeInCounter = 0.00f;

			Position = -SpawnOffset;
		}
		public void Degalvanize(float visualOffset) {
			if (!IsFadingOut) {
				DegalvanizeDelay = visualOffset;
			}
			
			IsGalvanized = false;
			IsFadingOut = true;
			LifeTimer = FadeOutTime;
		}

		public void Update() {
			if (GalvanizeDelay > 0.00f) {
				GalvanizeDelay -= Time.deltaTime;
				if (GalvanizeDelay <= 0.00f) {
					GalvanizeDelay = 0.00f;
				}
			}
			
			if (DegalvanizeDelay > 0.00f) {
				DegalvanizeDelay -= Time.deltaTime;
				if (DegalvanizeDelay <= 0.00f) {
					DegalvanizeDelay = 0.00f;
				}
			}
			
			if (LifeTimer > 0.00f && DegalvanizeDelay == 0.00f) {
				LifeTimer -= Time.deltaTime;
				if (!IsFadingOut && LifeTimer < FadeOutTime) {
					IsFadingOut = true;
				}
				if (LifeTimer < 0f) { LifeTimer = 0f; }
			}

			if (IsFadingIn && GalvanizeDelay == 0.00f) {
				FadeInCounter += Time.deltaTime;
				if (FadeInCounter > FadeInTime) {
					IsFadingIn = false;
					FadeInCounter = FadeInTime;
				}
			}

			var movementOffset = 0.00f;
			if (Movements.Count > 0) {
				for (var i = 0; i < Movements.Count; i++) {
					var movement = Movements[i];

					var movementDistance = movement.GetDistance();
					var p0 = new Vector2(movementDistance, 0);
					var p1 = new Vector2(movementDistance, 0.8f * movementDistance);
					var p2 = new Vector2(0.8f * movementDistance, movementDistance);
					var p3 = new Vector2(0, movementDistance);
					var time = movement.GetFraction();
					movementOffset += GetBezier(time, p0, p1, p2, p3).y - movementDistance;
					
					movement.Update();
					if (movement.IsDone()) {
						Movements.Remove(movement);
						i -= 1;
					}
				}
			}

			var fadingOffset = 0.00f;
			if (IsFadingIn) {
				var p0 = new Vector2(SpawnOffset, 0);
				var p1 = new Vector2(0, 0.05f * SpawnOffset);
				var p2 = new Vector2(0.2f * SpawnOffset, 0);
				var p3 = new Vector2(0, SpawnOffset);
				var time = (FadeInTime - FadeInCounter) / FadeInTime;
				fadingOffset = -GetBezier(time, p0, p1, p2, p3).y;
			} else if (IsFadingOut) {
				var p0 = new Vector2(SpawnOffset, 0);
				var p1 = new Vector2(SpawnOffset, 0.05f * SpawnOffset);
				var p2 = new Vector2(0.5f * SpawnOffset, SpawnOffset);
				var p3 = new Vector2(0, SpawnOffset);
				var time = (FadeOutTime - LifeTimer) / FadeOutTime;
				fadingOffset = GetBezier(time, p0, p1, p2, p3).y;
			}

			Position = SpawnOffset + AnchorPosition + fadingOffset + movementOffset;
		}
		
		private Vector2 GetBezier(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
		{
			var cx = 3 * (p1.x - p0.x);
			var cy = 3 * (p1.y - p0.y);
			var bx = 3 * (p2.x - p1.x) - cx;
			var by = 3 * (p2.y - p1.y) - cy;
			var ax = p3.x - p0.x - cx - bx;
			var ay = p3.y - p0.y - cy - by;
			var cube = t * t * t;
			var square = t * t;

			var resX = (ax * cube) + (bx * square) + (cx * t) + p0.x;
			var resY = (ay * cube) + (by * square) + (cy * t) + p0.y;

			return new Vector2(resX, resY);
		}

		public bool IsAlive() {
			return LifeTimer > 0.00f || IsGalvanized;
		}

		public static float GetLabelHeight(string text, float labelWidth, GUISkin withSkin) {
			var guiContent = new GUIContent(text);
			return withSkin.label.CalcHeight(guiContent, labelWidth);
		}
	}
}
