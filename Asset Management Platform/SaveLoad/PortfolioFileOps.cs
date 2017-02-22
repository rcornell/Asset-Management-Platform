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
        public async Task<bool> TrySaveSession(SessionData sessionData)
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
                var jsonString = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(sessionData));
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

        public async Task<SessionData> TryLoadSession()
        {
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
                return new SessionData();

            var path = dialog.FileName;

            try
            {
                var result = File.ReadAllText(path);
                var sessionData =
                    await Task.Factory.StartNew(
                        () => JsonConvert.DeserializeObject<SessionData>(result));

                return sessionData;
            }
            catch (JsonReaderException ex)
            {
                //var message = @"Problem reading your list of taxlots. Please check to see that the json is valid.";
                Messenger.Default.Send(new FileErrorMessage(ex.Message));
                return new SessionData();
            }
            catch (ArgumentException ex)
            {
                Messenger.Default.Send(new FileErrorMessage(ex.Message));
                return new SessionData();
            }
            catch (FileNotFoundException ex)
            {
                Messenger.Default.Send(new FileErrorMessage(ex.Message));
                return new SessionData();
            }
            catch (IOException ex)
            {
                Messenger.Default.Send(new FileErrorMessage(ex.Message));
                return new SessionData();
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new FileErrorMessage(ex.Message));
                return new SessionData();
            }
        }

        public void Dispose()
        {
            documentsPath = string.Empty;
        }
    }
}
