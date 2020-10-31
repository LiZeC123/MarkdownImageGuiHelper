using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;

namespace MarkdownImageGuiHelper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            txtPrefix.Text = Properties.Settings.Default.LastPrefix;
            cmbSelectType.Items.Add("Slide");
            cmbSelectType.Items.Add("Article");
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsImage())
            {
                try
                {
                    var path = GetLocationBySelect(cmbSelectType.Text);
                    SavingFile saving = GetFileName(path);

                    Clipboard.GetImage().Save(saving.FullName(path), ImageFormat.Jpeg);
                    
                    var markname = GetMarkdownName(cmbSelectType.Text, saving);
                    Clipboard.SetText(markname);

                    txtName.Text = string.Empty;
                }
                catch (Exception)
                {
                    MessageBox.Show("数据处理异常");
                }
            }
            else
            {
                MessageBox.Show("剪切板中不包含图片数据");
            }
        }


        private SavingFile GetFileName(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var prefix = txtPrefix.Text;

            var floder = Path.Combine(path, prefix);
            if (!Directory.Exists(floder))
            {
                Directory.CreateDirectory(floder);
            }


            SavingFile file = GenerateUserGivenName(path, prefix, txtName.Text);
            if (file == null)
            {
                file = GenerateNumberName(path, prefix);
            }

            return file;
        }

        private SavingFile GenerateUserGivenName(string path, string prefix, string userGivenName)
        {
            if(string.IsNullOrWhiteSpace(userGivenName))
            {
                return null;
            }

            string fullname = Path.Combine(path, prefix, $"{userGivenName},jpg");
            if(File.Exists(fullname))
            {
                return null;
            }

            return new SavingFile(prefix, $"{userGivenName}.jpg");
        }

        private SavingFile GenerateNumberName(string path, string prefix)
        {
            string name = Enumerable.Range(1, 1000)
                .Select(n => $"{n}.jpg")
                .First(filename => !File.Exists(Path.Combine(path, prefix, filename)));
            return new SavingFile(prefix, name);
        }



        private string GetMarkdownName(string selectType, SavingFile saving)
        {
            switch (selectType)
            {
                // 由于项目的结构, Article由于创建较早, 使用绝对路径, 而每个Slide使用相对路径
                case "Slide": return $"![image](images/{saving.Prefix}/{saving.FileName})";
                case "Article": return $"![image](/images/{saving.Prefix}/{saving.FileName})";
                default: throw new NotSupportedException("不支持的类型");
            }
        }

        private string GetLocationBySelect(string name)
        {
            switch (name)
            {
                case "Slide": return Properties.Settings.Default.SlideImageLocation;
                case "Article": return Properties.Settings.Default.ArticleImageLocation;
                default: throw new NotSupportedException("不支持的类型");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.LastPrefix = txtPrefix.Text;
            Properties.Settings.Default.Save();
        }
    }
}
