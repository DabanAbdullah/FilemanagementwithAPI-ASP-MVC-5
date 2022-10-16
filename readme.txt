first run the dbinitialize sqlserver.sql in sql server database query to initialize database or just backup db.bak in sql server 2014
and change connection string value in webconfig file in the project 

then open project in vs 2017 or later

right click on project solution in solution explorer
restore Nuget Packages

and run this in Package manager console
Update-Package Microsoft.CodeDom.Providers.DotNetCompilerPlatform -r


this should be all

