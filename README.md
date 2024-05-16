# Currency Converter API by Darab Javaid (darabjavaid@gmail.com)

## Requirements

- .NET Core SDK

## Setup

**I've created the application on visual studio 2022 (.NET 8)**

1. Clone the repository
2. Navigate to the project directory
3. Run the application using `dotnet run` or open it in Visual Studio 2022
4. The APIs will be available at swagger index `https://localhost:7299/swagger/index.html` or you can use below endpoints to run.
5. Tests are created in a separate project. 
6. Run the test by navigating to the project directory and using `dotnet test` or Open the Test Explorer in VS 2022, and CLick on `Run All`

## Endpoints

1. **Retrieve the latest exchange rates**: `GET /api/currency/latest?baseCurrency=EUR`
2. **Convert currency amounts**: `GET /api/currency/convert?baseCurrency=EUR&amount=100&targetCurrency=USD`
3. **Get historical rates of base currency by pagination**: `GET /api/currency/historical?baseCurrency=IDR&startDate=2020-01-01&endDate=2020-01-29&pageNumber=1&pageSize=4`

## FOR DEMO PURPOSE, I've deployed the APIs to AZURE PORTAL, use below APIs endpoints to test

1. **Retrieve the latest exchange rates**: `GET` https://currencycalculatorbydarabjavaid.azurewebsites.net/api/currency/latest?baseCurrency=EUR
2. **Convert currency amounts**:  `GET`  https://currencycalculatorbydarabjavaid.azurewebsites.net/api/currency/convert?baseCurrency=EUR&amount=100&targetCurrency=USD
3. **Get historical rates of base currency by pagination**: `GET`  https://currencycalculatorbydarabjavaid.azurewebsites.net/api/currency/historical?baseCurrency=IDR&startDate=2020-01-01&endDate=2020-01-29&pageNumber=1&pageSize=4


## Assumptions

- The Frankfurter API may not respond to the first request, but the service handles retries.
- The application excludes certain currencies (TRY, PLN, THB, and MXN) for the conversion endpoint. (either base or target.)

## Enhancements

- Add caching to reduce the number of requests to the Frankfurter API.
- Implement rate limiting to handle large numbers of requests efficiently.
- Add more comprehensive error handling and logging.
- API Documentation (implemented with swagger).
