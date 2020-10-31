using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownImageGuiHelper
{
    /// <summary>
    /// 表示需要存储的文件的必要信息
    /// </summary>
    class SavingFile
    {

        public SavingFile(string prefix, string filename)
        {
            Prefix = prefix;
            FileName = filename;
        }

        public string FileName { get; }
        public string Prefix { get; }

        public string FullName(String path)
        {
            return Path.Combine(path, Prefix, FileName);
        }
    }
}
