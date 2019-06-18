using System;
using System.IO;
using System.Text;

namespace VCF
{
    public class VCFExtractor
    {
        public VCFExtractor(string sourceFile, string destinationFolder)
        {
            VCFTools.ValidateFile(sourceFile);
            VCFTools.ValidateDirectory(destinationFolder);
            this.SourceFile = sourceFile; this.DestinationFolder = destinationFolder;
        }
        private string SourceFile { get; }
        private string DestinationFolder { get; }

        public event Action<int, string> OnWritingFile;
        public event Action<int, string> OnExtractDone;

        /// <summary>
        /// Takes a file, reads each contact and writes it to a new file
        /// </summary>
        /// <param name="source">Source filr</param>
        /// <param name="dest">Destination directory</param>
        public void ExtractToFiles()
        {
            int cardNumber = 0;


            using (StreamReader sourceFileReader = File.OpenText(SourceFile))
            {

                while (sourceFileReader.Peek() >= 0)
                {

                    cardNumber++;

                    StringBuilder contact = new StringBuilder();
                    string name = "";

                    while (sourceFileReader.Peek() >= 0)
                    {

                        string cardLine = sourceFileReader.ReadLine();


                        if (cardLine == "")
                            //ignore it...
                            continue;

                        contact.Append(cardLine + Environment.NewLine);

                        if (cardLine.StartsWith("END"))
                            //we have a contact, write it and go to next contact
                            break;

                        else if (cardLine.StartsWith("FN"))
                        {
                            //this line contains the name
                            name = ExtractNameFromNameLine(cardLine);
                        }

                    }
                    //name could be empty string if file ends with empty newline,
                    //this also means contact is empty.
                    if (name != "")
                    {
                        OnWritingFile?.Invoke(cardNumber, name);
                        VCFTools.WriteToFile(contact.ToString(), name, DestinationFolder);
                    }

                }
            }
            OnExtractDone?.Invoke(cardNumber, DestinationFolder);
        }
        /// <summary>
        /// Takes the FN line of a contact and returns the name of it.
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

    }
}