# Code Test
#### Prerequisite:
###### Please make sure that you have .NET 5.0 installed on your machine. The .NET 5.0 SDK can be found here: 
###### https://dotnet.microsoft.com/download/dotnet/5.0

### Building and Running:

1. Download the code to your machine and open it in **Visual Studio** If necessary, set the debugging profile as **Development**.
2. Open up **appsettings.json** in the **Core.Api project**. In the "Initialization" section, look for "CoreConnectionString". This is the connection string to use to create a database. Modify this as necessary.
3. Open up **appsettings.test.json** in the **Core.Tests** project. In the "Initialization" section, look for "CoreConnectionString". Make sure that the connection string here is the same as the one in the previous step.
4. Build and run the solution (If necessary, please set **Core.Api** as the startup project). Visual Studio should automatically take care of restoring the packages. The database should also be created with seeded values. 

    **Note**:  It's also possible to use Sqlite instead of SQL Server as a database. To do so, simply edit **SqliteCoreConnectionString** on both JSON files in steps 2 and 3, and **in the Startup.cs in Core.Api**, in the **ConfigureServices** method, comment out this line of code:
    > .AddDbContext<CoreContext>(options => options.UseSqlServer(defaultConfigSection["CoreConnectionString"]))

    and uncomment this line of code:
    > //.AddDbContext<CoreContext>(options => options.UseSqlite(defaultConfigSection["SqliteCoreConnectionString"]))

### Walkthrough:
### Security
Security is a feature of the web API. The account details endpoint required for the test can be found in http://localhost:8090/api/accountdetails. However, this endpoint (as well as many others in the API) are secured. To successfully access the endpoint, we need to be authenticated and authorized. 

**Note**:  If you were to try to access the endpoint right now as it is with a browser or a REST client, you will get a response with the message **"Invalid Authorization header or NOnce header"**, an **exception in Visual Studio** with the exact same message, and a new record in the **Messages** table in the database with details of the error, like the stack trace.

1. Before being able to access any endpoint, we must first be authenticated. Using **Postman** (or any other REST API client such as **Telerik Fiddler**), set the URL field's value to **http://localhost:8090/api/user/authorize** and set the action to be a **POST request**. We must include a **Credentials** request header with this request; the value of this header is in the form:
    >base64(appid:&lt;appid&gt;\nusername:&lt;username&gt;\npassword:&lt;password&gt;)

    That is, we have to append the **appid**, **username**, and **password** labels with their values by a colon, with each key-value pair having a newline between them, and converted to their base64 format. Using our default values in our seeds in the database without converting it to its base64 format, this will turn out to be:
    >appid:defaultapp\nusername:superuser\npassword:reallyBadHardcodedPassword
    
    If we convert this to base64, it would be:
    >YXBwaWQ6ZGVmYXVsdGFwcAp1c2VybmFtZTpzdXBlcnVzZXIKcGFzc3dvcmQ6cmVhbGx5QmFkSGFyZGNvZGVkUGFzc3dvcmQ=
    
    We can easily verify this by going to https://base64decode.org and pasting in that value in the textbox, and clicking on the Decode button. 
    
    **Note:** While it is not ideal to hardcode credentials into the database and submit them to an endpoint without using TLS, I decided to do this for ease of demonstration. In the real world, we would not submit any data to an unsecured endpoint nor hardcode credentials.
    
    **Copy and paste the above encoded value as the value of the Credentials header, and submit a POST request to http://localhost:8090/api/user/authorize**, You should have received a **200 OK** response. Open the response and check the headers in your REST client, and you will find an **Authorization** header and an **NOnce** header with their values. These two values are what allow us to make authorized requests to the API's endpoints.
    
2. Using your REST API client, set the URL field's value to **http://localhost:8090/api/accountdetails** and set the action to be a **GET request**. Next, add the headers (**Authorization** and **NOnce**) and their values which we retrieved from step 1 to the GET request. Finally, submit the request. You should receive a **200 OK** response with the **account details data**, including **id**, **status**, **isClosed**, **reasonForClosing**, **accountBalance**, etc, but  transactions data is null. In order to view the transactions of each account, we must adjust the URL's value by **appending the id of the account to the URL**. That is, if the id is "1234567889", then the url would be:

    >http://localhost:8090/api/accountdetails/1234567889
    
    But before you submit this request, we must first change the value of the NOnce again (leave the Authorization value unchanged). Along with  every authorized response is a new **NOnce** value for the purpose of making subsequent requests which helps prevent replay attacks; this is necessary (NOnce stands for "**N**umeric **Once**" after all). **Get the value of the new NOnce from the response header, replace the NOnce of the next request header, and submit the new GET request.** If you appended the id of the account Id, you should now have the transactions data available. Please take note that payments are ordered newest date first, which is a requirement of the test.
    
    **Note:** The web API has **OData capabilities** which allow us to select certain fields and perform some limited queries which are useful for limiting the response to the fieleds that we are only interested in. For example, if we would like to only see the **id**, **account balance** and **transactions*** in the previous request, we would append to the query parameter **"$select=accountbalance,status&$expand=transactions"**. So we should have something similar to **"http://localhost:8090/api/accountdetails/1234567889?$select=accountbalance,status&$expand=transactions"** and we would only retrieve the id, account balance, and transactions aka payments.

### Solution structure

The entire solution is divided into three projects: Core.Api, Core.Data, and Core.Tests.

- **Core.Data** is mainly responsible for providing database connectivity and operations, handling credentials information and validation. The **Attributes** folder contains the **RequiresPermissionAttribute** class which is responsible for checking the user's authentication and authorization. The **Contexts** folder contains the **CoreContext** class which provides the **abstraction models of the database as well as seed data for the database.** The **Contracts** folder contains two interfaces, **IContext**, which is used for seeding and checking if the database exists, and **IDataAccess**, which provides method signatures for interfacing with the database and most common database operations. The **Handlers** folder contains files used to handle various information such as **security** and **input**. The **Models** folder contains the **entities used as an abstract representation of the database structure.** Finally, the **Repositories** folder contains the **data access file** which provides methods for data access.
- **Core.Api** is the Web API. It exposes endpoints to allow clients to connect through these endpoints in a secure way. The **Controllers** folder contains the various classes which **expose endpoints which allow connectivity to an Http client.** The **Middleware** folder contains the **ExceptionLoggerMiddleware**, which is crucial for **catching errors and logging them to the database with relevant information like the stack trace.** The **Models** folder contains the Settings classes used by the web API. 
- **Core.Tests** is the project for unit testing and integrated / functional testing. The **API** folder contains the tests for **Core.Api**. The **Data** folder contains the tests for **Core.Data**, and the **Helpers** folder contains the set of files which provide useful methods for testing purposes.
        
**Note:** Due to time constraints, only a subset of tests were created. The created tests are **succeeding tests** and only on pertinent objects such as the **exposed API endpoints and a subset of functions from the SecurityHandler class**

### Miscellaneous:
- The stated user story which describes the fields of the payment list (highlighted in **bold**) is confusing : 
    >   "Fields: Account balance and for the payment list: Date, Amount, **status Closed reason if it exists**" 

    I interpreted it to mean that each Account has an Account Balance and a Status field (which has at least one valid value, "closed"), and Reason for closing the account (I have never encountered a payment which I have made which had its status described as "closed" and a reason why it was closed before). In the payment list, the fields are: Date (which is the Payment Date) and Amount. To help facilitate whether or not an account is closed, I created another field, "IsClosed", so as to be certain that it is closed because I do not know what the valid values are for the "Status" field.

- To integrate CI / CD (Continuous Integration / Continuous Deployment) using Windows Azure involves linking the project to Azure Devops and creating a CI/CD pipeline. This involves **Azure service** to deploy the application and selecting **Windows Web App** as the type of project to deploy. I would configure "nightly builds" by configuring scheduled triggers with the **cron schedule defined as:** 
    >0 0 * * * 

    which means that the trigger is set to run at midnight. I would also configure the pipeline to run ALL test projects (**unit tests, integration tests, fully-automated UI testing, as well as any other automated tests**) as well to ensure that the code is stable everytime developers report to work.
- In the Core.Api project is a **launchSettings.json file**. This file contains 4 profiles that match to common environments: **Development, QA, Staging**, and **Production**. The environment variable used to detect the environment is **ASPNETCORE_ENVIRONMENT**. This helps in detecting which particular settings to use based on the environment.
- The dates of the entities are all saved in **UTC Time**. This is important to do because we want to avoid saving in local time and struggle with time zone differences and daylight savings. By saving the dates in UTC time, all that the client calling our API have to do is convert it into local time. In C#, this is accomplished by using **DateTime.ToLocalTime**.