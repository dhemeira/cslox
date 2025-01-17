﻿namespace cslox
{
    internal class Lox
    {
        private static readonly Interpreter interpreter = new Interpreter();
        static bool hadError = false;
        static bool hadRuntimeError = false;
        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: cslox [script]");
                System.Environment.Exit(64);
            }
            else if (args.Length == 1)
                if (File.Exists(args[0]))
                    RunFile(args[0]);
                else
                {
                    Console.WriteLine($"Error: File not found at path '{Path.GetFullPath(args[0])}'");
                    System.Environment.Exit(66);
                }
            else
                RunPrompt();
        }

        private static void RunFile(string path)
        {
            if (hadError) System.Environment.Exit(65);
            if (hadRuntimeError) System.Environment.Exit(70);

            string source = File.ReadAllText(path);
            Run(source);
        }

        private static void RunPrompt()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("cslox");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("> ");
                string? line = Console.ReadLine();
                if (line == null) break;
                Run(line);

                hadError = false;
            }
        }

        private static void Run(string source)
        {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();

            Parser parser = new Parser(tokens);
            List<Stmt> statements = parser.Parse();

            if (hadError) return;

            interpreter.Interpret(statements);
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        private static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
            hadError = true;
        }

        public static void Error(Token token, string message)
        {
            if (token.type == TokenType.EOF)
                Report(token.line, " at end", message);
            else
                Report(token.line, $" at '{token.lexeme}'", message);
        }

        public static void RuntimeError(RuntimeError error)
        {
            Console.Error.WriteLine($"{error.Message}\n[line {error.token.line}]");
            hadRuntimeError = true;
        }
    }
}
