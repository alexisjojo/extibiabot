namespace Consts 
{
		extern DWORD XTeaKey;
        extern DWORD RecvPointer;
        extern DWORD SendPointer;
		extern DWORD ParserFunc;
		extern DWORD GetNextPacketCall;
		extern DWORD RecvStream;
}

enum PipeConstantType : BYTE
{
		XTeaKey = 0x01,
        RecvPointer = 0x02,
        SendPointer = 0x03,
		ParserFunc = 0x04,
		GetNextPacketCall = 0x05,
		RecvStream = 0x06
};