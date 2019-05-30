using System;
using System.IO;

namespace VcfApp
{
    static class VCF
    {

        /// <summary>
        /// Fires when a contact is written
        /// </summary>
        public static event Action<string, int> WritingContact;


        /// <summary>
        /// Gets an array of name card paths, reads each file and appends it to the specified file.
        /// If any of the files are not of name card type, or doesn't exist, an exception is thrown.
        /// </summary>
        /// <param name="sourcePaths">The array of paths</param>
        /// <param name="dest">The destination file</param>
        public static void MergeContacts(string[] sourcePaths, string dest)
        {
            
            foreach (string file in sourcePaths)
            {
                ValidateFile(file);
            }
            ValidateFileExtension(dest);

            using (StreamWriter writer = File.AppendText(dest))
            {

                int cardNumber = 0;

                foreach (string path in sourcePaths)
                {
                    cardNumber++;
                    WritingContact?.Invoke(Path.GetFileName(path), cardNumber);

                    string card = File.ReadAllText(path);
                    writer.Write(card + Environment.NewLine);
                }
            }
        }


        /// <summary>
        /// Makes sure file is contact file
        /// </summary>
        /// <param name="dest">File to check</param>
        private static void ValidateFileExtension(string file)
        {
            if (Path.GetExtension(file) != ".vcf" && Path.GetExtension(file) != ".vcard")
                throw new InvalidDataException($"{file} Invalid file type!");
        }


        /// <summary>
        /// Takes a file, reads each contact and writes it to a new file
        /// </summary>
        /// <param name="source">Source filr</param>
        /// <param name="dest">Destination directory</param>
        public static void ExtractContactsAndWriteToFiles(string source, string dest)
        {
            ValidateFile(source);
            ValidateDirectory(dest);

            using (StreamReader sourceFileReader = File.OpenText(source))
            {

                int cardNumber = 0;

                while (sourceFileReader.Peek() >= 0)
                {

                    cardNumber++;

                    int line = 0;
                    string contact = "";
                    string name = "";

                    while (true)
                    {

                        line++;

                        string cardLine = sourceFileReader.ReadLine();

                        //break if this line is a contact seperator.
                        if (cardLine == "") break;

                        contact += cardLine + Environment.NewLine;
                        if (line == 4)
                            //this line contains the name
                            name = ExtractNameFromNameLine(cardLine);
                    }

                    WritingContact?.Invoke(name, cardNumber);
                    WriteFile(contact, name, dest);
                }
            }
        }


        /// <summary>
        /// Checks if a directory exists. If not, throws InvalidDataException
        /// </summary>
        /// <param name="dir">The directory to be checked</param>
        private static void ValidateDirectory(string dir)
        {
            if (!Directory.Exists(dir))
            {
                throw new InvalidDataException($"{dir} Invalid output directory!");
            }
        }


        /// <summary>
        /// Checks if file exists, and has .vcf or .vcard extension.
        /// If not, InvalidDataException is thrown.
        /// </summary>
        /// <param name="file">File to be checked</param>
        private static void ValidateFile(string file)
        {

            if (!File.Exists(file))
            {
                throw new InvalidDataException($"{file} File does not exist!");
            }
            ValidateFileExtension(file);
        }


        /// <summary>
        /// Takes the fourth line of a contact and returns the name of it.
        /// If it does not have a name, "Unnamed" is returned.
        /// </summary>
        /// <param name="nameLine">The fourth line of a contact</param>
        /// <returns>The contact name</returns>
        private static string ExtractNameFromNameLine(string nameLine)
        {
            string fileName;
            var splittedThirdLine = nameLine.Split(':');

            if (splittedThirdLine.Length < 2)
            {
                throw new InvalidDataException($"Invalid name line! -{nameLine}");
            }

            if (string.IsNullOrWhiteSpace(splittedThirdLine[1]))
            {

                fileName = "Unnamed";
                return fileName;
            }

            fileName = $"{splittedThirdLine[1]}";
            return fileName;
        }


        /// <summary>
        /// Takes a contact, creates a file named by the contact name, and saves it in specified directory.
        /// If the file name exists, a number is added to the file name.
        /// </summary>
        /// <param name="contact">The contact to save</param>
        /// <param name="name">Name of the contact</param>
        /// <param name="dest">Directory</param>
        private static void WriteFile(string contact, string name, string dest)
        {

            int duplicateCounter = 0;

            string path = Path.Combine(dest, $"{name}.vcf");

            while (File.Exists(path))
            {
                path = Path.Combine(dest, $"{name}-{++duplicateCounter}.vcf");
            }

            File.WriteAllText(path, contact);
        }
    }
}
