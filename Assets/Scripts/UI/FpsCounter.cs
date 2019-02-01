using Misc;
using UnityEngine;
using Texture = Misc.Texture;

namespace UI {
	public class FpsCounter : MonoBehaviour {

		private readonly Assets Assets = AutowireFactory.GetInstanceOf<Assets>(); 
		
		private float DeltaTime;

		private void Update() {
			DeltaTime += (Time.unscaledDeltaTime - DeltaTime) * 0.1f;
		}

		private void OnGUI() {
			var screenWidth = Screen.width;
			var screenHeight = Screen.height;
			var backgroundWidth = screenHeight / 5f;
 
			var style = new GUIStyle();
 
			var rect = new Rect(screenWidth - backgroundWidth, 0, screenHeight / 5f, screenHeight * 2f / 90f);
			style.alignment = TextAnchor.UpperLeft;
			style.fontSize = screenHeight * 2 / 100;
			style.normal.background = Assets.Get(Texture.HealthBarEmpty); 
			style.normal.textColor = new Color (1.0f, 1.0f, 1.0f, 1.0f);
			var msecs = DeltaTime * 1000.0f;
			var fps = 1.0f / DeltaTime;
			var text = string.Format("{0:0.0} ms ({1:0.} fps)", msecs, fps);
			GUI.Label(rect, text, style);
		}
	}
}