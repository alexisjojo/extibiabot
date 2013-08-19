#include "StdAfx.h"
#include "PipeClient.h"
#include "windows.h"
#include "stdio.h"
#include <cstdlib>
#include <stdio.h>
#include <wchar.h>


PipeClient::PipeClient(void)
{
	int result = swprintf_s(pipeName, sizeof(pipeName)/sizeof(wchar_t), L"\\\\.\\pipe\\exTibiaS%d", GetCurrentProcessId());
	if (result == -1)
	{
		MessageBox(NULL, L"An error has occurred attempting to create PIPE client. \nError code: Wrong client name.", L"exTibia", MB_OK);
	}
	
	hPipe = NULL;
	Initialize();
}

PipeClient::~PipeClient(void)
{

}

int PipeClient::Initialize()
{
	if (hPipe != NULL)
		return 0;

	if (WaitNamedPipe(pipeName, NMPWAIT_WAIT_FOREVER))
	{
		hPipe = CreateFile(pipeName, GENERIC_READ | GENERIC_WRITE, 0, NULL, OPEN_EXISTING, NULL, NULL);

		if ( hPipe == INVALID_HANDLE_VALUE )
		{
			DWORD MsgID = GetLastError();

			char *TextSize;
			FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS, 0, MsgID, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), (LPTSTR) &TextSize, 0, 0);
			MessageBox(NULL, (LPTSTR)TextSize, L"exTibia", MB_OK);
			
			if (GetLastError() == ERROR_PIPE_BUSY)
			{
				MessageBox(NULL, L"ERROR_PIPE_BUSY", L"exTibia", MB_OK);
			}
		}
		else
		{
			return 0;
		}
		
	}
	return 0;
}

void PipeClient::Disconnect()
{
	CloseHandle(hPipe);
}

void PipeClient::SendToServer(LPBYTE bytes, DWORD amount)
{
	DWORD BYTESWRITTEN = 0;

	BOOL successed = WriteFile(
		hPipe,
		bytes,
		amount,
		&BYTESWRITTEN,
		NULL);
}