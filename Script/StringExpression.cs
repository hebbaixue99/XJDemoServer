using System;
using System.Collections.Generic;
using System.Text;

public class StringExpression
{
    public static string InfixToPostfix(string expression)
    {
        Stack<char> stack = new Stack<char>();
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < expression.Length; i++)
        {
            char ch2;
            char c = expression[i];
            if (char.IsWhiteSpace(c))
            {
                continue;
            }
            switch (c)
            {
                case '(':
                {
                    stack.Push(c);
                    continue;
                }
                case ')':
                {
                    while (stack.Count > 0)
                    {
                        char ch4 = stack.Pop();
                        if (ch4 == '(')
                        {
                            break;
                        }
                        builder.Append(ch4);
                    }
                    continue;
                }
                case '*':
                case '/':
                    goto  Label_00F8;

                case '+':
                case '-':
                    goto  Label_0089;

                default:
                    goto  Label_015D;
            }
        Label_0062:
            ch2 = stack.Pop();
            if (ch2 == '(')
            {
                stack.Push(ch2);
                goto  Label_0095;
            }
            builder.Append(ch2);
        Label_0089:
            if (stack.Count > 0)
            {
                goto  Label_0062;
            }
        Label_0095:
            stack.Push(c);
            builder.Append(" ");
            continue;
        Label_00F8:
            while (stack.Count > 0)
            {
                char item = stack.Pop();
                if (item == '(')
                {
                    stack.Push(item);
                    break;
                }
                if ((item == '+') || (item == '-'))
                {
                    stack.Push(item);
                    break;
                }
                builder.Append(item);
            }
            stack.Push(c);
            builder.Append(" ");
            continue;
        Label_015D:
            builder.Append(c);
        }
        while (stack.Count > 0)
        {
            builder.Append(stack.Pop());
        }
        return builder.ToString();
    }

    public static double Parse(string expression)
    {
        string str = InfixToPostfix(expression);
        Stack<double> stack = new Stack<double>();
        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];
            if (!char.IsWhiteSpace(c))
            {
                double num;
                double num2;
                switch (c)
                {
                    case '*':
                    {
                        num2 = stack.Pop();
                        num = stack.Pop();
                        stack.Push(num * num2);
                        continue;
                    }
                    case '+':
                    {
                        num2 = stack.Pop();
                        num = stack.Pop();
                        stack.Push(num + num2);
                        continue;
                    }
                    case '-':
                    {
                        num2 = stack.Pop();
                        num = stack.Pop();
                        stack.Push(num - num2);
                        continue;
                    }
                    case '/':
                    {
                        num2 = stack.Pop();
                        num = stack.Pop();
                        stack.Push(num / num2);
                        continue;
                    }
                }
                int num4 = i;
                StringBuilder builder = new StringBuilder();
                do
                {
                    builder.Append(str[num4]);
                    num4++;
                }
                while (char.IsDigit(str[num4]) || (str[num4] == '.'));
                i = --num4;
                stack.Push(double.Parse(builder.ToString()));
            }
        }
        return stack.Peek();
    }
}

