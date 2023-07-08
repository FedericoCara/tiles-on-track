using System;
using System.Collections.Generic;


namespace Mimic.Math {

    public abstract class BinaryOperator : Operator {

        public Expression expA;
        public Expression expB;

        public BinaryOperator() { }
        
        public BinaryOperator(Expression expA, Expression expB) {
            this.expA = expA;
            this.expB = expB;
        }

        public BinaryOperator(float numberA, float numberB) {
            this.expA = new Literal(numberA);
            this.expB = new Literal(numberB);
        }
    }
}