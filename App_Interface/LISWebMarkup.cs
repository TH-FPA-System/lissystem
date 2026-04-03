using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMarkupMin.AspNet.Common;
using WebMarkupMin.AspNet.Common.Compressors;
using WebMarkupMin.AspNet4.Common;

namespace lissystem.App_Interface
{
    public static class LISWebMarkupConfig
    {
        public static void Configure(WebMarkupMinConfiguration configuration)
        {
            configuration.AllowMinificationInDebugMode = true;
            configuration.AllowCompressionInDebugMode = true;

            IXhtmlMinificationManager xhtmlMinificationManager = XhtmlMinificationManager.Current;
            IXmlMinificationManager xmlMinificationManager = XmlMinificationManager.Current;

            IHttpCompressionManager httpCompressionManager = HttpCompressionManager.Current;
            httpCompressionManager.CompressorFactories = new List<ICompressorFactory>
            {
                new DeflateCompressorFactory(),
                new GZipCompressorFactory()
            };
        }
    }
}