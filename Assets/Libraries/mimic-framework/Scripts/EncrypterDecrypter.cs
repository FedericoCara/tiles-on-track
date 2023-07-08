using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System;

namespace Mimic{

	public class EncrypterDecrypter : MonoBehaviour {

		public static string Encrypt (string toEncrypt)
		{
			byte[] keyArray = UTF8Encoding.UTF8.GetBytes ("12723450144672893896123905678501");
			// 256-AES key
			byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes (toEncrypt);
			RijndaelManaged rDel = new RijndaelManaged ();
			rDel.Key = keyArray;
			rDel.Mode = CipherMode.ECB;
			// http://msdn.microsoft.com/en-us/library/system.security.cryptography.ciphermode.aspx
			rDel.Padding = PaddingMode.PKCS7;
			// better lang support
			ICryptoTransform cTransform = rDel.CreateEncryptor ();
			byte[] resultArray = cTransform.TransformFinalBlock (toEncryptArray, 0, toEncryptArray.Length);
			return Convert.ToBase64String (resultArray, 0, resultArray.Length);
		}

		public static string Decrypt (string toDecrypt)
		{
			byte[] keyArray = UTF8Encoding.UTF8.GetBytes ("12723450144672893896123905678501");
			// AES-256 key
			byte[] toEncryptArray = Convert.FromBase64String (toDecrypt);
			RijndaelManaged rDel = new RijndaelManaged ();
			rDel.Key = keyArray;
			rDel.Mode = CipherMode.ECB;
			// http://msdn.microsoft.com/en-us/library/system.security.cryptography.ciphermode.aspx
			rDel.Padding = PaddingMode.PKCS7;
			// better lang support
			ICryptoTransform cTransform = rDel.CreateDecryptor ();
			byte[] resultArray = cTransform.TransformFinalBlock (toEncryptArray, 0, toEncryptArray.Length);
			return UTF8Encoding.UTF8.GetString (resultArray);
		}

	}
}
