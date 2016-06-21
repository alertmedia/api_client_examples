***AlertMediaSolutions/ClientLibrary***

This package contains all basic level APIs for user & group management.

Steps to use this library inside your application

1.  Initialize the library as given below

     `AlertMediaClient.setBaseUrl()` (i.e.
[*https://dashboard.alertmedia.com/api*](https://dashboard.alertmedia.com/api))

     `AlertMediaClient.setClientId()` 

     `AlertMediaClient.setClientKey()`

You should be able to generate Client Id & Client Secret using your
account. If you need any further assistance, contact your AlertMedia
customer success representative.

2.  APIs available for user management 

     `AlertMediaClient.User.get()` // Get a specific user details

     `AlertMediaClient.User.list()` // Get all user details in your
account

     `AlertMediaClient.User.create()` // Create a new user account

     `AlertMediaClient.User.update()` // Update an existing user account

     `AlertMediaClient.User.delete()` // Delete an existing user account

Implementation of above API calls is in file
`AlertMediaSolutions/ClientLibrary/UserClient.cs`

3.  APIs available for group management

     `AlertMediaClient.Group.get()` // Get a specific group details

     `AlertMediaClient.Group.list()` // Get all group details

     `AlertMediaClient.Group.Create()` // Create new group

     `AlertMediaClient.Group.update()` // Update an existing group

     `AlertMediaClient.Group.delete()` // Delete an existing group

     `AlertMediaClient.Group.listGroupUsers()` // Get all users belonging
to list of groups

     `AlertMediaClient.Group.addUsersToGroup()` // Add user to an existing
group

     `AlertMediaClient.Group.deleteUsersFromGroup()` // Delete users from
an existing group

Implementation of above API calls is in file
`AlertMediaSolutions/ClientLibrary/GroupClient.cs`

***AlertMediaSolutions/ADLibrary***

ActiveDirectoryClient file will help in importing users from
ActiveDirectory to AlertMedia. All configuration data required for this
library are stored main applications App.Config file. 

***App.Config (Located in ADSyncClient & ADSyncConsole)***

The config file has 4 main mappings:

1.  AMSettings - All configuration related to AlertMedia

2.  ADSettings - All configuration details related to ActiveDirectory

3.  FieldMappings - Mapping of user fields in ActiveDirectory
    to AlertMedia. This is needed to make sure the fields in AD are
    mapped to the corresponding field in AM User object correctly.

4.  GroupMappings - Mapping of groups in ActiveDirectory to AlertMedia.
    Groups can further be mapped in 2 different ways as explained below

**AMSettings**

     

Configuration fields of AMSettings are

1.  BaseUrl - AlertMedia server URL. It will always be
    https://dashboard.alertmedia.com/api

2.  ClientID - Client ID for API integration. Please go through the help
    documents to generate this file for your account

3.  ClientSecret - Client Secret Key for API integration. Please go
    through the help documents to generate this file for your account

4.  Customer - This key is for internal purposes. Customers are
    requested not to modify value of this key.

5.  GroupMappingType - ActiveDirectory groups can be mapped to
    corresponding AlertMedia groups in 2 ways. Possible values for this
    field are:
  1.  OU

  2.  FieldValue - more explanation is given below in
      GroupMapping section.

6.  ADUserFieldForGroupMapping - This field is required if the option of
    GroupMappingType is **FieldValue**. More explanation is given below
    in GroupMapping section option 2.

**ADSettings **

Configuration fields of ADSettings are

1.  LdapUrl - Customer’s internal LDAP Server URL. This should begin
    with LDAP://

2.  Username - LDAP Admin user name that will help in fetching
    all users. (i.e. administrator@domainname.local)

3.  Password - Password of LDAP admin user

**FieldMappings **

In this section each of the corresponding AlertMedia user fields are
mapped to an ActiveDirectory user object field.

Please do not delete any the rows in this section nor modify any of the
keys.

For a specific user field in AlertMedia, if there is no corresponding
ActiveDirectory user field, then let both key & value have the same
text.

**GroupMappings **

There are 2 ways AlertMedia Group can be mapped to an ActiveDirectory
group

*Option 1: Using ActiveDirectory’s OU approach *

Each row in this section will look as described below -

`<add key=“{{ alertmedia\_group\_id\~alertmedia\_group\_name }}" value=“ {{ active\_directory\_u\_name }}">`

Example: `<add key=“12\~New Group” value=“OU-NewGroup”>`

In the above example:

-   12 is AlertMedia Group ID

-   New Group is AlertMedia group name

-   OU-NewGroup is ActiveDirectory group name

*Option 2: Based on ActiveDirectory user field values *

In this approach, values of a specific user field in ActiveDirectory
determines the actual group the user belongs to in AlertMedia. The AD
user field name is configured in ADUserFieldForGroupMapping.

Each row in this section will look as described below:

-   `<add key=“{{ alertmedia\_group\_id\~alertmedia\_group\_name }}" value=“ {{ field\_value }}">`

Example:

-   ADUserFieldForGroupMapping value in AMSettings will be “location”

-   Each row in GroupMapping will look as described below:

    -   `<add key=“{{ 12\~Austin Group }}" value=“ {{ austin }}">`

    -   `<add key=“{{ 13\~Seattle Group }}" value=“ {{ seattle }}">`

***AlertMediaSolutions/ADSyncConsole***

This console application can be used to import users from AD to
AlertMedia once the configuration file is manually created.

***AlertMediaSolutions/ADSyncClient***

This UI application serves 2 purposes

1.  Using a desktop client application, users will be able to populate
    the configuration file

2.  Users will be able to preview all ActiveDirectory users before
    importing them to AlertMedia
