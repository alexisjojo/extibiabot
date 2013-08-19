#pragma once
class Hook
{
public:
	Hook(void);
	~Hook(void);

	DWORD Hook::HookCall(DWORD dwAddress, DWORD dwFunction);
	void Hook::UnhookCall(DWORD dwAddress, DWORD dwOldCall);
	DWORD Hook::HookCall(DWORD dwCallAddress, DWORD dwNewAddress, LPDWORD pOldAddress);
};

