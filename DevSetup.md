### Server-Side API Development Setup
- Download DataAccess project from https://github.com/sogatechgit/DataAccess 
<p align="center">
<img width="700" src="https://github.com/sogatechgit/GitAssets/blob/master/DevServerAPISetup/download_DataAccess.png" />
  </p>

- Download NgArbi project from https://github.com/sogatechgit/NgArbiServer 
<p align="center">
<img width="700" src="https://github.com/sogatechgit/GitAssets/blob/master/DevServerAPISetup/download_NgArbi.png" />
  </p>

- Extract both package into the target solution folder
<p align="center">
<img width="700" src="https://github.com/sogatechgit/GitAssets/blob/master/DevServerAPISetup/extract_DataAccess.png" />
<img width="700" src="https://github.com/sogatechgit/GitAssets/blob/master/DevServerAPISetup/extract_NgArbiServer.png" />
</p>

- Extract IMSA_DATA.mdb
<p align="center">
<img width="700" src="https://github.com/sogatechgit/GitAssets/blob/master/DevServerAPISetup/extract_imsa_Data.png" />
</p>

- Rename both project folders and get rid of the "- master" suffix
<p align="center">
<img width="700" src="https://github.com/sogatechgit/GitAssets/blob/master/DevServerAPISetup/rename_folders.png" />
</p>

- Copy NgArbiSLN.zip from NgArbiServer project folder to the solution folder and extract the files into the solution folder
<p align="center">
<img width="700" src="https://github.com/sogatechgit/GitAssets/blob/master/DevServerAPISetup/copy_NgArbiSLN.png" />
  
<img width="700" src="https://github.com/sogatechgit/GitAssets/blob/master/DevServerAPISetup/extract_NgArbiSLN.png" />
</p>

- Open NgArbi.sln file
<p align="center">
<img width="700" src="https://github.com/sogatechgit/GitAssets/blob/master/DevServerAPISetup/open_NgArbiSLN.png" />
</p>

- Rebuild solution to re-install all dependencies
<p align="center">
<img width="700" src="https://github.com/sogatechgit/GitAssets/blob/master/DevServerAPISetup/rebuild_solution.png" />
</p>

- Create an IIS application which points to NgArbiServer project folder
<p align="center">
<img width="700" src="https://github.com/sogatechgit/GitAssets/blob/master/DevServerAPISetup/create_iis_application.png" />
</p>

- Download and install postman application from https://postman.com/downloads/
<p align="center">
<img width="700" src="https://github.com/sogatechgit/GitAssets/blob/master/DevServerAPISetup/download_and_install_postman.png" />
</p>

- Test API GET Request using Postman. Send http://localhost/NgArbi/api/app/@gents request to initialize the API Tables. The same output as shown in the image below must reflect in the body panel of Postman as the response from the server. Note that there are 15 tables currently configured in the API as indicated in the response
```json
[
    {
        "returnDescription": null,
        "returnType": "table",
        "subsKey": "Hello Test Me!",
        "returnCode": "",
        "requestDateTime": "2020-06-05T16:56:51",
        "requestDuration": 0,
        "recordCount": 0,
        "records": [],
        "returnDataParams": null,
        "globalMesages": null,
        "recordsProps": null,
        "returnStrings": [],
        "props": {
            "Tables": 15,
            "Views": 0,
            "StoredProcedures": 0
        },
        "input": {}
     }
]
```
<p align="center">
<img width="850" src="https://github.com/sogatechgit/GitAssets/blob/master/DevServerAPISetup/test_with_postman_@gents.png" />
</p>

- Test retrieving data from a configured table. (Say...from anomalies where the table code assigned is 'an'. Send a GET Request with url set to http://localhost/NgArbi/api/app/an/1,2,3 . This API call's response must reflect that there are 3 records extracted as shown in the image below.
<p align="center">
<img width="700" src="https://github.com/sogatechgit/GitAssets/blob/master/DevServerAPISetup/test_with_postman.png" />
</p>

### Switching Database Provider
In DALData Class:
```c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public static class DALData
    {

        private static DALDataOleDb _DataOleDb = null;
        public static DALDataOleDb DataOleDb
        {
            get
            {
                if (_DataOleDb == null) _DataOleDb = new DALDataOleDb();
                return _DataOleDb;
            }
        }

        private static DALDataOracle _DataOracle = null;
        public static DALDataOracle DataOracle
        {
            get
            {
                if (_DataOracle == null) _DataOracle = new DALDataOracle();
                return _DataOracle;
            }
        }


        private static DALDataMSSQL _DataMSSQL = null;
        public static DALDataMSSQL DataMSSQL
        {
            get
            {
                if (_DataMSSQL == null) _DataMSSQL = new DALDataMSSQL();
                return _DataMSSQL;
            }
        }

        // Switch Data Provider by assigning the correct Provider class to DAL property

        // To use OleDb, name the OleDbProvider property as DAL and 
        //   rename the other data provider properties as something else
        // OleDbProvider Property
        public static DALDataOleDb DAL
        {
            get { return DataOleDb; }
        }

        // To use Oracle, name the OracleProvider property as DAL
        //   rename the other data provider properties as something else
        // OracleProvider Property
        public static DALDataOracle DALOracle
        {
            get { return DataOracle; }
        }

        // To use MS SQL, name the MSSQLProvider property as DAL
        //   rename the other data provider properties as something else
        // MSSQLProvider Property
        public static DALDataMSSQL DALMSSQL
        {
            get { return DataMSSQL; }
        }
    }
}

```
- Rename the current DAL Property to its original name<br/>
  ie.<br/>
  rename current DAL from...
  ```c#
      // current DAL property using DALDataOleDb class
        public static DALDataOleDb DAL
        {
            get { return DataOleDb; }
        }
  ```
  to...
  ```c#
      // current DAL property using DALDataOleDb class
        public static DALDataOleDb DALOleDb
        {
            get { return DataOleDb; }
        }
  ```
- Rename the target DAL Property and rename it to 'DAL'<br/>
  ie.<br/>
  rename target DAL from...
  ```c#
      // target DAL property using DALOracle class
        public static DALDataOracle DALOracle
        {
            get { return DataOracle; }
        }
  ```
  to...
  ```c#
      // target DAL property using DALOracle class
        public static DALDataOracle DAL
        {
            get { return DataOracle; }
        }
  ```

### Oracle Datbase Provider Class
Write implementation codes for DALDataOracle class in DALDataOracle.cs file
```c#
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace DataAccess
{
    public class DALDataOracle : DALDataBase
    {
        public override List<ReturnObject> Excute(List<CommandParam> commandParams,bool commit = false)
        {
            return null;
        }
        public override ReturnObject Excute(CommandParam cmdParam, dynamic cmdConnectDynamic = null, dynamic cmdTransactDynamic = null)
        {
            return null;
        }

        public override Int64 GetScalar(string cmdText)
        {
            return -1;
        }
        public override JArray GetJSONArray(CommandParam cmdParam)
        {
            return null;
        }

        public override List<Dictionary<string, dynamic>> GetDictionaryArray(CommandParam cmdParam)
        {
            return null;
        }

        public override dynamic GetDataReaderCommand(CommandParam cmdParam)
        {
            return null;
        }

        public override ReturnObject GetRecordset(CommandParam cmdParam, bool returnFields = false, bool withFields = false,
            long pageNumber = 0, long pageSize = 0)
        {
            return null;
        }

        public override List<Dictionary<string, dynamic>> DALReaderToDictionary(dynamic rdr)
        {
            return null;
        }
        public override JArray DALReaderToJSON(dynamic rdr)
        {
            return null;
        }
        public override List<List<object>> DALReaderToList(dynamic rdr, long pageNumber = 0, long pageSize = 0)
        {
            return null;
        }

        class DALOracleConnection
        {
            // oledb connection classe
            const string VER = "Oracle";
            public DALOracleConnection()
            {

            }

        }

    }
}

```
### MS SQL Datbase Provider Class

