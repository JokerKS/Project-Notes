using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Project.Data
{
    class mYFile
    {
        private static string format = ".jks";
        private static string key = "KażtomęczyK";

        private string filename = null;
        private string shortfilename = null;

        public mYFile() { }

        public string ShortFileName
        {
            get { return shortfilename; }
        }

        public void RemoveFileName()
        {
            filename = shortfilename = null;
        }

        public void SaveAsToFile(string data)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "JKSnot";
            sfd.DefaultExt = format;
            sfd.Filter = "JKSnot documents (.jks)|*.jks";

            //https://msdn.microsoft.com/uk-ua/library/2cf62fcy.aspx
            bool? result = sfd.ShowDialog();

            if (result == true)
            {
                filename = sfd.FileName;
                shortfilename = Path.GetFileName(sfd.FileName);
                FileInfo file = new FileInfo(sfd.FileName);
                if (file.Exists) file.Delete();
                StreamWriter wr_text = file.AppendText();
                Encryption(ref data);
                wr_text.WriteLine(data);
                wr_text.Close();
            }
        }

        public void SaveToFile(string data)
        {
            if (filename != null)
            {
                StreamWriter wr_text;
                FileInfo file = new FileInfo(filename);
                if (file.Exists) file.Delete();
                wr_text = file.AppendText();
                Encryption(ref data);
                wr_text.WriteLine(data);
                wr_text.Close();
            }
            else SaveAsToFile(data);
        }

        private static void Encryption(ref string data)
        {
            var enc = new UTF32Encoding();
            Byte[] encodedKey = enc.GetBytes(key);

            Byte[] encodedData = enc.GetBytes(data);

            for (int i = 0, k = 0; i < encodedData.Length; i++)
            {
                encodedData[i] += encodedKey[k];
                if (k == encodedKey.Length - 1)
                    k = 0;
                else k++; 
            }
            data = enc.GetString(encodedData);
        }

        public string OpenFromFile()
        {
            string text = null;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = format;
            ofd.Filter = "JKSnot documents (.jks)|*.jks";

            if (ofd.ShowDialog() == true)
            {
                filename = ofd.FileName;
                shortfilename = Path.GetFileName(ofd.FileName);
                text = File.ReadAllText(ofd.FileName);
                text = text.Remove(text.Length - 2, 2);
                Decryption(ref text);
            }
            return text;
        }

        private static void Decryption(ref string data)
        {
            var enc = new UTF32Encoding();
            Byte[] encodedKey = enc.GetBytes(key);
            Byte[] encodedData = enc.GetBytes(data);

            for (int i = 0, k = 0; i < encodedData.Length; i++)
            {
                encodedData[i] -= encodedKey[k];
                if (k == encodedKey.Length - 1)
                    k = 0;
                else k++;
            }
            data = enc.GetString(encodedData);
        }
    }
}
