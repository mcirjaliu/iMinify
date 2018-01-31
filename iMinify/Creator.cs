using System;
using System.Text;
using System.IO;

namespace iMinify
{
    class Creator
    {
        private Random random = new Random();

        private string[] namespaces = 
        {
            "System",
            "System.Collections.Generic",
            "System.Linq",
            "System.Text" ,
            "System.Threading.Tasks" ,
            "System.Media.SoundPlayer",
            "System.Net.Sockets",
            "System.Net.WebSockets"
        };

        private string[] variables = 
        {
            "int",
            "float",
            "double"
        };

        private string[] names = 
        {
            "x",
            "y",
            "z",
            "level",
            "name",
            "test",
            "counter",
            "i",
            "j",
            "k"
        };

        private string[] comments =
        {
            "This is just another comment",
            "Lorem ipsum",
            "dolor blah blah",
            "this is a comment",
            "comment here",
            "here is a comment",
            "commmmmeeeent",
            "coment again",
            "and here is another....comment !",
        };

        public string CreateRandomFile(int approximateSize, string extension)
        {
            if (!Directory.Exists("Tests"))
                Directory.CreateDirectory("Tests");

            string filename = "Tests/File" + random.Next(5000);

            StringBuilder sb = new StringBuilder();

            int commentsNo = random.Next(0, approximateSize * 4);
            int variableNo = random.Next(0, approximateSize * 4);

            using (StreamWriter writer = new StreamWriter(filename + extension))
            {
                for (int i = 0; i < namespaces.Length; i++)
                    sb.Append("using " + namespaces[i] + ";" + Environment.NewLine);

                sb.Append("class Program");
                sb.Append("{");

                sb.Append(GetRandomSingleComment() + Environment.NewLine);

                sb.Append(GetRandomMultiComment(random.Next(0, approximateSize)) + Environment.NewLine);

                for (int i = 0; i < approximateSize * 4; i++)
                {
                    if ((random.Next(0, 100)) % 33 == 0)
                        sb.Append(GetRandomSingleComment() + " ");
                    else if (random.Next(0, 100) % 16 == 0)
                        sb.Append(GetRandomMultiComment(1) + " ");

                    sb.Append(GetRandomVariable() + " " + GetRandomVariableName() + ";" + Environment.NewLine);
                }

                sb.Append("static void Main(string[] args)" + Environment.NewLine);
                sb.Append("{");

                for (int i = 0; i < approximateSize; i++)
                {
                    if ((random.Next(0, 100)) % 33 == 0)
                        sb.Append(GetRandomSingleComment() + " ");
                    else if (random.Next(0, 100) % 16 == 0)
                        sb.Append(GetRandomMultiComment(1) + " ");

                    sb.Append(GetRandomVariable() + " " + GetRandomVariableName() + " = " + random.Next(0,int.MaxValue) + ";");

                    if ((random.Next(0, 100)) % 33 == 0)
                        sb.Append(GetRandomSingleComment() + " ");
                    else if (random.Next(0, 100) % 16 == 0)
                        sb.Append(GetRandomMultiComment(1) + " ");

                    sb.Append(Environment.NewLine);
                }

                sb.Append("}");

                sb.Append("}");

                writer.Write(sb);
            }

            return filename;
        }

        private string GetRandomVariable()
        {
            return variables[random.Next(0, variables.Length - 1)];
        }

        private string GetRandomVariableName()
        {
            return names[random.Next(0, names.Length - 1)] + random.Next(0, 50000);
        }

        private string GetRandomNamespace()
        {
            return namespaces[random.Next(0, namespaces.Length - 1)];
        }

        private string GetRandomSingleComment()
        {
            return "//" + comments[random.Next(0, comments.Length - 1)];
        }

        private string GetRandomMultiComment(int amountOfLines)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("/*");

            for (int i = 0; i < amountOfLines; i++)
            {
                if (random.Next(0, 100) % 20 == 0)
                    sb.Append(GetRandomSingleComment());

                sb.Append(comments[random.Next(0, comments.Length - 1)] + (amountOfLines > 1 ? Environment.NewLine : ""));
            }

            sb.Append("*/");

            return sb.ToString();
        }
    }
}
