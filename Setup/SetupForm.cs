using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using GMLib;

namespace GMSpy.SetupInstaller
{
    public partial class SetupForm : Form
    {
        static readonly string GETTOKEN_URL = "http://gmaster.youzijie.com/register/getToken";
        static readonly string CONFIG_FILE = "config.properties";

        public SetupForm()
        {
            InitializeComponent();
        }

        private void SetupForm_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (shopIdText.Text.Trim().Equals(""))
            {
                MessageBox.Show("请填写门店ID!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            QueryParam param = QueryParam.create("shopId", shopIdText.Text);
            string ret = HttpClient.post(param.catQueryString(GETTOKEN_URL), null);

            if (ret == null)
                MessageBox.Show("请求出错!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if ("failure".Equals(ret))
                MessageBox.Show("无效账号, 请联系代理商申请安装或咨询QQ客服!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {

                try
                {
                    if (File.Exists(CONFIG_FILE))
                        File.Delete(CONFIG_FILE);

                    File.WriteAllLines(CONFIG_FILE, new string[] { "shopId=" + shopIdText.Text.Trim(), "token=" + ret.Trim() }, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("安装异常, 请联系代理商获取正确的账号或咨询QQ客服!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                MessageBox.Show("恭喜您，游戏大师安装成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }
    }
}
