using System;
using UnityEngine;

namespace Mimic.Math {
    
    [Serializable]
    public class Fraction : CustomEqualsObject, IComparable {

        [SerializeField] protected int numerator = 1;
        public int Numerator {
            get { return numerator; }
        }

        [SerializeField] protected int denominator = 1;
        public int Denominator {
            get { return denominator; }
        }

        protected float value = 0;
        public virtual float Value {
            get {
                value = numerator / (float)denominator;
                return value;
            }
        }

        protected Fraction simplifiedFraction;
        public virtual Fraction SimplifiedFraction {
            get {
                if (simplifiedFraction == null) {
                    int greatestCommonDenominator = Utils.GetGreatestCommonDenominator(numerator, denominator);
                    if (greatestCommonDenominator == 1) {
                        simplifiedFraction = this;
                    } else {
                        simplifiedFraction = new Fraction(numerator / greatestCommonDenominator, denominator / greatestCommonDenominator);
                    }
                }
                return simplifiedFraction;
            }
        }

        public int WholeNumberPart {
            get {
                return (int)Value;
            }
        }

        public Fraction FractionNumberPart {
            get {
                return new Fraction((int)((Value- WholeNumberPart)* denominator), denominator) ;
            }
        }

        public Fraction(int numerator, int denominator) {
            this.numerator = numerator;
            this.denominator = denominator;
            value = Value;
        }

        public override bool Equals(object obj) {
            if (obj is Fraction) {
                Fraction other = ((Fraction)obj).SimplifiedFraction;
                return other.numerator == SimplifiedFraction.numerator && other.denominator == SimplifiedFraction.denominator;
            } else {
                return false;
            }
        }

        public override int GetHashCode() {
            return 143 * SimplifiedFraction.numerator + SimplifiedFraction.denominator;
        }

        public override string ToString() {
            if (value == (int)value) {
                return value.ToString();
            } else {
                return numerator + "/" + denominator;
            }
        }

        public string ToMixedFractionString() {
            value = Value;
            if (value == (int)value) {
                return WholeNumberPart.ToString();
            } else {
                return WholeNumberPart+" "+FractionNumberPart;
            }
        }

        public int CompareTo(object obj) {
            if (obj is Fraction) {
                return this.value.CompareTo(((Fraction)obj).value);
            } else
                return -1;
        }

        public static bool operator >(Fraction a, Fraction b) {
            return a == b ? false : a.value > b.value;
        }

        public static bool operator <(Fraction a, Fraction b) {
            return a == b ? false : a.value < b.value;
        }

    }
}