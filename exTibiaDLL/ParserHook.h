#include "PipeClient.h"

#pragma once
class ParserHook
{
public:
	ParserHook(DWORD baseAddress);
	~ParserHook(void);
	static void ParserHook::Print();
	static void ParsePacket(LPBYTE bytes, DWORD amount);

private:

};

struct TPacketStream;

int OnGetNextPacket();

typedef void TF_PARSER();
typedef int TF_GETNEXTPACKET();

extern TF_GETNEXTPACKET* TfGetNextPacket;

extern DWORD OldParsePacket;
extern TF_PARSER* TfParser;
extern TPacketStream* pRecvStream;