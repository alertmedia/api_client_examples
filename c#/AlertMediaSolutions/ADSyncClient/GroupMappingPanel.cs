using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using alertmedia;
using System.Configuration;
using alertmedia.activedirectory;

namespace ADSyncClient
{
    public partial class GroupMappingPanel : UserControl
    {
        public AMForm parentForm = null;

        private ArrayList labels;
        private ArrayList textBoxs;
        private Configuration config = null;
        private AppSettingsSection amSection, groupSection = null;

        private ActiveDirectoryClient client = null;

        public GroupMappingPanel(Configuration config)
        {
            InitializeComponent();
            labels = new ArrayList();
            textBoxs = new ArrayList();

            this.config = config;
            amSection = (AppSettingsSection)config.Sections["AMSettings"];
            groupSection = (AppSettingsSection)config.Sections["GroupMappings"];

            Label descLabel = new Label();
            descLabel.Text = "The list on the left shows the Groups from AlertMedia." +
                " Please enter the corresponding Active Directory groups (including OU=) to map users" +
                " into those Groups during sync. Note If you make any changes on this screen" +
                " please click the \"UPDATE\" button";
            descLabel.AutoSize = true;
            descLabel.MaximumSize = new Size(600, 0);
            descLabel.Location = new Point(25, 10);
            this.Controls.Add(descLabel);

            GroupBox groupBox = new GroupBox();
            groupBox.Text = "User Group Mappings";
            groupBox.Width = 400;
            groupBox.Height = 280;
            groupBox.Location = new Point(25, 40);

            Panel fieldBox = new Panel();
            fieldBox.AutoScroll = true;
            fieldBox.Width = 400;
            fieldBox.Height = 280;
            groupBox.Controls.Add(fieldBox);
            fieldBox.Location = new Point(25, 40);

            int row = 50;
            foreach (string key in groupSection.Settings.AllKeys)
            {
                Label label = new Label();
                label.Text = key;
                label.AutoSize = true;
                fieldBox.Controls.Add(label);
                label.Location = new Point(10, row);
                labels.Add(label);

                TextBox textBox = new TextBox();
                textBox.Width = 200;
                textBox.Text = groupSection.Settings[key].Value;
                fieldBox.Controls.Add(textBox);
                textBox.Location = new Point(160, row);
                textBoxs.Add(textBox);

                row = row + 30;
            }

            this.Controls.Add(fieldBox);

            Button mapGroupsBT = new Button();
            mapGroupsBT.AutoSize = true;
            mapGroupsBT.Click += new System.EventHandler(this.mapsGroups_Button_Click);
            mapGroupsBT.Text = "Update";
            mapGroupsBT.Location = new Point(50, 350);
            this.Controls.Add(mapGroupsBT);


            Button prevBT = new Button();
            prevBT.AutoSize = true;
            prevBT.Click += new System.EventHandler(this.prev_Button_Click);
            prevBT.Text = "Back";
            prevBT.Location = new Point(150, 350);
            this.Controls.Add(prevBT);

            Button nextBT = new Button();
            nextBT.AutoSize = true;
            nextBT.Click += new System.EventHandler(this.next_Button_Click);
            nextBT.Text = "Next";
            nextBT.Location = new Point(250, 350);
            this.Controls.Add(nextBT);
            
            ArrayList groupList = null;
            try
            {
                client = new ActiveDirectoryClient(this.config);
                groupList = client.getActiveDirectoryUserGroups();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Not able to connect to your Active Directory Server. Please check you internet connection and try again. If problem persists, please contact your internal support team.");
                parentForm.decremenPage();
                parentForm.displayPanel();
                Cursor.Current = Cursors.Default;
                return;
            }
            GroupBox groupBox2 = new GroupBox();
            groupBox2.Width = 275;
            groupBox2.Height = 300;
            groupBox2.Location = new Point(430, 40);

            Panel fieldBox2 = new Panel();
            fieldBox2.AutoScroll = true;
            fieldBox2.Width = 275;
            fieldBox2.Height = 300;
            groupBox2.Controls.Add(fieldBox2);
            fieldBox2.Location = new Point(430, 40);


            row = 25;

            Label title = new Label();
            title.Text = "List of Active Directory groups are";
            title.Font = new Font(title.Font, FontStyle.Bold);
            title.AutoSize = true;
            fieldBox2.Controls.Add(title);
            title.Location = new Point(25, row);
            row = row + title.Height + 20;

            for (int i = 0; i < groupList.Count; i++)
            {
                TextBox groupLabel = new TextBox();
                groupLabel.ReadOnly = true;
                groupLabel.BorderStyle = 0;
                groupLabel.BackColor = this.BackColor;
                groupLabel.TabStop = false;
                groupLabel.Text = groupList[i].ToString();
                groupLabel.Width = 250;
                fieldBox2.Controls.Add(groupLabel);
                groupLabel.Location = new Point(5, row);

                row = row + groupLabel.Height + 7;
            }

            this.Controls.Add(fieldBox2);
        }

        private void mapsGroups_Button_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Hashtable args = new Hashtable();
            args.Add("customer", amSection.Settings["customer"].Value);
            Hashtable groupsData = AlertMediaClient.Group.list(args);
            Group[] groups = (Group[]) groupsData["data"];
            for(int i=0; i<groups.Length; i++)
            {
                groupSection.Settings.Remove(groups[i].id+"~"+groups[i].name);
            }
            for (int i = 0; i < labels.Count; i++)
            {
                Label label = (Label)labels[i];
                string key = label.Text;
                TextBox textbox = (TextBox)textBoxs[i];
                string value = textbox.Text;                
                for (int j = 0; j < groups.Length; j++)
                {
                    if (key.Equals(groups[j].name))
                    {
                        key = groups[j].id + "~" + key;
                        break;
                    }
                }
                this.groupSection.Settings.Add(key, value);
            }
            config.Save();
            Cursor.Current = Cursors.Default;
            MessageBox.Show("Group mappings are completed. Click on Next button to continue");
        }

        private void prev_Button_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            parentForm.decremenPage();
            parentForm.displayPanel();
            Cursor.Current = Cursors.Default;           
        }

        private void next_Button_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            parentForm.incrementPage();
            parentForm.displayPanel();
            Cursor.Current = Cursors.Default;
        }
    }
}
