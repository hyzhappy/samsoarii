using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Communication
{
    //PLC返回执行代码
    public enum FGs_ERR_CODE
    {
        FGs_CARRY_OK = 0x20,            //写入成功
        FGs_CRC_ERR,                  //CRC校验失败
        FGs_LENTH_ERR,             //长度错误
        FGs_ADDRESS_ERR,           //元件寻址错误
        FGs_ADDRESS_TYPE_ERR,	   //地址种类错误
        FGs_DES_ERR,                //上载条形码密钥校验错误
        FGs_ADDRESS_BEYOND_ERR	   //地址越界错误(用于VZ偏移地址越界)
    }
    public class CommunicationDataDefine
    {
        /* FGs_Connect Macro Define
         * 
         */
        //读写命令定义
        public const byte FGS_READ = 0xFE;
        public const byte FGS_WRITE = 0xFF;
        //最多一次读写元件个数 ,以字节为长度
        public const byte MAX_ELEM_NUM = 0x20;
        //一包数据读写地址种类
        public const byte MAX_ADDRESS_TYPE = 0x06;
        //从站地址
        public const byte SLAVE_ADDRESS = 0x01;
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
