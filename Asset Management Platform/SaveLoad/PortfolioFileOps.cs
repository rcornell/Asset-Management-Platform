using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Asset_Management_Platform.Messages;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using Microsoft.Win32;

namespace Asset_Management_Platform.SaveLoad
{
    public class PortfolioFileOps : IDisposable
    {
        private string documentsPath;

        public PortfolioFileOps()
        {
            documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        public bool TrySaveTaxlots(ObservableCollection<Position> positions)
        {
            


            return false;
        }
        public async Task<bool> TrySaveTaxlots(ObservableCollection<Taxlot> taxlots)
        {
            var dialog = new SaveFileDialog
            {
                InitialDirectory = documentsPath,
                CheckFileExists = false,
                CheckPathExists = true,
                AddExtension = true,
                DefaultExt = ".json",
                FileName = "MyPortfolio",
                Filter = "JSON|*.json",
                OverwritePrompt = true,
                Title = @"Choose where to save your portfolio data."
            };

            bool? savePathConfirmed = dialog.ShowDialog();

            if (savePathConfirmed == null || !savePathConfirmed.Value)
                return false;

            var path = dialog.FileName;
            try
            {
                var jsonString = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(taxlots));
                File.WriteAllText(path, jsonString);
                return true;
            }
            catch (JsonSerializationException ex)
            {

            }
            catch (ArgumentException ex)
            {

            }
            catch (FileNotFoundException ex)
            {

            }
            catch (IOException ex)
            {

            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public async Task<ObservableCollection<Taxlot>> TryLoadPortfolio()
        {
            var taxlotsToReturn = new ObservableCollection<Taxlot>();

            var dialog = new OpenFileDialog()
            {
                InitialDirectory = documentsPath,
                CheckFileExists = true,
                CheckPathExists = true,
                AddExtension = true,
                DefaultExt = ".json",
                FileName = "MyPortfolio",
                Filter = "JSON|*.json",
                Title = @"Choose a portfolio to load."
            };

            bool? loadPathConfirmed = dialog.ShowDialog();

            if (loadPathConfirmed == null || !loadPathConfirmed.Value)
                return taxlotsToReturn;

            var path = dialog.FileName;

            try
            {
                var result = File.ReadAllText(path);
                var taxlotList =
                    await Task.Factory.StartNew(
                        () => JsonConvert.DeserializeObject<ObservableCollection<Taxlot>>(result));

                taxlotsToReturn = taxlotList;
                return taxlotsToReturn;
            }
            catch (JsonReaderException ex)
            {
                //var message = @"Problem reading your list of taxlots. Please check to see that the json is valid.";
                Messenger.Default.Send(new FileErrorMessage(ex.Message));
                return taxlotsToReturn;
            }
            catch (ArgumentException ex)
            {
                Messenger.Default.Send(new FileErrorMessage(ex.Message));
                return taxlotsToReturn;
            }
            catch (FileNotFoundException ex)
            {
                Messenger.Default.Send(new FileErrorMessage(ex.Message));
                return taxlotsToReturn;
            }
            catch (IOException ex)
            {
                Messenger.Default.Send(new FileErrorMessage(ex.Message));
                return taxlotsToReturn;
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new FileErrorMessage(ex.Message));
                return taxlotsToReturn;
            }
        }

        public void Dispose()
        {
            documentsPath = string.Empty;
        }
    }
}
