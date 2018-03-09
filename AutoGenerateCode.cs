//------------------------------------------------------------
// Game Framework v3.x
// Copyright © 2013-2018 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
// Add this file by Ron Tang.
//------------------------------------------------------------

using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;


namespace UnityGameFramework.Editor
{
    /// <summary>
    /// 生成数据表行代码
    /// </summary>
    internal static class AutoGenerateCode
    {
        private static string tablePath = "\\GameMain\\DataTables\\";
        private static string codePath = "\\GameMain\\Scripts\\DataTable\\";
        [MenuItem("Game Framework/AutoGenerateCode", false, 100)]
        private static void HandleAllDataTables()
        {

            //设置进度条  
            //EditorUtility.DisplayProgressBar("设置AssetName名称", "正在设置AssetName名称中...", 0.50f);
            //EditorUtility.ClearProgressBar();

            //路径  
            string fullPath = Application.dataPath + tablePath ;

            //获取指定路径下面的所有资源文件  
            if (Directory.Exists(fullPath))
            {
                DirectoryInfo direction = new DirectoryInfo(fullPath);
                FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

                Debug.Log(files.Length);

                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Name.EndsWith(".meta"))
                    {
                        continue;
                    }
                    //Debug.Log("Name:" + files[i].Name);
                    //Debug.Log( "FullName:" + files[i].FullName );  
                    //Debug.Log( "DirectoryName:" + files[i].DirectoryName );  
                    LoadFile(files[i].FullName);
                }
            }
        }


        static void LoadFile(string filePath)
        {
            using (StreamReader sr = File.OpenText(filePath))
            {
                string line;
                List<string> lines = new List<string>();
                int lineCount = 0;
                while ((line = sr.ReadLine()) != null && lineCount < 3)
                {
                    //Debug.Log(line);
                    lines.Add(line);
                    lineCount++;
                }
                sr.Close();
                sr.Dispose();
                HandleData(lines);
            }

        }
        static void HandleData(List<string> info)
        {
            string[] textOfTableNames = info[0].Split('\t');
            string textOfTableName = textOfTableNames[1];

            string[] textOfPropertyNames = info[1].Split('\t');
          
            string[] textOfPropertyTypeNames= info[2].Split('\t');
           

            StreamWriter sw;
            FileInfo t = new FileInfo(Application.dataPath+ codePath + textOfTableName + ".cs");
            sw = t.CreateText();

            WriteHeader(sw);
            WriteNameSpcace(sw, "StarForce");
            sw.WriteLine("{");
            sw.WriteLine(string.Format("public class {0} : IDataRow", textOfTableName));
            sw.WriteLine("{");
            WriteAllProperty(sw, textOfPropertyNames, textOfPropertyTypeNames);
            WriteParseDataRow(sw, textOfPropertyNames,textOfPropertyTypeNames);
            WriteAvoidJIT(sw, textOfTableName);
            sw.WriteLine("}");
            sw.WriteLine("}");
            sw.Flush();
            sw.Close();
            sw.Dispose();

        }

        static void WriteNameSpcace(StreamWriter sw, string name)
        {
            sw.WriteLine("namespace "+ name);
        }

        static void WriteAllProperty(StreamWriter sw, string[] textOfPropertyNames, string[] textOfPropertyTypeNames)
        {
            for (int i = 1; i < textOfPropertyNames.Length; i++)
            {
                if (string.IsNullOrEmpty(textOfPropertyTypeNames[i]))
                    continue;
                if (string.IsNullOrEmpty(textOfPropertyNames[i]))
                    continue;
                WriteProperty(sw, textOfPropertyTypeNames[i], textOfPropertyNames[i]);
               
            }
        }

        static void WriteHeader(StreamWriter sw)
        {
            sw.WriteLine("using GameFramework.DataTable;") ;
            sw.WriteLine("using System.Collections.Generic;");
        }



        static void WriteProperty(StreamWriter sw, string type, string name)
        {
            sw.WriteLine("  public"+" "+type+" "+ name);
            sw.WriteLine("  {");
            sw.WriteLine("    get;");
            sw.WriteLine("    protected set;");
            sw.WriteLine("  }");
            sw.WriteLine("");
        }

        static void WriteAvoidJIT(StreamWriter sw,string classTypeName)
        {
            sw.WriteLine("  private void AvoidJIT()");
            sw.WriteLine("  {");
            sw.WriteLine("    "+string.Format("new Dictionary<int, {0} > ();", classTypeName));
            sw.WriteLine("  }");
            
        }

        static void WriteParseDataRow(StreamWriter sw,string[] names,string[] types)
        {
            sw.WriteLine("  public void ParseDataRow(string dataRowText)");
            sw.WriteLine("  {");
            sw.WriteLine("    string[] text = dataRowText.Split('\\t');");
            sw.WriteLine("    int index = 0;");
            sw.WriteLine("    index++;");
            sw.WriteLine(string.Format("    {0} = {1}.Parse(text[index++]);",names[1],types[1]));
            sw.WriteLine("    index++;");
            for (int i = 2; i < names.Length; i++)
            {
                if(string.IsNullOrEmpty(names[i])) continue;

                if(types[i]!="string")
                    sw.WriteLine(string.Format("    {0} = {1}.Parse(text[index++]);", names[i],types[i]));
                else
                    sw.WriteLine(string.Format("    {0} = text[index++];", names[i]));
            }
              
            sw.WriteLine("  }");

        }

        
    }
}
