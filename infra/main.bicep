targetScope = 'resourceGroup'

@description('Workload short name, e.g. mealcycle')
param workloadName string = 'mealcycle'

@description('Environment name: dev | staging | production')
@allowed([
  'dev'
  'staging'
  'production'
])
param environmentName string = 'dev'

@description('Azure region for deployed resources')
param location string = resourceGroup().location

@description('Storage account SKU')
param storageSkuName string = 'Standard_LRS'

@description('Deploy App Service resources for the backend API')
param deployWebApp bool = true

@description('App Service Plan SKU for the backend API')
param appServicePlanSkuName string = 'F1'

@description('Table names for application persistence')
param recipesTableName string = 'recipes'
param mealPlanTableName string = 'mealplanitems'
param cookProgressTableName string = 'cookprogress'

var storageAccountName = toLower('${workloadName}${environmentName}st')
var appServicePlanName = '${workloadName}-${environmentName}-api-plan'
var webAppName = toLower('${workloadName}-${environmentName}-api')

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: storageAccountName
  location: location
  kind: 'StorageV2'
  sku: {
    name: storageSkuName
  }
  properties: {
    allowBlobPublicAccess: false
    supportsHttpsTrafficOnly: true
    minimumTlsVersion: 'TLS1_2'
  }
}

resource tableService 'Microsoft.Storage/storageAccounts/tableServices@2023-05-01' = {
  parent: storageAccount
  name: 'default'
}

resource recipesTable 'Microsoft.Storage/storageAccounts/tableServices/tables@2023-05-01' = {
  parent: tableService
  name: recipesTableName
}

resource mealPlanTable 'Microsoft.Storage/storageAccounts/tableServices/tables@2023-05-01' = {
  parent: tableService
  name: mealPlanTableName
}

resource cookProgressTable 'Microsoft.Storage/storageAccounts/tableServices/tables@2023-05-01' = {
  parent: tableService
  name: cookProgressTableName
}

resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = if (deployWebApp) {
  name: appServicePlanName
  location: location
  sku: {
    name: appServicePlanSkuName
    capacity: 1
  }
  kind: 'app'
  properties: {
    reserved: false
  }
}

resource webApp 'Microsoft.Web/sites@2023-12-01' = if (deployWebApp) {
  name: webAppName
  location: location
  kind: 'app'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      appSettings: [
        {
          name: 'AzureStorage__TableServiceUri'
          value: 'https://${storageAccount.name}.table.${environment().suffixes.storage}/'
        }
        {
          name: 'AzureStorage__RecipesTableName'
          value: recipesTable.name
        }
        {
          name: 'AzureStorage__MealPlanTableName'
          value: mealPlanTable.name
        }
        {
          name: 'AzureStorage__CookProgressTableName'
          value: cookProgressTable.name
        }
      ]
    }
  }
}

// Foundry resources are intentionally not created yet in this prep stage.
// We will confirm model/deployment strategy before adding AI resource modules.

output storageAccountName string = storageAccount.name
output storageTableEndpoint string = 'https://${storageAccount.name}.table.${environment().suffixes.storage}/'
output recipesTableOutput string = recipesTable.name
output mealPlanTableOutput string = mealPlanTable.name
output cookProgressTableOutput string = cookProgressTable.name
output webAppNameOutput string = deployWebApp ? webApp.name : ''
