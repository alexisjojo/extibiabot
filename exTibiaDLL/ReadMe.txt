========================================================================
    DYNAMIC LINK LIBRARY : exTibiaDLL Project Overview
========================================================================

AppWizard has created this exTibiaDLL DLL for you.

This file contains a summary of what you will find in each of the files that
make up your exTibiaDLL application.


exTibiaDLL.vcxproj
    This is the main project file for VC++ projects generated using an Application Wizard.
    It contains information about the version of Visual C++ that generated the file, and
    information about the platforms, configurations, and project features selected with the
    Application Wizard.

exTibiaDLL.vcxproj.filters
    This is the filters file for VC++ projects generated using an Application Wizard. 
    It contains information about the association between the files in your project 
    and the filters. This association is used in the IDE to show grouping of files with
    similar extensions under a specific node (for e.g. ".cpp" files are associated with the
    "Source Files" filter).

exTibiaDLL.cpp
    This is the main DLL source file.

	When created, this DLL does not export any symbols. As a result, it
	will not produce a .lib file when it is built. If you wish this project
	to be a project dependency of some other project, you will either need to
	add code to export some symbols from the DLL so that an export library
	will be produced, or you can set the Ignore Input Library property to Yes
	on the General propert page of the Linker folder in the project's Property
	Pages dialog box.

/////////////////////////////////////////////////////////////////////////////
Other standard files:

StdAfx.h, StdAfx.cpp
    These files are used to build a precompiled header (PCH) file
    named exTibiaDLL.pch and a precompiled types file named StdAfx.obj.

/////////////////////////////////////////////////////////////////////////////
Other notes:

AppWizard uses "TODO:" comments to indicate parts of the source code you
should add to or customize.
















/////////////////////////////////////////////////////////////////////////////
//addresses
#define FUNC_PARSER				0x45B810 //8.50
#define CALL_GET_NEXT_PACKET	0x45B845 //8.50 
#define ADDR_RECV_STREAM		0x78BF24 //8.50 
 
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
 
//flag indicates that we are sending to client
BOOL fSendingToClient = FALSE;




void HookCall(DWORD dwCallAddress, DWORD dwNewAddress, LPDWORD pOldAddress)
{
	DWORD dwOldProtect, dwNewProtect, dwOldCall, dwNewCall;
	BYTE call[5] = {0xE8, 0x00, 0x00, 0x00, 0x00};
 
	dwNewCall = dwNewAddress - dwCallAddress - 5;
	memcpy(&call[1], &dwNewCall, 4);
 
	VirtualProtectEx(GetCurrentProcess(), (LPVOID)(dwCallAddress), 5, PAGE_READWRITE, &dwOldProtect);
	if(pOldAddress)
	{
		memcpy(&dwOldCall, (LPVOID)(dwCallAddress+1), 4);
		*pOldAddress = dwCallAddress + dwOldCall + 5;
	}
	memcpy((LPVOID)(dwCallAddress), &call, 5);
	VirtualProtectEx(GetCurrentProcess(), (LPVOID)(dwCallAddress), 5, dwOldProtect, &dwNewProtect);
}

int OnGetNextPacket()
{
	int iCmd = TfGetNextPacket();
	if(iCmd != -1)
	{
		if((pRecvStream->dwPos-1) == 8)
		{
			LPBYTE pWholePacket = (LPBYTE)pRecvStream->pBuffer + pRecvStream->dwPos - 1;
			DWORD dwPacketSize = pRecvStream->dwSize - pRecvStream->dwPos + 1;
 
			//now you can process complete packet
			//example: MyParsePacket(pWholePacket, dwPacketSize);
		}
	}
	return iCmd;
}

void SendToClient(LPBYTE pBuffer, DWORD dwSize)
{
	//turn on new behavior for GetNextPacket
	fSendingToClient = TRUE;
 
	//store Tibia recv stream
	TPacketStream StreamHolder = *pRecvStream;
 
	//point the stream to our buffer with packet
	pRecvStream->pBuffer = pBuffer;
	pRecvStream->dwSize = dwSize;
	pRecvStream->dwPos = 0;
 
	//call Tibia packet parser directly
	TfParser();
 
	//restore Tibia recv stream
	*pRecvStream = StreamHolder;
 
	//turn off new behavior for GetNextPacket
	fSendingToClient = FALSE;
}
//we hook a single CALL instruction in packet parser
//that calls GetNextPacket function

//HookCall(CALL_GET_NEXT_PACKET, (DWORD)&OnGetNextPacket, (LPDWORD)&TfGetNextPacket);

//http://tpforums.org/forum/thread-3915.html

