namespace Mimic.Math {

    public class SubtractionOperator : BinaryOperator {

        public SubtractionOperator() : base() { }
        public SubtractionOperator(Expression expA, Expression expB) : base(expA, expB) { }
        public SubtractionOperator(float numberA, float numberB) : base(numberA, numberB) { }

        public override int GetPrecedence() {
            return 2;
        }
        
        public override int SolveAsInt(bool considerNegatives = true) {
            int expAValue = expA.SolveAsInt(considerNegatives), expBValue = expB.SolveAsInt(considerNegatives);
            if (expAValue < expBValue && !considerNegatives) {
                throw new UnsolvableException(expAValue + " is smaller than " + expBValue + ". Can't substract");
            } else {
                return expAValue - expBValue;
            }
        }

        public override float SolveAsFloat() {
            return expA.SolveAsFloat() - expB.SolveAsFloat();
        }
    }

}