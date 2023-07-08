using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mimic.UI{
	
	public class Table<T, K> : MonoBehaviour where T : TableRow<K> {

		[SerializeField]
		private T rowPrefab;

		[SerializeField]
		private RectTransform rowsParent;

		protected List<T> rows = new List<T> ();

		public virtual List<K> Data{
			get{ 
				List<K> data = new List<K> ();
				rows.ForEach (r => data.Add (r.Data));
				return data;
			}
			set{ 
				Clear (false);
				AddRows (value);
			}
		}

		private Action onDataChangedListener;
		public Action OnDataChangedListener{
			set{ onDataChangedListener = value; }
		}

		protected virtual void Start(){
			for (int i = 0; i < rowsParent.childCount; i++) {
				if(!rows.Contains(rowsParent.GetChild(i).GetComponent<T>())){
					Destroy(rowsParent.GetChild(i).gameObject);
				}					
			}
		}

		public void AddRow(K data){
			AddRow (data, false);
		}

		private void AddRow(K data, bool fireOnDataChangedListener){
			T row = Instantiate<T> (rowPrefab);
			row.transform.SetParent (rowsParent, false);
			row.Data = data;
			rows.Add (row);
			if (fireOnDataChangedListener) {
				FireOnDataChanged ();
			}
		}

		public void AddRows(List<K> data){
			data.ForEach (r => AddRow(r, false));
			FireOnDataChanged ();
		}

		public void Clear(){
			Clear (true);
		}

		public void Clear(bool fireOnDataChangedListener){
			rows.ForEach (r => Destroy (r.gameObject));
			rows.Clear ();
			if(fireOnDataChangedListener)
				FireOnDataChanged ();
		}

		private void FireOnDataChanged(){
			if (onDataChangedListener != null) {
				onDataChangedListener ();
			}
		}

	}

}
