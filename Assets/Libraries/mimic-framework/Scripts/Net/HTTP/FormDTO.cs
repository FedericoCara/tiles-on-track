using System;
using System.Collections.Generic;

using Mimic.Persistance;

namespace Mimic.Net.HTTP
{
	[Serializable, Persisted]
	public class FormDTO
	{
		public RequestType requestType;

		private URLForm urlForm;
		public virtual URLForm URLForm {
			get{ return urlForm; }			
		}

		public virtual string URL{
			get{ return urlForm.URL; }
		}

		public virtual List<KeyValuePair<string, string>> Fields{
			get{ return urlForm.Fields;}
		}

		public FormDTO(RequestType type, string url, List<KeyValuePair<string, string>> fields = null){
			this.requestType = type;
			fields = fields!=null ? fields : new List<KeyValuePair<string, string>>();
			urlForm = new URLForm(url, fields);
		}
	}
}

