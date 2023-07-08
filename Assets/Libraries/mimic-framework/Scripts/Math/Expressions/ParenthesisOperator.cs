namespace Mimic.Math {

    public class ParenthesisOperator : UnaryOperator {
        public override int GetPrecedence() {
            return 5;
        }

        public override int SolveAsInt(bool considerNegatives) {
            return expA.SolveAsInt(considerNegatives);
        }

        public override float SolveAsFloat() {
            return expA.SolveAsFloat();
        }
    }
    
}