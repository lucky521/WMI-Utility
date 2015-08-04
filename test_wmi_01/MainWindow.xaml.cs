using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace test_wmi_01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            System.Console.WriteLine("\n\n###########################################################");
            System.Console.WriteLine("Begin!");

            
            string my_password = "****";


            //Test for CheckService
            /*
            CheckService cs = new CheckService("10.117.172.201", "luliu", my_password, "LIULUVIEW");
            string result = cs.check_service("v4v_broker_agent");
            Console.WriteLine("State of The Service is "  + result);
            */

            //Test for RegistryOperate
            /*
            RegistryOperate rop = new RegistryOperate("10.117.172.201","luliu", my_password, "LIULUVIEW");
            string data;
            //rop.read_registry((uint)(RegistryOperate.RegHive.HKEY_LOCAL_MACHINE),  @"SOFTWARE\vmware, inc.\vcenter operations for view\broker agent",  "liulu", out data);
            rop.read_registry((uint)(RegistryOperate.RegHive.HKEY_LOCAL_MACHINE), @"SOFTWARE\Teradici", "liulute", out data);
            System.Console.WriteLine(data);
            */
            //Test for InstallerCheck
            //InstallerCheck ich = new InstallerCheck("10.117.172.202", "luliu", my_password, "LIULUVIEW");
            //System.Console.WriteLine(ich.list_installer());


            
            //Test for ScpOperate
            //ScpOperate.download_file("10.117.172.203", "root", my_password, "remote_luliu.log", @"/storage/vcops/log/adapters/V4VAdapter/", @"e:\");
            //ScpOperate scp = new ScpOperate();
            //int x = ScpOperate.download_file("10.117.172.203", "root", "Vca$hc0w", "agent-config.xml", "/usr/lib/vmware-vcops/tomcat-web-app/webapps/ui/ContentPack/V4V/conf/", "c:\\");
            //System.Console.WriteLine("download_file return value:" + x);
            
            //Test for remote windows folder access
            /*
            UNCAccess unc = new UNCAccess();
            int result = unc.RemoteAccess(@"\\10.117.172.204\c$", "luliu", "LIULUVIEW", my_password);
            System.Console.WriteLine(result);
             */

            /*
            string filename = "v4v-truststore.jks";
            result = unc.download_file(@"\\10.117.172.204\c$\ProgramData\VMware\vCenter Operations for View\conf\", "luliu", "LIULUVIEW", my_password, filename, @"E:\");
            System.Console.WriteLine(result);
            */



            System.Console.WriteLine("End!");
            System.Console.WriteLine("###########################################################\n\n");
        }

    }


}
