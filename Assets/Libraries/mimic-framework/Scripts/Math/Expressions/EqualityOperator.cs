namespace Mimic.Math {

    public class EqualityOperator : BinaryOperator {

        public EqualityOperator() : base() { }
        public EqualityOperator(Expression expA, Expression expB) : base(expA, expB) { }
        public EqualityOperator(float numberA, float numberB) : base(numberA, numberB) { }

        public override int GetPrecedence() {
            return 0;
        }

        public override int SolveAsInt(bool considerNegatives = true) {
            return (expA.SolveAsInt(considerNegatives) == expB.SolveAsInt(considerNegatives)) ? 1 : 0;
        }

        public override float SolveAsFloat() {
            return (expA.SolveAsFloat() == expB.SolveAsFloat()) ? 1 : 0;
        }

    }

}