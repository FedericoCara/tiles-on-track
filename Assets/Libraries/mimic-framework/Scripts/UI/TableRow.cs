using System.Collections;
using UnityEngine;

namespace Mimic.UI{

	public abstract class TableRow<T> : MonoBehaviour {

		public abstract T Data {
			get;
			set;
		}

	}

}
