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
    using System.Threading;

    public class GSheetsService 
    {
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static string ApplicationName = "MilkshakeBot";
        SheetsService Service = null;

        public void CreateService() 
        {
            UserCredential credential;
            credential = GetCredentials();

            // Create Google Sheets API service.
            Service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        public void GetSheet() 
        {
            // Define request parameters.
            String spreadsheetId = "1wmEGftpCfUU0fyUf0amrV8Zp0GEkR9cIHJAnvKBQRRQ";
            String range = "'Grupo A - Tabla'!A10:I";
            SpreadsheetsResource.ValuesResource.GetRequest request = Service.Spreadsheets.Values.Get(spreadsheetId, range);

            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;
            if (values != null && values.Count > 0)
            {
                Console.WriteLine("Equipo | Partidos");
                foreach (var row in values)
                {
                    // Print columns A and E, which correspond to indices 0 and 4.
                    Console.WriteLine("{0}, {1}", row[0], row[1]);
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }
        }

        private UserCredential GetCredentials()
        {
            UserCredential credential;

            using (var stream = new FileStream("g_credentials.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "MilkshakeBot",
                    CancellationToken.None,
                    new FileDataStore(".credentials/milkshakecup-bot.json", true)).Result;
                Console.WriteLine("Credential file saved to: /.credentials/milkshakecup-bot.json");
            }

            return credential;
        }

    }

}