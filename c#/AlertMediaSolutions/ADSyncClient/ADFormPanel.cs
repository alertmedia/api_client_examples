using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using alertmedia;
using System.Collections;

namespace ADSyncClient
{
    public partial class ADFormPanel : UserControl
    {
        public AMForm parentForm = null;
        private Configuration config = null;
        private AppSettingsSection adSection, amSection, groupSection = null;
        private TextBox adServerURLTB, adUsernameTB, adPasswordTB, 
            amServerNameTB, amClientIdTB, amClientSecretTB = null;

        public ADFormPanel(Configuration config)
        {
            InitializeComponent();
            this.config = config; 
            adSection = (AppSettingsSection)config.Sections["ADSettings"];
            amSection = (AppSettingsSection)config.Sections["AMSettings"];
            groupSection = (AppSettingsSection)config.Sections["GroupMappings"];

            GroupBox groupBox1 = new GroupBox();
            groupBox1.Text = "Active Directory Configuration Details";
            groupBox1.Width = 300;
            groupBox1.Height = 150;
            groupBox1.AutoSize = true;
            groupBox1.Location = new Point(155, 25);

            Label adServerURL = new Label();
            adServerURL.Text = "Active Directory Server URL";
            adServerURL.AutoSize = true;
            groupBox1.Controls.Add(adServerURL);
            adServerURL.Location = new Point(10, 40);

            adServerURLTB = new TextBox();
            adServerURLTB.Width = 200;
            adServerURLTB.Text = adSection.Settings["LdapUrl"].Value;
            groupBox1.Controls.Add(adServerURLTB);
            adServerURLTB.Location = new Point(200, 40);

            Label adUsername = new Label();
            adUsername.Text = "Username";
            adUsername.AutoSize = true;
            groupBox1.Controls.Add(adUsername);
            adUsername.Location = new Point(10, 70);

            adUsernameTB = new TextBox();
            adUsernameTB.Width = 200;
            adUsernameTB.Text = adSection.Settings["UserName"].Value;
            groupBox1.Controls.Add(adUsernameTB);
            adUsernameTB.Location = new Point(200, 70);

            Label adPassword = new Label();
            adPassword.Text = "Password";
            adPassword.AutoSize = true;
            groupBox1.Controls.Add(adPassword);
            adPassword.Location = new Point(10, 100);

            adPasswordTB = new TextBox();
            adPasswordTB.PasswordChar = '*';
            adPasswordTB.Width = 200;
            adPasswordTB.Text = adSection.Settings["Password"].Value;
            groupBox1.Controls.Add(adPasswordTB);
            adPasswordTB.Location = new Point(200, 100);

            this.Controls.Add(groupBox1);
            
            GroupBox groupBox2 = new GroupBox();
            groupBox2.Text = "AlertMedia Server Configuration Details";
            groupBox2.Width = 300;
            groupBox2.Height = 150;
            groupBox2.AutoSize = true;
            groupBox2.Location = new Point(155, 200);

            Label amServerName = new Label();
            amServerName.Text = "Server Name";
            amServerName.AutoSize = true;
            groupBox2.Controls.Add(amServerName);
            amServerName.Location = new Point(10, 40);

            amServerNameTB = new TextBox();
            amServerNameTB.Width = 200;
            amServerNameTB.Text = amSection.Settings["BaseUrl"].Value;
            groupBox2.Controls.Add(amServerNameTB);
            amServerNameTB.Location = new Point(200, 40);

            Label amClientId = new Label();
            amClientId.Text = "Client ID";
            amClientId.AutoSize = true;
            groupBox2.Controls.Add(amClientId);
            amClientId.Location = new Point(10, 70);

            amClientIdTB = new TextBox();
            amClientIdTB.Width = 200;
            amClientIdTB.Text = amSection.Settings["ClientID"].Value;
            groupBox2.Controls.Add(amClientIdTB);
            amClientIdTB.Location = new Point(200, 70);

            Label amClienSecret = new Label();
            amClienSecret.Text = "Secret Key";
            amClienSecret.AutoSize = true;
            groupBox2.Controls.Add(amClienSecret);
            amClienSecret.Location = new Point(10, 100);

            amClientSecretTB = new TextBox();
            amClientSecretTB.Width = 200;
            amClientSecretTB.Text = amSection.Settings["ClientSecret"].Value;
            groupBox2.Controls.Add(amClientSecretTB);
            amClientSecretTB.Location = new Point(200, 100);

            this.Controls.Add(groupBox2);

            Button connectADBT = new Button();
            connectADBT.AutoSize = true;
            connectADBT.Click += new System.EventHandler(this.connectToAd_Button_Click);
            connectADBT.Text = "Connect to AD";
            this.Controls.Add(connectADBT);
            connectADBT.Location = new Point(325, 375);

        }

        private void connectToAd_Button_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            adSection.Settings["LdapURL"].Value = adServerURLTB.Text;
            adSection.Settings["Username"].Value = adUsernameTB.Text;
            adSection.Settings["Password"].Value = adPasswordTB.Text;
            amSection.Settings["BaseUrl"].Value = amServerNameTB.Text;
            amSection.Settings["ClientId"].Value = amClientIdTB.Text;
            amSection.Settings["ClientSecret"].Value = amClientSecretTB.Text;
            

            AlertMediaClient.setBaseUrl(amServerNameTB.Text);
            AlertMediaClient.setClientId(amClientIdTB.Text);
            AlertMediaClient.setClientKey(amClientSecretTB.Text);
            try
            {
                amSection.Settings["Customer"].Value = AlertMediaClient.Customer.getCustomerId();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Not able to connect to AlertMedia servers. Please check you internet connection and try again. If problem persists, please get in touch with support.");
                Cursor.Current = Cursors.Default;
                return;
            }
            Hashtable args = new Hashtable();
            args.Add("customer", amSection.Settings["customer"].Value);
            Hashtable returnObject = null;
            try
            {
                returnObject = AlertMediaClient.Group.list(args);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Not able to connect to AlertMedia servers. Please check you internet connection and try again. If problem persists, please get in touch with support.");
                Cursor.Current = Cursors.Default;
                return;
            }
            groupSection.Settings.Clear();
            /*
            Group[] groups = (Group[])returnObject["data"];
            for (int i = 0; i < groups.Length; i++)
            {
                if (!(groups[i].name.Equals("Everyone") || groups[i].name.Equals("Admins")))
                {
                    string groupName = groups[i].id + "~" + groups[i].name;
                    groupSection.Settings.Add(new KeyValueConfigurationElement(groupName, ""));                    
                }
            }
            config.Save(ConfigurationSaveMode.Minimal);
            */
            config.Save(ConfigurationSaveMode.Minimal);
            parentForm.incrementPage();
            parentForm.displayPanel();
            Cursor.Current = Cursors.Default;
        }
    }
}
