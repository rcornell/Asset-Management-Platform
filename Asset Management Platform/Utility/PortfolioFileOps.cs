using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Win32;

namespace Asset_Management_Platform.Utility
{
    public class PortfolioFileOps : IDisposable
    {
        public PortfolioFileOps()
        {
            
        }

        public bool TrySaveTaxlots(ObservableCollection<Position> positions)
        {
            


            return false;
        }
        public async Task<bool> TrySaveTaxlots(ObservableCollection<Taxlot> taxlots)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
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
            var jsonString = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(taxlots));
            File.WriteAllText(path, jsonString);

            return true;
        }

        public List<Taxlot> TryLoadPortfolio()
        {
            var taxlotsToReturn = new List<Taxlot>();



            return taxlotsToReturn;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
