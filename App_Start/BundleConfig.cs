using lissystem.App_Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace lissystem.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            var lessBundle = new Bundle("~/lis/less").IncludeDirectory("~/Public/less", "*.less");
            lessBundle.Transforms.Add(new LISLessTransform());
            lessBundle.Transforms.Add(new CssMinify());
            bundles.Add(lessBundle);

            // Style sheet bundles

            bundles.Add(new StyleBundle("~/adminlte/css").Include(
                "~/Vendors/AdminLTE/css/adminlte.css",
                new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/bootstrap-icons").Include(
                "~/Vendors/bootstrap-icons/font/*.min.css"
                , new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/overlayscrollbars-css").Include(
                "~/Vendors/overlayscrollbars/styles/*.min.css"
                ));

            bundles.Add(new StyleBundle("~/select2css").Include(
                "~/Vendors/select2/css/select2.min.css",
                "~/Vendors/select2/theme/select2-bootstrap-5-theme.min.css"
                ));

            bundles.Add(new StyleBundle("~/datatable-css").Include(
                //"~/Vendors/datatables/datatables.bootstrap5.css",
                "~/Vendors/datatables/datatables.min.css"
                ));

            bundles.Add(new StyleBundle("~/jquery-toast-css").Include(
                "~/Vendors/jquery/plugins/toast/jquery.toast.min.css"
                ));

            bundles.Add(new StyleBundle("~/sweetalert2css").Include(
                "~/Vendors/sweetalert2/sweetalert2.min.css"
                ));

            bundles.Add(new StyleBundle("~/bs-daterangepicker-css").Include(
                "~/Vendors/bootstrap-daterangepicker/daterangepicker.min.css"
                ));

            // JavaScript bundles

            bundles.Add(new Bundle("~/bootstrap/js").Include(
                "~/Vendors/bootstrap/js/bootstrap.min.js"
                ));

            bundles.Add(new Bundle("~/adminlte/js").Include(
                "~/Vendors/AdminLTE/js/adminlte.js"
                ));

            bundles.Add(new Bundle("~/JQuery").Include(
                "~/Vendors/jquery/jquery.min.js"
                ));

            bundles.Add(new Bundle("~/JQuery/plugins").Include(
                "~/Vendors/jquery/plugins/query_validate/*.min.js"
                ));

            bundles.Add(new Bundle("~/momentjs").Include(
                "~/Vendors/moment-js/moment.min.js",
                "~/Vendors/moment-js/timezone/moment-timezone.min.js",
                "~/Vendors/moment-js/timezone/moment-timezone-with-data.min.js"
                ));

            bundles.Add(new Bundle("~/overlayscrollbars-js").Include(
                "~/Vendors/overlayscrollbars/browser/overlayscrollbars.browser.es6.min.js"
                ));

            bundles.Add(new Bundle("~/popper-umd").Include(
                "~/Vendors/popperjs/umd/popper.min.js"
                ));

            bundles.Add(new Bundle("~/LISApp").Include(
                "~/Public/js/LISApp.js"
                ));

            bundles.Add(new Bundle("~/LISAppLogin").Include(
                "~/Public/js/AppLogin.js"
                ));

            bundles.Add(new Bundle("~/datatable-js").Include(
                "~/Vendors/datatables/datatables.min.js",
                "~/Vendors/datatables/sorting/natural.min.js",
                "~/Vendors/datatables/sorting/numString.min.js",
                "~/Vendors/datatables/sorting/percent.js",
                "~/Vendors/datatables/sorting/ip-address.js",
                "~/Vendors/datatables/sorting/numeric-comma.js",
                "~/Vendors/datatables/sorting/date-euro.js",
                "~/Vendors/datatables/sorting/datetime-moment.js",
                "~/Vendors/datatables/sorting/file-size.js",
                "~/Vendors/datatables/sorting/dom-text.js",
                "~/Vendors/datatables/sorting/dom-text-numeric.js",
                "~/Vendors/datatables/sorting/dom-checkbox.js",
                "~/Vendors/datatables/sorting/dom-select.js"
                ));

            bundles.Add(new Bundle("~/select2js").Include(
                "~/Vendors/select2/js/select2.full.min.js"
                ));

            bundles.Add(new Bundle("~/jquery-toast-js").Include(
                "~/Vendors/jquery/plugins/toast/jquery.toast.min.js"
                ));

            bundles.Add(new Bundle("~/sweetalert2js").Include(
                "~/Vendors/sweetalert2/sweetalert2.all.min.js"
                ));

            bundles.Add(new Bundle("~/fusioncharts").Include(
                "~/Vendors/fusioncharts/fusioncharts.min.js",
                "~/Public/js/AppFusioncharts.min.js",
                "~/Vendors/fusioncharts-theme/fusioncharts.theme.candy.min.js",
                "~/Vendors/fusioncharts-theme/fusioncharts.theme.carbon.min.js",
                "~/Vendors/fusioncharts-theme/fusioncharts.theme.fusion.min.js",
                "~/Vendors/fusioncharts-theme/fusioncharts.theme.gammel.min.js",
                "~/Vendors/fusioncharts-theme/fusioncharts.theme.ocean.min.js",
                "~/Vendors/fusioncharts-theme/fusioncharts.theme.zune.min.js",
                "~/Vendors/fusioncharts-theme/fusioncharts.jqueryplugin.min.js"
                )
                .IncludeDirectory("~/Vendors/fusioncharts", "*.js")
                .IncludeDirectory("~/Vendors/fusioncharts-theme", "*.js"));

            bundles.Add(new Bundle("~/bs-daterangepicker-js").Include(
                "~/Vendors/bootstrap-daterangepicker/daterangepicker.min.js"
                ));

            BundleTable.EnableOptimizations = true;
        }
    }
}