using System;
using System.Web;
using System.Net;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Configuration;
using System.Linq;
using alertmedia;
using Microsoft.Win32;
using System.Text;
using System.DirectoryServices.AccountManagement;


namespace alertmedia.activedirectory
{
    /**********************************************************************************
 * 
 * This code helps customer ActiveDirectory and their AlertMedia instance in sync
 * 
 * Note: There are few blocks of code that requires customizations. AlertMedia customers
 * are required to verify these blocks and make necessary changes
 * 
 * These blocks of code are identifed by tag "TO BE MODIFIED BY CUSTOMER" in comment section
 * 
 * ************************************************************************/


    public class ActiveDirectoryClient
    {

        /***************************************************************************
             * 		 
             * *********** Alert Media User Fields **************
             * 
             * Note: Please dont modify any of the AlertMedia user field names below
             * 
             ***************************************************************************/

        public static string AM_FIRST_NAME = "first_name";
        public static string AM_LAST_NAME = "last_name";
        public static string AM_EMAIL = "email";
        //public static string AM_DATE_JOINED = "date_joined";
        //public static string AM_LAST_LOGIN = "last_login";
        public static string AM_TITLE = "title";
        public static string AM_EMAIL2 = "email2";
        public static string AM_MOBILE_PHONE = "mobile_phone";
        public static string AM_MOBILE_PHONE2 = "mobile_phone2";
        public static string AM_HOME_PHONE = "home_phone";
        public static string AM_OFFICE_PHONE = "office_phone";
        public static string AM_ADDRESS = "address";
        public static string AM_ADDRESS2 = "address2";
        public static string AM_CITY = "city";
        public static string AM_STATE = "state";
        public static string AM_COUNTRY = "country";
        public static string AM_ZIPCODE = "zipcode";
        public static string AM_CUSTOM1 = "custom1";
        public static string AM_CUSTOM2 = "custom2";
        public static string AM_CUSTOM3 = "custom3";
        public static string AM_NOTES = "notes";
        //public static string AM_OUTBOUND_NUMBER = "outbound_number";
        public static string AM_USER_UNIQUE_KEY = "customer_user_id";


        private string amServer = null;
        private string clientId = null;
        private string clientSecret = null;
        private string groupMappingType = null;
        private string adUserFieldForGroupMapping = null;
        private string ignoreDeleteUserList = null;

        private string ldapUrl = null;
        private string username = null;
        private string password = null;
        private string adEmailField = null;
        private string adMobileNumberField = null;


        private DirectorySearcher userSearcher, groupSearcher = null;


        private ArrayList activeDirectoryUsers;
        private ArrayList alertMediaGroups;
        private Hashtable groupMappings; //  AlertMedia groups mapped to Active Directory groups
        public Hashtable userFieldMappings; // Alert Media user fields to Active Directory user fields

        public Configuration config = null;

        public ActiveDirectoryClient(Configuration config)
        {
            AppSettingsSection amSection, adSection, groupMappingsSection, fieldMappingsSection;
            this.config = config;
            amSection = (AppSettingsSection)config.Sections["AMSettings"];
            adSection = (AppSettingsSection)config.Sections["ADSettings"];
            groupMappingsSection = (AppSettingsSection)config.Sections["GroupMappings"];
            fieldMappingsSection = (AppSettingsSection)config.Sections["FieldMappings"];

            this.activeDirectoryUsers = new ArrayList();
            this.alertMediaGroups = new ArrayList();
            this.groupMappings = new Hashtable();
            this.userFieldMappings = new Hashtable();

            

            this.amServer = amSection.Settings["BaseUrl"].Value;
            this.clientId = amSection.Settings["ClientID"].Value;
            this.clientSecret = amSection.Settings["ClientSecret"].Value;
            this.groupMappingType = amSection.Settings["GroupMappingType"].Value;
            this.adUserFieldForGroupMapping = amSection.Settings["ADUserFieldForGroupMapping"].Value;
            this.ignoreDeleteUserList = amSection.Settings["IgnoreDeleteUserList"].Value;

            AlertMediaClient.setBaseUrl(this.amServer);
            AlertMediaClient.setClientId(this.clientId);
            AlertMediaClient.setClientKey(this.clientSecret);

            this.ldapUrl = adSection.Settings["LdapUrl"].Value;
            this.username = adSection.Settings["UserName"].Value;
            this.password = adSection.Settings["Password"].Value;
            this.adEmailField = fieldMappingsSection.Settings["email"].Value;
            this.adMobileNumberField = fieldMappingsSection.Settings["mobile_phone"].Value;

            foreach (var key in groupMappingsSection.Settings.AllKeys)
            {
                this.alertMediaGroups.Add(key);
                this.groupMappings.Add(key, groupMappingsSection.Settings[key].Value);
            }

            foreach (var key in fieldMappingsSection.Settings.AllKeys)
            {
                this.userFieldMappings.Add(key, fieldMappingsSection.Settings[key].Value);                
            }

            if (this.userSearcher == null)
            {
                connectToActiveDirectory();
            }

        }

        public SortedDictionary<String, String> fetchUserFields(string excludeDisableAccounts)
        {
            try
            {
                SortedDictionary<String, String> userObject = new SortedDictionary<String, String>();
                SearchResultCollection resultCol = this.userSearcher.FindAll();
                if (resultCol != null && resultCol.Count > 1)
                {
                    for (int i = 0; i < resultCol.Count; i++)
                    {
                        SearchResult adUserObject = resultCol[i];
                        if (isUserActive(adUserObject, excludeDisableAccounts))
                        {
                            if (adUserObject.Properties.Contains(this.adEmailField) ||
                                adUserObject.Properties.Contains(this.adMobileNumberField))
                            {
                                foreach (string propName in adUserObject.Properties.PropertyNames)
                                {
                                    userObject.Add(propName, adUserObject.Properties[propName][0].ToString());
                                }
                                return userObject;
                            }
                        }
                    }
                }
                return userObject;
            }
            catch(Exception ex)
            {
                using (StreamWriter w = File.AppendText("adsynclog.txt"))
                {
                    Utils.Log(ex.ToString(), w);                   
                }
                throw ex;
            }
        }

        public ArrayList getActiveDirectoryUserGroups()
        {
            try
            {
                ArrayList groupList = new ArrayList();
                if (this.groupSearcher == null)
                {
                    this.connectToActiveDirectory();
                }
                SearchResultCollection results = this.groupSearcher.FindAll();
                if (results.Count > 0)
                {
                    foreach (SearchResult resEn in results)
                    {
                        string OUName = resEn.GetDirectoryEntry().Name;
                        groupList.Add(OUName);
                    }
                }
                groupList.Sort();
                return groupList;
            }
            catch(Exception ex)
            {
                using (StreamWriter w = File.AppendText("adsynclog.txt"))
                {
                    Utils.Log(ex.ToString(), w);
                }
                throw ex;
            }
        }


        /********************************** TO BE MODIFIED BY CUSTOMER ************************************
             * 
             * 
             * Return Arraylist where each item is a hashtable.
             * In the hashtable, the key will be AD user object field names and value contains
             * the actual value stored in AD.
             *
             * 		 
             ************************************************************************************/

        public void getUserDetailsFromActiveDirectory(string filterQuery, string excludeDisableAccounts)
        {
            Hashtable amUserObject = null;
            try
            {
                if(!filterQuery.Equals(""))
                {
                    this.userSearcher.Filter = string.Format(filterQuery);
                }                
                SearchResultCollection resultCol = this.userSearcher.FindAll();
                if (resultCol != null)
                {
                    for (int counter = 0; counter < resultCol.Count; counter++)
                    {
                        amUserObject = null;
                        SearchResult adUserObject = resultCol[counter];
                        if (isUserActive(adUserObject, excludeDisableAccounts))
                        {
                            if (adUserObject.Properties.Contains(this.adEmailField) ||
                                adUserObject.Properties.Contains(this.adMobileNumberField))
                            {
                                amUserObject = this.copyFromADUserObject2AMUserObject(adUserObject);
                                if (this.groupMappingType.Equals("FieldValue"))
                                {                                    
                                    for (int i = 0; i < this.alertMediaGroups.Count; i++)
                                    {
                                        if (amUserObject.ContainsKey(this.adUserFieldForGroupMapping) && this.groupMappings[this.alertMediaGroups[i]].ToString() != "")
                                        {
                                            if (amUserObject[this.adUserFieldForGroupMapping].ToString().Equals(this.groupMappings[this.alertMediaGroups[i]].ToString(), StringComparison.InvariantCultureIgnoreCase))
                                            {
                                                amUserObject[this.alertMediaGroups[i]] = "yes";
                                            }
                                            else
                                            {
                                                amUserObject[this.alertMediaGroups[i]] = "no";
                                            }
                                        }
                                        else
                                        {
                                            amUserObject[this.alertMediaGroups[i]] = "no";
                                        }
                                    }
                                }
                                else if (this.groupMappingType.Equals("OU"))
                                {
                                    for (int i = 0; i < this.alertMediaGroups.Count; i++)
                                    {
                                        amUserObject[this.alertMediaGroups[i]] = "no";
                                    }
                                    string ouNameList = adUserObject.Properties["distinguishedname"][0].ToString();
                                    bool flag = true;
                                    while(flag)
                                    {
                                        int idx = ouNameList.IndexOf("OU=");
                                        if(idx == -1)
                                        {
                                            flag = false;
                                        }
                                        else
                                        {
                                            string ouName = "";
                                            int commaIdx = ouNameList.IndexOf(",", idx);
                                            if (commaIdx != -1)
                                            {
                                                ouName = ouNameList.Substring(idx, (commaIdx-idx));
                                                ouNameList = ouNameList.Substring(commaIdx+1, (ouNameList.Length-(commaIdx+1)));
                                            }
                                            else
                                            {
                                                ouName = ouNameList.Substring(idx, (ouNameList.Length-idx));
                                                ouNameList = "";
                                            }
                                            foreach(var amGroup in this.groupMappings.Keys)
                                            {
                                                if(this.groupMappings[amGroup].ToString().Equals(ouName, StringComparison.InvariantCultureIgnoreCase))
                                                {
                                                    amUserObject[amGroup] = "yes";
                                                    break;
                                                }
                                            }
                                            
                                        }
                                    }
                                }                                
                                activeDirectoryUsers.Add(amUserObject);
                            }
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                using (StreamWriter w = File.AppendText("adsynclog.txt"))
                {
                    Utils.Log(ex.ToString(), w);
                    if(amUserObject != null)
                    {
                        Utils.Log(amUserObject["email"].ToString(), w);
                    }                    
                }
                throw ex;
            }
        }

        private bool isUserActive(SearchResult sr, string excludeDisableAccounts)
        {
            if (excludeDisableAccounts.Equals("no"))
            {
                return true;
            }
            DirectoryEntry de = sr.GetDirectoryEntry();
            if (de == null || de.NativeGuid == null) return false;
            int flags = (int)de.Properties["userAccountControl"].Value;
            if(flags == 0)
            {
                return false;
            }
            else
            {
                return !Convert.ToBoolean(flags & 0x0002);
            }            
        }

        /****************** TO BE MODIFIED BY CUSTOMER *************
             * 
             * Code to connect to ActiveDirectory
             * 
             *****************************************************************/


        private void connectToActiveDirectory()
        {
            try
            {
                if (this.userSearcher == null)
                {
                    if (string.IsNullOrEmpty(this.ldapUrl))
                    {
                        throw new System.InvalidOperationException("Please set Ldap Url!");
                    }
                    if (string.IsNullOrEmpty(this.username))
                    {
                        throw new System.InvalidOperationException("Please set domainPath!");
                    }
                    if (string.IsNullOrEmpty(this.password))
                    {
                        throw new ArgumentException("Please set active directory password!");
                    }
                    this.userSearcher = new DirectorySearcher(new DirectoryEntry(
                        this.ldapUrl,
                        this.username,
                        this.password,
                        AuthenticationTypes.ServerBind));
                    this.userSearcher.PageSize = 1000;
                    this.userSearcher.Filter = "(&(objectCategory=person)(objectClass=user))";
                }
                if (this.groupSearcher == null)
                {
                    if (string.IsNullOrEmpty(this.ldapUrl))
                    {
                        throw new System.InvalidOperationException("Please set Ldap Url!");
                    }
                    if (string.IsNullOrEmpty(this.username))
                    {
                        throw new System.InvalidOperationException("Please set domainPath!");
                    }
                    if (string.IsNullOrEmpty(this.password))
                    {
                        throw new ArgumentException("Please set active directory password!");
                    }
                    this.groupSearcher = new DirectorySearcher(new DirectoryEntry(
                        this.ldapUrl,
                        this.username,
                        this.password,
                        AuthenticationTypes.ServerBind));
                    this.groupSearcher.PageSize = 250;
                    this.groupSearcher.Filter = "(&(objectClass=organizationalUnit))";
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter w = File.AppendText("adsynclog.txt"))
                {
                    Utils.Log(ex.ToString(), w);                    
                }
                throw ex;
            }
        }


        private Hashtable copyFromADUserObject2AMUserObject(SearchResult adUserObject)
        {
            Hashtable amUserObject = new Hashtable();

            if (adUserObject.Properties.Contains(this.userFieldMappings[AM_FIRST_NAME].ToString()))
            {
                if (adUserObject.Properties[this.userFieldMappings[AM_FIRST_NAME].ToString()][0].ToString().Length > 30)
                {
                    amUserObject.Add(AM_FIRST_NAME, adUserObject.Properties[this.userFieldMappings[AM_FIRST_NAME].ToString()][0].ToString().Substring(0, 30));
                }
                else
                {
                    amUserObject.Add(AM_FIRST_NAME, adUserObject.Properties[this.userFieldMappings[AM_FIRST_NAME].ToString()][0].ToString());
                }
            }
            else
            {
                amUserObject.Add(AM_FIRST_NAME, "");
            }

            if (adUserObject.Properties.Contains(this.userFieldMappings[AM_LAST_NAME].ToString()))
            {
                if (adUserObject.Properties[this.userFieldMappings[AM_LAST_NAME].ToString()][0].ToString().Length > 30)
                {
                    amUserObject.Add(AM_LAST_NAME, adUserObject.Properties[this.userFieldMappings[AM_LAST_NAME].ToString()][0].ToString().Substring(0, 30));
                }
                else
                {
                    amUserObject.Add(AM_LAST_NAME, adUserObject.Properties[this.userFieldMappings[AM_LAST_NAME].ToString()][0].ToString());
                }
            }
            else
            {
                amUserObject.Add(AM_LAST_NAME, "");
            }

            if (adUserObject.Properties.Contains(this.userFieldMappings[AM_EMAIL].ToString()))
            {
                if (adUserObject.Properties[this.userFieldMappings[AM_EMAIL].ToString()][0].ToString().Length > 254)
                {
                    amUserObject.Add(AM_EMAIL, adUserObject.Properties[this.userFieldMappings[AM_EMAIL].ToString()][0].ToString().Substring(0, 254));
                }
                else
                {
                    amUserObject.Add(AM_EMAIL, adUserObject.Properties[this.userFieldMappings[AM_EMAIL].ToString()][0].ToString());
                }                
            }
            else
            {
                amUserObject.Add(AM_EMAIL, "");
            }

            if (adUserObject.Properties.Contains(this.userFieldMappings[AM_TITLE].ToString()))
            {
                if (adUserObject.Properties[this.userFieldMappings[AM_TITLE].ToString()][0].ToString().Length > 60)
                {
                    amUserObject.Add(AM_TITLE, adUserObject.Properties[this.userFieldMappings[AM_TITLE].ToString()][0].ToString().Substring(0, 60));
                }
                else
                {
                    amUserObject.Add(AM_TITLE, adUserObject.Properties[this.userFieldMappings[AM_TITLE].ToString()][0].ToString());
                }
            }
            else
            {
                amUserObject.Add(AM_TITLE, "");
            }

            if (adUserObject.Properties.Contains(this.userFieldMappings[AM_EMAIL2].ToString()))
            {
                if(adUserObject.Properties[this.userFieldMappings[AM_EMAIL2].ToString()][0].ToString().Length > 254)
                {
                    amUserObject.Add(AM_EMAIL2, adUserObject.Properties[this.userFieldMappings[AM_EMAIL2].ToString()][0].ToString().Substring(0, 254));
                }
                else
                {
                    amUserObject.Add(AM_EMAIL2, adUserObject.Properties[this.userFieldMappings[AM_EMAIL2].ToString()][0].ToString());
                }                
            }
            else
            {
                amUserObject.Add(AM_EMAIL2, "");
            }

            if (adUserObject.Properties.Contains(this.userFieldMappings[AM_MOBILE_PHONE].ToString()))
            {
                if (adUserObject.Properties[this.userFieldMappings[AM_MOBILE_PHONE].ToString()][0].ToString().Length > 20)
                {
                    amUserObject.Add(AM_MOBILE_PHONE, adUserObject.Properties[this.userFieldMappings[AM_MOBILE_PHONE].ToString()][0].ToString().Substring(0, 20));
                }
                else
                {
                    amUserObject.Add(AM_MOBILE_PHONE, adUserObject.Properties[this.userFieldMappings[AM_MOBILE_PHONE].ToString()][0].ToString());
                }
            }
            else
            {
                amUserObject.Add(AM_MOBILE_PHONE, "");
            }

            if (adUserObject.Properties.Contains(this.userFieldMappings[AM_MOBILE_PHONE2].ToString()))
            {
                if (adUserObject.Properties[this.userFieldMappings[AM_MOBILE_PHONE2].ToString()][0].ToString().Length > 20)
                {
                    amUserObject.Add(AM_MOBILE_PHONE2, adUserObject.Properties[this.userFieldMappings[AM_MOBILE_PHONE2].ToString()][0].ToString().Substring(0, 20));
                }
                else
                {
                    amUserObject.Add(AM_MOBILE_PHONE2, adUserObject.Properties[this.userFieldMappings[AM_MOBILE_PHONE2].ToString()][0].ToString());
                }
            }
            else
            {
                amUserObject.Add(AM_MOBILE_PHONE2, "");
            }

            if (adUserObject.Properties.Contains(this.userFieldMappings[AM_HOME_PHONE].ToString()))
            {
                if (adUserObject.Properties[this.userFieldMappings[AM_HOME_PHONE].ToString()][0].ToString().Length > 20)
                {
                    amUserObject.Add(AM_HOME_PHONE, adUserObject.Properties[this.userFieldMappings[AM_HOME_PHONE].ToString()][0].ToString().Substring(0, 20));
                }
                else
                {
                    amUserObject.Add(AM_HOME_PHONE, adUserObject.Properties[this.userFieldMappings[AM_HOME_PHONE].ToString()][0].ToString());
                }
            }
            else
            {
                amUserObject.Add(AM_HOME_PHONE, "");
            }

            if (adUserObject.Properties.Contains(this.userFieldMappings[AM_OFFICE_PHONE].ToString()))
            {
                if (adUserObject.Properties[this.userFieldMappings[AM_OFFICE_PHONE].ToString()][0].ToString().Length > 20)
                {
                    amUserObject.Add(AM_OFFICE_PHONE, adUserObject.Properties[this.userFieldMappings[AM_OFFICE_PHONE].ToString()][0].ToString().Substring(0, 20));
                }
                else
                {
                    amUserObject.Add(AM_OFFICE_PHONE, adUserObject.Properties[this.userFieldMappings[AM_OFFICE_PHONE].ToString()][0].ToString());
                }
            }
            else
            {
                amUserObject.Add(AM_OFFICE_PHONE, "");
            }

            if (adUserObject.Properties.Contains(this.userFieldMappings[AM_ADDRESS].ToString()))
            {
                if (adUserObject.Properties[this.userFieldMappings[AM_ADDRESS].ToString()][0].ToString().Length > 100)
                {
                    amUserObject.Add(AM_ADDRESS, adUserObject.Properties[this.userFieldMappings[AM_ADDRESS].ToString()][0].ToString().Substring(0, 100));
                }
                else
                {
                    amUserObject.Add(AM_ADDRESS, adUserObject.Properties[this.userFieldMappings[AM_ADDRESS].ToString()][0].ToString());
                }
            }
            else
            {
                amUserObject.Add(AM_ADDRESS, "");
            }

            if (adUserObject.Properties.Contains(this.userFieldMappings[AM_ADDRESS2].ToString()))
            {
                if (adUserObject.Properties[this.userFieldMappings[AM_ADDRESS2].ToString()][0].ToString().Length > 100)
                {
                    amUserObject.Add(AM_ADDRESS2, adUserObject.Properties[this.userFieldMappings[AM_ADDRESS2].ToString()][0].ToString().Substring(0, 100));
                }
                else
                {
                    amUserObject.Add(AM_ADDRESS2, adUserObject.Properties[this.userFieldMappings[AM_ADDRESS2].ToString()][0].ToString());
                }
            }
            else
            {
                amUserObject.Add(AM_ADDRESS2, "");
            }

            if (adUserObject.Properties.Contains(this.userFieldMappings[AM_CITY].ToString()))
            {
                if (adUserObject.Properties[this.userFieldMappings[AM_CITY].ToString()][0].ToString().Length > 50)
                {
                    amUserObject.Add(AM_CITY, adUserObject.Properties[this.userFieldMappings[AM_CITY].ToString()][0].ToString().Substring(0, 50));
                }
                else
                {
                    amUserObject.Add(AM_CITY, adUserObject.Properties[this.userFieldMappings[AM_CITY].ToString()][0].ToString());
                }
            }
            else
            {
                amUserObject.Add(AM_CITY, "");
            }

            if (adUserObject.Properties.Contains(this.userFieldMappings[AM_STATE].ToString()))
            {
                if (adUserObject.Properties[this.userFieldMappings[AM_STATE].ToString()][0].ToString().Length > 20)
                {
                    amUserObject.Add(AM_STATE, adUserObject.Properties[this.userFieldMappings[AM_STATE].ToString()][0].ToString().Substring(0, 20));
                }
                else
                {
                    amUserObject.Add(AM_STATE, adUserObject.Properties[this.userFieldMappings[AM_STATE].ToString()][0].ToString());
                }
            }
            else
            {
                amUserObject.Add(AM_STATE, "");
            }

            if (adUserObject.Properties.Contains(this.userFieldMappings[AM_COUNTRY].ToString()))
            {
                if (adUserObject.Properties[this.userFieldMappings[AM_COUNTRY].ToString()][0].ToString().Length > 2)
                {
                    amUserObject.Add(AM_COUNTRY, adUserObject.Properties[this.userFieldMappings[AM_COUNTRY].ToString()][0].ToString().Substring(0, 2));
                }
                else
                {
                    amUserObject.Add(AM_COUNTRY, adUserObject.Properties[this.userFieldMappings[AM_COUNTRY].ToString()][0].ToString());
                }
            }
            else
            {
                amUserObject.Add(AM_COUNTRY, "");
            }

            if (adUserObject.Properties.Contains(this.userFieldMappings[AM_ZIPCODE].ToString()))
            {
                if (adUserObject.Properties[this.userFieldMappings[AM_ZIPCODE].ToString()][0].ToString().Length > 15)
                {
                    amUserObject.Add(AM_ZIPCODE, adUserObject.Properties[this.userFieldMappings[AM_ZIPCODE].ToString()][0].ToString().Substring(0, 15));
                }
                else
                {
                    amUserObject.Add(AM_ZIPCODE, adUserObject.Properties[this.userFieldMappings[AM_ZIPCODE].ToString()][0].ToString());
                }
            }
            else
            {
                amUserObject.Add(AM_ZIPCODE, "");
            }

            if (adUserObject.Properties.Contains(this.userFieldMappings[AM_CUSTOM1].ToString()))
            {
                amUserObject.Add(AM_CUSTOM1, adUserObject.Properties[this.userFieldMappings[AM_CUSTOM1].ToString()][0]);
            }
            else
            {
                amUserObject.Add(AM_CUSTOM1, "");
            }

            if (adUserObject.Properties.Contains(this.userFieldMappings[AM_CUSTOM2].ToString()))
            {
                amUserObject.Add(AM_CUSTOM2, adUserObject.Properties[this.userFieldMappings[AM_CUSTOM2].ToString()][0]);
            }
            else
            {
                amUserObject.Add(AM_CUSTOM2, "");
            }

            if (adUserObject.Properties.Contains(this.userFieldMappings[AM_CUSTOM3].ToString()))
            {
                amUserObject.Add(AM_CUSTOM3, adUserObject.Properties[this.userFieldMappings[AM_CUSTOM3].ToString()][0]);
            }
            else
            {
                amUserObject.Add(AM_CUSTOM3, "");
            }

            if (adUserObject.Properties.Contains(this.userFieldMappings[AM_NOTES].ToString()))
            {
                amUserObject.Add(AM_NOTES, adUserObject.Properties[this.userFieldMappings[AM_NOTES].ToString()][0]);
            }
            else
            {
                amUserObject.Add(AM_NOTES, "");
            }           
            if (adUserObject.Properties.Contains(this.userFieldMappings[AM_USER_UNIQUE_KEY].ToString()))
            {
                amUserObject.Add(AM_USER_UNIQUE_KEY, adUserObject.Properties[this.userFieldMappings[AM_USER_UNIQUE_KEY].ToString()][0]);
            }
            else
            {
                amUserObject.Add(AM_USER_UNIQUE_KEY, "");
            }

            return amUserObject;
        }

        public string[] createImportFile()
        {
            string[] csvStrings = new string[activeDirectoryUsers.Count + 1];
            try
            {
                var headerRow = string.Format(
                    "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\",\"{16}\",\"{17}\",\"{18}\",\"{19}\"",
                    AM_FIRST_NAME,
                    AM_LAST_NAME,
                    AM_EMAIL,
                    AM_TITLE,
                    AM_EMAIL2,
                    AM_MOBILE_PHONE,
                    AM_MOBILE_PHONE2,
                    AM_HOME_PHONE,
                    AM_OFFICE_PHONE,
                    AM_ADDRESS,
                    AM_ADDRESS2,
                    AM_CITY,
                    AM_STATE,
                    AM_COUNTRY,
                    AM_ZIPCODE,
                    AM_CUSTOM1,
                    AM_CUSTOM2,
                    AM_CUSTOM3,
                    AM_NOTES,
                    AM_USER_UNIQUE_KEY
                );
                for (int i = 0; i < alertMediaGroups.Count; i++)
                {
                    headerRow = headerRow + ",\"" + alertMediaGroups[i] + "\"";
                }
                csvStrings[0] = headerRow;

                for (int i = 0; i < activeDirectoryUsers.Count; i++)
                {
                    Hashtable userObject = (Hashtable)activeDirectoryUsers[i];
                    var dataRow = string.Format(
                        "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\",\"{16}\",\"{17}\",\"{18}\",\"{19}\"",
                        userObject[AM_FIRST_NAME],
                        userObject[AM_LAST_NAME],
                        userObject[AM_EMAIL],
                        userObject[AM_TITLE],
                        userObject[AM_EMAIL2],
                        userObject[AM_MOBILE_PHONE],
                        userObject[AM_MOBILE_PHONE2],
                        userObject[AM_HOME_PHONE],
                        userObject[AM_OFFICE_PHONE],
                        userObject[AM_ADDRESS],
                        userObject[AM_ADDRESS2],
                        userObject[AM_CITY],
                        userObject[AM_STATE],
                        userObject[AM_COUNTRY],
                        userObject[AM_ZIPCODE],
                        userObject[AM_CUSTOM1],
                        userObject[AM_CUSTOM2],
                        userObject[AM_CUSTOM3],
                        userObject[AM_NOTES],
                        userObject[AM_USER_UNIQUE_KEY]
                    );
                    for (int j = 0; j < alertMediaGroups.Count; j++)
                    {
                        dataRow = dataRow + ",\"" + userObject[alertMediaGroups[j]] + "\"";
                    }
                    csvStrings[i + 1] = dataRow;
                }
                return csvStrings;
            }
            catch (Exception ex)
            {
                using (StreamWriter w = File.AppendText("adsynclog.txt"))
                {
                    Utils.Log(ex.ToString(), w);                    
                }
                throw ex;
            }
        }

        public string uploadToAlertMedia(string csvStr)
        {
            try
            {
                string customer = AlertMediaClient.Customer.getCustomerId();
                Hashtable syncUserResponse = AlertMediaClient.User.importUserData(csvStr, customer);
                string syncUserResults = syncUserResponse["data"].ToString();                
                return syncUserResults;
            }
            catch (Exception ex)
            {
                using (StreamWriter w = File.AppendText("adsynclog.txt"))
                {
                    Utils.Log(ex.ToString(), w);                   
                }
                throw ex;
            }
        }

        public string[] previewDeleteUserList(ArrayList userList)
        {
            try
            {
                ArrayList deletedUserList = new ArrayList();                
                string customer = AlertMediaClient.Customer.getCustomerId();                
                Hashtable missingUserResponse = AlertMediaClient.User.getUsersToBeDeleted(customer, userList);
                JavaScriptSerializer j = new JavaScriptSerializer();
                Dictionary<string, object> muResponseDict = (Dictionary<string, object>)j.Deserialize(missingUserResponse["data"].ToString(),
                        typeof(Dictionary<string, object>));               
                if (muResponseDict.ContainsKey("missing"))
                {
                    ArrayList missingUserList = (ArrayList)muResponseDict["missing"];
                    if (missingUserList.Count > 0)
                    {
                        for (int i = 0; i < missingUserList.Count; i++)
                        {
                            if (ignoreDeleteUserList.IndexOf(missingUserList[i].ToString()) == -1)
                            {
                                Hashtable userDetailsResponse = AlertMediaClient.User.get(missingUserList[i].ToString());
                                Dictionary<string, object> d = (Dictionary<string, object>)j.Deserialize(
                                    userDetailsResponse["data"].ToString(),
                                    typeof(Dictionary<string, object>));
                                deletedUserList.Add(d);
                            }
                        }
                    }                    
                }
                
                
                string[] csvStrings = new string[deletedUserList.Count + 1];
                var headerRow = string.Format(
                    "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\",\"{16}\",\"{17}\",\"{18}\",\"{19}\"",
                    "ID",
                    AM_FIRST_NAME,
                    AM_LAST_NAME,
                    AM_EMAIL,
                    AM_TITLE,
                    AM_EMAIL2,
                    AM_MOBILE_PHONE,
                    AM_MOBILE_PHONE2,
                    AM_HOME_PHONE,
                    AM_OFFICE_PHONE,
                    AM_ADDRESS,
                    AM_ADDRESS2,
                    AM_CITY,
                    AM_STATE,
                    AM_COUNTRY,
                    AM_ZIPCODE,
                    AM_CUSTOM1,
                    AM_CUSTOM2,
                    AM_CUSTOM3,
                    AM_NOTES
                );
                csvStrings[0] = headerRow;
                if (deletedUserList.Count > 0)
                {                    
                    for (int i = 0; i < deletedUserList.Count; i++)
                    {
                        Dictionary<string, Object> userObject = (Dictionary<string, Object>)deletedUserList[i];
                        var dataRow = string.Format(
                            "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\",\"{16}\",\"{17}\",\"{18}\",\"{19}\"",
                            GetValue(userObject, "id"),
                            GetValue(userObject,AM_FIRST_NAME),
                            GetValue(userObject, AM_LAST_NAME),
                            GetValue(userObject, AM_EMAIL),
                            GetValue(userObject, AM_TITLE),
                            GetValue(userObject, AM_EMAIL2),
                            GetValue(userObject, AM_MOBILE_PHONE),
                            GetValue(userObject, AM_MOBILE_PHONE2),
                            GetValue(userObject, AM_HOME_PHONE),
                            GetValue(userObject, AM_OFFICE_PHONE),
                            GetValue(userObject, AM_ADDRESS),
                            GetValue(userObject, AM_ADDRESS2),
                            GetValue(userObject, AM_CITY),
                            GetValue(userObject, AM_STATE),
                            GetValue(userObject, AM_COUNTRY),
                            GetValue(userObject, AM_ZIPCODE),
                            GetValue(userObject, AM_CUSTOM1),
                            GetValue(userObject, AM_CUSTOM2),
                            GetValue(userObject, AM_CUSTOM3),
                            GetValue(userObject, AM_NOTES)
                        );                        
                        csvStrings[i + 1] = dataRow;
                    }
                }                
                return csvStrings;                
            }
            catch(Exception ex)
            {
                using (StreamWriter w = File.AppendText("adsynclog.txt"))
                {
                    Utils.Log(ex.ToString(), w);                 
                }
                throw ex;
            }


        }

        public void deleteUsersFromAlertMedia(ArrayList userIdList)
        {
            for(int i = 0; i < userIdList.Count; i++)
            {
                try
                {
                    AlertMediaClient.User.delete(userIdList[i].ToString());
                }
                catch(Exception ex)
                {
                    using (StreamWriter w = File.AppendText("adsynclog.txt"))
                    {
                        Utils.Log(ex.ToString(), w);
                        Utils.Log(userIdList[i].ToString(), w);
                    }
                }
                
            }
        }

        private string GetValue(Dictionary<string, object> dict, string key)
        {
            object returnValue;
            if (!dict.TryGetValue(key, out returnValue))
            {
                returnValue = string.Empty;
            }
            if(returnValue != null)
            {
                return returnValue.ToString();
            }
            else
            {
                return string.Empty;
            }
            
        }
    }
}
