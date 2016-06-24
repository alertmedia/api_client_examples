using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using alertmedia.activedirectory;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Web.Script.Serialization;
using System.Collections;
using System.IO;

namespace ADSyncClient
{
    public partial class SyncUsersPanel : UserControl
    {
        public AMForm parentForm = null;
        private Configuration config = null;
        private Button prevButton, previewButton, syncButton, nextButton = null;
        private WebBrowser tablePanel = null;
        private TextBox resultsPanel = null;
        private TextBox filterField = null;
        private CheckBox disableAccounts = null;
        private AppSettingsSection amSection = null;


        private string importCSVData = "";

        private ActiveDirectoryClient client = null;
        private ArrayList importedUserList;

        public SyncUsersPanel(Configuration config)
        {
            InitializeComponent();
            this.config = config;
            amSection = (AppSettingsSection)config.Sections["AMSettings"];

            Label l1 = new Label();
            l1.AutoSize = true;
            l1.Text = "To import users to AlertMedia from Active Directory, click on list Users button";
            l1.Location = new Point(200, 10);
            this.Controls.Add(l1);

            Label l2 = new Label();
            l2.AutoSize = true;
            l2.Text = "Additional Filter for AD";          
            l2.Location = new Point(50, 35);
            this.Controls.Add(l2);

            filterField = new TextBox();
            filterField.Text = amSection.Settings["UserExcludeFilter"].Value;
            filterField.Width = 200;
            filterField.Height = 25;
            filterField.Location = new Point(220, 35);
            this.Controls.Add(filterField);

            Label l4 = new Label();
            l4.AutoSize = true;
            l4.Text = "Use LDAP search syntax, i.e. (title=*)";
            l4.Font = new Font("Serif", 8, FontStyle.Regular);
            l4.Location = new Point(220, 55);
            this.Controls.Add(l4);

            Label l3 = new Label();
            l3.AutoSize = true;
            l3.Text = "Exclude disabled accounts";
            l3.Location = new Point(450, 35);
            this.Controls.Add(l3);

            disableAccounts = new CheckBox();
            if (amSection.Settings["ExcludeDisableAccount"].Value.Equals("yes"))
            {
                disableAccounts.Checked = true;
            }
            else
            {
                disableAccounts.Checked = false;
            }
            disableAccounts.Location = new Point(585, 30);
            this.Controls.Add(disableAccounts);

            prevButton = new Button();
            prevButton.Text = "Back";
            prevButton.Click += new System.EventHandler(this.prevButton_Click);
            prevButton.Location = new Point(125, 70);
            this.Controls.Add(prevButton);

            previewButton = new Button();
            previewButton.Text = "List Users";
            previewButton.Click += new System.EventHandler(this.previewButton_Click);
            previewButton.Location = new Point(275, 70);
            this.Controls.Add(previewButton);

            syncButton = new Button();
            syncButton.Text = "Sync Users";
            syncButton.Click += new System.EventHandler(this.syncUsersButton_Click);
            syncButton.Location = new Point(425, 70);
            this.Controls.Add(syncButton);
            syncButton.Enabled = false;

            nextButton = new Button();
            nextButton.Text = "Next";
            nextButton.Click += new System.EventHandler(this.nextButton_Click);
            nextButton.Location = new Point(575, 70);
            this.Controls.Add(nextButton);
            nextButton.Enabled = false;

            tablePanel = new WebBrowser();
            tablePanel.Width = 690;
            tablePanel.Height = 170;
            tablePanel.Location = new Point(20, 100);
            this.Controls.Add(tablePanel);

            resultsPanel = new TextBox();
            resultsPanel.Multiline = true;
            resultsPanel.Width = 350;
            resultsPanel.Height = 125;
            resultsPanel.ScrollBars = ScrollBars.Vertical;
            resultsPanel.Location = new Point(200, 300);
            this.Controls.Add(resultsPanel);

            resultsPanel.Text = "Results of syncing users from Active Directory to AlertMedia will be displayed here";
        }

        private void prevButton_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            parentForm.decremenPage();
            parentForm.displayPanel();
            Cursor.Current = Cursors.Default;
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            parentForm.incrementPage();
            parentForm.importedUserList = importedUserList;
            parentForm.displayPanel();
            Cursor.Current = Cursors.Default;
        }

        private void previewButton_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            amSection.Settings["UserExcludeFilter"].Value = filterField.Text;
            if(disableAccounts.Checked)
            {
                amSection.Settings["ExcludeDisableAccount"].Value = "yes";
            }
            else
            {
                amSection.Settings["ExcludeDisableAccount"].Value = "no";
            }
            config.Save(ConfigurationSaveMode.Minimal);
            syncButton.Enabled = false;
            resultsPanel.Text = "Results of syncing users from Active Directory to AlertMedia will be displayed here";
            this.Controls.Remove(tablePanel);
            this.Controls.Add(tablePanel);
            StringBuilder dataStr = new StringBuilder();
            string[] csvStr;
            try
            {

                client = new ActiveDirectoryClient(this.config);
                client.getUserDetailsFromActiveDirectory(filterField.Text, amSection.Settings["ExcludeDisableAccount"].Value);
                csvStr = client.createImportFile();
                string htmlContent = "<html><body style=\"padding: 0; margin: 0;\"><table>";
                string[] headers = this.SplitCSV(csvStr[0]);
                htmlContent = htmlContent + "<tr>";
                dataStr.AppendLine(csvStr[0]);
                for (int i=0; i<headers.Length; i++)
                {
                    string value = headers[i].Replace("\"", "");
                    if (value == "")
                    {
                        value = "&nbsp;";
                    }
                    htmlContent = htmlContent + "<th style=\"font-family: sans-serif; font-size: 12px; background: #f2e6d9; border: solid #DEDEDE 1.0pt; mso-border-alt: solid #DEDEDE .75pt; padding: 0cm 0cm 0cm 0cm; height: 10.75pt;\" ><b>" + value + "</b></th>";
                }
                htmlContent = htmlContent + "</tr>";
                for (int i = 1; i < csvStr.Length; i++)
                {
                    string strLine = csvStr[i];
                    dataStr.AppendLine(strLine);
                    string[] fields = this.SplitCSV(strLine);
                    htmlContent = htmlContent + "<tr>";
                    for (int j = 0; j < fields.Length; j++)
                    {
                        string value = fields[j].Replace("\"", "");
                        if(value == "")
                        {
                            value = "&nbsp;";
                        }
                        htmlContent = htmlContent + "<td style=\"font-family: sans-serif; font-size: 10px; border: solid #DEDEDE 1.0pt; mso-border-alt: solid #DEDEDE .75pt; padding: 0cm 0cm 0cm 0cm; height: 10.75pt;\" >" + value + "</td>";
                    }
                    htmlContent = htmlContent + "</tr>";
                }
                htmlContent = htmlContent + "</table></body></html>";
                tablePanel.DocumentText = htmlContent;


                importCSVData = dataStr.ToString();
                syncButton.Enabled = true;
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                tablePanel.DocumentText = "";
                using (StreamWriter w = File.AppendText("adsynclog.txt"))
                {
                    parentForm.Log(ex.ToString(), w);
                }

                MessageBox.Show("Not able to connect to you Active Directory instance. Please check you internet connection and try again. If problem persists, please contact your internal support team.");
                Cursor.Current = Cursors.Default;
                return;
            }
        }

        private void syncUsersButton_Click(object sender, EventArgs e)
        {

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                string response = client.uploadToAlertMedia(importCSVData);
                JavaScriptSerializer j = new JavaScriptSerializer();
                Dictionary<string, object> a = (Dictionary<string, object>)j.Deserialize(response, typeof(Dictionary<string, object>));
                StringBuilder textBoxResponse = new StringBuilder();
                if (a.ContainsKey("stats"))
                {
                    this.importedUserList = (ArrayList)a["successes"];
                    ArrayList failuresList = (ArrayList)a["failures"];
                    for(int i=0; i < failuresList.Count; i++)
                    {
                        this.importedUserList.Add(failuresList[i]);
                    }                  
                    Dictionary<string, object> a2 = (Dictionary<string, object>)a["stats"];
                    string total = a2["total"].ToString();
                    string created = a2["created"].ToString();
                    string updated = a2["updated"].ToString();
                    string failure = a2["failed"].ToString();
                    var responseStr = string.Format("Total number of records {0}. Number of records created {1}. Number of records updated {2}. Number of failures {3}", total, created, updated, failure);
                    textBoxResponse.AppendLine(" Total number of users - " + total);
                    textBoxResponse.AppendLine(" Number of users created - " + created);
                    textBoxResponse.AppendLine(" Number of users updated - " + updated);
                    textBoxResponse.AppendLine(" Number of failures - " + failure);
                    textBoxResponse.AppendLine("");
                    if (a.ContainsKey("failureMessages"))
                    {
                        textBoxResponse.AppendLine("Reasons for failure:");
                        textBoxResponse.AppendLine("----------------------------");
                        ArrayList failureList = (ArrayList)a["failureMessages"];
                        for (int count = 0; count < failureList.Count; count++)
                        {
                            textBoxResponse.AppendLine(failureList[count].ToString());
                        }
                    }
                }
                else
                {
                    if (a["status"].ToString().Equals("error"))
                    {
                        textBoxResponse.AppendLine("Error - " + a["message"].ToString());
                    }
                    else
                    {
                        textBoxResponse.AppendLine(response);
                    }
                    
                }
                resultsPanel.Text = textBoxResponse.ToString();
                MessageBox.Show("Syncing of users from your ActiveDirectory instance to AlertMedia server has been completed. Please look at the response box for more details. Click on Next button to delete users from AlertMedia server");
                Cursor.Current = Cursors.Default;
                nextButton.Enabled = true;
            }
            catch (Exception ex)
            {
                using (StreamWriter w = File.AppendText("adsynclog.txt"))
                {
                    parentForm.Log(ex.ToString(), w);
                }
                MessageBox.Show("Not able to connect to AlertMedia servers. Please check you internet connection and try again. If problem persists, please get in touch with support.");
                Cursor.Current = Cursors.Default;
                return;
            }

        }

        public string[] SplitCSV(string input)
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

            return list.ToArray<string>();
        }
    }
}
