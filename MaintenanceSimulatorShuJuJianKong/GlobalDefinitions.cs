using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MaintenanceSimulatorShuJuJianKong;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using MSGeneralMethods;
using System.Text.RegularExpressions;
using System.Globalization;

namespace MaintenanceSimulatorShuJuJianKong
{
    public class GlobalDefinitions
    {
        public static string[] deviceNames = { "导调软件", "联动软件", "桌面监控软件", "数据监控软件", "训练代理软件", "全站仪", "控显器", "陀螺仪", "激光测距机", "北斗用户机", "打印机", "定位定向系统", "驾驶模拟系统" };
        public enum DeviceIndex
        {
            DaoDiao,
            LianDong,
            ZhuoMianJianKong,
            ShuJuJianKong,
            XunLianDaiLi,
            QuanZhanYi,
            KongXianQi,
            TuoLuoYi,
            JiGuangCeJuJi,
            BeiDouYongHuJi,
            DaYinJi,
            DingWeiDingXiangXiTong,
            JiaShiMoNiXiTong
        };

        public class Command
        {
            public string commandReceivedTimestamp { get; set; }
            public string transmittingDeviceAddress { get; set; }
            public string transmittingDeviceAddressInChinese { get; set; }
            public string receivingDeviceAddress { get; set; }
            public string receivingDeviceAddressInChinese { get; set; }
            public string commandCode { get; set; }
            public string commandCodeInChinese { get; set; }
            public string commandBuffer { get; set; }

            ///   <summary>   
            ///   创建命令类，一个命令类的实例包含时间戳，发送方地址，发送方地址（中文释义），接收方地址，接收方地址（中文释义），命令码，命令码（中文释义），数据包内容。
            ///   </summary>   
            ///   <param   name="commandReceivedTimestamp">时间戳，表明数据监控软件接收到此条报文的时刻</param>   
            ///   <param   name="transmittingDeviceAddress">发送方地址（十六进制）</param>   
            ///   <param   name="transmittingDeviceAddressInChinese">发送方地址（中文释义）</param>   
            ///   <param   name="receivingDeviceAddress">接收方地址（十六进制）</param>   
            ///   <param   name="receivingDeviceAddressInChinese">接收方地址（中文释义）</param>   
            ///   <param   name="commandCode">命令码（十六进制）</param>   
            ///   <param   name="commandCodeInChinese">命令码（中文释义）</param>   
            ///   <param   name="commandBuffer">数据包内容（十六进制）</param> 
            ///   <returns>命令类</returns>
            public Command(string commandReceivedTimestamp, string transmittingDeviceAddress, string transmittingDeviceAddressInChinese,
                            string receivingDeviceAddress, string receivingDeviceAddressInChinese,
                            string commandCode, string commandCodeInChinese, string commandBuffer)
            {
                this.commandReceivedTimestamp = commandReceivedTimestamp;
                this.transmittingDeviceAddress = transmittingDeviceAddress;
                this.transmittingDeviceAddressInChinese = transmittingDeviceAddressInChinese;
                this.receivingDeviceAddress = receivingDeviceAddress;
                this.receivingDeviceAddressInChinese = receivingDeviceAddressInChinese;
                this.commandCode = commandCode;
                this.commandCodeInChinese = commandCodeInChinese;
                this.commandBuffer = commandBuffer;
            }

            public Command(string commandReceivedTimestamp, MethodsClass.DeviceAddress transmittingDeviceAddress, string transmittingDeviceAddressInChinese,
                            MethodsClass.DeviceAddress receivingDeviceAddress, string receivingDeviceAddressInChinese,
                            MethodsClass.CommandCode commandCode, string commandCodeInChinese, string commandBuffer)
            {
                this.commandReceivedTimestamp = commandReceivedTimestamp;
                this.transmittingDeviceAddress = String.Format("0x{0:X}", transmittingDeviceAddress);
                this.transmittingDeviceAddressInChinese = transmittingDeviceAddressInChinese;
                this.receivingDeviceAddress = String.Format("0x{0:X}", receivingDeviceAddress);
                this.receivingDeviceAddressInChinese = receivingDeviceAddressInChinese;
                this.commandCode = String.Format("0x{0:X}", commandCode);
                this.commandCodeInChinese = commandCodeInChinese;
                this.commandBuffer = commandBuffer;
            }
        }

        ///   <summary>   
        ///   程序关键状态，这些状态错误时将影响程序运行及功能
        ///   </summary>   
        ///   <param   name="isDatabaseReady">数据库是否准备好</param>   
        ///   <param   name="isMainSerialPortOpen">主串口是否开启</param>
        ///   <returns>程序关键状态类</returns>        
        public class AppKeyStatus
        {
            public bool isDatabaseReady;
            public bool isMainSerialPortOpen;
            public bool isPowerbootEnabled;
            public bool appCanMove;
        }

        public class DateTimeHelper
        {
            /// <summary>
            /// 设置系统的年月日
            /// </summary>
            /// <param name="year">年</param>
            /// <param name="month">月</param>
            /// <param name="day">日</param>
            public static void SetLocalDate(int year, int month, int day)
            {
                //实例一个Process类，启动一个独立进程 
                Process p = new Process();
                //Process类有一个StartInfo属性 
                //设定程序名 
                p.StartInfo.FileName = "cmd.exe";
                //设定程式执行参数 “/C”表示执行完命令后马上退出
                p.StartInfo.Arguments = string.Format("/c date {0}-{1}-{2}", year, month, day);
                //关闭Shell的使用 
                p.StartInfo.UseShellExecute = false;
                //重定向标准输入 
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                //重定向错误输出 
                p.StartInfo.RedirectStandardError = true;
                //设置不显示doc窗口 
                p.StartInfo.CreateNoWindow = true;
                //启动 
                p.Start();
                //从输出流取得命令执行结果 
                p.StandardOutput.ReadToEnd();
            }

            /// <summary>
            /// 设置系统的时分秒
            /// </summary>
            /// <param name="hour">时</param>
            /// <param name="min">分</param>
            /// <param name="sec">秒</param>
            public static void SetLocalTime(int hour, int min, int sec)
            {
                //实例一个Process类，启动一个独立进程 
                Process p = new Process();
                //Process类有一个StartInfo属性 
                //设定程序名 
                p.StartInfo.FileName = "cmd.exe";
                //设定程式执行参数 “/C”表示执行完命令后马上退出
                p.StartInfo.Arguments = string.Format("/c time {0}:{1}:{2}", hour, min, sec);
                //关闭Shell的使用 
                p.StartInfo.UseShellExecute = false;
                //重定向标准输入 
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                //重定向错误输出 
                p.StartInfo.RedirectStandardError = true;
                //设置不显示doc窗口 
                p.StartInfo.CreateNoWindow = true;
                //启动 
                p.Start();
                //从输出流取得命令执行结果 
                p.StandardOutput.ReadToEnd();
            }

            /// <summary>
            /// 设置系统的年月日和时分秒
            /// </summary>
            /// <param name="time">DataTime类</param>
            public static void SetLocalDateTime(DateTime time)
            {
                SetLocalDate(time.Year, time.Month, time.Day);
                SetLocalTime(time.Hour, time.Minute, time.Second);
            }
        }

        ///   <summary>   
        ///   设备被监控状态
        ///   </summary>   
        ///   <param   name="BeginMonitoring">开始监控</param>   
        ///   <param   name="StopMonitoring">停止监控</param>   
        ///   <param   name="PauseMonitoring">暂停监控</param>
        ///   <param   name="ContinueMonitoring">继续监控</param>
        ///   <returns>设备被监控状态枚举</returns>  
        public enum DeviceMonitoringStatus
        {
            BeginMonitoring,
            StopMonitoring,
            PauseMonitoring,
            ContinueMonitoring
        };

        ///   <summary>   
        ///   设备类，一个设备类的实例包含枚举类型设备被监控状态以及设备索引号
        ///   </summary>   
        ///   <param   name="monitoringStatus">DeviceMonitoringStatus枚举类型变量，表示设备当前的被监控的状态</param>   
        ///   <param   name="deviceIndex">设备索引号</param>
        ///   <returns>设备类</returns>  
        public class Device
        {
            public DeviceMonitoringStatus monitoringStatus;
            public DeviceIndex deviceIndex;
            public long transmittedDataCounter;
            public long receivedDataCounter;
        }
        
        public delegate void QueryDatabaseHandler(QueryDatabaseEventArgs e);
        public static event QueryDatabaseHandler QueryResult;
        public class QueryDatabaseEventArgs : EventArgs
        {
            public enum QueryDatabaseCondition
            {
                ByTransmittingDeviceAddress,
                ByReceivingDeviceAddress, 
                ByCommandTimeStamp,
                ByDate
            };

            public QueryDatabaseCondition queryDatabaseContidion = QueryDatabaseCondition.ByTransmittingDeviceAddress;
            public string queryResult = string.Empty;

            public QueryDatabaseEventArgs(QueryDatabaseCondition queryDatabaseCondition, string queryResult)
            {
                this.queryDatabaseContidion = queryDatabaseCondition;
                this.queryResult = queryResult;
            }
        }

        ///   <summary>   
        ///   在查询条件确定后向主窗口更新数据查询结果
        ///   </summary>   
        ///   <param   name="queryDatabaseCondition">QueryDatabaseEventArgs.QueryDatabaseCondition枚举类型变量，表示当前查询方式</param>   
        ///   <param   name="queryResult">查询条件</param>
        ///   <returns>空</returns>  
        public static void UpdateQueryResult(QueryDatabaseEventArgs.QueryDatabaseCondition queryDatabaseCondition, string queryResult)
        {
            QueryDatabaseEventArgs args = new QueryDatabaseEventArgs(queryDatabaseCondition, queryResult);
            QueryResult(args);
        }

        public struct DatabaseFile
        {
            public string path;
            public string name;
            public string information;
        }

        public static string AppInformation =
            "BCL003炮兵测地车维修模拟训练器数据监控软件\r\n" +
            "V1.00.01 Build20200317\r\n" +
            "Copyright © 2018-现在，江西中船航海仪器有限公司\r\n" +
            "程序及UI设计：毕挺\r\n\r\n";
        public static string MSPLCopyRight =
        "[b]Microsoft Public License (Ms-PL)[/b][/color]\r\n" +
        "This license governs use of the accompanying software. If you use the software, you " +
        "accept this license. If you do not accept the license, do not use the software.\r\n" +
        "[b]1. Definitions[/b]\r\n" +
        "The terms [i]\"reproduce,\"[/i] [i]\"reproduction,\"[/i] [i]\"derivative works,\"[/i] and [i]\"distribution\"[/i] " +
        "have the same meaning here as under U.S. copyright law." +
        "A [i]\"contribution\"[/i] is the original software, or any additions or changes to the software." +
        "A [i]\"contributor\"[/i] is any person that distributes its contribution under this license." +
        "[i]\"Licensed patents\"[/i] are a contributor's patent claims that read directly on its contribution.\r\n" +
        "[b]2. Grant of Rights[/b]\r\n" +
        "[b](A)[/b] Copyright Grant- Subject to the terms of this license, including the license conditions " +
        "and limitations in section 3, each contributor grants you a non-exclusive, worldwide, " +
        "royalty-free copyright license to reproduce its contribution, prepare derivative works of " +
        "its contribution, and distribute its contribution or any derivative works that you create.\r\n" +
        "[b](B)[/b] Patent Grant- Subject to the terms of this license, including the license conditions " +
        "and limitations in section 3, each contributor grants you a non-exclusive, worldwide, " +
        "royalty-free license under its licensed patents to make, have made, use, sell, offer for " +
        "sale, import, and/or otherwise dispose of its contribution in the software or derivative works " +
        "of the contribution in the software.\r\n" +
        "[b]3. Conditions and Limitations[/b]\r\n" +
        "[b](A)[/b] No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.\r\n" +
        "[b](B)[/b] If you bring a patent claim against any contributor over patents that you claim are " +
        "infringed by the software, your patent license from such contributor to the software ends automatically.\r\n" +
        "[b](C)[/b] If you distribute any portion of the software, you must retain all copyright, patent, " +
        "trademark, and attribution notices that are present in the software.\r\n" +
        "[b](D)[/b] If you distribute any portion of the software in source code form, you may do so only " +
        "under this license by including a complete copy of this license with your distribution. If you " +
        "distribute any portion of the \r\nsoftware in compiled or object code form, you may only do so " +
        "under a license that complies with this license.\r\n" +
        "[b](E)[/b] The software is licensed [i]\" as-is.\"[/i] You bear the risk of using it. The contributors " +
        "give no express warranties, guarantees or conditions. You may have additional consumer rights " +
        "under your local laws which this \r\nlicense cannot change. To the extent permitted under your " +
        "local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular " +
        "purpose and non-infringement.\r\n" +
        "\r\n" +
        "[b]开源库[/b]:\r\n" +
        "FirstFloor.ModernUI\r\nPanuon.UI";

        //坐标的三种格式的结构体定义
        public struct Geodetic
        {
            public double inRad;
            public double inDegree;
            public double inNormalFormat;
        }

        //地理坐标结构体定义
        public struct GeodeticCoordinate
        {
            public Geodetic latitude, longitude, altitude;
            public GeodeticCoordinate(double latitudeInRad, double longitudeInRad, double altitude)
            {
                this.latitude.inRad = latitudeInRad;
                this.latitude.inDegree = RadToDeg(latitudeInRad);
                this.latitude.inNormalFormat = latitudeInRad;

                this.longitude.inRad = longitudeInRad;
                this.longitude.inDegree = RadToDeg(longitudeInRad);
                this.longitude.inNormalFormat = longitudeInRad;

                this.altitude.inRad = altitude;
                this.altitude.inDegree = altitude;
                this.altitude.inNormalFormat = altitude;
            }
        }

        //高斯坐标结构体定义
        public struct GaussCoordinate
        {
            public double x, y, h;
            public GaussCoordinate(double x, double y, double h)
            {
                this.x = x;
                this.y = y;
                this.h = h;
            }
        }

        //已选择的串口结构体定义
        public struct UartParams
        {
            public byte selComNum;

            public UartParams(byte selComNumToInit)
            {
                this.selComNum = selComNumToInit;
            }
        };


        //功能：共用体FloatUnion定义
        //By the way: 还可以用BitConverter类来转换     
        //记得要引用System.Runtime.InteropServices;
        [StructLayout(LayoutKind.Explicit, Size = 4)]
        public struct FloatUnion
        {
            [FieldOffset(0)]
            public byte DataInEightBit0;
            [FieldOffset(1)]
            public byte DataInEightBit1;
            [FieldOffset(2)]
            public byte DataInEightBit2;
            [FieldOffset(3)]
            public byte DataInEightBit3;
            [FieldOffset(0)]
            public float DataInFloat;
        }

        [StructLayout(LayoutKind.Explicit, Size = 4)]
        public struct IntUnion
        {
            [FieldOffset(0)]
            public byte DataInEightBit0;
            [FieldOffset(1)]
            public byte DataInEightBit1;
            [FieldOffset(2)]
            public byte DataInEightBit2;
            [FieldOffset(3)]
            public byte DataInEightBit3;
            [FieldOffset(0)]
            public int DataInInt;
        }

        //共用体DoubleUnion定义        
        [StructLayout(LayoutKind.Explicit, Size = 8)]
        public struct DoubleUnion
        {
            [FieldOffset(0)]
            public byte DataInEightBit0;
            [FieldOffset(1)]
            public byte DataInEightBit1;
            [FieldOffset(2)]
            public byte DataInEightBit2;
            [FieldOffset(3)]
            public byte DataInEightBit3;
            [FieldOffset(4)]
            public byte DataInEightBit4;
            [FieldOffset(5)]
            public byte DataInEightBit5;
            [FieldOffset(6)]
            public byte DataInEightBit6;
            [FieldOffset(7)]
            public byte DataInEightBit7;
            [FieldOffset(0)]
            public double DataInDouble;
        }

        //测地车定位定向系统数据结构体定义
        public struct DwdxDataStruct
        {
	        public double dCoordinateX;
            public double dCoordinateY;
            public double dCoordinateH;
            public double dDirection;
            public double dLatitude;
            public double dLongitude;
            public double dPitch;
            public double dRoll;
            public int iZeroVelocityCountDown;
            public byte status;
            public byte errorCode1;
            public byte errorCode2;
            public byte minute;
            public byte second;
        };

        //数据监控的过滤显示结构体定义
        public struct DeviceInformationFilter
        {
            public bool showInformationOfDaoDiaoRuanJian;
            public bool showInformationOfLianDongRuanJian;
            public bool showInformationOfZhuoMianJianKongRuanJian;
            public bool showInformationOfShuJuJianKongRuanJian;
            public bool showInformationOfXunLianDaiLiRuanJian;
            public bool showInformationOfJiaShiMoNiXiTong;
            public bool showInformationOfQuanZhanYi;
            public bool showInformationOfKongXianQi;
            public bool showInformationOfTuoLuoYi;
            public bool showInformationOfJiGuangCeJuJi;
            public bool showInformationOfBeiDouYongHuJi;
            public bool showInformationOfDaYinJi;
            public bool showInformationOfDingWeiDingXiangXiTong;
        }

        public const double PI = 3.1415926535897932384626433832795;
        //54坐标系参数
        /*public const double f = 1 / 298.3;
        public const double RE = 6378245.0;
        public const double RP = 6356863.01877;
        //public const double e1 = Math.Sqrt((RE * RE - RP * RP) / (RP * RP));
        public const double e1 = 0.0820885218264324998319658187927;*/

        //2000坐标系参数
        public const double f = 1 / 298.257222101;
        public const double RE = 6378137.0;
        public const double RP = 6356752.3141;
        public const double e1 = 0.081819191042816;

        /*
         * 函数名：double RadToDeg(double radIn)
         * 描述：弧度转换为度
         * 输入：double radIn：弧度
         * 输出：度：double
         * 版本：V1.0.0.0，20180731
         * 编者：Biting
         */
        public static double RadToDeg(double radIn)
        {
            return radIn * 180.0 / PI;
        }

        /*
         * 函数名：double DegToRad(double degreeIn)
         * 描述：度转换为弧度
         * 输入：double degreeIn：度
         * 输出：弧度：double
         * 版本：V1.0.0.0，20180731
         * 编者：Biting
         */
        public static double DegToRad(double degreeIn)
        {
            return degreeIn * PI / 180.0;
        }

        /*
         * 函数名：string DegToDMS(double degreeIn)
         * 描述：度转换为度分秒
         * 输入：double degreeIn：度
         * 输出：度分秒：double，格式：X°X′X″
         * 版本：V1.0.0.0，20180731
         * 编者：Biting
         */
        public static string DegToDMS(double degreeIn)
        {
            string strOut = String.Empty;
            int degree = 0, minute = 0, second = 0;
            degree = (int)degreeIn;
            minute = (int)((degreeIn - degree) * 60.0);
            second = (int)(((degreeIn - degree) * 60.0 - minute) * 60.0);
            strOut = String.Format("{0}°{1}′{2}″", degree, minute, second);
            return strOut;
        }

        /*
         * 函数名：double MilToRad(double milIn)
         * 描述：密位转换为弧度
         * 输入：double milIn：密位
         * 输出：弧度：double
         * 版本：V1.0.0.0，20180731
         * 编者：Biting
         */
        public static double MilToRad(double milIn)
        {
            return milIn * PI / 3000.0;
        }

        /*
         * 函数名：RadToMil(double radIn)
         * 描述：弧度转换为密位
         * 输入：double radIn：弧度
         * 输出：密位：double
         * 版本：V1.0.0.0，20180731
         * 编者：Biting
         */
        public static double RadToMil(double radIn)
        {
            return radIn * 3000.0 / PI;
        }

        /*
         * 函数名：void GetHourMinuteSecond(int data, ref int hour, ref int minute, ref int second)
         * 描述：将整型数据转换为时、分、秒
         * 输入：int data：待转换的整型数据（单位：秒，如5323秒）;
         *       ref int hour：引用类型-时
         *       ref int minute：引用类型-分
         *       ref int second：引用类型-秒
         * 输出：null
         * 版本：V1.0.0.0，20180731
         * 编者：Biting
         */
        public static void GetHourMinuteSecond(int data, ref int hour, ref int minute, ref int second)
        {
            int tempH, tempM;
            tempH = (int)(data / 3600);
            hour = tempH;
            tempM = (int)((data - (int)(data / 3600) * 3600) / 60);
            minute = tempM;
            second = data - tempH * 3600 - tempM * 60;           
        }

        /*
         * 函数名：GaussCoordinate ConvertGeodeticToGauss(GeodeticCoordinate geodeticCoordinate)
         * 描述：将WGS54地理坐标转换为WGS54高斯坐标
         * 输入：GeodeticCoordinate geodeticCoordinate：WGS54地理坐标
         * 输出：WGS54高斯坐标：GaussCoordinate
         * 版本：V1.0.0.0，20180731
         * 编者：Biting
         */
        public static GlobalDefinitions.GaussCoordinate ConvertGeodeticToGauss(GlobalDefinitions.GeodeticCoordinate geodeticCoordinate)
        {
            double x, Rn, t, n2, ll, i;
            int u;//区号
            GlobalDefinitions.GaussCoordinate gaussCoordinate;

            Rn = RE * (1 + f * Math.Sin(geodeticCoordinate.latitude.inRad) * Math.Sin(geodeticCoordinate.latitude.inRad));
            t = Math.Tan(geodeticCoordinate.latitude.inRad);
            n2 = Math.Cos(geodeticCoordinate.latitude.inRad) * Math.Cos(geodeticCoordinate.latitude.inRad) * e1 * e1;
            i = Math.Truncate((geodeticCoordinate.longitude.inRad * 180 / PI) / 6);

            u = (int)i + 1;
            ll = geodeticCoordinate.longitude.inRad - (6 * u - 3) * PI / 180;

            x = 6367558.497 * geodeticCoordinate.latitude.inRad - 16036.48 * Math.Sin(2 * geodeticCoordinate.latitude.inRad) +
                16.828 * Math.Sin(4 * geodeticCoordinate.latitude.inRad) - 0.022 * Math.Sin(6 * geodeticCoordinate.latitude.inRad);
            gaussCoordinate.x = x + Rn * Math.Sin(geodeticCoordinate.latitude.inRad) * Math.Cos(geodeticCoordinate.latitude.inRad) * ll * ll / 2;
            gaussCoordinate.x += Rn * Math.Sin(geodeticCoordinate.latitude.inRad) * Math.Pow(Math.Cos(geodeticCoordinate.latitude.inRad), 3) * 
                                Math.Pow(ll, 4) * (5 - t * t - 9 * n2) / 24;

            gaussCoordinate.y = Rn * Math.Cos(geodeticCoordinate.latitude.inRad) * ll + Rn * Math.Pow(Math.Cos(geodeticCoordinate.latitude.inRad), 3) * 
                                Math.Pow(ll, 3) * (1 - t * t + n2) / 6;
            gaussCoordinate.y += Rn * Math.Pow(Math.Cos(geodeticCoordinate.latitude.inRad), 5) * 
                                    Math.Pow(ll, 5) * (5 - 18 * t * t + Math.Pow(t, 4)) / 120 + 500000;
            gaussCoordinate.y += u * 1000000;

            gaussCoordinate.h = geodeticCoordinate.altitude.inRad;

            return gaussCoordinate;
        }

        /*
         * 函数名：GeodeticCoordinate ConvertGaussToGeodetic(GaussCoordinate gaussCoordinate)
         * 描述：将WGS54高斯坐标转换为WGS54地理坐标
         * 输入：GaussCoordinate gaussCoordinate：WGS54高斯坐标
         * 输出：WGS54地理坐标：GeodeticCoordinate
         * 版本：V1.0.0.0，20180731
         * 编者：Biting
         */
        public static GlobalDefinitions.GeodeticCoordinate ConvertGaussToGeodetic(GlobalDefinitions.GaussCoordinate gaussCoordinate)
        {
            double Rmf, Rnf, tf, nf2, midB1 = 0, midB2, bf;
            int u = 0;//区号
            u = Convert.ToInt32((gaussCoordinate.y - Convert.ToInt32(gaussCoordinate.y % 1000000.0)) / 1000000);
            gaussCoordinate.y -= (u * 1000000);
            GlobalDefinitions.GeodeticCoordinate geodeticCoordinate;

            do
            {
                bf = (gaussCoordinate.x + 16036.48 * Math.Sin(2 * midB1) - 16.828 * Math.Sin(4 * midB1) + 0.022 * Math.Sin(6 * midB1)) / 6367558.497;
                midB2 = midB1;
                midB1 = bf;
            } while (Math.Abs(midB1 - midB2) > 0.000005);

            gaussCoordinate.y -= 500000;
            tf = Math.Tan(bf);
            nf2 = e1 * e1 * Math.Pow(Math.Cos(bf), 2);
            Rnf = RE * (1 + f * Math.Sin(bf) * Math.Sin(bf));
            Rmf = RE * (1 - 2 * f + 3 * f * Math.Sin(bf) * Math.Sin(bf));
            geodeticCoordinate.latitude.inRad = bf - tf * Math.Pow((gaussCoordinate.y), 2) / (2 * Rmf * Rnf);
            geodeticCoordinate.latitude.inRad += tf * Math.Pow((gaussCoordinate.y), 4) * (5 + 3 * Math.Pow(tf, 2) + nf2 - 9 * nf2 * tf * tf) / (24 * Rmf * Math.Pow(Rnf, 3));
            geodeticCoordinate.latitude.inDegree = RadToDeg(geodeticCoordinate.latitude.inRad);
            geodeticCoordinate.latitude.inNormalFormat = geodeticCoordinate.latitude.inRad;

            geodeticCoordinate.longitude.inRad = gaussCoordinate.y / (Rnf * Math.Cos(bf) + (1 + 2 * tf * tf + nf2) * Math.Cos(bf) * Math.Pow(gaussCoordinate.y, 2) / (6 * Rnf));
            geodeticCoordinate.longitude.inRad += (5 + 44 * tf * tf + 32 * Math.Pow(tf, 4)) * Math.Pow(gaussCoordinate.y, 5) / (360 * Math.Pow(Rnf, 5) * Math.Cos(bf));
            geodeticCoordinate.longitude.inRad += (6 * u - 3) * PI / 180;
            geodeticCoordinate.longitude.inDegree = RadToDeg(geodeticCoordinate.longitude.inRad);
            geodeticCoordinate.longitude.inNormalFormat = geodeticCoordinate.longitude.inRad;

            geodeticCoordinate.altitude.inRad = gaussCoordinate.h;
            geodeticCoordinate.altitude.inDegree = gaussCoordinate.h;
            geodeticCoordinate.altitude.inNormalFormat = gaussCoordinate.h;

            gaussCoordinate.y += 500000;

            return geodeticCoordinate;
        }

        /*
         * 函数名：GaussCoordinate ConvertGeodeticToGauss(GeodeticCoordinate geodeticCoordinate)
         * 描述：将CGCS2000地理坐标转换为CGCS2000高斯坐标
         * 输入：GeodeticCoordinate geodeticCoordinate：CGCS2000地理坐标
         * 输出：CGCS2000高斯坐标：GaussCoordinate
         * 版本：V1.0.0.0，20180731
         * 编者：Biting
         */
        public static GlobalDefinitions.GaussCoordinate ConvertGeodeticToGauss2000(GlobalDefinitions.GeodeticCoordinate geodeticCoordinate)
        {
            double X0, N, L0, dL, g;
            int u;
            GlobalDefinitions.GaussCoordinate gaussCoordinate;         

            X0 = 111132.952547 * geodeticCoordinate.latitude.inDegree;
            X0 += 0 - 16038.508741268 * Math.Sin(2 * geodeticCoordinate.latitude.inRad) + 16.832613326622 * Math.Sin(4 * geodeticCoordinate.latitude.inRad) - 
                0.021984374201268 * Math.Sin(6 * geodeticCoordinate.latitude.inRad) + 3.1141625291648 * Math.Sin(8 * geodeticCoordinate.latitude.inRad) / 100000;
            N = RE / Math.Sqrt(1 - e1 * e1 * Math.Sin(geodeticCoordinate.latitude.inRad) * Math.Sin(geodeticCoordinate.latitude.inRad));

            u = (int)(geodeticCoordinate.longitude.inDegree / 6) + 1;
            L0 = u * 6 - 3;
            dL = geodeticCoordinate.longitude.inRad - GlobalDefinitions.DegToRad(L0);
            g = Math.Cos(geodeticCoordinate.latitude.inRad) * Math.Cos(geodeticCoordinate.latitude.inRad) * e1 * e1 / Math.Sqrt(1 - e1 * e1);
            gaussCoordinate.x = X0 + N / 2 * dL * dL * Math.Sin(geodeticCoordinate.latitude.inRad) * Math.Cos(geodeticCoordinate.latitude.inRad) + 
                                N / 24 * Math.Pow(dL, 4) * Math.Sin(geodeticCoordinate.latitude.inRad) * Math.Pow(Math.Cos(geodeticCoordinate.latitude.inRad), 3) * 
                                (5 - Math.Tan(geodeticCoordinate.latitude.inRad) * Math.Tan(geodeticCoordinate.latitude.inRad) + 9 * g + 4 * g * g);
            gaussCoordinate.x += N / 720 * Math.Pow(dL, 6) * Math.Sin(geodeticCoordinate.latitude.inRad) * Math.Pow(Math.Cos(geodeticCoordinate.latitude.inRad), 5) * 
                                (61 - 58 * Math.Tan(geodeticCoordinate.latitude.inRad) * Math.Tan(geodeticCoordinate.latitude.inRad) + Math.Pow(Math.Tan(geodeticCoordinate.latitude.inRad), 4));
            gaussCoordinate.y = 500000 + N * dL * Math.Cos(geodeticCoordinate.latitude.inRad);
            gaussCoordinate.y += N / 6 * Math.Pow(dL * Math.Cos(geodeticCoordinate.latitude.inRad), 3) * (1 - Math.Tan(geodeticCoordinate.latitude.inRad) * Math.Tan(geodeticCoordinate.latitude.inRad) + g);
            gaussCoordinate.y += N / 120 * Math.Pow(dL * Math.Cos(geodeticCoordinate.latitude.inRad), 5) * (5 - 18 * Math.Tan(geodeticCoordinate.latitude.inRad) * Math.Tan(geodeticCoordinate.latitude.inRad) +
                    Math.Pow(Math.Tan(geodeticCoordinate.latitude.inRad), 4) + 14 * g - 58 * g * Math.Tan(geodeticCoordinate.latitude.inRad) * Math.Tan(geodeticCoordinate.latitude.inRad));

            gaussCoordinate.y += u * 1000000;

            gaussCoordinate.h = geodeticCoordinate.altitude.inRad;

            return gaussCoordinate;
        }

        /*
         * 函数名：GeodeticCoordinate ConvertGaussToGeodetic(GaussCoordinate gaussCoordinate)
         * 描述：将CGCS2000高斯坐标转换为CGCS2000地理坐标
         * 输入：GaussCoordinate gaussCoordinate：CGCS2000高斯坐标
         * 输出：CGCS2000地理坐标：GeodeticCoordinate
         * 版本：V1.0.0.0，20180731
         * 编者：Biting
         */
        public static GlobalDefinitions.GeodeticCoordinate ConvertGaussToGeodetic2000(GlobalDefinitions.GaussCoordinate gaussCoordinate)
        {         
            double L0, db1, db0, bf, tf, Nf, Mf, g2;
            int u = 0;//区号
            GlobalDefinitions.GeodeticCoordinate geodeticCoordinate = new GeodeticCoordinate();

            u = Convert.ToInt32((gaussCoordinate.y - Convert.ToInt32(gaussCoordinate.y % 1000000.0)) / 1000000);
            gaussCoordinate.y -= (u * 1000000);

            gaussCoordinate.y -= 500000;
            L0 = (6 * u - 3) * PI / 180;
            db0 = 0; db1 = 0; bf = 0;
            do
            {
                bf = 8.9982311914 * gaussCoordinate.x / 1000000 + 0.144318209619 * Math.Sin(2 * bf) - 1.514637462683 * Math.Sin(4 * bf) / 10000;
                bf += 1.9782048166 * Math.Sin(6 * bf) / 10000000 - 2.80219544 * Math.Sin(8 * bf) / 10000000000;
                bf = bf * PI / 180;
                db1 = db0;
                db0 = bf;
            } while (Math.Abs(db1 - db0) >= 0.00003 * PI / (180 * 3600));

            tf = Math.Tan(bf);
            g2 = e1 * e1 * Math.Cos(bf) * Math.Cos(bf) / (1 - e1 * e1);
            Nf = RE / Math.Sqrt(1 - e1 * e1 * Math.Sin(bf) * Math.Sin(bf));
            Mf = Nf * (1 - e1 * e1) / (1 - e1 * e1 * Math.Sin(bf) * Math.Sin(bf));

            geodeticCoordinate.latitude.inRad = 1 - gaussCoordinate.y * gaussCoordinate.y / (12 * Nf * Nf) * (5 + g2 + 3 * tf * tf - 9 * g2 * tf * tf) + Math.Pow(gaussCoordinate.y, 4) / (360 * Math.Pow(Nf, 4)) * (61 + 90 * tf * tf + 45 * Math.Pow(tf, 4));
            geodeticCoordinate.latitude.inRad *= gaussCoordinate.y * gaussCoordinate.y * tf / (2 * Mf * Nf);
            geodeticCoordinate.latitude.inRad = bf - geodeticCoordinate.latitude.inRad;
            geodeticCoordinate.latitude.inDegree = RadToDeg(geodeticCoordinate.latitude.inRad);
            geodeticCoordinate.latitude.inNormalFormat = geodeticCoordinate.latitude.inRad;

            geodeticCoordinate.longitude.inRad = 1 - gaussCoordinate.y * gaussCoordinate.y / (6 * Nf * Nf) * (1 + g2 + 2 * tf * tf) + Math.Pow(gaussCoordinate.y, 4) / (120 * Math.Pow(Nf, 4)) * (5 + 6 * g2 + 28 * tf * tf + 8 * g2 * tf * tf + 24 * Math.Pow(tf, 4));
            geodeticCoordinate.longitude.inRad *= gaussCoordinate.y / (Nf * Math.Cos(bf));
            geodeticCoordinate.longitude.inRad += L0;
            geodeticCoordinate.longitude.inDegree = RadToDeg(geodeticCoordinate.longitude.inRad);
            geodeticCoordinate.longitude.inNormalFormat = geodeticCoordinate.longitude.inRad;

            geodeticCoordinate.altitude.inRad = gaussCoordinate.h;
            geodeticCoordinate.altitude.inDegree = gaussCoordinate.h;
            geodeticCoordinate.altitude.inNormalFormat = gaussCoordinate.h;

            gaussCoordinate.y += 500000;

            return geodeticCoordinate;
        }

        /*
         * 函数名：double GetConvergenceAngle(GlobalDefinitions.GaussCoordinate gaussCoordinate)
         * 描述：获取给定高斯坐标下的收敛角
         * 输入：GaussCoordinate gaussCoordinate
         * 输出：收敛角：double
         * 版本：V1.0.0.0，20180731
         * 编者：Biting
         */
        public static double GetConvergenceAngle(GlobalDefinitions.GaussCoordinate gaussCoordinate)
        {
            double n2, ll, t2, B1;
            GlobalDefinitions.GeodeticCoordinate geodeticCoordinate = new GeodeticCoordinate();
            int u = Convert.ToInt32((gaussCoordinate.y - Convert.ToInt32(gaussCoordinate.y % 1000000.0)) / 1000000);
            //gaussCoordinate.y -= (u * 1000000);           

            geodeticCoordinate = ConvertGaussToGeodetic2000(gaussCoordinate);

            n2 = Math.Cos(geodeticCoordinate.latitude.inRad) * Math.Cos(geodeticCoordinate.latitude.inRad) * e1 * e1;
            ll = geodeticCoordinate.longitude.inRad - (6 * u - 3) * PI / 180;
            t2 = Math.Pow(Math.Tan(geodeticCoordinate.latitude.inRad), 2);
            B1 = 1 + ll * ll * Math.Pow(Math.Cos(geodeticCoordinate.latitude.inRad), 2) * (1 + 3 * n2 + 2 * n2 * n2) / 3;
            B1 += Math.Pow(ll, 4) * Math.Pow(Math.Cos(geodeticCoordinate.latitude.inRad), 4) * (2 - t2) / 15;
            B1 *= GlobalDefinitions.RadToMil(ll * Math.Sin(geodeticCoordinate.latitude.inRad));            

            return B1;
        }

        //public static DateTime currentTime;
        public const string datePatternForSave = @"yyyyMMdd";//@"yyyy年M月d日";
        public const string timePatternForSave = @"HHmmss";//@"tt_hh.mm.ss";

        //全局的委托和事件的定义
        //public delegate void GlobalEventHandler(object sender, GlobalEventArgs e);
        //public event GlobalEventHandler ResponseFromGlobalEvent;

        /*
         * 函数名：int ConvertCharToHex(byte ch)
         * 描述：将字符转换为十六进制数据
         * 输入：byte ch：字符
         * 输出：十六进制数据：int
         * 版本：V1.0.0.0，20180731
         * 编者：Biting
         */
        public static int ConvertCharToHex(byte ch)
        {
            if ((ch >= '0') && (ch <= '9'))
            {
                return Convert.ToInt32(ch - 0x30);
            }
            else
            {
                if ((ch >= 'A') && (ch <= 'F'))
                {
                    return Convert.ToInt32(ch - 'A' + 10);
                }
                else
                {
                    if ((ch >= 'a') && (ch <= 'f'))
                    {
                        return Convert.ToInt32(ch - 'a' + 10);
                    }
                    else
                    {
                        return Convert.ToInt32(-1);
                    }
                }
            }
        }

        /*
         * 函数名：ConvertStringToHex(string strSource, byte[] byteTarget)
         * 描述：将string转换为十六进制数据
         * 输入：string strSource：字符串
         *       byte[] byteTarget：十六进制数据存储区
         * 输出：存储区数据长度：int
         * 版本：V1.0.0.0，20180731
         * 编者：Biting
         */
        public static int ConvertStringToHex(string strSource, byte[] byteTarget)
        {
            int iHexData = 0, iLowHexData = 0, iHexDataLength = 0, iLength = 0;

            iLength = strSource.Length;
            char[] arraySource = strSource.ToCharArray();

            for (int j = 0; j < iLength; )
            {
                byte lstr = 0, hstr = Convert.ToByte(arraySource[j]);
                if (hstr == ' ')
                {
                    j++;
                    continue;
                }
                j++;
                if (j > iLength)
                {
                    break;
                }
                lstr = Convert.ToByte(arraySource[j]);
                iHexData = ConvertCharToHex(hstr);
                iLowHexData = ConvertCharToHex(lstr);
                if ((16 == iHexData) && (16 == iLowHexData))
                {
                    break;
                }
                else
                {
                    iHexData = iHexData * 16 + iLowHexData;
                    j++;
                    byteTarget[iHexDataLength++] = Convert.ToByte(iHexData);

                }
            }
            return iHexDataLength;
        }

        /*
         * 函数名：double ConvertByteToDouble(int startIndex, int length, byte[] sourceBuffer, int decimalDigits)
         * 描述：字节数据转换为double型变量
         * 输入：int startIndex：字节数据在缓冲区中的起始位置
         *       int length：字节数据长度
         *       byte[] sourceBuffer：字节数据所在的缓冲区
         *       int decimalDigits：输出数据的小数点位数
         * 输出：转换结果：double
         * 版本：V1.0.0.0，20180731
         * 编者：Biting
         */
        public static double ConvertByteToDouble(int startIndex, int length, byte[] sourceBuffer, int decimalDigits)
        {
            byte[] temp = new byte[12];
            double valueOut = 0.0;
            string strValue = String.Empty;
            for (int loop = 0; loop < length; loop++)
            {    
                strValue = strValue + Convert.ToChar(sourceBuffer[startIndex + loop]);
            }

            //valueOut = Convert.ToDouble(String.Format("{0:F1}", Convert.ToDouble(strValue) / Math.Pow(10, decimalDigits)));
            valueOut = Convert.ToDouble(strValue) / Math.Pow(10, decimalDigits);

            return valueOut;
        }

        /*
         * 函数名：IsNumber(string dataIn)
         * 描述：给定字符串是否为纯数字
         * 输入：string dataIn：字符串
         * 输出：给定字符串为纯数字：true/给定字符串中含有纯数字外的其他字符：false
         * 版本：V1.0.0.0，20180731
         * 编者：Biting
         */
        public static bool IsNumber(string dataIn)
        {
            try
            {
                Convert.ToDouble(dataIn);
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        public const byte SynchCodeFront = 0x55, SynchCodeRear = 0xAA;

        /*
         * 函数名：int ConverBufferInSendProtocol(byte cmd, byte bufferLength, byte[] sourceBuffer, ref byte[] targetBuffer)
         * 描述：将字节数组中的数据打包帧头、缓冲区长度、命令码后添加CRC和校验码
         * 输入：byte cmd：命令码
         *       byte bufferLength：输入的字节数据长度
         *       byte[] sourceBuffer：字节数据缓冲区
         *       ref byte[] targetBuffer：打包输出缓冲区
         * 输出：输出缓冲区长度：int
         * 版本：V1.0.0.0，20180731
         * 编者：Biting
         */
        public static int ConverBufferInSendProtocol(byte cmd, byte bufferLength, byte[] sourceBuffer, ref byte[] targetBuffer)
        {
            int targetBufferLength = 0, crcCheckCode = 0;
            targetBuffer[0] = SynchCodeFront;
            targetBuffer[1] = SynchCodeRear;
            targetBuffer[2] = cmd;
            targetBuffer[3] = (byte)(bufferLength + 1);
            crcCheckCode = SynchCodeFront + SynchCodeRear + cmd + bufferLength + 1;
            targetBufferLength += 4;

            if (bufferLength > 0)
            {
                foreach (byte b in sourceBuffer)
                {
                    targetBuffer[targetBufferLength++] = b;
                    crcCheckCode += b;
                }
                targetBuffer[targetBufferLength] = (byte)(256 - (crcCheckCode % 256));
            }

            return targetBufferLength;
        }

        /*
         * 函数名：bool JudgeHeadFrame(byte[] sourceuffer, int headStartPos)
         * 描述：给定缓冲区中的数据是否满足CRC和校验
         * 输入：byte[] sourceuffer：字节数组缓冲区
         *       int headStartPos：字节数组在缓冲区中的起始位置
         * 输出：满足CRC和校验：true/不满足CRC和校验：false
         * 版本：V1.0.0.0，20180731
         * 编者：Biting
         */
        public static bool JudgeHeadFrame(byte[] sourceuffer, int headStartPos)
        {
            short headFrameSum = 0;
            for (int loop = 0; loop < 6; loop++)
            {
                headFrameSum += sourceuffer[loop];
            }
            if ((headFrameSum % 256) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /*
         * 函数名：int TransformSourceBufferWithStdProtocol(byte synchCode, byte sourceAddress, byte targetAddress,
                                                                byte commandCode, int sourceBufferLength, byte[] sourceBuffer,
                                                                ref byte[] targetBuffer)
         * 描述：以测地车标准通信协议规定的格式转换源缓冲区中的数据并转存至目标缓冲区中 
         * 输入：byte synchCode：帧头同步码
         *       byte sourceAddress：源地址
         *       byte targetAddress：目标地址
         *       byte commandCode：命令码
         *       int sourceBufferLength：源缓冲区长度
         *       byte[] sourceBuffer：源缓冲区
         *       ref byte[] targetBuffer：目标缓冲区
         * 输出：目标缓冲区长度：int
         * 版本：V1.0.0.0，20180731
         * 编者：Biting
         */
        public static int TransformSourceBufferWithStdProtocol(byte synchCode, byte sourceAddress, byte targetAddress,
                                                                byte commandCode, int sourceBufferLength, byte[] sourceBuffer,
                                                                ref byte[] targetBuffer)
        {
            int modHead = 0x00, modData = 0x00;
            int pointer = 0;
            targetBuffer[pointer++] = synchCode;
            targetBuffer[pointer++] = sourceAddress;
            targetBuffer[pointer++] = targetAddress;
            targetBuffer[pointer++] = commandCode;
            modHead = Convert.ToInt16(synchCode + sourceAddress + targetAddress + commandCode);
            if (sourceBufferLength > 0)
            {
                targetBuffer[pointer++] = Convert.ToByte(sourceBufferLength + 1);
                modHead += (sourceBufferLength + 1);
                if (modHead == 256)
                {
                    targetBuffer[pointer++] = 0x00;
                }
                else
                {
                    targetBuffer[pointer++] = Convert.ToByte(256 - (modHead % 256));
                }
                for (int loop = 0; loop < sourceBufferLength; loop++)
                {
                    targetBuffer[pointer++] = sourceBuffer[loop];
                    modData += sourceBuffer[loop];
                }
                if (modData == 256)
                {
                    targetBuffer[pointer++] = 0x00;
                }
                else
                {
                    targetBuffer[pointer++] = Convert.ToByte(256 - (modData % 256));
                }
                //targetBuffer[pointer++] = Convert.ToByte(256 - (modData % 256));
            }
            else
            {
                targetBuffer[pointer++] = Convert.ToByte(sourceBufferLength);
                if (modHead == 256)
                {
                    targetBuffer[pointer++] = 0x00;
                }
                else
                {
                    targetBuffer[pointer++] = Convert.ToByte(256 - (modHead % 256));
                }
            }

            return pointer;
        }

        /*
         * 函数名：int TransformSourceBufferWithStdProtocol(byte synchCode, byte sourceAddress, byte targetAddress,
                                                                byte commandCode, string sourceBuffer,
                                                                ref byte[] targetBuffer)
         * 描述：以测地车标准通信协议规定的格式转换源缓冲区中的数据并转存至目标缓冲区中 
         * 输入：byte synchCode：帧头同步码
         *       byte sourceAddress：源地址
         *       byte targetAddress：目标地址
         *       byte commandCode：命令码
         *       string sourceBuffer：字符串源缓冲区
         *       ref byte[] targetBuffer：目标缓冲区
         * 输出：目标缓冲区长度：int
         * 版本：V1.0.0.0，20180731
         * 编者：Biting
         */
        public static int TransformSourceBufferWithStdProtocol(byte synchCode, byte sourceAddress, byte targetAddress,
                                                                byte commandCode, string sourceBuffer,
                                                                ref byte[] targetBuffer)
        {
            int modHead = 0x00, modData = 0x00;
            int pointer = 0;
            int sourceBufferLength = sourceBuffer.Length;
            targetBuffer[pointer++] = synchCode;
            targetBuffer[pointer++] = sourceAddress;
            targetBuffer[pointer++] = targetAddress;
            targetBuffer[pointer++] = commandCode;
            modHead = Convert.ToInt16(synchCode + sourceAddress + targetAddress + commandCode);
            if (sourceBufferLength > 0)
            {
                targetBuffer[pointer++] = Convert.ToByte(sourceBufferLength + 1);
                modHead += (sourceBufferLength + 1);
                if (modHead == 256)
                {
                    targetBuffer[pointer++] = 0x00;
                }
                else
                {
                    targetBuffer[pointer++] = Convert.ToByte(256 - (modHead % 256));
                }
                //targetBuffer[pointer++] = Convert.ToByte(256 - (modHead % 256));
                foreach (char ch in sourceBuffer.ToCharArray())
                {
                    targetBuffer[pointer++] = Convert.ToByte(ch);
                    modData += Convert.ToByte(ch);
                }
                if (modData == 256)
                {
                    targetBuffer[pointer++] = 0x00;
                }
                else
                {
                    targetBuffer[pointer++] = Convert.ToByte(256 - (modData % 256));
                }
                //targetBuffer[pointer++] = Convert.ToByte(256 - (modData % 256));
            }
            else
            {
                targetBuffer[pointer++] = Convert.ToByte(sourceBufferLength);
                if (modHead == 256)
                {
                    targetBuffer[pointer++] = 0x00;
                }
                else
                {
                    targetBuffer[pointer++] = Convert.ToByte(256 - (modHead % 256));
                }
                targetBuffer[pointer++] = Convert.ToByte(256 - (modHead % 256));
            }

            return pointer;
        }

        /*
         * 函数名：int TransformSourceBufferWithStdProtocol(byte synchCode, byte sourceAddress, byte targetAddress,
                                                                byte commandCode, ref byte[] targetBuffer)
         * 描述：以测地车标准通信协议规定的格式转换数据并转存至目标缓冲区中 
         * 输入：byte synchCode：帧头同步码
         *       byte sourceAddress：源地址
         *       byte targetAddress：目标地址
         *       byte commandCode：命令码
         *       ref byte[] targetBuffer：目标缓冲区
         * 输出：目标缓冲区长度：int
         * 版本：V1.0.0.0，20180731
         * 编者：Biting
         */
        public static int TransformSourceBufferWithStdProtocol(byte synchCode, byte sourceAddress, byte targetAddress,
                                                                byte commandCode, ref byte[] targetBuffer)
        {
            int modHead = 0x00;
            int pointer = 0;
            targetBuffer[pointer++] = synchCode;
            targetBuffer[pointer++] = sourceAddress;
            targetBuffer[pointer++] = targetAddress;
            targetBuffer[pointer++] = commandCode;
            modHead = Convert.ToInt16(synchCode + sourceAddress + targetAddress + commandCode);
            targetBuffer[pointer++] = 0x00;
            if (modHead == 256)
            {
                targetBuffer[pointer++] = 0x00;
            }
            else
            {
                targetBuffer[pointer++] = Convert.ToByte(256 - (modHead % 256));
            }
            //targetBuffer[pointer++] = Convert.ToByte(256 - (modHead % 256));

            return pointer;
        }

        /*
         * 函数名：void GetCoordinateIntegerAndDecimal(double value, ref int integerPart, ref int decimalPart)
         * 描述：获取double型数据中的整数部分和小数部分
         * 输入：double value：待解析的double型数据
         *       ref int integerPart：引用类型-保存待解析的double型数据中的整数部分
         *       ref int decimalPart：引用类型-保存待解析的double型数据中的小数部分
         * 输出：null
         * 版本：V1.0.0.0，20180731
         * 编者：Biting
         */
        public static void GetCoordinateIntegerAndDecimal(double value, ref int integerPart, ref int decimalPart)
        {
            int numberBeforeDot = 0, numberAfterDot = 0;
            double getDecimalFraction = 0.0, temp = 0.0;

            temp = Math.Abs(value);
            numberBeforeDot = (int)temp;
            getDecimalFraction = Convert.ToDouble(String.Format("{0:F1}", temp - numberBeforeDot));
            numberAfterDot = (int)(getDecimalFraction * 10.0);

            integerPart = numberBeforeDot;
            decimalPart = numberAfterDot;
        }

        /*
         * 函数名：int PackageCoordinateData(double coordinateX, double coordinateY, double coordinateH, ref byte[] targetBuffer)
         * 描述：将double型数据：X、Y、H坐标打包成字节类型的数组
         * 输入：double coordinateX：double型X坐标
         *       double coordinateY：double型Y坐标
         *       double coordinateH：double型H坐标
         *       ref byte[] targetBuffer：引用类型-目标缓冲区
         * 输出：目标缓冲区长度：int
         * 版本：V1.0.0.0，20180731
         * 编者：Biting
         */
        public static int PackageCoordinateData(double coordinateX, double coordinateY, double coordinateH, ref byte[] targetBuffer)
        {
            int pointer = 0, numberBeforeDot = 0, numberAfterDot = 0;
            
            string temp;

            //打包坐标X
            GetCoordinateIntegerAndDecimal(coordinateX, ref numberBeforeDot, ref numberAfterDot);
            temp = coordinateX >= 0.0 ? String.Format("+{0:D7}{1:D1}", numberBeforeDot, numberAfterDot) : 
                                        String.Format("-{0:D7}{1:D1}", numberBeforeDot, numberAfterDot);
            foreach (char ch in temp.ToCharArray())
            {
                targetBuffer[pointer++] = Convert.ToByte(ch);
            }

            //打包坐标Y
            GetCoordinateIntegerAndDecimal(coordinateY, ref numberBeforeDot, ref numberAfterDot);
            temp = String.Format("{0:D8}{1:D1}", numberBeforeDot, numberAfterDot);
            foreach (char ch in temp.ToCharArray())
            {
                targetBuffer[pointer++] = Convert.ToByte(ch);
            }

            //打包坐标H
            GetCoordinateIntegerAndDecimal(coordinateH, ref numberBeforeDot, ref numberAfterDot);
            temp = coordinateH >= 0.0 ? String.Format("+{0:D4}{1:D1}", numberBeforeDot, numberAfterDot) :
                                        String.Format("-{0:D4}{1:D1}", numberBeforeDot, numberAfterDot);
            foreach (char ch in temp.ToCharArray())
            {
                targetBuffer[pointer++] = Convert.ToByte(ch);
            }

            return pointer;
        }

        /*
         * 函数名：int PackageReaimedMethod1Data(ref byte[] targetBuffer)
         * 描述：按测地车标准协议打包定位定向系统重新对准1命令
         * 输入：ref byte[] targetBuffer：引用类型-目标缓冲区
         * 输出：目标缓冲区长度：int
         * 版本：V1.0.0.0，20180731
         * 编者：Biting
         */
        public static int PackageReaimedMethod1Data(ref byte[] targetBuffer)
        {
            int pointer = 0;         

            targetBuffer[pointer++] = 0x01;

            return pointer;
        }

        /*
         * 函数名：PackageReaimedMethod2Data(double coordinateX, double coordinateY, double coordinateH, ref byte[] targetBuffer)
         * 描述：按测地车标准协议打包定位定向系统重新对准2命令
         * 输入：double coordinateX：坐标X
         *       double coordinateY：坐标Y
         *       double coordinateH：坐标H
         *       ref byte[] targetBuffer：引用类型-目标缓冲区
         * 输出：目标缓冲区长度：int
         * 版本：V1.0.0.0，20180731
         * 编者：Biting
         */
        public static int PackageReaimedMethod2Data(double coordinateX, double coordinateY, double coordinateH, ref byte[] targetBuffer)
        {
            int pointer = 0, tempBufferLength = 0;
            byte[] tempBuffer = new byte[64];

            //重新对准2
            targetBuffer[pointer++] = 0x02;
            tempBufferLength = GlobalDefinitions.PackageCoordinateData(coordinateX, coordinateY, coordinateH, ref tempBuffer);
            for (int loop = 0; loop < tempBufferLength; loop ++ )
            {
                targetBuffer[pointer++] = tempBuffer[loop];
            }             

            return pointer;
        }

        /*
         * 函数名：long GetFileTotalNumberOfLine(string filePath)
         * 描述：获取文件总行数
         * 输入：string filePath：文件绝对路径
         * 输出：文件总行数：long
         * 版本：V1.0.0.0，20180731
         * 编者：Biting
         */
        public static long GetFileTotalNumberOfLine(string filePath)
        {
            long lines = 0;
            using (var sr = new StreamReader(filePath))
            {
                var ls = "";
                while((ls = sr.ReadLine()) != null)
                {
                    lines++;
                }
            }
            return lines;
        }

        /*
         * 函数名：bool DecodeGyroDataFromString(string source, ref DwdxDataStruct gyroData)
         * 描述：从字符串中尝试解析测地车定位定向系统数据
         * 输入：string source：字符串数据
         *       ref DwdxDataStruct gyroData：引用类型-定位定向系统数据结构体
         * 输出：解析成功：true/解析失败：false
         * 版本：V1.0.0.0，20180731
         * 编者：Biting
         */
        public static bool DecodeGyroDataFromString(string source, ref DwdxDataStruct gyroData)
        {
            //
            
            string[] array = source.Split(' ');
            if(array.Length == 12)
            {
                try
                {
                    gyroData.dCoordinateX = Convert.ToDouble(array[0]);
                    gyroData.dCoordinateY = Convert.ToDouble(array[1]);
                    gyroData.dCoordinateH = Convert.ToDouble(array[2]);
                    gyroData.dDirection = Convert.ToDouble(array[3]);
                    gyroData.dLatitude = Convert.ToDouble(array[4]);
                    gyroData.dLongitude = Convert.ToDouble(array[5]);
                    gyroData.dPitch = Convert.ToDouble(array[6]);
                    gyroData.dRoll = Convert.ToDouble(array[7]);
                    gyroData.iZeroVelocityCountDown = Convert.ToInt32(array[8]);

                    byte[] temp = new byte[3];
                    ConvertStringToHex(array[9] + array[10] + array[11], temp);
                    gyroData.status = temp[0];
                    gyroData.errorCode1 = temp[1];
                    gyroData.errorCode2 = temp[2];
                    //gyroData.status = Convert.ToByte(array[9]);
                    //gyroData.errorCode1 = Convert.ToByte(array[10]);
                    //gyroData.errorCode2 = Convert.ToByte(array[11]);

                    return true;
                }
                catch(Exception e)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }            
        }

        ///   <summary>   
        ///   返回在给定开始日期、时间和结束日期、时间范围内的所有日期（精确到每天）、时间（精确到每小时）
        ///   </summary>   
        ///   <param name="shouldReturnDate">返回值选择，=true，函数返回日期；=false，函数返回时间</param>
        ///   <param name="dateBegin">起始日期</param>
        ///   <param name="dateEnd">结束日期</param>
        ///   <param name="timeBegin">起始时间（24h制）</param>
        ///   <param name="timeEnd">结束时间（24h制）</param>
        ///   <returns>根据输入参数返回不同值。若输入参数shouldReturnDate=true，则函数返回给定范围内的所有日期（精确到每天）；若输入参数shouldReturnDate=false，则函数返回给定范围内的所有时间（精确到每小时）</returns>
        public static string[] QueryDateTime(bool shouldReturnDate, string dateBegin, string dateEnd, string timeBegin, string timeEnd)
        {            
            //首先确保各个日期和时间的格式的正确性           
            Regex regDate = new Regex(@"^\d{4}\d{2}\d{2}");
            //Regex regTime = new Regex(@"^(0?[1-9]|1[0-2])$");
            Regex regTime = new Regex(@"^\d{2}:\d{2}:\d{2}");

            if (shouldReturnDate)
            {
                //返回给定范围内的所有日期（精确到每天）
                if (regDate.IsMatch(dateBegin) && regDate.IsMatch(dateEnd))
                {
                    int yearBegin = 0, yearEnd = 0, monthBegin = 0, monthEnd = 0, dayBegin = 0, dayEnd = 0;
                    yearBegin = int.Parse(dateBegin.Substring(0, 4));
                    monthBegin = int.Parse(dateBegin.Substring(4, 2));
                    dayBegin = int.Parse(dateBegin.Substring(6, 2));
                    yearEnd = int.Parse(dateEnd.Substring(0, 4));
                    monthEnd = int.Parse(dateEnd.Substring(4, 2));
                    dayEnd = int.Parse(dateEnd.Substring(6, 2));
                
                    int totalDays = 0;                    
                    DateTime begin = new DateTime(yearBegin, monthBegin, dayBegin), end = new DateTime(yearEnd, monthEnd, dayEnd);
                    TimeSpan temp = end.Subtract(begin);//end.DayOfYear - begin.DayOfYear;
                    totalDays = (int)temp.TotalDays;

                    //未避免起始日期和结束日期同一天时变量为0的情况，故做如下逻辑处理
                    totalDays = totalDays == 0 ? 1 : totalDays;
                    string[] results = new string[totalDays];
                    int loop = 0;                    
                    do
                    {
                        results[loop ++] = begin.ToString(@"yyyyMMdd");
                        begin = begin.AddDays(1);
                    } while (begin < end);

                    return results;
                }
                else
                    return null;
            }
            else
            {
                //返回给定范围内的所有时间（精确到每小时）
                if (regDate.IsMatch(dateBegin) && regDate.IsMatch(dateEnd) && regTime.IsMatch(timeBegin) && regTime.IsMatch(timeEnd))
                {
                    int yearBegin = 0, yearEnd = 0, monthBegin = 0, monthEnd = 0, dayBegin = 0, dayEnd = 0;
                    yearBegin = int.Parse(dateBegin.Substring(0, 4));
                    monthBegin = int.Parse(dateBegin.Substring(4, 2));
                    dayBegin = int.Parse(dateBegin.Substring(6, 2));
                    yearEnd = int.Parse(dateEnd.Substring(0, 4));
                    monthEnd = int.Parse(dateEnd.Substring(4, 2));
                    dayEnd = int.Parse(dateEnd.Substring(6, 2));

                    int hourBegin = 0, minuteBegin = 0, secondBegin = 0, hourEnd = 0, minuteEnd = 0, secondEnd = 0;
                    hourBegin = int.Parse(timeBegin.Substring(0, 2));
                    minuteBegin = int.Parse(timeBegin.Substring(3, 2));
                    secondBegin = int.Parse(timeBegin.Substring(6, 2));
                    hourEnd = int.Parse(timeEnd.Substring(0, 2));
                    minuteEnd = int.Parse(timeEnd.Substring(3, 2));
                    secondEnd = int.Parse(timeEnd.Substring(6, 2));

                    int resultLength = 0;
                    DateTime begin = new DateTime(yearBegin, monthBegin, dayBegin, hourBegin, minuteBegin, secondBegin), 
                             end = new DateTime(yearEnd, monthEnd, dayEnd, hourEnd, minuteEnd, secondEnd);
                    
                    TimeSpan timeElapse = end.Subtract(begin);
                    resultLength = (int)timeElapse.TotalHours;

                    //未避免起始日期时间和结束日期时间完全相同时变量为0的情况，故做如下逻辑处理
                    resultLength = resultLength == 0 ? 1 : resultLength;
                    string[] results = new string[resultLength];
                    int loop = 0;                    
                    do
                    {
                        results[loop++] = begin.ToString(@"yyyyMMdd HH");
                        begin = begin.AddHours(1);
                    } while (begin < end);

                    return results;
                }
                else
                    return null;
            }            
        }        
    }
}
