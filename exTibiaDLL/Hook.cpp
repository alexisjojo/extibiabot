#include "StdAfx.h"
#include "Hook.h"


Hook::Hook(void)
{

}


Hook::~Hook(void)
{

}


DWORD Hook::HookCall(DWORD dwAddress, DWORD dwFunction)
{   

	DWORD dwOldProtect, dwNewProtect, dwOldCall, dwNewCall;
	BYTE callByte[5] = {0xE8, 0x00, 0x00, 0x00, 0x00};
	dwNewCall = dwFunction - dwAddress - 5;
	memcpy(&callByte[1], &dwNewCall, 4);

	VirtualProtect((LPVOID)(dwAddress), 5, PAGE_READWRITE, &dwOldProtect);
	memcpy(&dwOldCall, (LPVOID)(dwAddress+1), 4);
	memcpy((LPVOID)(dwAddress), &callByte, 5);
	VirtualProtect((LPVOID)(dwAddress), 5, dwOldProtect, &dwNewProtect);
	return dwOldCall;
}

DWORD Hook::HookCall(DWORD dwCallAddress, DWORD dwNewAddress, LPDWORD pOldAddress)
{
	DWORD dwOldProtect, dwNewProtect, dwOldCall, dwNewCall;
	BYTE call[5] = {0xE8, 0x00, 0x00, 0x00, 0x00};
	dwNewCall = dwNewAddress - dwCallAddress - 5;
	memcpy(&call[1], &dwNewCall, 4);
 
	VirtualProtectEx(GetCurrentProcess(), (LPVOID)(dwCallAddress), 5, PAGE_READWRITE, &dwOldProtect);
	memcpy(&dwOldCall, (LPVOID)(dwCallAddress+1), 4);
	if(pOldAddress)
	{
		memcpy(&dwOldCall, (LPVOID)(dwCallAddress+1), 4);
		*pOldAddress = dwCallAddress + dwOldCall + 5;
	}
	memcpy((LPVOID)(dwCallAddress), &call, 5);
	VirtualProtectEx(GetCurrentProcess(), (LPVOID)(dwCallAddress), 5, dwOldProtect, &dwNewProtect);
	return dwOldCall;
}

void Hook::UnhookCall(DWORD dwAddress, DWORD dwOldCall)
{
	DWORD dwOldProtect, dwNewProtect;
	BYTE callByte[5] = {0xE8, 0x00, 0x00, 0x00, 0x00};

	memcpy(&callByte[1], &dwOldCall, 4);

	VirtualProtect((LPVOID)(dwAddress), 5, PAGE_READWRITE, &dwOldProtect);
	memcpy((LPVOID)(dwAddress), &callByte, 5);
	VirtualProtect((LPVOID)(dwAddress), 5, dwOldProtect, &dwNewProtect);
}