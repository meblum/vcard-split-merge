using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCF;

namespace VCF
{
    public class VCFExtractorLocations
    {

        private string sourceFile;
        public string SourceFile
        {
            get
            { return sourceFile; }
            set { VCFValidator.ValidateFile(value); sourceFile = value; }
        }
        private string destinationFolder;
        public string DestinationFolder
        {
            get
            { return destinationFolder; }
            set { VCFValidator.ValidateDirectory(value); destinationFolder = value; }
        }
    }
}
