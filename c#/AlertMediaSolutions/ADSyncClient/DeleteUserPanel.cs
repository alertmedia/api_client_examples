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

namespace ADSyncClient
{
    public partial class DeleteUserPanel : UserControl
    {
        public AMForm parentForm = null;
        private Configuration config = null;
        private Button prevButton, previewButton, deleteButton;
        private WebBrowser tablePanel = null;
        private TextBox resultsPanel = null;
        private AppSettingsSection amSection = null;

        private ActiveDirectoryClient client = null;
        public ArrayList importedUserList;
        private ArrayList deletedUserIdList = new ArrayList();

        public DeleteUserPanel(Configuration config)
        {
            InitializeComponent();
            this.config = config;
            amSection = (AppSettingsSection)config.Sections["AMSettings"];
            Label l1 = new Label();
            l1.AutoSize = true;
            l1.Text = "To delete users from AlertMedia, click on list users";
            l1.Location = new Point(200, 25);
            this.Controls.Add(l1);

            prevButton = new Button();
            prevButton.Text = "Back";
            prevButton.Click += new System.EventHandler(this.prevButton_Click);
            prevButton.Location = new Point(150, 60);
            this.Controls.Add(prevButton);

            previewButton = new Button();
            previewButton.Text = "List Users";
            previewButton.Click += new System.EventHandler(this.previewButton_Click);
            previewButton.Location = new Point(300, 60);
            this.Controls.Add(previewButton);

            deleteButton = new Button();
            deleteButton.Text = "Delete Users";
            deleteButton.Click += new System.EventHandler(this.deleteUsersButton_Click);
            deleteButton.Location = new Point(450, 60);
            this.Controls.Add(deleteButton);
            deleteButton.Enabled = false;
            
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

            resultsPanel.Text = "Results of delete users from AlertMedia will be displayed here";
        }

        private void prevButton_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            parentForm.decremenPage();
            parentForm.displayPanel();
            Cursor.Current = Cursors.Default;
        }
        
        private void previewButton_Click(object sender, EventArgs e)
        {
            resultsPanel.Text = "Results of delete users from AlertMedia will be displayed here";
            this.Controls.Remove(tablePanel);
            this.Controls.Add(tablePanel);
            StringBuilder dataStr = new StringBuilder();
            Cursor.Current = Cursors.WaitCursor;
            string[] csvStr;
            try
            {
                client = new ActiveDirectoryClient(this.config);
                csvStr = client.previewDeleteUserList(importedUserList);
                string htmlContent = "<html><body style=\"padding: 0; margin: 0;\"><table>";
                htmlContent = htmlContent + "<tr>";
                dataStr.AppendLine(csvStr[0]);
                string[] headers = this.SplitCSV(csvStr[0]);
                for (int i = 0; i < headers.Length; i++)
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
                        if (j == 0)
                        {
                            deletedUserIdList.Add(value);
                        }
                        if (value == "")
                        {
                            value = "&nbsp;";
                        }
                        htmlContent = htmlContent + "<td style=\"font-family: sans-serif; font-size: 10px; border: solid #DEDEDE 1.0pt; mso-border-alt: solid #DEDEDE .75pt; padding: 0cm 0cm 0cm 0cm; height: 10.75pt;\" >" + value + "</td>";
                    }
                    htmlContent = htmlContent + "</tr>";
                }
                htmlContent = htmlContent + "</table></body></html>";
                tablePanel.DocumentText = htmlContent;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Not able to connect to you Active Directory instance. Please check you internet connection and try again. If problem persists, please contact your internal support team.");
                Cursor.Current = Cursors.Default;
                return;
            }
            MessageBox.Show("Click on Delete Users button to delete all users listed in preview box");
            deleteButton.Enabled = true;
            Cursor.Current = Cursors.Default;
        }

        private void deleteUsersButton_Click(object sender, EventArgs e)
        {

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                client.deleteUsersFromAlertMedia(deletedUserIdList);
                resultsPanel.Text = "All users in the preview list above have been deleted";
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
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
