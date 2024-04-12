using System.Web;
using System.Web.Optimization;

namespace SuperbrainManagement
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/signalR").Include(
                       "~/Scripts/jquery.signalR-{version}.js"));
        }
    }
}
