using System;
using System.Xml;
using System.Reflection;
using UnityEngine;

using Mimic.Persistance;

namespace Mimic.Editor {	

    /// <summary>
    /// Class used to serialize a field outside the unity serialization system
    /// </summary>
    [Serializable]
	public class PropertySerializer {
		
        [SerializeField]
        protected string typeFullName;

        [SerializeField]
        protected string serialization;
        
        private bool isLoaded = false;

        private bool unloadAfterSerialization = true;
        public bool UnloadAfterSerialization {
            set => unloadAfterSerialization = value;
        }

        public bool IsLoaded {
            set => isLoaded = value;
            get => isLoaded;
        }

        public void OnBeforeSerialize(object target) {
            if(isLoaded) {
                if(target == null) {
                    typeFullName = typeof(object).FullName;
                    serialization = "";
                } else {
                    XMLPersister serializer = new XMLPersister();
                    serializer.UseEncryption = false;
                    typeFullName = target.GetType().FullName;
                    XmlDocument xmlDoc = serializer.SerializeToXMLDocument(target, "serialized");
                    serialization = xmlDoc.DocumentElement.InnerXml;
                }
            }
        }

        public void OnAfterDeserialize() {
            if(unloadAfterSerialization) {
                isLoaded = false;
                //UnloadAfterNextSerialization = false;
            }
        }

        public T Load<T>() {
            XMLPersister deserializer = new XMLPersister();
            deserializer.UseEncryption = false;
            Type type = ReflectionUtils.GetTypeFromAllAssemblies(typeFullName);

            if(type == null) {
                isLoaded = true;
                return default(T);
            } else {
                MethodInfo genericMethod = ReflectionUtils.GetGenericMethod(typeof(XMLPersister), nameof(deserializer.LoadFromXMLInnerXML), type);
                string[] parameters = new string[] { serialization, "serialized" };
                T value = (T) genericMethod.Invoke(deserializer, parameters);
                isLoaded = true;
                return value;
            }
        }
	}

}