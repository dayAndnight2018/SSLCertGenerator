using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreateCertification
{
    public partial class Form1 : Form
    {
        private  String CREATE_KEY = null;
        private String CREATE_PEM = null;
        private String CREATE_P12 = null;
        String currentPath = Environment.CurrentDirectory.ToString();
        String binPath = null;
        String certificationPath =null;
        String exePath = null;
        String configPath = null;
        String keyPath = null;
        String p12Path = null;
        String pemPath = null;
        Process process;
        public Form1()
        {
            InitializeComponent();

            binPath = currentPath.Substring(0, currentPath.LastIndexOf("\\"));
            certificationPath = binPath + "\\certifications";
            keyPath = certificationPath+"\\my.key";
            pemPath = certificationPath + "\\my.pem";
            p12Path = certificationPath+"\\my.p12";
            exePath = binPath + "\\tools\\openssl.exe";
            configPath = binPath + "\\tools\\openssl.cnf";
            CREATE_KEY = "genrsa -out " + keyPath + " 2048";
            CREATE_P12 = "pkcs12 -export -out " + p12Path + " -inkey " + keyPath + " -in " + pemPath + " -passout pass:";
            process = new Process();
            if (!File.Exists("c:\\Windows\\System32\\ssleay32.dll"))
            {
                try
                {
                    File.Copy(binPath + "\\tools\\ssleay32.dll", "c:\\Windows\\System32\\ssleay32.dll", true);
                    ProcessStartInfo startInfo = new ProcessStartInfo("c:\\Windows\\System32\\regsvr32.exe", "ssleay32.dll");
                    startInfo.CreateNoWindow = true;
                    startInfo.RedirectStandardInput = true;
                    startInfo.RedirectStandardOutput = true;
                    startInfo.UseShellExecute = false;
                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();
                    process.Close();
                    if (process.ExitCode != 0)
                    {
                        MessageBox.Show("导入库文件ssleay32.dll失败！");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    process.Close();
                }
                
            }
            if (!File.Exists("c:\\Windows\\System32\\libeay32.dll"))
            {
                try
                {
                    File.Copy(binPath + "\\tools\\libeay32.dll", "c:\\Windows\\System32\\libeay32.dll", true);
                    ProcessStartInfo startInfo = new ProcessStartInfo("c:\\Windows\\System32\\regsvr32.exe", "libeay32.dll");
                    startInfo.CreateNoWindow = true;
                    startInfo.RedirectStandardInput = true;
                    //startInfo.RedirectStandardOutput = true;
                    startInfo.UseShellExecute = false;
                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();
                    process.Close();
                    if (process.ExitCode != 0)
                    {
                        MessageBox.Show("导入库文件libeay32.dll失败！");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    process.Close();
                }
                
            }                     
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ipAddress.Text.ToString()))
            {
                MessageBox.Show("ip地址为空！");
                return;
            }
            String ip = ipAddress.Text.ToString().Trim();
           CREATE_PEM = "req -new -x509 -days 3650 -key " + keyPath + " -out " + pemPath + " -subj \"/C=CN/ST=China/L=Beijing/O=Beijing/OU=Lab/CN=" + ip +"\""+ " -config " + configPath;
            ClearFiles();

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(exePath);
                startInfo.CreateNoWindow = true;
                startInfo.RedirectStandardInput = true;
                startInfo.RedirectStandardOutput = true;        
                startInfo.Arguments = CREATE_KEY;
                startInfo.UseShellExecute = false;
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    MessageBox.Show("生成Key失败！");
                    return;
                }
                process.Close();

                startInfo.Arguments = CREATE_PEM;
                process.Start();
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    MessageBox.Show("生成Pem失败！");
                    return;
                }
                process.Close();

                startInfo.Arguments = CREATE_P12;
                process.Start();
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    MessageBox.Show("生成P12失败！");
                    return;
                }
                process.Close();
                MessageBox.Show("证书生成成功！");
                Process.Start(certificationPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }       
        
        }

        private void ClearFiles()
        {
            if (File.Exists(keyPath))
            {
                File.Delete(keyPath);
            }
            if (File.Exists(pemPath))
            {
                File.Delete(pemPath);
            }
            if (File.Exists(p12Path))
            {
                File.Delete(p12Path);
            }
        }

    }
}
