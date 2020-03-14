using FirstFloor.ModernUI.Windows.Controls;
using MSGeneralMethods;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using WinForm = System.Windows.Forms;


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

        #region 主界面相关配置（最小化、关闭、移动等）        
        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (currentKeyStatus.appCanMove)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            }
        }
        #endregion

        #region 程序初始化
        private void InitDlgCtrls()
        {
            //配置关键状态中的程序可移动状态
            currentKeyStatus.appCanMove = false;

            //获取*.ini配置文件地址
            gConfigIniFilePath = GetIniFilePath();

            //总是显示最新的消息
            cb_monitor_show_latest_information.IsChecked = true;
            cb_monitor_show_latest_information_Click(this, new RoutedEventArgs());

            //初始化数据库地址
            InitDatabase();

            //初始化ComboBox
            InitComboBox();

            //初始化ListView和数据源之间的数据绑定
            InitDataBinding();

            //设置各软件的监控状态及过滤显示状态
            SetDeviceMonitoringStatus();

            //初始化数据库保存计数器
            gSaveReceivedPackageCounter = 0;

            //配置后台工人
            InitBackgroundWorker();

            //初始化数据分析中的各类汇总信息
            InitAnalyseInformation();

            //初始化数据查询界面中的查询选项
            Radio_query_by_transmiting_device_address_Click(this, new RoutedEventArgs());

            //初始化数据查询界面的查询方式事件响应
            GlobalDefinitions.QueryResult += new GlobalDefinitions.QueryDatabaseHandler(ExecuteQueryResult);

            //初始化数据查询界面的数据文件显示
            ShowDatabaseFileName(gDatabaseFolderPath);

            //初始化开机启动选项
            InitPowerboot();

            #region 测试函数
            //正式版需屏蔽以下测试函数
            /*
            byte[] targetBuffer = new byte[128];// { 0x55, 0xaa, 0x02, 0x00, 0x16, 0x2c, 0x01, 0x2a, 0x92, 0x0d, 0x0a };
            int targetBufferLength = 0;
            MethodsClass.DeviceAddress targetAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize, sourceAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize;
            MethodsClass.CommandCode command = MethodsClass.CommandCode.CommandCodeForInitialize;
            //MethodsClass.DesktopMonitorControlCategory category = MethodsClass.DesktopMonitorControlCategory.DesktopMonitorControlCategoryForInitialize;
            //PackingDesktopMonitorControl(MethodsClass.DeviceAddress.ZhuoMianJianKong, MethodsClass.DeviceAddress.DaoDiao, MethodsClass.CommandCode.DesktopMonitorControl,
            //MethodsClass.DesktopMonitorControlCategory.BeginMonitoringDesktop, ref targetBuffer, ref targetBufferLength);
            //UnpackingDesktopMonitorControl(targetBuffer, 0, targetBufferLength, ref sourceAddress, ref targetAddress, ref command, ref category);

            //上传软件状态报文测试
            //MethodsClass.PackingUploadSoftwareStatus(MethodsClass.DeviceAddress.ZhuoMianJianKong, MethodsClass.DeviceAddress.DaoDiao, MethodsClass.CommandCode.UploadSoftwareStatus,
            //                                        MethodsClass.SoftwareStatus.Error, "无法获取全站仪图像信息", "V1.00.00/2019年1月2日", ref targetBuffer, ref targetBufferLength);
            //MethodsClass.SoftwareStatus softwareStatus = MethodsClass.SoftwareStatus.SoftwareStatusForInitialize;
            //string information = String.Empty, version = String.Empty;
            //MethodsClass.ErrorCode errorCode = MethodsClass.UnpackingUploadSoftwareStatus(targetBuffer, 0, targetBufferLength, ref sourceAddress, ref targetAddress, ref command, ref softwareStatus,
            //                                            ref information, ref version);

            //完成训练科目报文测试
            //MethodsClass.PackingTrainingPlanAccomplished(MethodsClass.DeviceAddress.JiGuangCeJuJi, MethodsClass.DeviceAddress.LianDong, MethodsClass.CommandCode.TrainingPlanAccomplished,
            //                                            MethodsClass.TrainingPlan.LianCeJiaoZhengDian, 0x16, MethodsClass.TrainingPlanAccomplishedFlag.TrainingPlanStep2Accomplished,
            //                                            "正在进行测距", ref targetBuffer, ref targetBufferLength);
            //MethodsClass.TrainingPlan completedTrainingPlan = MethodsClass.TrainingPlan.TrainingPlanForInitialize;
            //byte totalSteps = 0x00;
            //MethodsClass.TrainingPlanAccomplishedFlag accomplishedFlag = MethodsClass.TrainingPlanAccomplishedFlag.TrainingPlanAccomplishedFlagForInitialize;
            //string content = String.Empty;
            //MethodsClass.ErrorCode error = MethodsClass.UnpackingTrainingPlanAccomplished(targetBuffer, 0, targetBufferLength, ref sourceAddress, ref targetAddress, ref command,
            //                                                                            ref completedTrainingPlan, ref totalSteps, ref accomplishedFlag, ref content);

            //同步时间信息报文测试
            MethodsClass.PackingSynchronizeTimeInformation(MethodsClass.DeviceAddress.DaoDiao, MethodsClass.DeviceAddress.LianDong, MethodsClass.CommandCode.SynchronizeTimeInformation,
                                                            "2019", "03", "12", "09", "56", "48", ref targetBuffer, ref targetBufferLength);
            string year = String.Empty, month = String.Empty, day = String.Empty, hour = String.Empty, minute = String.Empty, second = String.Empty;
            MethodsClass.ErrorCode error = MethodsClass.UnpackingSynchronizeTimeInformation(targetBuffer, 0, targetBufferLength,
                                                                                            ref sourceAddress, ref targetAddress, ref command,
                                                                                            ref year, ref month, ref day, ref hour, ref minute, ref second);     
            int flag = 0;
            MethodsClass.TrainingPlanAccomplishedFlag accomplishedFlag = MethodsClass.TrainingPlanAccomplishedFlag.TrainingPlanAccomplishedFlagForInitialize;
            accomplishedFlag = MethodsClass.TrainingPlanAccomplishedFlag.TrainingPlanStep11Accomplished;
            flag = (int)accomplishedFlag;
            string test = accomplishedFlag.ToString();
            */
            //end test            
            #endregion
        }

        #region 测试函数
        //正式版需屏蔽以下测试函数
        /*
        private void SetBuffer(byte sourceByte, ref byte[] targetBuffer, ref int targetBufferPointer)
        {
            targetBuffer[targetBufferPointer++] = sourceByte;
        }

        public MethodsClass.ErrorCode PackingDesktopMonitorControl(MethodsClass.DeviceAddress transmittingDeviceAddress,
                                                             MethodsClass.DeviceAddress receivingDeviceAddress,
                                                             MethodsClass.CommandCode commandCode,
                                                             MethodsClass.DesktopMonitorControlCategory desktopMonitorControlCategory,
                                                             ref byte[] targetBuffer,
                                                             ref int targetBufferLength)
        {
            int pointer = 0;

            //打包帧头和帧头分隔符
            SetBuffer(MethodsClass.SynchronizeCode55, ref targetBuffer, ref pointer);
            SetBuffer(MethodsClass.SynchronizeCodeAA, ref targetBuffer, ref pointer);
            SetBuffer((byte)transmittingDeviceAddress, ref targetBuffer, ref pointer);
            SetBuffer((byte)receivingDeviceAddress, ref targetBuffer, ref pointer);
            SetBuffer((byte)commandCode, ref targetBuffer, ref pointer);
            SetBuffer(MethodsClass.FieldSeparator, ref targetBuffer, ref pointer);

            //打包桌面监视类别和被监视的设备号
            SetBuffer((byte)desktopMonitorControlCategory, ref targetBuffer, ref pointer);

            //打包结束符和帧尾
            SetBuffer(MethodsClass.CheckCodeSeparator, ref targetBuffer, ref pointer);
            SetBuffer(MethodsClass.GetCrcCode(targetBuffer, 0, pointer), ref targetBuffer, ref pointer);
            SetBuffer(MethodsClass.TerminationCode0D, ref targetBuffer, ref pointer);
            SetBuffer(MethodsClass.TerminationCode0A, ref targetBuffer, ref pointer);

            targetBufferLength = pointer;

            return MethodsClass.ErrorCode.NoError;
        }

        public MethodsClass.ErrorCode UnpackingDesktopMonitorControl(byte[] sourceBuffer,
                                                               int offset,
                                                               int sourceBufferLength,
                                                               ref MethodsClass.DeviceAddress transmittingDeviceAddress,
                                                               ref MethodsClass.DeviceAddress receivingDeviceAddress,
                                                               ref MethodsClass.CommandCode commandCode,
                                                               ref MethodsClass.DesktopMonitorControlCategory desktopMonitorControlCategory)
        {
            int pointer = offset;

            if (sourceBuffer[pointer++] == MethodsClass.SynchronizeCode55 && sourceBuffer[pointer++] == MethodsClass.SynchronizeCodeAA)
            {
                transmittingDeviceAddress = (MethodsClass.DeviceAddress)sourceBuffer[pointer++];
                receivingDeviceAddress = (MethodsClass.DeviceAddress)sourceBuffer[pointer++];
                commandCode = (MethodsClass.CommandCode)sourceBuffer[pointer++];
                pointer++;//分隔符

                desktopMonitorControlCategory = (MethodsClass.DesktopMonitorControlCategory)sourceBuffer[pointer++];
                pointer++;//分隔符                

                if (sourceBuffer[pointer++] == MethodsClass.GetCrcCode(sourceBuffer, offset, sourceBufferLength - 3))//减去缓冲区中的CRC校验符,结束符0x0D,0x0A
                {
                    return MethodsClass.ErrorCode.NoError;
                }
                else
                {
                    return MethodsClass.ErrorCode.ErrorWrongCheckCode;
                }
            }
            else
            {
                return MethodsClass.ErrorCode.ErrorNoSynchCodeMatched;
            }
        }
        */
        #endregion

        private string gConfigIniFilePath = String.Empty;

        /// <summary>
        /// 获取config.ini文件全路径
        /// </summary>
        /// <returns>config.ini文件在系统中的全路径</returns>
        private string GetIniFilePath()
        {
            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
        }

        private string gDatabaseFilePath = String.Empty, gDatabaseFolderPath = String.Empty;
        /// <summary>
        /// 配置数据库，如获取数据库文件夹全路径、创建当前数据库文件等
        /// </summary>
        /// <returns>null</returns>
        private void InitDatabase()
        {
            //获取数据库文件夹路径
            gDatabaseFolderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database");

            //当数据库文件夹不存在时，创建它！
            if (!Directory.Exists(gDatabaseFolderPath))
            {
                try
                {
                    Directory.CreateDirectory(gDatabaseFolderPath);
                }
                catch (Exception e)
                {
                    currentKeyStatus.isDatabaseReady = false;
                }
            }

            //按照日期获取数据库文件名称，当数据库文件不存在时，从初始地址拷贝它至数据库文件夹地址
            string databaseName = DateTime.Now.ToLocalTime().ToString(@"yyyyMMdd") + ".mdb";
            gDatabaseFilePath = System.IO.Path.Combine(gDatabaseFolderPath, databaseName);

            try
            {
                if (!File.Exists(gDatabaseFilePath))
                {
                    File.Copy(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mdb.mdb"), gDatabaseFilePath);
                }
                AccessHelper.SetDatabasePath(gDatabaseFolderPath, databaseName);
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

        /// <summary>
        /// 配置界面中ComboBox下拉显示
        /// </summary>
        /// <returns>null</returns>
        private void InitComboBox()
        {
            foreach (string s in GlobalDefinitions.deviceNames)
            {
                if (!s.Equals("导调软件") && !s.Equals("联动软件") && !s.Equals("桌面监控软件") && !s.Equals("数据监控软件") && !s.Equals("训练代理软件"))
                {
                    comboBox_monitor_training_control.Items.Add(s);
                }
            }
            comboBox_monitor_training_control.Text = comboBox_monitor_training_control.Items[0] as string;
        }

        /// <summary>
        /// 配置后台工人，如设置数据库保存后台工人等
        /// </summary>
        private void InitBackgroundWorker()
        {
            BackgroundWorker databaseSaveWorker = new BackgroundWorker();
            databaseSaveWorker.DoWork += DatabaseSaveWorker_DoWork;
            databaseSaveWorker.RunWorkerAsync();
        }        

        /// <summary>
        /// 从注册表中获取开机启动信息并配置开机启动界面
        /// </summary>
        private void InitPowerboot()
        {
            //初始化注册表中的开机启动项
            int value = Convert.ToInt16(IniFileOperations.ReadIniData("STATUS", "PowerbootEnable", "", gConfigIniFilePath));
            currentKeyStatus.isPowerbootEnabled = (value == 1 ? true : false);
            SetPowerbootStatus(currentKeyStatus.isPowerbootEnabled);

            //初始化开机启动界面选项
            cb_other_powerbootEnabled.IsChecked = currentKeyStatus.isPowerbootEnabled;
            
        }

        private System.Timers.Timer timerPerSecond = new System.Timers.Timer(1000);
        private delegate void DelegateTimerPerSecond();

        /// <summary>
        /// 配置定时器
        /// </summary>
        private void InitTimers()
        {
            timerPerSecond.Elapsed += TimerPerSecond_Elapsed;
            timerPerSecond.Enabled = true;
        }

        /// <summary>
        /// 定时器Elapsed事件响应函数
        /// </summary>
        /// <param name="sender">Elapsed事件调用方</param>
        /// <param name="e">ElapsedEventArgs返回值</param>
        private void TimerPerSecond_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new DelegateTimerPerSecond(ExecutePerSecond));
        }


        #region 定时器测试函数
        //private byte testCount = 0;
        #endregion

        private string gCurrentTime = string.Empty;

        /// <summary>
        /// 定时器Elapsed事件执行函数，每秒调用一次
        /// </summary>
        private void ExecutePerSecond()
        {
            gCurrentTime = DateTime.Now.ToLocalTime().ToString(@"yyyyMMdd HH:mm:ss");//@"yyyy年M月d日tt_hh.mm.ss"
            string timeToShow = DateTime.Now.ToLocalTime().ToString(@"yyyy年MM月dd日 HH:mm:ss");
            tb_main_currentTime.Text = "现在时间: " + timeToShow;

            #region 在程序调试时不进行置顶，在正式版软件中屏蔽如下语句
            //this.Topmost = false;
            #endregion

            //定时打开串口（防止串口被关闭）
            if (!currentKeyStatus.isMainSerialPortOpen)
            {
                OpenPort();
            }

            //定时刷新数据分析信息
            ShowCollectionOfBaseInformation();
            ShowCollectionOfSerialData();
            ShowCollectionOfUserInformation();

            //每秒刷新数据分析中的图表

            //到达条件时清除数据分析界面中的图表，20190730决定还是放在图表绘制时清除
            /*if(analyzedReceivedPackageCount >= 500)
            {
                chart_memoryAndCpuPerSecond.DataPoints.Clear();
                chart_serialDataTransmittedQuantityPerSecond.DataPoints.Clear();
            }*/

            //自动获取数据库文件夹的文件数量并与原数量作比较
            if (gDatabaseFiles.Count != GetFileNumberInCurrentDatabaseFolder(gDatabaseFolderPath))
            {
                ShowDatabaseFileName(gDatabaseFolderPath);
            }

            #region 定时器测试函数
            /*
            ProcessPackage(new GlobalDefinitions.Command(currentTime,
                                                        String.Format("0x{0:X}", MethodsClass.DeviceAddress.DaoDiao), 
                                                        GlobalDefinitions.deviceNames[(int)GlobalDefinitions.DeviceIndex.DaoDiao],
                                                        String.Format("0x{0:X}", MethodsClass.DeviceAddress.XunLianDaiLi),
                                                        GlobalDefinitions.deviceNames[(int)GlobalDefinitions.DeviceIndex.XunLianDaiLi],
                                                        String.Format("0x{0:X}", MethodsClass.CommandCode.UploadSoftwareStatus),
                                                        MethodsClass.TranslateCommandCodeToChinese(MethodsClass.CommandCode.UploadSoftwareStatus), 
                                                        String.Format("{0:X2} 1F 2C C4 8A 7D 00 01 1F 2A 3C 0D 0A", testCount)));

            ProcessPackage(new GlobalDefinitions.Command(currentTime,                                                            
                                                        String.Format("0x{0:X}", MethodsClass.DeviceAddress.BeiDou),
                                                        GlobalDefinitions.deviceNames[(int)GlobalDefinitions.DeviceIndex.BeiDouYongHuJi],
                                                        String.Format("0x{0:X}", MethodsClass.DeviceAddress.LianDong),
                                                        GlobalDefinitions.deviceNames[(int)GlobalDefinitions.DeviceIndex.LianDong],
                                                        String.Format("0x{0:X}", MethodsClass.CommandCode.TrainingPlanAccomplished),
                                                        MethodsClass.TranslateCommandCodeToChinese(MethodsClass.CommandCode.TrainingPlanAccomplished),
                                                        String.Format("{0:X2} 1F 2C C4 8A 7D 00 01 1F 2A 3C 0D 0A", testCount)));
            
            if (testCount >= Byte.MaxValue)
            {
                testCount = Byte.MinValue;
            }
            else
            {
                testCount++;
            }
            */
            #endregion
        }

        #endregion

        #region 主程序运行时的相关逻辑
        /// <summary>
        /// 执行数据库保存操作的后台工人DoWork响应函数
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">DoWorkEventArgs</param>
        private void DatabaseSaveWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (gSaveReceivedPackageCounter >= 10)
                {
                    SaveReceivedPackage(gSaveReceivedPackageBuffer);
                    gSaveReceivedPackageCounter = 0;
                    gSaveReceivedPackageBuffer.Clear();
                }
            }
        }

        /// <summary>
        /// 保存接收到的单个Command类型的数据包
        /// </summary>
        /// <param name="command">Command类型变量</param>
        private void SaveReceivedPackage(GlobalDefinitions.Command command)
        {
            if (currentKeyStatus.isDatabaseReady)
            {
                //AccessHelper.SaveItemToLibrary(command);   
                string result = AccessHelper.SaveItemToLibrary(command);
                MessageBox.Show(result);
            }
        }

        /// <summary>
        /// 将存于List表中的Command类型的数据包保存至数据库
        /// </summary>
        /// <param name="buffer">存储Command类型的数据包的List表</param>
        private void SaveReceivedPackage(List<GlobalDefinitions.Command> buffer)
        {
            if (currentKeyStatus.isDatabaseReady)
            {
                foreach (GlobalDefinitions.Command item in buffer)
                {
                    //AccessHelper.SaveItemToLibrary(item);
                    string result = AccessHelper.SaveItemToLibrary(item);
                    MessageBox.Show(result);
                }
            }
        }

        private delegate void ShowReceivedPackageDelegate(GlobalDefinitions.Command command);
        private delegate void SaveReceivedPackageDelegate(List<GlobalDefinitions.Command> buffer);
        private delegate void AnalyzingReceivedPackageAndShowDelegate(GlobalDefinitions.Command command);
        private int gSaveReceivedPackageCounter = 0;
        private List<GlobalDefinitions.Command> gSaveReceivedPackageBuffer = new List<GlobalDefinitions.Command>();

        /// <summary>
        /// 处理接收到的Command数据包，如在数据分析界面显示、在数据监控界面显示等
        /// </summary>
        /// <param name="command">Command类型变量</param>
        private void ProcessPackage(GlobalDefinitions.Command command)
        {
            //在数据分析界面显示接收到的数据
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new AnalyzingReceivedPackageAndShowDelegate(AnalyzingReceivedPackageAndShow), command);

            //在数据监控界面显示接收到的数据
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new ShowReceivedPackageDelegate(ShowReceivedPackage), command);

            //数据库缓冲区存入接收到的数据
            if (++gSaveReceivedPackageCounter < 10)
            {
                gSaveReceivedPackageBuffer.Add(command);
            }
        }

        private SerialPortClass mainSerialPort = new SerialPortClass();//主串口，对外输入输出        

        /// <summary>
        /// 从config.ini文件中读取串口设置，如波特率、默认串口号等
        /// </summary>
        /// <param name="iniFilePath">config.ini文件全路径地址</param>
        /// <param name="mainSerialPortNumber">SerialPortName引用类型变量，用于获取串口号</param>
        /// <param name="mainSerialBaudrate">SerialBaudrate引用类型变量，用于获取串口波特率</param>
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

        /// <summary>
        /// 打开串口
        /// </summary>
        private void OpenPort()
        {
            SerialPortClass.SerialPortName mainSerialPortName = SerialPortClass.SerialPortName.com1;
            SerialPortClass.SerialBaudrate mainSerialBaudrate = SerialPortClass.SerialBaudrate.baudrate_115200;
            GetPortNameAndBaudrate(gConfigIniFilePath, ref mainSerialPortName, ref mainSerialBaudrate);

            currentKeyStatus.isMainSerialPortOpen = mainSerialPort.OpenSerialPort(mainSerialPortName, mainSerialBaudrate, true);
            if (currentKeyStatus.isMainSerialPortOpen)
            {
                //保存本次成功开启的串口号及波特率
                
                
                //串口接收后台工人开始工作
                BackgroundWorker serialReceiveWorker = new BackgroundWorker();
                serialReceiveWorker.DoWork += MainSerialReceiveWorkerDoWork;
                serialReceiveWorker.RunWorkerAsync(mainSerialPort);
            }
        }

        private SerialPortClass.SerialPortName gLastSuccessfulOpeningSerialPortName = SerialPortClass.SerialPortName.com1;
        private SerialPortClass.SerialBaudrate gLastSuccessfulOpeningSerialBaudrate = SerialPortClass.SerialBaudrate.baudrate_115200;
        
        private void ClosePort()
        {

        }

        public delegate void ShowDatabaseFileNameDelegate(GlobalDefinitions.DatabaseFile result);
        private List<GlobalDefinitions.DatabaseFile> gDatabaseFiles = new List<GlobalDefinitions.DatabaseFile>();
        ///   <summary>   
        ///   显示数据库文件夹内的数据文件
        ///   </summary>   
        ///   <param   name="folderPath">数据库文件夹全路径</param>
        ///   <returns>真：数据库文件夹内有数据文件；假：数据库文件夹内无数据文件</returns>  
        private bool ShowDatabaseFileName(string folderPath)
        {
            gDatabaseFiles.Clear();
            lb_query_databaseFileName.Items.Clear();
            DirectoryInfo di = new DirectoryInfo(folderPath);
            foreach (FileInfo fi in di.GetFiles())
            {
                if (fi.Extension.ToLower().Contains("mdb"))
                {
                    GlobalDefinitions.DatabaseFile item = new GlobalDefinitions.DatabaseFile();
                    item.name = fi.Name;
                    item.path = fi.FullName;
                    item.information = String.Format("创建时间：{0}\r\n修改时间：{1}\r\n访问时间：{2}", fi.CreationTime, fi.LastWriteTime, fi.LastAccessTime);
                    gDatabaseFiles.Add(item);
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ShowDatabaseFileNameDelegate(UpdateDatabaseFileName), item);
                }
            }

            return gDatabaseFiles.Count == 0 ? false : true;
        }

        /// <summary>
        /// 更新数据库文件名并更新数据查询界面的“数据库文件”ListBox数据源
        /// </summary>
        /// <param name="result">数据库文件</param>
        private void UpdateDatabaseFileName(GlobalDefinitions.DatabaseFile result)
        {
            BBCodeBlock item = new BBCodeBlock();
            item.BBCode = result.name;
            item.Tag = result.path;
            item.ToolTip = result.information;//result.path + "\r\n" + result.information;
            lb_query_databaseFileName.Items.Add(item);
            lb_query_databaseFileName.ScrollIntoView(item);
        }

        /// <summary>
        /// 按需切换程序开机启动
        /// </summary>
        /// <param name="isPowerbootEnable">是否需要开机启动。=true，则向注册表写入开机自启动信息；=false，则删除注册表中的相关开机自启动信息</param>
        /// <returns>null</returns>
        private void SetPowerbootStatus(bool isPowerbootEnable)
        {
            if (isPowerbootEnable)
            {
                if (String.IsNullOrEmpty(RegistryOperations.ReadRegistry("Microsoft\\Windows\\CurrentVersion\\Run", "MSShuJuJianKong")))
                {
                    string exeName = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                    RegistryOperations.WriteRegistry("Microsoft\\Windows\\CurrentVersion\\Run", "MSShuJuJianKong", exeName);
                }
            }
            else
            {
                RegistryOperations.WriteRegistry("Microsoft\\Windows\\CurrentVersion\\Run", "MSShuJuJianKong", String.Empty);
            }
        }

        /// <summary>
        /// 获取数据库文件内的符合数据库文件命名要求的文件数量
        /// </summary>
        /// <param name="databaseFolderPath">数据库文件夹全路径</param>
        /// <returns>文件夹内的有效数据库文件数量</returns>
        private int GetFileNumberInCurrentDatabaseFolder(string databaseFolderPath)
        {
            int result = 0;
            DirectoryInfo di = new DirectoryInfo(databaseFolderPath);
            foreach(FileInfo fi in di.GetFiles())
            {
                if (fi.Extension.ToLower().Contains("mdb"))
                {
                    result++;
                }
            }
            return result;
            //FileInfo[] fi = di.GetFiles();
            //return (fi == null) ? 0 : fi.Length;
        }

        private string ScrollTextFromRightToLeft(string value)
        {
            return value.Substring(1) + value.Substring(0, 1);
        }

        #endregion//主程序运行时的相关逻辑

        #region 串口相关逻辑，如获取串口数据、串口缓冲区前移等

        /// <summary>
        /// 向缓冲区中添加从串口中读取到的字节数据
        /// </summary>
        /// <param name="targetArray">目标缓冲区</param>
        /// <param name="sourceArray">待添加的源缓冲区</param>
        /// <param name="targetArrayNumber">目标缓冲区长度</param>
        /// <param name="sourceArrayNumber">源缓冲区长度</param>
        /// <returns>缓冲区有效数据长度</returns>
        private static int StringAppendWithLimit(byte[] targetArray, byte[] sourceArray, int targetArrayNumber, int sourceArrayNumber)
        {
            for (int iLoop = 0; iLoop < sourceArrayNumber; iLoop++)
            {
                targetArray[targetArrayNumber + iLoop] = sourceArray[iLoop];
            }

            return (int)targetArrayNumber + sourceArrayNumber;
        }

        /// <summary>
        /// 在处理完数据包后获取缓冲区中的剩余有效数据长度
        /// </summary>
        /// <param name="iValidStartPosition">缓冲区中有效数据起始索引号</param>
        /// <param name="iUsedDataLength">已处理的数据帧长度</param>
        /// <param name="iTotalDataLength">缓冲区有效数据总长度</param>
        /// <returns>缓冲区剩余有效数据长度</returns>
        private static int GetRemainDataLength(int iValidStartPosition, int iUsedDataLength, int iTotalDataLength)
        {
            if (iTotalDataLength > iUsedDataLength)
            {
                return (iTotalDataLength - iUsedDataLength - iValidStartPosition);
            }
            else
            {
                return (0);
            }
        }

        /// <summary>
        /// 在处理完数据包后将缓冲区内容前移
        /// </summary>
        /// <param name="UsedDataPos">已处理完的数据包最后字节在缓冲区中的索引号</param>
        /// <param name="RemainLength">缓冲区有效数据长度</param>
        /// <param name="targetBuffer">目标缓冲区</param>
        /// <returns>缓冲区剩余有效数据长度</returns>
        private static int MoveForwardQueue(int UsedDataPos, int RemainLength, byte[] targetBuffer)
        {
            int ReturnLength = 0, iLoop = 0;
            if (RemainLength > 0)
            {
                for (iLoop = 0; iLoop < RemainLength; iLoop++)
                {
                    targetBuffer[iLoop] = targetBuffer[UsedDataPos + iLoop];
                }
                ReturnLength = iLoop;

                return (ReturnLength);
            }
            else
            {
                return (ReturnLength);
            }
        }

        private const short DATA_QUEUE_MAX = 1024;
        private byte[] gDataQueue = new byte[DATA_QUEUE_MAX];
        private int gDataQueuePointer = 0;
        //private byte[] gTempBufferForReceiving = new byte[1];
        private int gLoopForDataValidation = 0, gStartPosition = 0, gValidDataPosition = 0, gRemainDataLength = 0;
        private byte[] gValidBuffer = new byte[DATA_QUEUE_MAX];
        private bool gIsDataProcessing = false;

        /// <summary>
        /// 串口接收后台工人DoWork事件响应函数
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">DoWork事件参数</param>
        private void MainSerialReceiveWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            SerialPortClass currentSerial = e.Argument as SerialPortClass;
            while (true)
            {
                if (currentSerial.ReceiveReady)
                {
                    currentSerial.ReceiveReady = false;

                    if (gDataQueuePointer + 1 > DATA_QUEUE_MAX)
                    {
                        gDataQueuePointer = 0;
                        continue;
                    }

                    gDataQueuePointer = StringAppendWithLimit(gDataQueue, currentSerial.ReceivedData, gDataQueuePointer, currentSerial.ReceivedData.Length);

                    gIsDataProcessing = true;

                    LOOP_BEGIN: for (gLoopForDataValidation = 0; gLoopForDataValidation < gDataQueuePointer; gLoopForDataValidation++)
                    {
                        if (gDataQueue[gLoopForDataValidation] == MethodsClass.SynchronizeCode55 && gDataQueue[gLoopForDataValidation + 1] == MethodsClass.SynchronizeCodeAA)
                        {
                            gStartPosition = 0;
                            gValidDataPosition = gLoopForDataValidation;
                        }
                        gValidBuffer[gStartPosition++] = gDataQueue[gLoopForDataValidation];
                        if (gValidBuffer[gStartPosition - 1] == MethodsClass.TerminationCode0A && gValidBuffer[gStartPosition - 2] == MethodsClass.TerminationCode0D)
                        {
                            switch (gValidBuffer[4])
                            {
                                //同步时间信息(0x10)
                                case (byte)MethodsClass.CommandCode.SynchronizeTimeInformation:
                                    {
                                        MethodsClass.DeviceAddress transmittingDeviceAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize,
                                                                   receivingDeviceAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize;
                                        MethodsClass.CommandCode commandCode = MethodsClass.CommandCode.CommandCodeForInitialize;
                                        string year = String.Empty, month = String.Empty, day = String.Empty, hour = String.Empty, minute = String.Empty, second = String.Empty;
                                        MethodsClass.ErrorCode unpackingResult = MethodsClass.UnpackingSynchronizeTimeInformation(gValidBuffer,
                                                                                                                                  0,
                                                                                                                                  gStartPosition,
                                                                                                                                  ref transmittingDeviceAddress,
                                                                                                                                  ref receivingDeviceAddress,
                                                                                                                                  ref commandCode,
                                                                                                                                  ref year, ref month, ref day,
                                                                                                                                  ref hour, ref minute, ref second);
                                        string content = String.Format("同步时间至：{0}年{1}月{2}日{3}时{4}分{5}秒", year, month, day, hour, minute, second);
                                        GlobalDefinitions.Command command = new GlobalDefinitions.Command(gCurrentTime,
                                                                                                          String.Format("0x{0:X}", transmittingDeviceAddress),
                                                                                                          GlobalDefinitions.deviceNames[(int)transmittingDeviceAddress],
                                                                                                          String.Format("0x{0:X}", receivingDeviceAddress),
                                                                                                          GlobalDefinitions.deviceNames[(int)receivingDeviceAddress],
                                                                                                          String.Format("0x{0:X}", commandCode),
                                                                                                          MethodsClass.TranslateCommandCodeToChinese(commandCode),
                                                                                                          content);
                                        ProcessPackage(command);

                                        //同步本机时间
                                        MessageBoxResult userResult = MessageBox.Show("接收到外部发来同步时间报文，是否需要同步本机时间？\r\n点击“是”将同步本机时间；点击“否”取消", "提示", MessageBoxButton.YesNo, MessageBoxImage.Information);
                                        if (userResult == MessageBoxResult.Yes)
                                        {
                                            GlobalDefinitions.DateTimeHelper.SetLocalDateTime(new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(day),
                                                                                                           Convert.ToInt32(hour), Convert.ToInt32(minute), Convert.ToInt32(second)));
                                            MessageBox.Show("成功同步本机时间！");
                                        }
                                    }
                                    break;

                                //同步坐标及方位角(0x11)
                                case (byte)MethodsClass.CommandCode.SynchronizePositionAndDirection:
                                    {
                                        MethodsClass.DeviceAddress transmittingDeviceAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize,
                                                                   receivingDeviceAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize;
                                        MethodsClass.CommandCode commandCode = MethodsClass.CommandCode.CommandCodeForInitialize;
                                        string x = String.Empty, y = String.Empty, h = String.Empty, direction = String.Empty;
                                        MethodsClass.ErrorCode unpackingResult = MethodsClass.UnpackingSynchronizePositionAndDirection(gValidBuffer,
                                                                                                                                       0,
                                                                                                                                       gStartPosition,
                                                                                                                                       ref transmittingDeviceAddress,
                                                                                                                                       ref receivingDeviceAddress,
                                                                                                                                       ref commandCode,
                                                                                                                                       ref x, ref y, ref h, ref direction);
                                        string content = String.Format("同步坐标及方位角为：X={0}Y={1}H={2}方位角(mil)={3}", x, y, h, direction);
                                        GlobalDefinitions.Command command = new GlobalDefinitions.Command(gCurrentTime,
                                                                                                          String.Format("0x{0:X}", transmittingDeviceAddress),
                                                                                                          GlobalDefinitions.deviceNames[(int)transmittingDeviceAddress],
                                                                                                          String.Format("0x{0:X}", receivingDeviceAddress),
                                                                                                          GlobalDefinitions.deviceNames[(int)receivingDeviceAddress],
                                                                                                          String.Format("0x{0:X}", commandCode),
                                                                                                          MethodsClass.TranslateCommandCodeToChinese(commandCode),
                                                                                                          content);
                                        ProcessPackage(command);
                                    }
                                    break;

                                //下发训练科目(0x12)
                                case (byte)MethodsClass.CommandCode.DistributeTrainingPlan:
                                    {
                                        MethodsClass.DeviceAddress transmittingDeviceAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize,
                                                                   receivingDeviceAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize;
                                        MethodsClass.CommandCode commandCode = MethodsClass.CommandCode.CommandCodeForInitialize;
                                        string year = String.Empty, month = String.Empty, day = String.Empty, hour = String.Empty, minute = String.Empty, second = String.Empty;
                                        MethodsClass.TrainingPlan trainingPlan = MethodsClass.TrainingPlan.TrainingPlanForInitialize;
                                        MethodsClass.ErrorCode unpackingResult = MethodsClass.UnpackingDistributeTrainingPlan(gValidBuffer,
                                                                                                                              0,
                                                                                                                              gStartPosition,
                                                                                                                              ref transmittingDeviceAddress,
                                                                                                                              ref receivingDeviceAddress,
                                                                                                                              ref commandCode,
                                                                                                                              ref year, ref month, ref day,
                                                                                                                              ref hour, ref minute, ref second,
                                                                                                                              ref trainingPlan);
                                        string content = String.Format("{0}年{1}月{2}日{3}时{4}分{5}秒，开始进行{6}", year, month, day, hour, minute, second,
                                                                                                                    MethodsClass.TranslateTrainingPlanToChinese(trainingPlan));
                                        GlobalDefinitions.Command command = new GlobalDefinitions.Command(gCurrentTime,
                                                                                                          String.Format("0x{0:X}", transmittingDeviceAddress),
                                                                                                          GlobalDefinitions.deviceNames[(int)transmittingDeviceAddress],
                                                                                                          String.Format("0x{0:X}", receivingDeviceAddress),
                                                                                                          GlobalDefinitions.deviceNames[(int)receivingDeviceAddress],
                                                                                                          String.Format("0x{0:X}", commandCode),
                                                                                                          MethodsClass.TranslateCommandCodeToChinese(commandCode),
                                                                                                          content);
                                        ProcessPackage(command);
                                    }
                                    break;

                                //下发战时信息(0x13)
                                case (byte)MethodsClass.CommandCode.DistributeWartimeInformation:
                                    {
                                        MethodsClass.DeviceAddress transmittingDeviceAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize,
                                                                   receivingDeviceAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize;
                                        MethodsClass.CommandCode commandCode = MethodsClass.CommandCode.CommandCodeForInitialize;
                                        MethodsClass.FloatUnion step = new MethodsClass.FloatUnion();
                                        string wartimeYear = String.Empty, wartimeMonth = String.Empty, wartimeDay = String.Empty,
                                               wartimeHour = String.Empty, wartimeMinute = String.Empty, wartimeSecond = String.Empty,
                                               astronomicaltimeYear = String.Empty, astronomicaltimeMonth = String.Empty, astronomicaltimeDay = String.Empty,
                                               astronomicaltimeHour = String.Empty, astronomicaltimeMinute = String.Empty, astronomicaltimeSecond = String.Empty;
                                        MethodsClass.ErrorCode unpackingResult = MethodsClass.UnpackingDistributeWartimeInformation(gValidBuffer,
                                                                                                                                    0,
                                                                                                                                    gStartPosition,
                                                                                                                                    ref transmittingDeviceAddress,
                                                                                                                                    ref receivingDeviceAddress,
                                                                                                                                    ref commandCode,
                                                                                                                                    ref step,
                                                                                                                                    ref wartimeYear, ref wartimeMonth, ref wartimeDay,
                                                                                                                                    ref wartimeHour, ref wartimeMinute, ref wartimeSecond,
                                                                                                                                    ref astronomicaltimeYear,
                                                                                                                                    ref astronomicaltimeMonth,
                                                                                                                                    ref astronomicaltimeDay,
                                                                                                                                    ref astronomicaltimeHour,
                                                                                                                                    ref astronomicaltimeMinute,
                                                                                                                                    ref astronomicaltimeSecond);
                                        string content = String.Format("步长{0:F4}，作战时间{1}年{2}月{3}日{4}时{5}分{6}秒，天文时间{1}年{2}月{3}日{4}时{5}分{6}秒",
                                                                        step.DataInFloat,
                                                                        wartimeYear, wartimeMonth, wartimeDay, wartimeHour, wartimeMinute, wartimeSecond,
                                                                        astronomicaltimeYear, astronomicaltimeMonth, astronomicaltimeDay,
                                                                        astronomicaltimeHour, astronomicaltimeMinute, astronomicaltimeSecond);
                                        GlobalDefinitions.Command command = new GlobalDefinitions.Command(gCurrentTime,
                                                                                                          String.Format("0x{0:X}", transmittingDeviceAddress),
                                                                                                          GlobalDefinitions.deviceNames[(int)transmittingDeviceAddress],
                                                                                                          String.Format("0x{0:X}", receivingDeviceAddress),
                                                                                                          GlobalDefinitions.deviceNames[(int)receivingDeviceAddress],
                                                                                                          String.Format("0x{0:X}", commandCode),
                                                                                                          MethodsClass.TranslateCommandCodeToChinese(commandCode),
                                                                                                          content);
                                        ProcessPackage(command);
                                    }
                                    break;

                                //控制训练过程(0x14)
                                case (byte)MethodsClass.CommandCode.ControlTrainingProcess:
                                    {
                                        MethodsClass.DeviceAddress transmittingDeviceAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize,
                                                                   receivingDeviceAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize;
                                        MethodsClass.CommandCode commandCode = MethodsClass.CommandCode.CommandCodeForInitialize;
                                        MethodsClass.TrainingProcessControlStrategy trainingProcessControlStrategy = MethodsClass.TrainingProcessControlStrategy.TrainingProcessForInitialize;
                                        MethodsClass.ErrorCode unpackingResult = MethodsClass.UnpackingControlTrainingProcess(gValidBuffer,
                                                                                                                              0,
                                                                                                                              gStartPosition,
                                                                                                                              ref transmittingDeviceAddress,
                                                                                                                              ref receivingDeviceAddress,
                                                                                                                              ref commandCode,
                                                                                                                              ref trainingProcessControlStrategy);
                                        GlobalDefinitions.Command command = new GlobalDefinitions.Command(gCurrentTime,
                                                                                                          String.Format("0x{0:X}", transmittingDeviceAddress),
                                                                                                          GlobalDefinitions.deviceNames[(int)transmittingDeviceAddress],
                                                                                                          String.Format("0x{0:X}", receivingDeviceAddress),
                                                                                                          GlobalDefinitions.deviceNames[(int)receivingDeviceAddress],
                                                                                                          String.Format("0x{0:X}", commandCode),
                                                                                                          MethodsClass.TranslateCommandCodeToChinese(commandCode),
                                                                                                          MethodsClass.TranslateTrainingProcessControlStrategyToChinese(trainingProcessControlStrategy));
                                        ProcessPackage(command);
                                    }
                                    break;

                                //软件状态监测(0x15)
                                case (byte)MethodsClass.CommandCode.SoftwareStatusCheck:
                                    {
                                        MethodsClass.DeviceAddress transmittingDeviceAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize,
                                                                   receivingDeviceAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize;
                                        MethodsClass.CommandCode commandCode = MethodsClass.CommandCode.CommandCodeForInitialize;
                                        MethodsClass.SoftwareStatusSubjectReport softwareStatusSubjectReport = MethodsClass.SoftwareStatusSubjectReport.SoftwareStatusSubjectReportForInitialize;
                                        MethodsClass.ErrorCode unpackingResult = MethodsClass.UnpackingSoftwareStatusCheck(gValidBuffer,
                                                                                                                           0,
                                                                                                                           gStartPosition,
                                                                                                                           ref transmittingDeviceAddress,
                                                                                                                           ref receivingDeviceAddress,
                                                                                                                           ref commandCode,
                                                                                                                           ref softwareStatusSubjectReport);
                                        GlobalDefinitions.Command command = new GlobalDefinitions.Command(gCurrentTime,
                                                                                                          String.Format("0x{0:X}", transmittingDeviceAddress),
                                                                                                          GlobalDefinitions.deviceNames[(int)transmittingDeviceAddress],
                                                                                                          String.Format("0x{0:X}", receivingDeviceAddress),
                                                                                                          GlobalDefinitions.deviceNames[(int)receivingDeviceAddress],
                                                                                                          String.Format("0x{0:X}", commandCode),
                                                                                                          MethodsClass.TranslateCommandCodeToChinese(commandCode),
                                                                                                          MethodsClass.TranslateSoftwareStatusSubjectReportToChinese(softwareStatusSubjectReport));
                                        ProcessPackage(command);
                                    }
                                    break;

                                //桌面监视控制(0x16)
                                case (byte)MethodsClass.CommandCode.DesktopMonitorControl:
                                    {
                                        MethodsClass.DeviceAddress transmittingDeviceAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize,
                                                                   receivingDeviceAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize;
                                        MethodsClass.CommandCode commandCode = MethodsClass.CommandCode.CommandCodeForInitialize;
                                        MethodsClass.DesktopMonitorControlCategory desktopMonitorControlCategory = MethodsClass.DesktopMonitorControlCategory.DesktopMonitorControlCategoryForInitialize;
                                        MethodsClass.ErrorCode unpackingResult = MethodsClass.UnpackingDesktopMonitorControl(gValidBuffer,
                                                                                                                             0,
                                                                                                                             gStartPosition,
                                                                                                                             ref transmittingDeviceAddress,
                                                                                                                             ref receivingDeviceAddress,
                                                                                                                             ref commandCode,
                                                                                                                             ref desktopMonitorControlCategory);
                                        GlobalDefinitions.Command command = new GlobalDefinitions.Command(gCurrentTime,
                                                                                                          String.Format("0x{0:X}", transmittingDeviceAddress),
                                                                                                          GlobalDefinitions.deviceNames[(int)transmittingDeviceAddress],
                                                                                                          String.Format("0x{0:X}", receivingDeviceAddress),
                                                                                                          GlobalDefinitions.deviceNames[(int)receivingDeviceAddress],
                                                                                                          String.Format("0x{0:X}", commandCode),
                                                                                                          MethodsClass.TranslateCommandCodeToChinese(commandCode),
                                                                                                          MethodsClass.TranslateDesktopMonitorControlCategoryToChinese(desktopMonitorControlCategory));
                                        ProcessPackage(command);
                                    }
                                    break;

                                //请求上传作业过程数据(0x17)
                                case (byte)MethodsClass.CommandCode.RequestUploadProcessData:
                                    {
                                        MethodsClass.DeviceAddress transmittingDeviceAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize,
                                                                   receivingDeviceAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize,
                                                                   deviceWaitForUploadProcessData = MethodsClass.DeviceAddress.DeviceAddressForInitialize;
                                        MethodsClass.CommandCode commandCode = MethodsClass.CommandCode.CommandCodeForInitialize;
                                        MethodsClass.RequestUploadProcessDataFlag requestUploadProcessDataFlag = MethodsClass.RequestUploadProcessDataFlag.TrainingProcessForInitialize;
                                        MethodsClass.ErrorCode unpackingResult = MethodsClass.UnpackingRequestUploadProcessData(gValidBuffer,
                                                                                                                                0,
                                                                                                                                gStartPosition,
                                                                                                                                ref transmittingDeviceAddress,
                                                                                                                                ref receivingDeviceAddress,
                                                                                                                                ref commandCode,
                                                                                                                                ref deviceWaitForUploadProcessData,
                                                                                                                                ref requestUploadProcessDataFlag);
                                        GlobalDefinitions.Command command = new GlobalDefinitions.Command(gCurrentTime,
                                                                                                          String.Format("0x{0:X}", transmittingDeviceAddress),
                                                                                                          GlobalDefinitions.deviceNames[(int)transmittingDeviceAddress],
                                                                                                          String.Format("0x{0:X}", receivingDeviceAddress),
                                                                                                          GlobalDefinitions.deviceNames[(int)receivingDeviceAddress],
                                                                                                          String.Format("0x{0:X}", commandCode),
                                                                                                          MethodsClass.TranslateCommandCodeToChinese(commandCode),
                                                                                                          MethodsClass.TranslateRequestUploadProcessDataFlagToChinese(requestUploadProcessDataFlag));
                                        ProcessPackage(command);
                                    }
                                    break;

                                //完成训练科目(0x18)
                                case (byte)MethodsClass.CommandCode.TrainingPlanAccomplished:
                                    {
                                        MethodsClass.DeviceAddress transmittingDeviceAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize,
                                                                   receivingDeviceAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize;
                                        MethodsClass.CommandCode commandCode = MethodsClass.CommandCode.CommandCodeForInitialize;
                                        MethodsClass.TrainingPlan compeletedTrainingPlan = MethodsClass.TrainingPlan.LianCeJiaoZhengDian;
                                        byte trainingPlanTotalSteps = 0x00;
                                        MethodsClass.TrainingPlanAccomplishedFlag trainingPlanAccomplishedFlag = MethodsClass.TrainingPlanAccomplishedFlag.TrainingPlanAccomplishedFlagForInitialize;
                                        string trainingContent = string.Empty;
                                        MethodsClass.ErrorCode unpackingResult = MethodsClass.UnpackingTrainingPlanAccomplished(gValidBuffer,
                                                                                                                                0,
                                                                                                                                gStartPosition,
                                                                                                                                ref transmittingDeviceAddress,
                                                                                                                                ref receivingDeviceAddress,
                                                                                                                                ref commandCode,
                                                                                                                                ref compeletedTrainingPlan,
                                                                                                                                ref trainingPlanTotalSteps,
                                                                                                                                ref trainingPlanAccomplishedFlag,
                                                                                                                                ref trainingContent);
                                        int temp = (int)trainingPlanAccomplishedFlag;
                                        GlobalDefinitions.Command command = new GlobalDefinitions.Command(gCurrentTime,
                                                                                                          String.Format("0x{0:X}", transmittingDeviceAddress),
                                                                                                          GlobalDefinitions.deviceNames[(int)transmittingDeviceAddress],
                                                                                                          String.Format("0x{0:X}", receivingDeviceAddress),
                                                                                                          GlobalDefinitions.deviceNames[(int)receivingDeviceAddress],
                                                                                                          String.Format("0x{0:X}", commandCode),
                                                                                                          MethodsClass.TranslateCommandCodeToChinese(commandCode),
                                                                                                          trainingContent);

                                        ProcessPackage(command);
                                    }
                                    break;

                                //完成战时信息配置(0x19)
                                case (byte)MethodsClass.CommandCode.WartimgInformationConfigureAccomplished:
                                    {
                                        MethodsClass.DeviceAddress transmittingDeviceAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize,
                                                                   receivingDeviceAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize;
                                        MethodsClass.CommandCode commandCode = MethodsClass.CommandCode.CommandCodeForInitialize;
                                        MethodsClass.ErrorCode unpackingResult = MethodsClass.UnpackingWartimgInformationConfigureAccomplished(gValidBuffer,
                                                                                                                                               0,
                                                                                                                                               gStartPosition,
                                                                                                                                               ref transmittingDeviceAddress,
                                                                                                                                               ref receivingDeviceAddress,
                                                                                                                                               ref commandCode);
                                        GlobalDefinitions.Command command = new GlobalDefinitions.Command(gCurrentTime,
                                                                                                          String.Format("0x{0:X}", transmittingDeviceAddress),
                                                                                                          GlobalDefinitions.deviceNames[(int)transmittingDeviceAddress],
                                                                                                          String.Format("0x{0:X}", receivingDeviceAddress),
                                                                                                          GlobalDefinitions.deviceNames[(int)receivingDeviceAddress],
                                                                                                          String.Format("0x{0:X}", commandCode),
                                                                                                          MethodsClass.TranslateCommandCodeToChinese(commandCode),
                                                                                                          "完成战时信息配置");
                                        ProcessPackage(command);
                                    }
                                    break;

                                //完成训练过程控制(0x1A)
                                case (byte)MethodsClass.CommandCode.ControlTrainingProcessAccomplished:
                                    {
                                        MethodsClass.DeviceAddress transmittingDeviceAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize,
                                                                   receivingDeviceAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize;
                                        MethodsClass.CommandCode commandCode = MethodsClass.CommandCode.CommandCodeForInitialize;
                                        MethodsClass.TrainingProcessControlStrategy trainingProcessControlStrategy = MethodsClass.TrainingProcessControlStrategy.TrainingProcessForInitialize;
                                        MethodsClass.ErrorCode unpackingResult = MethodsClass.UnpackingControlTrainingProcessAccomplished(gValidBuffer,
                                                                                                                                          0,
                                                                                                                                          gStartPosition,
                                                                                                                                          ref transmittingDeviceAddress,
                                                                                                                                          ref receivingDeviceAddress,
                                                                                                                                          ref commandCode,
                                                                                                                                          ref trainingProcessControlStrategy);
                                        GlobalDefinitions.Command command = new GlobalDefinitions.Command(gCurrentTime,
                                                                                                          String.Format("0x{0:X}", transmittingDeviceAddress),
                                                                                                          GlobalDefinitions.deviceNames[(int)transmittingDeviceAddress],
                                                                                                          String.Format("0x{0:X}", receivingDeviceAddress),
                                                                                                          GlobalDefinitions.deviceNames[(int)receivingDeviceAddress],
                                                                                                          String.Format("0x{0:X}", commandCode),
                                                                                                          MethodsClass.TranslateCommandCodeToChinese(commandCode),
                                                                                                          MethodsClass.TranslateTrainingProcessControlStrategyToChinese(trainingProcessControlStrategy));

                                        ProcessPackage(command);
                                    }
                                    break;

                                //上传软件状态(0x1B)
                                case (byte)MethodsClass.CommandCode.UploadSoftwareStatus:
                                    {
                                        MethodsClass.DeviceAddress transmittingDeviceAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize,
                                                                   receivingDeviceAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize;
                                        MethodsClass.CommandCode commandCode = MethodsClass.CommandCode.CommandCodeForInitialize;
                                        MethodsClass.SoftwareStatus softwareStatus = MethodsClass.SoftwareStatus.SoftwareStatusForInitialize;
                                        string softwareStatusIllustration = String.Empty, softwareVersion = String.Empty;
                                        MethodsClass.ErrorCode unpackingResult = MethodsClass.UnpackingUploadSoftwareStatus(gValidBuffer,
                                                                                                                            0,
                                                                                                                            gStartPosition,
                                                                                                                            ref transmittingDeviceAddress,
                                                                                                                            ref receivingDeviceAddress,
                                                                                                                            ref commandCode,
                                                                                                                            ref softwareStatus,
                                                                                                                            ref softwareStatusIllustration,
                                                                                                                            ref softwareVersion);
                                        string content = String.Format("{0}，{1}，版本号：{2}", MethodsClass.TranslateSoftwareStatusToChinese(softwareStatus),
                                                                       softwareStatusIllustration, softwareVersion);
                                        GlobalDefinitions.Command command = new GlobalDefinitions.Command(gCurrentTime,
                                                                                                          String.Format("0x{0:X}", transmittingDeviceAddress),
                                                                                                          GlobalDefinitions.deviceNames[(int)transmittingDeviceAddress],
                                                                                                          String.Format("0x{0:X}", receivingDeviceAddress),
                                                                                                          GlobalDefinitions.deviceNames[(int)receivingDeviceAddress],
                                                                                                          String.Format("0x{0:X}", commandCode),
                                                                                                          MethodsClass.TranslateCommandCodeToChinese(commandCode),
                                                                                                          content);
                                        ProcessPackage(command);
                                    }
                                    break;

                                //上传作业数据(0x1D)
                                case (byte)MethodsClass.CommandCode.UploadProcessData:
                                    {
                                        MethodsClass.DeviceAddress transmittingDeviceAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize,
                                                                   receivingDeviceAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize;
                                        MethodsClass.CommandCode commandCode = MethodsClass.CommandCode.CommandCodeForInitialize;
                                        MethodsClass.ProcessDataCommand processDataCommand = MethodsClass.ProcessDataCommand.ProcessDataCommandForInitialize;
                                        byte[] targetBuffer = new byte[256];
                                        int targetBufferLength = 0;
                                        MethodsClass.ErrorCode unpackingResult = MethodsClass.UnpackingUploadProcessData(gValidBuffer,
                                                                                                                            0,
                                                                                                                            gStartPosition,
                                                                                                                            ref transmittingDeviceAddress,
                                                                                                                            ref receivingDeviceAddress,
                                                                                                                            ref commandCode,
                                                                                                                            ref processDataCommand,
                                                                                                                            ref targetBuffer,
                                                                                                                            ref targetBufferLength);
                                        GlobalDefinitions.Command command = new GlobalDefinitions.Command(gCurrentTime,
                                                                                                          String.Format("0x{0:X}", transmittingDeviceAddress),
                                                                                                          GlobalDefinitions.deviceNames[(int)transmittingDeviceAddress],
                                                                                                          String.Format("0x{0:X}", receivingDeviceAddress),
                                                                                                          GlobalDefinitions.deviceNames[(int)receivingDeviceAddress],
                                                                                                          String.Format("0x{0:X}", commandCode),
                                                                                                          MethodsClass.TranslateCommandCodeToChinese(commandCode),
                                                                                                          MethodsClass.TranslateProcessDataCommandToChinese(processDataCommand));
                                        ProcessPackage(command);
                                    }
                                    break;
                            }

                            gRemainDataLength = GetRemainDataLength(gValidDataPosition, gStartPosition, gDataQueuePointer);
                            gDataQueuePointer = MoveForwardQueue(gStartPosition + gValidDataPosition, gRemainDataLength, gDataQueue);
                            gStartPosition = 0;
                            gLoopForDataValidation = 0;
                            goto LOOP_BEGIN;
                        }
                    }
                    gIsDataProcessing = false;
                }
            }
        }
        #endregion//串口相关逻辑，如获取串口数据、串口缓冲区前移等

        #region 数据监控界面配置和处理逻辑

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

        /// <summary>
        /// 配置数据监控界面中的各ListView和数据源之间的数据绑定
        /// </summary>
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

        private List<GlobalDefinitions.Device> gDevices = new List<GlobalDefinitions.Device>();

        /// <summary>
        /// 设置数据监控界面中各软件的监控状态及过滤显示状态
        /// </summary>
        private void SetDeviceMonitoringStatus()
        {
            GlobalDefinitions.Device device = new GlobalDefinitions.Device();
            device.transmittedDataCounter = 0;
            device.receivedDataCounter = 0;

            //导调软件监控状态
            device.deviceIndex = GlobalDefinitions.DeviceIndex.DaoDiao;
            device.monitoringStatus = GlobalDefinitions.DeviceMonitoringStatus.BeginMonitoring;
            deviceInformationFilter.showInformationOfDaoDiaoRuanJian = true;
            gDevices.Add(device);

            device = new GlobalDefinitions.Device();
            device.transmittedDataCounter = 0;
            device.receivedDataCounter = 0;
            //联动软件监控状态
            device.deviceIndex = GlobalDefinitions.DeviceIndex.LianDong;
            device.monitoringStatus = GlobalDefinitions.DeviceMonitoringStatus.BeginMonitoring;
            deviceInformationFilter.showInformationOfLianDongRuanJian = true;
            gDevices.Add(device);

            device = new GlobalDefinitions.Device();
            device.transmittedDataCounter = 0;
            device.receivedDataCounter = 0;
            //桌面监控软件监控状态
            device.deviceIndex = GlobalDefinitions.DeviceIndex.ZhuoMianJianKong;
            device.monitoringStatus = GlobalDefinitions.DeviceMonitoringStatus.BeginMonitoring;
            deviceInformationFilter.showInformationOfZhuoMianJianKongRuanJian = true;
            gDevices.Add(device);

            device = new GlobalDefinitions.Device();
            device.transmittedDataCounter = 0;
            device.receivedDataCounter = 0;
            //数据监控软件监控状态
            device.deviceIndex = GlobalDefinitions.DeviceIndex.ShuJuJianKong;
            device.monitoringStatus = GlobalDefinitions.DeviceMonitoringStatus.BeginMonitoring;
            deviceInformationFilter.showInformationOfShuJuJianKongRuanJian = true;
            gDevices.Add(device);

            device = new GlobalDefinitions.Device();
            device.transmittedDataCounter = 0;
            device.receivedDataCounter = 0;
            //训练代理软件监控状态
            device.deviceIndex = GlobalDefinitions.DeviceIndex.XunLianDaiLi;
            device.monitoringStatus = GlobalDefinitions.DeviceMonitoringStatus.BeginMonitoring;
            deviceInformationFilter.showInformationOfXunLianDaiLiRuanJian = true;
            gDevices.Add(device);

            device = new GlobalDefinitions.Device();
            device.transmittedDataCounter = 0;
            device.receivedDataCounter = 0;
            //全站仪监控状态
            device.deviceIndex = GlobalDefinitions.DeviceIndex.QuanZhanYi;
            device.monitoringStatus = GlobalDefinitions.DeviceMonitoringStatus.BeginMonitoring;
            deviceInformationFilter.showInformationOfQuanZhanYi = true;
            gDevices.Add(device);

            device = new GlobalDefinitions.Device();
            device.transmittedDataCounter = 0;
            device.receivedDataCounter = 0;
            //控显器监控状态
            device.deviceIndex = GlobalDefinitions.DeviceIndex.KongXianQi;
            device.monitoringStatus = GlobalDefinitions.DeviceMonitoringStatus.BeginMonitoring;
            deviceInformationFilter.showInformationOfKongXianQi = true;
            gDevices.Add(device);

            device = new GlobalDefinitions.Device();
            device.transmittedDataCounter = 0;
            device.receivedDataCounter = 0;
            //陀螺仪监控状态
            device.deviceIndex = GlobalDefinitions.DeviceIndex.TuoLuoYi;
            device.monitoringStatus = GlobalDefinitions.DeviceMonitoringStatus.BeginMonitoring;
            deviceInformationFilter.showInformationOfTuoLuoYi = true;
            gDevices.Add(device);

            device = new GlobalDefinitions.Device();
            device.transmittedDataCounter = 0;
            device.receivedDataCounter = 0;
            //激光测距机监控状态
            device.deviceIndex = GlobalDefinitions.DeviceIndex.JiGuangCeJuJi;
            device.monitoringStatus = GlobalDefinitions.DeviceMonitoringStatus.BeginMonitoring;
            deviceInformationFilter.showInformationOfJiGuangCeJuJi = true;
            gDevices.Add(device);

            device = new GlobalDefinitions.Device();
            device.transmittedDataCounter = 0;
            device.receivedDataCounter = 0;
            //北斗用户机监控状态
            device.deviceIndex = GlobalDefinitions.DeviceIndex.BeiDouYongHuJi;
            device.monitoringStatus = GlobalDefinitions.DeviceMonitoringStatus.BeginMonitoring;
            deviceInformationFilter.showInformationOfBeiDouYongHuJi = true;
            gDevices.Add(device);

            device = new GlobalDefinitions.Device();
            device.transmittedDataCounter = 0;
            device.receivedDataCounter = 0;
            //打印机监控状态
            device.deviceIndex = GlobalDefinitions.DeviceIndex.DaYinJi;
            device.monitoringStatus = GlobalDefinitions.DeviceMonitoringStatus.BeginMonitoring;
            deviceInformationFilter.showInformationOfDaYinJi = true;
            gDevices.Add(device);

            device = new GlobalDefinitions.Device();
            device.transmittedDataCounter = 0;
            device.receivedDataCounter = 0;
            //定位定向系统监控状态
            device.deviceIndex = GlobalDefinitions.DeviceIndex.DingWeiDingXiangXiTong;
            device.monitoringStatus = GlobalDefinitions.DeviceMonitoringStatus.BeginMonitoring;
            deviceInformationFilter.showInformationOfDingWeiDingXiangXiTong = true;
            gDevices.Add(device);

            device = new GlobalDefinitions.Device();
            device.transmittedDataCounter = 0;
            device.receivedDataCounter = 0;
            //驾驶模拟系统监控状态
            device.deviceIndex = GlobalDefinitions.DeviceIndex.JiaShiMoNiXiTong;
            device.monitoringStatus = GlobalDefinitions.DeviceMonitoringStatus.BeginMonitoring;
            deviceInformationFilter.showInformationOfJiaShiMoNiXiTong = true;
            gDevices.Add(device);
        }

        /// <summary>
        /// 在数据监控界面显示接收到的Command格式的数据包
        /// </summary>
        /// <param name="command">Command类型变量</param>
        private void ShowReceivedPackage(GlobalDefinitions.Command command)
        {
            if (!gPauseShowInformation)
            {
                //switch (command.receivingDeviceAddressInChinese)
                switch (command.transmittingDeviceAddressInChinese)
                {
                    case "导调软件":
                        {
                            if (deviceInformationFilter.showInformationOfDaoDiaoRuanJian)
                            {
                                lvDaoDiaoReceivedPackageSource.Add(command);

                                if (gNeedShowLatestInformation)
                                {
                                    lvDaoDiaoReceivedPackages.ScrollIntoView(command);
                                }

                                if (gDevices[(int)GlobalDefinitions.DeviceIndex.DaoDiao].transmittedDataCounter % 500 == 0)
                                {
                                    lvDaoDiaoReceivedPackages.ItemsSource = null;
                                    lvDaoDiaoReceivedPackageSource.Clear();
                                    lvDaoDiaoReceivedPackages.ItemsSource = lvDaoDiaoReceivedPackageSource;
                                }
                            }
                        }
                        break;
                    case "联动软件":
                        {
                            if (deviceInformationFilter.showInformationOfLianDongRuanJian)
                            {
                                lvLianDongReceivedPackageSource.Add(command);

                                if (gNeedShowLatestInformation)
                                {
                                    lvLianDongReceivedPackages.ScrollIntoView(command);
                                }

                                if (gDevices[(int)GlobalDefinitions.DeviceIndex.LianDong].transmittedDataCounter % 500 == 0)
                                {
                                    lvLianDongReceivedPackages.ItemsSource = null;
                                    lvLianDongReceivedPackageSource.Clear();
                                    lvLianDongReceivedPackages.ItemsSource = lvLianDongReceivedPackageSource;
                                }
                            }
                        }
                        break;
                    case "桌面监控软件":
                        {
                            if (deviceInformationFilter.showInformationOfZhuoMianJianKongRuanJian)
                            {
                                lvZhuoMianJianKongReceivedPackageSource.Add(command);

                                if (gNeedShowLatestInformation)
                                {
                                    lvZhuoMianJianKongReceivedPackages.ScrollIntoView(command);
                                }

                                if (gDevices[(int)GlobalDefinitions.DeviceIndex.ZhuoMianJianKong].transmittedDataCounter % 500 == 0)
                                {
                                    lvZhuoMianJianKongReceivedPackages.ItemsSource = null;
                                    lvZhuoMianJianKongReceivedPackageSource.Clear();
                                    lvZhuoMianJianKongReceivedPackages.ItemsSource = lvZhuoMianJianKongReceivedPackageSource;
                                }
                            }
                        }
                        break;
                    case "训练代理软件":
                        {
                            if (deviceInformationFilter.showInformationOfXunLianDaiLiRuanJian)
                            {
                                lvXunLianDaiLiReceivedPackageSource.Add(command);

                                if (gNeedShowLatestInformation)
                                {
                                    lvXunLianDaiLiReceivedPackages.ScrollIntoView(command);
                                }

                                if (gDevices[(int)GlobalDefinitions.DeviceIndex.XunLianDaiLi].transmittedDataCounter % 500 == 0)
                                {
                                    lvXunLianDaiLiReceivedPackages.ItemsSource = null;
                                    lvXunLianDaiLiReceivedPackageSource.Clear();
                                    lvXunLianDaiLiReceivedPackages.ItemsSource = lvXunLianDaiLiReceivedPackageSource;
                                }
                            }
                        }
                        break;
                    case "全站仪":
                        {
                            if (deviceInformationFilter.showInformationOfQuanZhanYi)
                            {
                                lvQuanZhanYiReceivedPackageSource.Add(command);

                                if (gNeedShowLatestInformation)
                                {
                                    lvQuanZhanYiReceivedPackages.ScrollIntoView(command);
                                }

                                if (gDevices[(int)GlobalDefinitions.DeviceIndex.QuanZhanYi].transmittedDataCounter % 500 == 0)
                                {
                                    lvQuanZhanYiReceivedPackages.ItemsSource = null;
                                    lvQuanZhanYiReceivedPackageSource.Clear();
                                    lvQuanZhanYiReceivedPackages.ItemsSource = lvQuanZhanYiReceivedPackageSource;
                                }
                            }
                        }
                        break;
                    case "控显器":
                        {
                            if (deviceInformationFilter.showInformationOfKongXianQi)
                            {
                                lvKongXianQiReceivedPackageSource.Add(command);

                                if (gNeedShowLatestInformation)
                                {
                                    lvKongXianQiReceivedPackages.ScrollIntoView(command);
                                }

                                if (gDevices[(int)GlobalDefinitions.DeviceIndex.KongXianQi].transmittedDataCounter % 500 == 0)
                                {
                                    lvKongXianQiReceivedPackages.ItemsSource = null;
                                    lvKongXianQiReceivedPackageSource.Clear();
                                    lvKongXianQiReceivedPackages.ItemsSource = lvKongXianQiReceivedPackageSource;
                                }
                            }
                        }
                        break;
                    case "陀螺仪":
                        {
                            if (deviceInformationFilter.showInformationOfTuoLuoYi)
                            {
                                lvTuoLuoYiReceivedPackageSource.Add(command);

                                if (gNeedShowLatestInformation)
                                {
                                    lvTuoLuoYiReceivedPackages.ScrollIntoView(command);
                                }

                                if (gDevices[(int)GlobalDefinitions.DeviceIndex.TuoLuoYi].transmittedDataCounter % 500 == 0)
                                {
                                    lvTuoLuoYiReceivedPackages.ItemsSource = null;
                                    lvTuoLuoYiReceivedPackageSource.Clear();
                                    lvTuoLuoYiReceivedPackages.ItemsSource = lvTuoLuoYiReceivedPackageSource;
                                }
                            }
                        }
                        break;
                    case "激光测距机":
                        {
                            if (deviceInformationFilter.showInformationOfJiGuangCeJuJi)
                            {
                                lvJiGuangCeJuJiReceivedPackageSource.Add(command);

                                if (gNeedShowLatestInformation)
                                {
                                    lvJiGuangCeJuJiReceivedPackages.ScrollIntoView(command);
                                }

                                if (gDevices[(int)GlobalDefinitions.DeviceIndex.JiGuangCeJuJi].transmittedDataCounter % 500 == 0)
                                {
                                    lvJiGuangCeJuJiReceivedPackages.ItemsSource = null;
                                    lvJiGuangCeJuJiReceivedPackageSource.Clear();
                                    lvJiGuangCeJuJiReceivedPackages.ItemsSource = lvJiGuangCeJuJiReceivedPackageSource;
                                }
                            }
                        }
                        break;
                    case "北斗用户机":
                        {
                            if (deviceInformationFilter.showInformationOfBeiDouYongHuJi)
                            {
                                lvBeiDouYongHuJiReceivedPackageSource.Add(command);

                                if (gNeedShowLatestInformation)
                                {
                                    lvBeiDouYongHuJiReceivedPackages.ScrollIntoView(command);
                                }

                                if (gDevices[(int)GlobalDefinitions.DeviceIndex.BeiDouYongHuJi].transmittedDataCounter % 500 == 0)
                                {
                                    lvBeiDouYongHuJiReceivedPackages.ItemsSource = null;
                                    lvBeiDouYongHuJiReceivedPackageSource.Clear();
                                    lvBeiDouYongHuJiReceivedPackages.ItemsSource = lvBeiDouYongHuJiReceivedPackageSource;
                                }
                            }
                        }
                        break;
                    case "打印机":
                        {
                            if (deviceInformationFilter.showInformationOfDaYinJi)
                            {
                                lvDaYinJiReceivedPackageSource.Add(command);

                                if (gNeedShowLatestInformation)
                                {
                                    lvDaYinJiReceivedPackages.ScrollIntoView(command);
                                }

                                if (gDevices[(int)GlobalDefinitions.DeviceIndex.DaYinJi].transmittedDataCounter % 500 == 0)
                                {
                                    lvDaYinJiReceivedPackages.ItemsSource = null;
                                    lvDaYinJiReceivedPackageSource.Clear();
                                    lvDaYinJiReceivedPackages.ItemsSource = lvDaYinJiReceivedPackageSource;
                                }
                            }
                        }
                        break;
                    case "定位定向系统":
                        {
                            if (deviceInformationFilter.showInformationOfDingWeiDingXiangXiTong)
                            {
                                lvDingWeiDingXiangXiTongReceivedPackageSource.Add(command);


                                if (gNeedShowLatestInformation)
                                {
                                    lvDingWeiDingXiangXiTongReceivedPackages.ScrollIntoView(command);
                                }

                                if (gDevices[(int)GlobalDefinitions.DeviceIndex.DingWeiDingXiangXiTong].transmittedDataCounter % 500 == 0)
                                {
                                    lvDingWeiDingXiangXiTongReceivedPackages.ItemsSource = null;
                                    lvDingWeiDingXiangXiTongReceivedPackageSource.Clear();
                                    lvDingWeiDingXiangXiTongReceivedPackages.ItemsSource = lvDingWeiDingXiangXiTongReceivedPackageSource;
                                }
                            }
                        }
                        break;
                    case "驾驶模拟系统":
                        {
                            if (deviceInformationFilter.showInformationOfJiaShiMoNiXiTong)
                            {
                                lvJiaShiMoNiXiTongReceivedPackageSource.Add(command);

                                if (gNeedShowLatestInformation)
                                {
                                    lvJiaShiMoNiXiTongReceivedPackages.ScrollIntoView(command);
                                }

                                if (gDevices[(int)GlobalDefinitions.DeviceIndex.JiaShiMoNiXiTong].transmittedDataCounter % 500 == 0)
                                {
                                    lvJiaShiMoNiXiTongReceivedPackages.ItemsSource = null;
                                    lvJiaShiMoNiXiTongReceivedPackageSource.Clear();
                                    lvJiaShiMoNiXiTongReceivedPackages.ItemsSource = lvJiaShiMoNiXiTongReceivedPackageSource;
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 向数据监控界面中的过滤显示按钮的上下文菜单中添加信息
        /// </summary>
        /// <param name="menuItemHeader">待显示的标题</param>
        /// <param name="cm">上下文菜单引用类型变量</param>
        private void AddAttachmentToContextMenu(string menuItemHeader, ref ContextMenu cm)
        {
            MenuItem mi = new MenuItem()
            {
                Header = menuItemHeader
            };
            mi.Click += Btn_monitor_informationFilter_MenuItemClicked;
            cm.Items.Add(mi);
        }

        /// <summary>
        /// 向数据监控界面中的过滤显示按钮的上下文菜单中添加信息
        /// </summary>
        /// <param name="menuItemHeader">待显示的标题</param>
        /// <param name="isChecked">标题是否被选中（打勾）</param>
        /// <param name="cm">上下文菜单引用类型变量</param>
        private void AddAttachmentToContextMenu(string menuItemHeader, bool isChecked, ref ContextMenu cm)
        {
            MenuItem mi = new MenuItem()
            {
                Header = menuItemHeader,
                IsChecked = isChecked,
                IsCheckable = true
            };
            mi.Click += Btn_monitor_informationFilter_MenuItemClicked;
            cm.Items.Add(mi);
        }

        private GlobalDefinitions.DeviceInformationFilter deviceInformationFilter;

        /// <summary>
        /// 数据监控界面中的过滤显示按钮的上下文菜单内的菜单项被点击后的响应函数
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">RoutedEventArgs</param>
        private void Btn_monitor_informationFilter_MenuItemClicked(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;

            switch ((string)item.Header)
            {
                case "显示 导调软件 发送的数据":
                    {
                        if (item.IsChecked)
                        {
                            deviceInformationFilter.showInformationOfDaoDiaoRuanJian = true;
                        }
                        else
                        {
                            deviceInformationFilter.showInformationOfDaoDiaoRuanJian = false;
                        }
                    }
                    break;
                case "显示 联动软件 发送的数据":
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
                case "显示 桌面监控软件 发送的数据":
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
                case "显示 数据监控软件 发送的数据":
                    {
                        if (item.IsChecked)
                        {
                            deviceInformationFilter.showInformationOfShuJuJianKongRuanJian = true;
                        }
                        else
                        {
                            deviceInformationFilter.showInformationOfShuJuJianKongRuanJian = false;
                        }
                    }
                    break;
                case "显示 训练代理软件 发送的数据":
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
                case "显示 驾驶模拟系统 发送的数据":
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
                case "显示 全站仪 发送的数据":
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
                case "显示 控显器 发送的数据":
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
                case "显示 陀螺仪 发送的数据":
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
                case "显示 激光测距机 发送的数据":
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
                case "显示 北斗用户机 发送的数据":
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
                case "显示 打印机 发送的数据":
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
                case "显示 定位定向系统 发送的数据":
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

        private ContextMenu gMonitorInformationFilterContextMenu = new ContextMenu();
        private bool gIsMonitorInformationFilterContextMenuInitialized = false;

        /// <summary>
        /// 数据监控界面中过滤显示按钮点击事件响应函数
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">RoutedEventArgs</param>
        private void Btn_monitor_informtionFilter_Click(object sender, RoutedEventArgs e)
        {
            if (!gIsMonitorInformationFilterContextMenuInitialized)
            {
                gIsMonitorInformationFilterContextMenuInitialized = true;
                gMonitorInformationFilterContextMenu = new ContextMenu();

                AddAttachmentToContextMenu("显示 导调软件 发送的数据", true, ref gMonitorInformationFilterContextMenu);
                AddAttachmentToContextMenu("显示 联动软件 发送的数据", true, ref gMonitorInformationFilterContextMenu);
                AddAttachmentToContextMenu("显示 桌面监控软件 发送的数据", true, ref gMonitorInformationFilterContextMenu);
                AddAttachmentToContextMenu("显示 训练代理软件 发送的数据", true, ref gMonitorInformationFilterContextMenu);
                AddAttachmentToContextMenu("显示 驾驶模拟系统 发送的数据", true, ref gMonitorInformationFilterContextMenu);
                AddAttachmentToContextMenu("显示 全站仪 发送的数据", true, ref gMonitorInformationFilterContextMenu);
                AddAttachmentToContextMenu("显示 控显器 发送的数据", true, ref gMonitorInformationFilterContextMenu);
                AddAttachmentToContextMenu("显示 陀螺仪 发送的数据", true, ref gMonitorInformationFilterContextMenu);
                AddAttachmentToContextMenu("显示 激光测距机 发送的数据", true, ref gMonitorInformationFilterContextMenu);
                AddAttachmentToContextMenu("显示 北斗用户机 发送的数据", true, ref gMonitorInformationFilterContextMenu);
                AddAttachmentToContextMenu("显示 打印机 发送的数据", true, ref gMonitorInformationFilterContextMenu);
                AddAttachmentToContextMenu("显示 定位定向系统 发送的数据", true, ref gMonitorInformationFilterContextMenu);

                /*AddAttachmentToContextMenu("菜单项1", ref cmMonitorInformationFilter);
                AddAttachmentToContextMenu("菜单项2", true, ref cmMonitorInformationFilter);
                cmMonitorInformationFilter.Items.Add(new MenuItem { Header = "菜单项3", IsChecked = true, IsEnabled = false });
                cmMonitorInformationFilter.Items.Add(new Separator());
                cmMonitorInformationFilter.Items.Add(CreateSubMenu("菜单项4"));                
                var menu = CreateSubMenu("菜单项5");
                menu.IsEnabled = false;
                cmMonitorInformationFilter.Items.Add(menu);*/

                gMonitorInformationFilterContextMenu.IsOpen = true;
            }
            else
            {
                //过滤显示菜单被初始化，直接显示即可
                gMonitorInformationFilterContextMenu.IsOpen = true;
            }
        }

        private bool gNeedShowLatestInformation = false;
        /// <summary>
        /// 数据监控界面中“总是显示最新”按钮点击事件响应函数
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">RoutedEventArgs</param>
        private void cb_monitor_show_latest_information_Click(object sender, RoutedEventArgs e)
        {
            gNeedShowLatestInformation = (bool)cb_monitor_show_latest_information.IsChecked;
        }

        private bool gPauseShowInformation = false;
        /// <summary>
        /// 数据监控界面中“暂停显示”按钮点击事件响应函数
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">RoutedEventArgs</param>
        private void cb_monitor_show_information_pause_Click(object sender, RoutedEventArgs e)
        {
            gPauseShowInformation = (bool)cb_monitor_show_information_pause.IsChecked;
        }

        /// <summary>
        /// 数据监控界面中“开始监控”按钮点击事件响应函数
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">路由事件参数</param>
        private void Btn_monitor_uploadProcessData_start_Click(object sender, RoutedEventArgs e)
        {
            MethodsClass.DeviceAddress receivingDeviceAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize;

            switch (comboBox_monitor_training_control.SelectedItem.ToString())
            {
                case "全站仪":
                    receivingDeviceAddress = MethodsClass.DeviceAddress.QuanZhanYi;
                    break;
                case "控显器":
                    receivingDeviceAddress = MethodsClass.DeviceAddress.KongXianQi;
                    break;
                case "陀螺仪":
                    receivingDeviceAddress = MethodsClass.DeviceAddress.TuoLuoYi;
                    break;
                case "激光测距机":
                    receivingDeviceAddress = MethodsClass.DeviceAddress.JiGuangCeJuJi;
                    break;
                case "北斗用户机":
                    receivingDeviceAddress = MethodsClass.DeviceAddress.BeiDou;
                    break;
                case "打印机":
                    receivingDeviceAddress = MethodsClass.DeviceAddress.DaYinJi;
                    break;
                case "定位定向系统":
                    receivingDeviceAddress = MethodsClass.DeviceAddress.DingWeiDingXiangXiTong;
                    break;
                case "驾驶模拟系统":
                    receivingDeviceAddress = MethodsClass.DeviceAddress.JiaShiMoNiXiTong;
                    break;
                default:
                    receivingDeviceAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize;
                    break;
            }

            byte[] targetBuffer = new byte[64];
            int targetBufferLength = 0;
            MethodsClass.PackingRequestUploadProcessData(MethodsClass.DeviceAddress.ShuJuJianKong, receivingDeviceAddress, MethodsClass.CommandCode.RequestUploadProcessData,
                                                         MethodsClass.RequestUploadProcessDataFlag.BeginUploadProcessData, ref targetBuffer, ref targetBufferLength);
            mainSerialPort.SerialSend(targetBuffer, 0, targetBufferLength);
        }

        /// <summary>
        /// 数据监控界面“停止监控”按钮点击事件响应函数
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">路由事件参数</param>
        private void Btn_monitor_uploadProcessData_stop_Click(object sender, RoutedEventArgs e)
        {
            MethodsClass.DeviceAddress receivingDeviceAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize;

            switch (comboBox_monitor_training_control.SelectedItem.ToString())
            {
                case "全站仪":
                    receivingDeviceAddress = MethodsClass.DeviceAddress.QuanZhanYi;
                    break;
                case "控显器":
                    receivingDeviceAddress = MethodsClass.DeviceAddress.KongXianQi;
                    break;
                case "陀螺仪":
                    receivingDeviceAddress = MethodsClass.DeviceAddress.TuoLuoYi;
                    break;
                case "激光测距机":
                    receivingDeviceAddress = MethodsClass.DeviceAddress.JiGuangCeJuJi;
                    break;
                case "北斗用户机":
                    receivingDeviceAddress = MethodsClass.DeviceAddress.BeiDou;
                    break;
                case "打印机":
                    receivingDeviceAddress = MethodsClass.DeviceAddress.DaYinJi;
                    break;
                case "定位定向系统":
                    receivingDeviceAddress = MethodsClass.DeviceAddress.DingWeiDingXiangXiTong;
                    break;
                case "驾驶模拟系统":
                    receivingDeviceAddress = MethodsClass.DeviceAddress.JiaShiMoNiXiTong;
                    break;
                default:
                    receivingDeviceAddress = MethodsClass.DeviceAddress.DeviceAddressForInitialize;
                    break;
            }

            byte[] targetBuffer = new byte[64];
            int targetBufferLength = 0;
            MethodsClass.PackingRequestUploadProcessData(MethodsClass.DeviceAddress.ShuJuJianKong, receivingDeviceAddress, MethodsClass.CommandCode.RequestUploadProcessData,
                                                         MethodsClass.RequestUploadProcessDataFlag.TerminateUploadProcessData, ref targetBuffer, ref targetBufferLength);
            mainSerialPort.SerialSend(targetBuffer, 0, targetBufferLength);
        }

        #endregion

        #region 数据分析界面配置及处理逻辑
        /// <summary>
        /// 配置数据分析界面中的各类汇总信息
        /// </summary>
        private void InitAnalyseInformation()
        {
            ShowCollectionOfBaseInformation();
            ShowCollectionOfSerialData();
            ShowCollectionOfUserInformation();
        }

        private PerformanceCounter gCpuPercentage = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        private PerformanceCounter gMemoryOccupied = new PerformanceCounter("Process", "Working Set", "BCL003炮兵测地车维修模拟训练器数据监控软件");

        /// <summary>
        /// 在数据分析界面中显示系统基本信息（如CPU占用率、内存消耗等）
        /// </summary>
        private void ShowCollectionOfBaseInformation()
        {
            string collectionOfBaseInformation = String.Empty;
            float currentMemoryOccupy = gMemoryOccupied.NextValue() / 1048576;
            collectionOfBaseInformation = String.Format("CPU占用率：[b][color=#000000]{0:F2} %[/color][/b]\r\n", gCpuPercentage.NextValue());
            collectionOfBaseInformation += String.Format("内存消耗：[b]{0:F2} MB[/b]\r\n", currentMemoryOccupy);
            bbcb_analyse_collectionOfBaseInformation.BBCode = collectionOfBaseInformation;

            Telerik.Charting.CategoricalDataPoint item = new Telerik.Charting.CategoricalDataPoint();
            item.Value = Convert.ToDouble(currentMemoryOccupy);
            chart_memoryAndCpuPerSecond.DataPoints.Add(item);
        }

        private long gSerialDataTransmittedQuantityLastSecond = 0, gSerialDataReceivedQuantityLastSecond = 0;
        /// <summary>
        /// 在数据分析界面中显示报文统计信息
        /// </summary>
        private void ShowCollectionOfSerialData()
        {
            string collectionOfSerialData = String.Empty;

            collectionOfSerialData += String.Format("导调软件共发送报文：[b]{0}[/b]\t\t接收报文：[b]{1}[/b]\r\n", gDevices[(int)GlobalDefinitions.DeviceIndex.DaoDiao].transmittedDataCounter, gDevices[(int)GlobalDefinitions.DeviceIndex.DaoDiao].receivedDataCounter);
            collectionOfSerialData += String.Format("联动软件共发送报文：[b]{0}[/b]\t\t接收报文：[b]{1}[/b]\r\n", gDevices[(int)GlobalDefinitions.DeviceIndex.LianDong].transmittedDataCounter, gDevices[(int)GlobalDefinitions.DeviceIndex.LianDong].receivedDataCounter);
            collectionOfSerialData += String.Format("桌面监控软件共发送报文：[b]{0}[/b]\t接收报文：[b]{1}[/b]\r\n", gDevices[(int)GlobalDefinitions.DeviceIndex.ZhuoMianJianKong].transmittedDataCounter, gDevices[(int)GlobalDefinitions.DeviceIndex.ZhuoMianJianKong].receivedDataCounter);
            collectionOfSerialData += String.Format("训练代理软件共发送报文：[b]{0}[/b]\t接收报文：[b]{1}[/b]\r\n", gDevices[(int)GlobalDefinitions.DeviceIndex.XunLianDaiLi].transmittedDataCounter, gDevices[(int)GlobalDefinitions.DeviceIndex.XunLianDaiLi].receivedDataCounter);
            collectionOfSerialData += String.Format("全站仪共发送报文：[b]{0}[/b]\t\t接收报文：[b]{1}[/b]\r\n", gDevices[(int)GlobalDefinitions.DeviceIndex.QuanZhanYi].transmittedDataCounter, gDevices[(int)GlobalDefinitions.DeviceIndex.QuanZhanYi].receivedDataCounter);
            collectionOfSerialData += String.Format("控显器共发送报文：[b]{0}[/b]\t\t接收报文：[b]{1}[/b]\r\n", gDevices[(int)GlobalDefinitions.DeviceIndex.KongXianQi].transmittedDataCounter, gDevices[(int)GlobalDefinitions.DeviceIndex.KongXianQi].receivedDataCounter);
            collectionOfSerialData += String.Format("陀螺仪共发送报文：[b]{0}[/b]\t\t接收报文：[b]{1}[/b]\r\n", gDevices[(int)GlobalDefinitions.DeviceIndex.TuoLuoYi].transmittedDataCounter, gDevices[(int)GlobalDefinitions.DeviceIndex.TuoLuoYi].receivedDataCounter);
            collectionOfSerialData += String.Format("激光测距机共发送报文：[b]{0}[/b]\t\t接收报文：[b]{1}[/b]\r\n", gDevices[(int)GlobalDefinitions.DeviceIndex.JiGuangCeJuJi].transmittedDataCounter, gDevices[(int)GlobalDefinitions.DeviceIndex.JiGuangCeJuJi].receivedDataCounter);
            collectionOfSerialData += String.Format("北斗用户机共发送报文：[b]{0}[/b]\t接收报文：[b]{1}[/b]\r\n", gDevices[(int)GlobalDefinitions.DeviceIndex.BeiDouYongHuJi].transmittedDataCounter, gDevices[(int)GlobalDefinitions.DeviceIndex.BeiDouYongHuJi].receivedDataCounter);
            collectionOfSerialData += String.Format("打印机共发送报文：[b]{0}[/b]\t\t接收报文：[b]{1}[/b]\r\n", gDevices[(int)GlobalDefinitions.DeviceIndex.DaYinJi].transmittedDataCounter, gDevices[(int)GlobalDefinitions.DeviceIndex.DaYinJi].receivedDataCounter);
            collectionOfSerialData += String.Format("定位定向系统共发送报文：[b]{0}[/b]\t接收报文：[b]{1}[/b]\r\n", gDevices[(int)GlobalDefinitions.DeviceIndex.DingWeiDingXiangXiTong].transmittedDataCounter, gDevices[(int)GlobalDefinitions.DeviceIndex.DingWeiDingXiangXiTong].receivedDataCounter);
            collectionOfSerialData += String.Format("驾驶模拟系统共发送报文：[b]{0}[/b]\t接收报文：[b]{1}[/b]\r\n", gDevices[(int)GlobalDefinitions.DeviceIndex.JiaShiMoNiXiTong].transmittedDataCounter, gDevices[(int)GlobalDefinitions.DeviceIndex.JiaShiMoNiXiTong].receivedDataCounter);

            bbcb_analyse_collectionOfSerialData.BBCode = collectionOfSerialData;

            //获取报文总发送数量
            long currentSerialDataTransmittedQuantity = 0, currentSerialDataReceivedQuantity = 0;
            foreach (GlobalDefinitions.Device device in gDevices)
            {
                currentSerialDataTransmittedQuantity += device.transmittedDataCounter;
            }
            long delta = currentSerialDataTransmittedQuantity - gSerialDataTransmittedQuantityLastSecond;
            gSerialDataTransmittedQuantityLastSecond = currentSerialDataTransmittedQuantity;

            Telerik.Charting.CategoricalDataPoint item = new Telerik.Charting.CategoricalDataPoint();
            item.Value = Convert.ToDouble(delta);
            chart_serialDataTransmittedQuantityPerSecond.DataPoints.Add(item);

            //获取报文总接收数量
            foreach (GlobalDefinitions.Device device in gDevices)
            {
                currentSerialDataReceivedQuantity += device.receivedDataCounter;
            }
            delta = currentSerialDataReceivedQuantity - gSerialDataReceivedQuantityLastSecond;
            gSerialDataReceivedQuantityLastSecond = currentSerialDataReceivedQuantity;

            item = new Telerik.Charting.CategoricalDataPoint();
            item.Value = Convert.ToDouble(delta);
            chart_serialDataReceivedQuantityPerSecond.DataPoints.Add(item);
        }

        /// <summary>
        /// 在数据分析界面中显示用户信息
        /// </summary>
        private void ShowCollectionOfUserInformation()
        {
            //TODO:Add function here...
        }

        /// <summary>
        /// 数据分析界面中“清空列表及曲线”按钮点击事件响应函数
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">RoutedEventArgs</param>
        private void Btn_analyse_clearShowData_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("是否需要清空列表及曲线？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                gAnalyzedReceivedPackageCountForClearListBox = 0;
                gAnalyzedReceivedPackageCountForClearChart = 0;
                ClearAnalyseChart();
                ClearAnalyseListBox();
            }
        }

        /// <summary>
        /// 清空数据分析界面中的Chart图表
        /// </summary>
        private void ClearAnalyseChart()
        {
            chart_memoryAndCpuPerSecond.DataPoints.Clear();
            chart_serialDataTransmittedQuantityPerSecond.DataPoints.Clear();
            chart_serialDataReceivedQuantityPerSecond.DataPoints.Clear();
        }

        /// <summary>
        /// 清空数据分析界面中的ListBox报文显示框
        /// </summary>
        private void ClearAnalyseListBox()
        {
            lb_analyse_showData.Items.Clear();
        }

        private long gAnalyzedReceivedPackageCountForClearListBox = 0, gAnalyzedReceivedPackageCountForClearChart = 0;
        /// <summary>
        /// 解析Command类型的数据包，并在数据分析界面中显示解析后的信息
        /// </summary>
        /// <param name="command">待解析的Command类型数据包</param>
        private void AnalyzingReceivedPackageAndShow(GlobalDefinitions.Command command)
        {
            //解析数据分析信息
            string information = String.Format("来报时间：[b]{0}[/b]\t发送成功：[b]是[/b]\t接收成功：[b]是[/b]\t报文发送席位：[b]{1}[/b] 报文接收席位：[b]{2}[/b] 报文类别：[b]{3}[/b]",
                                                command.commandReceivedTimestamp,
                                                command.transmittingDeviceAddressInChinese,
                                                command.receivingDeviceAddressInChinese,
                                                command.commandCodeInChinese);
            BBCodeBlock item = new BBCodeBlock();
            item.BBCode = information;
            lb_analyse_showData.Items.Add(item);
            lb_analyse_showData.ScrollIntoView(item);

            //数据超过500条时清除列表
            if (++gAnalyzedReceivedPackageCountForClearListBox >= 500)
            {
                gAnalyzedReceivedPackageCountForClearListBox = 0;
                ClearAnalyseListBox();


                //ClearAnalyseChart();
            }

            //数据超过4000条时清除图表
            if (++gAnalyzedReceivedPackageCountForClearChart >= 4000)
            {
                gAnalyzedReceivedPackageCountForClearChart = 0;
                //清除图表
                ClearAnalyseChart();
            }

            //统计来报信息
            switch (command.receivingDeviceAddressInChinese)
            {
                case "导调软件":
                    {
                        gDevices[(int)GlobalDefinitions.DeviceIndex.DaoDiao].receivedDataCounter++;
                    }
                    break;
                case "联动软件":
                    {
                        gDevices[(int)GlobalDefinitions.DeviceIndex.LianDong].receivedDataCounter++;
                    }
                    break;
                case "桌面监控软件":
                    {
                        gDevices[(int)GlobalDefinitions.DeviceIndex.ZhuoMianJianKong].receivedDataCounter++;
                    }
                    break;
                case "训练代理软件":
                    {
                        gDevices[(int)GlobalDefinitions.DeviceIndex.XunLianDaiLi].receivedDataCounter++;
                    }
                    break;
                case "全站仪":
                    {
                        gDevices[(int)GlobalDefinitions.DeviceIndex.QuanZhanYi].receivedDataCounter++;
                    }
                    break;
                case "控显器":
                    {
                        gDevices[(int)GlobalDefinitions.DeviceIndex.KongXianQi].receivedDataCounter++;
                    }
                    break;
                case "陀螺仪":
                    {
                        gDevices[(int)GlobalDefinitions.DeviceIndex.TuoLuoYi].receivedDataCounter++;
                    }
                    break;
                case "激光测距机":
                    {
                        gDevices[(int)GlobalDefinitions.DeviceIndex.JiGuangCeJuJi].receivedDataCounter++;
                    }
                    break;
                case "北斗用户机":
                    {
                        gDevices[(int)GlobalDefinitions.DeviceIndex.BeiDouYongHuJi].receivedDataCounter++;
                    }
                    break;
                case "打印机":
                    {
                        gDevices[(int)GlobalDefinitions.DeviceIndex.DaYinJi].receivedDataCounter++;
                    }
                    break;
                case "定位定向系统":
                    {
                        gDevices[(int)GlobalDefinitions.DeviceIndex.DingWeiDingXiangXiTong].receivedDataCounter++;
                    }
                    break;
                case "驾驶模拟系统":
                    {
                        gDevices[(int)GlobalDefinitions.DeviceIndex.JiaShiMoNiXiTong].receivedDataCounter++;
                    }
                    break;
                default:
                    break;
            }

            //统计发报信息
            switch (command.transmittingDeviceAddressInChinese)
            {
                case "导调软件":
                    {
                        gDevices[(int)GlobalDefinitions.DeviceIndex.DaoDiao].transmittedDataCounter++;
                    }
                    break;
                case "联动软件":
                    {
                        gDevices[(int)GlobalDefinitions.DeviceIndex.LianDong].transmittedDataCounter++;
                    }
                    break;
                case "桌面监控软件":
                    {
                        gDevices[(int)GlobalDefinitions.DeviceIndex.ZhuoMianJianKong].transmittedDataCounter++;
                    }
                    break;
                case "训练代理软件":
                    {
                        gDevices[(int)GlobalDefinitions.DeviceIndex.XunLianDaiLi].transmittedDataCounter++;
                    }
                    break;
                case "全站仪":
                    {
                        gDevices[(int)GlobalDefinitions.DeviceIndex.QuanZhanYi].transmittedDataCounter++;
                    }
                    break;
                case "控显器":
                    {
                        gDevices[(int)GlobalDefinitions.DeviceIndex.KongXianQi].transmittedDataCounter++;
                    }
                    break;
                case "陀螺仪":
                    {
                        gDevices[(int)GlobalDefinitions.DeviceIndex.TuoLuoYi].transmittedDataCounter++;
                    }
                    break;
                case "激光测距机":
                    {
                        gDevices[(int)GlobalDefinitions.DeviceIndex.JiGuangCeJuJi].transmittedDataCounter++;
                    }
                    break;
                case "北斗用户机":
                    {
                        gDevices[(int)GlobalDefinitions.DeviceIndex.BeiDouYongHuJi].transmittedDataCounter++;
                    }
                    break;
                case "打印机":
                    {
                        gDevices[(int)GlobalDefinitions.DeviceIndex.DaYinJi].transmittedDataCounter++;
                    }
                    break;
                case "定位定向系统":
                    {
                        gDevices[(int)GlobalDefinitions.DeviceIndex.DingWeiDingXiangXiTong].transmittedDataCounter++;
                    }
                    break;
                case "驾驶模拟系统":
                    {
                        gDevices[(int)GlobalDefinitions.DeviceIndex.JiaShiMoNiXiTong].transmittedDataCounter++;
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion//数据分析界面配置及处理逻辑

        #region 数据查询界面配置及处理逻辑
        private PageQueryByCommandTimeStamp gPageQueryByCommandTimeStamp;
        private PageQueryByDate gPageQueryByDate;
        private PageQueryByTransmittingDeviceAddress gPageQueryByTransmittingDeviceAddress;
        private PageQueryByReceivingDeviceAddress gPageQueryByReceivingDeviceAddress;

        /// <summary>
        /// 数据查询界面中“按发送方地址查询”按钮点击事件响应函数
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">路由事件参数</param>
        private void Radio_query_by_transmiting_device_address_Click(object sender, RoutedEventArgs e)
        {
            if (gPageQueryByTransmittingDeviceAddress == null)
            {
                gPageQueryByTransmittingDeviceAddress = new PageQueryByTransmittingDeviceAddress();
            }
            contentControl_data_query_choose_ways.Content = new Frame()
            {
                Content = gPageQueryByTransmittingDeviceAddress
            };
        }

        /// <summary>
        /// 数据查询界面中“按接收方地址查询”按钮点击事件响应函数
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">路由事件参数</param>
        private void Radio_query_by_receiving_device_address_Click(object sender, RoutedEventArgs e)
        {
            if (gPageQueryByReceivingDeviceAddress == null)
            {
                gPageQueryByReceivingDeviceAddress = new PageQueryByReceivingDeviceAddress();
            }
            contentControl_data_query_choose_ways.Content = new Frame()
            {
                Content = gPageQueryByReceivingDeviceAddress
            };
        }

        /// <summary>
        /// 数据查询界面中“按命令时间查询”按钮点击事件响应函数
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">路由事件参数</param>
        private void Radio_query_by_commandTimeStamp_Click(object sender, RoutedEventArgs e)
        {
            if (gPageQueryByCommandTimeStamp == null)
            {
                gPageQueryByCommandTimeStamp = new PageQueryByCommandTimeStamp();
            }
            contentControl_data_query_choose_ways.Content = new Frame()
            {
                Content = gPageQueryByCommandTimeStamp
            };
        }

        /// <summary>
        /// 数据查询界面中“按日期查询”按钮点击事件响应函数
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">路由事件参数</param>
        private void Radio_query_by_date_Click(object sender, RoutedEventArgs e)
        {
            if (gPageQueryByDate == null)
            {
                gPageQueryByDate = new PageQueryByDate();
            }
            contentControl_data_query_choose_ways.Content = new Frame()
            {
                Content = gPageQueryByDate
            };
        }

        private GlobalDefinitions.QueryDatabaseEventArgs.QueryDatabaseCondition gCurrentQueryDatabaseCondition = GlobalDefinitions.QueryDatabaseEventArgs.QueryDatabaseCondition.ByTransmittingDeviceAddress;
        private string gCurrentQueryResult = string.Empty;

        /// <summary>
        /// 按选定的查询方式在各数据查询界面中完成相关信息填写并点击“确定查询”按钮后的回调函数，用于完成事件响应
        /// </summary>
        /// <param name="e">QueryDatabaseEventArgs事件参数</param>
        private void ExecuteQueryResult(GlobalDefinitions.QueryDatabaseEventArgs e)
        {
            BBCodeBlock selectedDatabase = lb_query_databaseFileName.SelectedItem as BBCodeBlock;
            if (selectedDatabase == null)
            {
                MessageBox.Show("请在“数据库文件”栏选择一个数据库文件", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                if (File.Exists(selectedDatabase.Tag as string))
                {
                    DoRedraw(true);
                    BackgroundWorker doSearchWorker = new BackgroundWorker();
                    doSearchWorker.DoWork += DoSearchWorker_DoWork;
                    doSearchWorker.RunWorkerCompleted += DoSearchWorker_RunWorkerCompleted;

                    var args = new object[3];
                    args[0] = e;
                    args[1] = selectedDatabase.Tag as string;
                    args[2] = selectedDatabase.BBCode;
                    doSearchWorker.RunWorkerAsync(args);
                }
                else
                {
                    MessageBox.Show("数据库文件不存在！文件是否被删除/移动/重命名？\r\n请点击“刷新数据库文件夹信息”以获取数据库文件夹内的最新文件信息", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            /*BBCodeBlock selectedDatabase = lb_query_databaseFileName.SelectedItem as BBCodeBlock;
            if (selectedDatabase == null)
            {
                MessageBox.Show("请在“数据库文件”栏选择一个数据库文件", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                string databasePath = selectedDatabase.Tag as string, databaseName = selectedDatabase.BBCode;
                databasePath = Path.GetDirectoryName(databasePath);
                currentQueryDatabaseCondition = e.queryDatabaseContidion;
                currentQueryResult = e.queryResult;
                switch (currentQueryDatabaseCondition)
                {
                    case GlobalDefinitions.QueryDatabaseEventArgs.QueryDatabaseCondition.ByTransmittingDeviceAddress:
                        {
                            List<GlobalDefinitions.Command> result = new List<GlobalDefinitions.Command>();
                            AccessHelper.SetDatabasePath(databasePath, databaseName);
                            AccessHelper.ReadDatabaseByKeyword(AccessHelper.SortType.transmittingDeviceAddressInChinese, currentQueryResult, ref result);
                        }
                        break;
                    case GlobalDefinitions.QueryDatabaseEventArgs.QueryDatabaseCondition.ByReceivingDeviceAddress:
                        break;
                    case GlobalDefinitions.QueryDatabaseEventArgs.QueryDatabaseCondition.ByCommandTimeStamp:
                        {
                            //20191205 00:00:00;20191206 09:00:00
                        }
                        break;
                    case GlobalDefinitions.QueryDatabaseEventArgs.QueryDatabaseCondition.ByDate:
                        {
                            //20191204;20191207
                        }
                        break;
                    default:
                        break;
                }
            }*/
        }

        /// <summary>
        /// 重绘数据查询界面
        /// </summary>
        /// <param name="isBeforeSearch">=true，则在每次搜索前重置数据查询界面相关状态（如相关数据绑定、清空数据源、显示最上层蒙版等）；=false则在搜索结束后，重置界面相关状态</param>
        private void DoRedraw(bool isBeforeSearch)
        {
            if (isBeforeSearch)
            {
                //在每次搜索前，重置界面相关状态
                lv_query_showQueryResult.ItemsSource = null;
                gShowQueryResultListViewItemSource.Clear();
                ShowMaskForExecution(true);
            }
            else
            {
                //搜索结束后，重置界面相关状态
                ShowMaskForExecution(false);
            }
        }

        /// <summary>
        /// 显示或者取消显示数据查询界面的最上层蒙版（一般在开始执行数据查询操作时开启蒙版，在完成数据查询操作时关闭蒙版）
        /// </summary>
        /// <param name="isShowed">=true，则显示数据查询界面的最上层蒙版；=false，则不显示数据查询界面的最上层蒙版</param>
        private void ShowMaskForExecution(bool isShowed)
        {
            stack_query_maskForExecution.Visibility = isShowed ? Visibility.Visible : Visibility.Hidden;
        }

        /// <summary>
        /// 开始查询后台工人RunWorkerCompleted事件响应函数
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">RunWorkerCompleted事件参数</param>
        private void DoSearchWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lv_query_showQueryResult.ItemsSource = gShowQueryResultListViewItemSource;
            DoRedraw(false);

            //除“数据包内容”列外（因为其内容很长，自动设置后会导致超出窗体很多），自动设置列宽为列中的最大长度的数据宽度
            foreach (GridViewColumn column in lvgv_query_showQueryResult.Columns)
            {
                if (!column.Header.Equals("数据包内容"))
                {
                    column.Width = Double.NaN;
                }
            }

            //弹窗显示搜索结果
            if (gShowQueryResultListViewItemSource != null)
            {
                MessageBox.Show(String.Format("为您找到相关结果{0}个", gShowQueryResultListViewItemSource.Count), "搜索结果", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("未搜索到相关数据！", "搜索结果", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        List<GlobalDefinitions.Command> gShowQueryResultListViewItemSource = new List<GlobalDefinitions.Command>();

        /// <summary>
        /// 开始查询后台工人DoWork事件响应函数
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">DoWork事件参数</param>
        private void DoSearchWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var args = e.Argument as object[];
            GlobalDefinitions.QueryDatabaseEventArgs queryDatabaseEventArgs = (GlobalDefinitions.QueryDatabaseEventArgs)args[0];
            string databasePath = args[1] as string, databaseName = args[2] as string;
            databasePath = Path.GetDirectoryName(databasePath);
            gCurrentQueryDatabaseCondition = queryDatabaseEventArgs.queryDatabaseContidion;
            gCurrentQueryResult = queryDatabaseEventArgs.queryResult;
            switch (gCurrentQueryDatabaseCondition)
            {
                case GlobalDefinitions.QueryDatabaseEventArgs.QueryDatabaseCondition.ByTransmittingDeviceAddress:
                    {
                        AccessHelper.SetDatabasePath(databasePath, databaseName);
                        AccessHelper.ReadDatabaseByKeyword(AccessHelper.SortType.transmittingDeviceAddressInChinese, gCurrentQueryResult, ref gShowQueryResultListViewItemSource);
                    }
                    break;
                case GlobalDefinitions.QueryDatabaseEventArgs.QueryDatabaseCondition.ByReceivingDeviceAddress:
                    {
                        AccessHelper.SetDatabasePath(databasePath, databaseName);
                        AccessHelper.ReadDatabaseByKeyword(AccessHelper.SortType.receivingDeviceAddressInChinese, gCurrentQueryResult, ref gShowQueryResultListViewItemSource);
                    }
                    break;
                case GlobalDefinitions.QueryDatabaseEventArgs.QueryDatabaseCondition.ByCommandTimeStamp:
                    {
                        //搜索格式如：20191205 00:00:00;20191206 09:00:00
                        AccessHelper.SetDatabasePath(databasePath, databaseName);
                        string dateBegin = String.Empty, dateEnd = String.Empty, timeBegin = String.Empty, timeEnd = String.Empty;
                        string[] timestamps = gCurrentQueryResult.Split(';');
                        if (timestamps.Length == 2)
                        {
                            for (int loop = 0; loop < timestamps.Length; loop++)
                            {
                                string[] temp = timestamps[loop].Split(' ');
                                if (temp.Length == 2)
                                {
                                    if (loop == 0)
                                    {
                                        dateBegin = temp[0];
                                        timeBegin = temp[1];
                                    }
                                    else
                                    {
                                        dateEnd = temp[0];
                                        timeEnd = temp[1];
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("日期时间格式错误，未搜索到相关数据！", "搜索结果", MessageBoxButton.OK, MessageBoxImage.Information);
                                    break;
                                }
                            }
                            string[] allValidDateTime = GlobalDefinitions.QueryDateTime(false, dateBegin, dateEnd, timeBegin, timeEnd);
                            if (allValidDateTime != null)
                            {
                                foreach (string result in allValidDateTime)
                                {
                                    AccessHelper.ReadDatabaseByKeyword(AccessHelper.SortType.commandReceivedTimestamp, result, ref gShowQueryResultListViewItemSource);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("日期时间格式错误，未搜索到相关数据！", "搜索结果", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    break;
                case GlobalDefinitions.QueryDatabaseEventArgs.QueryDatabaseCondition.ByDate:
                    {
                        //搜索格式如：20191204;20191207
                        AccessHelper.SetDatabasePath(databasePath, databaseName);
                        string dateBegin = String.Empty, dateEnd = String.Empty;
                        string[] timestamps = gCurrentQueryResult.Split(';');
                        if (timestamps.Length == 2)
                        {
                            dateBegin = timestamps[0];
                            dateEnd = timestamps[1];

                            string[] allValidDate = GlobalDefinitions.QueryDateTime(true, dateBegin, dateEnd, "", "");

                            if (allValidDate != null)
                            {
                                foreach (string result in allValidDate)
                                {
                                    AccessHelper.ReadDatabaseByKeyword(AccessHelper.SortType.commandReceivedTimestamp, result, ref gShowQueryResultListViewItemSource);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("日期格式错误，未搜索到相关数据！", "搜索结果", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 数据查询界面中“定位数据库文件夹位置”按钮点击事件响应函数
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">路由事件参数</param>
        private void Btn_query_locateDatabaseFolder_Click(object sender, RoutedEventArgs e)
        {
            string question = String.Format("默认数据库文件夹位置位于：{0}\r\n需要重新定位文件夹位置吗？", gDatabaseFolderPath);
            MessageBoxResult result = MessageBox.Show(question, "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel);
            if (result == MessageBoxResult.OK)
            {
                /*  
                 * Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.DefaultExt = ".xml";
                ofd.Filter = "xml file|*.xml";
                if (ofd.ShowDialog() == true)
                {
                    //此处做你想做的事 ...=ofd.FileName; 
                }
                */
                WinForm.FolderBrowserDialog folderBrowserDialog = new WinForm.FolderBrowserDialog();
                folderBrowserDialog.Description = "选择数据库文件夹位置";
                folderBrowserDialog.ShowNewFolderButton = false;
                //folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
                folderBrowserDialog.SelectedPath = gDatabaseFolderPath;
                folderBrowserDialog.ShowDialog();
                if (!String.IsNullOrEmpty(folderBrowserDialog.SelectedPath))
                {
                    gDatabaseFolderPath = folderBrowserDialog.SelectedPath;
                    ShowDatabaseFileName(gDatabaseFolderPath);
                }
            }
        }

        /// <summary>
        /// 数据查询界面中“刷新数据库文件夹信息”按钮点击事件响应函数
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">路由事件参数</param>
        private void Btn_query_refreshFilesInDatabaseFolder_Click(object sender, RoutedEventArgs e)
        {
            ShowDatabaseFileName(gDatabaseFolderPath);
        }
        #endregion//数据查询界面配置及处理逻辑

        #region 其他界面配置及处理逻辑
        /// <summary>
        /// 其他界面中“是否开机启动”按钮点击事件响应函数
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">路由事件参数</param>
        private void Cb_other_powerbootEnabled_Click(object sender, RoutedEventArgs e)
        {
            bool needPowerboot = (bool)cb_other_powerbootEnabled.IsChecked;
            if (needPowerboot)
            {
                IniFileOperations.WriteIniData("STATUS", "PowerbootEnable", "1", gConfigIniFilePath);
            }
            else
            {
                IniFileOperations.WriteIniData("STATUS", "PowerbootEnable", "0", gConfigIniFilePath);
            }
            SetPowerbootStatus(needPowerboot);
        }

        /// <summary>
        /// 其他界面中“关于本软件”按钮点击事件响应函数
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">路由事件参数</param>
        private void BtnCopyright_Click(object sender, RoutedEventArgs e)
        {
            CopyRight copyRightDlg = new CopyRight("版本及版权信息", GlobalDefinitions.AppInformation + GlobalDefinitions.MSPLCopyRight);
            copyRightDlg.ShowDialog();
            copyRightDlg.Activate();
        }

        /// <summary>
        /// 其他界面中“界面置顶显示”按钮点击事件响应函数
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">路由事件参数</param>
        private void Cb_other_topmost_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = (Convert.ToBoolean(cb_other_topmost.IsChecked) ? true : false);
        }

        /// <summary>
        /// 其他界面中“重置界面位置”按钮点击事件响应函数
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">路由事件参数</param>
        private void Btn_other_resetAppPosition_Click(object sender, RoutedEventArgs e)
        {
            this.Top = 0;
            this.Left = 0;
        }

        /// <summary>
        /// 其他界面中“界面可移动”按钮点击事件响应函数
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">路由事件参数</param>
        private void Cb_other_appCanMove_Click(object sender, RoutedEventArgs e)
        {
            currentKeyStatus.appCanMove = (Convert.ToBoolean(cb_other_appCanMove.IsChecked) ? true : false);
        }


        #endregion//其他界面配置及处理逻辑

    }
}
