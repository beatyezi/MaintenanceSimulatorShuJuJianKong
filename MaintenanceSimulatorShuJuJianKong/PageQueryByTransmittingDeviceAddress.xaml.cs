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
    /// Interaction logic for PageQueryByTransmittingDeviceAddress.xaml
    /// </summary>
    public partial class PageQueryByTransmittingDeviceAddress : Page
    {
        public PageQueryByTransmittingDeviceAddress()
        {
            InitializeComponent();

            InitComboBox();
        }

        private void InitComboBox()
        {
            //初始化数据查询界面的ComboBox
            foreach (string s in GlobalDefinitions.deviceNames)
            {
                comboBox_query_transmitting_device_address.Items.Add(s);
            }
            comboBox_query_transmitting_device_address.Text = comboBox_query_transmitting_device_address.Items[0] as string;            
        }

        private void Btn_query_beginQueryByTransmittingDeviceAddress_Click(object sender, RoutedEventArgs e)
        {
            string queryResult = comboBox_query_transmitting_device_address.Text;

            GlobalDefinitions.UpdateQueryResult(GlobalDefinitions.QueryDatabaseEventArgs.QueryDatabaseCondition.ByTransmittingDeviceAddress, queryResult);
        }
    }
}
