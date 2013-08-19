#include "StdAfx.h"
#include "Constants.h"

Constants::Constants(void)
{
}


Constants::~Constants(void)
{
}

#include "stdafx.h"
#include <string>
#include <windows.h>
#include "Constants.h"

namespace Consts 
{
	DWORD PrintText = 0xce800;
	DWORD DrawItem = 0xcbb90;

	DWORD ptrPrintName = 0x10e7f1;
	DWORD ptrPrintFPS = 0x65ae6;
	DWORD ptrShowFPS = 0x59999B;
	DWORD ptrFuncParser =  0x68BD0;
	DWORD ptrCallGetNextPacket = 0x68C17;
	DWORD ptrAddRecStream = 0x5DA5F4;
}

CRITICAL_SECTION NormalTextCriticalSection;
CRITICAL_SECTION CreatureTextCriticalSection;
CRITICAL_SECTION DrawSkinCriticalSection;
CRITICAL_SECTION DrawItemCriticalSection;