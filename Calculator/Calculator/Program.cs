namespace Calculator
{
    internal class Calculator
    {
        static void Main(string[] args)
        {
            string eq1 = "1 + 1";
            string eq2 = "2 * 2";
            string eq3 = "1 + 2 + 3";
            string eq4 = "6 / 2";
            string eq5 = "11 + 23";
            string eq6 = "11.1 + 23";
            string eq7 = "1 + 1 * 3";
            string eq8 = "( 11.5 + 15.4 ) + 10.1";
            string eq9 = "23 - ( 29.3 - 12.5 )";
            string eq10 = "( 1 / 2 ) - 1 + 1";
            string eq11 = "10 - ( 2 + 3 * ( 7 - 5 ) )";

            Calculator calculator = new Calculator();
            Console.WriteLine("1 + 1 = " + calculator.SolveExpression(eq1));
            Console.WriteLine("2 * 2 = " + calculator.SolveExpression(eq2));
            Console.WriteLine("1 + 2 + 3 = " + calculator.SolveExpression(eq3));
            Console.WriteLine("6 / 2 = " + calculator.SolveExpression(eq4));
            Console.WriteLine("11 + 23 = " + calculator.SolveExpression(eq5));
            Console.WriteLine("11.1 + 23 = " + calculator.SolveExpression(eq6));
            Console.WriteLine("1 + 1 * 3 = " + calculator.SolveExpression(eq7));
            Console.WriteLine("( 11.5 + 15.4 ) + 10.1 = " + calculator.SolveExpression(eq8));
            Console.WriteLine("23 - ( 29.3 - 12.5 ) = " + calculator.SolveExpression(eq9));
            Console.WriteLine("( 1 / 2 ) - 1 + 1 = " + calculator.SolveExpression(eq10));
            Console.WriteLine("10 - ( 2 + 3 * ( 7 - 5 ) ) = " + calculator.SolveExpression(eq11));

        }

        static double Calculate(double leftNum, double rightNum, string oprSign)
        {
            switch (oprSign)
            {
                case "+":
                    return leftNum + rightNum;
                case "-":
                    return leftNum - rightNum;
                case "*":
                    return leftNum * rightNum;
                case "/":
                    return leftNum / rightNum;
                default:
                    return 0;
            }
        }

        static string GetInnerBracketExpression(string statement)
        {
            string innerExpr = "";
            int bracketCount = 0;
            for (var i = 0; i < statement.Length; i++)
            {
                var str = statement.Substring(i, 1);

                if (str.Equals("("))
                {
                    bracketCount++;
                }

                if (str.Equals(")"))
                {
                    if (bracketCount > 1) //for nested brackets
                    {
                        int startIndexOfStr = statement.LastIndexOf("(") + 1;
                        int totalLength = statement.IndexOf(")") - startIndexOfStr;
                        innerExpr = statement.Substring(startIndexOfStr, totalLength).Trim();
                    }
                    else //for separated multiple brackets
                    {
                        innerExpr += str;
                        break;
                    }
                }

                if (bracketCount > 0)
                {
                    innerExpr += str;
                }
            }

            return innerExpr.Replace("(", "").Replace(")", "").Trim();
        }
        double CalculateFromLeftToRight(List<string> exprList, List<string> oprList, string previousExpr, string innerBracketExp)
        {
            string currentStatement = "";

            double leftNum = Convert.ToDouble(exprList[0]);
            double rightNum = Convert.ToDouble(exprList[2]);

            double result = Calculate(leftNum, rightNum, oprList[0].ToString());
            exprList.RemoveAt(2);
            exprList.RemoveAt(1);
            exprList.RemoveAt(0);

            foreach (var item in exprList)
            {
                currentStatement += " " + item;
            }

            if (!string.IsNullOrEmpty(currentStatement))
            {
                result = SolveExpression(result.ToString() + currentStatement, innerBracketExp, previousExpr);
            }
            else
            {
                innerBracketExp = "( " + innerBracketExp + " )";
                var tempExpr = previousExpr.Replace(innerBracketExp, result.ToString());
                if (previousExpr != tempExpr)
                    result = SolveExpression(tempExpr);
            }

            return result;
        }

        double SpecialCalculation(List<string> exprList, string previousExpr, string innerBracketExp)
        {
            double result = 0;
            string currentStatement = "";
            if (exprList != null)
            {
                for (var i = 0; i < exprList.Count; i++)
                {
                    if (exprList[i].ToString() == "*" || exprList[i].ToString() == "/")
                    {
                        double leftNum = Convert.ToDouble(exprList[i - 1]);
                        double rightNum = Convert.ToDouble(exprList[i + 1]);
                        result = Calculate(leftNum, rightNum, exprList[i].ToString());
                        exprList.RemoveAt(i + 1);
                        exprList.RemoveAt(i);
                        exprList[i - 1] = result.ToString();

                        foreach (var item in exprList)
                        {
                            currentStatement += " " + item;
                        }
                    }
                    else
                    {
                        continue;
                    }

                }
            }

            if (exprList != null)
            {
                if (exprList.Count > 1)
                {
                    if (!string.IsNullOrEmpty(currentStatement))
                    {
                        result = SolveExpression(currentStatement.Trim(), innerBracketExp, previousExpr);
                    }
                    else
                    {
                        innerBracketExp = "( " + innerBracketExp + " )";
                        var tempExpr = previousExpr.Replace(innerBracketExp, result.ToString());
                        if (previousExpr != tempExpr)
                            result = SolveExpression(tempExpr);
                    }
                }
                else
                {
                    result = Convert.ToDouble(exprList[0].ToString());
                }
            }

            return result;
        }

        double SolveExpression(string statement, string innerBracketExp = "", string previousStatement = "")
        {
            double result = 0;
            List<string> splittedExpr;
            List<string> operatorsList;

            if (string.IsNullOrEmpty(previousStatement))
            {
                previousStatement = statement.Trim();
            }
            if (statement.Trim().Contains("("))
            {
                innerBracketExp = GetInnerBracketExpression(statement);
                splittedExpr = innerBracketExp.Split(" ").ToList();
                operatorsList = splittedExpr.Where((item, index) => index % 2 != 0).ToList();
            }
            else
            {
                splittedExpr = statement.Trim().Split(" ").ToList();
                operatorsList = splittedExpr.Where((item, index) => index % 2 != 0).ToList();
            }

            if (splittedExpr.Contains("*") || splittedExpr.Contains("/"))
            {
                if (operatorsList[0].ToString() == "*" || operatorsList[0].ToString() == "/")
                {
                    result = CalculateFromLeftToRight(splittedExpr, operatorsList, previousStatement, innerBracketExp);
                }
                else
                {
                    result = SpecialCalculation(splittedExpr, previousStatement, innerBracketExp);
                }

            }
            else
            {
                if (operatorsList.Count > 0)
                {
                    result = CalculateFromLeftToRight(splittedExpr, operatorsList, previousStatement, innerBracketExp);
                }
                else
                {
                    result = Convert.ToDouble(splittedExpr[0].ToString());
                }
            }
            return result;

        }
    }
}
