using System;
using UnityEngine;

namespace Mimic.Math.Algebra {

    [Serializable]
    public class Vector3 : CustomEqualsObject {

        [SerializeField]
        protected float x;
        public float X {
            get => x;
            set => x = value;
        }
        
        [SerializeField]
        protected float y;
        public float Y {
            get => y;
            set => y = value;
        }

        [SerializeField]
        protected float z;
        public float Z {
            get => z;
            set => z = value;
        }
        
        public float Magnitude => (float) System.Math.Sqrt(x * x + y * y + z * z);

        public Vector3(Vector3 other) : this(other.x, other.y, other.z) {
        }

        public Vector3(float x, float y, float z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public void Set(UnityEngine.Vector3 point) {
            Set(point.x, point.y, point.z);
        }

        public void Set(float x, float y, float z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override bool Equals (object obj) {
			if (obj is Vector3 otherVector) {
				return otherVector.x == x && otherVector.y == y && otherVector.z == z;
			} else
				return false;
		}

        public override int GetHashCode() {
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
        }
		
        public static implicit operator UnityEngine.Vector3(Vector3 v) => new UnityEngine.Vector3(v.x, v.y, v.z);

        // Adds two vectors.
        public static Vector3 operator+(Vector3 a, Vector3 b) => new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);

        // Subtracts one vector from another.
        public static Vector3 operator-(Vector3 a, Vector3 b) => new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);

        // Negates a vector.
        public static Vector3 operator-(Vector3 a) => new Vector3(-a.x, -a.y, -a.z); 

        // Multiplies a vector by a number.
        public static Vector3 operator*(Vector3 a, float d) => new Vector3(a.x * d, a.y * d, a.z * d); 

        // Multiplies a vector by a number.
        public static Vector3 operator*(float d, Vector3 a) => new Vector3(a.x * d, a.y * d, a.z * d); 

        // Divides a vector by a number.
        public static Vector3 operator/(Vector3 a, float d) => new Vector3(a.x / d, a.y / d, a.z / d); 

    }
    
}