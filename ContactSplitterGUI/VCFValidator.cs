using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace VCF
{

    

    
    public static class VCFValidator
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

        
    }
}
