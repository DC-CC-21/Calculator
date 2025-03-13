using MathEvaluation.Context;
using MathEvaluation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Runtime.InteropServices;

namespace Calculator
{

    class Math
    {

        /// <summary>
        /// Union Result to hold the math result
        /// It will hold either Value or Success depending on whether the math
        /// equation could be solved
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct Result
        {
            [FieldOffset(0)]
            public float Value;

            [FieldOffset(0)]
            public bool Success;

            public void SetValue(float value)
            {
                Value = value;
                Success = true;
            }
        }

        /// <summary>
        /// This function calls the MathEvaluator Lib to solve the math equation
        /// </summary>
        /// <param name="math"></param>
        /// <param name="Output"></param>
        /// <returns></returns>
        public static double? EvaluateMath(string math, Label Output)
        {
            Result result = new();
            try
            {
                // Replace instances of double minus signs so the equation can be correctly parsed
                math = math.Replace("--", "+");

                // solve the equation and convert the result to a float
                double answer = new MathExpression(math, new ScientificMathContext()).Evaluate();
                float a = Convert.ToSingle(answer);
                result.SetValue(a);
            }
            catch (Exception)
            {
                // If the equation fails, set the output message and success
                result.Success = false;
                Output.Content = "Error evaluating expression";
            }

            // Return the values
            if (!result.Success)
            {
                return null;
            }
            else
            {
                return MathF.Round(result.Value, 3);
            }
        }
    }
}
