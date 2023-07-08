using System;
using System.Text;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;

using Mimic.Persistance;

namespace Mimic.Net.HTTP
{
	[Serializable, Persisted]
	public class URLForm {

		private string url;
		public string URL{
			get{ return url; }
			set{ url = value; }
		}

		private List<KeyValuePair<string, string>> fields;
		public List<KeyValuePair<string, string>> Fields{
			get{ return fields; }
			set{ fields = value; }
		}

		[NonSerialized, NonPersisted]
		private WWWForm wwwForm;
		public WWWForm WWWForm{
			get{ 
				if (wwwForm == null) {
					wwwForm = new WWWForm ();
					fields.ForEach ((KeyValuePair<string, string> pair) => {
						wwwForm.AddField(pair.Key,pair.Value);	
					});
				}
				return wwwForm;
			}

            set {
                wwwForm = value;
            }
		}

		[NonSerialized, NonPersisted]
		private UnityWebRequest unityWebRequest;
		public UnityWebRequest UnityWebRequest{
			get{ return unityWebRequest; }
			set{ unityWebRequest = value; }
		}

		public URLForm(string url, List<KeyValuePair<string, string>> fields = null){
			this.url = url;
			this.fields = fields;
		}

		public override string ToString ()	{
			StringBuilder strBuilder = new StringBuilder (url);

			if (fields.Count > 0) {
				strBuilder.AppendLine ();

				int i;
				for (i = 0; i < fields.Count - 1; i++) {
					AppendField(strBuilder, i);
					strBuilder.Append (",");
					strBuilder.AppendLine ();
				}
				AppendField(strBuilder, i);
			}

			return strBuilder.ToString ();
		}

		private void AppendField(StringBuilder strBuilder, int fieldIndex){
			strBuilder.Append ("\"");
			strBuilder.Append (fields [fieldIndex].Key);
			strBuilder.Append ("\":");
			if (fields [fieldIndex].Value != null) {
				strBuilder.Append ("\"");
				strBuilder.Append (fields [fieldIndex].Value);
				strBuilder.Append ("\"");
			}
		}
	}
}

