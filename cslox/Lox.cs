namespace cslox
{
    internal class Lox
    {
        static bool hadError = false;
        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: cslox [script]");
                Environment.Exit(64);
            }
            else if (args.Length == 1)
                RunFile(args[0]);
            else
                RunPrompt();
        }

        private static void RunFile(string path)
        {
            if (hadError) Environment.Exit(65);

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

            foreach (Token token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        private static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
        }
    }
}
