namespace IFRAME.Controllers
{
    using SA.BAL;
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Threading;
    using System.Web;

    public class IFRMContext
    {
        private string _apiKey;
        private readonly HttpContext _context = HttpContext.Current;
        private SalonDB _salon;
        private string _workingLanguage = string.Empty;
        private string _workingTheme = string.Empty;
        private SalonUserDB _workingUser;

        private IFRMContext()
        {
        }

        public void SetCulture(CultureInfo culture)
        {
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
        }

        public string APIKey
        {
            get => 
                this._apiKey;
            set
            {
                this._apiKey = value;
            }
        }

        public static IFRMContext Current
        {
            get
            {
                if ((HttpContext.Current == null) && (HttpContext.Current == null))
                {
                    object data = Thread.GetData(Thread.GetNamedDataSlot("IFRMContext"));
                    if (data != null)
                    {
                        return (IFRMContext) data;
                    }
                    IFRMContext context = new IFRMContext();
                    Thread.SetData(Thread.GetNamedDataSlot("IFRMContext"), context);
                    return context;
                }
                if (HttpContext.Current.Items["IFRMContext"] == null)
                {
                    IFRMContext context2 = new IFRMContext();
                    HttpContext.Current.Items.Add("IFRMContext", context2);
                    return context2;
                }
                return (IFRMContext) HttpContext.Current.Items["IFRMContext"];
            }
        }

        public object this[string key]
        {
            get
            {
                if ((this._context != null) && (this._context.Items[key] != null))
                {
                    return this._context.Items[key];
                }
                return null;
            }
            set
            {
                if (this._context != null)
                {
                    this._context.Items.Remove(key);
                    this._context.Items.Add(key, value);
                }
            }
        }

        public SalonDB Salon
        {
            get => 
                this._salon;
            set
            {
                this._salon = value;
            }
        }

        public string WorkingLanguage
        {
            get => 
                this._workingLanguage;
            set
            {
                this._workingLanguage = value;
            }
        }

        public string WorkingTheme
        {
            get => 
                this._workingTheme;
            set
            {
                this._workingTheme = value;
            }
        }

        public SalonUserDB WorkingUser
        {
            get => 
                this._workingUser;
            set
            {
                this._workingUser = value;
            }
        }
    }
}

