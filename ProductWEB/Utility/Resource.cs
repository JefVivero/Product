using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductWEB.Utility
{
    public class Resource
    {
        public const string APIBaseURL = "https://localhost:44316/";
        public const string ProductAPIURL = APIBaseURL + "api/product/";
        public const string RegisterAPIURL = APIBaseURL + "api/user/register/";
        public const string LoginAPIURL = APIBaseURL + "api/user/login/";
        public const string ContentType = "application/json";
    }    
}
