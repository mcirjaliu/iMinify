using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iMinify
{
    public enum MinifierLanguage { CSharp }

    class Minifier
    {
        private MinifierLanguage Language;

        const byte Whitespace = 0x20;
        const byte Tab = 0x09;
        const byte Slash = 0x2f;
        const byte Times = 0x2a;
        const byte Newline = 0x0A;

        public Minifier(MinifierLanguage Language)
        {
            this.Language = Language;
        }

        public void Minify(string filename, bool debugging = false)
        {
            string extension = null;

            switch (this.Language)
            {
                case MinifierLanguage.CSharp:
                    extension = ".cs";
                    break;
                default:
                    extension = ".txt";
                    break;
            }

            Console.WriteLine("Starting to minify the file {0}{1}",filename,extension);

            this.CreateFile(filename, extension);
            this.RemoveComments(filename + ".minified", extension);
            this.RemoveWhitespaces(filename + ".minified", extension);

            Console.WriteLine("Done !");

            if (debugging)
                this.Debug(filename + ".minified", extension);
        }

        private void Debug(string filename, string extension)
        {
            try
            {
                using (StreamReader reader = new StreamReader(filename + extension))
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        for (int i = 0; i < line.Length; i++)
                        {
                            byte b = (byte)line[i];
                            Console.WriteLine("{0} => {1}", line[i], b.ToString("x"));
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine(string.Format("Error in function [{0}] : {1}", nameof(Debug), "File not found"));
            }
        }

        private void CreateFile(string filename, string extension)
        {
            Stopwatch creatingWatch = Stopwatch.StartNew();

            File.Copy(filename + extension, filename + ".minified" + extension, true);

            creatingWatch.Stop();

            Console.WriteLine("- {0} took {1}ms , original file size => {2}", "Creating the file", creatingWatch.ElapsedMilliseconds, GetFileSize(filename + extension));
        }

        private void RemoveComments(string filename, string extension)
        {
            Stopwatch commentsWatch = Stopwatch.StartNew();

            string output = null;

            try
            {
                using (StreamReader reader = new StreamReader(filename + extension))
                {
                    string line = null;
                    bool isMultiline = false;

                    while ((line = reader.ReadLine()) != null)
                    {
                        char[] arr = line.ToCharArray();
                        bool ignore = false;
                        int[] to_ignore = new int[arr.Length * 2];
                        int ignore_index = 0;

                        for (int i = 0; i < arr.Length; i++)
                        {
                            if (i == 0 && !isMultiline && !ignore)
                                if ((byte)arr[0] != Slash && (byte)arr[0] != Times)
                                    output += arr[0];

                            if (i != arr.Length - 1)
                            {
                                if ((byte)arr[i] == Slash && (byte)arr[i + 1] == Slash || ((byte)arr[i] == Times && isMultiline))
                                    ignore = true;

                                if ((byte)arr[i] == Slash && (byte)arr[i + 1] == Times)
                                {
                                    to_ignore[ignore_index++] = i;
                                    to_ignore[ignore_index++] = i + 1;
                                    ignore = true;
                                    isMultiline = true;
                                }

                                if ((byte)arr[i] == Times && (byte)arr[i + 1] == Slash)
                                {
                                    to_ignore[ignore_index++] = i;
                                    to_ignore[ignore_index++] = i + 1;
                                    ignore = false;
                                    isMultiline = false;
                                }
                            }

                            if (ignore)
                                to_ignore[ignore_index++] = i;
                            else
                                if (!to_ignore.Contains(i))
                                    if (!isMultiline)
                                        output += arr[i];
                        }
                    }
                }

                using (StreamWriter writer = new StreamWriter(filename + extension))
                    writer.Write(output);
            }

            catch (FileNotFoundException)
            {
                Console.WriteLine(string.Format("Error in function {0} : {1}", nameof(RemoveComments), "File not found"));
            }

            commentsWatch.Stop();

            Console.WriteLine("- {0} took {1}ms , new file size => {2}", "Removing the comments", commentsWatch.ElapsedMilliseconds, GetFileSize(filename + extension));
        }

        private void RemoveWhitespaces(string filename, string extension)
        {
            Stopwatch whitespaceWatch = Stopwatch.StartNew();

            string output = null;
            bool needsRecall = false;

            using (StreamReader reader = new StreamReader(filename + extension))
            {
                string line = null;

                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Replace((char)Tab, (char)Whitespace).Replace((char)Newline, (char)Whitespace);

                    char[] arr = line.ToCharArray();

                    bool lastCharacterWasSpace = false;

                    int whitespaceOccurences = 0;

                    for (int i = 0; i < arr.Length; i++)
                    {
                        if (IsWhiteSpace(line[i]))
                        {
                            if (!lastCharacterWasSpace)
                            {
                                output += line[i];
                                lastCharacterWasSpace = true;
                            }
                            else
                            {
                                whitespaceOccurences++;
                                if (whitespaceOccurences > 2)
                                    needsRecall = true;
                            }
                        }
                        else
                        {
                            lastCharacterWasSpace = false;
                            output += line[i];
                        }
                    }

                    if (output != null && output[output.Length - 1] != (char)Whitespace && line.Length > 0 && line[0] != (char)Whitespace)
                        output += (char)Whitespace;
                }
            }

            using (StreamWriter writer = new StreamWriter(filename + extension))
            {
                writer.Write(output);
            }

            whitespaceWatch.Stop();

            Console.WriteLine("- {0} took {1}ms , new file size => {2}", "Removing the white spaces", whitespaceWatch.ElapsedMilliseconds, GetFileSize(filename + extension));

            if (needsRecall)
                this.RemoveWhitespaces(filename, extension);
        }

        private string GetFileSize(string filename)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = new FileInfo(filename).Length;
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return String.Format("{0:0.#} {1}", len, sizes[order]);
        }

        private bool IsWhiteSpace(char c)
        {
            return (byte)c == Whitespace || (byte)c == Tab || (byte)c == Newline ? true : false;
        }
    }
}
