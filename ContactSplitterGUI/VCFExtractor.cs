using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace VCF
{
    public class VCFExtractor
    {
        public VCFExtractor(VCFExtractorLocations locations)
        {
            if (locations.SourceFile == null)
            {
                throw new InvalidDataException("Please supply a source file!");
            }
            else if (locations.DestinationFolder == null)
            {
                throw new InvalidDataException("Please supply a destination folder!");
            }

            SourceFile = locations.SourceFile; DestinationFolder = locations.DestinationFolder;

        }
        private string SourceFile { get; }
        private string DestinationFolder { get; }
        private readonly List<string> files = new List<string>();
        private int totalCards;
        public int TotalCardsInSource
        {
            get
            {
                if (totalCards == 0)
                {
                    totalCards = ContactsInSource();
                }
                return totalCards;
            }
        }
        public event Action<int, string> WritingFile;
        public event Action<int, string> ExtractDone;
        public event Action Cancelled;
        public int CardNumber { get; private set; }
        /// <summary>
        /// Takes a file, reads each contact and writes it to a new file
        /// </summary>
        /// <param name="source">Source filr</param>
        /// <param name="dest">Destination directory</param>
        public void ExtractToFiles(CancellationToken token)
        {



            using (StreamReader sourceFileReader = File.OpenText(SourceFile))
            {
                StringBuilder contact = new StringBuilder();

                while (sourceFileReader.Peek() >= 0)
                {


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
                    string card = contact.ToString();
                    if (name != string.Empty && card.StartsWith("BEGIN"))
                    {
                        WritingFile?.Invoke(++CardNumber, name);
                        
                        files.Add(VCFTools.WriteToFile(card, name, DestinationFolder));
                        contact.Clear();
                        if (token.IsCancellationRequested)
                        {
                            foreach (var item in files)
                            {
                                File.Delete(item);
                            }
                            Cancelled?.Invoke();
                            throw new OperationCanceledException(token);
                        }
                    }

                }
            }
            
            ExtractDone?.Invoke(CardNumber, DestinationFolder);
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

        private int ContactsInSource()
        {
            int count = 0;
            using (StreamReader sourceReader = new StreamReader(SourceFile))
            {
                string line;
                while (sourceReader.Peek() >= 0)
                {
                    line = sourceReader.ReadLine();
                    if (line.StartsWith("BEGIN")) count++;
                }

            }
            return count;

        }

    }
}