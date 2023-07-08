using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Mimic.Editor {
	
	public static class EditorUtils {

		[MenuItem("Tools/Find Missing references in scene")]
		public static void FindMissingReferences() {
			List<GameObject> objects = GetAllObjectsOnlyInScene();

			foreach (var go in objects) {
				Component[] components = go.GetComponents<Component>();

				foreach (Component c in components) {
					SerializedObject so = new SerializedObject(c);
					var sp = so.GetIterator();

					while (sp.NextVisible(true)) {
						if (sp.propertyType == SerializedPropertyType.ObjectReference) {
							if (sp.objectReferenceValue == null && sp.objectReferenceInstanceIDValue != 0) {
								ShowError(FullObjectPath(go), sp.name, go);
							}
						}
					}
				}
			}
		}

		public static List<GameObject> GetAllObjectsOnlyInScene() {
			List<GameObject> objectsInScene = new List<GameObject>();

			foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[]) {
				if (!EditorUtility.IsPersistent(go.transform.root.gameObject) && !(go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave))
					objectsInScene.Add(go);
			}

			return objectsInScene;
		}

		private static void ShowError(string objectName, string propertyName, GameObject gameObject) {
			Debug.LogError("Missing reference found in: " + objectName + ", Property : " + propertyName, gameObject);
		}

		private static string FullObjectPath(GameObject go) {
			return go.transform.parent == null ? go.name : FullObjectPath(go.transform.parent.gameObject) + "/" + go.name;
		}

		[MenuItem("CONTEXT/Component/Find references in the scene")]
		private static void FindReferences(MenuCommand data) {
			ShowReferences(FindReferencesInTheScene(data.context as Component));			
		}

		[MenuItem("Tools/Find references in the scene")]
		private static void FindReferencesToAsset(MenuCommand data) {
			List<GameObject> referencedBy = new List<GameObject>();
			if(Selection.activeObject is GameObject target) {
				Component[] components = target.GetComponents<Component>();
				for (int i = 0; i < components.Length; i++) {
					referencedBy.AddRange(FindReferencesInTheScene(components[i]));
				}
			}
			ShowReferences(referencedBy);
		}

		private static List<GameObject> FindReferencesInTheScene(Component target) {
			List<GameObject> referencedBy = new List<GameObject>();
			if(target == null) {
				return referencedBy;
			}

			List<GameObject> gameObjectsInTheScene = GetAllObjectsOnlyInScene();
			gameObjectsInTheScene.ForEach(gameObjectInTheScene => {
				
				if (PrefabUtility.GetPrefabType(gameObjectInTheScene) == PrefabType.PrefabInstance) {
					if (PrefabUtility.GetPrefabParent(gameObjectInTheScene) == target) {
						Debug.Log($"Referenced by {gameObjectInTheScene.name}, {gameObjectInTheScene.GetType()}");
						referencedBy.Add(gameObjectInTheScene);
					}
				}

				Component[] components = gameObjectInTheScene.GetComponents<Component>();
				for (int i = 0; i < components.Length; i++) {
					Component component = components[i];

					//Not sure if this null check is needed
					// if (c == null) {
					// 	continue;
					// } 

					SerializedObject so = new SerializedObject(component);
					SerializedProperty sp = so.GetIterator();

					while (sp.NextVisible(true)) {
						if (sp.propertyType == SerializedPropertyType.ObjectReference) {
							if (sp.objectReferenceValue == target) {
								Debug.Log($"Referenced by {component.name}, {component.GetType()}");
								referencedBy.Add(component.gameObject);
							}
						}
					}
				}
			});

			return referencedBy;
		}

		private static void ShowReferences(List<GameObject> referencedBy) {
			if (referencedBy.Count > 0) {
				Selection.objects = referencedBy.ToArray();
			} else {
				Debug.Log("no references in scene");
			}
		}
		
		[MenuItem("Tools/Order transform's children")]
		private static void OrderGameObjectsChildren() {
			GameObject selected = (GameObject)Selection.activeObject;
			if (EditorUtility.DisplayDialog("Ordering transform's children", $"Are you sure you want to order all's {selected.name} children?", "Yes", "Cancel")) {
				List<Transform> children = new List<Transform>();
				foreach (Transform child in selected.transform) {
					children.Add(child);
				}
				children.Sort(CompareChildrenByName);
				children.ForEach(child => child.SetParent(null));
				children.ForEach(child => child.SetParent(selected.transform));
			}
		}
		private static int CompareChildrenByName(Transform x, Transform y) {
			return x.name.CompareTo(y.name);
		}

	}
}