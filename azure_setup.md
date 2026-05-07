## Azure Setup Summary

All required steps for deploying the application infrastructure in Azure have been completed:

1. **App Service Plan**
    - Created a Free F1 App Service Plan for hosting the application backend.

2. **App Service**
    - Deployed a .NET 8 API as an App Service within the plan.

3. **Azure SQL Database**
    - Created an Azure SQL Database using the serverless tier for cost-effective, auto-scaling development and production.

4. **Firewall Configuration**
    - Configured SQL server firewall to allow Azure services (including App Service) to access the database.

5. **Connection String Management**
    - Retrieved the ADO.NET connection string from the Azure Portal.
    - Stored the connection string in the App Service Configuration under "Connection strings" as `DefaultConnection` (type: SQLServer), with credentials securely managed outside of source code.

6. **Environment-specific Configuration**
    - Application uses SQL Server locally (via appsettings.Development.json).
    - In production, the App Service connection string overrides the local setting, keeping secrets out of the repository.
