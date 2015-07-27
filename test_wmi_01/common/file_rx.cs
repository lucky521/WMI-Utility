using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Globalization;
using System.IO;
using WinSCP;  // from winscpnet


namespace test_wmi_01
{
    class ScpOperate
    {
        public static int download_file(string @hostname, string @username, string @password, string @filename, string @remotepath, string @localpath)
        {
            string current_hostkey = "ssh-rsa 2048 b2:de:6e:f3:b2:a0:ef:0f:ae:dc:2d:7e:22:9b:b7:75"; //TODO: change to read from config file

            // If hostkey error in first time, modify it in the second time, and save it to config file.
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    // Setup session options
                    SessionOptions sessionOptions = new SessionOptions
                    {
                        Protocol = Protocol.Sftp,
                        HostName = hostname,
                        UserName = username,
                        Password = password,
                        SshHostKeyFingerprint = current_hostkey
                    };

                    using (Session session = new Session())
                    {
                        // Connect
                        session.Open(sessionOptions);
                        String remotePath = remotepath + filename;
                        String localPath = localpath + filename;

                        // You can achieve the same using:
                        // session.SynchronizeDirectories(
                        //     SynchronizationMode.Local, localPath, remotePath, false, false, SynchronizationCriteria.Time, 
                        //     new TransferOptions { FileMask = fileName }).Check();
                        if (session.FileExists(remotePath))
                        {
                            bool download;
                            if (!File.Exists(localPath))
                            {
                                Console.WriteLine("File {0} exists, local backup {1} does not. Begin to download.", remotePath, localPath);
                                download = true;
                            }
                            else
                            {
                                DateTime remoteWriteTime = session.GetFileInfo(remotePath).LastWriteTime;
                                DateTime localWriteTime = File.GetLastWriteTime(localPath);

                                if (remoteWriteTime > localWriteTime)
                                {
                                    Console.WriteLine(
                                        "File {0} as well as local backup {1} exist, " +
                                        "but remote file is newer ({2}) than local backup ({3})",
                                        remotePath, localPath, remoteWriteTime, localWriteTime);
                                    download = true;
                                }
                                else
                                {
                                    Console.WriteLine(
                                        "File {0} as well as local backup {1} exist, " +
                                        "but remote file is not newer ({2}) than local backup ({3})",
                                        remotePath, localPath, remoteWriteTime, localWriteTime);
                                    download = false;
                                }
                            }

                            if (download)
                            {
                                // Download the file and throw on any error
                                session.GetFiles(remotePath, localPath).Check();
                                Console.WriteLine("Download to backup done.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("File {0} does not exist yet", remotePath);
                        }
                    }
                    return 0;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e);
                    int offset = e.ToString().IndexOf("Host key fingerprint is ");  //Modify the fingerprint according to Error Message
                    if (offset != -1)
                    {
                        current_hostkey = e.ToString().Substring(offset + 24, 60);
                        Console.WriteLine(current_hostkey);
                    }
                    else
                        return -1;
                }
            }
            return -1;
        }
    }


   
}
