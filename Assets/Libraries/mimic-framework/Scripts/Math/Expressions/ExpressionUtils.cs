using System;
using System.Collections.Generic;
using System.Text;

namespace Mimic.Math {
    public static class ExpressionUtils {

        public static List<string> ParseStringIntoNumbersAndOperators(string expressionStr) {
            if(string.IsNullOrEmpty(expressionStr)) {
                return new List<string>();
            }
            char[] expressionChars = expressionStr.ToCharArray();
            StringBuilder numberSB = new StringBuilder();
            char expressionChar, nextChar;
            List<string> resultTokens = new List<string>();
            for (int i = 0; i < expressionChars.Length; i++) {
                expressionChar = expressionChars[i];
                if (char.IsDigit(expressionChar)) {
                    numberSB.Append(expressionChar);
                    while (i + 1 < expressionChars.Length) {
                        nextChar = expressionChars[i + 1];
                        if (char.IsDigit(nextChar)||nextChar=='.') {
                            numberSB.Append(nextChar);
                            i++;
                        } else {
                            break;
                        }
                    }
                    resultTokens.Add(numberSB.ToString());
                    numberSB.Clear();
                } else {
                    resultTokens.Add(expressionChar.ToString());
                }
            }
            return resultTokens;
        }

        public static List<string> TranslateToPostfixWithShuntingYard(List<string> tokens, bool considerBracketsAndBraces = false) {
            string token;
            Stack<char> operatorStack = new Stack<char>();
            List<string> outputs = new List<string>();
            int currentOperatorPrecedence;
            char currentOperator, prevOperator;
            int tokensCount = tokens.Count;
            for (int i = 0; i < tokensCount; i++) {
                token = tokens[i];
                if (char.IsDigit(token[0])) {
                    outputs.Add(token);
                } else {
                    if (token.Length > 1) {
                        throw new Exception(token + " is not a valid operator");
                    }
                    currentOperator = token[0];
                    if (IsOperator(currentOperator)) {
                        currentOperatorPrecedence = GetPrecedence(currentOperator);
                        while (operatorStack.Count > 0 &&
                            GetPrecedence(prevOperator = operatorStack.Peek()) >= currentOperatorPrecedence &&
                            !IsStartingParenthesis(prevOperator, considerBracketsAndBraces)) {
                            outputs.Add(operatorStack.Pop().ToString());
                        }
                        operatorStack.Push(currentOperator);
                    } else if (IsStartingParenthesis(currentOperator, considerBracketsAndBraces)) {
                        operatorStack.Push(currentOperator);
                    } else if (IsEndingParenthesis(currentOperator, considerBracketsAndBraces)) {
                        currentOperatorPrecedence = GetPrecedence(currentOperator);
                        while (operatorStack.Count > 0 && !IsStartingParenthesis(prevOperator = operatorStack.Peek(), considerBracketsAndBraces)) {
                            outputs.Add(operatorStack.Pop().ToString());
                        }
                        if (operatorStack.Count > 0 && IsStartingParenthesis(prevOperator = operatorStack.Peek(), considerBracketsAndBraces)) {
                            operatorStack.Pop();
                        } else {
                            throw new Exception("No left parenthesis found");
                        }
                    }
                }
            }
            while (operatorStack.Count > 0) {
                outputs.Add(operatorStack.Pop().ToString());
            }
            return outputs;
        }


        private static float Evaluate(List<string> PostfixStrings, bool asInt, bool considerNegatives) {
            try {
                string[] tokensArray = PostfixStrings.ToArray();
                Stack<float> argumentsStack = new Stack<float>();
                float number;
                string token;

                //Read the tokens one by one
                for (int i = 0; i < tokensArray.Length; i++) {
                    token = tokensArray[i];
                    number = 0;

                    //If the token is a value - Push it onto the stack.
                    if (float.TryParse(token, out number)) {
                        argumentsStack.Push(number);
                    }
                    //if the token is an operator
                    else if (IsOperator(token[0])) {
                        int ArgumentsRequired = 2;

                        //If there are fewer values on the stack than the number of arguments(n) required by the operator
                        if (argumentsStack.Count < ArgumentsRequired) {
                            throw new Exception("The user has not provided sufficient values in the expression.");
                        } else {
                            //Pop the top n values from the stack.
                            List<float> argsList = new List<float>();
                            for (int k = 0; k < ArgumentsRequired; k++) {
                                argsList.Add(argumentsStack.Pop());
                            }

                            //We need to reverse the order of the elements in the list to resemble the order of stack.
                            argsList.Reverse();

                            //Evaluate the operator, with the values as arguments.
                            BinaryOperator binaryOperator = BinaryOperatorFactory(token[0], argsList);
                            float val = asInt? binaryOperator.SolveAsInt(considerNegatives) : binaryOperator.SolveAsFloat();

                            //Push the returned results, if any, back onto the stack.
                            argumentsStack.Push(val);
                        }
                    } else {
                        throw new Exception("Unknown operator found in the input: " + token);
                    }
                }

                //If there is only one value in the stack
                if (argumentsStack.Count == 1) {
                    //That value is the result of the calculation.
                    return argumentsStack.Pop();
                } else {
                    throw new Exception("The user has provided too many values.");
                }
            } catch (Exception ex) {
                throw ex;
            }
        }

        public static float EvaluateToFloat(string expressionStr) {
            return EvaluateToFloat(TranslateToPostfixWithShuntingYard(ParseStringIntoNumbersAndOperators(expressionStr)));
        }

        public static float EvaluateToFloat(List<string> PostfixStrings) {
            return Evaluate(PostfixStrings,false,true);
        }

        public static int EvaluateToInt(List<string> PostfixStrings, bool considerNegative) {
            return (int)Evaluate(PostfixStrings, true, considerNegative);
        }

        //  method to return all possible output  
        // from input expression 
        public static List<int> GetPossibleResult(string input) {
            Dictionary<string, List<int>> memo = new Dictionary<string, List<int>>();
            return GetPossibleResultUtil(input, memo);
        }

        //  Utility recursive method to get all possible 
        // result of input string 
        private static List<int> GetPossibleResultUtil(string input,
                    Dictionary<string, List<int>> memo) {
            //  If already calculated, then return from memo 
            if (memo.ContainsKey(input))
                return memo[input];

            List<int> res = new List<int>();
            for (int i = 0; i < input.Length; i++) {
                if (IsOperator(input[i])) {
                    // If character is operator then split and 
                    // calculate recursively 
                    List<int> resPre =
                      GetPossibleResultUtil(input.Substring(0, i), memo);
                    List<int> resSuf =
                      GetPossibleResultUtil(input.Substring(i + 1), memo);

                    //  Combine all possible combination 
                    for (int j = 0; j < resPre.Count; j++) {
                        for (int k = 0; k < resSuf.Count; k++) {
                            if (input[i] == '+')
                                res.Add(resPre[j] + resSuf[k]);
                            else if (input[i] == '-')
                                res.Add(resPre[j] - resSuf[k]);
                            else if (input[i] == '*')
                                res.Add(resPre[j] * resSuf[k]);
                            else if (input[i] == '/') {
                                try {
                                    res.Add(resPre[j] / resSuf[k]);
                                } catch (Exception e) {
                                    //Division no solvable
                                }
                            } else if (input[i] == '^')
                                res.Add((int) UnityEngine.Mathf.Pow(resPre[j], resSuf[k]));
                        }
                    }
                }
            }

            // if input contains only number then save that  
            // into res vector 
            if (res.Count == 0)
                res.Add(int.Parse(input));

            // Store in memo so that input string is not  
            // processed repeatedly 
            memo[input] = res;
            return res;
        }

        public static int GetMaxDepth(List<string> tokens) {
            int childCount = tokens.Count, currentDepth = 0, maxDepth = 0;
            char charToken;
            for (int i = 0; i < childCount; i++) {
                charToken = tokens[i][0];
                if (ExpressionUtils.IsStartingParenthesis(charToken, true)) {
                    currentDepth++;
                    if (currentDepth > maxDepth)
                        maxDepth = currentDepth;
                } else if (ExpressionUtils.IsEndingParenthesis(charToken, true))
                    currentDepth--;
            }
            return maxDepth;
        }

        public static BinaryOperator BinaryOperatorFactory(char operatorChar, List<float> argsList) {
            switch (operatorChar) {
                case '+':
                    return new AdditionOperator(argsList[0], argsList[1]);
                case '-':
                    return new SubtractionOperator(argsList[0], argsList[1]);
                case '*':
                    return new MultiplicationOperator(argsList[0], argsList[1]);
                case '/':
                    return new DivisionOperator(argsList[0], argsList[1]);
                case '^':
                    return new ExponentiationOperator(argsList[0], argsList[1]);
            }
            throw new Exception("Unknown operator found in the input: " + operatorChar);
        }

        public static bool IsOperator(char possibleOperator) {
            return possibleOperator == '+' || possibleOperator == '-' || possibleOperator == '/' || possibleOperator == '*' || possibleOperator == '^';
        }

        public static bool IsParenthesis(char possibleParenthesis, bool considerBracesAndBrackets = false) {
            return IsStartingParenthesis(possibleParenthesis, considerBracesAndBrackets) || 
                IsEndingParenthesis(possibleParenthesis, considerBracesAndBrackets);
        }

        public static bool IsStartingParenthesis(char possibleParenthesis, bool considerBracesAndBrackets = false) {
            return possibleParenthesis == '(' || considerBracesAndBrackets
                && (possibleParenthesis == '[' || possibleParenthesis == '{');
        }

        public static bool IsEndingParenthesis(char possibleParenthesis, bool considerBracesAndBrackets = false) {
            return possibleParenthesis == ')' || considerBracesAndBrackets
                && (possibleParenthesis == ']' || possibleParenthesis == '}');
        }

        public static int GetPrecedence(char operatorChar) {
            switch (operatorChar) {
                case '{':
                    return 5;
                case '}':
                    return 5;
                case '[':
                    return 5;
                case ']':
                    return 5;
                case '(':
                    return 5;
                case ')':
                    return 5;
                case '+':
                    return 2;
                case '-':
                    return 2;
                case '*':
                    return 3;
                case '/':
                    return 3;
                case '^':
                    return 4;
            }
            throw new Exception(operatorChar + " is not an operator");
        }
    }
}