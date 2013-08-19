#include "StdAfx.h"
#include "PipeServer.h"
#include <cstdlib>
#include <stdio.h>
#include <wchar.h>
#include "Constants.h"
#include "packet.h"
#include "PrintHook.h"
#include "dllmain.h"

#define BUFSIZE 1024
BYTE Buffer[BUFSIZE];

PipeServer::PipeServer(void)
{
	stopPIPE = false;
	fConnected = false;
	hPipe = INVALID_HANDLE_VALUE;
	hThread = NULL;
	int result = swprintf_s(pipeName, sizeof(pipeName)/sizeof(wchar_t), L"\\\\.\\pipe\\exTibiaC%d", GetCurrentProcessId());
	
	if (result == -1)
	{
		MessageBox(NULL, L"An error has occurred attempting to create PIPE server. \nError code: Wrong server name.", L"exTibia", MB_OK);
	}	
}

PipeServer::~PipeServer(void)
{

}

void PipeServer::PipeKillThread()
{
	stopPIPE = 1;
	fConnected = 0;
	TerminateThread(hPipe, EXIT_SUCCESS);
	TerminateThread(pipeServerhThread, EXIT_SUCCESS);
}

int PipeServer::Initialize()
{
	while(!stopPIPE)
	{
		hPipe = CreateNamedPipe( 
          pipeName,					// pipe name 
          PIPE_ACCESS_DUPLEX,       // read/write access 
          PIPE_TYPE_MESSAGE |       // message type pipe 
          PIPE_READMODE_MESSAGE |   // message-read mode 
          PIPE_WAIT,                // blocking mode 
          PIPE_UNLIMITED_INSTANCES, // max. instances  
          BUFSIZE,                  // output buffer size 
          BUFSIZE,                  // input buffer size 
          0,                        // client time-out 
          NULL);                    // default security attribute 

      if (hPipe == INVALID_HANDLE_VALUE) 
      {
		  const DWORD last_error = GetLastError();

		  if (ERROR_NO_DATA == last_error)
            {
				MessageBox(NULL, L"An error has occurred attempting to create PIPE server. \nError code: ERROR_NO_DATA", L"exTibia", MB_OK);
            }
            else if (ERROR_PIPE_CONNECTED == last_error)
            {
				MessageBox(NULL, L"An error has occurred attempting to create PIPE server. \nError code: ERROR_PIPE_CONNECTED.", L"exTibia", MB_OK);
            }
            else if (ERROR_PIPE_LISTENING != last_error)
            {
				MessageBox(NULL, L"An error has occurred attempting to create PIPE server. \nError code: ERROR_PIPE_LISTENING", L"exTibia", MB_OK);
            }
		  
          return -1;
      }

	  fConnected = ConnectNamedPipe(hPipe, NULL) ? TRUE : (GetLastError() == ERROR_PIPE_CONNECTED); 

	  if (fConnected) 
      { 
         pipeServerhThread = CreateThread( 
            NULL,              // no security attribute 
            0,                 // default stack size 
			InstanceThread,    // thread proc
            (LPVOID) hPipe,    // thread parameter 
            0,                 // not suspended 
            &dwThreadId);      // returns thread ID 

         if (pipeServerhThread == NULL) 
         {
			MessageBox(NULL, L"An error has occurred attempting to create PIPE server. \nError code: hThread is NULL", L"exTibia", MB_OK);
            return -1;
         }
         else CloseHandle(pipeServerhThread); 
       } 
      else 
         CloseHandle(hPipe); 
	}

	return 0;
}

DWORD WINAPI PipeServer::InstanceThread(LPVOID lpvParam)
{ 
   DWORD cbBytesRead = 0, cbReplyBytes = 0, cbWritten = 0; 
   BOOL fSuccess = FALSE;
   HANDLE hPipe  = NULL;

   if (lpvParam == NULL)
   {
       return (DWORD)-1;
   }

   hPipe = (HANDLE) lpvParam; 

   while (hPipe != INVALID_HANDLE_VALUE) 
   { 
      fSuccess = ReadFile( 
         hPipe,        // handle to pipe 
         Buffer,    // buffer to receive data 
         sizeof(Buffer), // size of buffer 
         &cbBytesRead, // number of bytes read 
         NULL);        // not overlapped I/O 

      if (!fSuccess || cbBytesRead == 0) //
      {   
          if (GetLastError() == ERROR_BROKEN_PIPE)
          {
				FlushFileBuffers(hPipe); 
				DisconnectNamedPipe(hPipe); 
				CloseHandle(hPipe); 
				ProcessUninject();
			  return 1;
          }
          else
          {
				FlushFileBuffers(hPipe); 
				DisconnectNamedPipe(hPipe); 
				CloseHandle(hPipe); 
				ProcessUninject();
			  return 1;
          }
      }

	  ProcessMessage(Buffer);
  }

   FlushFileBuffers(hPipe); 
   DisconnectNamedPipe(hPipe); 
   CloseHandle(hPipe); 

   return 1;
}

VOID PipeServer::ProcessMessage( BYTE* Buffer )
{
	int position = 0;
	//WORD len = packet::ReadWord(Buffer, &position);
	BYTE PacketID = packet::ReadByte(Buffer, &position);
	switch (PacketID)
	{
		case 0x01:
			ProcessUninject();
			break;
		case 0x02:
			ProcessEnableHooks();
			break;
		case 0x03:
			ProcessDisableHooks();
			break;
		case 0x04:
			ProcessPrintText(Buffer, position);
			break;
		case 0x05:
			ProcessUnPrintText(Buffer, position);
			break;
		case 0x06:
			ProcessPrintItem(Buffer, position);
			break;
		case 0x07:
			ProcessUnPrintItem(Buffer, position);
			break;
	}
}

VOID PipeServer::ProcessUninject()
{
	UnloadSelf();
}

VOID PipeServer::ProcessEnableHooks()
{
	EnableHooks();
}

VOID PipeServer::ProcessDisableHooks()
{
	DisableHooks();
}

VOID PipeServer::ProcessPrintText( BYTE *Buffer, int position )
{	
	std::string TextName = packet::ReadString(Buffer, &position);
	int PosX = packet::ReadDWord(Buffer, &position);
	int PosY = packet::ReadDWord(Buffer, &position);
	int ColorRed = packet::ReadDWord(Buffer, &position);
	int ColorGreen = packet::ReadDWord(Buffer, &position);
	int ColorBlue = packet::ReadDWord(Buffer, &position);
    int Font = packet::ReadDWord(Buffer, &position);
	std::string Text = packet::ReadString(Buffer, &position);

    NormalText NewText;
    NewText.b = ColorBlue;
    NewText.g = ColorGreen;
    NewText.r = ColorRed;
    NewText.x = PosX;
    NewText.y = PosY;
    NewText.font = 1;

    NewText.TextName = new char[TextName.size() + 1];
    NewText.text = new char[Text.size() + 1];

	PrintHook::AddPrintText(PosX, PosY, ColorRed, ColorGreen, ColorBlue, 1, TextName, Text);
}

VOID PipeServer::ProcessUnPrintText( BYTE *Buffer, int position )
{
	std::string TextName = packet::ReadString(Buffer, &position);
	PrintHook::RemovePrintText(TextName);
}

VOID PipeServer::ProcessPrintItem( BYTE* buffer , int position )
{

}

VOID PipeServer::ProcessUnPrintItem( BYTE *Buffer, int position )
{

}