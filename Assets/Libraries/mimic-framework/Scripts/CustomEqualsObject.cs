using System;

using Mimic.Persistance;

namespace Mimic
{
	[Serializable, Persisted]
	public abstract class CustomEqualsObject {
		
		public static bool operator ==(CustomEqualsObject a, CustomEqualsObject b){
			// If both are null, or both are same instance, return true.
			if (System.Object.ReferenceEquals(a, b)){
				return true;
			}

			// If one is null, but not both, return false.
			if (((object)a == null) || ((object)b == null))	{
				return false;
			}

			return a.Equals(b);
		}

		public static bool operator !=(CustomEqualsObject a, CustomEqualsObject b){
			return !(a == b);
		}

		public override bool Equals(object obj) {
			return base.Equals(obj);
		}

		public override int GetHashCode () {
			return base.GetHashCode ();
		}
			
	}
}

