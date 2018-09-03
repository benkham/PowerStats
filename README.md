# PowerStats
Compute median for data from csv files of a configured directory - .Net Core 2.1, ASP.Net Core Web API, SwaggerUI, Console App Client, Unit Tests - XUnit


## PowerStats.API

Update the DataFilesPath settings value in `appsettings.json` file.

Run the API app from visual studio.

Swagger UI URL `http://localhost:<PORT>` and the api URL `http://localhost:<PORT>/api/powerstatistics`.

Note: Port number is configured in `launchSettings.json` as `http://localhost:62059`


## PowerStats.ConsoleUI

Update `API_HOST` value to `http://localhost:<PORT>` in `PowerStatistics.cs` file.

Run the API before starting a console app instance from visual studio.

ConsoleApp shows the list of statistics.


## PowerStats.UnitTests

Update the DataFilesPath settings value in `PowerStatisticsServiceTests.cs` file.
