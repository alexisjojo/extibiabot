#include "StdAfx.h"
#include "Hook.h"
#include "ParserHook.h"
#include "PipeClient.h"
#include "Constants.h"

DWORD OldParsePacket = 0;	

struct TPacketStream
{
	LPVOID pBuffer;
	DWORD dwSize;
	DWORD dwPos;
};

typedef void TF_PARSER();
typedef int TF_GETNEXTPACKET();
 
TF_PARSER* TfParser = (TF_PARSER*)Consts::ptrFuncParser;
TF_GETNEXTPACKET* TfGetNextPacket = NULL;
TPacketStream* pRecvStream = (TPacketStream*)Consts::ptrAddRecStream;
PipeClient * pClient = new PipeClient();

int OnGetNextPacket()
{
	int iCmd = TfGetNextPacket();
	if(iCmd != -1)
	{
		if((pRecvStream->dwPos-1) == 8)
		{
			LPBYTE pWholePacket = (LPBYTE)pRecvStream->pBuffer + pRecvStream->dwPos - 1;
			DWORD dwPacketSize = pRecvStream->dwSize - pRecvStream->dwPos + 1;
			ParserHook::ParsePacket(pWholePacket, dwPacketSize);
		}
	}
	return iCmd;
}

ParserHook::ParserHook(DWORD baseAddress)
{
	//pClient = new PipeClient();
}

ParserHook::~ParserHook(void)
{

}

void ParserHook::ParsePacket(LPBYTE bytes, DWORD amount)
{
	pClient->SendToServer(bytes, amount);
}