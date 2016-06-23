using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using Project.Data;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class mYFileTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            string first_text;
            string coder_text;
            string decoder_text;
            bool testflag = true;
            StreamWriter wr_text;

            FileInfo f = new FileInfo("Test.txt");
            if (f.Exists) f.Delete();
            wr_text = f.AppendText();
            for (int i = 0; i < 1000000; i++)
            {
                first_text = GenerateString();
                mYFile file = new mYFile();

                coder_text = first_text;
                mYFile.Encryption(ref coder_text);
                decoder_text = coder_text;
                mYFile.Decryption(ref decoder_text);

                if (first_text.Equals(decoder_text))
                {
                    wr_text.WriteLine("true");
                }
                else
                {
                    testflag = false;
                    wr_text.WriteLine("false + "+i);
                    break;
                }
            }
            wr_text.Close();
            Assert.IsTrue(testflag,"Error");
        }

        private string GenerateString()
        {
            Random rnd = new Random();
            int length = rnd.Next(0, 1000);

            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < length; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * rnd.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }
    }
}
