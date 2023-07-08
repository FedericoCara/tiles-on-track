namespace Mimic.Math {

    public abstract class Operator : Expression {

        public abstract int GetPrecedence();

    }

}