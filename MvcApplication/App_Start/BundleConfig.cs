﻿using System.Web.Optimization;

namespace MvcApplication
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            //Jquery.Unobtrusive
            bundles.Add(new ScriptBundle("~/bundles/jqueryajax").Include(
                       "~/Scripts/jquery.unobtrusive*"));


            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/sweetalert").Include("~/Scripts/sweetalert.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
   
                      "~/Content/bootstrap-simplex.min.css",
                      "~/Content/sweetalert.css",
                      "~/Content/jquery-ui.css",
                      "~/Content/site.css"));
        }
    }
}