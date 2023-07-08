using System;
using System.Collections.Generic;

namespace Mimic.Math {
    public class UnsolvableException : Exception {

        public UnsolvableException(string message) : base(message) {

        }
    }
}