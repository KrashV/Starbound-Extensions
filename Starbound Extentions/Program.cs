using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Starbound_Extensions
{
    class Program
    {
        private static string starbound_path;

        static void Main(string[] args)
        {
            string app_location = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath;

            if (args.Length == 0)
            {
                Console.WriteLine("Usage: \"Starbound Extensions\" path_to_pak_or_directory");
                Console.ReadLine();
                return;
            }


            // retreive the starbound directory
            starbound_path = Path.GetDirectoryName(app_location);

            // get the file attributes for file or directory
            FileAttributes attr = File.GetAttributes(@args[0]);

            // request the startInfo depending on the extension
            ProcessStartInfo starInfo;
            try
            {
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    starInfo = ProcessDirectoty(@args[0]);
                else
                    starInfo = ProcessFile(@args[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Path:" + starbound_path);
                return;
            }

            Process process = new Process();
            process.StartInfo = starInfo;
            process.Start();

            while (!process.HasExited) { Console.WriteLine(process.StandardOutput.ReadLine()); }

            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static ProcessStartInfo ProcessDirectoty(string directoryPath)
        {
                string folderName = Path.GetFileName(directoryPath);
                string folderDirectory = Path.GetDirectoryName(directoryPath);

                string packerPath = @starbound_path + "\\" + "asset_packer.exe";

                if (!File.Exists(packerPath))
                {
                    throw new FileNotFoundException("asset_packer.exe doesn't exist");
                }

                return new ProcessStartInfo(){
                    FileName = packerPath,
                    WorkingDirectory = @folderDirectory,
                    Arguments = "\"" + @directoryPath + "\" " + "\"" + @folderName + ".pak" + "\"",
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false
                };
        }


        private static ProcessStartInfo ProcessFile(string filePath)
        {
                string fileDirectory = Path.GetDirectoryName(filePath);
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string fileExtension = Path.GetExtension(filePath);

                string processName;
                string processExtension;

                switch (fileExtension)
                {
                    case ".pak":
                        processName = "asset_unpacker.exe";
                        processExtension = "";
                        break;
                    case ".player":
                        processName = "dump_versioned_json.exe";
                        processExtension = ".json";
                        break;
                    case ".json":
                        processName = "make_versioned_json.exe";
                        processExtension = ".player";
                        break;
                    default:
                        throw new FormatException(String.Format("Can't proceed the {0} file: {1} is not a starbound extension", fileName, fileExtension));
                }

                string processPath = @starbound_path + "\\" + processName;

                if (!File.Exists(processPath))
                {
                    throw new FileNotFoundException(processName + " doesn't exist");
                }

                return new ProcessStartInfo()
                {
                    FileName = processPath,
                    WorkingDirectory = @fileDirectory,
                    Arguments = "\"" + @filePath + "\"" + " " + "\"" + @fileName + processExtension + "\"",
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false
                };
        }
    }
}
