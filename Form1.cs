using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms;
using System.Diagnostics;
using System.Management;

namespace Windows_Optimiser_v2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string tempPath = Path.GetTempPath();
            int deletedFiles = 0;

            try
            {
                DirectoryInfo tempDir = new DirectoryInfo(tempPath);

                foreach (FileInfo file in tempDir.GetFiles())
                {
                    try
                    {
                        file.Delete();
                        deletedFiles++;
                    }
                    catch {  }
                }

                foreach (DirectoryInfo dir in tempDir.GetDirectories())
                {
                    try
                    {
                        dir.Delete(true);
                        deletedFiles++;
                    }
                    catch { }
                }

                MessageBox.Show($"Deleted {deletedFiles} temp files/folders.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cleaning temp files:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                RegistryKey rkUser = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Run", true);

                if (rkUser != null)
                {
                    foreach (string appName in rkUser.GetValueNames())
                    {
                        rkUser.DeleteValue(appName, false);
                    }
                    rkUser.Close();
                }

                RegistryKey rkMachine = Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

                if (rkMachine != null)
                {
                    foreach (string appName in rkMachine.GetValueNames())
                    {
                        rkMachine.DeleteValue(appName, false);
                    }
                    rkMachine.Close();
                }

                MessageBox.Show("All startup programs have been disabled.", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error disabling startup programs:\n{ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string highPerfGUID = "8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c";
                Process.Start(new ProcessStartInfo
                {
                    FileName = "powercfg",
                    Arguments = $"/setactive {highPerfGUID}",
                    CreateNoWindow = true,
                    UseShellExecute = false
                });

                MessageBox.Show("High-Performance power plan activated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting power plan:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string[] xboxServices = {
        "XboxGipSvc",          
        "XblAuthManager",     
        "XblGameSave",        
        "XboxNetApiSvc"       
    };

            int stoppedCount = 0;

            try
            {
                foreach (string svcName in xboxServices)
                {
                    try
                    {
                        ServiceController sc = new ServiceController(svcName);
                        if (sc.Status != ServiceControllerStatus.Stopped && sc.Status != ServiceControllerStatus.StopPending)
                        {
                            sc.Stop();
                            sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(5));
                            stoppedCount++;
                        }
                    }
                    catch {  }
                }

                MessageBox.Show($"Stopped {stoppedCount} Xbox/Windows services for gaming.", "Boost FPS",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error stopping services:\n{ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                string processName = "HD-Player"; 
                Process[] procs = Process.GetProcessesByName(processName);
                int updated = 0;

                foreach (Process proc in procs)
                {
                    try
                    {
                        proc.PriorityClass = ProcessPriorityClass.High; 
                        updated++;
                    }
                    catch {  }
                }

                MessageBox.Show($"Updated CPU priority for {updated} process(es) to High.", "CPU Priority",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting CPU priority:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                ManagementScope scope = new ManagementScope(@"\\localhost\root\default");
                scope.Connect();

                ManagementClass mc = new ManagementClass(scope, new ManagementPath("SystemRestore"), null);
                ManagementBaseObject inParams = mc.GetMethodParameters("CreateRestorePoint");

                inParams["Description"] = "Game Boost Restore Point"; 
                inParams["RestorePointType"] = 0; 
                inParams["EventType"] = 100; 

                ManagementBaseObject outParams = mc.InvokeMethod("CreateRestorePoint", inParams, null);

                uint result = (uint)outParams.Properties["ReturnValue"].Value;

                if (result == 0)
                    MessageBox.Show("Restore point created successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show($"Failed to create restore point. Error code: {result}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating restore point:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select a folder to clean";
                folderDialog.ShowNewFolderButton = false;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = folderDialog.SelectedPath;
                    int deletedFiles = 0;
                    int deletedFolders = 0;

                    try
                    {
                        DirectoryInfo dirInfo = new DirectoryInfo(folderPath);

                        foreach (FileInfo file in dirInfo.GetFiles())
                        {
                            try
                            {
                                file.Delete();
                                deletedFiles++;
                            }
                            catch { }
                        }

                        foreach (DirectoryInfo dir in dirInfo.GetDirectories())
                        {
                            try
                            {
                                dir.Delete(true); 
                                deletedFolders++;
                            }
                            catch { }
                        }

                        MessageBox.Show($"Deleted {deletedFiles} files and {deletedFolders} folders from:\n{folderPath}",
                                        "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error cleaning folder:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}