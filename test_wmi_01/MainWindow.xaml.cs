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
            System.Console.WriteLine("Beigin!");

            /*
            //Test for RegistryOperate
            RegistryOperate rop = new RegistryOperate();
            rop.read_registry();
            rop.list_installer();

            //Test for ScpOperate
            ScpOperate.download_file("10.117.172.203", "root", @"*****", "remote_luliu.log", @"/storage/vcops/log/adapters/V4VAdapter/", @"e:\");
             */

            //Test for remote windows folder access
            try
            {
                UNCAccess unc = new UNCAccess(@"\\10.117.172.204\c$", "luliu", "LIULUVIEW", @"*****");

                string filename = "v4v-truststore.jks";
                System.IO.File.Copy(@"\\10.117.172.204\c$\ProgramData\VMware\vCenter Operations for View\conf\" + filename, @"E:\" + filename);
                System.Console.WriteLine("Copy from remote windows succeed.");
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }






            System.Console.WriteLine("End!");
            System.Console.WriteLine("###########################################################\n\n");
        }

    }


}
