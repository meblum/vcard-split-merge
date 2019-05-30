using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using VcfApp;
using Ookii.Dialogs.Wpf;


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
            VCF.WritingContact += NameCard_WritingContact;

        }

        int contactCounter = 0;
        bool? gotSource;

        public string SourcePath { get; private set; }

        public string[] SourcePaths { get; private set; }

        public string DestPath { get; private set; }

        private void Button_Click_Input(object sender, RoutedEventArgs e)
        {

            if (spiltRadioBtn.IsChecked != mergeRadioBtn.IsChecked)
            {
                if (GetSourceFiles() == true)
                    sourceLbl.Content = spiltRadioBtn.IsChecked == true ? SourcePath : SourcePaths.Length + " Contacts selected";

            }


            else
            {
                MessageBox.Show("Please choose an option.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            if (gotSource != true)
            {
                MessageBox.Show("Source not selected!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            if (spiltRadioBtn.IsChecked == true)
            {
                if (GetDestFolder() == true)
                    destLbl.Content = DestPath;
                try
                {

                    VCF.ExtractContactsAndWriteToFiles(SourcePath, DestPath);
                }

                catch (InvalidDataException f)
                {
                    MessageBox.Show(f.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                doneLbl.Content = "Done!";
                MessageBox.Show($"{contactCounter} contacts succesfully extracted!", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
                BtnClear_Click(sender, e);
            }

            else if (mergeRadioBtn.IsChecked == true)
            {
                if (GetSaveFile() == true)
                    destLbl.Content = DestPath;

                try
                {
                    VCF.MergeContacts(SourcePaths, DestPath);
                }
                
                catch (InvalidDataException g)
                {
                    MessageBox.Show(g.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                doneLbl.Content = "Done!";
                MessageBox.Show($"{contactCounter} contacts succesfully merged!", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
                BtnClear_Click(sender, e);

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
            gotSource = result;
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

        private void NameCard_WritingContact(string name, int number)
        {
            contactCounter++;

            statsLbl.Content = $"{number} Writing {name} to file";

        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
            spiltRadioBtn.IsChecked = mergeRadioBtn.IsChecked = false;
            goButton.Content = "Choose Option";

        }


        private void Clear()
        {
            sourceLbl.Content = doneLbl.Content = statsLbl.Content = contactsFoundLbl.Content = destLbl.Content = null;
            SourcePath = DestPath = null; SourcePaths = null; contactCounter = 0; gotSource = false;
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
