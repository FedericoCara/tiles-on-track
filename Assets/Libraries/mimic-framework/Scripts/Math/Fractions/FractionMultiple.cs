using System;
using UnityEngine;

namespace Mimic.Math {
    [Serializable]
    public class FractionMultiple : Fraction {
        [SerializeField] private Fraction multiplier;
        public Fraction Multiplier {
            get { return multiplier; }
            set { multiplier = value; }
        }

        public override float Value{
            get{
                value = (multiplier != null ? multiplier.Value : 1) * numerator / (float)denominator;
                return value;
            }            
        }

        public FractionMultiple(float value):base(1,1) {
            

            float decimalPart = value - (int)value;

            numerator = 0;
            denominator = 0;
            bool isFounded = false;

            while (!isFounded && denominator<100) {
                denominator++;
                for (int i = 0; i < denominator; i++) {
                    if (Mathf.Approximately(i / (float)denominator,  decimalPart)) {
                        numerator = i;
                        isFounded = true;
                        continue;
                    }
                }
            }
            numerator += (int)value * denominator;

            if (numerator > 0) {
                Multiplier = new Fraction(numerator, 1);
                numerator = 1;
            } else {
                Multiplier = new Fraction(1, 1);
            }
            value = value;

        }

        public FractionMultiple(int numerator, int denominator, Fraction multiplier) : base(numerator, denominator) {
            this.multiplier = multiplier;
            value = Value;
        }
        public override bool Equals(object obj) {
            if (obj is FractionMultiple) {
                FractionMultiple other = ((FractionMultiple)obj);
                return other.Numerator == this.Numerator && other.Denominator == this.Denominator && this.Multiplier== other.Multiplier;
            } else {
                return base.Equals(obj);
            }
        }

        public override Fraction SimplifiedFraction {
            get {
                if (simplifiedFraction == null) {
                    int greatestCommonDenominator = Math.Utils.GetGreatestCommonDenominator((int)multiplier.Value * numerator, denominator);
                    if (greatestCommonDenominator == 1) {
                        simplifiedFraction = this;
                    } else {
                        simplifiedFraction = new Fraction((int)multiplier.Value * numerator / greatestCommonDenominator, denominator / greatestCommonDenominator);
                    }
                }
                return simplifiedFraction;
            }
        }

        public override int GetHashCode() {
            return 143 * SimplifiedFraction.Numerator + SimplifiedFraction.Denominator + (int)(Multiplier.Value*997);
        }

        public override string ToString() {
            if (value == (int)value) {
                return numerator.ToString();
            } else if (multiplier.Value ==1){
                return numerator + "/" + denominator;
            }
            else{
                return multiplier+"*"+ numerator + "/" + denominator;
            }
        }
    }
}
