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
        private Panel configFieldBox = null;

        private string groupMappingType = "";

        private RadioButton ouButton, fvButton, noButton = null;
        private Button mapGroupsBT = null;

        private ArrayList groupList = null;

        private Hashtable userObject = null;

        private Label descLabel = null;
        private Panel fieldBox2 = null;
        private ComboBox fieldList = null;

        public GroupMappingPanel(Configuration config)
        {
            InitializeComponent();
            labels = new ArrayList();
            textBoxs = new ArrayList();

            this.config = config;
            amSection = (AppSettingsSection)config.Sections["AMSettings"];
            groupSection = (AppSettingsSection)config.Sections["GroupMappings"];

            Label typeLabel = new Label();
            typeLabel.Text = "Map user groups between ActiveDirectory and AlertMedia using either one of the below approaches";
            typeLabel.AutoSize = true;
            typeLabel.MaximumSize = new Size(600, 0);
            typeLabel.Location = new Point(170, 10);
            this.Controls.Add(typeLabel);

            ouButton = new RadioButton();
            ouButton.Text = "OU Mappings";
            ouButton.AutoSize = true;
            ouButton.Location = new Point(150, 25);
            this.Controls.Add(ouButton);
            ouButton.Click += new System.EventHandler(this.ouButton_Click);

            fvButton = new RadioButton();
            fvButton.Text = "Field Value Mappings";
            fvButton.Location = new Point(300, 25);
            fvButton.AutoSize = true;
            this.Controls.Add(fvButton);
            fvButton.Click += new System.EventHandler(this.fvButton_Click);

            noButton = new RadioButton();
            noButton.Text = "Skip Group Mapping";
            noButton.AutoSize = true;
            noButton.Location = new Point(450, 25);
            this.Controls.Add(noButton);
            noButton.Click += new System.EventHandler(this.noButton_Click);

            GroupBox lineBox = new GroupBox();
            lineBox.Text = "";
            lineBox.Width = 730;
            lineBox.Height = 1;
            lineBox.Paint += delegate (object o, PaintEventArgs p)
            {
                p.Graphics.Clear(Color.Gray);
            };
            lineBox.Location = new Point(5, 50);
            this.Controls.Add(lineBox);

            configFieldBox = new Panel();
            configFieldBox.AutoScroll = true;
            configFieldBox.Width = 700;
            configFieldBox.Height = 320;            
            configFieldBox.Location = new Point(25, 70);
            this.Controls.Add(configFieldBox);

            mapGroupsBT = new Button();
            mapGroupsBT.AutoSize = true;
            mapGroupsBT.Click += new System.EventHandler(this.mapsGroups_Button_Click);
            mapGroupsBT.Text = "Update";
            mapGroupsBT.Location = new Point(200, 400);
            this.Controls.Add(mapGroupsBT);

            Button prevBT = new Button();
            prevBT.AutoSize = true;
            prevBT.Click += new System.EventHandler(this.prev_Button_Click);
            prevBT.Text = "Back";
            prevBT.Location = new Point(300, 400);
            this.Controls.Add(prevBT);

            Button nextBT = new Button();
            nextBT.AutoSize = true;
            nextBT.Click += new System.EventHandler(this.next_Button_Click);
            nextBT.Text = "Next";
            nextBT.Location = new Point(400, 400);
            this.Controls.Add(nextBT);

            descLabel = new Label();
            descLabel.Text = "";
            descLabel.AutoSize = true;
            descLabel.MaximumSize = new Size(600, 0);
            descLabel.Location = new Point(25, 5);
            configFieldBox.Controls.Add(descLabel);

            this.fieldBox2 = new Panel();
            fieldBox2.AutoScroll = true;
            fieldBox2.Width = 225;
            fieldBox2.Height = 250;
            fieldBox2.Location = new Point(5, 50);
            configFieldBox.Controls.Add(fieldBox2);

            this.displayGroupMappingScreen();            

            Cursor.Current = Cursors.WaitCursor;
            try
            {
                client = new ActiveDirectoryClient(this.config);
                groupList = client.getActiveDirectoryUserGroups();
                userObject = client.userFieldMappings;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Not able to connect to your Active Directory Server. Please check you internet connection and try again. If problem persists, please contact your internal support team.");
                Cursor.Current = Cursors.Default;
                return;
            }
            Cursor.Current = Cursors.Default;

            this.groupMappingType = amSection.Settings["GroupMappingType"].Value;
            if (this.groupMappingType.Equals("OU"))
            {
                ouButton.Checked = true;
                ouButton.PerformClick();
            }
            else if (this.groupMappingType.Equals("FieldValue"))
            {
                fvButton.Checked = true;
                fvButton.PerformClick();
            }
            else if(this.groupMappingType.Equals(""))
            {
                noButton.Checked = true;
                noButton.PerformClick();
            }

        }

        private void displayGroupMappingScreen()
        {

            Panel fieldBox = new Panel();
            fieldBox.AutoScroll = true;
            fieldBox.Width = 400;
            fieldBox.Height = 250;
            fieldBox.Location = new Point(275, 50);

            int row = 10;
            foreach (string key in groupSection.Settings.AllKeys)
            {
                Label label = new Label();
                label.Text = key;
                label.AutoSize = true;
                label.Location = new Point(10, row);
                labels.Add(label);
                fieldBox.Controls.Add(label);

                TextBox textBox = new TextBox();
                textBox.Width = 100;
                textBox.Text = groupSection.Settings[key].Value;
                textBox.Location = new Point(250, row);
                textBoxs.Add(textBox);
                fieldBox.Controls.Add(textBox);
                row = row + 30;
            }

            configFieldBox.Controls.Add(fieldBox);            

        }

        private void ouMappingPanel()
        {

            descLabel.Text = "OU - The list on the right shows the Groups from AlertMedia." +
                " Please enter the corresponding Active Directory groups (including OU=) to map users" +
                " into those Groups during sync. Note If you make any changes on this screen" +
                " please click the \"UPDATE\" button";
            

            int row = 25;
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
                groupLabel.Width = 200;
                fieldBox2.Controls.Add(groupLabel);
                groupLabel.Location = new Point(5, row);
                row = row + groupLabel.Height + 7;
            }
        }

        private void fvMappingPanel()
        {            
            descLabel.Text = "FV - The list on the right shows the Groups from AlertMedia." +
                " Please enter the corresponding values for the selected field to map users" +
                " into those Groups during sync. Note If you make any changes on this screen" +
                " please click the \"UPDATE\" button";
            
            int row = 25;
            Label title = new Label();
            title.Text = "List of fields in AlertMedia User Object";
            title.Font = new Font(title.Font, FontStyle.Bold);
            title.AutoSize = true;
            title.Location = new Point(25, row);
            fieldBox2.Controls.Add(title);
            row = row + title.Height + 20;

            fieldList = new ComboBox();
            fieldList.Location = new Point(25, 75);
            fieldList.IntegralHeight = false;
            fieldList.MaxDropDownItems = 5;
            fieldList.DropDownStyle = ComboBoxStyle.DropDownList;
            fieldList.Size = new System.Drawing.Size(175, 81);
            List<string> keyList = userObject.Keys.Cast<string>().ToList();
            keyList.Sort();
            for (int i=0;i<keyList.Count;i++)
            {
                fieldList.Items.Add(keyList[i]);
            }
            string sItem = this.amSection.Settings["ADUserFieldForGroupMapping"].Value;
            fieldList.SelectedItem = sItem;
            fieldBox2.Controls.Add(fieldList);
        }

        private void noButton_Click(object sender, EventArgs e)
        {
            this.groupMappingType = "";
            fieldBox2.Controls.Clear();
            descLabel.Text = "Click on update button and then on next button to proceed";
            configFieldBox.Hide();
        }

        private void ouButton_Click(object sender, EventArgs e)
        {
            this.groupMappingType = "OU";
            fieldBox2.Controls.Clear();
            ouMappingPanel();
            configFieldBox.Show();
        }

        private void fvButton_Click(object sender, EventArgs e)
        {
            this.groupMappingType = "FieldValue";
            fieldBox2.Controls.Clear();
            fvMappingPanel();
            configFieldBox.Show();
        }
    
        private void mapsGroups_Button_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Object selectedItem = null;
            if (this.groupMappingType.Equals("FieldValue"))
            {
                if(fieldList != null)
                {
                    selectedItem = fieldList.SelectedItem;
                    if(selectedItem == null)
                    {
                        MessageBox.Show(" Kindly select Active Directory user object field to decide group mapping");
                        return;
                    }
                }
            }
            this.amSection.Settings["GroupMappingType"].Value = this.groupMappingType;
            if(this.groupMappingType.Equals("FieldValue"))
            {
                this.amSection.Settings["ADUserFieldForGroupMapping"].Value = selectedItem.ToString();
            }
            if (this.groupMappingType.Equals(""))
            {
                foreach (var tKey in groupSection.Settings.AllKeys)
                {
                    groupSection.Settings.Remove(tKey);
                }
            }
            else
            {
                Hashtable args = new Hashtable();
                args.Add("customer", amSection.Settings["customer"].Value);
                Hashtable groupsData = AlertMediaClient.Group.list(args);
                Group[] groups = (Group[])groupsData["data"];
                for (int i = 0; i < groups.Length; i++)
                {
                    groupSection.Settings.Remove(groups[i].id + "~" + groups[i].name);
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
