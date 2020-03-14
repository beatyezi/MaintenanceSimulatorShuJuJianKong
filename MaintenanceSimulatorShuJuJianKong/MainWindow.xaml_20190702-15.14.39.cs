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
using FirstFloor.ModernUI.Windows.Controls;
using System.Windows.Threading;
using MSGeneralMethods;
using System.ComponentModel;
using System.IO;
using System.Collections.ObjectModel;

namespace MaintenanceSimulatorShuJuJianKong
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GlobalDefinitions.AppKeyStatus currentKeyStatus = new GlobalDefinitions.AppKeyStatus();
        public MainWindow()
        {
            InitializeComponent();
            InitDlgCtrls();
            InitTimers();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
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

        private void btnCopyright_Click(object sender, RoutedEventArgs e)
        {
            CopyRight copyRightDlg = new CopyRight("版本及版权信息", GlobalDefinitions.AppInformation + GlobalDefinitions.MSPLCopyRight);
            copyRightDlg.ShowDialog();
            copyRightDlg.Activate();            
        }

        private void InitDlgCtrls()
        {
            iniFilePath = GetIniFilePath();            

            //总是显示最新的消息
            cb_monitor_show_latest_information.IsChecked = true;
            cb_monitor_show_latest_information_Click(this, new RoutedEventArgs());

            //初始化数据库地址
            InitDatabase();

            //初始化ComboBox
            InitComboBox();

            InitDataBinding();
        }

        private List<GlobalDefinitions.Device> devices = new List<GlobalDefinitions.Device>();
        private void SetDeviceMonitoringStatus()
        {
            GlobalDefinitions.Device device;

            
        }

        //private string[] deviceNames = { "导调软件", "联动软件", "桌面监控软件", "训练代理软件", "全站仪", "控显器", "陀螺仪", "激光测距机", "北斗用户机", "打印机", "定位定向系统", "驾驶模拟系统" };
        private void InitComboBox()
        {
            foreach (string s in GlobalDefinitions.deviceNames)
            {
                comboBox_monitor_training_control.Items.Add(s);
            }
            comboBox_monitor_training_control.Text = comboBox_monitor_training_control.Items[0] as string;
        }

        private string databaseFilePath = String.Empty;
        private void InitDatabase()
        {
            //获取数据库文件夹路径
            string databaseFolderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database");           

            //当数据库文件夹不存在时，创建它！
            if (!Directory.Exists(databaseFolderPath))
            {
                try
                {
                    Directory.CreateDirectory(databaseFolderPath);                    
                }
                catch (Exception e)
                {
                    currentKeyStatus.isDatabaseReady = false;
                }
            }

            //按照日期获取数据库文件名称，当数据库文件不存在时，从初始地址拷贝它至数据库文件夹地址
            string databaseName = DateTime.Now.ToLocalTime().ToString(@"yyyyMMdd") + ".mdb";
            databaseFilePath = System.IO.Path.Combine(databaseFolderPath, databaseName);

            try
            {
                if (!File.Exists(databaseFilePath))
                {
                    File.Copy(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mdb.mdb"), databaseFilePath);                    
                }
                AccessHelper.SetDatabasePath(databaseFolderPath, databaseName);
                //AccessHelper.SetDatabasePath(AppDomain.CurrentDomain.BaseDirectory, "mdb.mdb");
                currentKeyStatus.isDatabaseReady = true;

            }
            catch (Exception e)
            {
                currentKeyStatus.isDatabaseReady = false;
            }                       

            if (!currentKeyStatus.isDatabaseReady)
            {
                //程序没有权限创建数据库文件夹，程序将不会自动保存数据
                MessageBox.Show("创建数据库文件夹失败！数据库未更新\r\n没有创建文件夹权限", "失败", MessageBoxButton.OK);
            }                        
        }

        private System.Timers.Timer timerPerSecond = new System.Timers.Timer(1000);
        private delegate void DelegateTimerPerSecond();
        private void InitTimers()
        {
            timerPerSecond.Elapsed += TimerPerSecond_Elapsed;
            timerPerSecond.Enabled = true;
        }

        private void TimerPerSecond_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new DelegateTimerPerSecond(ExecutePerSecond));
        }

        private byte testCount = 0;
        private void ExecutePerSecond()
        {
            string time = DateTime.Now.ToLocalTime().ToString(@"yyyyMMdd HH:mm:ss");//@"yyyy年M月d日tt_hh.mm.ss"
            currentTime.Text = "现在时间: " + time;
            this.Topmost = false;

            //定时打开D:\Programs\0_WPF\MaintenanceSimulator\MaintenanceSimulatorShuJuJianKong\MaintenanceSimulatorShuJuJianKong\Icons\close_hover.png串口（防止串口被关闭）
            if(!currentKeyStatus.isMainSerialPortOpen)
            {
                OpenPort();
            }

            //test
            ProcessPackage(new GlobalDefinitions.Command(time,
                                                                     String.Format("0x{0:X}", MSGeneralMethods.MethodsClass.DeviceAddress.DaoDiao), 
                                                                     GlobalDefinitions.deviceNames[(int)GlobalDefinitions.DeviceIndex.DaoDiao],
                                                                     String.Format("0x{0:X}", MSGeneralMethods.MethodsClass.DeviceAddress.XunLianDaiLi),
                                                                     GlobalDefinitions.deviceNames[(int)GlobalDefinitions.DeviceIndex.XunLianDaiLi],
                                                                     String.Format("0x{0:X}", MSGeneralMethods.MethodsClass.CommandCode.UploadSoftwareStatus),
                                                                     MSGeneralMethods.MethodsClass.TranslateCommandCodeToChinese(MSGeneralMethods.MethodsClass.CommandCode.UploadSoftwareStatus), 
                                                                     String.Format("{0:X2} 1F 2C C4 8A 7D 00 01 1F 2A 3C 0D 0A", testCount)));

            ProcessPackage(new GlobalDefinitions.Command(time,
                                                                     String.Format("0x{0:X}", MSGeneralMethods.MethodsClass.DeviceAddress.BeiDou),
                                                                     GlobalDefinitions.deviceNames[(int)GlobalDefinitions.DeviceIndex.BeiDouYongHuJi],
                                                                     String.Format("0x{0:X}", MSGeneralMethods.MethodsClass.DeviceAddress.LianDong),
                                                                     GlobalDefinitions.deviceNames[(int)GlobalDefinitions.DeviceIndex.LianDong],
                                                                     String.Format("0x{0:X}", MSGeneralMethods.MethodsClass.CommandCode.TrainingPlanAccomplished),
                                                                     MSGeneralMethods.MethodsClass.TranslateCommandCodeToChinese(MSGeneralMethods.MethodsClass.CommandCode.TrainingPlanAccomplished),
                                                                     String.Format("{0:X2} 1F 2C C4 8A 7D 00 01 1F 2A 3C 0D 0A", testCount)));

            if (testCount >= Byte.MaxValue)
            {
                testCount = Byte.MinValue;
            }
            else
            {
                testCount++;
            }
            //end test
        }

        private void LvReceivedPackages_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private delegate void SaveAndShowReceivedPackageDelegate(GlobalDefinitions.Command command);

        private void ProcessPackage(GlobalDefinitions.Command command)
        {
            //this.Dispatcher.BeginInvoke(new SaveAndShowReceivedPackageDelegate(SaveAndShowReceivedPackage), command);
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new SaveAndShowReceivedPackageDelegate(ShowReceivedPackage), command);
            //Dispatcher.BeginInvoke(DispatcherPriority.Background, new SaveAndShowReceivedPackageDelegate(SaveReceivedPackage), command);
        }

        /*private List<GlobalDefinitions.Command> lvDaoDiaoReceivedPackageSource = new List<GlobalDefinitions.Command>();
        private List<GlobalDefinitions.Command> lvLianDongReceivedPackageSource = new List<GlobalDefinitions.Command>();
        private List<GlobalDefinitions.Command> lvZhuoMianJianKongReceivedPackageSource = new List<GlobalDefinitions.Command>();
        private List<GlobalDefinitions.Command> lvXunLianDaiLiReceivedPackageSource = new List<GlobalDefinitions.Command>();
        private List<GlobalDefinitions.Command> lvQuanZhanYiReceivedPackageSource = new List<GlobalDefinitions.Command>();
        private List<GlobalDefinitions.Command> lvKongXianQiReceivedPackageSource = new List<GlobalDefinitions.Command>();
        private List<GlobalDefinitions.Command> lvTuoLuoYiReceivedPackageSource = new List<GlobalDefinitions.Command>();
        private List<GlobalDefinitions.Command> lvJiGuangCeJuJiReceivedPackageSource = new List<GlobalDefinitions.Command>();
        private List<GlobalDefinitions.Command> lvBeiDouYongHuJiReceivedPackageSource = new List<GlobalDefinitions.Command>();
        private List<GlobalDefinitions.Command> lvDaYinJiReceivedPackageSource = new List<GlobalDefinitions.Command>();
        private List<GlobalDefinitions.Command> lvDingWeiDingXiangXiTongReceivedPackageSource = new List<GlobalDefinitions.Command>();
        private List<GlobalDefinitions.Command> lvJiaShiMoNiXiTongReceivedPackageSource = new List<GlobalDefinitions.Command>();

        private void SaveAndShowReceivedPackage(GlobalDefinitions.Command command)
        {                   
            if (!isShowInformationPause)
            {
                switch(command.receivingDeviceAddressInChinese)
                {
                    case "导调软件":
                        {
                            lvDaoDiaoReceivedPackageSource.Add(command);
                            lvDaoDiaoReceivedPackages.ItemsSource = null;
                            lvDaoDiaoReceivedPackages.ItemsSource = lvDaoDiaoReceivedPackageSource;

                            if (isShowLatestInformation)
                            {
                                lvDaoDiaoReceivedPackages.ScrollIntoView(command);
                            }
                        }
                        break;
                    case "联动软件":
                        {
                            lvLianDongReceivedPackageSource.Add(command);
                            lvLianDongReceivedPackages.ItemsSource = null;
                            lvLianDongReceivedPackages.ItemsSource = lvLianDongReceivedPackageSource;

                            if (isShowLatestInformation)
                            {
                                lvLianDongReceivedPackages.ScrollIntoView(command);
                            }
                        }
                        break;
                    case "桌面监控软件":
                        {
                            lvZhuoMianJianKongReceivedPackageSource.Add(command);
                            lvZhuoMianJianKongReceivedPackages.ItemsSource = null;
                            lvZhuoMianJianKongReceivedPackages.ItemsSource = lvZhuoMianJianKongReceivedPackageSource;

                            if (isShowLatestInformation)
                            {
                                lvZhuoMianJianKongReceivedPackages.ScrollIntoView(command);
                            }
                        }
                        break;
                    case "训练代理软件":
                        {
                            lvXunLianDaiLiReceivedPackageSource.Add(command);
                            lvXunLianDaiLiReceivedPackages.ItemsSource = null;
                            lvXunLianDaiLiReceivedPackages.ItemsSource = lvXunLianDaiLiReceivedPackageSource;

                            if (isShowLatestInformation)
                            {
                                lvXunLianDaiLiReceivedPackages.ScrollIntoView(command);
                            }
                        }
                        break;
                    case "全站仪":
                        {
                            lvQuanZhanYiReceivedPackageSource.Add(command);
                            lvQuanZhanYiReceivedPackages.ItemsSource = null;
                            lvQuanZhanYiReceivedPackages.ItemsSource = lvQuanZhanYiReceivedPackageSource;

                            if (isShowLatestInformation)
                            {
                                lvQuanZhanYiReceivedPackages.ScrollIntoView(command);
                            }
                        }
                        break;
                    case "控显器":
                        {
                            lvKongXianQiReceivedPackageSource.Add(command);
                            lvKongXianQiReceivedPackages.ItemsSource = null;
                            lvKongXianQiReceivedPackages.ItemsSource = lvKongXianQiReceivedPackageSource;

                            if (isShowLatestInformation)
                            {
                                lvKongXianQiReceivedPackages.ScrollIntoView(command);
                            }
                        }
                        break;
                    case "陀螺仪":
                        {
                            lvTuoLuoYiReceivedPackageSource.Add(command);
                            lvTuoLuoYiReceivedPackages.ItemsSource = null;
                            lvTuoLuoYiReceivedPackages.ItemsSource = lvTuoLuoYiReceivedPackageSource;

                            if (isShowLatestInformation)
                            {
                                lvTuoLuoYiReceivedPackages.ScrollIntoView(command);
                            }
                        }
                        break;
                    case "激光测距机":
                        {
                            lvJiGuangCeJuJiReceivedPackageSource.Add(command);
                            lvJiGuangCeJuJiReceivedPackages.ItemsSource = null;
                            lvJiGuangCeJuJiReceivedPackages.ItemsSource = lvJiGuangCeJuJiReceivedPackageSource;

                            if (isShowLatestInformation)
                            {
                                lvJiGuangCeJuJiReceivedPackages.ScrollIntoView(command);
                            }
                        }
                        break;
                    case "北斗用户机":
                        {
                            lvBeiDouYongHuJiReceivedPackageSource.Add(command);
                            lvBeiDouYongHuJiReceivedPackages.ItemsSource = null;
                            lvBeiDouYongHuJiReceivedPackages.ItemsSource = lvBeiDouYongHuJiReceivedPackageSource;

                            if (isShowLatestInformation)
                            {
                                lvBeiDouYongHuJiReceivedPackages.ScrollIntoView(command);
                            }
                        }
                        break;
                    case "打印机":
                        {
                            lvDaYinJiReceivedPackageSource.Add(command);
                            lvDaYinJiReceivedPackages.ItemsSource = null;
                            lvDaYinJiReceivedPackages.ItemsSource = lvDaYinJiReceivedPackageSource;

                            if (isShowLatestInformation)
                            {
                                lvDaYinJiReceivedPackages.ScrollIntoView(command);
                            }
                        }
                        break;
                    case "定位定向系统":
                        {
                            lvDingWeiDingXiangXiTongReceivedPackageSource.Add(command);
                            lvDingWeiDingXiangXiTongReceivedPackages.ItemsSource = null;
                            lvDingWeiDingXiangXiTongReceivedPackages.ItemsSource = lvDingWeiDingXiangXiTongReceivedPackageSource;
                            

                            if (isShowLatestInformation)
                            {
                                lvDingWeiDingXiangXiTongReceivedPackages.ScrollIntoView(command);
                            }
                        }
                        break;
                    case "驾驶模拟系统":
                        {
                            lvJiaShiMoNiXiTongReceivedPackageSource.Add(command);
                            lvJiaShiMoNiXiTongReceivedPackages.ItemsSource = null;
                            lvJiaShiMoNiXiTongReceivedPackages.ItemsSource = lvJiaShiMoNiXiTongReceivedPackageSource;

                            if (isShowLatestInformation)
                            {
                                lvJiaShiMoNiXiTongReceivedPackages.ScrollIntoView(command);
                            }
                        }
                        break;
                    default:
                        break;
                }                
            }

            if (currentKeyStatus.isDatabaseReady)
            {
                try
                {
                    AccessHelper.SaveItemToLibrary(command);
                }
                catch (Exception e)
                {

                }
            }
        }*/

        private ObservableCollection<GlobalDefinitions.Command> lvDaoDiaoReceivedPackageSource = new ObservableCollection<GlobalDefinitions.Command>();
        private ObservableCollection<GlobalDefinitions.Command> lvLianDongReceivedPackageSource = new ObservableCollection<GlobalDefinitions.Command>();
        private ObservableCollection<GlobalDefinitions.Command> lvZhuoMianJianKongReceivedPackageSource = new ObservableCollection<GlobalDefinitions.Command>();
        private ObservableCollection<GlobalDefinitions.Command> lvXunLianDaiLiReceivedPackageSource = new ObservableCollection<GlobalDefinitions.Command>();
        private ObservableCollection<GlobalDefinitions.Command> lvQuanZhanYiReceivedPackageSource = new ObservableCollection<GlobalDefinitions.Command>();
        private ObservableCollection<GlobalDefinitions.Command> lvKongXianQiReceivedPackageSource = new ObservableCollection<GlobalDefinitions.Command>();
        private ObservableCollection<GlobalDefinitions.Command> lvTuoLuoYiReceivedPackageSource = new ObservableCollection<GlobalDefinitions.Command>();
        private ObservableCollection<GlobalDefinitions.Command> lvJiGuangCeJuJiReceivedPackageSource = new ObservableCollection<GlobalDefinitions.Command>();
        private ObservableCollection<GlobalDefinitions.Command> lvBeiDouYongHuJiReceivedPackageSource = new ObservableCollection<GlobalDefinitions.Command>();
        private ObservableCollection<GlobalDefinitions.Command> lvDaYinJiReceivedPackageSource = new ObservableCollection<GlobalDefinitions.Command>();
        private ObservableCollection<GlobalDefinitions.Command> lvDingWeiDingXiangXiTongReceivedPackageSource = new ObservableCollection<GlobalDefinitions.Command>();
        private ObservableCollection<GlobalDefinitions.Command> lvJiaShiMoNiXiTongReceivedPackageSource = new ObservableCollection<GlobalDefinitions.Command>();
        private void InitDataBinding()
        {
            lvDaoDiaoReceivedPackages.ItemsSource = lvDaoDiaoReceivedPackageSource;
            lvLianDongReceivedPackages.ItemsSource = lvLianDongReceivedPackageSource;
            lvZhuoMianJianKongReceivedPackages.ItemsSource = lvZhuoMianJianKongReceivedPackageSource;
            lvXunLianDaiLiReceivedPackages.ItemsSource = lvXunLianDaiLiReceivedPackageSource;
            lvQuanZhanYiReceivedPackages.ItemsSource = lvQuanZhanYiReceivedPackageSource;
            lvKongXianQiReceivedPackages.ItemsSource = lvKongXianQiReceivedPackageSource;
            lvTuoLuoYiReceivedPackages.ItemsSource = lvTuoLuoYiReceivedPackageSource;
            lvJiGuangCeJuJiReceivedPackages.ItemsSource = lvJiGuangCeJuJiReceivedPackageSource;
            lvBeiDouYongHuJiReceivedPackages.ItemsSource = lvBeiDouYongHuJiReceivedPackageSource;
            lvDaYinJiReceivedPackages.ItemsSource = lvDaYinJiReceivedPackageSource;
            lvDingWeiDingXiangXiTongReceivedPackages.ItemsSource = lvDingWeiDingXiangXiTongReceivedPackageSource;
            lvJiaShiMoNiXiTongReceivedPackages.ItemsSource = lvJiaShiMoNiXiTongReceivedPackageSource;
        }
        
        private void ShowReceivedPackage(GlobalDefinitions.Command command)
        {
            if (!isShowInformationPause)
            {
                switch (command.receivingDeviceAddressInChinese)
                {
                    case "导调软件":
                        {
                            lvDaoDiaoReceivedPackageSource.Add(command);                            
                            //lvDaoDiaoReceivedPackages.ItemsSource = null;
                            //lvDaoDiaoReceivedPackages.ItemsSource = lvDaoDiaoReceivedPackageSource;

                            if (isShowLatestInformation)
                            {
                                lvDaoDiaoReceivedPackages.ScrollIntoView(command);
                            }
                        }
                        break;
                    case "联动软件":
                        {
                            lvLianDongReceivedPackageSource.Add(command);
                            //lvLianDongReceivedPackages.ItemsSource = null;
                            //lvLianDongReceivedPackages.ItemsSource = lvLianDongReceivedPackageSource;

                            if (isShowLatestInformation)
                            {
                                lvLianDongReceivedPackages.ScrollIntoView(command);
                            }
                        }
                        break;
                    case "桌面监控软件":
                        {
                            lvZhuoMianJianKongReceivedPackageSource.Add(command);
                            //lvZhuoMianJianKongReceivedPackages.ItemsSource = null;
                            //lvZhuoMianJianKongReceivedPackages.ItemsSource = lvZhuoMianJianKongReceivedPackageSource;

                            if (isShowLatestInformation)
                            {
                                lvZhuoMianJianKongReceivedPackages.ScrollIntoView(command);
                            }
                        }
                        break;
                    case "训练代理软件":
                        {
                            lvXunLianDaiLiReceivedPackageSource.Add(command);
                            //lvXunLianDaiLiReceivedPackages.ItemsSource = null;
                            //lvXunLianDaiLiReceivedPackages.ItemsSource = lvXunLianDaiLiReceivedPackageSource;

                            if (isShowLatestInformation)
                            {
                                lvXunLianDaiLiReceivedPackages.ScrollIntoView(command);
                            }
                        }
                        break;
                    case "全站仪":
                        {
                            lvQuanZhanYiReceivedPackageSource.Add(command);
                            //lvQuanZhanYiReceivedPackages.ItemsSource = null;
                            //lvQuanZhanYiReceivedPackages.ItemsSource = lvQuanZhanYiReceivedPackageSource;

                            if (isShowLatestInformation)
                            {
                                lvQuanZhanYiReceivedPackages.ScrollIntoView(command);
                            }
                        }
                        break;
                    case "控显器":
                        {
                            lvKongXianQiReceivedPackageSource.Add(command);
                            //lvKongXianQiReceivedPackages.ItemsSource = null;
                            //lvKongXianQiReceivedPackages.ItemsSource = lvKongXianQiReceivedPackageSource;

                            if (isShowLatestInformation)
                            {
                                lvKongXianQiReceivedPackages.ScrollIntoView(command);
                            }
                        }
                        break;
                    case "陀螺仪":
                        {
                            lvTuoLuoYiReceivedPackageSource.Add(command);
                            //lvTuoLuoYiReceivedPackages.ItemsSource = null;
                            //lvTuoLuoYiReceivedPackages.ItemsSource = lvTuoLuoYiReceivedPackageSource;

                            if (isShowLatestInformation)
                            {
                                lvTuoLuoYiReceivedPackages.ScrollIntoView(command);
                            }
                        }
                        break;
                    case "激光测距机":
                        {
                            lvJiGuangCeJuJiReceivedPackageSource.Add(command);
                            //lvJiGuangCeJuJiReceivedPackages.ItemsSource = null;
                            //lvJiGuangCeJuJiReceivedPackages.ItemsSource = lvJiGuangCeJuJiReceivedPackageSource;

                            if (isShowLatestInformation)
                            {
                                lvJiGuangCeJuJiReceivedPackages.ScrollIntoView(command);
                            }
                        }
                        break;
                    case "北斗用户机":
                        {
                            lvBeiDouYongHuJiReceivedPackageSource.Add(command);
                            //lvBeiDouYongHuJiReceivedPackages.ItemsSource = null;
                            //lvBeiDouYongHuJiReceivedPackages.ItemsSource = lvBeiDouYongHuJiReceivedPackageSource;

                            if (isShowLatestInformation)
                            {
                                lvBeiDouYongHuJiReceivedPackages.ScrollIntoView(command);
                            }
                        }
                        break;
                    case "打印机":
                        {
                            lvDaYinJiReceivedPackageSource.Add(command);
                            //lvDaYinJiReceivedPackages.ItemsSource = null;
                            //lvDaYinJiReceivedPackages.ItemsSource = lvDaYinJiReceivedPackageSource;

                            if (isShowLatestInformation)
                            {
                                lvDaYinJiReceivedPackages.ScrollIntoView(command);
                            }
                        }
                        break;
                    case "定位定向系统":
                        {
                            lvDingWeiDingXiangXiTongReceivedPackageSource.Add(command);
                            //lvDingWeiDingXiangXiTongReceivedPackages.ItemsSource = null;
                            //lvDingWeiDingXiangXiTongReceivedPackages.ItemsSource = lvDingWeiDingXiangXiTongReceivedPackageSource;


                            if (isShowLatestInformation)
                            {
                                lvDingWeiDingXiangXiTongReceivedPackages.ScrollIntoView(command);
                            }
                        }
                        break;
                    case "驾驶模拟系统":
                        {
                            lvJiaShiMoNiXiTongReceivedPackageSource.Add(command);
                            //lvJiaShiMoNiXiTongReceivedPackages.ItemsSource = null;
                            //lvJiaShiMoNiXiTongReceivedPackages.ItemsSource = lvJiaShiMoNiXiTongReceivedPackageSource;

                            if (isShowLatestInformation)
                            {
                                lvJiaShiMoNiXiTongReceivedPackages.ScrollIntoView(command);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            /*if (currentKeyStatus.isDatabaseReady)
            {
                try
                {
                    AccessHelper.SaveItemToLibrary(command);
                }
                catch (Exception e)
                {

                }
            }*/
            /*BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += SaveItemToDatabaseInBackground;
            var sender = new object[1];
            sender[0] = command;
            worker.RunWorkerAsync(sender);*/
        }

        private void SaveReceivedPackage(GlobalDefinitions.Command command)
        {
            if (currentKeyStatus.isDatabaseReady)
            {                
                AccessHelper.SaveItemToLibrary(command);                
            }
        }

        private string iniFilePath = String.Empty;
        private string GetIniFilePath()
        {
            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
        }
        
        private SerialPortClass mainSerialPort = new SerialPortClass();//主串口，对外输入输出
        private void GetPortNameAndBaudrate(string iniFilePath, ref SerialPortClass.SerialPortName mainSerialPortNumber, ref SerialPortClass.SerialBaudrate mainSerialBaudrate)
        {
            string number = IniFileOperations.ReadIniData("PORT", "MainPortNumber", "", iniFilePath);
            if (!String.IsNullOrEmpty(number))
            {
                mainSerialPortNumber = (SerialPortClass.SerialPortName)int.Parse(number);
            }

            string baud = IniFileOperations.ReadIniData("PORT", "MainBaudrate", "", iniFilePath);
            if (!String.IsNullOrEmpty(baud))
            {
                mainSerialBaudrate = (SerialPortClass.SerialBaudrate)int.Parse(baud);
            }
        }

        private void OpenPort()
        {
            SerialPortClass.SerialPortName mainSerialPortName = SerialPortClass.SerialPortName.com1;
            SerialPortClass.SerialBaudrate mainSerialBaudrate = SerialPortClass.SerialBaudrate.baudrate_115200;
            GetPortNameAndBaudrate(iniFilePath, ref mainSerialPortName, ref mainSerialBaudrate);

            currentKeyStatus.isMainSerialPortOpen = mainSerialPort.OpenSerialPort(mainSerialPortName, mainSerialBaudrate, true);
            if(currentKeyStatus.isMainSerialPortOpen)
            {
                BackgroundWorker serialReceiveWorker = new BackgroundWorker();
                serialReceiveWorker.DoWork += MainSerialReceiveWorkerDoWork;
                serialReceiveWorker.RunWorkerAsync(mainSerialPort);
            }
        }

        private void MainSerialReceiveWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            SerialPortClass currentSerial = e.Argument as SerialPortClass;
            while (true)
            {
                if (currentSerial.ReceiveReady)
                {
                    currentSerial.ReceiveReady = false;
                }
            }
        }

        /* private GlobalDefinitions.Command UnpackingCommand(byte[] buffer, int offset, int bufferLength)
         {                        
             for(int loop = offset; loop < bufferLength; loop ++)
             {
                 if(buffer[loop] == MethodsClass.SynchronizeCode55)
                 {

                 }
             }
         }*/

        private void AddAttachmentToContextMenu(string menuItemHeader, ref ContextMenu cm)
        {
            MenuItem mi = new MenuItem()
            {
                Header = menuItemHeader
            };
            mi.Click += btn_monirot_informationFilter_MenuItemClicked;
            cm.Items.Add(mi);
        }

        private void AddAttachmentToContextMenu(string menuItemHeader, bool isChecked, ref ContextMenu cm)
        {
            MenuItem mi = new MenuItem()
            {
                Header = menuItemHeader,
                IsChecked = isChecked,
                IsCheckable = true
            };
            mi.Click += btn_monirot_informationFilter_MenuItemClicked;
            cm.Items.Add(mi);
        }

        private GlobalDefinitions.DeviceInformationFilter deviceInformationFilter;
        private void btn_monirot_informationFilter_MenuItemClicked(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;

            switch ((string)item.Header)
            {
                case "显示 导调软件 数据":
                    {
                        if(item.IsChecked)
                        {
                            deviceInformationFilter.showInformationOfDaoDiaoRuanJian = true;
                        }
                        else
                        {
                            deviceInformationFilter.showInformationOfDaoDiaoRuanJian = false;
                        }
                    }
                    break;
                case "显示 联动软件 数据":
                    {
                        if (item.IsChecked)
                        {
                            deviceInformationFilter.showInformationOfLianDongRuanJian = true;
                        }
                        else
                        {
                            deviceInformationFilter.showInformationOfLianDongRuanJian = false;
                        }
                    }
                    break;
                case "显示 桌面监控软件 数据":
                    {
                        if (item.IsChecked)
                        {
                            deviceInformationFilter.showInformationOfZhuoMianJianKongRuanJian = true;
                        }
                        else
                        {
                            deviceInformationFilter.showInformationOfZhuoMianJianKongRuanJian = false;
                        }
                    }
                    break;
                case "显示 训练代理软件 数据":
                    {
                        if (item.IsChecked)
                        {
                            deviceInformationFilter.showInformationOfXunLianDaiLiRuanJian = true;
                        }
                        else
                        {
                            deviceInformationFilter.showInformationOfXunLianDaiLiRuanJian = false;
                        }
                    }
                    break;
                case "显示 驾驶模拟系统 数据":
                    {
                        if (item.IsChecked)
                        {
                            deviceInformationFilter.showInformationOfJiaShiMoNiXiTong = true;
                        }
                        else
                        {
                            deviceInformationFilter.showInformationOfJiaShiMoNiXiTong = false;
                        }
                    }
                    break;
                case "显示 全站仪 数据":
                    {
                        if (item.IsChecked)
                        {
                            deviceInformationFilter.showInformationOfQuanZhanYi = true;
                        }
                        else
                        {
                            deviceInformationFilter.showInformationOfQuanZhanYi = false;
                        }
                    }
                    break;
                case "显示 控显器 数据":
                    {
                        if (item.IsChecked)
                        {
                            deviceInformationFilter.showInformationOfKongXianQi = true;
                        }
                        else
                        {
                            deviceInformationFilter.showInformationOfKongXianQi = false;
                        }
                    }
                    break;
                case "显示 陀螺仪 数据":
                    {
                        if (item.IsChecked)
                        {
                            deviceInformationFilter.showInformationOfTuoLuoYi = true;
                        }
                        else
                        {
                            deviceInformationFilter.showInformationOfTuoLuoYi = false;
                        }
                    }
                    break;
                case "显示 激光测距机 数据":
                    {
                        if (item.IsChecked)
                        {
                            deviceInformationFilter.showInformationOfJiGuangCeJuJi = true;
                        }
                        else
                        {
                            deviceInformationFilter.showInformationOfJiGuangCeJuJi = false;
                        }
                    }
                    break;
                case "显示 北斗用户机 数据":
                    {
                        if (item.IsChecked)
                        {
                            deviceInformationFilter.showInformationOfBeiDouYongHuJi = true;
                        }
                        else
                        {
                            deviceInformationFilter.showInformationOfBeiDouYongHuJi = false;
                        }
                    }
                    break;
                case "显示 打印机 数据":
                    {
                        if (item.IsChecked)
                        {
                            deviceInformationFilter.showInformationOfDaYinJi = true;
                        }
                        else
                        {
                            deviceInformationFilter.showInformationOfDaYinJi = false;
                        }
                    }
                    break;
                case "显示 定位定向系统 数据":
                    {
                        if (item.IsChecked)
                        {
                            deviceInformationFilter.showInformationOfDingWeiDingXiangXiTong = true;
                        }
                        else
                        {
                            deviceInformationFilter.showInformationOfDingWeiDingXiangXiTong = false;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private ContextMenu cmMonitorInformationFilter = new ContextMenu();
        private bool isMonitorInformationFilterMenuInitialized = false;
        private void btn_monitor_informtionFilter_Click(object sender, RoutedEventArgs e)
        {
            if (!isMonitorInformationFilterMenuInitialized)
            {
                isMonitorInformationFilterMenuInitialized = true;
                cmMonitorInformationFilter = new ContextMenu();

                AddAttachmentToContextMenu("显示 导调软件 数据", true, ref cmMonitorInformationFilter);
                AddAttachmentToContextMenu("显示 联动软件 数据", true, ref cmMonitorInformationFilter);
                AddAttachmentToContextMenu("显示 桌面监控软件 数据", true, ref cmMonitorInformationFilter);
                AddAttachmentToContextMenu("显示 训练代理软件 数据", true, ref cmMonitorInformationFilter);
                AddAttachmentToContextMenu("显示 驾驶模拟系统 数据", true, ref cmMonitorInformationFilter);
                AddAttachmentToContextMenu("显示 全站仪 数据", true, ref cmMonitorInformationFilter);
                AddAttachmentToContextMenu("显示 控显器 数据", true, ref cmMonitorInformationFilter);
                AddAttachmentToContextMenu("显示 陀螺仪 数据", true, ref cmMonitorInformationFilter);
                AddAttachmentToContextMenu("显示 激光测距机 数据", true, ref cmMonitorInformationFilter);
                AddAttachmentToContextMenu("显示 北斗用户机 数据", true, ref cmMonitorInformationFilter);
                AddAttachmentToContextMenu("显示 打印机 数据", true, ref cmMonitorInformationFilter);
                AddAttachmentToContextMenu("显示 定位定向系统 数据", true, ref cmMonitorInformationFilter);

                /*AddAttachmentToContextMenu("菜单项1", ref cmMonitorInformationFilter);
                AddAttachmentToContextMenu("菜单项2", true, ref cmMonitorInformationFilter);
                cmMonitorInformationFilter.Items.Add(new MenuItem { Header = "菜单项3", IsChecked = true, IsEnabled = false });
                cmMonitorInformationFilter.Items.Add(new Separator());
                cmMonitorInformationFilter.Items.Add(CreateSubMenu("菜单项4"));                
                var menu = CreateSubMenu("菜单项5");
                menu.IsEnabled = false;
                cmMonitorInformationFilter.Items.Add(menu);*/

                cmMonitorInformationFilter.IsOpen = true;
            }
            else
            {
                //过滤显示菜单被初始化，直接显示即可
                cmMonitorInformationFilter.IsOpen = true;
            }
        }

        private bool isShowLatestInformation = false;
        private void cb_monitor_show_latest_information_Click(object sender, RoutedEventArgs e)
        {
            isShowLatestInformation = (bool)cb_monitor_show_latest_information.IsChecked;
        }

        private bool isShowInformationPause = false;
        private void cb_monitor_show_information_pause_Click(object sender, RoutedEventArgs e)
        {
            isShowInformationPause = (bool)cb_monitor_show_information_pause.IsChecked;
        }
    }
}
