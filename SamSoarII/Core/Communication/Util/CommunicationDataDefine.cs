using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Core.Communication
{
    //PLC返回执行代码
    public enum FGs_ERR_CODE : byte
    {
        FGs_CARRY_OK = 0x20,            //写入成功
        FGs_CRC_ERR,                  //CRC校验失败
        FGs_LENTH_ERR,             //长度错误
        FGs_ADDRESS_ERR,           //元件寻址错误
        FGs_ADDRESS_TYPE_ERR,	   //地址种类错误
        FGs_DES_ERR,                //上载条形码密钥校验错误
        FGs_ADDRESS_BEYOND_ERR,	   //地址越界错误(用于VZ偏移地址越界)
        FGs_ISNOTANERRCODE,      // 非错误代码

        COMCODE_CARRY_OK = 0x60,         //成功
        COMCODE_LENGTH_ERR = 0x61,       //长度错误
        COMCODE_NODEID_ERR = 0x62,       //站号
        COMCODE_CRC_ERR = 0x63,          //CRC校验错误
        COMCODE_INVALID_CMD = 0x64,      //无效功能码(或当前PLC型号不支持该下载功能码)
        COMCODE_DOWNLOAD_BEYOND = 0x65,  //待下载数据大小超出限制
        COMCODE_PASSWD_ERR = 0x66,       //密码错误
    }
    public class CommunicationDataDefine
    {
        #region 监视
        public const byte USB_MAX_READ_LEN = 0x40;
        /* FGs_Connect Macro Define
         * 
         */
        //读写命令定义
        public const byte FGS_READ = 0xFE;
        public const byte FGS_WRITE = 0xFF;
        public const byte FORCE_CANCEL = 0xF6;
        //最多一次读写元件个数 ,以字节为长度
        public const byte MAX_ELEM_NUM = 0x20;
        //一包数据读写地址种类
        public const byte MAX_ADDRESS_TYPE = 0x06;
        //从站地址
        public const byte SLAVE_ADDRESS = 0x01;
        #endregion

        #region 上，下载
        /// <summary> 选项：是否包含程序 </summary>
        public const int OPTION_PROGRAM = 0x01;
        /// <summary> 选项：是否包含软元件 </summary>
        public const int OPTION_COMMENT = 0x02;
        /// <summary> 选项：是否包含初始化 </summary>
        public const int OPTION_INITIALIZE = 0x04;
        /// <summary> 选项：是否包含设置 </summary>
        public const int OPTION_SETTING = 0x08;
        
        public const byte CMD_COMMU_FLAG = 0xF0;//下载标志位
        public const int USB_COMMU_LEN = 56;//USB下载一包的最大长度
        public const int SERIAL_COMMU_LEN = 1024;//串口下载一包的最大长度
        //功能码列表

        public const byte CMD_COM_TEST = 0x01;//通信测试
        public const byte CMD_STATE_RUN = 0x02;//切换PLC状态为RUN
        public const byte CMD_STATE_STOP = 0x03;//切换PLC状态为STOP
        public const byte CMD_RETURN_PROTAG = 0x04;//返回当前程序标志(即bin文件的CRC)
        public const byte CMD_PASSWD_UPLOAD = 0x05;//上载密码
        public const byte CMD_PASSWD_DOWNLOAD = 0x06;//下载密码
        public const byte CMD_PASSWD_MONITOR = 0x07;//监控密码


        public const byte CMD_IAP_REBOOT = 0x10;//IAP引导命令
        public const byte CMD_IAP_DES_KEY = 0x11;//8个随机数秘钥
        public const byte CMD_IAP_PRO_BIN = 0x12;//下载bin文件
        public const byte CMD_IAP_FINISH = 0x13;//bin文件下载完毕

        //下载
        public const byte CMD_DOWNLOAD_START = 0x20;//某功能码下载开始
        public const byte CMD_DOWNLOAD_FINISH = 0x21;//某功能码下载完毕
        public const byte CMD_DOWNLOAD_ICON = 0x22;//下载条形码
        public const byte CMD_DOWNLOAD_PRO = 0x23;//下载程序(供上载使用的xml格式文件)
        public const byte CMD_DOWNLOAD_COMMENT = 0x24;//下载注释(仅供上载使用)
        public const byte CMD_DOWNLOAD_ELEMTABLE = 0x25;//下载软元件表
        public const byte CMD_DOWNLOAD_CONFIG = 0x26;//下载配置信息
        public const byte CMD_DOWNLOAD_MODBUSTABLE = 0x27;//下载Modbus表
        public const byte CMD_DOWNLOAD_PLSTABLE = 0x28;//下载脉冲Table表
        public const byte CMD_DOWNLOAD_PLSBLOCK = 0x29;//下载脉冲Block表

        //上载
        public const byte CMD_UPLOAD_START = 0x40;//某功能码上载开始
        public const byte CMD_UPLOAD_FINISH = 0x41;//某功能码上载完毕
        public const byte CMD_UPLOAD_ICON = 0x42;//上载条形码
        public const byte CMD_UPLOAD_PRO = 0x43;//上载程序
        public const byte CMD_UPLOAD_COMMENT = 0x44;//上载注释
        public const byte CMD_UPLOAD_ELEMTABLE = 0x45;//上载软元件表
        public const byte CMD_UPLOAD_CONFIG = 0x46;//上载配置信息
        public const byte CMD_UPLOAD_MODBUSTABLE = 0x47;//上载Modbus表
        public const byte CMD_UPLOAD_PLSTABLE = 0x48;//上载脉冲Table表
        public const byte CMD_UPLOAD_PLSBLOCK = 0x49;//上载脉冲Block表

        #endregion
        //PLC地址编号
        public const byte ADDRESS_TYPE_X = 0x00;
        public const byte ADDRESS_TYPE_Y = 0x01;
        public const byte ADDRESS_TYPE_M = 0x02;
        public const byte ADDRESS_TYPE_S = 0x03;
        public const byte ADDRESS_TYPE_C = 0x04;
        public const byte ADDRESS_TYPE_T = 0x05;

        public const byte ADDRESS_TYPE_AI = 0x06;
        public const byte ADDRESS_TYPE_AO = 0x07;
        public const byte ADDRESS_TYPE_D = 0x08;
        public const byte ADDRESS_TYPE_V = 0x09;
        public const byte ADDRESS_TYPE_Z = 0x10;
        public const byte ADDRESS_TYPE_CV = 0x11;
        public const byte ADDRESS_TYPE_TV = 0x12;
        public const byte ADDRESS_TYPE_CV32 = 0x13;
    }
}
