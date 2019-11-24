using System.Collections;
using System.Collections.Generic;

using System.Threading;

using System.IO;

namespace MIL
{

    public class Interpreter
    {
        public class Variable
        {
            public string name;
            public string type;

            public string value;

            public bool CanBeCasted(Variable v)
            {
                if (type == v.type) return true;
                else return false;
            }

            private void Set(string value, string name, string type)
            {
                this.name = name;
                this.type = type;
                this.value = value;
            }

            public Variable(string value, string name, string type)
            {
                this.name = name;
                this.type = type;
                this.value = value;
            }

            public Variable(Variable v)
            {
                name = v.name;
                value = v.value;
                type = v.type;
            }

            public Variable Copy()
            {

                return new Variable(name, type, value);
            }

            private bool Set(Variable v, bool force)
            {
                if (!CanBeCasted(v) && !force)
                {
                    return false;
                }

                name = v.name;
                value = v.value;

                return true;
            }

            public bool Set(Variable v)
            {
                return Set(v, false);
            }

            /// <summary>
            /// Creates error variable
            /// </summary>
            /// <param name="errorMessage"></param>
            /// <param name="isError"></param>
            public Variable(string errorMessage, bool isError)
            {
                Set(error, true);
                value = errorMessage;
            }


            public Variable(string value, string name = "")
            {
                Set(value, name, "string");
            }

            public Variable(long value, string name = "")
            {
                Set(value.ToString(), name, "int");
            }

            public Variable(double value, string name = "")
            {
                Set(value.ToString(), name, "float");
            }

            

            public bool IsError()
            {
                

                if (name == error.name) return true;
                else return false;
            }

            public override string ToString()
            {
                return value;
            }

            public static Variable empty = new Variable("", "", "");
            public static Variable error = new Variable("", "__ERROR__", "ERROR");
        }

        public class SourceCode
        {
            public class Function
            {
                public int startLine;

                public string name;
                public string src;
                public List<Variable> arguments;


                public Function(string name, string source, int startLine, List<Variable> args = null)
                {
                    this.name = name;
                    src = source;

                    this.startLine = startLine;

                    if (args == null)
                    {
                        arguments = new List<Variable>();
                    }
                    else
                    {
                        arguments = args;
                    }

                }
            }

            public string src;
            public string og;

            public List<Function> functions;



            public void ListFunctions()
            {

                string fs = "";

                for (int i = 0; i < functions.Count; i++)
                {
                    fs += i.ToString() + ") " + functions[i].name + "()" + functions[i].src + "\n\n";
                }
                if (fs.Length == 0)
                {
                    Debug.LogError("No functions found");
                    return;
                }

                Debug.Log(fs);
            }

            public string GetSource()
            {
                return og;
            }

            public int GetLine(int pos)
            {
                int line = 0;
                if (pos >= src.Length)
                {
                    return -1;
                }

                for (int i = 0; i < pos; i++)
                {
                    if (src[i] == '\n')
                    {
                        line++;
                    }
                }

                return line;
            }

            List<Function> FindFunctions(string src)
            {
                List<Function> f = new List<Function>();



                string[] keys = { "fun", "def", "function" };

                for (int i = 0; i < keys.Length; i++)
                {

                    int it = 0;
                    while (src.Contains(keys[i]))
                    {
                        /*if (it > 10000) // Failsafe
                        {
                            Debug.LogError("Failsafe");
                            break;
                        }*/

                        //it++;

                        int level = 0;
                        int brLevel = 0;

                        int pos = src.IndexOf(keys[i]);

                        string name = "";
                        string args = "";
                        string content = "";

                        int state = 0;

                        int startPoint = -1;
                        for (int j = 0; j < src.Length; j++)
                        {
                            if (state == 0) // Name
                            {
                                if (src[j] == '(')
                                {
                                    startPoint = GetLine(j);

                                    state = 1;
                                    level = 1;

                                    j++;
                                    while (j == '\n')
                                    {
                                        j++;
                                    }
                                    j--;
                                    continue;
                                }

                                name += src[j];


                            }
                            else if (state == 1) // Args
                            {
                                if (level == 0)
                                {
                                    //Debug.LogError("Error in line " + GetLine(j) + " - expected closing bracket");
                                    InterpreterErrors.DisplayError(1, GetLine(j));
                                    return null;
                                }

                                if (src[j] == '(')
                                {
                                    level++;

                                    args += src[j];
                                    continue;
                                }
                                if (src[j] == ')')
                                {
                                    level--;

                                    if (level == 0)
                                    {
                                        state = 2;
                                        continue;
                                    }
                                }

                                if (src[j] == '{' || src[j] == '}')
                                {
                                    InterpreterErrors.DisplayError(2, GetLine(j));
                                    return null;
                                }

                                args += src[j];


                            }
                            else // Content
                            {
                                if (src[j] == '{')
                                {
                                    brLevel++;

                                    if (brLevel > 1)
                                    {
                                        InterpreterErrors.DisplayError(2, GetLine(j));
                                        return null;
                                    }

                                    if (brLevel == 1) // We don't want { in our src
                                    {
                                        j++;
                                        while (j == '\n')
                                        {
                                            j++;
                                        }
                                        j--;

                                        continue;
                                    }
                                }

                                if (src[j] == '}')
                                {
                                    brLevel--;

                                    if (brLevel == 0)
                                    {
                                        src = src.Remove(0, j);

                                        break;
                                    }


                                }

                                content += src[j];
                            }


                        }

                        f.Add(new Function(name.Substring(keys[i].Length + 1), content, startPoint + 1));
                    }
                }

                src = og;

                return f;
            }

            public Function GetFunction(string name)
            {
                for (int i = 0; i < functions.Count; i++)
                {
                    if (functions[i].name == name)
                    {
                        return functions[i];
                    }
                }

                return null;
            }


            void GenerateConstStringVariables(List<Variable> variables)
            {
                bool quote = false;

                int startPoint = 0;
                for (int i = 0; i < src.Length; i++)
                {
                    if (src[i] == '\"')
                    {
                        if (!quote)
                        {
                            startPoint = i;
                            quote = true;
                        }
                        else
                        {
                            string var = src.Substring(startPoint, i - startPoint + 1);

                            src = src.Remove(startPoint, i - startPoint + 1);
                            src = src.Insert(startPoint, "__arg" + variables.Count.ToString() + "__");

                            variables.Add(new Variable(var.Remove(0,1).Remove(var.Length - 2,1) , "__arg" + variables.Count.ToString() + "__", "string"));

                            quote = false;

                            i = startPoint;
                        }
                    }
                }

                Debug.Log("Source: \n" + src);
            }

            public SourceCode(string source, List<Variable> variables)
            {
                src = source;
                GenerateConstStringVariables(variables);
                og = src;

                functions = FindFunctions(src);
                if (functions == null)
                {
                    Debug.LogError("Error parsing functions");
                    return;
                }
            }
        }


        List<Variable> variables = new List<Variable>();

        // Source code
        public string sourceFile;

        private SourceCode src;

        void ExecuteFunctions(ref string line, int lineNumber)
        {
            int level = 0;

            string name = "";
            int startPoint = -1;

            for (int i = 0; i < line.Length; i++)
            {

                if (line[i] == '(')
                {
                    startPoint = i;
                    if (level == 0)
                    {
                        for (int j = i - 1; j >= 0; j--)
                        {
                            if (line[j] == ' ' || line[j] == '\n' || line[j] == '\t')
                            {
                                break;
                            }
                            else
                            {
                                name = name.Insert(0, line.Substring(j, 1));
                            }
                        }

                        //Debug.Log("Function name: \"" + name + "\"");
                        

                    }
                    level++;
                }

                if(line[i] == ')')
                {
                    level--;
                    if(level == 0)
                    {
                        List<Variable> vars = new List<Variable>();

                        string arg = "";
                        for(int j = startPoint + 1;j<i;j++)
                        {
                            if(line[j] == ',')
                            {
                                vars.Add(GetVariable(arg, lineNumber));
                                arg = "";
                                continue;
                            }

                            arg += line[j];
                        }
                        if(arg.Length != 0)
                        {
                            vars.Add(GetVariable(arg, lineNumber));
                        }

                        

                        Variable result = Functions.Execute(name, vars.ToArray());
                        if (result.IsError())
                        {
                            Debug.LogError("Error executing: " + result.value);
                            return;
                        }

                        name = "";
                    }
                }
            }

            if(level != 0)
            {
                InterpreterErrors.DisplayError(0, lineNumber);
                return;
            }
        }

        bool Execute(SourceCode.Function f, List<Variable> args, int functionStartLine)
        {
            string[] lines = f.src.Split('\n');//MUtil.StringToStringArray(f.src);

            for(int i = 0;i<lines.Length;i++)
            {
                //Debug.Log("Lines: " + lines[i]);
                ExecuteFunctions(ref lines[i], functionStartLine + i);
            }
            

            return true;
        }

        public bool Execute(string startFunction)
        {
            try
            {
                SourceCode.Function f = src.GetFunction(startFunction);
                if (f == null)
                {
                    InterpreterErrors.DisplayError(3);
                    return false;
                }

                Execute(f, new List<Variable>(), f.startLine);

                return true;
            }
            catch(System.Exception e)
            {
                Debug.Exception(e);
                return false;
            }
        }

        Variable GetVariable(string name, int line)
        {
            for(int i = 0;i<variables.Count;i++)
            {
                if(variables[i].name == name)
                {
                    return variables[i];
                }
            }

            throw new VariableNotFoundException(name, line);
        }



        public Thread executionThread;
        // Start is called before the first frame update
        void Start()
        {
            if (sourceFile.Length == 0)
            {
                Debug.LogError("Source code not specified");
                return;
            }
            if (!File.Exists(sourceFile))
            {
                Debug.LogError("Source file doesn't exist");
                return;
            }


            string text = File.ReadAllText(sourceFile);



            executionThread = new Thread(() =>
            {
                src = new SourceCode(text, variables);


                //src.ListFunctions();

                Execute("main");
            });

            executionThread.Start();

        }

        public Interpreter(string filePath)
        {
            sourceFile = filePath;

            Start();
        }
    }

    public static class InterpreterErrors
    {
        public class Error
        {
            public int code;
            public string message;

            public Error(int code, string message)
            {
                this.code = code;
                this.message = message;
            }

            public void DisplayError(int line)
            {
                Debug.LogError(message + " on line " + line.ToString());
            }

            public void DisplayError()
            {
                Debug.LogError(message);
            }
        }



        public static bool DisplayError(int code, int line)
        {
            for (int i = 0; i < errors.Length; i++)
            {
                if (errors[i].code == code)
                {
                    if (line != -1)
                    {
                        errors[i].DisplayError(line);
                    }
                    else
                    {
                        errors[i].DisplayError();
                    }

                    return true;
                }
            }

            if (line != -1)
            {
                Debug.LogError("Unknown Error in line " + line);
            }
            else
            {
                Debug.LogError("Unknown Error that affects the whole file");
            }

            return false;
        }

        public static bool DisplayError(int code)
        {
            return DisplayError(code, -1);
        }


        public static Error[] errors = new Error[]
        {
            new Error(1, "Cannot find closing bracket"),
            new Error(2, "Unexpected { or } character"),
            new Error(3, "Start function not found"),
            new Error(4, "No closing quote"),
        };
    }

}
