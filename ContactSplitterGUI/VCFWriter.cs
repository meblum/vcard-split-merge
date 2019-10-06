using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCF
{
    public static class VCFWriter
    {
        /// <summary>
        /// Takes a contact, creates a file and saves it in specified directory.
        /// If the file name exists, a number is added to the file name.
        /// </summary>
        /// <param name="contact">The contact to save</param>
        /// <param name="name">File name</param>
        /// <param name="dest">Output directory</param>
        public static string WriteToFile(string contact, string name, string dest)
        {

            int duplicateCounter = 0;

            string cleanName = RemoveInvalidChars(name);
            string path = Path.Combine(dest, $"{cleanName}.vcf");


            while (File.Exists(path))
            {
                path = Path.Combine(dest, $"{cleanName}-{++duplicateCounter}.vcf");
            }

            File.WriteAllText(path, contact);
            return path;
        }


        public static string RemoveInvalidChars(string filename)
        {
            return string.Concat(filename.Split(Path.GetInvalidFileNameChars()));
        }
    }
}
