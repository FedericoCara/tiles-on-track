using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mimic.Math {


    public class Utils {

        public static bool IsInteger(float value) {
            return System.Math.Abs(value % 1) <= float.Epsilon;
        }

        public static bool IsNumber(float value) {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }

        public static int GetGreatestCommonDenominator(int a, int b) {
            if(a < 0)
                a = -a;
            if(b < 0)
                b = -b;
                
            while (a != 0 && b != 0) {
                if (a > b) {
                    a %= b;
                } else {
                    b %= a;
                }
            }
            return a == 0 ? b : a;
        }    

        public static Vector3 Multiply(Vector3 a, Vector3 b) {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }    

        public static int Pow(int a, int b) {
            if(b < 0) {
                throw new System.ArgumentException("Negative numbers not supported");
            } else if(b == 0) {
                return 1;
            } else {
                int result = a;
                for (int i = 1; i < b; i++) {
                    result *= a;
                }
                return result;
            }
        }    
        
        public static float Pow(float a, int b) {
            if(b < 0) {
                return Pow(1/a, -b);
            } else if(b == 0) {
                return 1;
            } else {
                float result = a;
                for (int i = 1; i < b; i++) {
                    result *= a;
                }
                return result;
            }
        }    

        public static Vector2Int AbsDiff(Vector2Int v1, Vector2Int v2) {
            Vector2Int distance = v2 - v1;
            return new Vector2Int(Mathf.Abs(distance.x), Mathf.Abs(distance.y));
        }

        public static (Vector2Int, Vector2Int) GetUndirectionDimentions(Vector2Int pointA, Vector2Int pointB) {
            Vector2Int position = new Vector2Int(Mathf.Min(pointA.x, pointB.x), Mathf.Min(pointA.y, pointB.y));
            Vector2Int size = AbsDiff(pointA, pointB) + Vector2Int.one;
            return (position, size);
        }

        public static float CalcPrismVolume(float a, float b, float c) {
            return a * b * c;
        }

        public static float CalcPrismArea(float a, float b, float c) {
            return 2 * a * b +
                2 * a * c +
                2 * b * c;
        }

        public static float SolvePolynomialEquation(float leftValue, int grade, float aproxStep) {
            float value = 0;             
            float result = GetPolynomailFunctionValue(value, grade);
            float lastResult = result;
            while(result < leftValue) {
                lastResult = result;
                value += aproxStep;
                result = GetPolynomailFunctionValue(value, grade);
            }
            return result - leftValue < leftValue - lastResult ? value : value - aproxStep;
        }

        public static float GetPolynomailFunctionValue(float x, int grade) {
            if(grade < 0) {
                throw new NotSupportedException("negative grades not supported");
            } else if(grade == 0) {
                return 1;
            } else if(grade == 1) {
                return x;
            } else {
                return Mathf.Pow(x, grade) + GetPolynomailFunctionValue(x, grade - 1);
            }
        }

        public static float Map(float value, Vector2 baseRange, Vector2 targetRange) {
            return targetRange.x + (value - baseRange.x) * (targetRange.y - targetRange.x) / (baseRange.y - baseRange.x);
        }

        public static bool GetLinePlaneIntersection(Vector3 linePoint, Vector3 lineVector, Vector3 planePoint, Vector3 planeNormal, out Vector3 intersection) {
            float a = Vector3.Dot(planePoint - linePoint, planeNormal);
            float b = Vector3.Dot(lineVector, planeNormal);

            //if b is 0: the line and plane are parallel
            if(b == 0) {
                //if a is also 0: the line is exactly on the plane
                intersection = Vector3.zero;
                return false;
            } else { 
                float d = a / b;
                intersection = d * lineVector + linePoint;
                return true;
            }
        }

        public static float RoundDecimal(float number, int decimalPosition) {
            return (float) System.Math.Round(number, decimalPosition);
            // float multiplier = Pow(10, decimalPosition);
            // return Mathf.RoundToInt(number * multiplier) / multiplier;
        }

        public static List<int> GetDigits(int source, int minimum = 1) {
            int individualFactor = 0;
            int length = source.ToString().Length;
            int tennerFactor = Convert.ToInt32(Mathf.Pow(10, length));
            List<int> digits = new List<int>(Mathf.Max(length, minimum));

            //Fill with 0 to reach minimum
            for (int i = length; i < minimum; i++) {
                digits.Add(0);
            }

            do {
                source -= tennerFactor * individualFactor;
                tennerFactor /= 10;
                individualFactor = source / tennerFactor;

                digits.Add(individualFactor);
            } while (tennerFactor > 1);

            return digits;
        }

    }

}