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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MSGeneralMethods;

namespace MaintenanceSimulatorShuJuJianKong
{
    /// <summary>
    /// Interaction logic for PageQueryByCommandTimeStamp.xaml
    /// </summary>
    public partial class PageQueryByCommandTimeStamp : Page
    {
        public PageQueryByCommandTimeStamp()
        {
            InitializeComponent();
        }

        private void Btn_query_beginQueryByCommandTimeStamp_Click(object sender, RoutedEventArgs e)
        {
            string queryResult = string.Empty;

            try
            {
                DateTime begin = (DateTime)dateTimePicker_query_commandTimeStampBegin.SelectedValue;
                DateTime end = (DateTime)dateTimePicker_query_commandTimeStampEnd.SelectedValue;
                //判断是否起始时间大于结束时间
                TimeSpan delta = end - begin;
                if (delta.TotalSeconds >= 0)
                {
                    //考虑到时间跨度过长会导致搜索时间太长，故在此限制只允许搜索起始日期开始的7天内的数据
                    if (delta.TotalDays <= 7)
                    {
                        //起始及结束时间正常，可以进行查询条件获取操作                
                        queryResult = begin.ToString(@"yyyyMMdd HH:mm:ss;");
                        queryResult += end.ToString(@"yyyyMMdd HH:mm:ss");
                        GlobalDefinitions.UpdateQueryResult(GlobalDefinitions.QueryDatabaseEventArgs.QueryDatabaseCondition.ByCommandTimeStamp, queryResult);                        
                    }
                    else
                    {
                        MessageBox.Show("只允许搜索以起始日期、时间开始的7日范围内的数据！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("结束日期、时间需大于起始日期、时间！\r\n请重新选择", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }                
            }
            catch(Exception exception)
            {
                MessageBox.Show("日期、时间格式错误！\r\n\r\n错误原因：\r\n1.未选择起始日期、时间\r\n2.未选择结束日期、时间\r\n3.日期、时间输入格式有误", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
