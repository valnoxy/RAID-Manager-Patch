using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.ServiceProcess;

namespace RAIDManagerPatch
{
    internal class Program
    {
        public static string MegaRAID = "C:\\Program Files (x86)\\MegaRAID Storage Manager";
        public static string maxView = "C:\\Program Files\\Adaptec\\maxView Storage Manager\\apache-tomcat\\webapps\\maxview\\WEB-INF\\lib";

        static void Main(string[] args)
        {
            Console.WriteLine("[i] Log4j Patcher for RAID Managers [Version: 1.0.0]");
            Console.WriteLine("[i] by valnoxy (https://valnoxy.dev)");
            Console.WriteLine("\n[i] This tool is open source! See: https://github.com/valnoxy/RAID-Manager-Patch");

            string service = CheckSys();

            RunService(service, false);

            string log4j = String.Empty;
            if (service == "MSMFramework") // MegaRAID Storage Manager
            {
                if (File.Exists(Path.Combine(MegaRAID, "log4j-1.2.15.jar"))) // Checked with version 17.05.02.01, 16.06.04.00, 15.11.00.13
                    log4j = "log4j-1.2.15.jar";
            }

            if (service == "maxViewWebServer") // maxView Storage Manager
            {
                if (File.Exists(Path.Combine(maxView, "log4j-core-2.16.0.jar")))
                    log4j = "2.16.0";
                if (File.Exists(Path.Combine(maxView, "log4j-core-2.15.0.jar")))
                    log4j = "2.15.0";
                if (File.Exists(Path.Combine(maxView, "log4j-core-2.14.1.jar")))
                    log4j = "2.14.1";
                if (File.Exists(Path.Combine(maxView, "log4j-core-2.14.0.jar")))
                    log4j = "2.14.0";
                if (File.Exists(Path.Combine(maxView, "log4j-core-2.13.3.jar")))
                    log4j = "2.13.3";
                if (File.Exists(Path.Combine(maxView, "log4j-core-2.13.2.jar")))
                    log4j = "2.13.2";
                if (File.Exists(Path.Combine(maxView, "log4j-core-2.13.1.jar")))
                    log4j = "2.13.1";
                if (File.Exists(Path.Combine(maxView, "log4j-core-2.13.0.jar")))
                    log4j = "2.13.0";
                if (File.Exists(Path.Combine(maxView, "log4j-core-2.12.4.jar")))
                    log4j = "2.12.4";
                if (File.Exists(Path.Combine(maxView, "log4j-core-2.12.3.jar")))
                    log4j = "2.12.3";
                if (File.Exists(Path.Combine(maxView, "log4j-core-2.12.2.jar")))
                    log4j = "2.12.2";
                if (File.Exists(Path.Combine(maxView, "log4j-core-2.12.1.jar")))
                    log4j = "2.12.1";
                if (File.Exists(Path.Combine(maxView, "log4j-core-2.12.0.jar")))
                    log4j = "2.12.0";
                if (File.Exists(Path.Combine(maxView, "log4j-core-2.11.2.jar")))
                    log4j = "2.11.2";
                if (File.Exists(Path.Combine(maxView, "log4j-core-2.11.1.jar")))
                    log4j = "2.11.1";
                if (File.Exists(Path.Combine(maxView, "log4j-core-2.11.0.jar")))
                    log4j = "2.11.0";
                if (File.Exists(Path.Combine(maxView, "log4j-core-2.10.0.jar")))
                    log4j = "2.10.0";
                if (File.Exists(Path.Combine(maxView, "log4j-core-2.9.1.jar")))
                    log4j = "2.9.1";
                if (File.Exists(Path.Combine(maxView, "log4j-core-2.9.0.jar")))
                    log4j = "2.9.0";
                if (File.Exists(Path.Combine(maxView, "log4j-core-2.8.2.jar")))
                    log4j = "2.8.2";
                if (File.Exists(Path.Combine(maxView, "log4j-core-2.8.1.jar")))
                    log4j = "2.8.1";
                if (File.Exists(Path.Combine(maxView, "log4j-core-2.8.0.jar")))
                    log4j = "2.8.0";
                if (File.Exists(Path.Combine(maxView, "log4j-core-2.7.0.jar")))
                    log4j = "2.7.0";
                if (File.Exists(Path.Combine(maxView, "log4j-core-2.6.2.jar")))
                    log4j = "2.6.2";
                if (File.Exists(Path.Combine(maxView, "log4j-core-2.6.1.jar")))
                    log4j = "2.6.1";
                if (File.Exists(Path.Combine(maxView, "log4j-core-2.6.0.jar")))
                    log4j = "2.6.0";
                if (File.Exists(Path.Combine(maxView, "log4j-core-2.5.0.jar")))
                    log4j = "2.5.0";
            }

            if (String.IsNullOrEmpty(log4j))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[!] This Version of the RAID Manager is too old. Please update it before using this patch.");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("[!] Restart service ...");
                RunService(service, true);
                
                Console.WriteLine("[!] Terminating in 10 sec ...");
                System.Threading.Thread.Sleep(10000);
                Environment.Exit(-1);
            }

            Console.WriteLine("[i] Removing vulnerable classes from jar file ...");

            if (service == "MSMFramework") // MegaRAID Storame Manager
                RemoveClass(service, Path.Combine(MegaRAID, log4j));

            if (service == "maxViewWebServer") // maxView Storage Manager
                UpdateClass(maxView, "https://dl.exploitox.de/other/vuln/log4j/v2.17.1/log4j-core-2.17.1.jar", log4j, "core");

            RunService(service, true);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[i] The RAID manager was successfully patched! Closing ...");
            Console.ForegroundColor = ConsoleColor.White;
            System.Threading.Thread.Sleep(5000);
        }

        #region Check System
        static string CheckSys()
        {
            Console.WriteLine("[i] Searching for RAID Manager ...");
            if (Directory.Exists(MegaRAID))
            {
                Console.WriteLine("[i] MegaRAID Storage Manager found!");
                return "MSMFramework";
            }
            if (Directory.Exists(maxView))
            {
                Console.WriteLine("[i] maxView Storage Manager found!");
                return "maxViewWebServer";
            }
            else
            {
                Console.WriteLine("[!] Error: Cannot find any RAID managers on this device.");
                System.Threading.Thread.Sleep(5000);
                Environment.Exit(-1);
                return "";
            }
        }
        #endregion
        #region Start / Stop Service
        private static void RunService(string servicename, bool v)
        {
            // Check whether the apcpbeagent service is started.
            ServiceController sc = new ServiceController();
            sc.ServiceName = servicename;
            Console.WriteLine($"[i] The {servicename} service status is currently set to {sc.Status}");

            if (v == true)
            {
                if (sc.Status != ServiceControllerStatus.Running)
                {
                    // Start the service if the current status is stopped.
                    Console.WriteLine($"[i] Starting the {servicename} service ...");
                    try
                    {
                        // Start the service, and wait until its status is "Running".
                        sc.Start();
                        sc.WaitForStatus(ServiceControllerStatus.Running);

                        // Display the current service status.
                        Console.WriteLine($"[i] The {servicename} service status is now set to {sc.Status}.");
                    }
                    catch (InvalidOperationException)
                    {
                        Console.WriteLine($"[!] Could not start the {servicename} service.");
                        System.Threading.Thread.Sleep(5000);
                        Environment.Exit(-1);
                    }
                }
            }

            if (v == false)
            {
                if (sc.Status != ServiceControllerStatus.Stopped)
                {
                    // Stop the service if the current status is started.
                    Console.WriteLine($"[i] Stopping the {servicename} service ...");
                    try
                    {
                        // Stop the service, and wait until its status is "Stopped".
                        sc.Stop();
                        sc.WaitForStatus(ServiceControllerStatus.Stopped);

                        // Display the current service status.
                        Console.WriteLine($"[i] The {servicename} service status is now set to {sc.Status}.");
                    }
                    catch (InvalidOperationException)
                    {
                        Console.WriteLine($"[!] Could not stop the {servicename} service.");
                        System.Threading.Thread.Sleep(5000);
                        Environment.Exit(-1);
                    }
                }
            }
        }
        #endregion
        #region Update / Remove class
        static void RemoveClass(string servicename, string file)
        {
            try
            {
                using (ZipArchive zip = ZipFile.Open(@file, ZipArchiveMode.Update))
                {
                    zip.Entries.Where(x => x.FullName.Contains("JndiManager.class")).ToList()
                        .ForEach(y =>
                        {
                            zip.GetEntry(y.FullName).Delete();
                            Console.WriteLine("[i] Removing: JndiManager.class");
                        });
                    zip.Entries.Where(x => x.FullName.Contains("JndiLookup.class")).ToList()
                        .ForEach(y =>
                        {
                            zip.GetEntry(y.FullName).Delete();
                            Console.WriteLine("[i] Removing: JndiLookup.class");
                        });
                    zip.Entries.Where(x => x.FullName.Contains("SocketNode.class")).ToList()
                        .ForEach(y =>
                        {
                            zip.GetEntry(y.FullName).Delete();
                            Console.WriteLine("[i] Removing: SocketNode.class");
                        });
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[!] An error has occurred while updating the file:\n\n" + ex);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("[!] Restart service ...");
                RunService(servicename, true);

                Console.WriteLine("[!] Terminating in 10 sec ...");
                System.Threading.Thread.Sleep(10000);
                Environment.Exit(-1);
            }
            Console.WriteLine($"[i] File {file} successfully updated.");
        }

        private static void UpdateClass(string path, string url, string log4jver, string log4jmodule)
        {
            using (var client = new WebClient())            // TODO: Switch to HttpClient
            {
                // =================================
                // =      Removing old Class       =
                // =================================
                try
                {
                    Console.Write($"[i] Removing log4j-{log4jmodule}-{log4jver}.jar ... ");
                    File.Delete(Path.Combine(path, $"log4j-{log4jmodule}-{log4jver}.jar"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n[!] An error has occurred while removing: {ex}");
                    System.Threading.Thread.Sleep(5000);
                    Environment.Exit(-1);
                }
                Console.WriteLine("OK!");

                // =================================
                // =      Downloading Class        =
                // =================================
                try
                {
                    Console.Write($"[i] Downloading log4j-{log4jmodule}-2.17.1.jar ... ");
                    client.DownloadFile(url, Path.Combine(path, $"log4j-{log4jmodule}-2.17.1.jar"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n[!] An error has occurred while downloading: {ex}");
                    System.Threading.Thread.Sleep(5000);
                    Environment.Exit(-1);
                }
                Console.WriteLine("OK!");
            }
        }
        #endregion
    }
}
