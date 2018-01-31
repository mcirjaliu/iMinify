using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace iMinify
{
    class Program
    {
        static Minifier minifier = new Minifier(MinifierLanguage.CSharp);
        static Creator creator = new Creator();

        static void Main(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                if (args[0] == "/testmode")
                    TestMode(Convert.ToInt32(args[1]), Convert.ToInt32(args[2]));
                else if (args[0] == "/file")
                    if (args.Length == 3)
                        MinifyFile(args[1], Convert.ToBoolean(args[2]));
                    else
                        MinifyFile(args[1]);
                else if (args[0] == "/folder")
                    MinifyFolder(args[1]);
            }
            else
            {
                Console.WriteLine("Please start the program with the required arguments.");
            }
            Console.Read();
        }

        static void MinifyFile(string file,bool debugging = false)
        {
            Minifier minifier = new Minifier(MinifierLanguage.CSharp);
            minifier.Minify(file,debugging);
        }

        static void MinifyFolder(string folder)
        {
            string[] files = Directory.GetFiles(folder, "*.cs");
            for (int i = 0; i < files.Length; i++)
                MinifyFile(files[i].Replace(".cs",""));
        }

        static void TestMode(int generations, int approximation)
        {
            Console.WriteLine("Test Mode started with {0} generations", generations);

            Stopwatch watch = Stopwatch.StartNew();

            for (int i = 0; i < generations; i++)
            {
                string file = creator.CreateRandomFile(approximation, ".cs");
                minifier.Minify(file);
            }

            watch.Stop();

            Console.WriteLine("Time to generate : {0}ms", watch.ElapsedMilliseconds);
        }
    }
}