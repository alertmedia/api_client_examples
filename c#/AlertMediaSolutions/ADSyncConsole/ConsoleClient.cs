using System;
using alertmedia.activedirectory;
using System.Text;
using System.Configuration;
using System.Collections;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace My {
	
	public class ConsoleClient {

        public static void Main(string[] args) {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppSettingsSection amSection = (AppSettingsSection)config.Sections["AMSettings"];
            ActiveDirectoryClient client = new ActiveDirectoryClient(config);
            client.getUserDetailsFromActiveDirectory(
                amSection.Settings["UserExcludeFilter"].Value,
                amSection.Settings["ExcludeDisableAccount"].Value
                );
            string[] csvStr = client.createImportFile();
            StringBuilder importDataStr = new StringBuilder();
            for(int i=0; i<csvStr.Length; i++)
            {
                importDataStr.AppendLine(csvStr[i]);
            }
            Console.WriteLine(" ******************* Sync Users - Start ***************** ");
            string response = client.uploadToAlertMedia(importDataStr.ToString());            
            Console.WriteLine(response);
            Console.WriteLine(" ******************* Sync Users - End ***************** ");
            ArrayList importedUserList = new ArrayList();
            JavaScriptSerializer j = new JavaScriptSerializer();
            Dictionary<string, object> a = (Dictionary<string, object>)j.Deserialize(response, typeof(Dictionary<string, object>));
            StringBuilder textBoxResponse = new StringBuilder();
            if (a.ContainsKey("stats"))
            {
                importedUserList = (ArrayList)a["successes"];
                ArrayList failuresList = (ArrayList)a["failures"];
                for (int i = 0; i < failuresList.Count; i++)
                {
                    importedUserList.Add(failuresList[i]);
                }
            }
            Console.WriteLine(" ******************* Delete Users - Start ***************** ");
            string[] delCsvStr = client.previewDeleteUserList(importedUserList);
            ArrayList deletedUserIdList = new ArrayList();
            for (int i = 1; i < delCsvStr.Length; i++)
            {
                string strLine = delCsvStr[i];                
                List<string> fields = SplitCSV(strLine);
                deletedUserIdList.Add(fields[0].Replace("\"", ""));
            }            
            client.deleteUsersFromAlertMedia(deletedUserIdList);
            Console.WriteLine(" ******************* Delete Users - End ***************** ");
            Console.ReadLine();            
           
        }

        public static List<string> SplitCSV(string input)
        {
            Regex csvSplit = new Regex("(?:^|,)(\"(?:[^\"]+|\"\")*\"|[^,]*)", RegexOptions.Compiled);
            List<string> list = new List<string>();
            string curr = null;
            foreach (Match match in csvSplit.Matches(input))
            {
                curr = match.Value;
                if (0 == curr.Length)
                {
                    list.Add("");
                }

                list.Add(curr.TrimStart(','));
            }

            return list;
        }
    }

    

}