# **Lab 2**

1. Create an ILog instance for a specific type in Program.cs using the LogFactory
2. Log the path to the log file using the LogFactory
3. Parse the first sample file using the XmlParser
4. Log the CoreFilingMessage created in Step 3 (HINT: Let Serilog do the work for you)

### APITrainingUtilities
The solution provided in this repository comes with a few utilites to help you along the way.
* **LogFactory**
    * This class allows you to log to both the console and a file.
    * Built using [Serilog](https://serilog.net/)
    * LogFactory.CreateForType<T>() and LogFactory.Create() gives you a logger
    * Five levels: Verbose, Debug, Information, Warning, Error, Fatal
    * Able to log objects easily using Serilog _{@Placeholder}_ syntax
    * LogFactory.LogFilePath gives you the path to the log file
    * Example
```csharp
using APITrainingUtilities.Logging;
public class Example
{
    private static readonly ILog _logger = LogFactory.CreateForType<Example>();
    
    public void TestLogging()
    {
        _logger.Information("Hello World!");
        int number = 12;
        _logger.Warning("Number = {Number}", number);
        var fruit = new [] { "Apple", "Bannana", "Pear" };
        _logger.Debug("Fruit: {@Fruit}", fruit);
        var myObject = new { First = "John", Last = "Doe" };
        _logger.Debug("Name: {@MyObject}", myObject);
    }
}
```
#### Console Output:
```cmd
[10:37:58.3548<Example>INF]Hello World!
[10:37:58.3848<Example>WRN]Number = 12
[10:37:58.3898<Example>DBG]Fruit: ["Apple", "Bannana", "Pear"]
[10:37:58.3978<Example>DBG]Name: {First="John", Last="Doe"}
```
* **XmlParser**
    * Converts AgencyX XML file into C# objects
    * XmlParser.CreateMessage(filepath)
    * Object structure mimics XML XPath.  _For example CoreFilingMessage/Case/CaseDocketID would be CoreFilingMessage.Case.CaseDocketID_
    * Example
```csharp
using System.Linq;
public class Example
{
    public void ParseFile()
    {
        CoreFilingMessage message = XmlParser.CreateMessage("some/path/file.xml");
        CaseParticipant defendant = message.Case.CaseParticipants
            .SingleOrDefault(p => p.CaseParticipantRoleCode.Equals("DEF"));
    }
}
```
![Diagram](https://journaltech-my.sharepoint.com/personal/rhartley_journaltech_com/_layouts/15/guestaccess.aspx?docid=0a0f8d334f22942438384317240122f14&authkey=AfCNrhb6_ZSj1gfO7dPBHEE)