#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include <stdarg.h>

#include "libusb.h"
#include "usbdll.h"

/** \def SAMSOAR_PLC_VENDOR
 * the VENDOR id of the currently linking PLC Device
 */
#define SAMSOAR_PLC_VENDOR 0xffff
/** \def SAMSOAR_PLC_PRODUCT
 * the PRODUCT id of the currently linking PLC Device
 */
#define SAMSOAR_PLC_PRODUCT 0xffff

/** \def DEVICE_ACTIVE
 * Error code : happened when (device != NULL) and try to create a new one.
 */
#define DEVICE_ACTIVE 0x01
/** \def HANDLE_ACTIVE
 * Error code : happened when (handle != NULL) and try to create a new one.
 */
#define HANDLE_ACTIVE 0x02
/** \def DEVICE_NULL
 * Error code : happened when (device == NULL) and try to use it to do something.
 */
#define DEVICE_NULL 0x03
/** \def HANDLE_NULL
 * Error code : happened when (handle == NULL) and try to use it to do something.
 */
#define HANDLE_NULL 0x04
/** \def CANNOT_GET_DEVICE
 * Error code : happened when cannot found the specified VENDOR id and PRODUCT id of an accepted device 
 */
#define CANNOT_GET_DEVICE 0x05
/** \def CANNOT_GET_HANDLE
 * Error code : happened when cannot create a new handle from device.
 */
#define CANNOT_GET_HANDLE 0x06
/** \def CANNOT_TRANSFER
 * Error code : happened when cannot transfer data between PC and PLC Device.
 */
#define CANNOT_TRANSFER 0x07
/** \def CANNOT_SEND_COMMAND
 * Error code : happened when cannot send a request to PLC Device.
 */
#define CANNOT_SEND_COMMAND 0x08
/** \def CANNOT_RECV_REPORT
 * Error code : happened when cannot receive a report from PLC Device.
 */
#define CANNOT_RECV_REPORT 0x09
/** \def ERROR_RECV_REPORT
 * Error code : happened when cannot transfer data between PC and PLC Device.
 */
#define ERROR_RECV_REPORT 0x0a
/** \def OUT_OF_LENGTH_LIMIT
 * Error code : happened when try to transfer a data package above the length limit. 
 */
#define OUT_OF_LENGTH_LIMIT 0x0b
/** \def CANNOT_FOUND_FILE
 * Error code : happened when cannot found the file from the targeted file path.
 */
#define CANNOT_FOUND_FILE 0x0c

/** \def CANNOT_TRANSFER
 * The allowed maxinum time when transfer.
 */
#define TIMEOUT 1000
/** \def RETRY_MAX
 * The allowed maxinum retry number when transfer error happen.
 */
#define RETRY_MAX 8

/** \var devicelist
 * The USB device list for PC.
 */
static libusb_device **devicelist;
/** \var device
 * The current device to deal with.
 */
static libusb_device *device = NULL;
/** \var detail
 * The infomation of current device (include VENDOR and PRODUCT id)
 */
static struct libusb_device_descriptor detail;
/** \var handle
 * The handle of current device.
 */
static libusb_device_handle *handle = NULL;

/** \name open
 * Check all of USB devices, find one which belongs to SamSoar PLC.
 * Seem it as the current device, create the device handle corrospond to it. 
 * 
 * \return Error code when it happen, otherwise return 0 which means OK.
 */
EXPORT int open()
{
	if (device != NULL)
	{
		return DEVICE_ACTIVE;
	}
	if (handle != NULL)
	{
		return HANDLE_ACTIVE;
	}
	ssize_t ct = libusb_get_device_list(NULL, &devicelist);
	device = NULL;
	int i = 0;
	for (i = 0 ; i < ct ; i++)
	{
		libusb_get_device_descriptor(devicelist[i], &detail);
		if (detail.idVendor == SAMSOAR_PLC_VENDOR &&
			detail.idProduct == SAMSOAR_PLC_PRODUCT)
		{
			device = devicelist[i];
			break;
		}
	}
	if (device == NULL)
	{
		return CANNOT_GET_DEVICE;
	}
	int err = libusb_open(device, &handle);
	if (err != 0)
	{
		device = NULL;
		handle = NULL;
		return CANNOT_GET_HANDLE;
	}
	return 0;
}

/** \name close
 * Close the current device and handle, assign them to NULL
 * 
 * \return Error code when it happen, otherwise return 0 which means OK.
 */
EXPORT int close()
{
	if (device == NULL)
	{
		return DEVICE_NULL;
	}
	if (handle == NULL)
	{
		return HANDLE_NULL;
	}
	libusb_close(handle);
	device = NULL;
	handle = NULL;
	return 0;
}

/** \name transfer
 * Send / Receive a series of data to / from the PLC Device.
 *
 * \param data		the base address of data
 * \param length 	the length of data
 *
 * \return Error code when it happen, otherwise return 0 which means OK.  
 */
EXPORT int transfer(uint8_t* data, int len)
{
	if (device == NULL)
	{
		return DEVICE_NULL;
	}
	if (handle == NULL)
	{
		return HANDLE_NULL;
	}
	int retrycount = 0;		/* Retry count */
	int pipe;				/* Pipe number */
	int transferred;		/* Transferred size of sent data*/
	uint8_t endpoint = 0;	/* Endpoint */
	do /* Start of transfer loop */
	{
		pipe = libusb_bulk_transfer(handle, endpoint, data, len, &transferred, TIMEOUT);
		// Clear the endpoint halt
		if (pipe == LIBUSB_ERROR_PIPE)
		{
			libusb_clear_halt(handle, endpoint);
		}
	}
	while ((pipe == LIBUSB_ERROR_PIPE) && (++retrycount <= RETRY_MAX));
	// Retry it over the maxinum? 
	if (retrycount > RETRY_MAX)
	{
		return CANNOT_TRANSFER;
	}
	return 0;
}

/** \var plc_word_size
 * the size (number of bytes) of a WORD integer
 * it represent the bit number of a PLC Device.
 * As normally, 16-bits : 2-bytes WORD, 32-bits : 4-bytes WORD.  
 */
int plc_word_size = 4;

/** \name set the config message of it
 * It influence the stategy (data format..) of USB commucation. 
 *
 * \param bit		the number of bit (of a WORD integer)
 */
EXPORT void config(int bit)
{
	if (bit >= 32)
	{
		plc_word_size = 4;
	}
	else if (bit >= 16)
	{
		plc_word_size = 2;
	}
	else
	{
		plc_word_size = 1;
	}
}

/** \def FGS_READ
 * command code : read
 */
#define FGS_READ 0xfe
/** \def FGS_WRITE
 * command code : write
 */
#define FGS_WRITE 0xff

/** \def MAX_ADDRESS_TYPE
 * the maxinum number of address types
 */ 
#define MAX_ADDRESS_TYPE 0x06
/** \def MAX_ELEMENT_NUMBER
 * the maxinum element number for a data package
 */
#define MAX_ELEMENT_NUMBER 32

/** \def ADDRESS_VZ
 * Package type: VZ
 */
#define ADDRESS_VZ 0x00
/** \def ADDRESS_NORMAL
 * Package type : NORMAL
 */
#define ADDRESS_NORMAL 0x01

/** \def ADDRESS_TYPE_X
 * Address type : X Bit
 */
#define ADDRESS_TYPE_X 0x00
/** \def ADDRESS_TYPE_Y
 * Address type : Y Bit
 */
#define ADDRESS_TYPE_Y 0x01
/** \def ADDRESS_TYPE_M
 * Address type : M Bit
 */
#define ADDRESS_TYPE_M 0x02
/** \def ADDRESS_TYPE_S
 * Address type : S Bit
 */
#define ADDRESS_TYPE_S 0x03
/** \def ADDRESS_TYPE_C
 * Address type : C Bit
 */
#define ADDRESS_TYPE_C 0x04
/** \def ADDRESS_TYPE_T
 * Address type : T Bit
 */
#define ADDRESS_TYPE_T 0x05
/** \def ADDRESS_TYPE_AI
 * Address type : AI Word
 */
#define ADDRESS_TYPE_AI 0x06
/** \def ADDRESS_TYPE_AO
 * Address type : AO Word
 */
#define ADDRESS_TYPE_AO 0x07
/** \def ADDRESS_TYPE_D
 * Address type : D Word
 */
#define ADDRESS_TYPE_D 0x08
/** \def ADDRESS_TYPE_V
 * Address type : V Word
 */
#define ADDRESS_TYPE_V 0x09
/** \def ADDRESS_TYPE_Z
 * Address type : Z Word
 */
#define ADDRESS_TYPE_Z 0x10
/** \def ADDRESS_TYPE_CV
 * Address type : CV Word
 */
#define ADDRESS_TYPE_CV 0x11
/** \def ADDRESS_TYPE_TV
 * Address type : TV Word
 */
#define ADDRESS_TYPE_TV 0x12
/** \def ADDRESS_TYPE_CV32
 * Address type : CV32 DoubleWord
 */
#define ADDRESS_TYPE_CV32 0x13

/** \enum FGS Info Code
 * the OK flag and Error code of FGS
 */
enum eFGsInfoCode
{
	FGs_CARRY_OK = 0x20,		/* All OK */
	FGs_CRC_ERROR,				/* CRC checking failed */
	FGs_LENGTH_ERROR,			/* Data length error */
	FGs_ADDRESS_ERROR,			/* Data address error */
	FGs_ADDRESS_TYPE_ERROR,		/* Address type error */
	FGs_DES_ERROR,				/* Decodeing key error when upload */
	FGs_ADDRESS_BEYOND_ERROR	/* Addrees beyond the limit (use to V/Z verify address) */
};

/** \typedef FGs_INFO_CODE
 */
typedef enum eFGsInfoCode FGs_INFO_CODE;

/** \struct data_package
 * The header (or whole) of a data package
 */
struct data_package
{
	uint8_t address_base; /* Address type */
	uint8_t length;		  /* Package data length */
	uint8_t offset_low;   /* The lower half of offset */
	uint8_t offset_high;  /* The upper half of offset */
};
/** \struct vz_package
 * The header (or whole) of a vz data package
 */
struct vz_package
{
	uint8_t address_base;  /* Address type */
	uint8_t offset_low;    /* Address offset */
};
/** \struct read_command1
 * The request command to read a couple series of data.
 */
struct read_command1
{
	uint8_t plc_port;				/* PLC port (0x01 as default) */
	uint8_t command;				/* Command code : 0xfe as read command */
	uint8_t address_type;			/* Package type : 0x00 as normal type */
	struct data_package data1;		/* package 1 */
	struct data_package data2;		/* package 2 */
	uint8_t crc_low;				/* The lower half of CRC Code */ 
	uint8_t crc_high;				/* The upper half of CRC Code */
};
/** \struct write_command1
 * The request command to write a couple series of data.
 */
struct write_command1
{
	uint8_t plc_port;				/* PLC port (0x01 as default) */
	uint8_t command;				/* Command code : 0xff as write command */
	uint8_t address_type;			/* Package type : 0x00 as normal type */
									/* Remains data zone */
};
/** \struct read_command2
 * The request command to read the series of data which's address is verified by V/Z value
 */
struct read_command2
{
	uint8_t plc_port;				/* PLC port (0x01 as default) */
	uint8_t command;				/* Command code : 0xfe as read command */
	uint8_t address_type;			/* Package type : 0x01 as VZ type */
	struct data_package data1;		/* Data package */
	struct vz_package data2;		/* VZ package */
	uint8_t crc_low;				/* The lower half of CRC Code */ 
	uint8_t crc_high;				/* The upper half of CRC Code */
};

/** \struct write_command2
 * The request command to write the series of data which's address is verified by V/Z value
 */
struct write_command2
{
	uint8_t plc_port;				/* PLC port (0x01 as default) */
	uint8_t command;				/* Command code : 0xff as write command */
	uint8_t address_type;			/* Package type : 0x01 as VZ type */
	struct vz_package data2;		/* VZ package */
									/* Remains data zone */
};

/** \struct read_report1
 * The report message to read a couple series of data.
 */
struct read_report1
{
	uint8_t plc_port;				/* PLC port (0x01 as default) */
	uint8_t command;				/* Command code : 0xfe as read command */
	uint8_t address_type;			/* Package type or FGs Info Code*/
};

/** \struct write_report1
 * The report message to write a couple series of data.
 */
struct write_report1
{
	uint8_t plc_port;				/* PLC port (0x01 as default) */
	uint8_t command;				/* Command code : 0xff as write command */
	uint8_t address_type;			/* FGs Info Code */
};

/** \struct read_report2
 * The report message to read the series of data which's address is verified by V/Z value
 */
struct read_report2
{
	uint8_t plc_port;				/* PLC port (0x01 as default) */
	uint8_t command;				/* Command code : 0xfe as read command */
	uint8_t address_type;			/* Package type or FGs Info Code*/
};

/** \struct write_report2
 * The report message to write the series of data which's address is verified by V/Z value
 */
struct write_report2
{
	uint8_t plc_port;				/* PLC port (0x01 as default) */
	uint8_t command;				/* Command code : 0xff as write command */
	uint8_t address_type;			/* FGs Info Code */
};

/** \var _rcmd1
 * the memory of read command 1
 */
static struct read_command1 _rcmd1;
/** \var _rcmd2
 * the memory of read command 1
 */
static struct read_command2 _rcmd2;
/** \var rcmd1
 * the pointer of read command 1
 */
static struct read_command1 *rcmd1 = &_rcmd1;
/** \var rcmd2
 * the pointer of read command 2
 */
static struct read_command2 *rcmd2 = &_rcmd2;
/** \var _wcmd1
 * the memory of write command 1
 */
static uint8_t _wcmd1[1024];
/** \var _wcmd2
 * the memory of write command 2
 */
static uint8_t _wcmd2[1024];
/** \var wcmd1
 * the pointer of write command 1
 */
static struct write_command1 *wcmd1 = (struct write_command1*)(&_wcmd1[0]);
/** \var wcmd2
 * the pointer of write command 2
 */
static struct write_command2 *wcmd2 = (struct write_command2*)(&_wcmd2[0]);
/** \var _rrpt1
 * the memory of read report 1
 */
static uint8_t _rrpt1[1024];
/** \var _rrpt2
 * the memory of read report 2
 */
static uint8_t _rrpt2[1024];
/** \var rrpt1
 * the pointer of read report 1
 */
static struct read_report1 *rrpt1 = (struct read_report1*)(&_rrpt1[0]);
/** \var rrpt2
 * the pointer of read report 2
 */
static struct read_report2 *rrpt2 = (struct read_report2*)(&_rrpt2[0]);
/** \var _wrpt1
 * the memory of write report 1
 */
static struct write_report1 _wrpt1;
/** \var _wrpt2
 * the memory of write report 2
 */
static struct write_report2 _wrpt2;
/** \var wrpt1
 * the pointer of write report 1
 */
static struct write_report1 *wrpt1 = &_wrpt1;
/** \var wrpt2
 * the pointer of write report 1
 */
static struct write_report2 *wrpt2 = &_wrpt2;

/** \name _get_unit_length
 * Get the number of bytes of the address type
 *
 * \param 	address_type 	Address type
 * \return 					The result
 */
int _get_unit_length(int address_type)
{
	switch (address_type)
	{
		case ADDRESS_TYPE_X:
		case ADDRESS_TYPE_Y:
		case ADDRESS_TYPE_M:
		case ADDRESS_TYPE_S:
		case ADDRESS_TYPE_T:
		case ADDRESS_TYPE_C:
			return 1;
		case ADDRESS_TYPE_D:
		case ADDRESS_TYPE_CV:
		case ADDRESS_TYPE_TV:
		case ADDRESS_TYPE_AI:
		case ADDRESS_TYPE_AO:
		case ADDRESS_TYPE_Z:
		case ADDRESS_TYPE_V:
			return plc_word_size;
		case ADDRESS_TYPE_CV32:
			return plc_word_size * 2;
		default:
			return 1;
	}
}

/** \name _get_crc
 * Get the CRC code of the series of data 
 *
 * \param 	data	Base address
 * \param	length 	Data Length (pre byte)
 * \param	low		Return the lower half of result
 * \param 	high	Return the upper half of result
 */
int _get_crc
(
	uint8_t*	data,
	int			length,
	uint8_t*	low,
	uint8_t*	high
)
{
	// Make sure that ((crct<<2)+crc)%crcp == 0
	int crct = 0;
	int crcp = 60000;
	int i = 0;
	// Calculate (crct<<2)
	for (i = 0 ; i < length; i++)
	{
		crct <<= 8;
		crct += data[i];
		crct %= crcp;
	}
	crct <<= 8;
	crct %= data[i];
	crct <<= 8;
	crct %= data[i];
	// Calculate crc and return
	return crct == 0 ? 0 : (crcp - crct);
}

/** \name _read
 * Read a series of register data which's length is not allowed to go over 255.
 *
 * \param	address_type	Address type
 * \param	offset			Address offset
 * \param	length			Data length (per byte)
 * \param	vz_type			VZ type (0 means not use)
 * \param	vz_offset		VZ Address offset
 * \param	input			Register data memory to write
 * \param	vz_input		VZ_Register data memory to write
 */
int _read
(
	uint8_t		address_type,
	uint16_t	offset,
	uint16_t	length,
	uint8_t		vz_type,
	uint8_t		vz_offset,
	uint8_t**	input,
	uint8_t*	vz_input
)
{
	uint16_t div_length = 0; 		/* The left halt length if it is normal read command and divide the data into couple of parts */
	uint8_t* rpt 		= NULL;	 	/* Report pointer */
	uint32_t rpt_length = 0; 		/* Report data length */
	// vz_type is not 0, means V/Z command 
	if (vz_type)
	{
		// out of length limit?
		if (length >= 256) 
		{
			return OUT_OF_LENGTH_LIMIT;
		}
		// set the read command struct
		rcmd2->plc_port = 0x01;
		rcmd2->command = FGS_READ;
		rcmd2->address_type = ADDRESS_VZ;
		// data package 1 (normal)
		rcmd2->data1.address_base = address_type;
		rcmd2->data1.length = length / _get_unit_length(address_type);		/* notice that it is register count length */
		rcmd2->data1.offset_low = offset & 0xff;							
		rcmd2->data1.offset_high = (offset >> 8) & 0xff;
		// data package 2 (vz)
		rcmd2->data2.address_base = vz_type;
		rcmd2->data2.offset_low = vz_offset & 0xff;
		// get CRC code
		_get_crc((uint8_t*)rcmd2, sizeof(struct read_command2) - 2,
			&(rcmd2->crc_low), &(rcmd2->crc_high));
		// send the command
		if (transfer((uint8_t*)rcmd2, sizeof(struct read_command2)))
		{
			return CANNOT_SEND_COMMAND;
		}
		// receive the report
		if (transfer((uint8_t*)rrpt2, sizeof(_rrpt2)))
		{
			return CANNOT_RECV_REPORT;
		}
		// get the address type, which is FGs info code?
		if (rrpt2->address_type != ADDRESS_VZ)
		{
			return ERROR_RECV_REPORT | ((int)(rrpt2->address_type) << 8);
		}
		// get the read report and data values inside of it. 
		else
		{
			// seek it to the end of report header
			rpt = (uint8_t*)rrpt2;
			rpt += sizeof(struct read_report2);
			// get the data length (per byte) and write
			rpt_length = _get_unit_length(*rpt++);
			rpt_length *= *rpt++;
			memcpy(*input, rpt, rpt_length);
			// move the input stream and report stream next
			*input += rpt_length;
			rpt += rpt_length;
			// write the vz data
			rpt_length = _get_unit_length(*rpt++);
			memcpy(vz_input, rpt, rpt_length);
			rpt += rpt_length;
		}
	}
	// normal read command
	else
	{
		if (length >= 511)
		{
			return OUT_OF_LENGTH_LIMIT;
		}
		// Calculate the divide length
		div_length = length / (_get_unit_length(address_type) * 2);
		rcmd1->plc_port = 0x01;
		rcmd1->command = FGS_READ;
		rcmd1->address_type = ADDRESS_NORMAL;
		// data package 1
		rcmd1->data1.address_base = address_type;
		rcmd1->data1.length = div_length;
		rcmd1->data1.offset_low = offset & 0xff;
		rcmd1->data1.offset_high = (offset >> 8) & 0xff;
		// data package 2
		rcmd1->data2.address_base = address_type;
		rcmd1->data2.length = (length / _get_unit_length(address_type)) - div_length;
		rcmd1->data2.offset_low = (offset + div_length) & 0xff;
		rcmd1->data2.offset_high = ((offset + div_length) >> 8) & 0xff;
		// get CRC code
		_get_crc((uint8_t*)rcmd2, sizeof(struct read_command1) - 2,
			&(rcmd1->crc_low), &(rcmd1->crc_high));
		// send the command, receive the report, get the info code
		if (transfer((uint8_t*)rcmd1, sizeof(struct read_command1)))
		{
			return CANNOT_SEND_COMMAND;
		}
		if (transfer((uint8_t*)rrpt1, sizeof(_rrpt1)))
		{
			return CANNOT_RECV_REPORT;
		}
		if (rrpt1->address_type != ADDRESS_VZ)
		{
			return ERROR_RECV_REPORT | ((int)(rrpt1->address_type) << 8);
		}
		else
		{
			// move to data zone
			rpt = (uint8_t*)rrpt1;
			rpt += sizeof(struct read_report1);
			// read left part
			rpt_length = _get_unit_length(*rpt++);
			rpt_length *= *rpt++;
			memcpy(*input, rpt, rpt_length);
			*input += rpt_length;
			rpt += rpt_length;
			// read right part
			rpt_length = _get_unit_length(*rpt++);
			rpt_length *= *rpt++;
			memcpy(*input, rpt, rpt_length);
			*input += rpt_length;
			rpt += rpt_length;
		}
	}
	return 0;
}

/** \name _write
 * Write a series of register data which's length is not allowed to go over 255.
 *
 * \param	address_type	Address type
 * \param	offset			Address offset
 * \param	length			Data length (per byte)
 * \param	vz_type			VZ type (0 means not use)
 * \param	vz_offset		VZ Address offset
 * \param	output			Register data memory to read
 * \param	vz_output		VZ_Register data memory to read
 */
int _write
(
	uint8_t 	address_type,
	uint16_t 	offset,
	uint16_t 	length,
	uint8_t 	vz_type,
	uint8_t 	vz_offset,
	uint8_t** 	output,
	uint8_t* 	vz_output
)
{
	uint16_t 	div_length 	= 0; 		/* The left halt length if it is normal read command and divide the data into couple of parts */
	uint8_t* 	cmd 	  	= NULL;	 	/* Report pointer */
	uint8_t 	cmd_length 	= 0; 		/* Report data length */
	// vz_type is not 0, means V/Z command 
	if (vz_type)
	{
		// out of length limit?
		if (length >= 256) 
		{
			return OUT_OF_LENGTH_LIMIT;
		}
		// set the write command
		wcmd2->plc_port = 0x01;
		wcmd2->command = FGS_WRITE;
		wcmd2->address_type = ADDRESS_VZ;
		// move to data zone
		cmd = (uint8_t*)wcmd1;
		cmd += sizeof(struct write_command2);
		// data package 1
		*cmd++ = vz_type;
		*cmd++ = vz_offset;
		// data package 2
		*cmd++ = address_type;
		*cmd++ = length / _get_unit_length(address_type);
		*cmd++ = offset & 0xff;
		*cmd++ = (offset >> 8) & 0xff;
		cmd_length = length;
		memcpy(cmd, *output, cmd_length);
		*output += cmd_length;
		cmd += cmd_length;
		// the length of whole command struct
		cmd_length = (int)cmd - (int)wcmd2;
		// get CRC code
		_get_crc((uint8_t*)wcmd2, cmd_length, cmd, cmd+1);
		cmd_length += 2;
		// USB transfer
		if (transfer((uint8_t*)wcmd2, cmd_length))
		{
			return CANNOT_SEND_COMMAND;
		}
		if (transfer((uint8_t*)wrpt2, sizeof(struct write_report2)))
		{
			return CANNOT_RECV_REPORT;
		}
		if (wrpt2->address_type != FGs_CARRY_OK)
		{
			return ERROR_RECV_REPORT | ((int)(wrpt2->address_type) << 8);
		}
	}
	else
	{
		// out of limit?
		if (length >= 511)
		{
			return OUT_OF_LENGTH_LIMIT;
		}
		div_length = length / (_get_unit_length(address_type) * 2);
		wcmd1->plc_port = 0x00;
		wcmd1->command = FGS_WRITE;
		wcmd1->address_type = ADDRESS_NORMAL;
		// move to data zone
		cmd = (uint8_t*)wcmd1;
		cmd += sizeof(struct write_command1);
		// data package 1
		*cmd = address_type;
		cmd_length = _get_unit_length(*cmd++);
		*cmd++ = div_length;
		*cmd++ = offset & 0xff;
		*cmd++ = (offset >> 8) & 0xff;
		cmd_length *= div_length;
		memcpy(cmd, *output, cmd_length);
		*output += cmd_length;
		cmd += cmd_length;
		// data package 2
		*cmd = address_type;
		*cmd++ = length / _get_unit_length(address_type) - div_length;
		*cmd++ = (offset + div_length) & 0xff;
		*cmd++ = ((offset + div_length) >> 8) & 0xff;
		cmd_length = length - cmd_length;
		memcpy(cmd, *output, cmd_length);
		*output += cmd_length;
		cmd += cmd_length;
		// get SRC code
		cmd_length = (int)cmd - (int)wcmd1;
		_get_crc((uint8_t*)wcmd1, cmd_length,
			cmd, cmd+1);
		cmd_length += 2;
		// USB transfers
		if (transfer((uint8_t*)wcmd1, cmd_length))
		{
			return CANNOT_SEND_COMMAND;
		}
		if (transfer((uint8_t*)wrpt1, sizeof(struct write_report1)))
		{
			return CANNOT_RECV_REPORT;
		}
		if (wrpt1->address_type != FGs_CARRY_OK)
		{
			return ERROR_RECV_REPORT | ((int)(wrpt1->address_type) << 8);
		}
	}
	return 0;
}

/** \var _address_type
 * Parameter for read \ write
 */
static uint8_t _address_type = 0;
/** \var _offset
 * Parameter for read \ write
 */
static uint8_t _offset = 0;
/** \var _length
 * Parameter for read \ write
 */
static uint8_t _length = 0;
/** \var _vz_type
 * Parameter for read \ write
 */
static uint8_t _vz_type = 0;
/** \var _vz_offset
 * Parameter for read \ write
 */
static uint8_t _vz_offset = 0;
/** \var _vz_value 
 * Parameter for read \ write
 */
static uint8_t _vz_value = 0;

/** \name _analysis_name
 * Analysis the register name and get the parameters to read / write
 *
 * \param 	name	Register name
 * \param	length	Data length
 */
void _analysis_name
(
	char* 	name,
	int 	length
)
{
	// first char of string name
	switch (name[0])
	{
	case 'X':
		_address_type = ADDRESS_TYPE_X;
		// move to offset and v/z
		name += 1;
		break;
	case 'Y':
		_address_type = ADDRESS_TYPE_Y;
		name += 1;
		break;
	case 'S':
		_address_type = ADDRESS_TYPE_S;
		name += 1;
		break;
	case 'M':
		_address_type = ADDRESS_TYPE_M;
		name += 1;
		break;
	case 'T':
		// T or TV ? look at second char
		switch (name[1])
		{
		case 'V':
			_address_type = ADDRESS_TYPE_TV;
			name += 2;
			break;
		default:
			_address_type = ADDRESS_TYPE_T;
			name += 1;
			break;
		}
		break;
		// C ot CV?
	case 'C':
		switch (name[1])
		{
		case 'V':
			_address_type = ADDRESS_TYPE_CV;
			name += 2;
			break;
		default:
			_address_type = ADDRESS_TYPE_C;
			name += 1;
			break;
		}
		break;
	case 'D':
		_address_type = ADDRESS_TYPE_D;
		name += 1;
		break;
	case 'A':
		// AI or AO?
		switch (name[1])
		{
		case 'I':
			_address_type = ADDRESS_TYPE_AI;
			name += 2;
			break;
		case 'O':
			_address_type = ADDRESS_TYPE_AO;
			name += 2;
			break;
		}
		break;
	case 'V':
		_address_type = ADDRESS_TYPE_V;
		name += 1;
		break;
	case 'Z':
		_address_type = ADDRESS_TYPE_Z;
		name += 1;
		break;
	default:
		break;
	}
	_length = length;
	char _vz_char = '\0';
	// read offset and v/z
	sscanf(name, "%d%c%d", &_offset, &_vz_char, &_vz_offset);
	// has vz offset? Y or Z? 
	switch (_vz_char)
	{
		case 'V':
			_vz_type = ADDRESS_TYPE_V;
			break;
		case 'Z':
			_vz_type = ADDRESS_TYPE_Z;
			break;
		default:
			_vz_type = 0;
			break;
	}
}

/** \name read
 * Read the register data
 *
 * \param	name	Register name
 * \param 	length	Length to read (per byte)
 * \param	input	Memory to read
 */
EXPORT int read
(
	char* 		name,
	int 		length,
	uint8_t* 	input
)
{
	_analysis_name(name, length);
	
	int blocknum = _length >> 8;
	int blockrem = _length & 0xFF;
	int i = 0;
	int ret = 0;
	for (i = 0 ; i < blocknum ; i++)
	{
		ret = _read(_address_type, _offset + (i << 8), 255, 
			_vz_type, _vz_offset, &input, &_vz_value);
		if (ret != 0) return ret;
	}
	if (blockrem > 0)
	{
		ret = _read(_address_type, _offset + (blocknum << 8), blockrem, 
			_vz_type, _vz_offset, &input, &_vz_value);
		if (ret != 0) return ret;
	}
	return 0;
}

/** \name write
 * Write the register data
 *
 * \param	name	Register name
 * \param 	length	Length to read (per byte)
 * \param	input	Memory to read
 */
EXPORT int write
(
	char* 		name,
	int 		length,
	uint8_t* 	output
)
{
	_analysis_name(name, length);
	
	int blocknum = _length >> 8;
	int blockrem = _length & 0xFF;
	int i = 0;
	int ret = 0;
	for (i = 0 ; i < blocknum ; i++)
	{
		ret = _write(_address_type, _offset + (i << 8), 255, 
			_vz_type, _vz_offset, &output, &_vz_value);
		if (ret != 0) return ret;
	}
	if (blockrem > 0)
	{
		ret = _write(_address_type, _offset + (blocknum << 8), blockrem, 
			_vz_type, _vz_offset, &output, &_vz_value);
		if (ret != 0) return ret;
	}
	return 0;
}

/** \name read16
 * Read the 16-bit integer
 *
 * \param	name	Register name
 * \param 	length	Length to read (number)
 * \param	input	Memory to read
 */
EXPORT int read16
(
	char*		name,
	int			length,
	uint16_t*	input
)
{
	return read(name, length<<1, (uint8_t*)input);
}

/** \name read32
 * Read the 32-bit integer
 *
 * \param	name	Register name
 * \param 	length	Length to read (number)
 * \param	input	Memory to read
 */
EXPORT int read32
(
	char*		name,
	int			length,
	uint32_t*	input
)
{
	return read(name, length<<2, (uint8_t*)input);
}

/** \name read64
 * Read the 64-bit integer
 *
 * \param	name	Register name
 * \param 	length	Length to read (number)
 * \param	input	Memory to read
 */
EXPORT int read64
(
	char*		name,
	int			length,
	uint64_t*	input
)
{
	return read(name, length<<3, (uint8_t*)input);
}

/** \name read32f
 * Read the 32-bit float
 *
 * \param	name	Register name
 * \param 	length	Length to read (number)
 * \param	input	Memory to read
 */
EXPORT int read32f
(
	char*		name,
	int			length,
	float*		input
)
{
	return read(name, length<<2, (uint8_t*)input);
}

/** \name read64f
 * Read the 64-bit float (double)
 *
 * \param	name	Register name
 * \param 	length	Length to read (number)
 * \param	input	Memory to read
 */
EXPORT int read64f
(
	char*		name,
	int			length,
	double*		input
)
{
	return read(name, length<<3, (uint8_t*)input);
}

/** \name write16
 * Write the 16-bit integer
 *
 * \param	name	Register name
 * \param 	length	Length to read (number)
 * \param	input	Memory to read
 */
EXPORT int write16
(
	char*		name,
	int			length,
	uint16_t*	output
)
{
	return write(name, length<<1, (uint8_t*)output);
}

/** \name write32
 * Write the 32-bit integer
 *
 * \param	name	Register name
 * \param 	length	Length to read (number)
 * \param	input	Memory to read
 */
EXPORT int write32
(
	char*		name,
	int			length,
	uint32_t*	output
)
{
	return write(name, length<<2, (uint8_t*)output);
}

/** \name write64
 * Write the 64-bit integer
 *
 * \param	name	Register name
 * \param 	length	Length to read (number)
 * \param	input	Memory to read
 */
EXPORT int write64
(
	char*		name,
	int			length,
	uint64_t*	output
)
{
	return write(name, length<<3, (uint8_t*)output);
}

/** \name write32f
 * Write the 32-bit float
 *
 * \param	name	Register name
 * \param 	length	Length to read (number)
 * \param	input	Memory to read
 */
EXPORT int write32f
(
	char*		name,
	int			length,
	float*		output
)
{
	return write(name, length<<2, (uint8_t*)output);
}


/** \name write64f
 * Write the 64-bit float (double)
 *
 * \param	name	Register name
 * \param 	length	Length to read (number)
 * \param	input	Memory to read
 */
EXPORT int write64f
(
	char*		name,
	int			length,
	double*		output
)
{
	return write(name, length<<3, (uint8_t*)output);
}

static uint8_t data_bin[1<<20];
static uint8_t data_pro[1<<20];

int _send_upload_request()
{
	return 0;
}

int _send_download_request()
{
	return 0;
}

void _xml_encode
(
	char* xml_text,
	uint8_t* data_pro,
	int* length_pro
)
{
}

void _xml_decode
(
	char* xml_text,
	uint8_t* data_pro
)
{
}

EXPORT int upload
(
	char* file_bin,
	char* file_pro
)
{
	int ret = 0;
	ret = _send_upload_request();
	if (ret != 0)
	{
		return ret;
	}
	
	return 0;
}

EXPORT int download
(
	char* file_bin, 
	char* file_pro
)
{
	int ret = 0;
	ret = _send_download_request();
	if (ret != 0)
	{
		return ret;
	}
	
	FILE* fbin = fopen(file_bin, "rb");
	FILE* fpro = fopen(file_pro, "rb");
	if (fbin == NULL || fpro == NULL)
	{
		return CANNOT_FOUND_FILE;
	}
	fseek(fbin, 0, SEEK_END);
	int flbin = ftell(fbin);
	fseek(fbin, 0, SEEK_SET);
	fseek(fpro, 0, SEEK_END);
	int flpro = ftell(fpro);
	fseek(fpro, 0, SEEK_SET);
	fread(data_bin, sizeof(uint8_t), flbin, fbin);
	fread(data_pro, sizeof(uint8_t), flpro, fpro);
	fclose(fbin);
	fclose(fpro);
	
	ret = transfer(data_bin, flbin);
	if (ret != 0) return ret;
	ret = transfer(data_pro, flpro);
	if (ret != 0) return ret;
	return 0;
}






