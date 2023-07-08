using System;
using System.Collections.Generic;

using UnityEngine;

public static class TransformExtension
{
	//Breadth-first search
	public static Transform FindDeepChild(this Transform aParent, string aName)
	{
		var result = aParent.Find(aName);
		if (result != null)
			return result;
		foreach(Transform child in aParent)
		{
			result = child.FindDeepChild(aName);
			if (result != null)
				return result;
		}
		return null;
	}

	public static List<Transform> GetChildren(this Transform aParent, bool recursive = false){
		List<Transform> list = new List<Transform> ();
		if(recursive) {
			aParent.GetAllChildren (list);
		} else {
			foreach (Transform child in aParent) {
				list.Add (child);
			}
		}
		return list;
	} 

	private static void GetAllChildren(this Transform aParent, List<Transform> list){
		foreach (Transform child in aParent) {
			list.Add (child);
			child.GetAllChildren (list);
		}
	}

	public static bool ExistChild<T>(this Transform aParent, Predicate<T> condition, bool recursive = false) where T : Component {
		return aParent.FindChild<T>(condition, recursive) != null;
	}

	public static T FindChild<T>(this Transform aParent, Predicate<T> condition, bool recursive = false, bool includeInactive = false) where T : Component {
		if(!includeInactive && !aParent.gameObject.activeInHierarchy) {
			return null;
		}

		T component;				
		foreach (Transform child in aParent) {
			component = child.GetComponent<T>();
			if(component != null && (condition == null || condition(component))) {
				return component;
			} 
			if(recursive && (includeInactive || child.gameObject.activeInHierarchy)) {
				component = child.FindChild<T>(condition, recursive, includeInactive);
				if(component != null) {
					return component;
				}
			}
		}
		return null;
	} 

	public static List<T> FindChildren<T>(this Transform aParent, Predicate<T> condition = null) where T : Component {
		List<T> list = new List<T>();
		aParent.FindChildren<T>(list, condition);			
		return list;
	}

	private static void FindChildren<T>(this Transform aParent, List<T> list, Predicate<T> condition) where T : Component {
		T component;		
		foreach (Transform child in aParent) {
			component = child.GetComponent<T>();
			if(component != null && (condition == null || condition(component))) {
				list.Add(component);
			}
			child.FindChildren<T>(list, condition);
		}
	}	

    public static void DestroyAllChildren(this Transform aParent, bool immediate = false) {
        List<Transform> allChildren = aParent.GetChildren();
        if (immediate) {
            allChildren.ForEach(child => { if (child != null) GameObject.DestroyImmediate(child.gameObject); });
        } else {
            allChildren.ForEach(child => { if (child != null) GameObject.Destroy(child.gameObject); });
        }
    }

	public static void SetX(this Transform aParent, float x) {
		Vector3 position = aParent.position;
		position.x = x;
		aParent.position = position;
	}

	public static void SetY(this Transform aParent, float y) {
		Vector3 position = aParent.position;
		position.y = y;
		aParent.position = position;
	}

	public static void SetZ(this Transform aParent, float z) {
		Vector3 position = aParent.position;
		position.z = z;
		aParent.position = position;
	}

	public static void SetLocalX(this Transform aParent, float x) {
		Vector3 position = aParent.localPosition;
		position.x = x;
		aParent.localPosition = position;
	}

	public static void SetLocalY(this Transform aParent, float y) {
		Vector3 position = aParent.localPosition;
		position.y = y;
		aParent.localPosition = position;
	}

	public static void SetLocalZ(this Transform aParent, float z) {
		Vector3 position = aParent.localPosition;
		position.z = z;
		aParent.localPosition = position;
	}

	public static void SetAnchoredX(this RectTransform aParent, float x) {
		Vector3 position = aParent.anchoredPosition3D;
		position.x = x;
		aParent.anchoredPosition3D = position;
	}

	public static void SetAnchoredY(this RectTransform aParent, float y) {
		Vector3 position = aParent.anchoredPosition3D;
		position.y = y; 
		aParent.anchoredPosition3D = position;
	}

}