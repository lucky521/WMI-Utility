using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Management; //wmi

namespace test_wmi_01
{
    class wmi_base
    {
    }
    
    /* 
     * WMI - System Registry Provider
     * 
     */
    public class RegistryOperate: wmi_base
    {
        public ConnectionOptions ops;
        public ManagementScope scope;
        string address = "10.117.172.201";
        string username = "luliu";
        string password = @"******";
        string domain = "LIULUVIEW";

        public enum RegHive : uint
        {
            HKEY_CLASSES_ROOT = 0x80000000,
            HKEY_CURRENT_USER = 0x80000001,
            HKEY_LOCAL_MACHINE = 0x80000002,
            HKEY_USERS = 0x80000003,
            HKEY_CURRENT_CONFIG = 0x80000005
        };
        public enum RegType
        {
            REG_SZ = 1,
            REG_EXPAND_SZ,
            REG_BINARY,
            REG_DWORD,
            REG_MULTI_SZ = 7
        };

        public bool connection()
        {
            ops = new ConnectionOptions();
            //TODO: make them be input
            ops.Username = username;
            ops.Password = password;
            ops.Authority = @"ntlmdomain:" + domain;
            ops.EnablePrivileges = true;

            scope = new ManagementScope(@"\\" + address + @"\root\cimv2", ops);
            try
            {
                scope.Connect();
            }
            catch (Exception e)
            {
                Console.WriteLine("connect fail exception.");
                Console.WriteLine("Error: {0}", e);
                return false;
            }

            if (scope.IsConnected)
            {
                System.Console.WriteLine("connect success!");
                return true;
            }
            else
            {
                System.Console.WriteLine("connect fail!");
                return false;
            }
        }
        public int read_registry()
        {
            string wmi_name = "StdRegProv";
            string wmi_method_name = "GetStringValue";

            if (connection())
            {
                //ObjectGetOptions obj = new ObjectGetOptions(null, System.TimeSpan.MaxValue, true);  // 对象接收选项
                ManagementPath mypath = new ManagementPath(wmi_name); //StdRegProv class contains methods that manipulate system registry keys and values
                ManagementClass mgmtclass = new ManagementClass(scope, mypath, null);  // 生成远程的StdRegProv对象


                ManagementBaseObject inParams = mgmtclass.GetMethodParameters(wmi_method_name);
                inParams["hDefKey"] = RegHive.HKEY_LOCAL_MACHINE;
                inParams["sSubKeyName"] = @"SOFTWARE\VMware, Inc.\VMware Drivers";
                inParams["sValueName"] = "svga_wddm.status";

                ManagementBaseObject outParams = mgmtclass.InvokeMethod(wmi_method_name, inParams, null);
                Console.WriteLine(wmi_method_name + " return : " + outParams.Properties["ReturnValue"].Value);
                if ((uint)outParams.Properties["ReturnValue"].Value == 0)
                {   
                    Console.WriteLine("Data >>>> " + (string)outParams.Properties["sValue"].Value);
                }
            }
            return 0;
        }

        public void list_installer()
        {
            string wmi_name = "Win32_Product";
            if (connection())
            {
                ManagementPath mypath = new ManagementPath(wmi_name);
                ManagementClass mgmtclass = new ManagementClass(scope, mypath, null);

                ManagementObjectCollection instances = mgmtclass.GetInstances();

                Console.WriteLine("Name, Vendor");
                foreach (ManagementObject product in instances)
                {
                    Console.WriteLine(String.Format("{0}, {1} ", product["Name"], product["Vendor"]));
                }
            }
            Console.WriteLine("That's all.");
            return;
        }

    }

    public class InstallerCheck : wmi_base
    {
 
    }








}
