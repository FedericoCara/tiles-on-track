using System;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Mimic;
using Mimic.Persistance;

namespace Mimic.Persistance {
	
	public class XMLPersister	{

		private const string TYPE_KEY = "type";
		private const string REFERENCE_KEY = "object";

		private bool groupTypes;
		private List<Type> types;

		private bool groupReferences;

		private List<object> references;

		private bool useEncryption = true;
		public bool UseEncryption {
			set => useEncryption = value;
		}

		public XMLPersister (bool groupTypes = false, bool groupReferences = false) {
			this.groupTypes = groupTypes;
			this.groupReferences = groupReferences;
		}

		public T Load<T>(string name = "persistance"){
			return Load<T>(name, name);
		}

		public T Load<T>(string name = "model", string fileName = "persistance"){
			XmlDocument xmlDoc = new XmlDocument ();

			string filepath = System.IO.Path.Combine(Application.persistentDataPath, fileName+".xml");
			if (!File.Exists (filepath)) {
				return default(T);
			}
			xmlDoc.Load (filepath);

			return LoadFromXML<T>(xmlDoc, name);
		}

		public object LoadFromXMLInnerXMLByType(string xmlInnerXML, Type type, string name = "model") {
			MethodInfo genericMethod = ReflectionUtils.GetGenericMethod(typeof(XMLPersister), nameof(XMLPersister.LoadFromXMLInnerXML), type);
			object[] parameters = new object[] { xmlInnerXML, name };
			return genericMethod.Invoke(this, parameters);
		}

		public T LoadFromXMLInnerXML<T>(string xmlInnerXML, string name = "model") {
			XmlDocument xmlDoc = new XmlDocument();
			XmlElement elmRoot = xmlDoc.CreateElement("persistance");
			xmlDoc.AppendChild(elmRoot);
			elmRoot.InnerXml = xmlInnerXML;
			return LoadFromXML<T>(xmlDoc, name);
		}

		public T LoadFromXML<T>(XmlDocument xmlDoc, string name = "model") {

			XmlElement elmRoot = xmlDoc.DocumentElement;

			// ####### Encrypt the XML ####### // If the Xml is encrypted, so this piece of code decrypt it.
			
			#if UNITY_EDITOR
			if(useEncryption) {
			#endif
				if (elmRoot.ChildNodes.Count <= 1) {
					// call the function Decrypt() to get the original xml texts.
					if(elmRoot.InnerText.Length > 0) {
						elmRoot.InnerXml = EncrypterDecrypter.Decrypt (elmRoot.InnerText);
						//Debug.Log(elmRoot.InnerXml);
					}
				}	
			#if UNITY_EDITOR
			}	
			#endif
			//################################	

			XmlNode persistanceNode = xmlDoc.SelectSingleNode("persistance");


			if (persistanceNode != null) {

				XmlNode modelNode = persistanceNode.SelectSingleNode (name);

				object model = LoadEntity (modelNode, typeof(T));

				if (model is T) {
					return (T)model;
				} else {
					try {
						return (T)Convert.ChangeType(model, typeof(T));
					} catch (InvalidCastException) {
						return default(T);
					}
				}
			} else
				return default(T);
		}

		public object LoadEntity(XmlNode node, Type type){
			if(type == null) {
				return null;
			}

			if (type == typeof(string)) {
				XmlAttribute valueAttribute = node.Attributes ["value"];
				return valueAttribute == null ? null : valueAttribute.Value;
			} else if (type.IsEnum) {
				XmlAttribute valueAttribute = node.Attributes ["value"];
				if (valueAttribute != null) {
					return Enum.Parse (type, valueAttribute.Value);
				}
			}

			object entity;

			#if UNITY_EDITOR
			if(type.IsSubclassOf(typeof(UnityEngine.Object))) {
				XmlAttribute guidAttribute = node.Attributes["guid"];
				if(guidAttribute != null) {
					string guid = node.Attributes["guid"].Value;
					string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
					entity = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, type);
				} else {
					entity = null;
				}
			} else {
			#endif
				try {
					entity = Activator.CreateInstance(type);
				} catch (MissingMethodException ex) {
					return null;
				}

				if (entity is IList) {
					IList list = (IList)entity;
					foreach (XmlNode childNode in node.ChildNodes) {
						list.Add(LoadEntity(childNode, Type.GetType(childNode.Attributes["type"].Value)));
					}
				} else if (entity is IDictionary) {
					IDictionary dictionary = (IDictionary)entity;
					foreach (XmlNode childNode in node.ChildNodes) {
						//dictionary.Add(GetChild("key", childNode), GetChild("value", childNode));
						XmlNode keyNode = GetChild("key", childNode);
						XmlNode valueNode = GetChild("value", childNode);
						object key = LoadEntity(keyNode, Type.GetType(keyNode.Attributes["type"].Value));
						object value = LoadEntity(valueNode, Type.GetType(valueNode.Attributes["type"].Value));
						dictionary.Add(key, value);
					}
				} else if (entity is Vector3) {
					Vector3 entityAsVector = (Vector3) entity;
					entityAsVector.x = float.Parse(node.Attributes["x"].Value);
					entityAsVector.y = float.Parse(node.Attributes["y"].Value);
					entityAsVector.z = float.Parse(node.Attributes["z"].Value);
					return entityAsVector;
				} else if (Attribute.IsDefined(type, typeof(Persisted))) {
					List<FieldInfo> persistedFields = GetPersistedFields (type);

					FieldInfo field;
					foreach (XmlNode childNode in node.ChildNodes) {
						field = persistedFields.Find (pf => GetFieldName (pf) == childNode.Name);
						if (field != null) {
							XmlAttribute typeAttribute = childNode.Attributes ["type"];
							string valueType = typeAttribute == null ? childNode.Attributes ["fieldType"].Value : typeAttribute.Value;
							field.SetValue (entity, LoadEntity(childNode, ReflectionUtils.GetTypeFromAllAssemblies(valueType)));					
						}
					}
				} else {
					MethodInfo parseMethod = type.GetMethod("Parse", new Type[] { typeof(string) });
					if (parseMethod != null) {
						object[] parameters = new object[1];
                        parameters[0] = node.Attributes["value"].Value;

						if(entity is System.Single) {
						 	object value = parseMethod.Invoke(null, parameters);

							//Don't ask me why this is here, but is necessary for webgl
							//otherwise the persistence is broken
							//Also not sure which line is the one that makes it work
							#if UNITY_WEBGL
							// parameters[0].ToString();
							// value.ToString();
							Single.Parse((string)parameters[0]);
							//float.Parse((string)parameters[0].ToString()).ToString();
							// Single.Parse(parameters[0].ToString()).ToString();
							// float.Parse(parameters[0].ToString()).ToString();
							#endif

							// Debug.Log($"PARAMETER: {parameters[0]}");
							// Debug.Log($"VALUE OF SINGLE WITH REFLEXION: {value}");
							// Debug.Log($"VALUE OF SINGLE WITHOUT REFLEXION CASTED: {Single.Parse((string)parameters[0])}");
							// Debug.Log($"VALUE OF FLOAT WITHOUT REFLEXION CASTED: {float.Parse((string)parameters[0].ToString())}");
							// Debug.Log($"VALUE OF SINGLE WITHOUT REFLEXION TOSTRING: {Single.Parse(parameters[0].ToString())}");
							// Debug.Log($"VALUE OF FLOAT WITHOUT REFLEXION TOSTRING: {float.Parse(parameters[0].ToString())}");
							return value;
						} else {
							return parseMethod.Invoke(null, parameters);
						}
					} else {
						entity = null;
					}
				}

			#if UNITY_EDITOR
			}
			#endif

			return entity;
		}

		private XmlNode GetChild(string name, XmlNode parent){
			foreach(XmlNode child in parent.ChildNodes){
				if(child.Name == name){
					return child;
				}
			}
			return null;
		}

		public void Persist(object model, string name = "persistance"){
			Persist(model, name, name);
		}

		public void Persist(object model, string name, string fileName = "persistance") {
			// XML file path.
			string filepath = System.IO.Path.Combine(Application.persistentDataPath, fileName+".xml");

			XmlDocument xmlDoc = SerializeToXMLDocument(model, name);

			// save.
			xmlDoc.Save (filepath);

			Debug.Log("Saved "+name+" to " + filepath);
		}

		public XmlDocument SerializeToXMLDocument(object model, string name) {

			// xmlDoc is the new xml document.
			XmlDocument xmlDoc = new XmlDocument ();
			XmlElement elmRoot;

//			if (File.Exists (filepath)) {
//				xmlDoc.Load (filepath);
//				elmRoot = xmlDoc.DocumentElement;
//				elmRoot.InnerXml = "";
//			} else {
				elmRoot = xmlDoc.CreateElement("persistance");
				xmlDoc.AppendChild(elmRoot);
//				xmlDoc.Save(filepath);
//			}

			if (groupTypes) {
				if (types == null)
					types = new List<Type> ();
				else
					types.Clear ();
			}

			if (groupReferences) {
				if (references == null)
					references = new List<object> ();
				else
					references.Clear ();
			}

			AppendEntity (xmlDoc, elmRoot, name, model, model.GetType());

			if (groupTypes) {
				AppendTypes (xmlDoc, elmRoot);
			}

//			XmlNode gameNode;
//			XmlAttribute xmlAttribute;
//
//			AppendEntity(xmlDoc, elmRoot, "seasonEvent", game.SeasonEvent);
//			AppendEntity(xmlDoc, elmRoot, "ruleset", game.Ruleset);
//
//			AppendAttribute (xmlDoc, elmRoot, "date", game.Date);

			if (elmRoot.ChildNodes.Count >= 1) {
				// ####### Encrypt the XML #######
				#if UNITY_EDITOR
				if(useEncryption) {
				#endif
					// call the function Encrypt() to encrypt the original xml texts.
					string data = EncrypterDecrypter.Encrypt (elmRoot.InnerXml);
					// clear the xml file.
					elmRoot.RemoveAll ();
					//apply the new encrypted texts.
					elmRoot.InnerText = data;
				#if UNITY_EDITOR
				}	
				#endif
				// #######
				
			}

			return xmlDoc;
		}

		private void AppendTypes(XmlDocument xmlDoc, XmlNode node){
			XmlNode typesNode = xmlDoc.CreateElement("types");
			XmlNode typeNode;
			types.ForEach (type => {
				typeNode = xmlDoc.CreateElement ("type");
				AppendAttribute(xmlDoc, typeNode, GetTypeKey(type), type.ToString());
				typesNode.AppendChild (typeNode);
			});				
			node.AppendChild (typesNode);
		}

		private XmlNode AppendEntity(XmlDocument xmlDoc, XmlNode parent, string name, object entity, Type fieldType = null){
			XmlNode entityNode = xmlDoc.CreateElement(name);
			parent.AppendChild (entityNode);

			if (entity == null) {
				if(fieldType != null)
					AppendAttribute(xmlDoc, entityNode, "fieldType",  GetTypeKey(fieldType));
				return entityNode;
			}

			if (groupReferences) {
				int indexOf = references.IndexOf (entity);
				if (indexOf < 0) {
					indexOf = references.Count;
					references.Add (entity);
					AppendAttribute(xmlDoc, entityNode, "reference", REFERENCE_KEY + indexOf.ToString());
				} else{
					AppendAttribute(xmlDoc, entityNode, "reference", REFERENCE_KEY + indexOf.ToString());
					return entityNode;
				}
			}

			if(fieldType != null)
				AppendAttribute(xmlDoc, entityNode, "fieldType",  GetTypeKey(fieldType));

			Type type = entity.GetType ();
			AppendAttribute(xmlDoc, entityNode, "type", GetTypeKey(type));

			List<FieldInfo> persistedFields = GetPersistedFields (type);

//			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.NonPublic);
//			if (fields != null) {
//				for (int i = 0; i < fields.Length; i++) {				
//					if (!Attribute.IsDefined (fields [i], typeof(NonPersisted))) {

			persistedFields.ForEach (field => {
				object value = field.GetValue (entity);
				string fieldName = GetFieldName (field);
				if (value == null) {
					AppendField (xmlDoc, entityNode, fieldName, null, GetTypeKey (field.FieldType));
				} else if (value is IList) {
					XmlNode listNode = xmlDoc.CreateElement (fieldName);
					AppendAttribute (xmlDoc, listNode, "fieldType", GetTypeKey (field.FieldType));
					AppendAttribute (xmlDoc, listNode, "type", GetTypeKey (value.GetType ()));
					IList list = (IList)value;
					for (int j = 0; j < list.Count; j++) {
						if (list [j] == null) {
							AppendEntity (xmlDoc, listNode, "listIndex" + j.ToString (), null);
						} else if (Attribute.IsDefined (list [j].GetType (), typeof(Persisted))) {
							AppendEntity (xmlDoc, listNode, "listIndex" + j.ToString (), list [j], list [j].GetType ());
						} else {
							string typeKey = GetTypeKey (list [j].GetType ());
							AppendField (xmlDoc, listNode, "listIndex" + j.ToString (), list [j].ToString (), typeKey, typeKey);
						}
					}
					entityNode.AppendChild (listNode);
				} else if (value is IDictionary) {
					XmlNode dictionaryNode = xmlDoc.CreateElement (fieldName);
					AppendAttribute (xmlDoc, dictionaryNode, "fieldType", GetTypeKey (field.FieldType));
					AppendAttribute (xmlDoc, dictionaryNode, "type", GetTypeKey (value.GetType ()));
					IDictionary dictionary = (IDictionary) value;
					ICollection keys = dictionary.Keys;
					XmlNode keyValuePairNode;
					object keyRelatedValue;
					foreach(object key in keys) {
						keyValuePairNode = xmlDoc.CreateElement ("keyValuePair");
						if (Attribute.IsDefined (key.GetType (), typeof(Persisted))) {
							AppendEntity (xmlDoc, keyValuePairNode, "key", key, key.GetType ());
						} else {
							string typeKey = GetTypeKey (key.GetType ());
							AppendField (xmlDoc, keyValuePairNode, "key", key.ToString (), typeKey, typeKey);
						}
						keyRelatedValue = dictionary[key];
						if (keyRelatedValue == null) {
							AppendEntity (xmlDoc, keyValuePairNode, "value", null);
						} else if (Attribute.IsDefined (keyRelatedValue.GetType (), typeof(Persisted))) {
							AppendEntity (xmlDoc, keyValuePairNode, "value", keyRelatedValue, keyRelatedValue.GetType ());
						} else {
							string typeKey = GetTypeKey (keyRelatedValue.GetType ());
							AppendField (xmlDoc, keyValuePairNode, "value", keyRelatedValue.ToString (), typeKey, typeKey);
						}
						dictionaryNode.AppendChild (keyValuePairNode);
					}
					entityNode.AppendChild (dictionaryNode);
				} else if (value is Vector3) {
					Vector3 valueAsVector3 = (Vector3) value;
					XmlNode vector3Node = xmlDoc.CreateElement (fieldName);
					AppendAttribute (xmlDoc, vector3Node, "type", GetTypeKey (typeof(Vector3)));
					AppendAttribute (xmlDoc, vector3Node, "x", valueAsVector3.x.ToString());
					AppendAttribute (xmlDoc, vector3Node, "y", valueAsVector3.y.ToString());
					AppendAttribute (xmlDoc, vector3Node, "z", valueAsVector3.z.ToString());
					entityNode.AppendChild (vector3Node);
				} else if (Attribute.IsDefined (field.FieldType, typeof(Persisted))) {
					AppendEntity (xmlDoc, entityNode, fieldName, value, field.FieldType);
				#if UNITY_EDITOR
				} else if(value is UnityEngine.Object unityObject) {
					AppendUnityObject(xmlDoc, entityNode, fieldName, unityObject, GetTypeKey (field.FieldType), GetTypeKey (value.GetType()));
				#endif
				} else {
					AppendField (xmlDoc, entityNode, fieldName, value.ToString (), GetTypeKey (field.FieldType), GetTypeKey (value.GetType()));
				}
			});
//					}			
//				}
//			}

			return entityNode;
		}

		private List<FieldInfo> GetPersistedFields(Type type){	
			List<FieldInfo> persistedFields;
			if (Attribute.IsDefined (type.BaseType, typeof(Persisted))) {
				persistedFields = GetPersistedFields (type.BaseType);
			} else {
				persistedFields = new List<FieldInfo> ();
			}

			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (fields != null) {
				for (int i = 0; i < fields.Length; i++) {				
					if (!Attribute.IsDefined (fields [i], typeof(NonPersisted)) && !persistedFields.Exists(f => f.Name == fields[i].Name)) {
						persistedFields.Add (fields [i]);
					}
				}
			}

			return persistedFields;
		}

		private string GetTypeKey(Type type){
			if(groupTypes){
				int indexOf = types.IndexOf (type);
				if (indexOf < 0) {
					indexOf = types.Count;
					types.Add (type);
				}
				return TYPE_KEY + indexOf.ToString ();
			}else{
				return type.ToString();
			}
		}

		private string GetFieldName(FieldInfo field){
			string[] nameSeparatedByLessCharacter = field.Name.Split ('<');
			return nameSeparatedByLessCharacter.Length == 1 ? nameSeparatedByLessCharacter[0] : nameSeparatedByLessCharacter[1].Split('>')[0];
		}

		private void AppendAttribute(XmlDocument xmlDoc, XmlNode node, string name, string value){
			XmlAttribute xmlAttribute = xmlDoc.CreateAttribute (name);
			xmlAttribute.Value = value;
			node.Attributes.Append (xmlAttribute);
		}

		#if UNITY_EDITOR
		private void AppendUnityObject(XmlDocument xmlDoc, XmlNode parent, string name, UnityEngine.Object value, string fieldType, string type = null){
			XmlNode fieldNode = xmlDoc.CreateElement(name);
			AppendAttribute(xmlDoc, fieldNode, "fieldType", fieldType);
			if(type != null)
				AppendAttribute(xmlDoc, fieldNode, "type", type);
			if(value != null) {
				string guid;
				long localID;
				if(UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(value, out guid, out localID)) {
					AppendAttribute(xmlDoc, fieldNode, "guid", guid);
				}
			}
				
			parent.AppendChild (fieldNode);
		}
		#endif

		private void AppendField(XmlDocument xmlDoc, XmlNode parent, string name, string value, string fieldType, string type = null){
			XmlNode fieldNode = xmlDoc.CreateElement(name);
			AppendAttribute(xmlDoc, fieldNode, "fieldType", fieldType);
			if(type != null)
				AppendAttribute(xmlDoc, fieldNode, "type", type);
			if(value != null)
				AppendAttribute(xmlDoc, fieldNode, "value", value);
				
			parent.AppendChild (fieldNode);
		}

	}
}

