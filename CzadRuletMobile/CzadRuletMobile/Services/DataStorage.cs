using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using CzadRuletCommon.Models;

namespace CzadRuletMobile.Services
{
    public static class DataStorage
    {
        public static AuthenticatedModel user = null;
        public static String imgBase64 = null;
        public static String toSentImgBase64 = null;
    }
}