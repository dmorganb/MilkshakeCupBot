namespace MilkshakeCup
{
    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Sheets.v4;
    using Google.Apis.Sheets.v4.Data;
    using Google.Apis.Services;
    using Google.Apis.Util.Store;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;

    public class GSheetsService 
    {
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static string ApplicationName = "MilkshakeBot";
        SheetsService Service = null;
        private static string SpreadsheetId;

        public GSheetsService(string spreadsheetId) 
        {
            SpreadsheetId = spreadsheetId;
        }

        public async Task CreateService() 
        {
            var credential = await GetCredentials();

            // Create Google Sheets API service.
            Service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        // GetSheetAsync(): Will retrieve information from specific sheet
        // [param]sheetName: the name for the specific sheet the user wants from the spreadsheet file
        // [param]startRange: specific cell number where reading will start
        // [param]finishRange: specific cell number where reading will end
        public async Task<List<List<string>>> GetSheetAsync(string sheetName, string startRange, string finishRange) 
        {
            List<List<string>> responseValues = new List<List<string>>();

            string range = "'" + sheetName + "'!" + startRange + ":" + finishRange;
            SpreadsheetsResource.ValuesResource.GetRequest request = Service.Spreadsheets.Values.Get(SpreadsheetId, range);

            ValueRange response = await request.ExecuteAsync();
            IList<IList<Object>> values = response.Values;
            if (values != null && values.Count > 0)
            {
                foreach(var row in values) 
                {
                    var rowToAdd = new List<string>();
                    foreach(var column in row) 
                    {
                        rowToAdd.Add((string) column);
                    }
                    responseValues.Add(rowToAdd);
                }
            }    

            return responseValues;        
        }

        // UpdateSheetAsync(): Will update information for specific sheet
        // [param]sheetName: the name for the specific sheet the user wants from the spreadsheet file
        // [param]startRange: specific cell number where reading will start
        // [param]finishRange: specific cell number where reading will end
        // [param]values: the sheet will be updated with these values
        public async Task<int?> UpdateSheetAsync(string sheetName, string startRange, string finishRange, List<List<string>> values) 
        {
            string range = "'" + sheetName + "'!" + startRange + ":" + finishRange;
            var valueRange = new ValueRange();

            //We need a IList<IList<object>> object to send the data. This loop is to convert [param]values into that object
            IList<IList<object>> valuesObject = new List<IList<object>>();
            foreach (var rowValue in values) {
                var rowObject = new List<object>();
                foreach(var columnValue in rowValue) {
                    rowObject.Add(columnValue);
                }
                valuesObject.Add(rowObject);
            }

            valueRange.Values = valuesObject;

            SpreadsheetsResource.ValuesResource.UpdateRequest request = Service.Spreadsheets.Values.Update(valueRange, SpreadsheetId, range);
            request.ValueInputOption =  SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

            UpdateValuesResponse response = await request.ExecuteAsync();

            return response.UpdatedCells; 
        }

        private async Task<UserCredential> GetCredentials()
        {
            UserCredential credential;

            using (var stream = new FileStream("g_credentials.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "MilkshakeBot",
                    CancellationToken.None,
                    new FileDataStore(".credentials/milkshakecup-bot.json", true));
                Console.WriteLine("Credential file saved to: /.credentials/milkshakecup-bot.json");
            }

            return credential;
        }

    }

}