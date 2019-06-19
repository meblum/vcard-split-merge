using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using VCF;
using Ookii.Dialogs.Wpf;
using System.Threading.Tasks;

namespace ContactSplitMergeGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }



        private string SourcePath { get; set; }

        private string[] SourcePaths { get; set; }

        private string DestPath { get; set; }


        private void Button_Click_Input(object sender, RoutedEventArgs e)
        {
            if (spiltRadioBtn.IsChecked != mergeRadioBtn.IsChecked)
            {
                if (GetSourceFiles() == true)
                    sourceLbl.Content = spiltRadioBtn.IsChecked == true ? SourcePath : SourcePaths.Length + " Files selected";
            }

            else
            {
                MessageBox.Show("Please choose an option.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click_Output(object sender, RoutedEventArgs e)
        {
            if (spiltRadioBtn.IsChecked != mergeRadioBtn.IsChecked)
            {
                if (spiltRadioBtn.IsChecked == true)
                {
                    if (GetDestFolder() == true)
                        destLbl.Content = DestPath;
                }
                else if (mergeRadioBtn.IsChecked == true)
                {
                    if (GetSaveFile() == true)
                        destLbl.Content = DestPath;
                }
            }

            else
            {
                MessageBox.Show("Please choose an option.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            //if (gotSource != true)
            //{
            //    MessageBox.Show("Source not selected!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}


            if (spiltRadioBtn.IsChecked == true)
            {
                //if (GetDestFolder() == true)
                //    destLbl.Content = DestPath;
                VCFExtractor extractor;
                try
                {
                    extractor = new VCFExtractor(SourcePath, DestPath);

                }
                catch (InvalidDataException exception)
                {
                    MessageBox.Show(exception.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                progressBar.Maximum = extractor.TotalCardsInSource;
                contactsFoundLbl.Content = $"{extractor.TotalCardsInSource} Contacts found!";

                extractor.OnWritingFile += (contactNumber, fileName) => Dispatcher.Invoke(() =>
                {
                    progressBar.Value = contactNumber;
                    statsLbl.Content = $"{contactNumber} Extracting {fileName}";
                });

                extractor.OnExtractDone += (totalContacts, outputLocation) => Dispatcher.Invoke(() =>
                   {
                       MessageBox.Show($"{totalContacts} contacts succesfully extracted to {outputLocation}", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
                       BtnClear_Click(sender, e);
                   });

                Task extractorTask = new Task(extractor.ExtractToFiles);
                extractorTask.Start();
            }

            else if (mergeRadioBtn.IsChecked == true)
            {

                VCFMerger merger;
                try
                {
                     merger = new VCFMerger(SourcePaths, DestPath);
                }

                catch (InvalidDataException g)
                {
                    MessageBox.Show(g.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                progressBar.Maximum = merger.SourceFiles.Length;
                contactsFoundLbl.Content = $"{merger.SourceFiles.Length} contacts selected.";
                merger.OnWritingContact+= (contactNumber, fileName) => Dispatcher.Invoke(() =>
                {
                    progressBar.Value = contactNumber;
                    statsLbl.Content = $"{contactNumber} Merging {fileName}";
                });
                merger.OnMergeDone+=(totalContacts, outputLocation) => Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"{totalContacts} contacts succesfully merged to {outputLocation}", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
                    BtnClear_Click(sender, e);
                });

                Task extractorTask = new Task(merger.MergeContacts);
                extractorTask.Start();
                

            }

        }


        private bool? GetSourceFiles()
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Multiselect = spiltRadioBtn.IsChecked == true ? false : true,
                Filter = "Contact files (*.vcf, *.vcard)|*.vcf;*.vcard",
                Title = "Source:"
            };

            bool? result = fileDialog.ShowDialog();
            if (result == true)
            {
                if (mergeRadioBtn.IsChecked == true)
                    SourcePaths = fileDialog.FileNames;
                else if (spiltRadioBtn.IsChecked == true)
                    SourcePath = fileDialog.FileName;

            }
            return result;
        }


        private bool? GetSaveFile()
        {
            var dialog = new SaveFileDialog
            {
                AddExtension = true,
                CheckPathExists = true,
                Filter = "Contact file (*.vcf, *.vcard)|*.vcf;*.vcard",
                OverwritePrompt = true,
                ValidateNames = true,
                FileName = "vcard.vcf",
                DefaultExt = ".vcf",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
            };
            bool? result = dialog.ShowDialog();

            if (result == true)
                DestPath = dialog.FileName;
            return result;
        }

        private bool? GetDestFolder()
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog
            {
                Description = "Please select a folder.",
                UseDescriptionForTitle = true, // This applies to the Vista style dialog only, not the old dialog.
                ShowNewFolderButton = false,
                RootFolder = Environment.SpecialFolder.Desktop
            };
            bool? result = dialog.ShowDialog();
            if (result == true)
                DestPath = dialog.SelectedPath;
            return result;
        }

        
        public void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
            spiltRadioBtn.IsChecked = mergeRadioBtn.IsChecked = false;
            goButton.Content = "Choose Option";

        }


        private void Clear()
        {
            sourceLbl.Content = statsLbl.Content = contactsFoundLbl.Content = destLbl.Content = null;
            SourcePath = DestPath = null; SourcePaths = null; progressBar.Value = 0;
        }

        private void SplitChckBox_Checked(object sender, RoutedEventArgs e)
        {
            Clear();
            sourceLbl.Content = "No file selected";
            goButton.Content = "Split";
        }



        private void MergeChckBox_Checked(object sender, RoutedEventArgs e)
        {
            Clear();
            sourceLbl.Content = "0 Contacts selected";
            goButton.Content = "Merge";
        }


    }
}
