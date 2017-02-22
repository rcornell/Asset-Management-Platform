﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Utility
{
    public interface IPortfolioDatabaseService
    {

        Task BuildDatabaseTaxlots();

        List<Position> GetPositionsFromTaxlots(List<Security> portfolioSecurities);

        List<Position> GetPositionsFromTaxlots();

        List<Position> GetEmptyPositionsList();

        void SavePortfolioToDatabase();

        void UploadLimitOrdersToDatabase(List<LimitOrder> limitOrders);

        Task LoadLimitOrdersFromDatabase();

        void BackupDatabase();

        void AddToPortfolioDatabase(Taxlot taxlotToAdd);

        void DeletePortfolio(List<Position> positions);

        List<Taxlot> BuildLocalTaxlots(List<Taxlot> taxlots);

    }
}
