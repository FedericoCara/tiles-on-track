namespace Mimic.Math {

    public class MultiplicationOperator : BinaryOperator {

        public MultiplicationOperator() : base() { }
        public MultiplicationOperator(Expression expA, Expression expB) : base(expA, expB) { }
        public MultiplicationOperator(float numberA, float numberB) : base(numberA, numberB) { }

        public override int GetPrecedence() {
            return 3;
        }

        public override int SolveAsInt(bool considerNegatives) {
            return expA.SolveAsInt(considerNegatives) * expB.SolveAsInt(considerNegatives);
        }

        public override float SolveAsFloat() {
            return expA.SolveAsFloat() * expB.SolveAsFloat();
        }
    }

}