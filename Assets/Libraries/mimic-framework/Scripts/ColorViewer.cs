#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Mimic.Editor {

	public class ColorViewer : EditorWindow {

		[SerializeField]
		private Color replacement = Color.magenta;

		[SerializeField]
		private Color colorToReplace = Color.white;

		[SerializeField]
		private Texture2D texture;

		private Texture2D writableTexture;

		private string prevTexturePath = null;
		private Color prevColorToReplace = Color.black;
		private Color prevReplacementColor = Color.black;

		private Dictionary<Color, int> colorsCount = new Dictionary<Color, int>();
		private List<ColorTableRow> sortedColorRows;

		private Vector2 colorTableScrollPos = Vector2.zero;
		private Vector2 textureLabelScrollPos = Vector2.zero;

		private struct ColorTableRow{
			public Color color;
			public string count;
			public string hexValue;

			public ColorTableRow (Color color, string count, string hexValue) {
				this.color = color;
				this.count = count;
				this.hexValue = hexValue;
			}
			
		}

		[MenuItem ("Window/Color Viewer")]
		public static void ShowWindow () {
			GetWindow<ColorViewer> ("Color Viewer");
		}

		private void OnGUI () {
			replacement = EditorGUILayout.ColorField("Replacement", replacement);
			colorToReplace = EditorGUILayout.ColorField("Color to replace", colorToReplace);

			texture = (Texture2D) EditorGUILayout.ObjectField ("Texture", texture, typeof(Texture2D), true);

			if (prevTexturePath != AssetDatabase.GetAssetPath (texture)) {
				
				Debug.Log ("SOURCE CHANGED");
				prevTexturePath = AssetDatabase.GetAssetPath (texture);

				colorsCount.Clear ();
				
				if (texture != null) {
					
					CreateWritableTexture ();

					Color color;
					for (int x = 0; x < writableTexture.width; x++) {
						for (int y = 0; y < writableTexture.height; y++) {
							color = writableTexture.GetPixel (x, y);
							if (!colorsCount.ContainsKey (color)) {
								colorsCount.Add (color, 1);
							} else {
								colorsCount [color] = colorsCount [color] + 1;
							}
						}
					}

					List<Color> colors = new List<Color> (colorsCount.Keys);

					List<Color> sortedColors = new List <Color> (colors.Count);
					int index;
					colors.ForEach (c => {
						for (index = 0; index < sortedColors.Count; index++) {
							if (colorsCount [c] > colorsCount [sortedColors [index]]) {
								break;
							}
						}
						sortedColors.Insert (index, c);
					});

					if (sortedColors.Count > 0)
						colorToReplace = sortedColors [0];

					sortedColorRows = new List<ColorTableRow> (sortedColors.Count);
					sortedColors.ForEach (sc => sortedColorRows.Add(new ColorTableRow(sc, colorsCount [sc].ToString (), ColorUtility.ToHtmlStringRGBA (sc))));

					
				} else {
					writableTexture = null;
				}

			}

			if (texture != null) {
				
				if (colorToReplace != prevColorToReplace || replacement != prevReplacementColor) {				
					ReplaceColor ();
					prevColorToReplace = colorToReplace;
					prevReplacementColor = replacement;
				} 

				DrawTableAndTexture ();
			}
		}

		private void CreateWritableTexture(){
			Debug.Log ("Creating Writable Texture");

			texture.filterMode = FilterMode.Point;
			RenderTexture rt = RenderTexture.GetTemporary (texture.width, texture.height);
			rt.filterMode = FilterMode.Point;
			RenderTexture.active = rt;
			Graphics.Blit (texture, rt);
			writableTexture = new Texture2D (texture.width, texture.height);
			writableTexture.ReadPixels (new Rect (0, 0, texture.width, texture.height), 0, 0);
			writableTexture.Apply ();
			RenderTexture.active = null;
		}

		private void ReplaceColor(){
			Debug.Log ("REPLACING COLOR");

			CreateWritableTexture ();

			for (int x = 0; x < writableTexture.width; x++) {
				for (int y = 0; y < writableTexture.height; y++) {
					if (writableTexture.GetPixel (x, y) == colorToReplace) {
						writableTexture.SetPixel (x, y, replacement);
					}
				}
			}
			writableTexture.Apply ();
		}

		private void DrawTableAndTexture(){
			GUILayout.BeginHorizontal ();

			colorTableScrollPos = GUILayout.BeginScrollView (colorTableScrollPos, GUILayout.Width (400), GUILayout.ExpandWidth (false), GUILayout.ExpandHeight (true));

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Count");
			GUILayout.Label ("Color");
			GUILayout.Label ("Value");
			GUILayout.EndHorizontal (); 

			sortedColorRows.ForEach (sortedColorRow => {
				GUILayout.BeginHorizontal ();
				GUILayout.Label (sortedColorRow.count);
				EditorGUILayout.ColorField (sortedColorRow.color);
				GUILayout.TextField (sortedColorRow.hexValue);
				GUILayout.EndHorizontal (); 
			});

			GUILayout.EndScrollView ();
			textureLabelScrollPos = GUILayout.BeginScrollView (textureLabelScrollPos, GUILayout.ExpandWidth (true), GUILayout.ExpandHeight (true));

			GUILayout.Label (writableTexture == null ? texture : writableTexture);
			GUILayout.EndScrollView ();
			GUILayout.EndHorizontal ();
		}
	}

}
#endif