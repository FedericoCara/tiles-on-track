using System;
using System.Collections.Generic;

namespace Mimic.Math {
    public abstract class Expression {

        public abstract int SolveAsInt(bool considerNegatives = true);
        public abstract float SolveAsFloat();
    }
}