using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcCookieAuthSample.ViewModels
{
    public class ProcessConsentResult
    {
        public string ReturnUrl { get; set; }
        public bool IsRedirect => ReturnUrl != null;
        public ConsentViewModel ConsentViewModel { get; set; }
        public string ValidationError { get; set; }
    }
}
