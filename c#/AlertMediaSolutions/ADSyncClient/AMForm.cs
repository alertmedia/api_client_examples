﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using alertmedia;
using alertmedia.activedirectory;
using System.Configuration;
using System.Collections;
using System.IO;

namespace ADSyncClient
{
    public partial class AMForm : Form
    {
        private int pageCount = 0;
        private Configuration config = null;
        public ArrayList importedUserList = null;
        private AppSettingsSection amSection = null;

        public AMForm()
        {
            InitializeComponent();
            config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            amSection = (AppSettingsSection)config.Sections["AMSettings"];
            this.MaximizeBox = false;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            displayPanel();
        }

        public void displayPanel()
        {
            bodyPanel.Controls.Clear();
            if(pageCount == 0)
            {
                ADFormPanel panel = new ADFormPanel(config); 
                panel.parentForm = this;
                panel.Width = 740;
                panel.Height = 440;
                panel.AutoScroll = true;
                bodyPanel.Controls.Add(panel);
            }
            else if(pageCount == 1)
            {                
                FieldMappingPanel panel = new FieldMappingPanel(config);
                try
                {
                    ActiveDirectoryClient client = new ActiveDirectoryClient(this.config);
                    panel.userObject = client.fetchUserFields(amSection.Settings["ExcludeDisableAccount"].Value);
                }
                catch(Exception ex)
                {
                    using (StreamWriter w = File.AppendText("adsynclog.txt"))
                    {
                        this.Log(ex.ToString(), w);
                    }
                    MessageBox.Show("Not able to connect to you Active Directory instance. Please check you internet connection and try again. If problem persists, please contact your internal support team.");
                    this.decremenPage();
                    this.displayPanel();
                    Cursor.Current = Cursors.Default;
                    return;
                }
                panel.drawActiveDirectoryFields();
                panel.parentForm = this;
                panel.Width = 740;
                panel.Height = 440;
                panel.AutoScroll = true;
                bodyPanel.Controls.Add(panel);
            }
            else if (pageCount == 2)
            {
                SyncUsersPanel panel = new SyncUsersPanel(config);
                panel.parentForm = this;
                panel.Width = 740;
                panel.Height = 440;
                panel.AutoScroll = true;
                bodyPanel.Controls.Add(panel);
            }            
            else if (pageCount == 3)
            {
                DeleteUserPanel panel = new DeleteUserPanel(config);
                panel.parentForm = this;
                panel.Width = 740;
                panel.Height = 440;
                panel.AutoScroll = true;
                panel.importedUserList = importedUserList;
                bodyPanel.Controls.Add(panel);
            }
        }

        public void incrementPage()
        {
            pageCount++;
        }

        public void decremenPage()
        {
            if (pageCount > 0)
            {
                pageCount--;
            }
        }

        public void Log(string logMessage, TextWriter logWriter)
        {
            logWriter.Write("\r\nLog Entry : ");
            logWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());
            logWriter.WriteLine("  :");
            logWriter.WriteLine("  :{0}", logMessage);
            logWriter.WriteLine("---------------------------------------------------------------------------------------");
        }

    }
}
