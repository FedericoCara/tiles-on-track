using UnityEngine;

namespace Mimic.Math {

    public class ExponentiationOperator : BinaryOperator {

        public ExponentiationOperator() : base() { }
        public ExponentiationOperator(Expression expA, Expression expB) : base(expA, expB) { }
        public ExponentiationOperator(float numberA, float numberB) : base(numberA, numberB) { }

        public override int GetPrecedence() {
            return 4;
        }

        public override float SolveAsFloat() {
            return Mathf.Pow(expA.SolveAsFloat(), expB.SolveAsFloat());
        }

        public override int SolveAsInt(bool considerNegatives = true) {
            int expAValue = expA.SolveAsInt(considerNegatives);
            int expBValue = expB.SolveAsInt(considerNegatives);
            if ((expAValue < 0 || expBValue < 0) && !considerNegatives) {
                throw new UnsolvableException("Negative values not allowed");
            } else {
                return (int) Mathf.Pow(expAValue, expBValue);
            }
        }
    }
    
}