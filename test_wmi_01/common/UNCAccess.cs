using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using BOOL = System.Boolean;
using DWORD = System.UInt32;
using LPWSTR = System.String;
using NET_API_STATUS = System.UInt32;


/*
 * Interface:   
 *              int RemoteAccess(string UNCPath, string User, string Domain, string Password)
 *              int download_file(string UNCPath, string User, string Domain, string Password, string RemoteFilename, string LocalPath)
 * 
 */


namespace test_wmi_01
{
    public class UNCAccess
    {

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct USE_INFO_2
        {
            internal LPWSTR ui2_local;
            internal LPWSTR ui2_remote;
            internal LPWSTR ui2_password;
            internal DWORD ui2_status;
            internal DWORD ui2_asg_type;
            internal DWORD ui2_refcount;
            internal DWORD ui2_usecount;
            internal LPWSTR ui2_username;
            internal LPWSTR ui2_domainname;
        }

        [DllImport("NetApi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern NET_API_STATUS NetUseAdd(
            LPWSTR UncServerName,
            DWORD Level,
            ref USE_INFO_2 Buf,
            out DWORD ParmError);

        [DllImport("NetApi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern NET_API_STATUS NetUseDel(
            LPWSTR UncServerName,
            LPWSTR UseName,
            DWORD ForceCond);

        private string sUNCPath;
        private string sUser;
        private string sPassword;
        private string sDomain;
        private int iLastError;


    public UNCAccess()
    {
    }

    ~UNCAccess()
    {
        NetUseDelete();
    }

    public int RemoteAccess(string @UNCPath, string User, string Domain, string Password)
    {
        return login(UNCPath, User, Domain, Password);
    }

    public int download_file(string @UNCPath, string User, string Domain, string Password, string RemoteFilename, string LocalPath)
    {
        try
        {
            UNCAccess unc = new UNCAccess();
            unc.RemoteAccess(UNCPath, User, Domain, Password);
            System.IO.File.Copy(@UNCPath + RemoteFilename, @LocalPath + RemoteFilename);
            System.Console.WriteLine("Copy from remote windows succeed.");
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e);
            return -1;
        }
        return 0;
    
    }



    public int LastError
    {
        get { return iLastError; }
    }
    ///

    /// Connects to a UNC share folder with credentials
    ///
    /// UNC share path
    /// Username
    /// Domain
    /// Password
    /// True if login was successful
    public int login(string UNCPath, string User, string Domain, string Password)
    {
        sUNCPath = UNCPath;
        sUser = User;
        sPassword = Password;
        sDomain = Domain;
        return NetUseWithCredentials();
    }


    private int NetUseWithCredentials()
    {
        uint returncode;
        try
        {
            USE_INFO_2 useinfo = new USE_INFO_2();

            useinfo.ui2_remote = sUNCPath;
            useinfo.ui2_username = sUser;
            useinfo.ui2_domainname = sDomain;
            useinfo.ui2_password = sPassword;
            useinfo.ui2_asg_type = 0;
            useinfo.ui2_usecount = 1;
            uint paramErrorIndex;
            returncode = NetUseAdd(null, 2, ref useinfo, out paramErrorIndex);
            System.Console.WriteLine("NEtUseAdd ReturnValue " + returncode);
            iLastError = (int)returncode;
            return (int)returncode;
        }
        catch(Exception e)
        {
            iLastError = Marshal.GetLastWin32Error();
            System.Console.WriteLine(e);
            return -1;
        }
    }

        ///
        /// Closes the UNC share
        ///
        /// True if closing was successful
        public bool NetUseDelete()
        {
        uint returncode;
            try
            {
                returncode = NetUseDel(null, sUNCPath, 2);
                iLastError = (int)returncode;
                return (returncode == 0);
            }
            catch
            {
                iLastError = Marshal.GetLastWin32Error();
                return false;
            }
        }
    }
}




