#include "stdafx.h"
#include <windows.h>

extern DWORD CurrentPID;
extern BOOL fUnicode;
extern HWND hwndTibia;
extern WNDPROC oldWndProc;

void UnloadSelf();
void EnableHooks();
void DisableHooks();

DWORD dwGetModuleBaseAddress(DWORD dwProcessIdentifier, TCHAR *lpszModuleName);
void CALLBACK HudCleanerProc(HWND hWnd, UINT nMsg, UINT nIDEvent, DWORD dwTime);