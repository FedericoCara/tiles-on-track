
namespace Mimic.Math {
    
    public class AdditionOperator : BinaryOperator {

        public AdditionOperator() : base() { }
        public AdditionOperator(Expression expA, Expression expB) : base(expA, expB) { }
        public AdditionOperator(float numberA, float numberB) : base(numberA, numberB) { }

        public override int GetPrecedence() {
            return 2;
        }

        public override int SolveAsInt(bool considerNegatives = true) {
            return expA.SolveAsInt(considerNegatives) + expB.SolveAsInt(considerNegatives);
        }

        public override float SolveAsFloat() {
            return expA.SolveAsFloat() + expB.SolveAsFloat();
        }
    }
}