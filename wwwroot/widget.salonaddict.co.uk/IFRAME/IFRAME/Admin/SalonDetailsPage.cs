namespace IFRAME.Admin
{
    using AjaxControlToolkit;
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    public class SalonDetailsPage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected Button btnLocationAdd;
        protected Button btnPictureDelete;
        protected Button btnPictureSave;
        protected Button btnSave;
        protected IFRAME.Admin.Modules.Menu cntrlMenu;
        protected DropDownList ddlLocationAP;
        protected DropDownList ddlLocationCC;
        protected FileUpload fuPicture;
        protected GridView gvLocations;
        protected Literal imgNone;
        protected System.Web.UI.WebControls.Image imgPicture;
        protected Label lblError;
        protected Label lblLocationError;
        protected Label lblSENameAvail;
        protected Literal ltrAPICount;
        protected Literal ltrCreatedOn;
        protected Literal ltrHeader;
        protected Literal ltrUserCount;
        protected TabPanel p1;
        protected TabPanel p2;
        protected TabPanel p3;
        protected TabPanel p4;
        protected Panel pnl;
        protected Panel pnl2;
        protected TabContainer tc;
        protected TextBox txtAddressLine1;
        protected TextBox txtAddressLine2;
        protected TextBox txtCity;
        protected TextBox txtDescription;
        protected TextBox txtLatitude;
        protected TextBox txtLongitude;
        protected TextBox txtName;
        protected TextBox txtPhone;
        protected TextBox txtPostalCode;
        protected TextBox txtSalonId;
        protected TextBox txtSEName;
        protected UpdatePanel upLocation;
        protected UpdatePanel upSEName;
        protected RequiredFieldValidator valAddressLine1;
        protected ValidatorCalloutExtender valAddressLine1Ex;
        protected RequiredFieldValidator valAddressLine3;
        protected ValidatorCalloutExtender valAddressLine3Ex;
        protected RequiredFieldValidator valAddressLine5;
        protected ValidatorCalloutExtender valAddressLine5Ex;
        protected RequiredFieldValidator valLatitude;
        protected ValidatorCalloutExtender valLatitudeEx;
        protected RequiredFieldValidator valLongitude;
        protected ValidatorCalloutExtender valLongitudeEx;
        protected RequiredFieldValidator valName;
        protected ValidatorCalloutExtender valNameEx;
        protected RequiredFieldValidator valPhone;
        protected ValidatorCalloutExtender valPhoneEx;
        protected RequiredFieldValidator valSalonId;
        protected ValidatorCalloutExtender valSalonIdEx;
        protected RequiredFieldValidator valSEName;
        protected ValidatorCalloutExtender valSENameEx;

        private void ApplyLocalization()
        {
            this.tc.Tabs[0].HeaderText = base.GetLocaleResourceString("Tabs[0].HeaderText");
            this.tc.Tabs[1].HeaderText = base.GetLocaleResourceString("Tabs[1].HeaderText");
            this.tc.Tabs[2].HeaderText = base.GetLocaleResourceString("Tabs[2].HeaderText");
            this.gvLocations.Columns[0].HeaderText = base.GetLocaleResourceString("gvLocations.Columns[0].HeaderText");
        }

        private void BindLocationAP(Guid regionID)
        {
            ListItemCollection items = new ListItemCollection {
                new ListItem("Area or Postal Code", string.Empty)
            };
            if (regionID != Guid.Empty)
            {
                List<LocationDB> areasPostalCodes = IoC.Resolve<ILocationManager>().GetAreasPostalCodes(regionID);
                List<LocationDB> list2 = (from item in areasPostalCodes
                    where item.GetLocationTypeEnum() == LocationType.Area
                    select item).ToList<LocationDB>();
                List<LocationDB> list3 = (from item in areasPostalCodes
                    where item.GetLocationTypeEnum() == LocationType.ZipPostalCode
                    select item).ToList<LocationDB>();
                foreach (LocationDB ndb in list2)
                {
                    items.Add(new ListItem(ndb.Name, ndb.LocationId.ToString()));
                }
                if ((list2.Count > 0) && (list3.Count > 0))
                {
                    items.Add(new ListItem("--------------", string.Empty, false));
                }
                foreach (LocationDB ndb2 in list3)
                {
                    items.Add(new ListItem(ndb2.Name, ndb2.LocationId.ToString()));
                }
            }
            this.ddlLocationAP.DataSource = items;
            this.ddlLocationAP.DataTextField = "Text";
            this.ddlLocationAP.DataValueField = "Value";
            this.ddlLocationAP.DataBind();
        }

        private void BindLocationCC()
        {
            ListItemCollection items = new ListItemCollection {
                new ListItem("City or County", string.Empty)
            };
            foreach (LocationDB ndb in IoC.Resolve<ILocationManager>().GetCountiesCityTownsByCountry("in-united-kingdom"))
            {
                items.Add(new ListItem(ndb.Name, ndb.LocationId.ToString()));
            }
            this.ddlLocationCC.DataSource = items;
            this.ddlLocationCC.DataTextField = "Text";
            this.ddlLocationCC.DataValueField = "Value";
            this.ddlLocationCC.DataBind();
        }

        private void BindLocationSummary()
        {
            List<LocationSalonSummaryDB> locationSalonSummary = IoC.Resolve<ILocationManager>().GetLocationSalonSummary(this.PostedSalonId);
            this.gvLocations.DataSource = locationSalonSummary;
            this.gvLocations.DataBind();
        }

        private void BindSalonDetails(SalonDB value)
        {
            this.ltrHeader.Text = string.Format(base.GetLocaleResourceString("ltrHeader.Text"), value.Name);
            if (value.PictureId.HasValue)
            {
                PictureDB pictureById = IoC.Resolve<IMediaManager>().GetPictureById(value.PictureId.Value);
                this.imgPicture.ImageUrl = IFRMHelper.GetPictureURL(pictureById, 110);
                this.imgPicture.Visible = true;
                this.imgNone.Visible = false;
                this.btnPictureDelete.Visible = true;
            }
            else
            {
                this.imgNone.Visible = true;
                this.btnPictureDelete.Visible = false;
            }
            this.txtName.Text = value.Name;
            this.txtSEName.Text = value.SEName;
            this.txtSalonId.Text = value.Abbreviation;
            this.txtPhone.Text = value.PhoneNumber;
            this.txtAddressLine1.Text = value.AddressLine1;
            this.txtAddressLine2.Text = value.AddressLine2;
            this.txtCity.Text = value.CityTown;
            this.txtPostalCode.Text = value.ZipPostalCode;
            this.txtLongitude.Text = value.Longitude;
            this.txtLatitude.Text = value.Latitude;
            this.txtDescription.Text = value.ShortDescription;
            this.ltrCreatedOn.Text = value.CreatedOnUtc.ToString("MMM dd yyyy");
        }

        private void BindSalonOptions(SalonDB value)
        {
            List<SalonUserDB> list = (from item in IoC.Resolve<IUserManager>().GetSalonUsersBySalonId(value.SalonId)
                where !item.IsAdmin
                select item).ToList<SalonUserDB>();
            List<WidgetApiKeyDB> widgetApiKeyBySalonId = IoC.Resolve<IUserManager>().GetWidgetApiKeyBySalonId(value.SalonId);
            this.ltrUserCount.Text = list.Count.ToString();
            this.ltrAPICount.Text = widgetApiKeyBySalonId.Count.ToString();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (this.PostedReturnURL != null)
            {
                base.Response.Redirect(this.PostedReturnURL, true);
            }
            string uRL = IFRMHelper.GetURL("salons.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void btnLocationAdd_Click(object sender, EventArgs e)
        {
            Guid countyID;
            string selectedValue = this.ddlLocationCC.SelectedValue;
            if (selectedValue == string.Empty)
            {
                this.lblLocationError.Text = "You must select a location.";
            }
            else
            {
                countyID = new Guid(selectedValue);
                SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
                bool flag = false;
                List<LocationSalonSummaryDB> locationSalonSummary = IoC.Resolve<ILocationManager>().GetLocationSalonSummary(salonById.SalonId);
                if (!locationSalonSummary.Exists(item => item.LocationId == countyID))
                {
                    LocationSalonMappingDB mapping = new LocationSalonMappingDB {
                        LocationId = countyID,
                        SalonId = salonById.SalonId
                    };
                    IoC.Resolve<ILocationManager>().InsertLocationSalonMapping(mapping);
                    flag = true;
                }
                selectedValue = this.ddlLocationAP.SelectedValue;
                if (selectedValue != string.Empty)
                {
                    Guid areaID = new Guid(selectedValue);
                    if (locationSalonSummary.Exists(item => item.LocationId == areaID))
                    {
                        if (flag)
                        {
                            this.BindLocationSummary();
                        }
                        this.lblLocationError.Text = $"{this.ddlLocationAP.SelectedItem.Text} has already been added.";
                        return;
                    }
                    LocationSalonMappingDB gdb2 = new LocationSalonMappingDB {
                        LocationId = areaID,
                        SalonId = salonById.SalonId
                    };
                    IoC.Resolve<ILocationManager>().InsertLocationSalonMapping(gdb2);
                }
                else if (!flag)
                {
                    this.lblLocationError.Text = $"{this.ddlLocationCC.SelectedItem.Text} has already been added.";
                    return;
                }
                this.BindLocationSummary();
            }
        }

        protected void btnPictureDelete_Click(object sender, EventArgs e)
        {
            SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
            if (salonById.PictureId.HasValue)
            {
                PictureDB pictureById = IoC.Resolve<IMediaManager>().GetPictureById(salonById.PictureId.Value);
                salonById.PictureId = null;
                salonById = IoC.Resolve<ISalonManager>().UpdateSalon(salonById);
                IFRMHelper.DeletePictureFromWebServer(pictureById);
                IoC.Resolve<IMediaManager>().DeletePicture(pictureById);
            }
            this.BindSalonDetails(salonById);
        }

        protected void btnPictureSave_Click(object sender, EventArgs e)
        {
            SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
            if (this.fuPicture.HasFile)
            {
                if (salonById.PictureId.HasValue)
                {
                    this.lblError.Text = "You must first delete the current picture.";
                    goto Label_01DE;
                }
                if (!IFRMHelper.GetAllowedPictureMimeTypes().ContainsKey(this.fuPicture.PostedFile.ContentType.ToLower()))
                {
                    this.lblError.Text = "Image must be in jpg/jpeg or png format.";
                    goto Label_01DE;
                }
                if (this.fuPicture.PostedFile.ContentLength > 0x186a0)
                {
                    this.lblError.Text = "Image must have a file size of less than 100k.";
                    goto Label_01DE;
                }
                using (System.Drawing.Image image = System.Drawing.Image.FromStream(this.fuPicture.PostedFile.InputStream))
                {
                    if (image.Height != image.Width)
                    {
                        this.lblError.Text = "Image pixel dimensions must be square; width must equal height.";
                    }
                    else if ((image.Height < 110) || (image.Width < 110))
                    {
                        this.lblError.Text = "Image pixel dimensions  must be minimum 110px x 110px.";
                    }
                    else
                    {
                        PictureDB picture = new PictureDB {
                            Height = image.Height,
                            MimeType = this.fuPicture.PostedFile.ContentType.ToLower(),
                            Name = this.fuPicture.FileName.Replace(Path.GetExtension(this.fuPicture.FileName), string.Empty),
                            PictureBinary = this.fuPicture.FileBytes,
                            SEName = salonById.SEName,
                            Width = image.Width
                        };
                        picture = IoC.Resolve<IMediaManager>().InsertPicture(picture);
                        if (picture != null)
                        {
                            salonById.PictureId = new Guid?(picture.PictureId);
                            IoC.Resolve<ISalonManager>().UpdateSalon(salonById);
                            IFRMHelper.SavePictureToWebServer(picture, 110);
                        }
                    }
                    goto Label_01DE;
                }
            }
            this.lblError.Text = "Choose an image to upload above.";
        Label_01DE:
            this.BindSalonDetails(salonById);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
                salonById.Abbreviation = this.txtSalonId.Text.Trim();
                salonById.Name = this.txtName.Text.Trim();
                salonById.SEName = this.txtSEName.Text.Trim().ToLowerInvariant();
                salonById.PhoneNumber = this.txtPhone.Text.Trim();
                salonById.ShortDescription = this.txtDescription.Text.Trim().Replace("\r", string.Empty).Replace("\n", string.Empty);
                salonById = IoC.Resolve<ISalonManager>().UpdateSalon(salonById);
                salonById.AddressLine1 = this.txtAddressLine1.Text.Trim();
                salonById.AddressLine2 = this.txtAddressLine2.Text.Trim();
                salonById.AddressLine3 = this.txtCity.Text.Trim();
                salonById.AddressLine4 = string.Empty;
                salonById.AddressLine5 = this.txtPostalCode.Text.Trim();
                salonById.County = string.Empty;
                salonById.CityTown = this.txtCity.Text.Trim();
                salonById.ZipPostalCode = this.txtPostalCode.Text.Trim();
                salonById.Longitude = this.txtLongitude.Text.Trim();
                salonById.Latitude = this.txtLatitude.Text.Trim();
                salonById = IoC.Resolve<ISalonManager>().UpdateSalon(salonById);
                string str = $"{"s"}={salonById.Name.Substring(0, 1).ToUpperInvariant()}";
                string uRL = IFRMHelper.GetURL("salons.aspx", new string[] { str });
                base.Response.Redirect(uRL, true);
            }
        }

        protected void ddlLocationCC_SelectedIndexChanged(object sender, EventArgs e)
        {
            Guid empty = Guid.Empty;
            if (this.ddlLocationCC.SelectedValue != string.Empty)
            {
                empty = new Guid(this.ddlLocationCC.SelectedValue);
            }
            this.BindLocationAP(empty);
        }

        protected void gvLocation_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Guid mappingId = new Guid(this.gvLocations.DataKeys[e.RowIndex].Value.ToString());
            LocationSalonMappingDB locationSalonMappingById = IoC.Resolve<ILocationManager>().GetLocationSalonMappingById(mappingId);
            IoC.Resolve<ILocationManager>().DeleteLocationSalonMapping(locationSalonMappingById);
            this.BindLocationSummary();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ApplyLocalization();
            if (this.PostedSalonId == Guid.Empty)
            {
                string uRL = IFRMHelper.GetURL("salons.aspx", new string[0]);
                base.Response.Redirect(uRL, true);
            }
            if (!this.Page.IsPostBack)
            {
                SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
                if (salonById == null)
                {
                    string url = IFRMHelper.GetURL("salons.aspx", new string[0]);
                    base.Response.Redirect(url, true);
                }
                this.BindSalonDetails(salonById);
                this.BindSalonOptions(salonById);
                this.BindLocationCC();
                this.BindLocationAP(Guid.Empty);
                this.BindLocationSummary();
            }
        }

        protected void txtSEName_TextChanged(object sender, EventArgs e)
        {
            string sename = this.txtSEName.Text.Trim();
            SalonDB salonBySEName = IoC.Resolve<ISalonManager>().GetSalonBySEName(sename);
            if ((salonBySEName != null) && (salonBySEName.SalonId != this.PostedSalonId))
            {
                this.txtSEName.Text = string.Empty;
                this.lblSENameAvail.Text = $"'{sename}' is not available";
            }
        }

        public string PostedReturnURL
        {
            get
            {
                string str = base.Request.QueryString["url"];
                if (string.IsNullOrEmpty(str))
                {
                    return null;
                }
                if (!IFRMHelper.IsUrlLocalToHost(str))
                {
                    return null;
                }
                return str;
            }
        }

        public Guid PostedSalonId
        {
            get
            {
                string str = base.Request.QueryString["sid"];
                if (!string.IsNullOrEmpty(str))
                {
                    try
                    {
                        return new Guid(str);
                    }
                    catch
                    {
                    }
                }
                return Guid.Empty;
            }
        }
    }
}

