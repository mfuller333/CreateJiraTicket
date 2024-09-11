Certainly! Here's a comprehensive `README.md` file for the JIRA Ticket Creator class:

---

# JIRA Ticket Creator Class

## Overview

The JIRA Ticket Creator Class is a .NET library designed to automate the creation of JIRA tickets and manage issue tracking through SQL Server CLR integration. This class interacts with the JIRA REST API to create tickets for job failures and retrieve user account IDs based on usernames. This automation streamlines incident management and ensures timely issue tracking and resolution.

## Features

- **Create JIRA Tickets**: Automatically create JIRA tickets with custom summaries, descriptions, priorities, and assignees.
- **Add Watchers**: Add watchers to JIRA tickets to keep relevant stakeholders informed.
- **Retrieve Account IDs**: Fetch JIRA account IDs based on usernames for ticket assignment and management.

## Prerequisites

- **.NET Framework**: Compatible with .NET Framework 4.7.2 or later.
- **SQL Server**: Requires SQL Server 2012 or later for CLR integration.
- **JIRA API Token**: Necessary for authentication with JIRA's REST API.

## Installation

1. **Download the .NET Assembly**:
   Download the JIRA Ticket Creator .NET assembly from the [releases page](#) (replace with the actual link if available).

2. **Deploy the Assembly to SQL Server**:
   Use SQL Server Management Studio (SSMS) to deploy the assembly. Run the following SQL script to create the assembly and associated functions/procedures:

   ```sql
   CREATE ASSEMBLY JIRATicketCreator
   FROM 'C:\Path\To\JIRATicketCreator.dll'
   WITH PERMISSION_SET = UNSAFE;
   GO

   CREATE PROCEDURE dbo.CreateJiraTicket
       @jiraUrl NVARCHAR(150),
       @username NVARCHAR(100),
       @password NVARCHAR(500),
       @projectKey NVARCHAR(10),
       @summary NVARCHAR(200),
       @description NVARCHAR(2000),
       @type NVARCHAR(50),
       @priority NVARCHAR(50),
       @watcherAccountId NVARCHAR(100),
       @assigneeAccountId NVARCHAR(100),
       @issueKeyResult NVARCHAR(500) OUTPUT
   AS EXTERNAL NAME JIRATicketCreator.[Namespace.JIRATicketCreator].CreateTicket;
   GO

   CREATE FUNCTION dbo.GetAccountIdByUsername
       (@jiraUrl NVARCHAR(150),
        @username NVARCHAR(100),
        @password NVARCHAR(500))
   RETURNS NVARCHAR(100)
   AS EXTERNAL NAME JIRATicketCreator.[Namespace.JIRATicketCreator].GetAccountIdByUsername;
   GO
   ```

   Replace `[Namespace]` with the actual namespace used in your .NET class.

## Usage

### Creating a JIRA Ticket

To create a JIRA ticket, execute the `CreateJiraTicket` stored procedure from your SQL Server environment. Here’s an example:

```sql
DECLARE @issueKey NVARCHAR(500);

EXEC dbo.CreateJiraTicket
    @jiraUrl = 'https://your-jira-instance.atlassian.net',
    @username = 'your-username',
    @password = 'your-api-token',
    @projectKey = 'PROJ',
    @summary = 'Job Failure Alert',
    @description = 'The job failed due to an unexpected error.',
    @type = 'Task',
    @priority = 'High',
    @watcherAccountId = 'watcher-account-id',
    @assigneeAccountId = 'assignee-account-id',
    @issueKeyResult = @issueKey OUTPUT;

SELECT @issueKey AS IssueKey;
```

### Retrieving an Account ID

To retrieve a JIRA account ID based on a username, execute the `GetAccountIdByUsername` function:

```sql
DECLARE @accountId NVARCHAR(100);

SELECT @accountId = dbo.GetAccountIdByUsername
    (@jiraUrl = 'https://your-jira-instance.atlassian.net',
     @username = 'user-username',
     @password = 'your-api-token');

SELECT @accountId AS AccountId;
```

## Error Handling

- Ensure that the JIRA API URL and credentials are correct.
- Check network connectivity and API permissions if you encounter issues.
- Review error messages in SQL Server logs for debugging information.

## Contribution

Feel free to contribute to this project by submitting issues, feature requests, or pull requests on the [GitHub repository](#) (replace with the actual repository link).

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contact

For any questions or support, please contact [your-email@example.com](mailto:your-email@example.com).

---

This `README.md` provides a detailed guide to setting up, using, and troubleshooting the JIRA Ticket Creator class, making it easier for users to integrate and utilize the functionality.
