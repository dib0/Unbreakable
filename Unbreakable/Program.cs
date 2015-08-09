using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unbreakable
{
    class Program
    {
        #region Private properties
        private static string inFile = string.Empty;
        private static string keyFile = string.Empty;
        private static string outFile = string.Empty;
        private static bool encrypt = true;
        private static int offset = 0;
        private static StreamReader keyReader = null;
        #endregion

        #region Main
        static void Main(string[] args)
        {
            if (!HandleParams(args))
            {
                ShowHelp(false);
            }
            else
            {
                if (encrypt)
                    EncryptFile();
                else
                    DecryptFile();

                Console.WriteLine("Done.");
            }
        }
        #endregion

        #region Private methods
        private static bool HandleParams(string[] args)
        {
            if ((args == null) || (args.Length == 0))
            {
                ShowHelp(true);
                return false;
            }

            if (args.Contains("--help"))
            {
                ShowHelp(false);
                return false;
            }

            if (args.Contains("-d") || args.Contains("--decrypt"))
            {
                encrypt = false;
            }
            else if (args.Contains("-e") || args.Contains("--encrypt"))
            {
                encrypt = true;
            }
            else
            {
                Console.WriteLine("Error: No encrypt or decrypt option given.");
                return false;
            }

            if (args.Contains("-i") || args.Contains("--in"))
            {
                int i = args.Contains("-i") ? GetIndex(args, "-i") : GetIndex(args, "--in");

                inFile = args[i + 1];
                if (!File.Exists(inFile))
                {
                    Console.WriteLine("Error: Specified input file is not found!");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Error: No inputfile specified.");
                return false;
            }

            if (args.Contains("-o") || args.Contains("--out"))
            {
                int i = args.Contains("-o") ? GetIndex(args, "-o") : GetIndex(args, "--out");

                outFile = args[i + 1];
                try
                {
                    using (StreamWriter sw = new StreamWriter(outFile))
                        sw.Write("test");
                    File.Delete(outFile);
                }
                catch (IOException)
                {
                    Console.WriteLine("Error: Specified output file could not be created.");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Error: No outputfile specified.");
                return false;
            }

            if (args.Contains("-k") || args.Contains("--key"))
            {
                int i = args.Contains("-k") ? GetIndex(args, "-k") : GetIndex(args, "--key");

                keyFile = args[i + 1];
                if (!File.Exists(keyFile))
                {
                    Console.WriteLine("Error: Specified key file is not found!");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Error: No outputfile specified.");
                return false;
            }

            if (args.Contains("-f") || args.Contains("--offset"))
            {
                int i = args.Contains("-f") ? GetIndex(args, "-f") : GetIndex(args, "--offset");

                string ff = args[i + 1];
                if (!int.TryParse(ff, out offset))
                {
                    offset = 0;
                    Console.WriteLine("Warning: invalid offset given, default value of 0 will be used.");
                }
            }

            return true;
        }

        private static int GetIndex(string[] args, string v)
        {
            int result = -1;

            for (int i = 0; i < args.Length; i++)
                if (args[i] == v)
                    result = i;

            return result;
        }

        private static void ShowHelp(bool v)
        {
            if (v)
            {

            }
            else
            {
                Console.WriteLine("Usage: Unbreakable.exe [-e or -d] [-i infile] [-o outfile] [-k keyfile] [-f offset, optional]");
            }
        }

        private static void EncryptFile()
        {
            for (int i = 0; i < offset; i++)
                GetNextKey();

            using (StreamWriter of = new StreamWriter(outFile))
            {
                using (StreamReader fi = new StreamReader(inFile))
                {
                    while (!fi.EndOfStream)
                    {
                        int ch = fi.Read();
                        ch += GetNextKey();
                        if (ch > 255)
                            ch = ch % 255;

                        of.Write((char)ch);
                    }
                }
            }
            keyReader.Close();
        }

        private static void DecryptFile()
        {
            for (int i = 0; i < offset; i++)
                GetNextKey();

            using (StreamWriter of = new StreamWriter(outFile))
            {
                using (StreamReader fi = new StreamReader(inFile))
                {
                    while (!fi.EndOfStream)
                    {
                        int ch = fi.Read();
                        ch -= GetNextKey();
                        if (ch < 0)
                            ch = 255 - ch;

                        of.Write((char)ch);
                    }
                }
            }
            keyReader.Close();
        }

        private static int GetNextKey()
        {
            if (keyReader == null)
                keyReader = new StreamReader(keyFile);

            int result = keyReader.Read();
            if (keyReader.EndOfStream)
            {
                keyReader.Close();
                keyReader = new StreamReader(keyFile);
            }

            return result;
        }
        #endregion
    }
}
