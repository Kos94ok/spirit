using Misc;
using Settings;
using UI.UserInput;
using UnityEngine;
using Texture = Misc.Texture;

namespace UI {
	public class FpsCounter : MonoBehaviour {

		private readonly Assets Assets = AutowireFactory.GetInstanceOf<Assets>();
		private readonly CommandStatus CommandStatus = AutowireFactory.GetInstanceOf<CommandStatus>();

		private bool IsDisplayed;
		private float DeltaTime;

		private void Update() {
			DeltaTime += (Time.unscaledDeltaTime - DeltaTime) * 0.1f;
			if (CommandStatus.IsIssuedThisFrame(CommandBinding.Command.ToggleFpsCounter)) {
				IsDisplayed = !IsDisplayed;
			}
		}

		private void OnGUI() {
			if (!IsDisplayed) {
				return;
			}
			
			var screenWidth = Screen.width;
			var screenHeight = Screen.height;
			const float labelWidth = 160f;
 
			var style = new GUIStyle();
 
			var rect = new Rect(screenWidth - labelWidth, 2, labelWidth, 16 * 2);
			style.alignment = TextAnchor.UpperLeft;
			style.fontSize = 16;
			style.normal.textColor = new Color (1.0f, 1.0f, 1.0f, 1.0f);
			var fps = 1.0f / DeltaTime;
			var text = $"FPS: {fps:0.}";
			GUI.Label(rect, text, style);

			rect.y += 16;
			var msecs = DeltaTime * 1000.0f;
			text = $"Frame time: {msecs:0.0} ms";
			GUI.Label(rect, text, style);
		}
	}
}