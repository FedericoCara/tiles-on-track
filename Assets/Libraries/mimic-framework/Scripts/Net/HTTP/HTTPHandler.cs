using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;

namespace Mimic.Net.HTTP {

	public class HTTPHandler {

		private int timeout = 0;

		private bool logErrors = true;
		public bool LogErrors{
			set{ logErrors = value; }
		}

		public HTTPHandler(int timeout = 0){
			this.timeout = timeout;
		}

		public static IEnumerator CheckInternetConnection(Action<bool> action){
			WWW www = new WWW("http://www.google.com");
			yield return www;
			if (www.error != null) {
				action (false);
			} else {
				action (true);
			}
		} 

		public IEnumerator TryConnection(string url, Action callbackAction, Action<string> noConnectionHandlerAction){
			using (UnityWebRequest get = UnityWebRequest.Get (url)) {
				get.timeout = timeout;

				//TODO Replace with proper handling
                get.certificateHandler = new AcceptAllCertificates();

                yield return get.SendWebRequest();

				if (get.isNetworkError || get.isHttpError)
				{
					if (noConnectionHandlerAction != null)
						noConnectionHandlerAction(get.downloadHandler.text);
				}
				else if (callbackAction != null){
					callbackAction();
				}
			}
		}

		public IEnumerator SendForms(List<FormDTO> dtos,Action<string> callbackAction, Action<string> errorHandlerAction = null){
			bool exitOnError = false;
			string lastSuccessfulStr = "";
			FormDTO dto;
			int dtosCount = dtos.Count;
			for (int i = 0; i < dtosCount; i++){
				dto = dtos [i];
				yield return CoroutineSingleton.Instance.StartCoroutine(SendForm (dto, successStr => lastSuccessfulStr = successStr, errorStr => {
					if(i>0)
						dtos.RemoveRange(0,i-1);
					exitOnError = true;
					errorHandlerAction(errorStr);
				}));
				if (exitOnError)
					yield break;
			}
			dtos.Clear ();
			if(callbackAction!=null)
				callbackAction (lastSuccessfulStr);
		}

		public IEnumerator SendForm(FormDTO formDTO, Action<string> callbackAction, Action<string> errorHandlerAction = null){		
			switch (formDTO.requestType) {
			case RequestType.GET:
				return GetText (formDTO.URL, callbackAction, errorHandlerAction);
			case RequestType.POST:
				return PostText (formDTO.URLForm, callbackAction, errorHandlerAction);
			case RequestType.PUT:
				return PutText (formDTO.URLForm, callbackAction, errorHandlerAction);
			case RequestType.DELETE:
				return DeleteText (formDTO.URLForm, callbackAction, errorHandlerAction);
			default:
				throw new ArgumentException("WWW request type not recognised");
			}
		}

	    public IEnumerator GetText(string url, Action<string> callbackAction, Action<string> errorHandlerAction = null)
	    {
	        using (UnityWebRequest get = UnityWebRequest.Get(url))
	        {
                get.timeout = timeout;

				//TODO Replace with proper handling
                get.certificateHandler = new AcceptAllCertificates();

                yield return get.SendWebRequest();

				if (get.isNetworkError) {
					HandleNetworkError(get, url, errorHandlerAction);
				} else if (get.isHttpError) {					
					HandleHTTPError("Get", get, url, errorHandlerAction);
				} else {
					HandleSuccess("Get", get, url, callbackAction);
	            }
	        }
	    }

		public IEnumerator PutText(URLForm urlForm, Action<string> callbackAction, Action<string> errorHandlerAction = null)
	    {
			using (UnityWebRequest put = UnityWebRequest.Put(urlForm.URL, urlForm.WWWForm.data))
	        {
				urlForm.UnityWebRequest = put;
				put.timeout = timeout;
	            put.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
				
				//TODO Replace with proper handling
                put.certificateHandler = new AcceptAllCertificates();

                yield return put.SendWebRequest();

				if (put.isNetworkError)	{
					HandleNetworkError(put, urlForm.ToString(), errorHandlerAction);
				} else if (put.isHttpError) {
					HandleHTTPError("Put", put, urlForm.ToString(), errorHandlerAction);
                } else {
					HandleSuccess("Put", put, urlForm.ToString(), callbackAction);
	            }
	        }
	    }

		public IEnumerator PostText(URLForm urlForm, Action<string> callbackAction, Action<string> errorHandlerAction = null)
	    {
			using (UnityWebRequest post = UnityWebRequest.Post(urlForm.URL, urlForm.WWWForm))
	        {
				urlForm.UnityWebRequest = post;
				post.timeout = timeout;
	            post.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

				//TODO Replace with proper handling
                post.certificateHandler = new AcceptAllCertificates();

                yield return post.SendWebRequest();

				if (post.isNetworkError) { 
					HandleNetworkError(post, urlForm.ToString(), errorHandlerAction);
				} else if (post.isHttpError) {
					HandleHTTPError("Post", post, urlForm.ToString(), errorHandlerAction);
				} else {
					HandleSuccess("Post", post, urlForm.ToString(), callbackAction);
	            }
	        }
	    }
        public IEnumerator PostTextAsJson(Uri uri, string data , Action<string> callbackAction, Action<string> errorHandlerAction = null) {
            using (UnityWebRequest post = UnityWebRequest.PostWwwForm(uri,data)) {
                post.timeout = timeout;
                post.SetRequestHeader("Content-Type", "application/json");
                //TODO Replace with proper handling
                post.certificateHandler = new AcceptAllCertificates();

                yield return post.SendWebRequest();

                if (post.isNetworkError) {
                    HandleNetworkError(post, data, errorHandlerAction);
                } else if (post.isHttpError) {
                    HandleHTTPError("Post", post, data, errorHandlerAction);
                } else {
                    HandleSuccess("Post", post, data, callbackAction);
                }
            }
        }

        public IEnumerator DeleteText(URLForm urlForm, Action<string> callbackAction, Action<string> errorHandlerAction = null)
	    {
			using (UnityWebRequest delete = UnityWebRequest.Delete(urlForm.URL))
	        {
				urlForm.UnityWebRequest = delete;
				delete.timeout = timeout;
	            delete.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

				//TODO Replace with proper handling
                delete.certificateHandler = new AcceptAllCertificates();

                yield return delete.SendWebRequest();

				if (delete.isNetworkError) {
					HandleNetworkError(delete, urlForm.ToString(), errorHandlerAction);
	            } else if (delete.isHttpError) {
					HandleHTTPError("Delete", delete, urlForm.ToString(), errorHandlerAction);
                } else {
					HandleSuccess("Delete", delete, urlForm.ToString(), callbackAction);
	            }
	        }
	    }

		private void HandleNetworkError(UnityWebRequest request, string urlFormText, Action<string> errorHandlerAction){
			string text = request.downloadHandler == null || String.IsNullOrEmpty(request.downloadHandler.text) ? request.error : request.downloadHandler.text;
			
			if(logErrors){
				Debug.Log ("Network Error " + text);
				Debug.Log ("Form with error: " + urlFormText);
			}

			if (errorHandlerAction != null)
				errorHandlerAction (text);
		}

		private void HandleHTTPError(string requestType, UnityWebRequest request, string urlFormText, Action<string> errorHandlerAction){
			string text = request.downloadHandler == null || String.IsNullOrEmpty(request.downloadHandler.text) ? request.error : request.downloadHandler.text;
			
			if(logErrors){
				Debug.LogWarning("<color=red> HTTP Error. "+requestType+"Text </color> Response code: "+ request.responseCode + " Text: " + text +" - " + System.DateTime.UtcNow);                    
				Debug.Log ("Form with error: " + urlFormText);
			}

			if (errorHandlerAction != null)
				errorHandlerAction (text);
		}

		private void HandleSuccess(string requestType, UnityWebRequest request, string urlFormText, Action<string> callbackAction){
			string text = request.downloadHandler == null || String.IsNullOrEmpty(request.downloadHandler.text) ? urlFormText : request.downloadHandler.text;
			
			Debug.Log("<color=green> "+requestType+"Text </color> " + text);
			Debug.Log("Form "+requestType.ToUpper()+" request successful: " + urlFormText);
			if (callbackAction != null)
				callbackAction(text);
		}

	}
}
