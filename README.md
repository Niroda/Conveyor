# Conveyor

### What is Conveyor?
Conveyor simplifies data handling by allowing you to serialize lambda expressions and send them over HTTP(s). 
This enables direct server-side data filtering using Entity Framework for database access or LINQ for processing lists from various sources.

### Important Security Notice:
**Please exercise caution:** Using Conveyor means that you’re allowing external commands/queries to be executed on your server. This can introduce risks if not managed correctly.

### Use Cases

Conveyor is specifically designed for scenarios where back-end systems need to communicate or exchange data dynamically. Below are some key use cases where Conveyor can be effectively utilized:

1. #### API to API Communication
    - **Inter-Service Data Filtering:** Allows different APIs within the same ecosystem to request and retrieve data from each other using dynamic lambda expressions. This is particularly useful for microservices architectures where services need to interact and process data based on complex conditions.

2. ### Integrations to API Calls
    - **Enhancing Integration Capabilities:** Facilitates the integration of external systems with your API, allowing these systems to send customized queries that your API can interpret and execute securely. This is ideal for situations where external systems need to fetch specific subsets of data.

3. ### Desktop Application to API
    - **Desktop to Server Data Handling:** Enables desktop applications to perform complex queries on server-side data through your API. This ensures heavy data processing is offloaded to the server, enhancing application performance and reducing bandwidth usage.


### Best Practices and Examples:
Conveyor excels in environments where you control the creation of lambda expressions, using only parameters provided by your data sources. For example:

1. #### Filtering by Name Prefix
    Suppose a user wants to find all locations starting with "abc":
    ```csharp
    var userInput = "abc";
    var lambda = x => x.Name.StartsWith(userInput);
    ```

2. #### Selecting Courses by Codes
    Or maybe a user needs a list of courses based on specific codes:
    ```csharp
    var userInput = new[] { "abc123", "def456", "ghi789" };
    var lambda = x => userInput.Contains(x.Code);
    ```

### Configuration

#### Install the package
The package is available as a NuGet package and can be installed using:
  
`dotnet add package Conveyor.DotNet`  
Or  
`Install-Package Conveyor.DotNet`

#### Setting the Environment Variable
Before running your application, you must configure the `CONVEYOR_SECRET_KEY` environment variable.

#### Requirements for `CONVEYOR_SECRET_KEY`:
- **Minimum Length**: The key must be at least 12 characters long.
- **Character Diversity**: The key must include at least:
  - One uppercase letter (A-Z)
  - One lowercase letter (a-z)
  - One digit (0-9)
  - One symbol (e.g., `@`, `#`, `$`, etc.)

#### How to Set the Environment Variable
The method to set an environment variable depends on your operating system:

1. #### Windows
    1. Open the Start Search, type in "env", and select "Edit the system environment variables" or "Edit environment variables for your account".
    2. Under "Environment Variables", click "New..." to create a new variable.
        - Variable name: `CONVEYOR_SECRET_KEY`
        - Variable value: `<YourSecureKey>`
    3. Click OK and apply the changes.

2. #### macOS/Linux
    1. Open your terminal.
    2. Use the following command to set the variable for your session:
        ```bash
        export CONVEYOR_SECRET_KEY="<YourSecureKey>"
        ```
    3. To make this change permanent, add the export command to your shell's profile script (e.g., `.bashrc`, `.zshrc`).

#### Setting the Environment Variable Programatically (optional):
You can set it programmatically if you want to:   
```csharp
// secretValue could be fetched from a vault or any other location
// This way you can rotate the secret key using a timer or any other approach that suits your needs
Environment.SetEnvironmentVariable("CONVEYOR_SECRET_KEY", secretValue); 
```  
If you prefer to use the programmatic way, don't forget to clean the environment variable on application shutdown:

```csharp
Environment.SetEnvironmentVariable("CONVEYOR_SECRET_KEY", null); 
```

### Usage
Refer to the `Conveyor.Sample.Api` and `Conveyor.Sample.Client` projects to see how to use Conveyor.


### Security and Input Validation:
To ensure the integrity and security of your application, it's crucial to sanitize user inputs. This prevents potential injection attacks and ensures that data is processed correctly. 

### Acknowledgements
I created this library back in 2018 to be used on multiple internal systems at my workplace. Some classes I found somewhere here on GitHub, and I can't really remember the repo name, but big thanks and credits to that repo/person ❤️. I've maintained the code and added the ability to send the serialized JSON over HTTP in both .NET Core and Framework. Initially, I kept it private for use within my workplace, but since I needed it for my private projects, I modified it a bit and made it available to the public.


### Conclusion:
While Conveyor offers powerful capabilities for data processing, it requires careful management to ensure safe operation. Handle it with care to maintain seamless and secure data flows!
