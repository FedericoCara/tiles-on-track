
namespace Mimic.Math {
    public class DivisionOperator : BinaryOperator {

        public DivisionOperator() : base() { }
        public DivisionOperator(Expression expA, Expression expB) : base(expA, expB) { }
        public DivisionOperator(float numberA, float numberB) : base(numberA, numberB) { }

        public override int GetPrecedence() {
            return 3;
        }

        public override int SolveAsInt(bool considerNegatives) {
            int expAValue = expA.SolveAsInt(considerNegatives), expBValue = expB.SolveAsInt(considerNegatives);
            if (expAValue < expBValue && !considerNegatives) {
                throw new UnsolvableException(expAValue + " is smaller than " + expBValue + ". Can't divide");
            } else if (expAValue % expBValue > 0) {
                throw new UnsolvableException("Can't divide " + expAValue + " by " + expBValue);
            } else {
                return expAValue / expBValue;
            }
        }

        public override float SolveAsFloat() {
            return expA.SolveAsFloat() / expB.SolveAsFloat();
        }
    }
}