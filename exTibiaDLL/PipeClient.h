#pragma once
class PipeClient
{
public:
	PipeClient(void);
	~PipeClient(void);
	int Initialize();
	void SendToServer(LPBYTE bytes, DWORD amount);
	void Disconnect();
	static void Init();
private:
	HANDLE hPipe;
	wchar_t pipeName[32];
	char buffer[4096];

};

