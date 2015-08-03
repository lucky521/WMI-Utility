using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management; //wmi



/*
 * Interface:   int read_registry(string key, string subkey, string value, out string data)
 *              string list_installer()
 *              
 * 
 */

namespace test_wmi_01
{
    public class wmi_base
    {
        public ConnectionOptions ops;
        public ManagementScope scope;
        public string address;
        public string username;
        public string password;
        public string domain;

        public wmi_base()
        { }

        public wmi_base(string address, string username, string password, string domain)
        {
            this.address = address;
            this.username = username;
            this.password = password;
            this.domain = domain;
        
        }

        ~wmi_base() {

        }

        public bool connection()
        {
            ops = new ConnectionOptions();
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

    }
    
    /* 
     * WMI - System Registry Provider
     * 
     */
    public class RegistryOperate: wmi_base
    {

        public RegistryOperate()
        {}

        public RegistryOperate(string address, string username, string password, string domain) : base(address, username, password, domain)
        {
          

        }


        public enum RegHive :uint
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



        public int read_registry(uint key, string subkey, string value, out string data)
        {
            string wmi_name = "StdRegProv";
            string wmi_method_name = "GetStringValue";
            data = "";
            if (connection())
            {
                try
                {
                    //ObjectGetOptions obj = new ObjectGetOptions(null, System.TimeSpan.MaxValue, true); 
                    ManagementPath mypath = new ManagementPath(wmi_name); //StdRegProv class contains methods that manipulate system registry keys and values
                    ManagementClass mgmtclass = new ManagementClass(scope, mypath, null);  


                    ManagementBaseObject inParams = mgmtclass.GetMethodParameters(wmi_method_name);
                    inParams["hDefKey"] = key;
                    inParams["sSubKeyName"] = subkey;
                    inParams["sValueName"] = value;

                    ManagementBaseObject outParams = mgmtclass.InvokeMethod(wmi_method_name, inParams, null);
                    Console.WriteLine(wmi_method_name + " return : " + outParams.Properties["ReturnValue"].Value);
                    if ((uint)outParams.Properties["ReturnValue"].Value == 0)
                    {
                        Console.WriteLine("Data >>>> " + (string)outParams.Properties["sValue"].Value);
                        data = (string)outParams.Properties["sValue"].Value;
                    }
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e);
                    return -1;
                }
            }
            return 0;
        }


    }

    /* 
     * WMI - System Installer Provider
     * 
     */

    public class InstallerCheck : wmi_base
    {

        public InstallerCheck(string address, string username, string password, string domain) : base(address, username, password, domain)
        {

        }


        public string list_installer()
        {
            StringBuilder result = new StringBuilder();
            string wmi_name = "Win32_Product";
            if (connection())
            {
                try
                {
                    ManagementPath mypath = new ManagementPath(wmi_name);
                    ManagementClass mgmtclass = new ManagementClass(scope, mypath, null);

                    ManagementObjectCollection instances = mgmtclass.GetInstances();

                    Console.WriteLine("Name, Vendor");
                    foreach (ManagementObject product in instances)
                    {
                        string tmp = String.Format("{0}, {1} ", product["Name"], product["Vendor"]);
                        Console.WriteLine(tmp);
                        result.Append(tmp + "\r\n");
                    }
                }
                catch(Exception e)
                {
                    System.Console.WriteLine(e);
                    return result.ToString();
                }

            }
            Console.WriteLine("That's all.");
            return result.ToString();
        }
 
    }

    public class CheckService : wmi_base
    {
        public string servicename;

        public CheckService(string address, string username, string password, string domain, string servicename) : base(address, username, password, domain)
        {
            this.servicename = servicename;
        }


        ~CheckService()
        {

        }

        public string check()
        {
            try
            {
                ConnectionOptions connection = new ConnectionOptions();
                connection.Username = username;
                connection.Password = password;
                connection.Authority = "ntlmdomain:" + domain;

                ManagementScope scope = new ManagementScope(
                    "\\\\" + address +"\\root\\CIMV2", connection);
                scope.Connect();

                ObjectQuery query = new ObjectQuery(
                    "SELECT * FROM Win32_Service");

                ManagementObjectSearcher searcher =
                    new ManagementObjectSearcher(scope, query);

                foreach (ManagementObject queryObj in searcher.Get())
                {
                    if (queryObj["Name"].ToString() == servicename)
                    {
                        /*
                        Console.WriteLine("-----------------------------------");
                        Console.WriteLine("Win32_Service instance");
                        Console.WriteLine("-----------------------------------");
                        Console.WriteLine("Caption: {0}", queryObj["Caption"]);
                        Console.WriteLine("Description: {0}", queryObj["Description"]);
                        Console.WriteLine("Name: {0}", queryObj["Name"]);
                        Console.WriteLine("PathName: {0}", queryObj["PathName"]);
                        Console.WriteLine("State: {0}", queryObj["State"]);
                        Console.WriteLine("Status: {0}", queryObj["Status"]);
                         */
                        return queryObj["State"].ToString();
                    }

                }

            }
            catch (ManagementException err)
            {
                Console.WriteLine("An error occured while querying for WMI data: "
                    + err.Message);
                return err.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return e.ToString();
            }

            return "Service Not Exist";
        }

    }


    

}
