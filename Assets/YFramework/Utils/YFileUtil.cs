using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace GameFramwork
{
    public class YFileUtil : Singleton<YFileUtil>
    {
        /// <summary>
        /// 将代码类序列化成代码文件
        /// </summary>
        /// <param name="createPath"></param>
        /// <param name="unit"></param>
        /// <param name="replace"></param>
        public void CreatCodeFile(string createPath, string fileName, CodeCompileUnit unit, bool replace = true)
        {
            if (!Directory.Exists(createPath))
            {
                Directory.CreateDirectory(createPath);
            }

            var filePath = createPath + "/" + fileName;
            if (File.Exists(filePath) && !replace)
            {
                Debug.LogError("文件已经存在!");
            }
            else
            {
                FileStream fs = new FileStream(filePath, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);

                CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
                CodeGeneratorOptions options = new CodeGeneratorOptions();
                options.BracingStyle = "C";
                options.BlankLinesBetweenMembers = true;
                options.IndentString = "    ";
                provider.GenerateCodeFromCompileUnit(unit, sw, options);

                sw.Close();
                fs.Close();
                Debug.LogFormat("生成成功，存放路径{0}", filePath);
            }
        }

        /// <summary>
        /// 读取文本内容
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string RedTxtStr(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogError("文件路径不存在:" + path);
                return "";
            }

            FileStream fs = new FileStream(path, FileMode.Open);
            StreamReader sr = new StreamReader(fs, Encoding.Default);
            string res = sr.ReadToEnd();
            sr.Close();
            fs.Close();
            return res;
        }
    }
}
