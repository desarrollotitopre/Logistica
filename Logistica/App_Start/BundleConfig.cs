using System.Web;
using System.Web.Optimization;

namespace Logistica
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-3.7.1.min.js",
                "~/Scripts/jquery-{version}.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                "~/Scripts/jquery-ui-{version}.js"
            ));


            bundles.Add(new Bundle("~/bundles/complementos").Include(
                      "~/assets/vendor/bootstrap/js/bootstrap.bundle.min.js",
                      "~/assets/js/main.js",
                      "~/Scripts/DataTables/jquery.dataTables.js",
                      "~/Scripts/DataTables/dataTables.responsive.js",
                      "~/Scripts/toastr.min.js",
                      "~/Scripts/moment.min.js",
                      "~/Scripts/sweetalert.js",
                      "~/Scripts/select2.min.js",
                      "~/Scripts/jquery-ui-timepicker-addon.min.js",

                      "~/Content/Cropper/cropper.min.js",
                      "~/Scripts/loadingoverlay.min.js"));

            //bundles.Add(new Bundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(

                      "~/Content/bootstrap.min.css",
                      "~/Content/Cropper/cropper.min.css",
                      "~/assets/fontawesome/css/all.min.css",
                      "~/Content/DataTables/css/jquery.dataTables.css",
                      "~/Content/DataTables/css/responsive.dataTables.css",
                      "~/Content/toastr.css",
                      "~/Content/select2.min.css",
                      "~/Content/jquery-ui-timepicker-addon.css",

                      "~/Content/themes/base/jquery-ui.css",
                      "~/assets/css/style.css",
                      "~/assets/css/StyleDashboard.css"));
        }
    }
}
