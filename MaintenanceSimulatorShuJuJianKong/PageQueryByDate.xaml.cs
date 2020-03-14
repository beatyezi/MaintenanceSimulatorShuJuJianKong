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
    /// Interaction logic for PageQueryByDate.xaml
    /// </summary>
    public partial class PageQueryByDate : Page
    {
        public PageQueryByDate()
        {
            InitializeComponent();
        }

        private void Btn_query_beginQueryByDate_Click(object sender, RoutedEventArgs e)
        {
            string queryResult = String.Empty;

            try
            {
                DateTime begin = (DateTime)dateTimePicker_query_dateBegin.SelectedValue;
                DateTime end = (DateTime)dateTimePicker_query_dateEnd.SelectedValue;
                //判断是否起始时间大于结束时间
                TimeSpan delta = end - begin;
                if (delta.TotalSeconds >= 0)
                {
                    //考虑到时间跨度过长会导致搜索时间太长，故在此限制只允许搜索起始日期开始的7天内的数据
                    if (delta.TotalDays <= 7)
                    {
                        //起始及结束日期正常，可以进行查询条件获取操作                
                        queryResult = begin.ToString(@"yyyyMMdd;");
                        queryResult += end.ToString(@"yyyyMMdd");
                        GlobalDefinitions.UpdateQueryResult(GlobalDefinitions.QueryDatabaseEventArgs.QueryDatabaseCondition.ByDate, queryResult);
                    }
                    else
                    {
                        MessageBox.Show("只允许搜索以起始日期开始的7日范围内的数据！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("结束日期需大于起始日期！\r\n请重新选择", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("日期格式错误！\r\n\r\n错误原因：\r\n1.未选择起始日期\r\n2.未选择结束日期\r\n3.日期输入格式有误", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            //GlobalDefinitions.UpdateQueryResult(GlobalDefinitions.QueryDatabaseEventArgs.QueryDatabaseCondition.ByDate, queryResult);
        }
    }
}
