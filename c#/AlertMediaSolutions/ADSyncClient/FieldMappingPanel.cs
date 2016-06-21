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

namespace ADSyncClient
{
    public partial class FieldMappingPanel : UserControl
    {
        public AMForm parentForm = null;

        public SortedDictionary<string, string> userObject = null;

        private Label firstNameLB, lastNameFB, emailFB, titleFB, email2LB, mobilePhoneLB, mobilePhone2LB, homePhoneLB,
            officePhoneLB, addressLB, address2LB, zipcodeLB, cityLB, stateLB, countryLB, custom1LB, custom2LB, custom3LB,
            notesLB, outboundNumberLB, customerUserIdLB = null;

        private TextBox firstNameTB, lastNameTB, emailTB, titleTB, email2TB, mobilePhoneTB, mobilePhone2TB, homePhoneTB,
            officePhoneTB, addressTB, address2TB, zipcodeTB, cityTB, stateTB, countryTB, custom1TB, custom2TB, custom3TB,
            notesTB, outboundNumberTB, customerUserIdTB = null;

        private Label firstNameValue, lastNameValue, emailValue, titleValue, email2Value, mobilePhoneValue, mobilePhone2Value, homePhoneValue,
            officePhoneValue, addressValue, address2Value, zipcodeValue, cityValue, stateValue, countryValue, custom1Value, custom2Value, custom3Value,
            notesValue, outboundNumberValue, customerUserIdValue = null;

        private Button mapFieldsBT, prevBT, nextBT = null;

        private Configuration config = null;
        private AppSettingsSection fieldMappingsSection = null;

        private int labelLocation, textLocation, valueLocation;

        public FieldMappingPanel(Configuration config)
        {
            InitializeComponent();
            labelLocation = 10;
            textLocation = 125;
            valueLocation = 225;

            this.config = config;
            fieldMappingsSection = (AppSettingsSection)config.Sections["FieldMappings"];

            Label descLabel = new Label();
            descLabel.Text = "The list on the left shows the User fields from AlertMedia. " +
                " Please enter the corresponding Active Directory field name that contains" +
                " the proper value. You may use field names tha are no listed on he right side." +
                " NOTE: If you make any changes on this screen plase click the UPDATE button";
            descLabel.AutoSize = true;
            descLabel.MaximumSize = new Size(600, 0);
            descLabel.Location = new Point(25, 10);
            this.Controls.Add(descLabel);

            GroupBox groupBox = new GroupBox();
            groupBox.Width = 350;
            groupBox.Height = 750;
            groupBox.Location = new Point(25, 25);

            Panel fieldBox = new Panel();
            fieldBox.AutoScroll = true;
            fieldBox.Width = 350;
            fieldBox.Height = 750;
            groupBox.Controls.Add(fieldBox);
            fieldBox.Location = new Point(25, 25);

            firstNameLB = new Label();
            firstNameLB.Text = "first_name";
            firstNameLB.AutoSize = true;
            fieldBox.Controls.Add(firstNameLB);
            firstNameLB.Location = new Point(labelLocation, 50);

            firstNameTB = new TextBox();
            firstNameTB.Width = 100;
            firstNameTB.Text = fieldMappingsSection.Settings["first_name"].Value;
            fieldBox.Controls.Add(firstNameTB);
            firstNameTB.Location = new Point(textLocation, 50);

            firstNameValue = new Label();
            firstNameValue.ForeColor = System.Drawing.Color.Red;
            firstNameValue.AutoSize = true;
            fieldBox.Controls.Add(firstNameValue);
            firstNameValue.Location = new Point(valueLocation, 50);

            lastNameFB = new Label();
            lastNameFB.Text = "last_name";
            lastNameFB.AutoSize = true;
            fieldBox.Controls.Add(lastNameFB);
            lastNameFB.Location = new Point(labelLocation, 80);

            lastNameTB = new TextBox();
            lastNameTB.Width = 100;
            lastNameTB.Location = new Point(textLocation, 80);
            lastNameTB.Text = fieldMappingsSection.Settings["last_name"].Value;
            fieldBox.Controls.Add(lastNameTB);

            lastNameValue = new Label();
            lastNameValue.ForeColor = System.Drawing.Color.Red;
            lastNameValue.AutoSize = true;
            fieldBox.Controls.Add(lastNameValue);
            lastNameValue.Location = new Point(valueLocation, 80);

            emailFB = new Label();
            emailFB.AutoSize = true;
            emailFB.Text = "email";
            fieldBox.Controls.Add(emailFB);
            emailFB.Location = new Point(labelLocation, 110);

            emailTB = new TextBox();
            emailTB.Width = 100;
            emailTB.Text = fieldMappingsSection.Settings["email"].Value;
            fieldBox.Controls.Add(emailTB);
            emailTB.Location = new Point(textLocation, 110);

            emailValue = new Label();
            emailValue.ForeColor = System.Drawing.Color.Red;
            emailValue.AutoSize = true;
            fieldBox.Controls.Add(emailValue);
            emailValue.Location = new Point(valueLocation, 110);


            titleFB = new Label();
            titleFB.AutoSize = true;
            titleFB.Text = "title";
            fieldBox.Controls.Add(titleFB);
            titleFB.Location = new Point(labelLocation, 140);

            titleTB = new TextBox();
            titleTB.Width = 100;
            titleTB.Text = fieldMappingsSection.Settings["title"].Value;
            fieldBox.Controls.Add(titleTB);
            titleTB.Location = new Point(textLocation, 140);

            titleValue = new Label();
            titleValue.ForeColor = System.Drawing.Color.Red;
            titleValue.AutoSize = true;
            fieldBox.Controls.Add(titleValue);
            titleValue.Location = new Point(valueLocation, 140);

            email2LB = new Label();
            email2LB.AutoSize = true;
            email2LB.Text = "email2";
            fieldBox.Controls.Add(email2LB);
            email2LB.Location = new Point(labelLocation, 170);


            email2TB = new TextBox();
            email2TB.Width = 100;
            email2TB.Text = fieldMappingsSection.Settings["email2"].Value;
            fieldBox.Controls.Add(email2TB);
            email2TB.Location = new Point(textLocation, 170);

            email2Value = new Label();
            email2Value.ForeColor = System.Drawing.Color.Red;
            email2Value.AutoSize = true;
            fieldBox.Controls.Add(email2Value);
            email2Value.Location = new Point(valueLocation, 170);


            mobilePhoneLB = new Label();
            mobilePhoneLB.AutoSize = true;
            mobilePhoneLB.Text = "mobile_phone";
            fieldBox.Controls.Add(mobilePhoneLB);
            mobilePhoneLB.Location = new Point(labelLocation, 200);

            mobilePhoneTB = new TextBox();
            mobilePhoneTB.Width = 100;
            mobilePhoneTB.Text = fieldMappingsSection.Settings["mobile_phone"].Value;
            fieldBox.Controls.Add(mobilePhoneTB);
            mobilePhoneTB.Location = new Point(textLocation, 200);

            mobilePhoneValue = new Label();
            mobilePhoneValue.ForeColor = System.Drawing.Color.Red;
            mobilePhoneValue.AutoSize = true;
            fieldBox.Controls.Add(mobilePhoneValue);
            mobilePhoneValue.Location = new Point(valueLocation, 200);


            mobilePhone2LB = new Label();
            mobilePhone2LB.AutoSize = true;
            mobilePhone2LB.Text = "mobile_phone2";
            fieldBox.Controls.Add(mobilePhone2LB);
            mobilePhone2LB.Location = new Point(labelLocation, 230);

            mobilePhone2TB = new TextBox();
            mobilePhone2TB.Width = 100;
            mobilePhone2TB.Text = fieldMappingsSection.Settings["mobile_phone2"].Value;
            fieldBox.Controls.Add(mobilePhone2TB);
            mobilePhone2TB.Location = new Point(textLocation, 230);

            mobilePhone2Value = new Label();
            mobilePhone2Value.ForeColor = System.Drawing.Color.Red;
            mobilePhone2Value.AutoSize = true;
            fieldBox.Controls.Add(mobilePhone2Value);
            mobilePhone2Value.Location = new Point(valueLocation, 230);

            homePhoneLB = new Label();
            homePhoneLB.AutoSize = true;
            homePhoneLB.Text = "home_phone";
            fieldBox.Controls.Add(homePhoneLB);
            homePhoneLB.Location = new Point(labelLocation, 260);

            homePhoneTB = new TextBox();
            homePhoneTB.Width = 100;
            homePhoneTB.Text = fieldMappingsSection.Settings["home_phone"].Value;
            fieldBox.Controls.Add(homePhoneTB);
            homePhoneTB.Location = new Point(textLocation, 260);

            homePhoneValue = new Label();
            homePhoneValue.ForeColor = System.Drawing.Color.Red;
            homePhoneValue.AutoSize = true;
            fieldBox.Controls.Add(homePhoneValue);
            homePhoneValue.Location = new Point(valueLocation, 260);


            officePhoneLB = new Label();
            officePhoneLB.AutoSize = true;
            officePhoneLB.Text = "office_phone";
            fieldBox.Controls.Add(officePhoneLB);
            officePhoneLB.Location = new Point(labelLocation, 290);

            officePhoneTB = new TextBox();
            officePhoneTB.Width = 100;
            officePhoneTB.Text = fieldMappingsSection.Settings["office_phone"].Value;
            fieldBox.Controls.Add(officePhoneTB);
            officePhoneTB.Location = new Point(textLocation, 290);

            officePhoneValue = new Label();
            officePhoneValue.ForeColor = System.Drawing.Color.Red;
            officePhoneValue.AutoSize = true;
            fieldBox.Controls.Add(officePhoneValue);
            officePhoneValue.Location = new Point(valueLocation, 290);


            addressLB = new Label();
            addressLB.AutoSize = true;
            addressLB.Text = "address";
            fieldBox.Controls.Add(addressLB);
            addressLB.Location = new Point(labelLocation, 320);

            addressTB = new TextBox();
            addressTB.Width = 100;
            addressTB.Text = fieldMappingsSection.Settings["address"].Value;
            fieldBox.Controls.Add(addressTB);
            addressTB.Location = new Point(textLocation, 320);

            addressValue = new Label();
            addressValue.ForeColor = System.Drawing.Color.Red;
            addressValue.AutoSize = true;
            fieldBox.Controls.Add(addressValue);
            addressValue.Location = new Point(valueLocation, 320);


            address2LB = new Label();
            address2LB.AutoSize = true;
            address2LB.Text = "address2";
            fieldBox.Controls.Add(address2LB);
            address2LB.Location = new Point(labelLocation, 350);

            address2TB = new TextBox();
            address2TB.Width = 100;
            address2TB.Text = fieldMappingsSection.Settings["address2"].Value;
            fieldBox.Controls.Add(address2TB);
            address2TB.Location = new Point(textLocation, 350);

            address2Value = new Label();
            address2Value.ForeColor = System.Drawing.Color.Red;
            address2Value.AutoSize = true;
            fieldBox.Controls.Add(address2Value);
            address2Value.Location = new Point(valueLocation, 350);

            cityLB = new Label();
            cityLB.AutoSize = true;
            cityLB.Text = "city";
            fieldBox.Controls.Add(cityLB);
            cityLB.Location = new Point(labelLocation, 380);

            cityTB = new TextBox();
            cityTB.Width = 100;
            cityTB.Text = fieldMappingsSection.Settings["city"].Value;
            fieldBox.Controls.Add(cityTB);
            cityTB.Location = new Point(textLocation, 380);

            cityValue = new Label();
            cityValue.ForeColor = System.Drawing.Color.Red;
            cityValue.AutoSize = true;
            fieldBox.Controls.Add(cityValue);
            cityValue.Location = new Point(valueLocation, 380);

            stateLB = new Label();
            stateLB.AutoSize = true;
            stateLB.Text = "state";
            fieldBox.Controls.Add(stateLB);
            stateLB.Location = new Point(labelLocation, 410);

            stateTB = new TextBox();
            stateTB.Width = 100;
            stateTB.Text = fieldMappingsSection.Settings["state"].Value;
            fieldBox.Controls.Add(stateTB);
            stateTB.Location = new Point(textLocation, 410);

            stateValue = new Label();
            stateValue.ForeColor = System.Drawing.Color.Red;
            stateValue.AutoSize = true;
            fieldBox.Controls.Add(stateValue);
            stateValue.Location = new Point(valueLocation, 410);


            countryLB = new Label();
            countryLB.AutoSize = true;
            countryLB.Text = "country";
            fieldBox.Controls.Add(countryLB);
            countryLB.Location = new Point(labelLocation, 440);

            countryTB = new TextBox();
            countryTB.Width = 100;
            countryTB.Text = fieldMappingsSection.Settings["country"].Value;
            fieldBox.Controls.Add(countryTB);
            countryTB.Location = new Point(textLocation, 440);

            countryValue = new Label();
            countryValue.ForeColor = System.Drawing.Color.Red;
            countryValue.AutoSize = true;
            fieldBox.Controls.Add(countryValue);
            countryValue.Location = new Point(valueLocation, 440);


            zipcodeLB = new Label();
            zipcodeLB.AutoSize = true;
            zipcodeLB.Text = "zipcode";
            fieldBox.Controls.Add(zipcodeLB);
            zipcodeLB.Location = new Point(labelLocation, 470);

            zipcodeTB = new TextBox();
            zipcodeTB.Width = 100;
            zipcodeTB.Text = fieldMappingsSection.Settings["zipcode"].Value;
            fieldBox.Controls.Add(zipcodeTB);
            zipcodeTB.Location = new Point(textLocation, 470);

            zipcodeValue = new Label();
            zipcodeValue.ForeColor = System.Drawing.Color.Red;
            zipcodeValue.AutoSize = true;
            fieldBox.Controls.Add(zipcodeValue);
            zipcodeValue.Location = new Point(valueLocation, 470);

            custom1LB = new Label();
            custom1LB.AutoSize = true;
            custom1LB.Text = "custom1";
            fieldBox.Controls.Add(custom1LB);
            custom1LB.Location = new Point(labelLocation, 500);

            custom1TB = new TextBox();
            custom1TB.Width = 100;
            custom1TB.Text = fieldMappingsSection.Settings["custom1"].Value;
            fieldBox.Controls.Add(custom1TB);
            custom1TB.Location = new Point(textLocation, 500);

            custom1Value = new Label();
            custom1Value.ForeColor = System.Drawing.Color.Red;
            custom1Value.AutoSize = true;
            fieldBox.Controls.Add(custom1Value);
            custom1Value.Location = new Point(valueLocation, 500);

            custom2LB = new Label();
            custom2LB.AutoSize = true;
            custom2LB.Text = "custom2";
            fieldBox.Controls.Add(custom2LB);
            custom2LB.Location = new Point(labelLocation, 530);

            custom2TB = new TextBox();
            custom2TB.Width = 100;
            custom2TB.Text = fieldMappingsSection.Settings["custom2"].Value;
            fieldBox.Controls.Add(custom2TB);
            custom2TB.Location = new Point(textLocation, 530);

            custom2Value = new Label();
            custom2Value.ForeColor = System.Drawing.Color.Red;
            custom2Value.AutoSize = true;
            fieldBox.Controls.Add(custom2Value);
            custom2Value.Location = new Point(valueLocation, 530);

            custom3LB = new Label();
            custom3LB.Visible = true;
            custom3LB.Text = "custom3";
            fieldBox.Controls.Add(custom3LB);
            custom3LB.Location = new Point(labelLocation, 560);

            custom3TB = new TextBox();
            custom3TB.Width = 100;
            custom3TB.Text = fieldMappingsSection.Settings["custom3"].Value;
            fieldBox.Controls.Add(custom3TB);
            custom3TB.Location = new Point(textLocation, 560);

            custom3Value = new Label();
            custom3Value.ForeColor = System.Drawing.Color.Red;
            custom3Value.AutoSize = true;
            fieldBox.Controls.Add(custom3Value);
            custom3Value.Location = new Point(valueLocation, 560);

            notesLB = new Label();
            notesLB.Visible = true;
            notesLB.Text = "notes";
            fieldBox.Controls.Add(notesLB);
            notesLB.Location = new Point(labelLocation, 590);

            notesTB = new TextBox();
            notesTB.Width = 100;
            notesTB.Text = fieldMappingsSection.Settings["notes"].Value;
            fieldBox.Controls.Add(notesTB);
            notesTB.Location = new Point(textLocation, 590);

            notesValue = new Label();
            notesValue.ForeColor = System.Drawing.Color.Red;
            notesValue.AutoSize = true;
            fieldBox.Controls.Add(notesValue);
            notesValue.Location = new Point(valueLocation, 590);

            outboundNumberLB = new Label();
            outboundNumberLB.AutoSize = true;
            outboundNumberLB.Text = "outbound_number";
            fieldBox.Controls.Add(outboundNumberLB);
            outboundNumberLB.Location = new Point(labelLocation, 620);

            outboundNumberTB = new TextBox();
            outboundNumberTB.Width = 100;
            outboundNumberTB.Text = fieldMappingsSection.Settings["outbound_number"].Value;
            fieldBox.Controls.Add(outboundNumberTB);
            outboundNumberTB.Location = new Point(textLocation, 620);

            outboundNumberValue = new Label();
            outboundNumberValue.ForeColor = System.Drawing.Color.Red;
            outboundNumberValue.AutoSize = true;
            fieldBox.Controls.Add(outboundNumberValue);
            outboundNumberValue.Location = new Point(valueLocation, 620);

            customerUserIdLB = new Label();
            customerUserIdLB.Visible = true;
            customerUserIdLB.Text = "customer_user_id";
            fieldBox.Controls.Add(customerUserIdLB);
            customerUserIdLB.Location = new Point(labelLocation, 650);

            customerUserIdTB = new TextBox();
            customerUserIdTB.Width = 100;
            customerUserIdTB.Text = fieldMappingsSection.Settings["customer_user_id"].Value;
            fieldBox.Controls.Add(customerUserIdTB);
            customerUserIdTB.Location = new Point(textLocation, 650);

            customerUserIdValue = new Label();
            customerUserIdValue.ForeColor = System.Drawing.Color.Red;
            customerUserIdValue.AutoSize = true;
            fieldBox.Controls.Add(customerUserIdValue);
            customerUserIdValue.Location = new Point(valueLocation, 650);

            mapFieldsBT = new Button();
            mapFieldsBT.AutoSize = true;
            mapFieldsBT.Click += new System.EventHandler(this.mapsField_Button_Click);
            mapFieldsBT.Text = "Update";
            fieldBox.Controls.Add(mapFieldsBT);
            mapFieldsBT.Location = new Point(10, 700);

            prevBT = new Button();
            prevBT.AutoSize = true;
            prevBT.Click += new System.EventHandler(this.prev_Button_Click);
            prevBT.Text = "Back";
            fieldBox.Controls.Add(prevBT);
            prevBT.Location = new Point(100, 700);

            nextBT = new Button();
            nextBT.AutoSize = true;
            nextBT.Click += new System.EventHandler(this.next_Button_Click);
            nextBT.Text = "Next";
            fieldBox.Controls.Add(nextBT);
            nextBT.Location = new Point(190, 700);


            this.Controls.Add(fieldBox);

        }

        public void drawActiveDirectoryFields()
        {
            GroupBox groupBox = new GroupBox();
            groupBox.Width = 300;
            groupBox.Height = 950;
            groupBox.Location = new Point(375, 25);

            Panel fieldBox = new Panel();
            fieldBox.Width = 300;
            fieldBox.Height = 950;
            fieldBox.AutoScroll = true;
            groupBox.Controls.Add(fieldBox);
            fieldBox.Location = new Point(375, 25);

            int row = 25;

            Label title = new Label();            
            title.Text = "Active Directory User Fields";
            title.Font = new Font(title.Font, FontStyle.Bold);
            title.AutoSize = true;
            fieldBox.Controls.Add(title);
            title.Location = new Point(25, row);
            row = row + title.Height + 20;

            foreach (var obj in userObject)
            {
                TextBox adFieldName = new TextBox();
                adFieldName.ReadOnly = true;
                adFieldName.BorderStyle = 0;
                adFieldName.BackColor = this.BackColor;
                adFieldName.TabStop = false;
                adFieldName.Font = new Font(adFieldName.Font, FontStyle.Bold);
                adFieldName.Text = obj.Key + " :";
                adFieldName.AutoSize = true;
                fieldBox.Controls.Add(adFieldName);
                adFieldName.Location = new Point(10, row);

                TextBox adFieldValue = new TextBox();
                adFieldValue.ReadOnly = true;
                adFieldValue.BorderStyle = 0;
                adFieldValue.BackColor = this.BackColor;
                adFieldValue.TabStop = false;
                adFieldValue.Text = obj.Value;
                adFieldValue.AutoSize = true;
                fieldBox.Controls.Add(adFieldValue);
                adFieldValue.Location = new Point(150, row);

                row = row + adFieldName.Height + 7;
            }

            this.Controls.Add(fieldBox);

            this.refreshData();

        }

        public void refreshData()
        {

            firstNameTB.Text = fieldMappingsSection.Settings["first_name"].Value;
            lastNameTB.Text = fieldMappingsSection.Settings["last_name"].Value;
            emailTB.Text = fieldMappingsSection.Settings["email"].Value;
            titleTB.Text = fieldMappingsSection.Settings["title"].Value;
            email2TB.Text = fieldMappingsSection.Settings["email2"].Value;
            mobilePhoneTB.Text = fieldMappingsSection.Settings["mobile_phone"].Value;
            mobilePhone2TB.Text = fieldMappingsSection.Settings["mobile_phone2"].Value;
            homePhoneTB.Text = fieldMappingsSection.Settings["home_phone"].Value;
            officePhoneTB.Text = fieldMappingsSection.Settings["office_phone"].Value;
            addressTB.Text = fieldMappingsSection.Settings["address"].Value;
            address2TB.Text = fieldMappingsSection.Settings["address2"].Value;
            cityTB.Text = fieldMappingsSection.Settings["city"].Value;
            stateTB.Text = fieldMappingsSection.Settings["state"].Value;
            countryTB.Text = fieldMappingsSection.Settings["country"].Value;
            zipcodeTB.Text = fieldMappingsSection.Settings["zipcode"].Value;
            custom1TB.Text = fieldMappingsSection.Settings["custom1"].Value;
            custom2TB.Text = fieldMappingsSection.Settings["custom2"].Value;
            custom3TB.Text = fieldMappingsSection.Settings["custom3"].Value;
            notesTB.Text = fieldMappingsSection.Settings["notes"].Value;
            outboundNumberTB.Text = fieldMappingsSection.Settings["outbound_number"].Value;
            customerUserIdTB.Text = fieldMappingsSection.Settings["customer_user_id"].Value;


            if (userObject.ContainsKey(fieldMappingsSection.Settings["first_name"].Value))
            {
                firstNameValue.Text = userObject[fieldMappingsSection.Settings["first_name"].Value];
            }

            if (userObject.ContainsKey(fieldMappingsSection.Settings["last_name"].Value))
            {
                lastNameValue.Text = userObject[fieldMappingsSection.Settings["last_name"].Value];
            }

            if (userObject.ContainsKey(fieldMappingsSection.Settings["email"].Value))
            {
                emailValue.Text = userObject[fieldMappingsSection.Settings["email"].Value];
            }

            if (userObject.ContainsKey(fieldMappingsSection.Settings["title"].Value))
            {
                titleValue.Text = userObject[fieldMappingsSection.Settings["title"].Value];
            }

            if (userObject.ContainsKey(fieldMappingsSection.Settings["email2"].Value))
            {
                email2Value.Text = userObject[fieldMappingsSection.Settings["email2"].Value];
            }

            if (userObject.ContainsKey(fieldMappingsSection.Settings["mobile_phone"].Value))
            {
                mobilePhoneValue.Text = userObject[fieldMappingsSection.Settings["mobile_phone"].Value];
            }

            if (userObject.ContainsKey(fieldMappingsSection.Settings["mobile_phone2"].Value))
            {
                mobilePhone2Value.Text = userObject[fieldMappingsSection.Settings["mobile_phone2"].Value];
            }

            if (userObject.ContainsKey(fieldMappingsSection.Settings["home_phone"].Value))
            {
                homePhoneValue.Text = userObject[fieldMappingsSection.Settings["home_phone"].Value];
            }

            if (userObject.ContainsKey(fieldMappingsSection.Settings["office_phone"].Value))
            {
                officePhoneValue.Text = userObject[fieldMappingsSection.Settings["office_phone"].Value];
            }

            if (userObject.ContainsKey(fieldMappingsSection.Settings["address"].Value))
            {
                addressValue.Text = userObject[fieldMappingsSection.Settings["address"].Value];
            }

            if (userObject.ContainsKey(fieldMappingsSection.Settings["address2"].Value))
            {
                address2Value.Text = userObject[fieldMappingsSection.Settings["address2"].Value];
            }

            if (userObject.ContainsKey(fieldMappingsSection.Settings["city"].Value))
            {
                cityValue.Text = userObject[fieldMappingsSection.Settings["city"].Value];
            }

            if (userObject.ContainsKey(fieldMappingsSection.Settings["state"].Value))
            {
                stateValue.Text = userObject[fieldMappingsSection.Settings["state"].Value];
            }

            if (userObject.ContainsKey(fieldMappingsSection.Settings["country"].Value))
            {
                countryValue.Text = userObject[fieldMappingsSection.Settings["country"].Value];
            }

            if (userObject.ContainsKey(fieldMappingsSection.Settings["zipcode"].Value))
            {
                zipcodeValue.Text = userObject[fieldMappingsSection.Settings["zipcode"].Value];
            }

            if (userObject.ContainsKey(fieldMappingsSection.Settings["custom1"].Value))
            {
                custom1Value.Text = userObject[fieldMappingsSection.Settings["custom1"].Value];
            }

            if (userObject.ContainsKey(fieldMappingsSection.Settings["custom2"].Value))
            {
                custom2Value.Text = userObject[fieldMappingsSection.Settings["custom2"].Value];
            }

            if (userObject.ContainsKey(fieldMappingsSection.Settings["custom3"].Value))
            {
                custom3Value.Text = userObject[fieldMappingsSection.Settings["custom3"].Value];
            }

            if (userObject.ContainsKey(fieldMappingsSection.Settings["notes"].Value))
            {
                notesValue.Text = userObject[fieldMappingsSection.Settings["notes"].Value];
            }

            if (userObject.ContainsKey(fieldMappingsSection.Settings["outbound_number"].Value))
            {
                outboundNumberValue.Text = userObject[fieldMappingsSection.Settings["outbound_number"].Value];
            }

            if (userObject.ContainsKey(fieldMappingsSection.Settings["customer_user_id"].Value))
            {
                customerUserIdValue.Text = userObject[fieldMappingsSection.Settings["customer_user_id"].Value];
            }
        }

        private void mapsField_Button_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            fieldMappingsSection.Settings["first_name"].Value = firstNameTB.Text;
            fieldMappingsSection.Settings["last_name"].Value = lastNameTB.Text;
            fieldMappingsSection.Settings["email"].Value = emailTB.Text;
            fieldMappingsSection.Settings["title"].Value = titleTB.Text;
            fieldMappingsSection.Settings["email2"].Value = email2TB.Text;
            fieldMappingsSection.Settings["mobile_phone"].Value = mobilePhoneTB.Text;
            fieldMappingsSection.Settings["mobile_phone2"].Value = mobilePhone2TB.Text;
            fieldMappingsSection.Settings["home_phone"].Value = homePhoneTB.Text;
            fieldMappingsSection.Settings["office_phone"].Value = officePhoneTB.Text;
            fieldMappingsSection.Settings["address"].Value = addressTB.Text;
            fieldMappingsSection.Settings["address2"].Value = address2TB.Text;
            fieldMappingsSection.Settings["city"].Value = cityTB.Text;
            fieldMappingsSection.Settings["state"].Value = stateTB.Text;
            fieldMappingsSection.Settings["country"].Value = countryTB.Text;
            fieldMappingsSection.Settings["zipcode"].Value = zipcodeTB.Text;
            fieldMappingsSection.Settings["custom1"].Value = custom1TB.Text;
            fieldMappingsSection.Settings["custom2"].Value = custom2TB.Text;
            fieldMappingsSection.Settings["custom3"].Value = custom3TB.Text;
            fieldMappingsSection.Settings["notes"].Value = notesTB.Text;
            fieldMappingsSection.Settings["outbound_number"].Value = outboundNumberTB.Text;
            fieldMappingsSection.Settings["customer_user_id"].Value = customerUserIdTB.Text;
            config.Save(ConfigurationSaveMode.Minimal);

            this.refreshData();
            Cursor.Current = Cursors.Default;
            MessageBox.Show("Field mappings are completed. Click on Next button to continue");
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
