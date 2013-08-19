#include "StdAfx.h"
#include "Hook.h"

#include "PacketHook.h"

//addresses
#define FUNC_PARSER 0x468BD0
#define CALL_GET_NEXT_PACKET 0x468C17
#define ADDR_RECV_STREAM 0x9DA5F4
 
//structure Tibia uses to work with buffers
struct TPacketStream
{
	LPVOID pBuffer;
	DWORD dwSize;
	DWORD dwPos;
};

 //define types of functions we want to use or hook
typedef void TF_PARSER();
typedef int TF_GETNEXTPACKET();
 
//pointers to call original function
TF_PARSER *TfParser = (TF_PARSER*)FUNC_PARSER;
TF_GETNEXTPACKET *TfGetNextPacket = NULL;
 
//pointer to Tibia packet stream
TPacketStream * pRecvStream = (TPacketStream*)ADDR_RECV_STREAM;
 

void ParsePacket(LPBYTE packet, DWORD packetSize)
{
	int i = 0;
}


//overriding original OnGetNextPacket() function
int OnGetNextPacket()
{
	int iCmd = TfGetNextPacket();
	if(iCmd != -1)
	{
		if((pRecvStream->dwPos-1) == 8)
		{
			LPBYTE pWholePacket = (LPBYTE)pRecvStream->pBuffer + pRecvStream->dwPos - 1;
			DWORD dwPacketSize = pRecvStream->dwSize - pRecvStream->dwPos + 1;
			ParsePacket(pWholePacket, dwPacketSize);
		}
	}
	return iCmd;
}

PacketHook::PacketHook(void)
{
	Hook* hook = new Hook();
	hook->HookCall(CALL_GET_NEXT_PACKET, (DWORD)&OnGetNextPacket, (LPDWORD)&TfGetNextPacket); 
}

PacketHook::~PacketHook(void)
{

}
