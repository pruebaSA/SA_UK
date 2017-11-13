namespace IFRAME.Controllers
{
    using Resources;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Web;
    using System.Web.UI;

    public static class IFRMHelper
    {
        public static void DeletePictureFromWebServer(PictureDB picture)
        {
            foreach (string str2 in Directory.GetFiles(HttpContext.Current.Request.PhysicalApplicationPath + @"images\salon\"))
            {
                File.Delete(str2);
            }
        }

        public static void DeletePictureFromWebServer(PictureDB picture, int targetSize)
        {
            if (PictureExistsOnWebServer(picture, targetSize))
            {
                File.Delete(GetPicturePhysicalPath(picture, targetSize));
            }
        }

        public static DateTime FromHolidayFriendlyDate(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = $"{DateTime.Now.ToString("yyyy")}-{value}";
                try
                {
                    return DateTime.ParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                }
            }
            return DateTime.MinValue;
        }

        public static DateTime FromUrlFriendlyDate(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    return DateTime.ParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                }
            }
            return DateTime.MinValue;
        }

        public static string FromUrlFriendlyLanguage(string value) => 
            value.Replace('.', '-');

        public static DateTime FromUrlFriendlyTime(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    return DateTime.ParseExact(value, "HH.mm", CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                }
            }
            return DateTime.MinValue;
        }

        public static string GetAddressFriendlyString(SalonDB value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder();
            builder.Append(value.AddressLine1);
            if (!string.IsNullOrEmpty(value.AddressLine2))
            {
                builder.Append(" ");
                builder.Append(value.AddressLine2);
            }
            if (!string.IsNullOrEmpty(value.AddressLine3))
            {
                builder.Append(", ");
                builder.Append(value.AddressLine3);
            }
            if (!string.IsNullOrEmpty(value.AddressLine4))
            {
                builder.Append(" ");
                builder.Append(value.AddressLine4);
            }
            if (!string.IsNullOrEmpty(value.AddressLine5))
            {
                builder.Append(" ");
                builder.Append(value.AddressLine5);
            }
            return builder.ToString();
        }

        public static Dictionary<string, string> GetAllowedPictureMimeTypes() => 
            new Dictionary<string, string> { 
                { 
                    "image/jpg",
                    "jpg"
                },
                { 
                    "image/jpeg",
                    "jpeg"
                },
                { 
                    "image/pjpeg",
                    "pjeg"
                },
                { 
                    "image/gif",
                    "gif"
                },
                { 
                    "image/x-png",
                    "xpng"
                },
                { 
                    "image/png",
                    "png"
                }
            };

        public static DateTime GetMinimumSearchDate() => 
            DateTime.Today.AddDays(1.0);

        public static ImageCodecInfo GetPictureCodecInfo(string mimeType)
        {
            foreach (ImageCodecInfo info in ImageCodecInfo.GetImageEncoders())
            {
                if (info.MimeType.Equals(mimeType, StringComparison.OrdinalIgnoreCase))
                {
                    return info;
                }
            }
            return null;
        }

        public static string GetPictureFilename(PictureDB picture, int targetSize)
        {
            if (picture == null)
            {
                throw new ArgumentNullException("picture");
            }
            Dictionary<string, string> allowedPictureMimeTypes = GetAllowedPictureMimeTypes();
            if (!allowedPictureMimeTypes.ContainsKey(picture.MimeType))
            {
                throw new InvalidOperationException("mime type not supported.");
            }
            return $"{picture.SEName}_{targetSize}.{allowedPictureMimeTypes[picture.MimeType]}";
        }

        public static string GetPicturePhysicalPath(PictureDB picture, int targetSize)
        {
            if (picture == null)
            {
                throw new ArgumentNullException("picture");
            }
            string pictureFilename = GetPictureFilename(picture, targetSize);
            return Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath + @"images\salon\", pictureFilename);
        }

        public static string GetPictureURL(PictureDB picture, int targetSize)
        {
            if (picture == null)
            {
                throw new ArgumentNullException("picture");
            }
            if (!PictureExistsOnWebServer(picture, targetSize))
            {
                SavePictureToWebServer(picture, targetSize);
            }
            string pictureFilename = GetPictureFilename(picture, targetSize);
            return $"~/images/salon/{pictureFilename}";
        }

        public static string GetSalonClosingHour(DayOfWeek dayOfWeek, OpeningHoursDB hours)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return $"{hours.SunEnd1.Value.Hours.ToString("00")}:{hours.SunEnd1.Value.Minutes.ToString("00")}";

                case DayOfWeek.Monday:
                    return $"{hours.MonEnd1.Value.Hours.ToString("00")}:{hours.MonEnd1.Value.Minutes.ToString("00")}";

                case DayOfWeek.Tuesday:
                    return $"{hours.TueEnd1.Value.Hours.ToString("00")}:{hours.TueEnd1.Value.Minutes.ToString("00")}";

                case DayOfWeek.Wednesday:
                    return $"{hours.WedEnd1.Value.Hours.ToString("00")}:{hours.WedEnd1.Value.Minutes.ToString("00")}";

                case DayOfWeek.Thursday:
                    return $"{hours.ThuEnd1.Value.Hours.ToString("00")}:{hours.ThuEnd1.Value.Minutes.ToString("00")}";

                case DayOfWeek.Friday:
                    return $"{hours.FriEnd1.Value.Hours.ToString("00")}:{hours.FriEnd1.Value.Minutes.ToString("00")}";

                case DayOfWeek.Saturday:
                    return $"{hours.SatEnd1.Value.Hours.ToString("00")}:{hours.SatEnd1.Value.Minutes.ToString("00")}";
            }
            throw new ArgumentOutOfRangeException("dayOfWeek", dayOfWeek, "Day of week is not a valid day.");
        }

        public static string GetSalonOpeningHour(DayOfWeek dayOfWeek, OpeningHoursDB hours)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return $"{hours.SunStart1.Value.Hours.ToString("00")}:{hours.SunStart1.Value.Minutes.ToString("00")}";

                case DayOfWeek.Monday:
                    return $"{hours.MonStart1.Value.Hours.ToString("00")}:{hours.MonStart1.Value.Minutes.ToString("00")}";

                case DayOfWeek.Tuesday:
                    return $"{hours.TueStart1.Value.Hours.ToString("00")}:{hours.TueStart1.Value.Minutes.ToString("00")}";

                case DayOfWeek.Wednesday:
                    return $"{hours.WedStart1.Value.Hours.ToString("00")}:{hours.WedStart1.Value.Minutes.ToString("00")}";

                case DayOfWeek.Thursday:
                    return $"{hours.ThuStart1.Value.Hours.ToString("00")}:{hours.ThuStart1.Value.Minutes.ToString("00")}";

                case DayOfWeek.Friday:
                    return $"{hours.FriStart1.Value.Hours.ToString("00")}:{hours.FriStart1.Value.Minutes.ToString("00")}";

                case DayOfWeek.Saturday:
                    return $"{hours.SatStart1.Value.Hours.ToString("00")}:{hours.SatStart1.Value.Minutes.ToString("00")}";
            }
            throw new ArgumentOutOfRangeException("dayOfWeek", dayOfWeek, "Day of week is not a valid day.");
        }

        public static void GetThreeDayRangeBySearchDate(DateTime value, out DateTime start, out DateTime end)
        {
            DateTime minimumSearchDate = GetMinimumSearchDate();
            if (value.Date.CompareTo(minimumSearchDate.Date) < 0)
            {
                throw new ArgumentException($"Date cannot be less than the minimum searchable date {minimumSearchDate}: {value}", "value");
            }
            if (minimumSearchDate.Date.CompareTo(value.Date) == 0)
            {
                start = minimumSearchDate;
                end = start.AddDays(3.0);
            }
            else
            {
                start = value.AddDays(-1.0);
                end = value.AddDays(1.0);
            }
        }

        public static string GetURL(string url, params string[] parameters)
        {
            NameValueCollection values = new NameValueCollection();
            foreach (string str in parameters)
            {
                string[] strArray = str.Split(new char[] { '=' });
                if (strArray.Length == 2)
                {
                    values.Add(strArray[0], strArray[1]);
                }
                else if (strArray.Length == 1)
                {
                    values.Add(strArray[0], string.Empty);
                }
            }
            url = GetURL(url, values);
            return url;
        }

        public static string GetURL(string url, NameValueCollection parameters)
        {
            bool flag1 = url == "opening-hours.aspx";
            string str = $"{"api_key"}={IFRMContext.Current.APIKey}";
            string str2 = $"{"theme"}={IFRMContext.Current.WorkingTheme}";
            string str3 = $"{"lang"}={ToUrlFriendlyLanguage(IFRMContext.Current.WorkingLanguage)}";
            if (parameters.Get("api_key") != null)
            {
                parameters.Remove("api_key");
            }
            if (parameters.Get("theme") != null)
            {
                parameters.Remove("theme");
            }
            if (parameters.Get("lang") != null)
            {
                parameters.Remove("lang");
            }
            if (url.EndsWith("?"))
            {
                if (url.Length == 1)
                {
                    url = string.Empty;
                }
                else
                {
                    url = url.Substring(0, url.Length - 1);
                }
            }
            StringBuilder builder = new StringBuilder();
            builder.Append(url);
            builder.Append("?");
            builder.Append(str);
            foreach (string str4 in parameters)
            {
                builder.Append("&");
                builder.Append(str4);
                builder.Append("=");
                builder.Append(parameters[str4]);
            }
            builder.Append("&");
            builder.Append(str2);
            builder.Append("&");
            builder.Append(str3);
            return builder.ToString();
        }

        public static void GetWeekRangeBySearchDate(DateTime value, out DateTime start, out DateTime end)
        {
            DateTime minimumSearchDate = GetMinimumSearchDate();
            if (value.Date.CompareTo(minimumSearchDate.Date) < 0)
            {
                throw new ArgumentException($"Date cannot be less than the minimum searchable date {minimumSearchDate}: {value}", "value");
            }
            if ((minimumSearchDate.Date.CompareTo(value.Date) == 0) || (minimumSearchDate.AddDays(3.0).Date.CompareTo(value.Date) > 0))
            {
                start = minimumSearchDate;
                end = start.AddDays(7.0);
            }
            else
            {
                start = value.AddDays(-3.0);
                end = value.AddDays(3.0);
            }
        }

        public static bool InitiateDelivery(QueuedMessageDB value, bool isLoopBack)
        {
            if (value != null)
            {
                MailMessage message = new MailMessage {
                    From = new MailAddress(value.Sender)
                };
                message.To.Add(new MailAddress(value.Recipient));
                message.Subject = value.Subject;
                message.Body = value.Body;
                message.IsBodyHtml = true;
                SmtpClient client = new SmtpClient();
                try
                {
                    if (!isLoopBack)
                    {
                        client.Send(message);
                    }
                    return true;
                }
                catch
                {
                }
            }
            return false;
        }

        public static bool IsContentPageRequested()
        {
            HttpContext current = HttpContext.Current;
            if (current == null)
            {
                return false;
            }
            HttpRequest request = current.Request;
            if (request == null)
            {
                return false;
            }
            if ((!request.Url.LocalPath.ToLower().EndsWith(".aspx") && !request.Url.LocalPath.ToLower().EndsWith(".asmx")) && !request.Url.LocalPath.ToLower().EndsWith(".ashx"))
            {
                return false;
            }
            return true;
        }

        public static bool IsSalonClosed(DayOfWeek dayOfWeek, OpeningHoursDB hours)
        {
            if (hours == null)
            {
                return true;
            }
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return hours.SunClosed;

                case DayOfWeek.Monday:
                    return hours.MonClosed;

                case DayOfWeek.Tuesday:
                    return hours.TueClosed;

                case DayOfWeek.Wednesday:
                    return hours.WedClosed;

                case DayOfWeek.Thursday:
                    return hours.ThuClosed;

                case DayOfWeek.Friday:
                    return hours.FriClosed;

                case DayOfWeek.Saturday:
                    return hours.SatClosed;
            }
            throw new ArgumentOutOfRangeException("dayOfWeek", dayOfWeek.ToString());
        }

        public static bool IsSalonClosed(DateTime date, OpeningHoursDB hours, List<ClosingDayDB> holidays)
        {
            Predicate<ClosingDayDB> match = null;
            bool flag = IsSalonClosed(date.DayOfWeek, hours);
            if (flag || (holidays == null))
            {
                return flag;
            }
            if (match == null)
            {
                match = item => item.Date == date.Date;
            }
            return (holidays.Exists(match) || flag);
        }

        public static bool IsUrlLocalToHost(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }
            return (((url[0] == '/') && ((url.Length == 1) || ((url[1] != '/') && (url[1] != '\\')))) || (((url.Length > 1) && (url[0] == '~')) && (url[1] == '/')));
        }

        public static bool PictureExistsOnWebServer(PictureDB picture, int targetSize) => 
            File.Exists(GetPicturePhysicalPath(picture, targetSize));

        public static void RenderPageTitle(Page page, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                page.Title = value;
            }
        }

        public static string ReplaceTokens(string value, Dictionary<string, string> tokens)
        {
            tokens.Keys.ToList<string>().ForEach(delegate (string key) {
                value = value.Replace(key, tokens[key]);
            });
            return value;
        }

        public static void SavePictureToWebServer(PictureDB picture, int targetSize)
        {
            if (PictureExistsOnWebServer(picture, targetSize))
            {
                throw new ArgumentException("picture already exists");
            }
            string picturePhysicalPath = GetPicturePhysicalPath(picture, targetSize);
            using (MemoryStream stream = new MemoryStream(picture.PictureBinary))
            {
                stream.Position = 0L;
                using (Bitmap bitmap = new Bitmap(stream))
                {
                    Size size = new Size(targetSize, targetSize);
                    using (Bitmap bitmap2 = new Bitmap(size.Width, size.Height))
                    {
                        using (Graphics graphics = Graphics.FromImage(bitmap2))
                        {
                            graphics.SmoothingMode = SmoothingMode.HighQuality;
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            graphics.CompositingQuality = CompositingQuality.HighQuality;
                            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            graphics.DrawImage(bitmap, 0, 0, size.Width, size.Height);
                            EncoderParameters encoderParams = new EncoderParameters();
                            encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                            bitmap2.Save(picturePhysicalPath, GetPictureCodecInfo(picture.MimeType), encoderParams);
                        }
                    }
                }
            }
        }

        public static bool SendCusotmerOrderEmail(TicketSummaryDB order, TicketRowDB appointment, SalonDB salon, string specialRequest, bool isLoopBack)
        {
            MailMessage message = new MailMessage {
                From = new MailAddress(CustomerOrderConfirmation.From)
            };
            message.To.Add(new MailAddress(order.CustomerEmail));
            message.Subject = CustomerOrderConfirmation.Subject;
            message.Body = CustomerOrderConfirmation.Body;
            message.IsBodyHtml = true;
            Dictionary<string, string> dictionary = new Dictionary<string, string> {
                { 
                    "$$APPOINTMENT_DATE$$",
                    appointment.StartDate.Value.ToString("dddd, dd MMMM yyyy")
                },
                { 
                    "$$APPOINTMENT_SERVICE$$",
                    appointment.ServiceDisplayText
                },
                { 
                    "$$APPOINTMENT_EMPLOYEE$$",
                    string.IsNullOrEmpty(appointment.EmployeeDisplayText) ? CustomerOrderConfirmation.Any : appointment.EmployeeDisplayText
                }
            };
            dictionary.Add("$$APPOINTMENT_TIME$$", new DateTime(appointment.StartTime.Value.Ticks).ToString("HH:mm"));
            dictionary.Add("$$APPOINTMENT_PRICE$$", appointment.Price.ToString("#,#.00#"));
            dictionary.Add("$$APPOINTMENT_SPECIAL_REQUEST$$", string.IsNullOrEmpty(specialRequest) ? CustomerOrderConfirmation.None : specialRequest);
            dictionary.Add("$$ORDER_BILLING_FIRST_NAME$$", order.BillingFirstName);
            dictionary.Add("$$ORDER_BILLING_LAST_NAME$$", order.BillingLastName);
            dictionary.Add("$$ORDER_REFERENCE_NUMBER$$", order.TicketNumber);
            dictionary.Add("$$SALON_ADDRESS_COUNTY$$", salon.County);
            dictionary.Add("$$SALON_ADDRESS_LINE1$$", salon.AddressLine1);
            dictionary.Add("$$SALON_ADDRESS_LINE2$$", salon.AddressLine2);
            dictionary.Add("$$SALON_NAME$$", salon.Name);
            dictionary.Add("$$SALON_PHONE_NUMBER$$", salon.PhoneNumber);
            dictionary.Add("$$SALON_ADDRESS_ZIP_POSTAL_CODE$$", salon.ZipPostalCode);
            foreach (string str in dictionary.Keys)
            {
                message.Subject = message.Subject.Replace(str, dictionary[str]);
                message.Body = message.Body.Replace(str, dictionary[str]);
            }
            try
            {
                SmtpClient client = new SmtpClient();
                if (!isLoopBack)
                {
                    client.Send(message);
                }
                return true;
            }
            catch
            {
            }
            QueuedMessageDB edb = new QueuedMessageDB {
                Sender = message.From.ToString(),
                SenderDisplayName = message.From.DisplayName,
                Recipient = message.To[0].Address.ToString(),
                RecipientDisplayName = message.To[0].DisplayName,
                Subject = message.Subject,
                Body = message.Body,
                SendTries = 1,
                MessageType = "EMAIL",
                CreatedOn = DateTime.Now
            };
            edb = IoC.Resolve<IMessageManager>().InsertQueuedMessage(edb);
            return false;
        }

        public static bool SendSalesOrderEmail(TicketSummaryDB order, TicketRowDB appointment, SalonDB salon, string specialRequest, string domain, bool isLoopBack)
        {
            MailMessage message = new MailMessage {
                From = new MailAddress(SalesOrderConfirmation.From)
            };
            message.To.Add(new MailAddress(SalesOrderConfirmation.To));
            message.Subject = SalesOrderConfirmation.Subject;
            message.Body = SalesOrderConfirmation.Body;
            message.IsBodyHtml = true;
            StringBuilder builder = new StringBuilder();
            List<SalonUserDB> salonUsersBySalonId = IoC.Resolve<IUserManager>().GetSalonUsersBySalonId(salon.SalonId);
            if (salonUsersBySalonId.Count > 0)
            {
                builder.Append("<table cellpadding=\"0\" cellspacing=\"0\" >");
                foreach (SalonUserDB rdb in salonUsersBySalonId)
                {
                    if (rdb.Active)
                    {
                        builder.Append("<tr>");
                        builder.Append("<td style=\"padding-bottom:8px;\" nowrap=\"nowrap\" >");
                        builder.Append("<b>");
                        builder.Append((rdb.FirstName + " " + rdb.LastName).Trim());
                        builder.Append("</b>");
                        builder.Append("</td>");
                        builder.Append("<td style=\"padding-bottom:8px;padding-left:8px;\" nowrap=\"nowrap\" >");
                        builder.Append("<b>");
                        builder.Append(rdb.Email ?? string.Empty);
                        builder.Append("</b>");
                        builder.Append("</td>");
                        builder.Append("<td style=\"padding-bottom:8px;padding-left:8px;\" nowrap=\"nowrap\" >");
                        builder.Append("<b>");
                        builder.Append((string.IsNullOrEmpty(rdb.Mobile) ? ((bool) rdb.PhoneNumber) : ((bool) rdb.Mobile)) ?? string.Empty);
                        builder.Append("</b>");
                        builder.Append("</td>");
                        builder.Append("</tr>");
                    }
                }
                builder.Append("</table>");
            }
            Dictionary<string, string> dictionary = new Dictionary<string, string> {
                { 
                    "$$APPOINTMENT_DATE$$",
                    appointment.StartDate.Value.ToString("dddd, dd MMMM yyyy")
                },
                { 
                    "$$APPOINTMENT_SERVICE$$",
                    appointment.ServiceDisplayText
                },
                { 
                    "$$APPOINTMENT_EMPLOYEE$$",
                    string.IsNullOrEmpty(appointment.EmployeeDisplayText) ? CustomerOrderConfirmation.Any : appointment.EmployeeDisplayText
                }
            };
            dictionary.Add("$$APPOINTMENT_TIME$$", new DateTime(appointment.StartTime.Value.Ticks).ToString("HH:mm"));
            dictionary.Add("$$APPOINTMENT_PRICE$$", appointment.Price.ToString("#,#.00#"));
            dictionary.Add("$$APPOINTMENT_SPECIAL_REQUEST$$", string.IsNullOrEmpty(specialRequest) ? CustomerOrderConfirmation.None : specialRequest);
            dictionary.Add("$$DOMAIN$$", domain ?? string.Empty);
            dictionary.Add("$$ORDER_BILLING_FIRST_NAME$$", order.BillingFirstName);
            dictionary.Add("$$ORDER_BILLING_LAST_NAME$$", order.BillingLastName);
            dictionary.Add("$$ORDER_BILLING_EMAIL$$", order.BillingEmail);
            dictionary.Add("$$ORDER_BILLING_PHONE$$", (order.BillingPhone == string.Empty) ? order.BillingMobile : order.BillingPhone);
            dictionary.Add("$$ORDER_REFERENCE_NUMBER$$", order.TicketNumber);
            dictionary.Add("$$SALON_ADDRESS_COUNTY$$", salon.County);
            dictionary.Add("$$SALON_ADDRESS_LINE1$$", salon.AddressLine1);
            dictionary.Add("$$SALON_ADDRESS_LINE2$$", salon.AddressLine2);
            dictionary.Add("$$SALON_NAME$$", salon.Name);
            dictionary.Add("$$SALON_PHONE_NUMBER$$", salon.PhoneNumber);
            dictionary.Add("$$SALON_ADDRESS_ZIP_POSTAL_CODE$$", salon.ZipPostalCode);
            dictionary.Add("$$SALON_USERS$$", builder.ToString());
            foreach (string str in dictionary.Keys)
            {
                message.Subject = message.Subject.Replace(str, dictionary[str]);
                message.Body = message.Body.Replace(str, dictionary[str]);
            }
            try
            {
                SmtpClient client = new SmtpClient();
                if (!isLoopBack)
                {
                    client.Send(message);
                }
                return true;
            }
            catch
            {
            }
            QueuedMessageDB edb = new QueuedMessageDB {
                Sender = message.From.ToString(),
                SenderDisplayName = message.From.DisplayName,
                Recipient = message.To[0].Address.ToString(),
                RecipientDisplayName = message.To[0].DisplayName,
                Subject = message.Subject,
                Body = message.Body,
                SendTries = 1,
                MessageType = "EMAIL",
                CreatedOn = DateTime.Now
            };
            edb = IoC.Resolve<IMessageManager>().InsertQueuedMessage(edb);
            return false;
        }

        public static bool SendSalonOrderEmail(TicketSummaryDB order, TicketRowDB appointment, SalonDB salon, bool firstTimeAtSalon, string specialRequest, bool isLoopBack)
        {
            bool flag = false;
            foreach (TicketAlertDB tdb in IoC.Resolve<ITicketManager>().GetTicketAlertsBySalonId(salon.SalonId))
            {
                if ((tdb.Active && tdb.ByEmail) && !string.IsNullOrEmpty(tdb.Email))
                {
                    MailMessage message = new MailMessage {
                        From = new MailAddress(SalonOrderConfirmation.From)
                    };
                    message.To.Add(new MailAddress(tdb.Email));
                    message.Subject = SalonOrderConfirmation.Subject;
                    message.Body = SalonOrderConfirmation.Body;
                    message.IsBodyHtml = true;
                    Dictionary<string, string> dictionary = new Dictionary<string, string> {
                        { 
                            "$$APPOINTMENT_DATE$$",
                            appointment.StartDate.Value.ToString("dddd, dd MMMM yyyy")
                        },
                        { 
                            "$$APPOINTMENT_SERVICE$$",
                            appointment.ServiceDisplayText
                        },
                        { 
                            "$$APPOINTMENT_EMPLOYEE$$",
                            string.IsNullOrEmpty(appointment.EmployeeDisplayText) ? CustomerOrderConfirmation.Any : appointment.EmployeeDisplayText
                        }
                    };
                    dictionary.Add("$$APPOINTMENT_TIME$$", new DateTime(appointment.StartTime.Value.Ticks).ToString("HH:mm"));
                    dictionary.Add("$$APPOINTMENT_PRICE$$", appointment.Price.ToString("#,#.00#"));
                    dictionary.Add("$$APPOINTMENT_FIRST_TIME_AT_SALON$$", firstTimeAtSalon ? "Y" : "N");
                    dictionary.Add("$$APPOINTMENT_SPECIAL_REQUEST$$", string.IsNullOrEmpty(specialRequest) ? CustomerOrderConfirmation.None : specialRequest);
                    dictionary.Add("$$ORDER_BILLING_FIRST_NAME$$", order.BillingFirstName);
                    dictionary.Add("$$ORDER_BILLING_LAST_NAME$$", order.BillingLastName);
                    dictionary.Add("$$ORDER_BILLING_PHONE$$", (order.BillingPhone == string.Empty) ? order.BillingMobile : order.BillingPhone);
                    dictionary.Add("$$ORDER_BILLING_EMAIL$$", order.BillingEmail ?? string.Empty);
                    dictionary.Add("$$ORDER_REFERENCE_NUMBER$$", order.TicketNumber);
                    dictionary.Add("$$SALON_ADDRESS_COUNTY$$", salon.County);
                    dictionary.Add("$$SALON_ADDRESS_LINE1$$", salon.AddressLine1);
                    dictionary.Add("$$SALON_ADDRESS_LINE2$$", salon.AddressLine2);
                    dictionary.Add("$$SALON_NAME$$", salon.Name);
                    dictionary.Add("$$SALON_PHONE_NUMBER$$", salon.PhoneNumber);
                    dictionary.Add("$$SALON_ADDRESS_ZIP_POSTAL_CODE$$", salon.ZipPostalCode);
                    foreach (string str in dictionary.Keys)
                    {
                        message.Subject = message.Subject.Replace(str, dictionary[str]);
                        message.Body = message.Body.Replace(str, dictionary[str]);
                    }
                    try
                    {
                        SmtpClient client = new SmtpClient();
                        if (!isLoopBack)
                        {
                            client.Send(message);
                        }
                        flag = true;
                    }
                    catch
                    {
                        QueuedMessageDB edb = new QueuedMessageDB {
                            Sender = message.From.ToString(),
                            SenderDisplayName = message.From.DisplayName,
                            Recipient = message.To[0].Address.ToString(),
                            RecipientDisplayName = message.To[0].DisplayName,
                            Subject = message.Subject,
                            Body = message.Body,
                            SendTries = 1,
                            MessageType = "EMAIL",
                            CreatedOn = DateTime.Now
                        };
                        edb = IoC.Resolve<IMessageManager>().InsertQueuedMessage(edb);
                    }
                }
            }
            return flag;
        }

        public static string ToHolidayFriendlyDate(DateTime value) => 
            value.ToString("MM-dd");

        public static string ToUrlFriendlyDate(DateTime value) => 
            value.ToString("yyyy-MM-dd");

        public static string ToUrlFriendlyLanguage(string value) => 
            value.Replace('-', '.');

        public static string ToUrlFriendlyTime(DateTime value) => 
            value.ToString("HH.mm");
    }
}

