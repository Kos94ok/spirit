using System;
using Settings;
using UnityEngine;

namespace UI.ChatLog {
	public class ChatLogMessage {
		private readonly KeyStatus keyStatus = AutowireFactory.GetInstanceOf<KeyStatus>();
		
		private bool isGalvanized;
		private bool isFadingIn = true;
		private bool isFadingOut;
		private bool isMoving;
		private const float fadeInThreshold = 0.50f;
		private const float fadeOutThreshold = 0.50f;
		private const float movementThreshold = 0.50f;

		private float fadeInCounter;
		private float lifeTimer = 5.00f;
		private float delayTimer = 0.00f;
		private float movementTimer;
		private float movementDistance;
		
		private const float fadeSpeed = 10.00f;

		private const float spawnOffset = 30.00f;

		private readonly string text;
		private readonly Rect windowRect;
		private readonly Color color;

		private readonly float labelHeight;
		private float position;
		private float targetPosition;
		private float anchorPosition;

		public ChatLogMessage(string text, float labelHeight, Rect windowRect, Color color) {
			this.text = Utility.ConvertColorStrings(text);
			this.labelHeight = labelHeight;
			this.color = color;
			this.windowRect = windowRect;
			position = -spawnOffset;
			targetPosition = 0.00f;
			anchorPosition = 0.00f;
			if (keyStatus.IsPressed(KeyBinding.Action.GalvanizeChatLog)) {
				Galvanize(0.00f);
			}
		}

		public void Move(float distance) {
			targetPosition += distance;
			anchorPosition += distance;
			isMoving = true;
			movementTimer = 0.00f;
			movementDistance = distance;
		}
		
		public void DrawLabel() {
			if (!IsAlive())
				return;

			var fadeValue = 1f;
			if (isFadingIn) {
				fadeValue = fadeInCounter / fadeInThreshold;
			} else if (isFadingOut) {
				fadeValue = lifeTimer / fadeOutThreshold;
			}

			/* No idea what that is, but it's probably here for a reason */
			var displayText = FixColorCodes(text, fadeValue);

			var oldColor = GUI.skin.label.normal.textColor;
			GUI.skin.label.normal.textColor = new Color(color.r, color.g, color.b, fadeValue);
			GUI.Label(new Rect(0.00f, windowRect.height - labelHeight - position, windowRect.width, labelHeight), displayText);
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
			isGalvanized = true;
			lifeTimer = -1.00f;
			if (IsAlive() && !isFadingIn && !isFadingOut) {
				return;
			}
			isFadingIn = true;
			isFadingOut = false;
			fadeInCounter = 0.00f;
			delayTimer = visualOffset;

			targetPosition = anchorPosition;
			position = targetPosition - spawnOffset;
		}
		public void Degalvanize(float visualOffset) {
			isGalvanized = false;
			isFadingOut = true;
			targetPosition = anchorPosition + spawnOffset;
			lifeTimer = fadeOutThreshold;
			delayTimer = visualOffset;
		}

		public void Update() {			
			if (lifeTimer > 0.00f) {
				lifeTimer -= Time.deltaTime;
				if (!isFadingOut && lifeTimer < fadeOutThreshold) {
					isFadingOut = true;
					targetPosition += spawnOffset;
				}
				if (lifeTimer < 0f) { lifeTimer = 0f; }
			}

			if (isFadingIn) {
				fadeInCounter += Time.deltaTime;
				if (fadeInCounter > fadeInThreshold) {
					isFadingIn = false;
					fadeInCounter = fadeInThreshold;
				}
			}

			if (isMoving) {
				movementTimer += Time.deltaTime;
				if (movementTimer > movementThreshold) {
					isMoving = false;
					movementTimer = movementThreshold;
				}
			}

			var fadingOffset = 0.00f;
			if (isFadingIn) {
				var p0 = new Vector2(spawnOffset, spawnOffset);
				var p1 = new Vector2(0, spawnOffset);
				var p2 = new Vector2(0, 0.6f * spawnOffset);
				var p3 = new Vector2(0, 0);
				var time = (fadeInThreshold - fadeInCounter) / fadeInThreshold;
				fadingOffset = GetBezier(time, p0, p1, p2, p3).y - spawnOffset;
			} else if (isFadingOut) {
				var p0 = new Vector2(spawnOffset, 0);
				var p1 = new Vector2(spawnOffset, 0.6f * spawnOffset);
				var p2 = new Vector2(0.775f * spawnOffset, spawnOffset);
				var p3 = new Vector2(0, spawnOffset);
				var time = (fadeOutThreshold - lifeTimer) / fadeOutThreshold;
				fadingOffset = GetBezier(time, p0, p1, p2, p3).y;
			}

			var movementOffset = 0.00f;
			if (isMoving) {
				var p0 = new Vector2(movementDistance, 0);
				var p1 = new Vector2(movementDistance, 0.8f * movementDistance);
				var p2 = new Vector2(0.8f * movementDistance, movementDistance);
				var p3 = new Vector2(0, movementDistance);
				var time = movementTimer / movementThreshold;
				movementOffset = GetBezier(time, p0, p1, p2, p3).y - movementDistance;
			}

			position = spawnOffset + anchorPosition + fadingOffset + movementOffset;

			// Smooth movement to position
			/*if (position < targetPosition) {
				position += fadeSpeed * (targetPosition - position) * Time.deltaTime;
				if (position > targetPosition) {
					position = targetPosition;
				}
			}*/
		}
		
		private Vector2 GetBezier(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
		{
			float cx = 3 * (p1.x - p0.x);
			float cy = 3 * (p1.y - p0.y);
			float bx = 3 * (p2.x - p1.x) - cx;
			float by = 3 * (p2.y - p1.y) - cy;
			float ax = p3.x - p0.x - cx - bx;
			float ay = p3.y - p0.y - cy - by;
			float cube = t * t * t;
			float square = t * t;

			float resX = (ax * cube) + (bx * square) + (cx * t) + p0.x;
			float resY = (ay * cube) + (by * square) + (cy * t) + p0.y;

			return new Vector2(resX, resY);
		}

		public bool IsAlive() {
			return lifeTimer > 0.00f || isGalvanized;
		}

		public static float GetLabelHeight(string text, float labelWidth, GUISkin withSkin) {
			var guiContent = new GUIContent(text);
			return withSkin.label.CalcHeight(guiContent, labelWidth);
		}
	}
}
