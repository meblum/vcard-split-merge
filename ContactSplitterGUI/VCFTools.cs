using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace VCF
{

    public class VCFExtractorLocations
    {

        private string sourceFile;
        public string SourceFile
        {
            get
            { return sourceFile; }
            set { VCFTools.ValidateFile(value); sourceFile = value; }
        }
        private string destinationFolder;
        public string DestinationFolder
        {
            get
            { return destinationFolder; }
            set { VCFTools.ValidateDirectory(value); destinationFolder = value; }
        }
    }

    public class VCFMergerLocations
    {
        private string[] sourceFiles;
        public string[] SourceFiles
        {
            get { return sourceFiles; }
            set
            {
                foreach (string file in value)
                {
                    VCFTools.ValidateFile(file);
                }
                sourceFiles = value;
            }
        }

        private string destinationFile;
        public string DestinationFile
        {
            get { return destinationFile; }
            set { VCFTools.ValidateFileExtension(value); destinationFile = value; }
        }
    }
    public static class VCFTools
    {
        /// <summary>
        /// Makes sure file is contact file
        /// </summary>
        /// <param name="dest">File to check</param>
        public static void ValidateFileExtension(string file)
        {
            if (Path.GetExtension(file) != ".vcf" && Path.GetExtension(file) != ".vcard")

                throw new InvalidDataException($"{file} Invalid file type!");
        }

        /// <summary>
        /// Checks if a directory exists. If not, throws InvalidDataException
        /// </summary>
        /// <param name="dir">The directory to be checked</param>
        public static void ValidateDirectory(string dir)
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
        public static void ValidateFile(string file)
        {

            if (!File.Exists(file))
            {
                throw new InvalidDataException($"{file} File does not exist!");
            }
            ValidateFileExtension(file);
        }

        /// <summary>
        /// Takes a contact, creates a file and saves it in specified directory.
        /// If the file name exists, a number is added to the file name.
        /// </summary>
        /// <param name="contact">The contact to save</param>
        /// <param name="name">File name</param>
        /// <param name="dest">Output directory</param>
        public static void WriteToFile(string contact, string name, string dest)
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
