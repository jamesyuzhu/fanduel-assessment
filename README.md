# DepthChart API Documentation

---

## Build and Launch the API

### Prerequisites
1. **Target Framework:** .NET 6.0  
2. **Microsoft Visual Studio 2022:** Version 17.12.1 (Community Edition recommended)

### Build and Run
1. Unzip the provided file.
2. Navigate to the `fanduel-assessment` folder.
3. Open `DepthChart.Api.sln` using Visual Studio 2022.
4. Build the solution (the first build might take some time as it will download required NuGet packages).
5. Click the "IIS Express" button at the top of Visual Studio to launch the DepthChart API locally.

> **Note:**  
> The source code has been committed to a private GitHub repository. Contact me with your email address to request access and clone the repository.

---

## Testing the DepthChart API

### Endpoints Overview

The DepthChart API provides the following endpoints:
1. **AddPlayerToDepthChart**
2. **RemovePlayerFromDepthChart**
3. **GetBackUps**
4. **GetFullDepthChart**

The repository layer uses Entity Framework with an in-memory database. Testing data will exist in memory and be cleared when the API is stopped.

---

### Testing in a Web Browser

1. After clicking the "IIS Express" button in Visual Studio, a Swagger UI will open in your default browser at:  
   `https://localhost:44397/swagger`
2. The API will start running at:  
   `https://localhost:44397/api/depthchart/`
3. On the Swagger page, you can test each endpoint:
   - Expand an endpoint and click the "Try it out" button.
   - Provide values for the parameters (details below).
   - Fill the "Request body" as required.
   - Click "Execute" to view the response status and data.

Alternatively, you can test the endpoints using Postman with the provided URLs and parameter instructions.

---

### Endpoint Details

#### **AddPlayerToDepthChart**
- **URL:**  
  `https://localhost:44397/api/depthchart/{sportCode}/{teamCode}`
- **Method:** `POST`
- **Path Parameters:**  
  - `sportCode` (e.g., NFL): Specifies the sport type.  
  - `teamCode` (e.g., TampaBayBuccaneers): Specifies the team.
- **Query Parameter:**  
  - `chartDate` (optional): Date of the DepthChart (format: `yyyy-MM-dd`). Defaults to the first day of the current week (UTC).
- **Request Body:**  
```javascript
{
	"positionCode": "LT", // Mandatory
	"playerId": 1,        // Mandatory
	"playerName": "Susan Joe", // Mandatory
	"depth": 1            // Optional
}
```

---

#### RemovePlayerFromDepthChart

- **URL:**  
  `https://localhost:44397/api/depthchart/{sportCode}/{teamCode}`  
- **HTTP Method:** `DELETE`  
- **Path Parameters:**  
  Same as the path parameters for `AddPlayerToDepthChart`.  
- **Query Parameters:**  
  - `positionCode`: The position code (mandatory).  
  - `playerId`: The ID of the player to remove (mandatory).  
  - `chartDate`: Optional. If unspecified, default behavior is the same as in `AddPlayerToDepthChart`.  
- **Example URLs:**  
  - `https://localhost:44397/api/depthchart/NFL/TampaBayBuccaneers?positionCode=LT&playerId=1`  
  - `https://localhost:44397/api/depthchart/NFL/TampaBayBuccaneers?positionCode=LT&playerId=1&chartDate=2024-09-17`

---

#### GetBackUps

- **URL:**  
  `https://localhost:44397/api/depthchart/backups/{sportCode}/{teamCode}`  
- **HTTP Method:** `GET`  
- **Path Parameters:**  
  Same as the path parameters for `AddPlayerToDepthChart`.  
- **Query Parameters:**  
  - `positionCode`: The position code (mandatory).  
  - `playerId`: The ID of the player to retrieve backups for (mandatory).  
  - `chartDate`: Optional. Defaults as described in `AddPlayerToDepthChart`.  
- **Example URLs:**  
  - `https://localhost:44397/api/depthchart/backups/NFL/TampaBayBuccaneers?positionCode=LT&playerId=1`  
  - `https://localhost:44397/api/depthchart/backups/NFL/TampaBayBuccaneers?positionCode=LT&playerId=1&chartDate=2024-09-17`

---

#### GetFullDepthChart

- **URL:**  
  `https://localhost:44397/api/depthchart/full/{sportCode}/{teamCode}`  
- **HTTP Method:** `GET`  
- **Path Parameters:**  
  Same as the path parameters for `AddPlayerToDepthChart`.  
- **Query Parameters:**  
  - `chartDate`: Optional. Defaults as described in `AddPlayerToDepthChart`.  
- **Example URLs:**  
  - `https://localhost:44397/api/depthchart/full/NFL/TampaBayBuccaneers?chartDate=2024-09-17`  

---

## Integration Testing

The project `DepthChart.Api.IntegrationTests` includes various end-to-end test cases for each endpoint.  

1. Build the `DepthChart.Api.IntegrationTests` project in Visual Studio.  
2. In the **Test Explorer** tab at the bottom of Visual Studio:  
   - Right-click on `DepthChart.Api.IntegrationTests`.  
   - Select "Run" to execute all integration tests.

---

## Unit Testing

The project `DepthChart.Api.UnitTests` is designed to test the most critical functions.  

1. Build the `DepthChart.Api.UnitTests` project in Visual Studio.  
2. In the **Test Explorer** tab at the bottom of Visual Studio:  
   - Right-click on `DepthChart.Api.UnitTests`.  
   - Select "Run" to execute all unit tests.

---

## Assumptions

1. **NFL DepthChart is weekly-based.**  
   - Default `ChartDate`: The start day of the week unless otherwise specified.  
2. **UTC-based `ChartDate`:**  
   - Easier backend maintenance. However, UFL may require a US-based time zone.  
3. **DepthChart Identification Key:**  
   - `SportCode + TeamCode + ChartDate`.  
4. **Shared Business Rules for NFL Teams:**  
   - `NFLDepthChartService` covers shared rules across teams.  
   - Currently supports `TampaBayBuccaneers`. Add more teams to `TeamCodes` in `NFLDepthChartService.cs` as needed.  
   - For unique rules, create a new service implementing `IDepthChartService`. 
5. **A new sport with unique rules (like NBA):**   
	- For a new sport, create a new service implementing `IDepthChartService`.   
6. **Endpoint Specifics:**  
   - `RemovePlayerFromDepthChart`: Returns a player object or empty response.  
   - `GetFullDepthChart`: Returns a list of `PositionDepth` JSON objects, which can be transformed as needed.  
7. **Depth Management:**  
   - Adding a player at an out-of-bounds depth adjusts to the next available depth.  
   - Subsequent records update automatically on addition/removal.

> **Note:**  
> In this solution, only sport NFL and team TampaBayBuccaneers is implemented. For testing purpose, please always use "NFL" as sport code and "TampaBayBuccaneers" as team code.

---

## Validation Rules

- All mandatory fields must be provided.  
- `PlayerId` must be greater than 1.  
- `Depth` must be greater than 1.

---

## Further Considerations

### Database options

This solution utilizes Entity Framework's DbContext to store data in an in-memory database, chosen for ease of demonstration. While DbContext serves as an abstraction layer for relational databases, it is not well-suited for NoSQL databases. To achieve loose coupling between the service layer and the database layer, a repository layer has been implemented. This design enables seamless switching to other databases, whether relational or NoSQL.

---

### Security

For simplicity, no authentication is implemented. Consider adding:  
- API Key  
- IAM Authentication  
- Cognito User Pools  
- Lambda Authorizer  
- OpenID Connect  
- Mutual TLS  

---

### Logging

For better log management, integrate with tools like:  
- **Splunk**  
- **Datadog**  

---
