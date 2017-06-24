using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CATC.FIDS.Utils
{
    public class FileMethods
    {
        //读文件
        public static string readFile(string fileName)
        {
            try
            {
                string content = "";//返回的字符串

                // 以只读模式打开一个文本文件
                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(fs, Encoding.UTF8))
                    {
                        string text = string.Empty;

                        while (!reader.EndOfStream)
                        {
                            text = reader.ReadLine();
                            content = text;
                        }
                    }
                }

                return content;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        //写文件
        public static bool writeFile(string path, string str)
        {

            try
            {
                //如果文件path存在就打开，不存在就新建 .append 是追加写, CreateNew 是覆盖
                FileStream fst = new FileStream(path, FileMode.Append);
                StreamWriter swt = new StreamWriter(fst, System.Text.Encoding.GetEncoding("utf-8"));

                //写入
                swt.WriteLine(str);
                swt.Close();
                fst.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        //读取文件名字
        public static List<string> getFileName(string path)
        {
            try
            {
                DirectoryInfo folder = new DirectoryInfo(path);
                List<string> filenames = new List<string>();

                foreach (FileInfo file in folder.GetFiles("*.txt"))
                {
                    filenames.Add(file.FullName);
                }
                return filenames;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        //清空指定的文件夹，但不删除文件夹
        public static bool DeleteFolder(string dir)
        {
            try
            {
                foreach (string d in Directory.GetFileSystemEntries(dir))
                {
                    if (File.Exists(d))
                    {
                        FileInfo fi = new FileInfo(d);
                        if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                            fi.Attributes = FileAttributes.Normal;
                        File.Delete(d);//直接删除其中的文件  
                    }
                    else
                    {
                        DirectoryInfo d1 = new DirectoryInfo(d);
                        if (d1.GetFiles().Length != 0)
                        {
                            DeleteFolder(d1.FullName);////递归删除子文件夹
                        }
                        Directory.Delete(d);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        //删除文件夹及其内容
        public static bool DeleteFolder1(string dir)
        {
            try
            {
                foreach (string d in Directory.GetFileSystemEntries(dir))
                {
                    if (File.Exists(d))
                    {
                        FileInfo fi = new FileInfo(d);
                        if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                            fi.Attributes = FileAttributes.Normal;
                        File.Delete(d);//直接删除其中的文件  
                    }
                    else
                        DeleteFolder(d);////递归删除子文件夹
                    Directory.Delete(d);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        //删除文件
        public static bool DeleteOneFile(string pSavedPath1)
        {
            try
            {
                if (File.Exists(pSavedPath1))
                {
                    FileInfo fi = new FileInfo(pSavedPath1);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                        fi.Attributes = FileAttributes.Normal;
                    File.Delete(pSavedPath1);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
