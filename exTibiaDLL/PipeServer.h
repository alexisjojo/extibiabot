#include <windows.h>
#include <wchar.h>
#pragma once
class PipeServer
{
public:
	/* Constructors and Destructors */
	PipeServer(void);
	~PipeServer(void);
	void PipeKillThread();
	void PipeServerClose();
	/* Methods */
	int Initialize();	
	
	/* Static Functions */
	static DWORD WINAPI InstanceThread(LPVOID lpvParam);
	static VOID ProcessMessage(BYTE* buffer);
	static VOID PipeServer::ProcessUninject();
	static VOID PipeServer::ProcessEnableHooks();
	static VOID PipeServer::ProcessDisableHooks();
	static VOID PipeServer::ProcessPrintText( BYTE* buffer , int position );
	static VOID PipeServer::ProcessUnPrintText( BYTE *Buffer, int position );
	static VOID PipeServer::ProcessPrintItem( BYTE* buffer , int position );
	static VOID PipeServer::ProcessUnPrintItem( BYTE *Buffer, int position );	

private:
	HANDLE hPipe;
	BOOL stopPIPE;
	BOOL fConnected;
	DWORD currProcessID;
	DWORD dwThreadId;
	wchar_t pipeName[32];
	HANDLE hThread;
	HANDLE pipeServerhThread;
};

