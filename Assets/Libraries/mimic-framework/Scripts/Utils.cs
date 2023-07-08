using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace Mimic {

    public class Utils {

		public static float RandomRange(Vector2 range){
			return Random.Range(range.x, range.y);
		}

        public static T[] GetAllEnums<T>() {
            Array enumArray = Enum.GetValues(typeof(T));
            return (T[]) enumArray;
        }

        public static T RandomEnumValue<T>() {
            T[] enumArray = GetAllEnums<T>();
            return enumArray[Random.Range(0,enumArray.Length)];
        }

        public static void ReplaceChildrenWithGO(Transform transform, GameObject replacement){
			DestroyChildren(transform);
			if(replacement != null) {
				replacement.transform.SetParent(transform, false);
			}
		}

        public static void ReplaceChildrenWithPrefab(Transform transform, GameObject prefab) {
			ReplaceChildrenWithGO (transform, GameObject.Instantiate (prefab));
        }

        public static void DestroyChildren(Transform transform) {
            for (int i = 0; i < transform.childCount; i++) {
                GameObject.Destroy(transform.GetChild(i).gameObject);
			}
        }

		public static void SetLayerRecursively(GameObject gameObject, int newLayer) {
			if (gameObject == null){
				return;
			}
			
			gameObject.layer = newLayer;
			
			foreach (Transform child in gameObject.transform) {
				if (child != null) {
					SetLayerRecursively(child.gameObject, newLayer);
				}
			}
		}

        public static int CompareTo(int i1, int i2) {
            return i1 == i2 ? 0 : (int)Mathf.Sign(i1 - i2);
        }

        public static float CompareTo(float i1, float i2) {
            return i1 == i2 ? 0f : Mathf.Sign(i1 - i2);
        }
		
		/// <summary>
		/// Checks if a number is between two nombers
		/// </summary>
		/// <param name="number">the number to check
		/// <param name="min">the minimum value
		/// <param name="max">the maximum value
		public static bool IsBetween(float number, float min, float max) {
			return number >= min && number <= max;
		}

		public static double ToJulianDate(DateTime date) {
			return date.ToOADate() + 2415018.5;
		}

		public static DateTime GetNetworkTime() {
			//default Windows time server
			const string ntpServer = "time.windows.com";

			// NTP message size - 16 bytes of the digest (RFC 2030)
			var ntpData = new byte[48];

			//Setting the Leap Indicator, Version Number and Mode values
			ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

			var addresses = Dns.GetHostEntry(ntpServer).AddressList;

			//The UDP port number assigned to NTP is 123
			var ipEndPoint = new IPEndPoint(addresses[0], 123);
			//NTP uses UDP

			using(var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)) {
				socket.Connect(ipEndPoint);

				//Stops code hang if NTP is blocked
				socket.ReceiveTimeout = 3000;     

				socket.Send(ntpData);
				socket.Receive(ntpData);
				socket.Close();
			}

			//Offset to get to the "Transmit Timestamp" field (time at which the reply 
			//departed the server for the client, in 64-bit timestamp format."
			const byte serverReplyTime = 40;

			//Get the seconds part
			ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);

			//Get the seconds fraction
			ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

			//Convert From big-endian to little-endian
			intPart = SwapEndianness(intPart);
			fractPart = SwapEndianness(fractPart);

			var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

			//**UTC** time
			var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

			return networkDateTime.ToLocalTime();
		}

		// stackoverflow.com/a/3294698/162671
		static uint SwapEndianness(ulong x) {
			return (uint) (((x & 0x000000ff) << 24) +
				((x & 0x0000ff00) << 8) +
				((x & 0x00ff0000) >> 8) +
				((x & 0xff000000) >> 24));
		}

		//calculate physical inches with pythagoras theorem
		public static float DeviceDiagonalSizeInInches () {
			if (Screen.dpi <= 0)
				return 15;
			
			float screenWidth = Screen.width / Screen.dpi;
			float screenHeight = Screen.height / Screen.dpi;
			return Mathf.Sqrt (Mathf.Pow (screenWidth, 2) + Mathf.Pow (screenHeight, 2)); 
		}

		public static bool IsRunningOnIOSOrAndroid() {
			return SystemInfo.operatingSystem.Contains ("iOS") || SystemInfo.operatingSystem.Contains ("Android");
		}

        public static void LoadStartGameFromStreamingAssets(string filePath, Action<string> onComplete) {
            string startDataFilePath = Path.Combine(Application.streamingAssetsPath, filePath);
            #if UNITY_ANDROID || UNITY_WEBGL  
                UnityWebRequest webRequest  = UnityWebRequest.Get (startDataFilePath);
                webRequest.SendWebRequest().completed += request => {
					switch(webRequest.responseCode) {
						case 200L: onComplete(webRequest.downloadHandler.text); return;
						case 404L: Debug.LogError($"File {filePath} not found on Streaming Assets: {webRequest.error}"); onComplete(null); return;
						default: Debug.LogError($"Error accessing {filePath} on Streaming Assets: {webRequest.error}"); onComplete(null); return;
					} 
				};				
            #else
                if (File.Exists(startDataFilePath)) {
					onComplete(File.ReadAllText(startDataFilePath));
				} else {
					Debug.LogError($"File {filePath} not found on Streaming Assets"); 
					onComplete(null);;
				}
            #endif
        }

		public static Vector2 CalculateCentroid(params Vector2[] points) {
			if(points.Length == 0) {
				return Vector3.zero;
			}

			Vector2 centroid = Vector3.zero;	
			for (int i = 0; i < points.Length; i++) {
				centroid += points[i];
			}
			return centroid / points.Length;
		}

		/// <summary>
		/// Find the world center point between all points in a path of a PolygonCollider2D
		/// </summary>
		/// <param name="collider">the collider we will use
		/// <param name="pathIndex">the path index on which the center point will be calculated
		public static Vector2 GetCentroid(PolygonCollider2D collider, int pathIndex = 0) {
			return (Vector2)collider.transform.position + Vector2.Scale(CalculateCentroid(collider.GetPath(pathIndex)), (Vector2)collider.transform.localScale);
		}

		/// <summary>
		/// Find the first trigger or non-trigger Collider attached to a gameObject
		/// </summary>
		/// <param name="gameObject">the gameObject to look into
		/// <param name="trigger">true if we are looking for a trigger Collider
		public static Collider GetCollider(GameObject gameObject, bool trigger) {
			Collider[] colliders = gameObject.GetComponents<Collider>();
			for (int i = 0; i < colliders.Length; i++) {
				if(colliders[i].isTrigger == trigger) {
					return colliders[i];
				}
			}
			return null;
		}
		
		/// <summary>
		/// Find the first trigger or non-trigger Collider2D attached to a gameObject
		/// </summary>
		/// <param name="gameObject">the gameObject to look into
		/// <param name="trigger">true if we are looking for a trigger Collider2D
		public static Collider2D GetCollider2D(GameObject gameObject, bool trigger) {
			Collider2D[] colliders = gameObject.GetComponents<Collider2D>();
			for (int i = 0; i < colliders.Length; i++) {
				if(colliders[i].isTrigger == trigger) {
					return colliders[i];
				}
			}
			return null;
		}

		/// <summary>
		/// Creats a list of Transforms from an array based on the proximity to a position filtering
		///	with maximum distance
		/// </summary>
		/// <param name="position">The position from which the distance will be calculated
		/// <param name="array">The array of MonoBehaviours to filter and order
		/// <param name="maxDistanceSqr">The maximum distance squared to filter the array
		/// <param name="orderedList">The resulting ordered list (no alloc needed)
		public static List<T> OrderByProximity<T>(Vector3 position, T[] array, float maxDistanceSqr, List<T> orderedList) where T : MonoBehaviour {
			orderedList.Clear();
			float distanceSqr;
			for (int i = 0; i < array.Length; i++) {
				distanceSqr = (position - array[i].transform.position).sqrMagnitude;
				if (distanceSqr <= maxDistanceSqr) {
					int j = 0;
					while (j < orderedList.Count && distanceSqr >= (position - orderedList[j].transform.position).sqrMagnitude) {
						j++;
					}
					orderedList.Insert(j, array[i]);
				}
			}
			return orderedList;
		}

		/// <summary>
		/// Creats a list of Transforms from an array based on the proximity to a position filtering
		///	with maximum distance
		/// </summary>
		/// <param name="position">The position from which the distance will be calculated
		/// <param name="array">The array of MonoBehaviours to filter and order
		/// <param name="maxDistanceSqr">The maximum distance squared to filter the array
		public static List<T> OrderByProximity<T>(Vector3 position, T[] array, float maxDistanceSqr) where T : MonoBehaviour {
			List<T> orderedList = new List<T>(array.Length);
			return OrderByProximity<T>(position, array, maxDistanceSqr, orderedList);
		}
        
        /// <summary>
        /// Get the closest object to a position from an list of unity objects. You can include a matching condition
        /// </summary>
        /// <param name="position">The given position</param>
        /// <param name="objects">An list of objects to iterate and find the closest object to the position</param>
        /// <param name="optionalCondition">Optional condition that the object found must match if given</param>
        /// <returns>The closest object matching the condition if given</returns>
        public static T GetClosestObject<T>(Vector3 position, List<T> objects, Predicate<T> optionalCondition = null) where T : Component {
            float minSqrDist = float.MaxValue, sqrDist;
            T closestObject = null;
			objects.ForEach(objectFromList => {
                if (optionalCondition == null || optionalCondition(objectFromList)) {
					sqrDist = (position - objectFromList.transform.position).sqrMagnitude;
					if (sqrDist < minSqrDist) {
						minSqrDist = sqrDist;
						closestObject = objectFromList;
					}
				}
            });
            return closestObject;
        }

		public static Texture2D CreateSolidColorTexture(int width, int height, Color color) {
			Texture2D texture = new Texture2D(width, height);
			for (int x = 0; x < width; x++)	{
				for (int y = 0; y < height; y++) {					
					texture.SetPixel(x, y, color);
				}
			}
			return texture;
		}

		//first-order intercept using absolute target position
		public static Vector3 FirstOrderIntercept(Vector3 shooterPosition, Vector3 shooterVelocity, float shotSpeed, Vector3 targetPosition, Vector3 targetVelocity) {
			Vector3 targetRelativePosition = targetPosition - shooterPosition;
			Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;
			float t = FirstOrderInterceptTime(shotSpeed, targetRelativePosition, targetRelativeVelocity);
			return targetPosition + t*(targetRelativeVelocity);
		}

		//first-order intercept using relative target position
		public static float FirstOrderInterceptTime(float shotSpeed, Vector3 targetRelativePosition, Vector3 targetRelativeVelocity) {
			float velocitySquared = targetRelativeVelocity.sqrMagnitude;
			if(velocitySquared < 0.001f)
				return 0f;
		
			float a = velocitySquared - shotSpeed*shotSpeed;
		
			//handle similar velocities
			if (Mathf.Abs(a) < 0.001f) {
				float t = -targetRelativePosition.sqrMagnitude/(2f*Vector3.Dot(targetRelativeVelocity,targetRelativePosition));
				return Mathf.Max(t, 0f); //don't shoot back in time
			}
		
			float b = 2f*Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
			float c = targetRelativePosition.sqrMagnitude;
			float determinant = b*b - 4f*a*c;
		
			if (determinant > 0f) { //determinant > 0; two intercept paths (most common)
				float	t1 = (-b + Mathf.Sqrt(determinant))/(2f*a),
						t2 = (-b - Mathf.Sqrt(determinant))/(2f*a);
				if (t1 > 0f) {
					if (t2 > 0f)
						return Mathf.Min(t1, t2); //both are positive
					else
						return t1; //only t1 is positive
				} else
					return Mathf.Max(t2, 0f); //don't shoot back in time
			} else if (determinant < 0f) { //determinant < 0; no intercept path
				return 0f;
			} else {//determinant = 0; one intercept path, pretty much never happens
				return Mathf.Max(-b/(2f*a), 0f); //don't shoot back in time
			}
		}

		public static void InvokeRealtime(MonoBehaviour monoBehaviour, Action action, float delay){
			monoBehaviour.StartCoroutine(InvokeRealtime(action, delay));
		}

		public static IEnumerator InvokeRealtime(Action action, float delay){
			yield return new WaitForSecondsRealtime(delay);
			action?.Invoke();
		}

    }
}

