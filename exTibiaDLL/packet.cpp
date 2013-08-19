#include "stdafx.h"
#include <windows.h>
#include <string>
#include "packet.h"

packet::packet(){
        m_Packet = new BYTE[MAX_PACKETSIZE];
        o_Packet = 0;
        m_PacketSize = 0;
        m_CurrentPos = 0;
}

packet::packet(int PacketSize){
        m_Packet = new BYTE[PacketSize];
        o_Packet = 0;
        m_PacketSize = 0;
        m_CurrentPos = 0;
}

packet::packet(BYTE* Packet, int PacketSize){
        m_Packet = Packet;
        m_PacketSize = PacketSize;
        m_CurrentPos = 0;
}

packet::~packet(){
        delete [] m_Packet;
        if (o_Packet != 0)
                delete [] o_Packet;
}

void packet::AddByte(BYTE value){
        m_Packet[m_CurrentPos] = value;
        m_CurrentPos++;
        m_PacketSize++;
}

void packet::AddShort(short value){
        *(short*)(m_Packet + m_CurrentPos) = value;
        m_CurrentPos += 2;
        m_PacketSize += 2;
}

void packet::AddWord(WORD value){
        *(WORD*)(m_Packet + m_CurrentPos) = value;
        m_CurrentPos += 2;
        m_PacketSize += 2;
}

void packet::AddDWord(DWORD value){
        *(DWORD*)(m_Packet + m_CurrentPos) = value;
        m_CurrentPos += 4;
        m_PacketSize += 4;
}

void packet::AddString(std::string value){
        AddWord(value.length()); //Add string length to the packet
        strcpy((char*)(m_Packet + m_CurrentPos), value.c_str());
        m_CurrentPos += value.length();
        m_PacketSize += value.length();
}

BYTE* packet::GetPacket(){
        o_Packet = new BYTE[m_PacketSize+2];
        memcpy(o_Packet + 2, m_Packet, m_PacketSize); //Copy packet to outgoing packet
        *(WORD*)o_Packet = m_PacketSize; //Copy packet size to the start
        return o_Packet;
}

int packet::GetSize(){
        return m_PacketSize + 2;
}


BYTE packet::ReadByte(BYTE *buffer, int *offset){
        return buffer[(*offset)++];
}

WORD packet::ReadWord(BYTE *buffer, int *offset){
        WORD result;
        result = buffer[*offset]+(buffer[*offset+1]<<8);
        (*offset)+=2;
        return result;
}

short packet::ReadShort(BYTE *buffer, int *offset){
        short result;
        result = buffer[*offset]+(buffer[*offset+1]<<8);
        (*offset)+=2;
        return result;
}

DWORD packet::ReadDWord(BYTE *buffer, int *offset){
        DWORD result;
        result = buffer[*offset]+(buffer[*offset+1]<<8)+(buffer[*offset+2]<<0x10)+(buffer[*offset+3]<<0x18);
        (*offset)+=4;
        return result;
}

double packet::ReadDouble(BYTE *buffer, int *offset){
        BYTE a[8];
        double *result;
        int i;
        for (i=0;i<sizeof(double);i++)
                a[i] = buffer[*offset+7-i];
        result = (double*)&a[0];
        (*offset)+=8;
        return *result;
}

std::string packet::ReadString(BYTE *buffer, int *offset){
        WORD length = ReadWord(buffer, offset);
        std::string result = "";
        int i;
        for (i=0;i<length;i++)
                result += *(buffer+(*offset)++);
        return result;
}

Packet::Packet(void)
{
}


Packet::~Packet(void)
{
}
