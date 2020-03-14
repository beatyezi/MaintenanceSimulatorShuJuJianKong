using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MaintenanceSimulatorShuJuJianKong
{
    /// <summary>
    /// Interaction logic for CopyRight.xaml
    /// </summary>
    public partial class CopyRight : Window
    {
        public CopyRight()
        {
            InitializeComponent();
            copyRight_tbTitle.Text = "无标题";
            copyRight_bbcbContent.BBCode = "[b]无内容[/b]";
        }
        
        public CopyRight(string title, string content)
        {
            InitializeComponent();            
            copyRight_tbTitle.Text = title;
            copyRight_bbcbContent.BBCode = content;
        }

        private void copyRight_btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
