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
            Boolean syncParam = false;
            Boolean deleteParam = false;

            for(int i=0;i<args.Length;i++)
            {
                if(args[i].ToLower().Equals("-sync"))
                {
                    syncParam = true;
                }
                else if (args[i].ToLower().Equals("-delete"))
                {
                    deleteParam = true;
                }
            }

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppSettingsSection amSection = (AppSettingsSection)config.Sections["AMSettings"];
            ActiveDirectoryClient client = new ActiveDirectoryClient(config);
            client.getUserDetailsFromActiveDirectory(
                amSection.Settings["UserExcludeFilter"].Value,
                amSection.Settings["ExcludeDisableAccount"].Value
                );
            string[] csvStr = client.createImportFile();
            Console.WriteLine("################### Sync Users - Start ###################");
            Console.WriteLine("");
            StringBuilder importDataStr = new StringBuilder();
            for(int i=0; i<csvStr.Length; i++)
            {
                importDataStr.AppendLine(csvStr[i]);
                Console.WriteLine(csvStr[i]);
            }
            string response = "";
            if (syncParam)
            {                
                response = client.uploadToAlertMedia(importDataStr.ToString());
                Console.WriteLine("*********************************************************");
                Console.WriteLine("");
                Console.WriteLine(response);
                Console.WriteLine("");
                Console.WriteLine("*********************************************************");
            }
            else
            {
                Console.WriteLine("*********************************************************");
                Console.WriteLine("");
                Console.WriteLine(" Above listed users have not been synced to AlertMedia. This was a dry run. To actually sync them, you need to send an args parameter (-sync)");
                Console.WriteLine("");
                Console.WriteLine("*********************************************************");
            }
            Console.WriteLine("");
            Console.WriteLine("################### Sync Users - End ###################");
            if (syncParam)
            {
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
                Console.WriteLine("################### Delete Users - Start ###################");
                Console.WriteLine("");
                string[] delCsvStr = client.previewDeleteUserList(importedUserList);
                ArrayList deletedUserIdList = new ArrayList();
                for (int i = 1; i < delCsvStr.Length; i++)
                {
                    string strLine = delCsvStr[i];
                    Console.WriteLine(strLine);
                    List<string> fields = SplitCSV(strLine);
                    deletedUserIdList.Add(fields[0].Replace("\"", ""));
                }
                if (deleteParam)
                {
                    client.deleteUsersFromAlertMedia(deletedUserIdList);
                    Console.WriteLine("*********************************************************");
                    Console.WriteLine("");
                    Console.WriteLine(" All the " + deletedUserIdList.Count + " users have been deleted from AlertMedia ");
                    Console.WriteLine("");
                    Console.WriteLine("*********************************************************");
                }
                else
                {
                    Console.WriteLine("*********************************************************");
                    Console.WriteLine("");
                    Console.WriteLine(deletedUserIdList.Count + " of the above listed users have not been deleted from AlertMedia. This was a dry run. To actually delete them, you need to send two args parameter (-sync -delete)");
                    Console.WriteLine("");
                    Console.WriteLine("*********************************************************");
                }
                Console.WriteLine("");
                Console.WriteLine("################### Sync Users - End ###################");
                Console.ReadLine();
            }
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