using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCF
{
    class VCFMerger
    {
        public VCFMerger(VCFMergerLocations locations)
        {
            if (locations.SourceFiles == null)
            {
                throw new InvalidDataException("Please choose the source files!");
            }
            else if (locations.DestinationFile == null)
            {
                throw new InvalidDataException("Please supply a destination file!");
            }
            SourceFiles = locations.SourceFiles; DestinationFile = locations.DestinationFile;
        }
        private string[] SourceFiles { get; }
        private string DestinationFile { get; }
        public int TotalCards => SourceFiles.Length;
        /// <summary>
        /// Fires when a contact is written
        /// </summary>
        public event Action<int, string> OnWritingContact;
        public event Action<int, string> OnMergeDone;


        /// <summary>
        /// Gets an array of name card paths, reads each file and appends it to the specified file.
        /// If any of the source files are not of name card type, or doesn't exist, an exception is thrown.
        /// If the destination path doesn't exist, it's created, if its not of name card type,
        // an exception is thrown.
        /// </summary>
        /// <param name="sourcePaths">The array of paths</param>
        /// <param name="dest">The destination file</param>
        public void MergeContacts()
        {
            int cardNumber = 0;

            using (StreamWriter writer = File.AppendText(DestinationFile))
            {


                foreach (string path in SourceFiles)
                {
                    string card = File.ReadAllText(path);
                    cardNumber++;
                    OnWritingContact?.Invoke(cardNumber, Path.GetFileName(path));
                    writer.Write(card + Environment.NewLine);
                }
            }

            OnMergeDone?.Invoke(cardNumber, DestinationFile);
        }

    }

}


