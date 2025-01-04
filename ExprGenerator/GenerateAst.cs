namespace ExprGenerator
{
    internal class GenerateAst
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: ExprGenerator.exe <output directory>");
                Environment.Exit(64);
            }
            string outputDir = args[0];
            DefineAst(outputDir, "Expr", new List<string>
            {
                "Assign     : Token name, Expr value",
                "Binary     : Expr left, Token op, Expr right",
                "Grouping   : Expr expression",
                "Literal    : object? value",
                "Unary      : Token op, Expr right",
                "Variable   : Token name"
            });

            DefineAst(outputDir, "Stmt", new List<string>
            {
                "Block      : List<Stmt> statements",
                "Expression : Expr expression",
                "Print      : Expr expression",
                "Var        : Token name, Expr initializer"
            });
        }

        private static void DefineAst(string outputDir, string baseName, List<string> types)
        {
            string path = Path.Combine(outputDir, $"{baseName}.cs");
            using StreamWriter writer = new StreamWriter(path);

            writer.WriteLine("namespace cslox");
            writer.WriteLine("{");

            writer.WriteLine($"    public abstract class {baseName}");
            writer.WriteLine("    {");

            // The base accept() method.
            writer.WriteLine("        public abstract T Accept<T>(IVisitor<T> visitor);");
            writer.WriteLine();

            DefineVisitor(writer, baseName, types);

            // The AST classes.
            foreach (string type in types)
            {
                writer.WriteLine();
                string className = type.Split(":")[0].Trim();
                string fields = type.Split(":")[1].Trim();
                DefineType(writer, baseName, className, fields);
            }
            writer.WriteLine("    }");
            writer.WriteLine("}");
            writer.Close();
        }

        private static void DefineVisitor(StreamWriter writer, string baseName, List<string> types)
        {
            writer.WriteLine("        public interface IVisitor<T>");
            writer.WriteLine("        {");
            foreach (string type in types)
            {
                string typeName = type.Split(":")[0].Trim();
                writer.WriteLine($"            T Visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
            }
            writer.WriteLine("        }");
        }

        private static void DefineType(StreamWriter writer, string baseName, string className, string fieldList)
        {
            writer.WriteLine($"        public class {className} : {baseName}");
            writer.WriteLine("        {");

            // Fields.
            string[] fields = fieldList.Split(", ");
            foreach (string field in fields)
            {
                writer.WriteLine($"            public readonly {field};");
            }

            writer.WriteLine();

            // Constructor.
            writer.WriteLine($"            public {className}({fieldList})");
            writer.WriteLine("            {");

            // Store parameters in fields.
            foreach (string field in fields)
            {
                string name = field.Split(" ")[1];
                writer.WriteLine($"                this.{name} = {name};");
            }
            writer.WriteLine("            }");

            // Visitor pattern.
            writer.WriteLine();
            writer.WriteLine("            public override T Accept<T>(IVisitor<T> visitor)");
            writer.WriteLine("            {");
            writer.WriteLine($"                return visitor.Visit{className}{baseName}(this);");
            writer.WriteLine("            }");

            writer.WriteLine("        }");
        }
    }
}