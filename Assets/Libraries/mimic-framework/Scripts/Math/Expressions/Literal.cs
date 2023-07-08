namespace Mimic.Math {
    
    public class Literal : Expression {
        public float litValue;

        public Literal(float litValue) {
            this.litValue = litValue;
        }

        public override int SolveAsInt( bool considerNegatives = true) {
            if (litValue < 0 && !considerNegatives) {
                throw new UnsolvableException(litValue + " is negative");
            } else {
                return (int)litValue;
            }
        }

        public override float SolveAsFloat() {
            return litValue;
        }
    }
}