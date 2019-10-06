using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCF
{
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
                    VCFValidator.ValidateFile(file);
                }
                sourceFiles = value;
            }
        }

        private string destinationFile;
        public string DestinationFile
        {
            get { return destinationFile; }
            set { VCFValidator.ValidateFileExtension(value); destinationFile = value; }
        }
    }
}
