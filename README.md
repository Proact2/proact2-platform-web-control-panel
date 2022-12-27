# PROACT 2.0 CPanel
Web Application with asp.net core and MVC

## Configuration

1. Open `appsettings.json`
2. Put your `AzureAdB2C` info and scope (follow *[PROACT 2.0 Azure AD B2C Documentation](https://docs.google.com/document/d/1_49bVIjXpugXBPZOqmD4EyNlIzRrh7FQMA7NftG7ulY/edit?usp=sharing)*)

3. Put your API url addres in 'ProactApiBaseAddress' 

`AppSettings.json` has two separated files `AppSettings.Development.json` and `AppSettings.Production.json`. Be aware to configure in the right way your Environment.

## Deployment

1. Create an App Service resource within your Azure Portal subscription.
2. Publish application with Visual Studio (*[Deploy an ASP.NET web app](https://learn.microsoft.com/en-us/azure/app-service/quickstart-dotnetcore?tabs=net60&pivots=development-environment-vs#publish-your-web-app)*)
