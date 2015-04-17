﻿namespace Endjin.Leasing.Azure.Specs.Configuration
{
    #region Using Directives

    using Endjin.Contracts.Leasing.Azure.Configuration;

    #endregion

    public class ConnectionStringProvider : IConnectionStringProvider
    {
        public string ConnectionStringKey
        {
            get
            {
                return "Storage.Leasing.ConnectionString";
            }
        }
    }
}