using System.Collections;
using System.Collections.Generic;

using System;

namespace MIL
{
    public static class Functions
    {
        public class Function
        {
            public string name;
            public int argSize;
            public Func<Interpreter.Variable[], Interpreter.Variable> func;

            public Function(string name, Func<Interpreter.Variable[], Interpreter.Variable> func, int argSize)
            {
                this.name = name;
                this.func = func;
                this.argSize = argSize;
            }
        }

        public static Interpreter.Variable Execute(string func, Interpreter.Variable[] args)
        {
            for (int i = 0; i < functions.Length; i++)
            {
                if (functions[i].name == func && functions[i].argSize == args.Length)
                {
                    return functions[i].func(args);
                }
            }

            return new Interpreter.Variable("Not found", true);
        }

        public static Interpreter.Variable Print(Interpreter.Variable[] args)
        {
            //Debug.Log("[PRINT] " + args[0].ToString());
            Console.WriteLine(args[0].ToString());

            return Interpreter.Variable.empty;
        }

        public static Interpreter.Variable AddAndPrint(Interpreter.Variable[] args)
        {

            int i1 = MUtil.Parse<int>(args[0].value);
            int i2 = MUtil.Parse<int>(args[1].value);

            //Debug.Log("[PRINT] " + (i1 + i2).ToString());
            Console.WriteLine((i1 + i2).ToString());

            return Interpreter.Variable.empty;
        }

        public static Interpreter.Variable ChangeColor(Interpreter.Variable[] args)
        {
            switch (args[0].value)
            {
                case "green":
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case "red":
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case "blue":
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case "yellow":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;

                default:
                    return new Interpreter.Variable("Color " + args[0].value + " not found", true);
            }

            return Interpreter.Variable.empty;
        }


        public static Function[] functions = new Function[]
        {
            new Function("print", Print, 1),
            new Function("aap", AddAndPrint, 2),
            new Function("color", ChangeColor, 1),
        };
    }
}


