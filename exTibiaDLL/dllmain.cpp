// dllmain.cpp : Defines the entry point for the DLL application.

#include <stdio.h>
#include <windows.h>
#include <sddl.h>
#include <TlHelp32.h> 
#include <iostream> 
#include <TlHelp32.h> 
#include "tchar.h"
#include "stdafx.h" 
#include "dllmain.h"
#include "ParserHook.h"
#include "PipeServer.h"
#include "PrintHook.h"
#include <time.h>
#include "Constants.h"
#include "Hook.h"



HINSTANCE hMod;
DWORD tibiaBaseAddress;
DWORD CurrentPID;
BOOL fUnicode;
HWND hwndTibia=0;
WNDPROC oldWndProc;
BOOL HooksEnabled;
DWORD oldPacketHook;

HANDLE pipeThread;
HANDLE parserThread;

PipeServer* server = new PipeServer();

void InitializeParserHook()
{
	ParserHook *pareserHook = new ParserHook(dwGetModuleBaseAddress(GetCurrentProcessId(), _T("Tibia.exe")));
}

void InitializePipeServer()
{
	PipeServer* server = new PipeServer();
	server->Initialize();
}

void EnableHooks()
{
	Hook* hook = new Hook();
	HooksEnabled = TRUE;

	Consts::ptrPrintName += tibiaBaseAddress;
	Consts::ptrPrintFPS += tibiaBaseAddress;
	Consts::ptrShowFPS += tibiaBaseAddress;
	Consts::ptrFuncParser += tibiaBaseAddress;
	Consts::ptrCallGetNextPacket += tibiaBaseAddress;
	Consts::ptrAddRecStream += tibiaBaseAddress;

	PrintText = (_PrintText*)(Consts::PrintText + tibiaBaseAddress);
	DrawItem = (_DrawItem*)(Consts::DrawItem + tibiaBaseAddress);

	TfParser = (TF_PARSER*)(Consts::ptrFuncParser);
	pRecvStream = (TPacketStream*)(Consts::ptrAddRecStream);

	OldPrintName = hook->HookCall(Consts::ptrPrintName, (DWORD)&MyPrintName);
	OldPrintFPS =	hook->HookCall(Consts::ptrPrintFPS, (DWORD)&MyPrintFps);	

	oldPacketHook = hook->HookCall(Consts::ptrCallGetNextPacket, (DWORD)&OnGetNextPacket, (LPDWORD)&TfGetNextPacket);

	oldWndProc = (WNDPROC) ((fUnicode) ? SetWindowLongPtrW(hwndTibia, GWLP_WNDPROC, (LONG_PTR)SubClassProc) : SetWindowLongPtrA(hwndTibia, GWLP_WNDPROC, (LONG_PTR)SubClassProc));
}

void DisableHooks()
{
	Hook* hook = new Hook();

	HooksEnabled = FALSE;

	if (OldPrintName)
		hook->UnhookCall(Consts::ptrPrintName, OldPrintName);

	if (OldPrintFPS)
		hook->UnhookCall(Consts::ptrPrintFPS, OldPrintFPS);

	if (oldPacketHook)
		hook->UnhookCall(Consts::ptrCallGetNextPacket, oldPacketHook);

	fUnicode ? SetWindowLongPtrW(hwndTibia, GWLP_WNDPROC, (LONG_PTR)oldWndProc) :SetWindowLongPtrA(hwndTibia, GWLP_WNDPROC, (LONG_PTR)oldWndProc);
}

void __declspec(noreturn) UninjectSelf()
{       
        DWORD ExitCode=0;
        __asm
        {
                push hMod
                push ExitCode
                jmp dword ptr [FreeLibraryAndExitThread] 
        }
}

void StartUninjectSelf()
{
        try
        {
                CreateThread(NULL, NULL, (LPTHREAD_START_ROUTINE)UninjectSelf, hMod, NULL, NULL);
        }
        catch (...)
        {

        }
}

void UnloadSelf()
{
	if(HooksEnabled) 
	{
		PrintHook::ClearPrintText();
		PrintHook::ClearPrintItem();
		DisableHooks();
	}

	DeleteCriticalSection(&NormalTextCriticalSection);
	DeleteCriticalSection(&CreatureTextCriticalSection);
	DeleteCriticalSection(&DrawItemCriticalSection);
	DeleteCriticalSection(&DrawSkinCriticalSection);

	StartUninjectSelf();
}

DWORD dwGetModuleBaseAddress(DWORD dwProcessIdentifier, TCHAR *lpszModuleName) 
{ 
   HANDLE hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPMODULE, dwProcessIdentifier); 
   DWORD dwModuleBaseAddress = 0; 
   if(hSnapshot != INVALID_HANDLE_VALUE) 
   { 
      MODULEENTRY32 ModuleEntry32 = {0}; 
      ModuleEntry32.dwSize = sizeof(MODULEENTRY32); 
      if(Module32First(hSnapshot, &ModuleEntry32)) 
      { 
         do 
         { 
            if(_tcscmp(ModuleEntry32.szModule, lpszModuleName) == 0) 
            { 
               dwModuleBaseAddress = (DWORD)ModuleEntry32.modBaseAddr; 
               break; 
            } 
         } 
         while(Module32Next(hSnapshot, &ModuleEntry32)); 
      } 
      CloseHandle(hSnapshot); 
   } 
   return dwModuleBaseAddress; 
} 

BOOL CALLBACK EnumWindowsProc(HWND hwnd,LPARAM lParam)
{
	DWORD PID ;
	DWORD threadID;
	threadID=GetWindowThreadProcessId(hwnd,&PID);
	if(PID==CurrentPID)
	{
		hwndTibia=hwnd;
	}
	return hwndTibia ?0:1;
}

BOOL WINAPI DllMain(HINSTANCE hModule, DWORD reason, LPVOID reserved)
{
    switch(reason)
    {
        case DLL_PROCESS_ATTACH:
			{
				hMod = hModule;
				CurrentPID = GetCurrentProcessId();
				tibiaBaseAddress = dwGetModuleBaseAddress(GetCurrentProcessId(), _T("Tibia.exe"));

				EnumWindows(EnumWindowsProc,0);
				fUnicode=IsWindowUnicode(hwndTibia);

				InitializeCriticalSection(&NormalTextCriticalSection);
				InitializeCriticalSection(&CreatureTextCriticalSection);
				InitializeCriticalSection(&DrawItemCriticalSection);
				InitializeCriticalSection(&DrawSkinCriticalSection);

				pipeThread = CreateThread(0,0,(LPTHREAD_START_ROUTINE)&InitializePipeServer,NULL,0,NULL); 
				parserThread = CreateThread(0,0,(LPTHREAD_START_ROUTINE)&InitializeParserHook,NULL,0,NULL); 
				break;
			}

        case DLL_PROCESS_DETACH:	

			server->PipeKillThread();

			BOOL suc2 = TerminateThread(pipeThread, EXIT_SUCCESS);
			BOOL suc3 = TerminateThread(parserThread, EXIT_SUCCESS);

        break;
    }

    return TRUE;
}

